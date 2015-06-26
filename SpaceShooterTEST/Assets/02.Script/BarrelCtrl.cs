using UnityEngine;
using System.Collections;

public class BarrelCtrl : MonoBehaviour {
	// 爆発エフェクトパーティクルの設定変数
	public GameObject expeffect;

	// ランダムに選択するテクスチャ配列
	public Texture[] _textures;

	private Transform tr;

	// 弾丸が当たった回数を累積する変数
	private int hitCount = 0;

	// Use this for initialization
	void Start () {
		tr = GetComponent<Transform> ();
  		int idx = Random.Range (0, _textures.Length);
 		GetComponentInChildren<MeshRenderer> ().material.mainTexture = _textures [idx];
	}
	
	// 衝突時に発生するコールバック関数
	void OnCollisionEnter(Collision coll){
		if (coll.collider.tag == "BULLET") {
			// 衝突した弾丸を削除
			Destroy (coll.gameObject);

			// 弾丸にあたった回数を増加させ、3回以上であれば爆発処理
			if(++hitCount >= 3){
				StartCoroutine (this.ExplosionBarrel());
			}
		}
	}

 // ドラム缶を爆発させるコルーチン関数
 IEnumerator ExplosionBarrel(){
  // 爆発エフェクトパーティクルの作成
  Instantiate (expeffect, tr.position, Quaternion.identity);

  // 指定した原点を中心に10.0f半径内に入っているColliderオブジェクトを抽出
  Collider[] colls = Physics.OverlapSphere (tr.position, 10.0f);

  // 抽出したColliderオブジェクトに爆発力を渡す
  foreach (Collider coll in colls) {
    if(coll.GetComponent<Rigidbody>() != null){
     coll.GetComponent<Rigidbody>().mass = 1.0f;
    coll.GetComponent<Rigidbody>().AddExplosionForce(800.0f,tr.position,10.0f,300.0f);
   }
  }

  // 5秒後にドラム缶を削除
  Destroy (gameObject, 5.0f);
  yield return null;
 }

	void OnDamage(object[] _params)
	{
		// 発射原点ベクトル
		Vector3 firePos = (Vector3)_params [0];
		// ドラム缶に当たったhitのベクトル
		Vector3 hitPos = (Vector3)_params [1];
		// 入社ベクトル(レイの角度) = 当たった座標 - 発射原点
		Vector3 incomeVector = hitPos - firePos;

		Rigidbody rb = this.GetComponent<Rigidbody> ();

		// 入社ベクトルを正規化(Normalized)ベクトルに変更
		incomeVector = incomeVector.normalized;

		// レイのhit座標に入射ベクトルの角度の力を生成
		rb.AddForceAtPosition (incomeVector * 1000f, hitPos);

		// 弾丸に当たった回数を増加させて、3回以上であれば爆発処理
		if (++hitCount >= 3) {
			StartCoroutine (this.ExplosionBarrel());
		}
	}
}
