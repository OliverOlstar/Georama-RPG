
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameStateBehaviour : MonoBehaviour
{
	private bool m_GameStateTransitionFinished = false;
	private bool m_GameStateStarted = false;

	void Awake()
	{
		GameStateEvents.OnStateChangeStart += OnGameStateTransitionStarted;
	}

	void OnDestroy()
	{
		GameStateEvents.OnStateChangeStart -= OnGameStateTransitionStarted;
	}

	private void Update()
	{
		if (GameStateManager.CanUpdateGameStateBehaviours())
		{
			if (!m_GameStateTransitionFinished)
			{
				m_GameStateTransitionFinished = true;
				GameStateTransitionFinished();
			}
		}
		if (GameStateManager.IsStateChangeComplete())
		{
			return;
		}

		if (!m_GameStateStarted)
		{
			m_GameStateStarted = true;
			GameStateStart();
		}

		GameStateUpdate();
	}

	private void OnGameStateTransitionStarted(GameState pState, GameState pPreviousState, object pContext)
	{
		m_GameStateStarted = false;
		m_GameStateTransitionFinished = false;
	}

	protected virtual void GameStateTransitionFinished() { }
	protected virtual void GameStateStart() { }
	protected virtual void GameStateUpdate() { }
}
