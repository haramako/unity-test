using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Toydea {
	public class BuildUtil {
		public static bool BuildAssetBundle(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, string pathName, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform){

			return BuildPipeline.BuildAssetBundle(mainAsset, assets, pathName, assetBundleOptions);
		}
	}
}

public class TestProcessor: DeferedPostprocessor.Processor {
	static readonly string SrcDirectory = "Assets/BuildSource";
	static readonly string DestDirectory = "Assets/AssetBundles";
	static readonly Regex SrcMatcher = DeferedPostprocessor.GlobToRegex(SrcDirectory+"/**/*");
	static readonly Regex DestMatcher = DeferedPostprocessor.GlobToRegex(DestDirectory+"/*.unity3d");

	public TestProcessor() { }
	public override List<string> GetTargets(string src) {
		if (src.StartsWith(SrcDirectory)) {
			var path = src.Remove(0,SrcDirectory.Length+1).Split('/');
			if (path.Length >= 2) {
				return new List<string> { DestDirectory + "/" + path[0] + ".unity3d" };
			}
		}
		return new List<string>();
	}

	public override void Postprocess(string target) {
		var match = DestMatcher.Match(target);
		if (!match.Success) return;

		Debug.Log(match.Groups[1]);

		string destPath = DestDirectory + "/" + match.Groups[1] + ".unity3d";
		string srcPath = SrcDirectory + "/" + match.Groups[1];
		Selection.objects = new UnityEngine.Object[]{ AssetDatabase.LoadMainAssetAtPath (srcPath) };
		Object[] selection = Selection.GetFiltered (typeof(Object), SelectionMode.DeepAssets);

		try {
			BuildPipeline.PushAssetDependencies();

			BuildAssetBundleOptions bundleOption =
				BuildAssetBundleOptions.CollectDependencies |
				BuildAssetBundleOptions.CompleteAssets |
				BuildAssetBundleOptions.DeterministicAssetBundle;

			// build for android
			BuildPipeline.BuildAssetBundle(
				Selection.activeObject,
				selection,
				destPath,
				bundleOption,
				EditorUserBuildSettings.activeBuildTarget
			);
			Debug.Log(string.Format("Create {0} from {1} assets [{1}]", destPath, selection.Length, string.Join(",", selection.Select(x => x.name).ToArray())));
		} finally {
			BuildPipeline.PopAssetDependencies();
		}

	}

	[MenuItem("Tools/TestProcessor")]
	public static void Test() {
		var x = new TestProcessor();
		x.Postprocess("Assets/AssetBundles/Test01.unity3d");
	}
}

