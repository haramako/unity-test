using UnityEngine;
using System;

/// <summary>
/// Appの状態
/// </summary>
public enum AppState {
	/// <summary>初期化前</summary>
	BeforeInitialize = 0, 
	/// <summary>初期化中</summary>
	Initializing,
	/// <summary>初期化完了後</summary>
	Ready
}

namespace AppInner {
	public class DefaultLogger: App.ILogger {
		public void Log(params object[] message){
			Debug.Log(message);
		}
	}

}

public partial class App {

	public interface ILogger {
		void Log(params object[] message);
	}

	public static AppState State { get; private set; }

	static App() {}

	public static void SceneStarted() {
		switch (State) { 
		case AppState.BeforeInitialize:
			if (Application.loadedLevelName == "Scene01") {
				Initialize();
			} else {
				Application.LoadLevel("Scene01");
			}
			break;
		case AppState.Initializing:
		case AppState.Ready:
			break;
		}
	}

	/// <summary>
	/// 初期化処理
	/// </summary>
	public static void Initialize() {
		Debug.Log("start initializing");
		State = AppState.Initializing;
		DoInitialize();
	}

	public static void ChangeScene() {
	}

}
