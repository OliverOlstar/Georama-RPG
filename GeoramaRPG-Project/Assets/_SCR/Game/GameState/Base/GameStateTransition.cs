using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class GameStateTransition
{
	protected Core.TimedStepLogger m_Logger = new Core.TimedStepLogger();
	private GameState m_GameState = GameState.None;
	public GameState State => m_GameState;

	public GameStateTransition(GameState state)
	{
		m_GameState = state;
	}

	public void TransitionStarted(object stateChangeData)
	{
		OnTransitionStarted(stateChangeData);
	}

	public IEnumerator Unload(object stateChangeData, GameState previousState)
	{
		yield return OnUnload(stateChangeData, previousState);
	}

	public IEnumerator Load(object stateChangeData)
	{
		yield return OnLoad(stateChangeData);
	}

	public IEnumerator Finish(object stateChangeData)
	{
		yield return OnFinish(stateChangeData);
	}

	public virtual ThreadPriority ThreadPriority => ThreadPriority.Low;

	protected virtual IEnumerator OnUnload(object stateChangeData, GameState previousState)
	{
		// Coming from the state none or an in game state requires an unload.
		// Coming from any other state means we are in a chain of transitions and we should have already performed an unload once.
		if (previousState != GameState.None && !GameState.InGame.HasFlag(previousState))
		{
			if (GameState.InGame.HasFlag(State))
			{
				// When entering gameplay from the bootflow do a quick unload, this is mostly to get rid of extra load screens from memory
				Log("Unloading", "Only unload unused assets when coming from state " + previousState);
				yield return Resources.UnloadUnusedAssets();
				yield return null;
				Log("Unloaded unused assets");
				yield break;
			}
			else
			{
				Log("Unload skipped", "No need to unload when coming from state " + previousState);
				yield break;
			}
		}

		Log("Unloading");
		SceneManager.LoadScene("Empty");
		Log("Loaded empty scene");
		yield return null;
		GC.Collect();
		Log("Garbage Collected");
		yield return null;
		yield return Resources.UnloadUnusedAssets();
		Log("Unloaded unused assets");
		yield return null;
	}

	public IEnumerator AwaitAsyncSceneLoad(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool setActive = false)
	{
		Log($"Loading {sceneName} scene");
#if UNITY_EDITOR
		yield return UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsync(sceneName, loadSceneMode);

		if (setActive)
		{
			UnityEditor.SceneManagement.EditorSceneManager.SetActiveScene(UnityEditor.SceneManagement.EditorSceneManager.GetSceneByName(sceneName));
		}
#else
		yield return SceneManager.LoadSceneAsync(sceneName, loadSceneMode);

		if (setActive)
		{
			SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
		}
#endif

		Log($"Loading {sceneName} complete");
	}

	protected virtual void OnTransitionStarted(object stateChangeData) { }

	protected abstract IEnumerator OnLoad(object stateChangeData);

	protected abstract IEnumerator OnFinish(object stateChangeData);

	public void LogTransitionStarted() => m_Logger.OnStart("GameState", GetType().Name, Core.ColorConst.SeaFoam);

	public void LogTransitionCompleted() => m_Logger.OnComplete();

	public void Log(string step, string message) => m_Logger.Log(step, message);

	public void Log(string step) => m_Logger.Log(step);

	public void LogError(string step) => m_Logger.LogError(step);

}