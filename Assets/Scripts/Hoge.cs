using UnityEngine;
using UnityEditor;
using System.Collections;

[ExecuteInEditMode]
public class Hoge : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	[MenuItem("Tools/Hoge")]
	public static void Hogee() {
		var asset = AssetBundle.CreateFromFile("Assets/AssetBundles/Test01.unity3d");
		Debug.Log(asset);
	}
}
