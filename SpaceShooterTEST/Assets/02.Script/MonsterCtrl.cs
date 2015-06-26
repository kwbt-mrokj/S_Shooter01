using UnityEngine;
using System.Collections;

public class MonsterCtrl : MonoBehaviour {
	// モンスターのステート情報を列挙するEnumerable変数の宣言
	public enum MonsterState {idle, trace, attack, die};
	// モンスターの現在のステート情報を格納するEnum変数
	public MonsterState monsterState = MonsterState.idle;

	// 速度向上のためにさまざまなコンポーネントを変数に割り当てる
	private Transform monsterTr;
	private Transform playerTr;
	private NavMeshAgent nvAgent;
	private Animator _animator;
	private int hashGotHit = 0;
	private int hashFall = 0;
	private int hashDie = 0;

	// 追跡範囲距離
	public float traceDist = 10.0f;
	// 攻撃範囲距離
	public float attackDist = 2.0f;

	// モンスターは死亡したか
	public bool isDie = false;

	// 血痕エフェクトのプレハブ
	public GameObject bloodEffect;
	// 血痕デカールエフェクトのプレハブ
	public GameObject bloodDecal;
	// モンスターの生命変数
	private int hp = 100;


	// Use this for initialization
	IEnumerator Start () {
		// モンスターのTransformを割り当てる
		monsterTr = this.gameObject.GetComponent<Transform> ();
		// 追跡対象であるPlayerのTransformを割り当てる
		playerTr = GameObject.FindWithTag ("Player").GetComponent<Transform> ();
		// NavMeshAgentコンポーネントを割り当てる
		nvAgent = this.gameObject.GetComponent<NavMeshAgent> ();
		// Animatorコンポーネントの割り当て
		_animator = this.gameObject.GetComponent<Animator> ();


		yield return monsterTr;	yield return playerTr;
		yield return nvAgent;	yield return _animator;

		// 追跡対象の位置を設定したら、すぐに追跡を開始する
		nvAgent.destination = playerTr.position;

		// 一定の間隔でモンスターのステートをチェックするコルーチン巻数の実行
		StartCoroutine (this.CheckMonsterState ());

		// モンスターのステートに応じて動作するルーチンを実行するコルーチン関数の実行
		StartCoroutine (this.MonsterAction ());

		// Base Layer.gothitのハッシュ準備
		hashGotHit = Animator.StringToHash ("Base Layer.gothit");
		hashFall = Animator.StringToHash ("Base Layer.fall");
		hashDie = Animator.StringToHash ("Base Layer.die");
	}
	
	// Update is called once per frame
	void Update () {
		// ~StateInfo(x).nameHashは非推奨
		//if (_animator.GetCurrentAnimatorStateInfo (0).nameHash == hashGotHit)
		if(_animator.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash("Base Layer.gothit"))
		{
			_animator.SetBool("IsHit",false);
		}
//		if (_animator.GetCurrentAnimatorStateInfo (0).nameHash == hashFall) 
		if(_animator.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash("Base Layer.fall"))
		{	
			_animator.SetBool("IsPlayerDie",false);
		}
//		if (_animator.GetCurrentAnimatorStateInfo (0).nameHash == hashDie) 
		if (_animator.GetCurrentAnimatorStateInfo (0).fullPathHash == Animator.StringToHash ("Base Layer.die")) {
			_animator.SetBool("IsTrace",false);
			_animator.SetBool ("IsDie", false);
		}
	}
	
	void OnCollisionEnter(Collision coll)
	{
		if (coll.gameObject.tag == "BULLET") 
		{
			// 血痕エフェクトのコルーチン関数を呼び出す
			StartCoroutine (this.CreateBloodEffect(coll.transform.position));
			// 当たった弾丸のダメージを抽出して、モンスターhpを減少させる
			hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;
			if (hp <= 0)
			{
				MonsterDie();
			}
			// Bulletを削除
			Destroy (coll.gameObject);
			// IsHitをtrueに変更すると、AnyStateからgothitに遷移する
			_animator.SetBool("IsHit", true);
		}
	}

	// モンスターがレイに当たったときに呼び出される関数
	void OnDamage(object[] _params)
	{
		Debug.Log(string.Format("Hit ray {0} : {1}",_params[0],_params[1]));
		// 血痕エフェクトのコルーチン関数を呼び出す
		StartCoroutine(this.CreateBloodEffect((Vector3)_params[0]));

		// 当たった弾丸のダメージ
		hp -= (int)_params [1];
		if (hp <= 0) {
			MonsterDie();
		}

		// IsHitをtrueに変更すると、AnyStateからgothitに遷移する
		_animator.SetBool ("IsHit", true);
	}

	// モンスターの死亡時の処理ルーチン
	void MonsterDie()
	{
		// 死亡したモンスターのタグをUntaggedに変更
//		gameObject.tag = "Untagged";

		// すべてのコルーチンを停止
		StopAllCoroutines ();

		isDie = true;
		monsterState = MonsterState.die;
		nvAgent.Stop ();
		_animator.SetBool ("IsDie", true);

		// モンスターに追加されたColliderを無効にする
		gameObject.GetComponentInChildren<CapsuleCollider> ().enabled = false;

		foreach (Collider coll in gameObject.GetComponentsInChildren<SphereCollider>()) 
		{
			coll.enabled = false;
		}
	}

	IEnumerator CreateBloodEffect (Vector3 pos)
	{
		// 血痕エフェクトを作成
		GameObject _blood1 = (GameObject)Instantiate (bloodEffect, pos, Quaternion.identity);

		// デカール作成位置 - 床から少し上に上げた位置を計算
		Vector3 decalPos = monsterTr.position + (Vector3.up * 0.01f);
		// デカールの回転値をランダムに設定
		Quaternion decalRot = Quaternion.Euler (0, Random.Range (0, 360), 0);

		// デカールプレハブを作成
		GameObject _blood2 = (GameObject)Instantiate (bloodDecal, decalPos, decalRot);
		// デカールの大きさも不規則にスケールを調整
		float _scale = Random.Range (1.5f, 3.5f);
		_blood2.transform.localScale = new Vector3 (_scale, 1, _scale);

		Destroy (_blood1, 2.0f);

		yield return null;
	}

	// プレイヤーが死亡したときに実行される関数
	void OnPlayerDie()
	{
		// モンスターのステートをチェックするコルーチン関数をすべて停止させる
		StopAllCoroutines ();
		// 追跡を停止してアニメーションを実行
		nvAgent.destination = monsterTr.position;
		_animator.SetBool ("IsPlayerDie", true);
	}
	/*
	 * 一定の間隔でモンスターのステートをチェックしてmonsterState値を変更
	 */

	IEnumerator CheckMonsterState(){
		while (!isDie) {
			yield return new WaitForSeconds(0.2f);

			// モンスターとプレイヤーとの間の距離を測定
			float dist = Vector3.Distance(playerTr.position, monsterTr.position);
			if(dist <= attackDist)	// 攻撃範囲距離内に入っているかを確認
			{
				monsterState = MonsterState.attack;
			}
			else if (dist <= traceDist)	// 追跡範囲距離内に入っているかを確認
			{
				monsterState = MonsterState.trace;	// モンスターのステートを追跡モードに設定
			}
			else
			{
				monsterState = MonsterState.idle;	// モンスターのステートをidleモードに設定
			}
		}
	}

	/*
	 * モンスターのステート値に応じて、適切なアクションを実行する関数
	 */
	IEnumerator MonsterAction()
	{
		while (!isDie) 
		{
			switch(monsterState){
			// idleステート
			case MonsterState.idle:
				// 追跡停止
			//	nvAgent.destination = monsterTr.position;
				nvAgent.Stop();
				// AnimatorのIsAttack変数値をfalseに設定する
				_animator.SetBool("IsAttack",false);
				// AnimatorのIsTrace変数値をfalseに設定
				_animator.SetBool("IsTrace",false);
				break;
			
			// 追跡ステート
			case MonsterState.trace:
				// idle,attack時にnvAegnt.Stop()がされていると思われるので
				// nvAgent.Resume()で動くように指示する必要がある(
				// (ver5より前はResume()なしでも動いたらしい)
				nvAgent.Resume();
				// 追跡対象の位置を渡す
				nvAgent.destination = playerTr.position;

				// AnimatorのIsAttack変数値をfalseに設定する
				_animator.SetBool("IsAttack",false);
				// AnimatorのIsTrace変数値をtrueに設定
				_animator.SetBool("IsTrace",true);
				break;
			
			// 攻撃ステート
			case MonsterState.attack:
				// 追跡停止
//				nvAgent.destination = monsterTr.position;
				nvAgent.Stop ();
				// IsAttackをtrueに設定してにattackStateに遷移
				_animator.SetBool("IsAttack",true);
				break;
			}
			yield return null;
		}
	}

	void OnEnable()
	{
		PlayerCtrl.OnPlayerDie += this.OnPlayerDie;
	}

	void OnDisable()
	{
		PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
	}
}
