using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMoveController : CharacterBehaviour
{
	[SerializeField]
	private CharacterMoveForceController m_ForceController = new CharacterMoveForceController();

	[FoldoutGroup("Events"), SerializeField]
	private UnityEvent m_OnJumpEvent = new UnityEvent();

	public Vector3 Velocity => m_ForceController.Velocity;
	public bool IsGrounded => Character.OnGround.IsGrounded;

	public Vector3 Forward => m_ForceController.Forward();
	public Vector3 Right => m_ForceController.Right();
	public Vector3 Up => m_ForceController.Up;

	public UnityEvent OnJumpEvent => m_OnJumpEvent;

	protected override void OnInitalize()
	{
		Character.OnGround.OnStateChanged.AddListener(OnGroundStateChange);
		Character.Input.Jump.onPerformed.AddListener(DoJump);
		Character.MoveState.OnStateChangeEvent.AddListener(OnMoveStateChange);

		m_ForceController.Initalize(Character, GetComponentInChildren<Rigidbody>());
	}

	protected override void OnDestroyed()
	{
		Character.OnGround.OnStateChanged.RemoveListener(OnGroundStateChange);
		Character.Input.Jump.onPerformed.RemoveListener(DoJump);
		Character.MoveState.OnStateChangeEvent.RemoveListener(OnMoveStateChange);
	}

	private void OnGroundStateChange(CharacterOnGround.State pState)
	{
		m_ForceController.OnGroundStateChange(pState);
	}

	private void DoJump()
	{
		if (m_ForceController.TryDoJump())
		{
			m_OnJumpEvent.Invoke();
		}
	}

	private void OnMoveStateChange(CharacterMoveState.State pState)
	{
		m_ForceController.SetState(pState);
	}

	protected override void Tick(float pDeltaTime)
	{
		m_ForceController.Tick(pDeltaTime, Character.Input.Move.Input);
	}

	public void Move(Vector3 pMove)
	{
		m_ForceController.Move(pMove);
	}

	public void AddVelocity(Vector3 pVelocity)
	{
		m_ForceController.AddVelocity(pVelocity);
	}
}
