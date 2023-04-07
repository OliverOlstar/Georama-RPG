using I2.Loc;
using OliverLoescher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
	private Core.TimedStepLogger m_Logger = new Core.TimedStepLogger();

	[SerializeField]
	private GameObject m_CheatMenu;
	[SerializeField]
	private List<GameObject> m_Managers;

	private IEnumerator Start()
	{
		m_Logger.OnStart("GameState", GetType().Name, Core.ColorConst.SeaFoam);

		// First Launch
		bool firstLaunch = !PlayerPrefs.HasKey("firstLaunch");
		if (firstLaunch)
		{
			PlayerPrefs.SetInt("firstLaunch", 1);
			PlayerPrefs.Save(); // Guarentee this is only sent once per install
		}

#if !RELEASE
		// Cheat Menu
		GameObject obj = Instantiate(m_CheatMenu);
		GameObject.DontDestroyOnLoad(obj);

		m_Logger.Log("Dev cheat menu start");
		if (GeoDebugOptions.OpenCheatMenuOnBoot.IsSet())
		{
			CheatMenu cheatMenu = obj.GetComponent<CheatMenu>();
			cheatMenu.ForceOpen();
			while (cheatMenu.IsOpen)
			{
				yield return null;
			}
		}
		m_Logger.Log("Dev cheat menu closed");
#endif

		// Splash Screen

		// Quality
		//m_Logger.Log($"TryInitializeQuality Start");
		//yield return QualityManager.Initialize();
		//m_Logger.Log($"TryInitializeQuality End");

		// Localization
		m_Logger.Log("Localization Setup start");
		I2.Loc.LocalizationManager.HighlightLocalizedTargets = LocUtil.IsDebugLoc;
		string savedLang = LocUtil.SavedLanguage;

		Debug.Log($"[GameStart] saved language {savedLang}");
		Debug.Log($"[GameStart] debug language {LocUtil.DebugLanguage}");
		Debug.Log($"[GameStart] system language {LocalizationManager.GetCurrentDeviceLanguage()}");

		I2.Loc.LocalizationManager.CurrentLanguage = string.IsNullOrEmpty(LocUtil.DebugLanguage) ?
			!string.IsNullOrEmpty(savedLang) ? savedLang : LocalizationManager.GetCurrentDeviceLanguage() : LocUtil.DebugLanguage;

		Debug.Log($"[GameStart] selected language {LocalizationManager.CurrentLanguage}");
		m_Logger.Log("Localization Setup end");

		// Managers
		m_Logger.Log("Managers start");
		for (int i = 0; i < m_Managers.Count; ++i)
		{
			if (m_Managers[i] == null)
			{
				continue;
			}
			GameObject obj2 = Instantiate(m_Managers[i]);
			GameObject.DontDestroyOnLoad(obj2);
		}
		m_Logger.Log("Managers end");

		// We made it!
		GameStateManager.RequestState(GameState.BootFlow);
	}
}