using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverLoescher;

[System.Serializable]
public class CharacterMoveDodge : CharacterMoveManager.MoveController
{
	public enum DodgeType
	{
		RollForward = 0,
		SprintRollForward = 1,
		DodgeForward = 2,
		DodgeRight = 3,
		DodgeBackward = 4,
		DodgeLeft = 5
	}

	[SerializeField]
	private float m_DodgeSpeed = 10.0f;
	[SerializeField]
	private float m_SprintDodgeDelay = 0.5f;

	private bool m_IsDodging = false;
	private Vector3 m_MoveDirection = Vector3.zero;
	private float m_LastSprintTime = 0.1f;

	public override void OnDestroy()
	{
		OnDodgeEnd();
	}

	public override void OnMoveStateChange(CharacterMoveState.State pState)
	{
		if (Character.MoveState.PreviousState == CharacterMoveState.State.Sprint) // Stopped Sprinting
		{
			m_LastSprintTime = Time.time;
		}
		if (pState != CharacterMoveState.State.Dodge)
		{
			OnDodgeEnd();
			return;
		}
		OnDodgeStart();
	}

	public override void Tick(float pDeltaTime)
	{
		if (!m_IsDodging)
		{
			return;
		}
		Character.Controller.SetVelocity(m_MoveDirection * m_DodgeSpeed);
	}

	private void OnDodgeStart()
	{
		if (m_IsDodging)
		{
			return;
		}
		m_IsDodging = true;
		GetDodgeDirection(out Vector3 pMove, out Vector3 pFacing, out DodgeType pType);
		m_MoveDirection = pMove;
		Character.ModelMovement.SetOverrideFacing(pFacing);
		Character.AnimController.PlayDodge(pType);
	}

	private void GetDodgeDirection(out Vector3 pMove, out Vector3 pFacing, out DodgeType pType)
	{
		if (Character.Input.Move.Input.sqrMagnitude <= Util.NEARZERO)
		{
			// Backstep
			pFacing = Util.Horizontalize(Character.Model.forward);
			pMove = -pFacing;
			pType = DodgeType.DodgeBackward;
			return;
		}

		Vector3 forward = Character.Camera.TransformDirection(Character.Input.Move.InputHorizontal);
		pMove = Util.Horizontalize(forward);

		if (Character.Input.LockOn.Input)
		{
			// Strafe dodge
			pFacing = Util.Horizontalize(Character.Model.forward);
			pType = GetDodgeStrafeType(pFacing, pMove);
			return;
		}
		// Dodge forward
		pFacing = pMove;
		pType = Time.time < m_LastSprintTime + m_SprintDodgeDelay ? DodgeType.SprintRollForward : DodgeType.RollForward;
	}

	private DodgeType GetDodgeStrafeType(Vector3 pFacing, Vector3 pMove)
	{
		float angle = Vector3.SignedAngle(pFacing, pMove, Vector3.up);
		angle = Util.InverseSafeAngle(angle); // 0 - 360
		int index = Mathf.RoundToInt(angle / 90.0f); // 0 - 4
		if (index > 3) { index -= 4; } // 0 to 3
		return (DodgeType)(index + 2); // 2 - 5
	}

	private void OnDodgeEnd()
	{
		if (!m_IsDodging)
		{
			return;
		}
		m_IsDodging = false;
		Character.ModelMovement.ClearOverrideFacing();
	}
}
