using UnityEditor;
using UnityEngine;
using UnityEngine.Internal;

public class AppSetting : ScriptableObject {
	[SceneName(true)]
	public string StartScene;

}

[InitializeOnLoad]
public class AppSettingInitializer : Editor
{
	static AppSettingInitializer() {
		setupExecutionOrder();
	}

	/// <summary>
	/// 実行順の設定を自動で行う.
	/// See: http://gamedevrant.blogspot.jp/2013/07/unity3d-change-scripts-execution-order.html
	/// </summary>
	static void setupExecutionOrder(){
        // Get the name of the script we want to change it's execution order
		string scriptName = "AppManager";
 
        // Iterate through all scripts (Might be a better way to do this?)
        foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts())
        {
            // If found our script
            if (monoScript.name == scriptName)
            {
                // And it's not at the execution time we want already
                // (Without this we will get stuck in an infinite loop)
                if (MonoImporter.GetExecutionOrder(monoScript) != -500)
                {
                    MonoImporter.SetExecutionOrder(monoScript, -500);
                }
				return;
            }
        }
	}
}

/*
[CustomEditor(typeof(AppSetting))]
public class AppSettingEditor: EditorWindow {
	public void OnGUI() {

	}
}
*/