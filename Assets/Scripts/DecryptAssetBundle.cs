using UnityEngine; 
using UnityEditor;
using System.IO;

using System.Collections;

public class DecryptAssetBundle {

	public class TempObject : MonoBehaviour {
		public Object[] objects;
	}

	static void InspectAssetBundle(string path) {
		var data = File.ReadAllBytes(path);
		var tempObj = new GameObject("Temp of Inspect Asset Bundle");
		var temp = tempObj.AddComponent<TempObject>();

		var assetBundle = AssetBundle.CreateFromMemoryImmediate(data);
		temp.objects = assetBundle.LoadAll();
		foreach (var obj in temp.objects) {
			Debug.Log("" + obj.name + " " + obj.GetInstanceID() + " " + obj.GetHashCode() + " " + obj.GetType());
		}
		//assetBundle.Unload(true);
	}

	[MenuItem("Assets/Inspect Asset Bundle")]
	static void MenuInspectAssetBundle(){
		var path = AssetDatabase.GetAssetPath(Selection.activeObject);
		InspectAssetBundle(path);
	}


	[MenuItem("Assets/Inspect Asset Bundle",true)]
	static bool ValidateInspectAssetBundle() {
		var path = AssetDatabase.GetAssetPath(Selection.activeObject);
		return path.EndsWith(".unity3d");
	}
}
