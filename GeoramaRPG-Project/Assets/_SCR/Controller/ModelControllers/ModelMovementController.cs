using System;
using OliverLoescher;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelMovementController : CharacterBehaviour
{
	public enum RotationTarget
	{
		Velocity = 0,
		Target
	}

	[Header("Tilt"), SerializeField]
	private float m_TiltScalar = 1.0f;
	[SerializeField]
	private float m_TiltDampening = 5.0f;

	[Header("Rotation"), SerializeField]
	private float m_RotationDampening = 5.0f;
	[SerializeField]
	private Transform m_Target;

	private RotationTarget m_RotationTarget = RotationTarget.Velocity;

	protected override void OnInitalize()
	{
		Character.MoveState.OnStateChangeEvent.AddListener(OnMoveStateChange);
		Character.Input.LockOn.onChanged.AddListener(OnLockOnInput);
		Character.GetBehaviour<CharacterInteractions>().OnTargetChanged.AddListener(OnTargetChanged);
	}

	protected override void OnDestroyed()
	{
		Character.MoveState.OnStateChangeEvent.RemoveListener(OnMoveStateChange);
		Character.Input.LockOn.onChanged.RemoveListener(OnLockOnInput);
		Character.GetBehaviour<CharacterInteractions>().OnTargetChanged.RemoveListener(OnTargetChanged);
	}

	protected override void Tick(float pDeltaTime)
	{
		DoTilt(pDeltaTime);
		DoRotation(pDeltaTime);
	}

	private void DoTilt(in float pDeltaTime)
	{
		if (m_TiltDampening <= 0.0f || Character.Input.Move.Input == Vector2.zero)
		{
			return;
		}
		Vector3 eularAngles = transform.localEulerAngles;
		Quaternion toWorld = Quaternion.FromToRotation(Util.Horizontalize(transform.forward), Character.Forward);
		Vector3 move = toWorld * Character.Input.Move.InputHorizontal;
		eularAngles.x = move.z * m_TiltScalar; // Forward
		eularAngles.z = -move.x * m_TiltScalar; // Right
		transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(eularAngles), m_TiltDampening * pDeltaTime);
	}

	private void DoRotation(in float pDeltaTime)
	{
		if (m_RotationDampening <= 0.0f)
		{
			return;
		}
		Vector3 forward = GetForward();
		if (forward.sqrMagnitude <= Util.NEARZERO)
		{
			return;
		}
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(forward), m_RotationDampening * pDeltaTime);
	}

	private Vector3 GetForward()
	{
		if (m_RotationTarget == RotationTarget.Target)
		{
			if (m_Target == null)
			{
				// No Target
				return Character.Forward;
			}
			// Target
			return Util.Horizontalize(m_Target.position - transform.position, Character.Up);
		}
		// RotationTarget.Velocity
		return Util.Horizontalize(Character.Velocity, Character.Up);
	}

	private void UpdateState()
	{
		if (!Character.Input.LockOn.Input)
		{
			m_RotationTarget = RotationTarget.Velocity;
			return;
		}
		if (Character.MoveState.IsState(CharacterMoveState.State.Strafe))
		{
			m_RotationTarget = RotationTarget.Target;
			return;
		}
		if (Character.MoveState.IsState(CharacterMoveState.State.Airborne) && Character.Input.LockOn.Input)
		{
			m_RotationTarget = RotationTarget.Target;
			return;
		}
		m_RotationTarget = RotationTarget.Velocity;
	}

	private void OnMoveStateChange(CharacterMoveState.State _) => UpdateState();
	private void OnLockOnInput(bool _) => UpdateState();
	private void OnTargetChanged(ITargetable target)
	{
		m_Target = target == null ? null : target.Transform;
		UpdateState();
	}
}
