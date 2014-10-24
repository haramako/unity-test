using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class DeferedPostprocessorSetting : ScriptableObject {
	public bool LogEnabled = false;
	public MonoScript[] Processors = new MonoScript[]{};
}

