using UnityEngine;
using System.Collections;

public class LaserBeam : MonoBehaviour {
	private Transform tr;
	private LineRenderer _line;
	
	// Use this for initialization
	void Start () {
		tr = GetComponent<Transform> ();
		_line = GetComponent<LineRenderer> ();

		// LineRendererを無効にしてから開始
		_line.enabled = false;
		// LineRendererの始点と終点の幅を設定
		_line.SetWidth (0.3f, 0.01f);
	}
	
	// Update is called once per frame
	void Update () {
		// レイをあらかじめ作成
		Ray ray = new Ray(tr.position, tr.forward);

		// レイが目に見えるように設定する
		Debug.DrawRay (ray.origin, ray.direction * 100, Color.blue);

		// Biimに衝突したゲームオブジェクトの情報を受け取る変数
		RaycastHit hit;

		if (Input.GetMouseButtonDown (0)) 
		{
			// Line Rendererの最初の点の位置を設定する
			_line.SetPosition(0, ray.origin);
			// ある物体にレイが当たったときの位置をLineRendererの終点に設定
			if(Physics.Raycast (ray, out hit,100.0f)){
				_line.SetPosition(1,hit.point);
			}else{
				_line.SetPosition(1,ray.GetPoint(100.0f));
			}

			// レイを表示するコルーチン関数を呼び出す
			StartCoroutine(this.ShowLaserBeam());
		}
	}

	IEnumerator ShowLaserBeam()
	{
		_line.enabled = true;
		yield return new WaitForSeconds(Random.Range (0.01f,0.2f));
		_line.enabled = false;
	}
}
