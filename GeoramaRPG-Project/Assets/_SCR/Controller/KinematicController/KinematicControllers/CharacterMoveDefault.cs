using OliverLoescher;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[System.Serializable]
public class CharacterMoveDefault : CharacterMoveManager.MoveController
{
	[System.Serializable]
	public struct MoveValues
	{
		public static readonly MoveValues NULL = new MoveValues();

		[Min(0.0f)]
		public float ForwardAcceleration;
		[Min(0.0f)]
		public float SideAcceleration;
		[Min(0.0f)]
		public float Drag;
	}

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
	private MoveValues m_AirborneValues = new MoveValues();

	private MoveValues m_Values;
	private bool m_Enabled = true;

	public Vector3 Forward() => Vector3.ProjectOnPlane(Character.Camera.forward, Vector3.up).normalized;
	public Vector3 Right() => Vector3.ProjectOnPlane(Character.Camera.right, Vector3.up).normalized;

	public override void Tick(float pDeltaTime)
	{
		if (!m_Enabled)
		{
			return;
		}
		Vector2 input = Character.Input.Move.Input;
		if (input == Vector2.zero)
		{
			return;
		}
		Vector3 velocity = Vector3.zero;
		velocity += Forward() * input.y * pDeltaTime * m_Values.ForwardAcceleration;
		velocity += Right() * input.x * pDeltaTime * m_Values.SideAcceleration;
		Character.Controller.ModifyVelocity(velocity);
	}

	public override void OnMoveStateChange(CharacterMoveState.State pState)
	{
		m_Values = GetValues(pState);
		Character.Controller.SetDrag(m_Values.Drag);
	}

	private MoveValues GetValues(CharacterMoveState.State pState)
	{
		m_Enabled = true;
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
			case CharacterMoveState.State.Airborne:
				return m_AirborneValues;
			default:
				m_Enabled = false; // No a valid state for us, we are disabled for this state
				return MoveValues.NULL;
		}
	}
}
