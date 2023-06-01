using OliverLoescher;
using OliverLoescher.Input;
using UnityEngine;
using UnityEngine.Events;

public class KinematicForceController : KinematicController, ICharacterBehaviour
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

	[Header("Force")]
	[SerializeField]
	private Transform forwardTransform = null;
	[SerializeField]
	private MonoUtil.Updateable updateable = new MonoUtil.Updateable(MonoUtil.UpdateType.Fixed, MonoUtil.Priorities.CharacterController);

	[Header("Move")]
	[SerializeField]
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
	// [SerializeField]
	// private MoveValues m_SwimValues = new MoveValues();

	[Header("Jump")]
	[SerializeField, Min(0.0f)]
	private float m_JumpForce = 5.0f;
	[SerializeField, Min(0)]
	private int m_AirJumpCount = 0;

	private Character m_Character = null;
	private Vector3 m_Velocity = Vector3.zero;
	private MoveValues m_Values;
	private int m_RemainingJumps = 0;

	public UnityEvent OnJumpEvent = new UnityEvent();

	public Vector3 Velocity => m_Velocity;
	public Vector3 Forward()
	{
		if (forwardTransform == null)
		{
			if (Capsule.upTransform == null)
			{
				return Vector3.forward;
			}
			return Vector3.ProjectOnPlane(Vector3.forward, Capsule.Up).normalized;
		}
		return Vector3.ProjectOnPlane(forwardTransform.forward, Capsule.Up).normalized;
	}
	public Vector3 Right()
	{
		if (forwardTransform == null)
		{
			if (Capsule.upTransform == null)
			{
				return Vector3.right;
			}
			return Vector3.ProjectOnPlane(Vector3.right, Capsule.Up).normalized;
		}
		return Vector3.ProjectOnPlane(forwardTransform.right, Capsule.Up).normalized;
	}
	
	float ICharacterBehaviour.Priority => 0;

	void ICharacterBehaviour.Initalize(Character pCharacter)
	{
		m_Character = pCharacter;
		updateable.Register(Tick);
		m_Character.Input.Jump.onPerformed.AddListener(DoJump);
		m_Character.MoveState.OnStateChangeEvent.AddListener(SetState);
	}

	private void OnDestroy()
	{
		updateable.Deregister();
		m_Character.Input.Jump.onPerformed.RemoveListener(DoJump);
		m_Character.MoveState.OnStateChangeEvent.RemoveListener(SetState);
	}

	private void Tick(float pDeltaTime)
	{
		AddMove(pDeltaTime);

		m_Velocity -= m_Velocity * m_Values.Drag * pDeltaTime;
		m_Velocity.y = 0.0f;
		Move(m_Velocity * pDeltaTime);

		// TODO Move this to when isGrounded is set instead of here
		if (isGrounded)
		{
			m_RemainingJumps = m_AirJumpCount;
		}
	}

	private void AddMove(in float pDeltaTime)
	{
		Vector2 input = m_Character.Input.Move.Input;
		if (input == Vector2.zero)
		{
			return;
		}
		m_Velocity += Forward() * input.y * pDeltaTime * m_Values.ForwardAcceleration;
		m_Velocity += Right() * input.x * pDeltaTime * m_Values.SideAcceleration;
	}

	private void DoJump()
	{
		if (isGrounded || m_RemainingJumps-- > 0)
		{
			verticalVelocity = m_JumpForce;
			isGrounded = false;
			OnJumpEvent.Invoke();
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
			default:
				throw new System.NotImplementedException($"CharacterMoveState.MoveState.{System.Enum.GetName(typeof(CharacterMoveState.State), pState)} does not have any move values set");
		}
	}
}
