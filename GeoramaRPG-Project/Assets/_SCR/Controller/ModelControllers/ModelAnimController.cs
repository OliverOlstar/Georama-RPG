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
	private const string KEY_DODGE = "Dodge";
	private const string KEY_DODGE_INDEX = "Dodge_State";
	private const string KEY_RIGHT_ATTACK = "R1";
	private const string KEY_LEFT_ATTACK = "L1";
	private const string KEY_MOVE_STATE = "Move_State";
	private const string KEY_CROUCH_BLEND = "Crouch_Blend";

	[Header("Values"), SerializeField]
	private float m_MoveScalar = 0.08f;
	[SerializeField]
	private float m_MoveDampening = 1.0f;

	[SerializeField]
	private AnimatedValueInOut m_CrouchTransition = new AnimatedValueInOut();

	private Animator m_Animator = null;

	protected override void OnInitalize()
	{
		m_Animator = GetComponentInChildren<Animator>();
		Character.Input.Primary.onPerformed.AddListener(OnPrimary);
		Character.Input.Secondary.onPerformed.AddListener(OnSecondary);
		Character.Controller.OnJumpEvent.AddListener(OnJump);
		Character.MoveState.OnStateChangeEvent.AddListener(SetMoveState);
		Character.OnGround.OnStateChanged.AddListener(OnGroundChanged);
	}

	protected override void OnDestroyed()
	{
		Character.Input.Primary.onPerformed.AddListener(OnPrimary);
		Character.Input.Secondary.onPerformed.AddListener(OnSecondary);
		Character.Controller.OnJumpEvent.RemoveListener(OnJump);
		Character.MoveState.OnStateChangeEvent.RemoveListener(SetMoveState);
		Character.OnGround.OnStateChanged.RemoveListener(OnGroundChanged);
	}

	protected override void Tick(float pDeltaTime)
	{
		Vector3 localVelocity = Character.Model.InverseTransformDirection(Character.Controller.Velocity);
		localVelocity.y = 0.0f;
		float speed = localVelocity.magnitude;
		localVelocity.Normalize();
		SetFloat(KEY_MOVE_X, localVelocity.x, m_MoveDampening, pDeltaTime);
		SetFloat(KEY_MOVE_Z, localVelocity.z, m_MoveDampening, pDeltaTime);
		SetFloat(KEY_MOVE_SPEED, speed * m_MoveScalar, m_MoveDampening, pDeltaTime);
	}

	private void OnJump()
	{
		m_Animator.SetTrigger(KEY_JUMP);
	}

	public void PlayDodge(CharacterMoveDodge.DodgeType pDodgeType)
	{
		m_Animator.SetInteger(KEY_DODGE_INDEX, (int)pDodgeType);
		m_Animator.SetTrigger(KEY_DODGE);
	}

	private Vector3 m_RootMotion = Vector3.zero;
	public Vector3 GetRootMotionLastEvaluatedFrame()
	{
		Vector3 motion = m_RootMotion;
		m_RootMotion = Vector3.zero;
		return motion;
	}
	public void ResetRootMotion() { m_RootMotion = Vector3.zero; }

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
		m_CrouchTransition.StartOut(SetCrouchBlend);
		switch (pState)
		{
			case CharacterMoveState.State.Crouch:
				m_CrouchTransition.StartIn(SetCrouchBlend);
				break;
			case CharacterMoveState.State.Crawl:
				m_Animator.SetInteger(KEY_MOVE_STATE, 2);
				return;
			case CharacterMoveState.State.Swim:
				m_Animator.SetInteger(KEY_MOVE_STATE, 3);
				return;
		}
		m_Animator.SetInteger(KEY_MOVE_STATE, 0);
	}

	private void SetCrouchBlend(float pValue)
	{
		m_Animator.SetFloat(KEY_CROUCH_BLEND, pValue);
	}

	private void OnGroundChanged(CharacterOnGround.State pState)
	{
		m_Animator.SetBool(KEY_GROUNDED, pState != CharacterOnGround.State.Airborne);
	}

	private void OnAnimatorMove() { m_RootMotion += m_Animator.deltaPosition; }
}