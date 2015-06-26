using UnityEngine;
using System.Collections;

public class UIMgrOld : MonoBehaviour {

	// AssetBundleをダウンロードするアドレス
	string url = "file:///C:/UnityProject/SpaceShooter/Spaceshooter2.unity3d";
	// "http://www.Unity3dStudy.com/data/Spaceshooter.unity3d";
	// AssetBundleのバージョン
	int version = 17;
	// ver4...file版成功例
	// ver5...Assets入り
	// ver16...test
	// ver17...shooter2TEST

	// webにアクセスするための変数
	WWW www;

	IEnumerator Start()
	{
		// 指定されたurlアドレスにアクセスして、関連ファイルをダウンロードする
		www = WWW.LoadFromCacheOrDownload (url, version);

		yield return www;

		// エラーが発生したら、メッセージを出力
		if (!string.IsNullOrEmpty (www.error)) 
		{
			Debug.Log (www.error.ToString ());
		} else 
		{
			// ダウンロードしたAssetBundleをメモリにロード
			AssetBundle assetBundle = www.assetBundle;
		}
	}
	void OnGUI()
	{
		// AssetBundleをすべてダウンロードして取得し、GUIボタンを作成
		if(www.isDone && GUI.Button ( new Rect(20,50,100,30),"Start Game"))
		{
			LoadScene ();
		}
		// ダウンロードの進行状況を表示
		GUI.Label (new Rect (20, 20, 200, 30)
		           , "DownLoading..." + (www.progress * 100.0f).ToString () + "%");

	}
	void LoadScene()
	{
		Application.LoadLevel ("scLevel01");
		Application.LoadLevelAdditive ("scPlay");
	}
}
