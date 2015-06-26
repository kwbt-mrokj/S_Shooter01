using UnityEngine;
using System.Collections;

// 必須コンポーネントを明示して、該当コンポーネントが削除されることを防止する
[RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour {
	// 弾丸プレハブ
	public GameObject bullet;
	// 弾丸発射座標
	public Transform firePos;
	// 弾丸発射音
	public AudioClip fireSfx;
	// MuzzleFlashのMeshRendererコンポーネントの設定パラメータ
	public MeshRenderer _renderer;

	// ゲームマネージャにアクセスするための変数
	private GameMgr _gameMgr;

	void Start(){
		// 最初にはMuzzleFlash　MeshRendererを無効にする
		_renderer.enabled = false;

		// ゲームマネージャを探す
		_gameMgr = GameObject.Find ("GameManager").GetComponent<GameMgr> ();
	}

	// Update is called once per frame
	void Update () {
		// レイを視覚的に表示するために使用
		Debug.DrawRay (firePos.position, firePos.forward * 10.0f, Color.green);

		// マウスを左クリックしたときにFire関数を呼び出す
		if (Input.GetMouseButtonDown (0)) {
			Fire();

			// レイに当たったゲームオブジェクトの情報を受け取る変数
			RaycastHit hit;

			// Raycast関数でレイを発射してそれに当たったゲームオブジェクトがあるときにはTrueを返す
			if(Physics.Raycast(firePos.position,firePos.forward, out hit, 10.0f))
			{
				// レイに当たったゲームオブジェクトのタグを比較し、それがモンスターであるかをチェック
				if(hit.collider.tag == "MONSTER")
				{
					// SendMessageを用いて渡したい引数を配列に入れる
					object[] _params = new object[2];
					_params[0] = hit.point;	// レイに当たった正確な位置(Vector3)
					_params[1] = 20;		// モンスターに与えるダメージ値

					// モンスターにダメージを与える関数を呼び出す
					hit.collider.gameObject.SendMessage("OnDamage"
					                                    , _params
					                                    , SendMessageOptions.DontRequireReceiver);
				}

				// レイに当たったゲームオブジェクトがBarrelであるかを確認
				if(hit.collider.tag == "BARREL")
				{
					// ドラム缶に当たったレイの入射角を計算するために発射原点とレイが当たったポイントを渡す
					object[] _params = new object[2];
					_params[0] = firePos.position;
					_params[1] = hit.point;
					hit.collider.gameObject.SendMessage ("OnDamage"
					                                     ,_params
					                                     ,SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	void Fire(){
		// 並列処理のためのコルーチン関数を呼び出す
		StartCoroutine (this.ShowMuzzleFlash ());

		// レイキャスト方式に変更したので、弾丸を作成するルーチンはコメントauto
		//StartCoroutine (this.CreateBullet ());

		StartCoroutine (this.PlaySfx (fireSfx));
	}

	// MuzzleFlashの有効化/無効化を短時間の間に繰り返す
	IEnumerator ShowMuzzleFlash(){
		_renderer.enabled = true;

		// 不規則な間隔で遅らせた後、MeshRendererを無効にする
		yield return new WaitForSeconds (Random.Range (0.01f, 0.2f));
		_renderer.enabled = false;
	}

	// コルーチン関数
	IEnumerator CreateBullet(){
		// Bulletプレハブを動的に作成
		Instantiate (bullet, firePos.position, firePos.rotation);
		yield return null;
	}

	// 弾丸発射音をコルーチンで作成
	IEnumerator PlaySfx(AudioClip _clip){
		/*	既存のサウンド発生関数はコメントアウト
		GetComponent<AudioSource>().PlayOneShot (_clip, 0.9f);
		yield return null;*/

		// publicのサウンド関数を呼び出す
		_gameMgr.PlaySfx (firePos.position, _clip);
		yield return null;
	}
}
