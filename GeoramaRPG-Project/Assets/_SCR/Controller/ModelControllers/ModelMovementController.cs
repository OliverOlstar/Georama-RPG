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
		Target,
		None,
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
		Character.GetBehaviour<CharacterInteractions>().OnTargetChanged.AddListener(OnTargetChanged);
	}

	protected override void OnDestroyed()
	{
		Character.MoveState.OnStateChangeEvent.RemoveListener(OnMoveStateChange);
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
		Quaternion toWorld = Quaternion.FromToRotation(Util.Horizontalize(transform.forward), Character.Controller.Forward);
		Vector3 move = toWorld * Character.Input.Move.InputHorizontal;
		eularAngles.x = move.z * m_TiltScalar; // Forward
		eularAngles.z = -move.x * m_TiltScalar; // Right
		transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(eularAngles), m_TiltDampening * pDeltaTime);
	}

	private void DoRotation(in float pDeltaTime)
	{
		Vector3 forward = GetForward();
		if (forward.sqrMagnitude <= Util.NEARZERO)
		{
			return;
		}
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(forward), m_RotationDampening * pDeltaTime);
	}

	private Vector3 GetForward()
	{
		switch (m_RotationTarget)
		{
			case RotationTarget.None:
				return Util.Horizontal(transform.forward);

			case RotationTarget.Target:
				if (m_Target == null)
				{
					// No Target
					return Character.Controller.Forward;
				}
				// Target
				return Util.Horizontalize(m_Target.position - transform.position, Character.Controller.Up);

			case RotationTarget.Velocity:
				return Util.Horizontalize(Character.Controller.Velocity, Character.Controller.Up);
		}
		Core.DebugUtil.DevException(new NotImplementedException());
		return Vector3.forward;
	}

	private void OnMoveStateChange(CharacterMoveState.State pState)
	{
		switch (pState)
		{
			case CharacterMoveState.State.Strafe:
				m_RotationTarget = RotationTarget.Target;
				return;
			case CharacterMoveState.State.Airborne:
				m_RotationTarget = RotationTarget.None;
				return;
		}
		m_RotationTarget = RotationTarget.Velocity;
	}

	private void OnTargetChanged(ITargetable target)
	{
		m_Target = target == null ? null : target.Transform;
	}
}
