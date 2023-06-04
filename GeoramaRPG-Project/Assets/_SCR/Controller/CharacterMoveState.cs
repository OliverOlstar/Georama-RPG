using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterMoveState : CharacterBehaviour
{
	[System.Flags]
    public enum State
	{
		Run = 1 << 1,
		Sprint = 1 << 2,
		Walk = 1 << 3,
		Strafe = 1 << 4,
		Crouch = 1 << 5,
		Crawl = 1 << 6,
		Airborne = 1 << 7,
		Sliding = 1 << 8,
		Swim = 1 << 9,
	}

	private State m_State = State.Run;
	
	public UnityEvent<State> OnStateChangeEvent = new UnityEvent<State>();

	public override float Priority => 1000; // Last
	public State ActiveState => m_State;

	public bool IsState(State pState) => (pState & m_State) != 0;

	protected override void OnInitalize()
	{
		Character.Input.Walk.onPerformed.AddListener(OnWalkPreformed);
		Character.Input.Walk.onCanceled.AddListener(OnWalkCanceled);
		Character.Input.Crouch.onPerformed.AddListener(OnCrouchPreformed);
		Character.Input.Crouch.onCanceled.AddListener(OnCrouchCanceled);
		Character.Input.LockOn.onPerformed.AddListener(OnLockOnPreformed);
		Character.Input.LockOn.onCanceled.AddListener(OnLockOnCanceled);
		Character.OnGround.OnStateChanged.AddListener(OnGroundedChanged);

		OnStateChangeEvent.Invoke(m_State); // Inital State
	}

	protected override void OnDestroyed()
	{
		Character.Input.Walk.onPerformed.RemoveListener(OnWalkPreformed);
		Character.Input.Walk.onCanceled.RemoveListener(OnWalkCanceled);
		Character.Input.Crouch.onPerformed.RemoveListener(OnCrouchPreformed);
		Character.Input.Crouch.onCanceled.RemoveListener(OnCrouchCanceled);
		Character.Input.LockOn.onPerformed.RemoveListener(OnLockOnPreformed);
		Character.Input.LockOn.onCanceled.RemoveListener(OnLockOnCanceled);
		Character.OnGround.OnStateChanged.RemoveListener(OnGroundedChanged);
	}

	private bool m_IsSprinting = true;
	protected override void Tick(float pDeltaTime)
	{
		bool isSprinting = Character.Input.Sprint.Input && Character.Input.Move.Input != Vector2.zero;
		if (m_IsSprinting != isSprinting)
		{
			m_IsSprinting = isSprinting;
			OnSprintChanged(m_IsSprinting);
		}
	}

	private void OnGroundedChanged(CharacterOnGround.State pState)
	{
		switch (pState)
		{
			case CharacterOnGround.State.Grounded:
				ReturnToDefaultIfState(State.Airborne | State.Sliding);
				break;
			case CharacterOnGround.State.Sliding:
				SetState(State.Sliding);
				break;
			case CharacterOnGround.State.Airborne:
				SetState(State.Airborne);
				break;
		}
	}

	private void OnSprintChanged(bool isSprinting)
	{
		if (isSprinting)
		{
			SetStateIfState(State.Sprint, State.Run | State.Walk | State.Strafe | State.Crouch);
		}
		else
		{
			ReturnToDefaultIfState(State.Sprint);
		}
	}

	private void OnWalkPreformed() => SetStateIfState(State.Walk, State.Run);
	private void OnWalkCanceled() => ReturnToDefaultIfState(State.Walk);

	private void OnCrouchPreformed() => SetStateIfState(State.Crouch, State.Run | State.Sprint | State.Walk | State.Strafe);
	private void OnCrouchCanceled() => ReturnToDefaultIfState(State.Crouch);

	private void OnLockOnPreformed() => SetStateIfState(State.Strafe, State.Run | State.Walk);
	private void OnLockOnCanceled() => ReturnToDefaultIfState(State.Strafe);

	public void SetState(State pState)
	{
		if (pState == m_State)
		{
			return;
		}

		m_State = pState;
		OnStateChangeEvent.Invoke(m_State);
	}

	private void ReturnToDefault()
	{
		if (m_IsSprinting)
		{
			SetState(State.Sprint);
			return;
		}
		if (Character.Input.Walk.Input)
		{
			SetState(State.Walk);
			return;
		}
		if (Character.Input.LockOn.Input)
		{
			SetState(State.Strafe);
			return;
		}
		SetState(State.Run);
	}

	public void SetStateIfState(State pToState, State pIfState)
	{
		if (IsState(pIfState))
		{
			SetState(pToState);
		}
	}

	public void ReturnToDefaultIfState(State pIfState)
	{
		if (IsState(pIfState))
		{
			ReturnToDefault();
		}
	}
}
