using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour {

	public Transform target;			// 追跡するターゲットゲームオブジェクトのTransform変数
	public float dist = 10.0f;			// カメラとの一定距離
	public float height = 5.0f;			// カメラの高さを設定
	public float dampRotate = 5.0f;		// スムーズな回転のために変数

	private Transform tr;				// カメラ自身のTransform変数

	// Use this for initialization
	void Start () {
		// カメラ自身のTransfromコンポーネントをtrに割り当てる
		tr = GetComponent<Transform> ();

		// Update関数を呼び出した後に1回ずつ呼び出される関数であるLateUpdateを使用
		// 追跡したいターゲットの移動が終了した後でカメラが追跡するためにLateUpdate使用
	}
	
	// Update is called once per frame
	void Update () {
		// カメラのY軸をターゲットのY軸での回転角度でスムーズに回転
		float currYAngle = Mathf.LerpAngle ( tr.eulerAngles.y
		                                   , target.eulerAngles.y
		                                   , dampRotate * Time.deltaTime);

		// データをQuaternion型に変換
		Quaternion rot = Quaternion.Euler (0, currYAngle, 0);

		// カメラの位置をターゲットが回転した角度だけ回転した後に
		// distの分後ろに配置してheightの分だけ上げる
		tr.position = target.position - (rot * Vector3.forward * dist) + (Vector3.up * height);

		// カメラの視線の先をゲームオブジェクトに設定
		tr.LookAt (target);
	}
}
