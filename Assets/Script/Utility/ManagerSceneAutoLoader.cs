//  ManagerSceneAutoLoader.cs
//  http://kan-kikuchi.hatenablog.com/entry/ManagerSceneAutoLoader
//
//  Created by kan.kikuchi on 2016.08.04.

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Awake前にManagerSceneを自動でロードするクラス
/// </summary>
public class ManagerSceneAutoLoader
{

	//ゲーム開始時(シーン読み込み前)に実行される
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void LoadManagerScene()
	{
		string managerSceneName = "ManagerScene";

		//ManagerSceneが有効でない時(まだ読み込んでいない時)だけ追加ロードするように
		if (!SceneManager.GetSceneByName(managerSceneName).IsValid())
		{
			SceneManager.LoadScene(managerSceneName, LoadSceneMode.Additive);
		}
	}

}