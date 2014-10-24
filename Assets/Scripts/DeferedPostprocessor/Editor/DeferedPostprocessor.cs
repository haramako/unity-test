using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

public class DeferedPostprocessor : AssetPostprocessor {

	public abstract class Processor {
		public Processor() { }
		public abstract List<string> GetTargets(string src);
		public abstract void Postprocess(string target);
	}

	public static DeferedPostprocessorSetting Setting { get; private set; }
	public static List<string> Tasks { get; private set; }

	static DeferedPostprocessor() {
		Setting = Resources.Load<ScriptableObject>("DeferedPostprocessorSetting") as DeferedPostprocessorSetting;
		if (Setting == null) {
			Setting = new DeferedPostprocessorSetting();
		}
		if (EditorPrefs.HasKey("DeferedPostprocessor.Tasks")) {
			Tasks = EditorPrefs.GetString("DeferedPostprocessor.Tasks", "").Split('\t').Where(s=>s!="").ToList();
		}else{
			Tasks = new List<string>();
		}
		log("Now Tasks has {0} elements [{1}]", Tasks.Count, string.Join(", ", Tasks.ToArray()));
	}

	static public void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFrom) {
		var processors = createProcessors();
		foreach( var asset in imported.Concat(moved).Concat(movedFrom).Concat(deleted)){
			//IEnumerable<string> targets = new List<string>();
			log("Asset imported {0}", asset);
			var targets = processors.SelectMany(processor => {
				try {
					var newTargets = processor.GetTargets(asset);
					log("Add targets by {0} : {1} => [{2}]", processor.GetType().Name, asset, string.Join(", ", newTargets.ToArray()));
					return newTargets;
				}catch(Exception){
					Debug.LogWarning(string.Format("Processing failed. Processor={0}", processor.GetType().Name));
					return new List<string>();
				}
			})
			.Distinct();
			log("Target collected {0} => [{1}]", asset, string.Join(", ", targets.ToArray()));
			Tasks = Tasks.Concat(targets).Distinct().ToList();
			EditorPrefs.SetString("DeferedPostprocessor.Tasks", string.Join("\t", Tasks.ToArray()));
			log("Now Tasks has {0} elements [{1}]", Tasks.Count, string.Join(",", Tasks.ToArray()));
		}
	}

	[MenuItem("Tools/Defered Postprocessor/Clear List")]
	public static void ClearList() {
		Tasks = new List<string>();
		EditorPrefs.SetString("DeferedPostprocessor.Tasks", string.Join("\t", Tasks.ToArray()));
		Debug.Log("List cleared");
	}

	[MenuItem("Tools/Defered Postprocessor/Show List")]
	public static void ShowList(){
		foreach (var t in Tasks) Debug.Log(t);
	}

	[MenuItem("Tools/Defered Postprocessor/Execute Postprocess")]
	public static void ExcecutePostprocess() {
		Debug.Log("Execute postprocess");
		var processors = createProcessors();
		foreach (var target in Tasks) {
			log("processing {0} ...", target);
			foreach (var processor in processors) {
				processor.Postprocess(target);
			}
		}
		ClearList();
	}

	static void log(string format, params object[] param) {
		if (Setting.LogEnabled) Debug.Log(string.Format(format, param));
	}

	static List<Processor> createProcessors() {
		return Setting.Processors.Select(s => {
			var type = Type.GetType(s.name);
			if (type == null) {
				Debug.LogWarning("Processor " + s.name + " not found");
				return null;
			}
			ConstructorInfo[] ctors = type.GetConstructors();
			if (ctors.Length <= 0) {
				Debug.LogWarning(string.Format("{0} has at least one accesible ctor", type.Name));
				return null;
			}
			
			var instance = Activator.CreateInstance(type);
			if (!(instance is Processor)) {
				Debug.LogWarning(string.Format("{0} must be a Processor", type.Name));
				return null;
			}

			return instance as Processor;
		})
		.Where( p=>p!=null)
		.ToList();
	}

	/// <summary>
	/// Pathのglob形式の文字列（ワイルドカード指定 '*', '?' を含む文字列)を正規表現に変換する
	/// 使用例：
	/// GlobToRegex("*.txt"); // => new Regex(@"([^/]*)\.txt")
	/// GlobToRegex("hoge??.txt"); // => new Regex(@"hoge..\.txt")
	/// </summary>
	/// <param name="wildcard">glob形式の文字列</param>
	/// <returns></returns>
    public static Regex GlobToRegex(string wildcard) {
        string pattern = string.Format("^{0}$", Regex.Escape(wildcard)
			.Replace(@"\*\*", @"(.*)")
			.Replace(@"\*", @"([^/]*)")
			.Replace(@"\?", "([^/].)"));
        return new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
    }
}
