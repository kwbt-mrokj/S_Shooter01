using UnityEngine;
using UnityEditor;
using System.Collections;

public class BuildMyScene : MonoBehaviour {
	// 登録したい大分類メニューと中分類メニューを宣言
	[MenuItem("Build/Build AssetBundle %#d")]
	// メニューをクリックすると実行される関数
	public static void BuildSceneToAssetBundle()
	{
		// AssetBundleにするシーンのパスと名前を配列に格納
		string[] sceneName = new string[]{	"Assets/01.Scene/scPlay.unity"
											,"Assets/01.Scene/scLevel01.unity"};
		// AssetBundleにする
		BuildPipeline.BuildStreamedSceneAssetBundle (sceneName
		                                             	,"Spaceshooter2.unity3d"
			                                             ,BuildTarget.StandaloneOSXIntel);
	}
}
	