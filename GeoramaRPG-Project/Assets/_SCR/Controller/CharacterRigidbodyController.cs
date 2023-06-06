using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Rigidbody))]
public class CharacterRigidbodyController : CharacterBehaviour
{
	[SerializeField, Min(0.0f)]
	private float m_Gravity = 9.81f;

	private Rigidbody m_Rigidbody;
	private Vector2 m_Velocity = Vector2.zero;
	protected float m_VerticalVelocity = 0.0f;
	private float m_Drag = 1.0f;

	public Vector3 Velocity => new Vector3(m_Velocity.x, m_VerticalVelocity, m_Velocity.y);
	public void SetDrag(float pDrag) => m_Drag = pDrag;

	protected override void OnInitalize()
	{
		m_Rigidbody = GetComponent<Rigidbody>();
	}

	public void Move(Vector3 pMove)
	{
		m_Rigidbody.MovePosition(m_Rigidbody.position + pMove);
	}

	public void ModifyVelocity(Vector3 pVelocity)
	{
		m_Velocity.x += pVelocity.x;
		m_VerticalVelocity += pVelocity.y;
		m_Velocity.y += pVelocity.z;
	}

	public void SetVelocity(Vector3 pVelocity)
	{
		m_Velocity.x = pVelocity.x;
		m_VerticalVelocity = pVelocity.y;
		m_Velocity.y = pVelocity.z;
	}

	protected override void Tick(float pDeltaTime)
	{
		DoGravity(pDeltaTime);
		m_Velocity -= m_Velocity * m_Drag * pDeltaTime;

		Vector3 vel = RotateMoveDirection(m_Velocity) + (CalculateGravityDirection() * m_VerticalVelocity);
		m_Rigidbody.velocity = vel;
	}

	private void DoGravity(float pDeltaTime)
	{
		switch (Character.OnGround.GroundedState)
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

	private Vector3 CalculateGravityDirection()
	{
		if (!Character.OnGround.IsSliding)
		{
			return Vector3.up;
		}
		return Vector3.ProjectOnPlane(Vector3.up, Character.OnGround.GetAverageNormal()).normalized;
	}

	private Vector3 RotateMoveDirection(in Vector2 pVelocity)
	{
		Vector3 velocity = new Vector3(pVelocity.x, 0.0f, pVelocity.y);
		if (!Character.OnGround.IsGrounded)
		{
			return velocity;
		}
		float magnitude = pVelocity.magnitude;
		return Vector3.ProjectOnPlane(velocity, Character.OnGround.GetAverageNormal()).normalized * magnitude;
	}
}
