using UnityEngine;
using System.Collections;

public class PlayerCtrl : MonoBehaviour {
	private float h = 0.0f;
	private float v = 0.0f;

	// 頻繁に使用するコンポーネントは必ず変数に代入してから使用する
	private Transform tr;
	// 移動速度変数
	public float moveSpeed = 10.0f;
	// 回転速度変数
	public float rotSpeed = 100.0f;

	// Playerの生命変数
	public int hp = 100;

	// ゲームマネージャにアクセスするための変数
	private GameMgr _gameMgr;

	// デリゲートおよびイベント宣言
	public delegate void PlayerDieHandler();
	public static event PlayerDieHandler OnPlayerDie;

	/*
	クラスをInspectorビューに表示するためには、System.Serializableというアトリビュート(Attribute)を
	明示する必要がある
	*/
	[System.Serializable]
	// アニメーションクリップを保存するクラス
	public class Anim{
		public AnimationClip idle;
		public AnimationClip runForward;
		public AnimationClip runBackward;
		public AnimationClip runRight;
		public AnimationClip runLeft;
	}

	// Inspectorビューに表示するアニメーションクラス変数
	public Anim anim;
	// 下位にある3DモデルのAnimationコンポーネントにアクセスするための変数
	public Animation _animation;

	// Use this for initialization
	void Start () {
		// スクリプトの初めにTransformコンポーネントを割り当てる
		tr = GetComponent<Transform> ();
		// 自分の下位にあるAnimationコンポーネントを探して変数に代入する
		_animation = GetComponentInChildren<Animation> ();

		// GameMgrスクリプトを割り当てる
		_gameMgr = GameObject.Find ("GameManager").GetComponent<GameMgr> ();

		// Animationコンポーネントのアニメーションクリップを指定して実行
		_animation.clip = anim.idle;
		_animation.Play ();
	}
	
	// Update is called once per frame
	void Update () {
		h = Input.GetAxis ("Horizontal");
		v = Input.GetAxis ("Vertical");

		Debug.Log ("H = " + h.ToString ());
		Debug.Log ("V = " + v.ToString ());

		// 前後左右の移動ベクトルを計算する
		Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

		/// Transformクラスに定義された移動関連Translateメソッド
		// Translate (移動方向*速度*変化値*Time.deltaTime,Space.Self);
		tr.Translate (moveDir * Time.deltaTime * moveSpeed, Space.Self);

		// Vector3.up軸を中心にrotSpeedの速度で回転させる
		tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));

		// キーボード入力値に基づいて、動作するアニメーションを実行
		if (v >= 0.1f) {
			// 前進アニメーション
			_animation.CrossFade (anim.runForward.name, 0.3f);
		} else if (v <= -0.1f) {
			// 後退アニメーション
			_animation.CrossFade (anim.runBackward.name, 0.3f);
		} else if (h >= 0.1f) {
			// 右に移動するアニメーション
			_animation.CrossFade (anim.runRight.name, 0.3f);
		} else if (h <= -0.1f) {
			// 左に移動するアニメーション
			_animation.CrossFade (anim.runLeft.name, 0.3f);
		} else {
			// 停止時のidleアニメーション
			_animation.CrossFade (anim.idle.name, 0.3f);
		}
	}

	// 衝突したColliderのIsTriggerオプションがチェックされた時に発生
	void OnTriggerEnter(Collider coll)
	{
		// 衝突したColliderがモンスターのPUNCHだったら、PlayerのHPを減少させる
		if (coll.gameObject.tag == "PUNCH") 
		{
			hp -= 10;
			Debug.Log("Player HP = " + hp.ToString());

			// Playerの命が0以下であれば、死亡処理
			if(hp <= 0)
			{
				//PlayerDie();

				// イベントを発生させる
				OnPlayerDie();
				// ゲームマネージャのisGameOver変数を変更してモンスターの出現を停止させる
				_gameMgr.isGameOver = true;
			}
		}
	}

	// Playerの死亡ルーチン
	void PlayerDie()
	{
		Debug.Log ("Player Die !!");

		// MONSTERタグがついているすべてのゲームオブジェクトを見つける
		GameObject[] monsters = GameObject.FindGameObjectsWithTag ("MONSTER");

		// すべてのモンスターのOnPlayerDie関数を順次呼び出す
		foreach (GameObject monster in monsters) 
		{
			monster.SendMessage ("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
		}
	}
}
