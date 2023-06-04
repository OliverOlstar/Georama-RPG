using OliverLoescher;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[System.Serializable]
public class CharacterMoveForceController
{
	[System.Serializable]
	public struct MoveValues
	{
		[Min(0.0f)]
		public float ForwardAcceleration;
		[Min(0.0f)]
		public float SideAcceleration;
		[Min(0.0f)]
		public float Drag;
	}

	[Header("Force"), SerializeField]
	private Transform m_ForwardTransform = null;

	[Header("Move"), SerializeField]
	private MoveValues m_RunValues = new MoveValues();
	[SerializeField]
	private MoveValues m_SprintValues = new MoveValues();
	[SerializeField]
	private MoveValues m_WalkValues = new MoveValues();
	[SerializeField]
	private MoveValues m_StrafeValues = new MoveValues();
	[SerializeField]
	private MoveValues m_CrouchValues = new MoveValues();
	[SerializeField]
	private MoveValues m_CrawlValues = new MoveValues();
	[SerializeField]
	private MoveValues m_AirborneValues = new MoveValues();
	[SerializeField]
	private MoveValues m_SlidingValues = new MoveValues();
	// [SerializeField]
	// private MoveValues m_SwimValues = new MoveValues();

	[Header("Jump"), SerializeField, Min(0.0f)]
	private float m_JumpForce = 5.0f;
	[SerializeField, Min(0)]
	private int m_AirJumpCount = 0;
	[SerializeField, Min(0.0f)]
	private float m_Gravity = 9.81f;

	private Character m_Character;
	private Rigidbody m_Rigidbody;
	private Vector3 m_Velocity = Vector3.zero;
	protected float m_VerticalVelocity = 0.0f;
	private MoveValues m_Values;
	private int m_RemainingJumps = 0;

	public Vector3 Velocity => m_Velocity;
	public Vector3 Up => Vector3.up;

	public Vector3 Forward() => Vector3.ProjectOnPlane(m_ForwardTransform.forward, Up).normalized;
	public Vector3 Right() => Vector3.ProjectOnPlane(m_ForwardTransform.right, Up).normalized;

	public void Initalize(Character pCharacter, Rigidbody pRigidbody)
	{
		m_Character = pCharacter;
		m_Rigidbody = pRigidbody;
	}

	public void Move(Vector3 pMove)
	{
		m_Rigidbody.MovePosition(m_Rigidbody.position + pMove);
	}

	public void AddVelocity(Vector3 pVelocity)
	{
		m_Velocity.x += pVelocity.x;
		m_VerticalVelocity += pVelocity.y;
		m_Velocity.z += pVelocity.z;
	}

	public void Tick(float pDeltaTime, Vector2 pInput)
	{
		DoGravity(pDeltaTime);
		AddMove(pDeltaTime, pInput);
		m_Velocity -= m_Velocity * m_Values.Drag * pDeltaTime;

		Vector3 vel = RotateMoveDirection(m_Velocity) + (CalculateGravityDirection() * m_VerticalVelocity);
		m_Rigidbody.velocity = vel;
	}

	private void AddMove(in float pDeltaTime, in Vector2 pInput)
	{
		if (pInput == Vector2.zero)
		{
			return;
		}
		m_Velocity += Forward() * pInput.y * pDeltaTime * m_Values.ForwardAcceleration;
		m_Velocity += Right() * pInput.x * pDeltaTime * m_Values.SideAcceleration;
	}

	private void DoGravity(float pDeltaTime)
	{
		switch (m_Character.OnGround.GroundedState)
		{
			case CharacterOnGround.State.Grounded:
				m_VerticalVelocity = Mathf.Max(m_VerticalVelocity, 0.0f);
				break;

			case CharacterOnGround.State.Sliding:
				m_VerticalVelocity += -m_Gravity * pDeltaTime;
				break;

			case CharacterOnGround.State.Airborne:
				m_VerticalVelocity += -m_Gravity * pDeltaTime;
				break;
		}
	}

	public bool TryDoJump()
	{
		if (m_Character.OnGround.IsGrounded || m_RemainingJumps-- > 0)
		{
			m_VerticalVelocity = m_JumpForce;
			return true;
		}
		return false;
	}

	private Vector3 CalculateGravityDirection()
	{
		if (!m_Character.OnGround.IsSliding)
		{
			return Vector3.up;
		}
		return Vector3.ProjectOnPlane(Vector3.up, m_Character.OnGround.GetAverageNormal()).normalized;
	}

	private Vector3 RotateMoveDirection(in Vector3 pVelocity)
	{
		if (!m_Character.OnGround.IsGrounded)
		{
			return pVelocity;
		}
		float magnitude = pVelocity.magnitude;
		return Vector3.ProjectOnPlane(pVelocity, m_Character.OnGround.GetAverageNormal()).normalized * magnitude;
	}

	public void OnGroundStateChange(CharacterOnGround.State pState)
	{
		if (pState == CharacterOnGround.State.Grounded)
		{
			m_RemainingJumps = m_AirJumpCount;
		}
	}

	public void SetState(CharacterMoveState.State pState)
	{
		m_Values = GetValues(pState);
	}

	private MoveValues GetValues(CharacterMoveState.State pState)
	{
		switch (pState)
		{
			case CharacterMoveState.State.Run:
				return m_RunValues;
			case CharacterMoveState.State.Sprint:
				return m_SprintValues;
			case CharacterMoveState.State.Walk:
				return m_WalkValues;
			case CharacterMoveState.State.Strafe:
				return m_StrafeValues;
			case CharacterMoveState.State.Crouch:
				return m_CrouchValues;
			case CharacterMoveState.State.Crawl:
				return m_CrawlValues;
			case CharacterMoveState.State.Airborne:
				return m_AirborneValues;
			case CharacterMoveState.State.Sliding:
				return m_SlidingValues;
			default:
				throw new System.NotImplementedException($"CharacterMoveState.MoveState.{System.Enum.GetName(typeof(CharacterMoveState.State), pState)} does not have any move values set");
		}
	}
}