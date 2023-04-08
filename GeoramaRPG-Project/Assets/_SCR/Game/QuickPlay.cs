#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class QuickPlay
{
	private const string SCENE_PATH = "Assets/Scenes/GameStart.unity";
	private const string HOME_SCENE_KEY = "HomeSceneKey";
	public const string BOOT_TO_SCENE_KEY = "QuickPlayScene";

	// Called on load thanks to the InitializeOnLoad attribute
	static QuickPlay()
	{
		// Called on ever play mode state change (starting and stoping the game)
		EditorApplication.playModeStateChanged += PlayModeStateChanged;
	}

	[MenuItem("Tools/Quick Play %`")]
	public static void Play()
	{
		if (EditorApplication.isPlaying)
		{
			return;
		}
		string homeScenePath = SceneManager.GetActiveScene().path;
		EditorSceneManager.SaveOpenScenes();
		EditorPrefs.SetString(HOME_SCENE_KEY, homeScenePath);
		EditorApplication.isPlaying = true;
	}

	[MenuItem("Tools/Quick Play Active Scene %&`")]
	public static void PlayActiveScene()
	{
		if (EditorApplication.isPlaying)
		{
			return;
		}
		string homeScenePath = SceneManager.GetActiveScene().path;
		EditorSceneManager.SaveOpenScenes();
		EditorPrefs.SetString(BOOT_TO_SCENE_KEY, homeScenePath);
		EditorPrefs.SetString(HOME_SCENE_KEY, homeScenePath);
		EditorApplication.isPlaying = true;
	}
	
	[MenuItem("Tools/Quick Play Sandbox %#`")]
	public static void PlaySandbox()
	{
		if (EditorApplication.isPlaying)
		{
			return;
		}
		string homeScenePath = SceneManager.GetActiveScene().path;
		EditorSceneManager.SaveOpenScenes();
		GeoDebugOptions.BootToState.SetString(GeoDebugOptions.BootTo.Sandbox);
		EditorPrefs.SetString(HOME_SCENE_KEY, homeScenePath);
		EditorApplication.isPlaying = true;
	}

	private static void PlayModeStateChanged(PlayModeStateChange change)
	{
		string homeScene = EditorPrefs.GetString(HOME_SCENE_KEY, null);
		if (string.IsNullOrEmpty(homeScene))
		{
			return;
		}
		switch (change)
		{
			case PlayModeStateChange.ExitingEditMode:
				// open GameStart
				EditorSceneManager.OpenScene(SCENE_PATH);
				break;
			case PlayModeStateChange.EnteredEditMode:
				// return to home screen
				EditorSceneManager.OpenScene(homeScene);
				EditorPrefs.SetString(BOOT_TO_SCENE_KEY, null);
				EditorPrefs.SetString(HOME_SCENE_KEY, null);
				break;
		}
	}

	public static bool TryGetBootToScene(out string sceneName)
	{
		sceneName = EditorPrefs.GetString(BOOT_TO_SCENE_KEY);
		return !string.IsNullOrEmpty(sceneName);
	}
}
#endif