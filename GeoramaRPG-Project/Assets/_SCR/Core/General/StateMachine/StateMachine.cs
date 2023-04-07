using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace OliverLoescher 
{
	public class StateMachine : MonoBehaviour
	{
		[Header("StateMachine")]
		[SerializeField, DisableInPlayMode] private BaseState defaultState = null;
		[SerializeField, DisableInPlayMode, HideInEditorMode] private BaseState currState = null;
		[SerializeField, DisableInPlayMode, HideInEditorMode] private BaseState[] states = new BaseState[0];

		[SerializeField] private bool printDebugs = false;

		private void Start() 
		{
			states = GetComponentsInChildren<BaseState>();

			// Initizalize
			foreach (BaseState state in states)
				state.Init(this);

			// Enter first state
			SwitchState(defaultState);
		}

		private void FixedUpdate() 
		{
			foreach (BaseState state in states)
			{
				if (state != currState)
				{
					if (state.CanEnter())
					{
						SwitchState(state);
						Log(StateName(state) + " CanEnter() == true");
					}
				}
				else
				{
					if (state.CanExit())
					{
						ReturnToDefault();
						Log(StateName(state) + " CanExit() == true");
					}
				}
			}

			if (currState != null)
				currState.OnFixedUpdate();
		}

		private void Update() 
		{
			if (currState != null)
				currState.OnUpdate();
		}

		public void SwitchState(BaseState pState)
		{
			Log("SwitchState: from " + StateName(currState) + " - to " + StateName(pState));

			if (currState != null)
				currState.OnExit();

			currState = pState;
			
			if (currState != null)
				currState.OnEnter();
		}

		public void ReturnToDefault()
		{
			SwitchState(defaultState);
		}

		public bool IsState(BaseState pState)
		{
			return currState == pState;
		}
		public bool IsDefaultState()
		{
			return currState == defaultState;
		}

		private void Log(string pString)
		{
			if (printDebugs)
				Debug.Log("[StateMachine.cs] " + pString, this);
		}
		private string StateName(BaseState pState) => (pState == null ? "Null" : pState.ToString());
	}
}