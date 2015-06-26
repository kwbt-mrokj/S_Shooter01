using UnityEngine;
using System.Collections;

public class WallCtrl : MonoBehaviour {
	// Soarksパーティクルプレハブを設定するパラメータ
	public GameObject sparkEffect;

	IEnumerator Start()
	{
		MeshRenderer meshR = this.GetComponent<MeshRenderer> ();
		meshR.materials [0] = Resources.Load ("Resources/Wall") as Material;
		sparkEffect = Resources.Load ("Resources/Sparks") as GameObject;
		yield return sparkEffect;
	}

	// 衝突が始まるときに発生するイベント
	void OnCollisionEnter(Collision coll)
	{
		// 衝突したゲームオブジェクトのタグの値の比較
		if (coll.collider.tag == "BULLET") 
		{
			// 衝突したゲームオブジェクトのタグ値の比較
			Vector3 firePos = coll.gameObject.GetComponent<BulletCtrl>().firePos;
			// 入射角の逆ベクトル = 発射原点 - 衝突点
			Vector3 relativePos = firePos - coll.transform.position;

			// Sparksパーティクルを動的に作成
			Object obj = Instantiate (sparkEffect,
			                          coll.transform.position,
			                          Quaternion.identity);
			// 2秒後にSparksパーティクルを削除
			Destroy (obj,2.0f);

			// 衝突したゲームオブジェクトを削除
			Destroy (coll.gameObject);
		}
	}
}
