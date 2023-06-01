using OliverLoescher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelAnimController : CharacterBehaviour
{
	private const string KEY_MOVE_X = "Move_X";
	private const string KEY_MOVE_Z = "Move_Z";
	private const string KEY_MOVE_SPEED = "Move_Speed";
	private const string KEY_GROUNDED = "Grounded";
	private const string KEY_JUMP = "Jump";
	private const string KEY_RIGHT_ATTACK = "R1";
	private const string KEY_LEFT_ATTACK = "L1";
	private const string KEY_MOVE_STATE = "Move_State";

	[SerializeField]
	private Transform m_ModelForward = null;
	[Header("Values"), SerializeField]
	private float m_MoveScalar = 0.08f;
	[SerializeField]
	private float m_MoveDampening = 1.0f;

	private Animator m_Animator = null;

	protected override void OnInitalize()
	{
		m_Animator = GetComponentInChildren<Animator>();
		Character.Input.Primary.onPerformed.AddListener(OnPrimary);
		Character.Input.Secondary.onPerformed.AddListener(OnSecondary);
		Character.Controller.OnJumpEvent.AddListener(OnJump);
		Character.MoveState.OnStateChangeEvent.AddListener(SetMoveState);
	}

	protected override void OnDestroyed()
	{
		Character.Input.Primary.onPerformed.AddListener(OnPrimary);
		Character.Input.Secondary.onPerformed.AddListener(OnSecondary);
		Character.Controller.OnJumpEvent.RemoveListener(OnJump);
		Character.MoveState.OnStateChangeEvent.RemoveListener(SetMoveState);
	}

	protected override void Tick(float pDeltaTime)
	{
		Vector3 localVelocity = m_ModelForward.InverseTransformDirection(Character.Velocity);
		localVelocity.y = 0.0f;
		float speed = localVelocity.magnitude;
		localVelocity.Normalize();
		SetFloat(KEY_MOVE_X, localVelocity.x, m_MoveDampening, pDeltaTime);
		SetFloat(KEY_MOVE_Z, localVelocity.z, m_MoveDampening, pDeltaTime);
		SetFloat(KEY_MOVE_SPEED, speed * m_MoveScalar, m_MoveDampening, pDeltaTime);
		m_Animator.SetBool(KEY_GROUNDED, Character.IsGrounded);
	}

	private void OnJump()
	{
		m_Animator.SetTrigger(KEY_JUMP);
	}

	private void OnPrimary()
	{
		m_Animator.ResetTrigger(KEY_LEFT_ATTACK);
		m_Animator.SetTrigger(KEY_RIGHT_ATTACK);
	}

	private void OnSecondary()
	{
		m_Animator.ResetTrigger(KEY_RIGHT_ATTACK);
		m_Animator.SetTrigger(KEY_LEFT_ATTACK);
	}

	private void SetFloat(string pKey, float pValue, float pDampening, float pDeltaTime)
	{
		float value = m_Animator.GetFloat(pKey);
		value = Mathf.Lerp(value, pValue, pDampening * pDeltaTime);
		m_Animator.SetFloat(pKey, value);
	}

	private void SetMoveState(CharacterMoveState.State pState)
	{
		switch (pState)
		{
			case CharacterMoveState.State.Crouch:
				m_Animator.SetInteger(KEY_MOVE_STATE, 1);
				return;
			case CharacterMoveState.State.Crawl:
				m_Animator.SetInteger(KEY_MOVE_STATE, 2);
				return;
			case CharacterMoveState.State.Swim:
				m_Animator.SetInteger(KEY_MOVE_STATE, 3);
				return;
		}
		m_Animator.SetInteger(KEY_MOVE_STATE, 0);
	}

	private void FootR() {} // SUPRESS ANIM EVENTS
	private void FootL() {}
	private void Hit() {}
	private void Land() {}
}