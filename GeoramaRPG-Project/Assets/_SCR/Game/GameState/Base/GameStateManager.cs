using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Core.UI;
using UnityEngine;

public enum GameState
{
	None = 0,
	BootFlow = 1 << 0,
	MainMenu = 1 << 1,
	Overworld = 1 << 2,
	Georama = 1 << 3,
	Dungeon = 1 << 4,
	Sandbox = 1 << 5,
	InGame = Overworld | Georama | Dungeon | Sandbox,

	All = 0x7FFFFFFF
}

public static class GameStateEvents
{
	public delegate void OnStateChangeDelegate(GameState pState, GameState pPreviousState, object pContext);
	public delegate void OnAppPauseDelegate(bool pPaused);
	public delegate void OnAppQuitDelegate();

	public static OnStateChangeDelegate OnStateChangeStart;
	public static OnStateChangeDelegate OnStateChangeContinue;
	public static OnStateChangeDelegate OnStateChangeFinish;
	public static OnAppPauseDelegate OnAppPause;
	public static OnAppPauseDelegate OnAppFocus;
	public static OnAppQuitDelegate OnAppQuit;
}

public class GameStateManager : MonoBehaviour
{
	private static GameStateManager s_Singleton = null;
	public static GameStateManager Instance => s_Singleton;
	public static bool Exists => s_Singleton != null;

	private GameState m_State = GameState.None;
	public static GameState GameState => s_Singleton == null ? GameState.None : s_Singleton.m_State;
	public static bool IsInGame() => (GameState & GameState.InGame) != 0;
	public static bool IsPreGame() => GameState == GameState.None;
	public static bool IsGameState(GameState gamestate) => (GameState & gamestate) != 0;

	private GameState m_RequestState = GameState.None;
	private GameState m_PreviousState = GameState.None;

	private Coroutine m_StateChangeRoutine = null;

	private bool m_CanUpdateGameStateBehaviours = false;
	public static bool CanUpdateGameStateBehaviours() => s_Singleton != null && s_Singleton.m_CanUpdateGameStateBehaviours;

	private object m_RequestContext = null;
	private object m_StateContext = null;

	private GameStateTransition m_Transition = null;
	public static GameStateTransition CurrentTransition => s_Singleton != null ? s_Singleton.m_Transition : null;
	// Note: This needs to be true when OnTransitionStarted is called and false before OnTransitionFinished
	public static bool IsStateChangeComplete() => s_Singleton != null && s_Singleton.m_Transition == null;
	public static bool IsInternetRequired => s_Singleton != null && s_Singleton.m_Transition != null && s_Singleton.m_Transition.IsInternetRequired;

	private LoadingPercent m_Progress = new LoadingPercent();
	public static bool HasProgressToDisplay() => s_Singleton != null && s_Singleton.m_Progress.HasProgress();
	public static float GetUpdatedDisplayProgress() => s_Singleton == null ? 0.0f : s_Singleton.m_Progress.GetUpdatedDisplayProgress();

	private readonly Dictionary<GameState, GameStateTransition> m_Transitions = new Dictionary<GameState, GameStateTransition>()
	{
		//{ GameState.Login, new SuperLoginStateTransition(GameState.Login) },
		//{ GameState.Battle, new BattleStateTransition(GameState.Battle) },
		//{ GameState.Sandbox, new SandboxStateTransition(GameState.Sandbox) },
		//{ GameState.MainMenu, new MainMenuStateTransition(GameState.MainMenu) },
		//{ GameState.Profiling, new ProfilingStateTransition(GameState.Profiling) },
		//{ GameState.AccountLink, new AccountLinkStateTransition(GameState.AccountLink) },
	};

	public static GameStateManager Create()
	{
		if (s_Singleton == null)
		{
			s_Singleton = new GameObject("GameStateManager").AddComponent<GameStateManager>();
			DontDestroyOnLoad(s_Singleton.gameObject);
		}
		return s_Singleton;
	}

	private void OnDestroy()
	{
		s_Singleton = null;
	}

	private void OnApplicationPause(bool paused)
	{
		if (Application.isEditor && !Core.DebugOption.IsSet(GeoDebugOptions.BackgroundAppInEditor))
		{
			return;
		}
		GameStateEvents.OnAppPause?.Invoke(paused);
	}

    private void OnApplicationFocus(bool focus)
    {
		if (Application.isEditor && !Core.DebugOption.IsSet(GeoDebugOptions.BackgroundAppInEditor))
		{
			return;
		}
		GameStateEvents.OnAppFocus?.Invoke(focus);
	}

	private void OnApplicationQuit()
	{
		GameStateEvents.OnAppQuit?.Invoke();
	}

	public static void RequestState(GameState state, object context = null)
	{
		switch (state)
		{
			case GameState.BootFlow:
				// Need to force certain states in order to recover from stuck loading bugs
				// Specifically this can be used if we lose our session with the server during loading
				ForceStateInternal(state, context);
				break;
			default:
				RequestStateInternal(state, context);
				break;
		}
	}

	private static void RequestStateInternal(GameState state, object context)
	{
		Log(Core.Str.Build("Request state ", state.ToString()));
		GameStateManager gsm = Create();
		gsm.m_RequestState = state;
		gsm.m_RequestContext = context;
	}

	private static void ForceStateInternal(GameState state, object context)
	{
		GameStateManager gsm = Create();
		if (gsm.m_StateChangeRoutine != null)
		{
			Log(Core.Str.Build("Force state ", state.ToString(), " cancelling state transition ", s_Singleton.m_State.ToString()));
			gsm.StopCoroutine(s_Singleton.m_StateChangeRoutine);
			gsm.m_StateChangeRoutine = null;
		}
		else if (s_Singleton.m_RequestState != GameState.None)
		{
			Log(Core.Str.Build("Force state ", state.ToString(), " dequeueing ", s_Singleton.m_RequestState.ToString()));
		}
		else
		{
			Log(Core.Str.Build("Force state ", state.ToString()));
		}
		gsm.m_RequestState = state;
		gsm.m_RequestContext = context;
	}

	private void Update()
	{
		if (m_RequestState == GameState.None)
		{
			return;
		}
		if (m_StateChangeRoutine != null)
		{
			return;
		}
		Log(Core.Str.Build("Start transition ", m_RequestState.ToString(), " from ", m_State.ToString()));
		m_PreviousState = m_State;
		m_State = m_RequestState;
		m_RequestState = GameState.None;
		m_StateContext = m_RequestContext;
		m_RequestContext = null;
		m_StateChangeRoutine = StartCoroutine(DoTransition());
	}

	private IEnumerator DoTransition()
	{
		if (!m_Transitions.TryGetValue(m_State, out m_Transition))
		{
			Log(Core.Str.Build("Could not find transition for state ", m_State.ToString()));
			yield break;
		}
		if (GameState.InGame.HasFlag(m_PreviousState))
		{
			m_Progress.ResetPercent();
		}
		else
		{
			m_Progress.CachePercent();
		}
		if (!GameState.InGame.HasFlag(m_State))
		{
			m_Progress.ReservePercent(0.33f);
		}
		m_Progress.SetPercent(0.1f);

		PlayTransitionSplashScreen();
		

		m_Transition.TransitionStarted(m_StateContext);
		m_Transition.LogTransitionStarted();
		m_CanUpdateGameStateBehaviours = false;
		GameStateEvents.OnStateChangeStart?.Invoke(m_State, m_PreviousState, m_StateContext);

		Application.backgroundLoadingPriority = ThreadPriority.High;
		//if (MenuManager.Exists)
		//{
		//	MenuManager.Instance.ResetMenus();
		//}
		DestroyStateDirectors();
		Core.TimeScaleManager.EndAllTimeEvents(); // Make sure we don't get stuck with a weird time scale
		
		InterceptInputBehaviour.SetIgnoreAll(false);

		yield return m_Transition.Unload(m_StateContext, m_PreviousState);
		m_Transition.Log("Load bundles for state");
		m_Progress.SetPercent(0.2f);

		m_Transition.Log("Loading");
		m_Progress.SetPercent(0.3f);
		yield return m_Transition.Load(m_StateContext);

		m_Transition.Log("Initializing state Directors");
		m_Progress.SetPercent(0.5f);
		CreateStateDirectors();
		yield return WaitForLoadingDirectors();

		m_Transition.Log("Finishing");
		m_Progress.SetPercent(0.8f);
		yield return m_Transition.Finish(m_StateContext);

		m_Transition.Log("Collect garbage start");
		m_Progress.SetPercent(1.0f, !GameState.InGame.HasFlag(m_State)); // When we finish transitioning to in game force percent to 100
		m_CanUpdateGameStateBehaviours = true;
		System.GC.Collect(); // Sneak in a GC after transition is complete while we're waiting on theses behaviours, gives the new game state a clean slate
		yield return null; // This fame is for GameStateBehaviours to get an update pass before the loading screen drops
		m_Transition.Log("Collect garbage finished");

		m_Transition.LogTransitionCompleted();
		if (m_RequestState == GameState.None)
		{
			Application.backgroundLoadingPriority = m_Transition.ThreadPriority;
		}
		m_StateChangeRoutine = null;
		m_Transition = null; // Make sure IsStateChangeComplete is false before invoking OnTransitionFinished
		if (m_RequestState == GameState.None)
		{
			yield return new WaitForEndOfFrame();
			GameStateEvents.OnStateChangeFinish?.Invoke(m_State, m_PreviousState, m_StateContext);
		}
		else
		{
			GameStateEvents.OnStateChangeContinue?.Invoke(m_State, m_PreviousState, m_StateContext);
		}
	}

	private void PlayTransitionSplashScreen()
	{
		// TODO Add loading screen
	}

	private void DestroyStateDirectors()
	{
		List<IDirector> willDestroy = Core.ListPool<IDirector>.Request();
		Core.Director.GetMatching(willDestroy, ShouldDestroyDirector);
		foreach (IDirector director in willDestroy)
		{
			if (director is IPreDestroyDirector pre)
			{
				pre.OnPreDestroy();
			}
		}
		Core.ListPool<IDirector>.Return(willDestroy);
		Core.Director.DestroyMatching(ShouldDestroyDirector);
	}

	private void CreateStateDirectors()
	{
		List<IDirector> beforeCreation = Core.ListPool<IDirector>.Request();
		List<IDirector> afterCreation = Core.ListPool<IDirector>.Request();
		List<IPostCreateDirector> postCreateDirectors = Core.ListPool<IPostCreateDirector>.Request();
		Core.Director.GetAll(beforeCreation);
		Core.Director.GetOrCreateOfTypeWithPredicate(afterCreation, ShouldCreateDirector);
		foreach (IDirector director in afterCreation)
		{
			if (beforeCreation.Contains(director))
			{
				continue;
			}
			if (!(director is IPostCreateDirector postCreateDirector))
			{
				continue;
			}
			postCreateDirectors.Add(postCreateDirector);
		}
		Core.ListPool<IDirector>.Return(beforeCreation);
		Core.ListPool<IDirector>.Return(afterCreation);
		foreach (IPostCreateDirector director in postCreateDirectors)
		{
			director.OnPostCreate();
		}
		Core.ListPool<IPostCreateDirector>.Return(postCreateDirectors);
	}

	private bool ShouldDestroyDirector(IDirector director)
	{
		if (Core.Director.IsPersistent(director)) // Persistent directors should never be destroyed
		{
			return false;
		}
		// Destroy non-persistent directors that aren't allowed in this game state
		GameStateDirectorPersistAttribute attribute = 
			Attribute.GetCustomAttribute(director.GetType(), typeof(GameStateDirectorPersistAttribute), true) as GameStateDirectorPersistAttribute;
		return attribute == null || !attribute.IsValid(m_State);
	}

	private bool ShouldCreateDirector(Type directorType)
	{
		GameStateDirectorCreateAttribute attribute =
			Attribute.GetCustomAttribute(directorType, typeof(GameStateDirectorCreateAttribute), true) as GameStateDirectorCreateAttribute;
		return attribute != null && attribute.IsValid(m_State);
	}

	private IEnumerator WaitForLoadingDirectors()
	{
		List<ILoadingDirector> loadingDirectors = Core.ListPool<ILoadingDirector>.Request();
		Core.Director.GetAll(loadingDirectors);
		foreach (ILoadingDirector loadingDirector in loadingDirectors)
		{
			loadingDirector.StartLoading(m_State, m_StateContext);
		}
		int count = loadingDirectors.Count;
		bool finished = false;
		while (!finished)
		{
			yield return null;
			finished = true;
			for (int i = 0; i < count; i++)
			{
				ILoadingDirector director = loadingDirectors[i];
				if (director == null)
				{
					continue;
				}
				if (director.IsLoadingDone())
				{
					m_Transition.Log("LoadingDirector " + director.GetType().Name + " was finished");
					loadingDirectors[i] = null;
					continue;
				}
				finished = false;
			}
		}
		Core.ListPool<ILoadingDirector>.Return(loadingDirectors);
	}

	private static void Log(string s)
	{
		Debug.Log($"{Core.DebugUtil.ColorString(Core.ColorConst.SeaFoam, "[GameState]")} {s}");
	}
}

public class LoadingPercent
{
	public float LAGGY_PERCENT = 0.1f;

	private float m_TargetPercent = 0.0f;
	private float m_CachedPercent = 0.0f;

	private float m_CurrentPercent = 0.0f;
	private float m_CurrentChunk = 1.0f;

	private int m_LastUpdate = -1;

	public bool HasProgress()
	{
		return m_TargetPercent > Core.Util.EPSILON;
	}

	public float GetUpdatedDisplayProgress()
	{
		// Only update laggy percent when we're polled for progress and guard against updating multiple times per frame
		int frame = Time.frameCount;
		if (m_LastUpdate != frame)
		{
			m_LastUpdate = frame;
			m_CurrentPercent = Mathf.Lerp(m_CurrentPercent, m_TargetPercent, LAGGY_PERCENT);
		}
		return m_CurrentPercent;
	}

	public void SetPercent(float percent, bool laggy = true)
	{
		//Debug.LogError("LoadingPercent " + percent + " " + Time.time);
		m_TargetPercent = Mathf.Clamp(
			m_CachedPercent + m_CurrentChunk * percent,
			m_TargetPercent,
			1.0f);
		if (!laggy)
		{
			m_CurrentPercent = m_TargetPercent;
		}
	}

	public void CachePercent()
	{
		//Debug.LogError("LoadingPercent cache" + " " + Time.time);
		m_CachedPercent = m_TargetPercent;
		m_CurrentChunk = 1.0f - m_CachedPercent;
	}

	public void ReservePercent(float percent)
	{
		//Debug.LogError("LoadingPercent reserve " + percent + " " + Time.time);
		m_CurrentChunk = percent * (1.0f - m_CachedPercent);
	}

	public void ResetPercent()
	{
		m_TargetPercent = 0.0f;
		m_CachedPercent = 0.0f;
		m_CurrentPercent = 0.0f;
		m_CurrentChunk = 1.0f;
		m_LastUpdate = -1;
	}
}

interface IPostCreateDirector
{
	void OnPostCreate();
}

interface IPreDestroyDirector
{
	void OnPreDestroy();
}