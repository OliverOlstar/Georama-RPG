using OliverLoescher;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using static OliverLoescher.MonoUtil;

public class KinematicForceController : KinematicController
{
	[Header("Force")]
	[SerializeField]
	private InputBridge_KinematicController input = null;
	[SerializeField]
	private Transform forwardTransform = null;
	[SerializeField]
	private MonoUtil.Updateable updateable = new MonoUtil.Updateable(MonoUtil.UpdateType.Fixed, MonoUtil.Priorities.CharacterController);

	[Header("Move")]
	[SerializeField, Min(0.0f)]
	private float forwardAcceleration = 5.0f;
	[SerializeField, Min(0.0f)]
	private float sideAcceleration = 5.0f;
	[SerializeField, Min(0.0f)]
	private float drag = 5.0f;

	[Header("Sprint")]
	[SerializeField, Min(0.0f)]
	private float sprintForwardAcceleration = 5.0f;
	[SerializeField, Min(0.0f)]
	private float sprintSideAcceleration = 5.0f;

	[Header("Jump")]
	[SerializeField, Min(0.0f)]
	private float jumpForce = 5.0f;

	private Vector3 velocity = Vector3.zero;

	public Vector3 Velocity => velocity;
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

	public bool IsSprinting => input.Sprint.Input;
	private float ForwardAcceleration => IsSprinting ? sprintForwardAcceleration : forwardAcceleration;
	private float SideAcceleration => IsSprinting ? sprintSideAcceleration : sideAcceleration;

	private void Start()
	{
		updateable.Register(Tick);
		input.Jump.onPerformed.AddListener(DoJump);
	}

	private void OnDestroy()
	{
		updateable.Deregister();
		input.Jump.onPerformed.RemoveListener(DoJump);
	}

	private void Tick(float pDeltaTime)
	{
		AddMove(pDeltaTime);

		velocity -= velocity * drag * pDeltaTime;
		velocity.y = 0.0f;
		Move(velocity * pDeltaTime);
	}

	private void AddMove(in float pDeltaTime)
	{
		if (input.Move.Input == Vector2.zero)
		{
			return;
		}

		velocity += Forward() * input.Move.Input.y * pDeltaTime * ForwardAcceleration;
		velocity += Right() * input.Move.Input.x * pDeltaTime * SideAcceleration;
	}

	private void DoJump()
	{
		if (isGrounded)
		{
			verticalVelocity = jumpForce;
			isGrounded = false;
		}
	}
}
