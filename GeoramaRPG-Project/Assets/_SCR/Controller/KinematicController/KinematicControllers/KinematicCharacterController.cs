using OliverLoescher;
using RootMotion.Dynamics;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class KinematicCharacterController : MonoBehaviour
{
	[SerializeField]
	private Updateable updateable = new Updateable(MonoUtil.UpdateType.Fixed, MonoUtil.Priorities.CharacterController);

	[Header("References")]
	[SerializeField]
	private InputBridge_KinematicController input = null;
	[SerializeField]
	private Transform forwardTransform = null;
	[SerializeField]
	private CharacterControllerCapsule capsule = new CharacterControllerCapsule();

	[Header("Movement")]
	[SerializeField, Min(0.0f)]
	private float gravity = 9.81f;
	[SerializeField]
	private float groundedDistance = 0.05f;
	[SerializeField, Range(-1.0f, 1.0f)]
	private float groundAllowDot = 0.5f;
	[SerializeField]
	private LayerMask groundLayerMask = new LayerMask();

	[Header("Move")]
	[SerializeField, Min(0.0f)]
	private float forwardAcceleration = 5.0f;
	[SerializeField, Min(0.0f)]
	private float sideAcceleration = 5.0f;
	[SerializeField, Min(0.0f)]
	private float groundDrag = 5.0f;
	[SerializeField, Min(0.0f)]
	private float airDrag = 2.0f;

	[Header("Sprint")]
	[SerializeField, Min(0.0f)]
	private float sprintForwardAcceleration = 5.0f;
	[SerializeField, Min(0.0f)]
	private float sprintSideAcceleration = 5.0f;

	[Header("Jump")]
	[SerializeField, Min(0.0f)]
	private float jumpForce = 5.0f;

	protected bool isGrounded = false;
	protected Vector3 groundNormal = Vector3.up;

	private Vector3 velocity = Vector2.zero;

	public bool IsGrounded => isGrounded;
	public Vector3 Up => capsule.Up;
	private Transform UpTransfrom => capsule.upTransform;
	private CharacterController Controller => capsule.controller;
	private bool IsValidGround(in Vector3 normal) => Vector3.Dot(Up, normal) > groundAllowDot;

	public Vector3 Velocity => velocity;
	public Vector3 Forward()
	{
		if (forwardTransform == null)
		{
			if (UpTransfrom == null)
			{
				return Vector3.forward;
			}
			return Vector3.ProjectOnPlane(Vector3.forward, Up).normalized;
		}
		return Vector3.ProjectOnPlane(forwardTransform.forward, Up).normalized;
	}
	public Vector3 Right()
	{
		if (forwardTransform == null)
		{
			if (UpTransfrom == null)
			{
				return Vector3.right;
			}
			return Vector3.ProjectOnPlane(Vector3.right, Up).normalized;
		}
		return Vector3.ProjectOnPlane(forwardTransform.right, Up).normalized;
	}

	public bool IsSprinting => input.Sprint.Input;
	private float ForwardAcceleration => IsSprinting ? sprintForwardAcceleration : forwardAcceleration;
	private float SideAcceleration => IsSprinting ? sprintSideAcceleration : sideAcceleration;
	public float Drag => isGrounded ? groundDrag : airDrag;

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
		Vector3 move = Vector3.zero;

		UpdateGrounded(Time.fixedDeltaTime);
		velocity -= velocity * Drag * pDeltaTime;
		move += velocity * pDeltaTime;

		// Horizontal
		if (isGrounded)
		{
			if (input.Move.Input != Vector2.zero)
			{
				Vector3 horizontalVelocity = input.Move.Input.y * pDeltaTime * ForwardAcceleration * Forward();
				horizontalVelocity += input.Move.Input.x * pDeltaTime * SideAcceleration * Right();
				move += Util.Horizontalize(horizontalVelocity, groundNormal, horizontalVelocity.magnitude);
			}
		}

		if (capsule.CheckCollisions(move, transform.position, 3, groundLayerMask, out Vector3 resultPos, out _))
		{
			move = resultPos - transform.position;
		}

		Controller.Move(move);
	}

	private void UpdateGrounded(float pDeltaTime)
	{
		isGrounded = capsule.CheckCollisions(Up * -groundedDistance, transform.position + (Up * -0.1f), 0, groundLayerMask, out _, out Vector3 collisionNormal) && IsValidGround(collisionNormal);
		Debug.Log($"UpdateGrounded() {isGrounded}, normal {collisionNormal}, dot {Vector3.Dot(Up, collisionNormal)}");
		if (isGrounded)
		{
			velocity.y = 0.0f;
			groundNormal = collisionNormal;
		}
		else
		{
			velocity.y += -gravity * pDeltaTime;
			groundNormal = Up;
		}
	}

	private void DoJump()
	{
		if (IsGrounded)
		{
			velocity.y = jumpForce;
		}
	}

	private void OnDrawGizmos()
	{
		// Grounded Line
		Gizmos.color = isGrounded ? Color.green : Color.red;
		Gizmos.DrawLine(transform.position, transform.position - (Up * groundedDistance));

		// Capsule
		Gizmos.color = Color.cyan;
		capsule.DrawGizmos(transform.position);
	}
}