using OliverLoescher;
using RootMotion.Dynamics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicControllerDebug : MonoBehaviour
{
	[SerializeField]
	private Vector3 movement = Vector3.forward;
	[SerializeField, Min(0.0f)]
	private float gravity = 9.81f;
	[SerializeField]
	private float groundedDistance = 0.05f;
	[SerializeField, Range(-1.0f, 1.0f)]
	private float groundAllowDot = 0.5f;

	[Header("Capsule"), SerializeField]
	private float radius = 1.0f;
	[SerializeField]
	private float height = 1.0f;
	[SerializeField]
	private Vector3 center = Vector3.zero;
	[SerializeField]
	private LayerMask layerMask = new LayerMask();

	[Space, SerializeField]
	private Transform forwardTransform = null;
	[SerializeField]
	private Transform upTransform = null;
	[SerializeField]
	private int collisionIterations = 3;

	[Header("Debug")]
	[SerializeField]
	private bool snapToGround = false;

	private bool isGrounded = false;
	private float verticalVelocity = 0.0f;
	private Vector3 groundNormal = Vector3.up;

	public bool IsGrounded => isGrounded;
	public Vector3 Up => upTransform == null ? Vector3.up : upTransform.up;
	public Vector3 Forward => forwardTransform == null ? Vector3.forward : forwardTransform.forward;
	public Vector3 Right => Vector3.Cross(Up, Forward);
	private bool IsValidGround(in Vector3 normal)
	{
		//Debug.Log($"DOT: {Vector3.Dot(Up, normal)}");
		return Vector3.Dot(Up, normal) > groundAllowDot;
	}
	public Vector3 CapsuleOffset => center + (Vector3.down * ((height * 0.5f) + radius));

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		if (StartCheckCollisions(transform.position, Up * -groundedDistance, 0, out Vector3 resultPos, out Vector3 collisionNormal) && IsValidGround(collisionNormal))
		{
			verticalVelocity = 0.0f;
			groundNormal = collisionNormal;
			isGrounded = true;
		}
		else
		{
			Gizmos.color = Color.magenta;
			verticalVelocity = gravity;
			isGrounded = StartCheckCollisions(transform.position, Up * -verticalVelocity, 1, out resultPos, out collisionNormal) && IsValidGround(collisionNormal);
			groundNormal = isGrounded ? collisionNormal : Up;
		}

		if (snapToGround)
		{
			transform.position = resultPos;
		}

		Gizmos.color = Color.yellow;
		Move(movement);
	}

	private void Move(Vector3 pMove)
	{
		if (isGrounded)
		{
			float y = pMove.y;
			pMove.y = 0;
			pMove = Util.Horizontalize(pMove, groundNormal, pMove.magnitude);
			pMove.y += y;
		}
		StartCheckCollisions(transform.position, pMove, collisionIterations, out _, out _);

		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, transform.position + pMove);
		Vector3 up = Up * ((height * 0.5f) - radius);
		Util.GizmoCapsule(transform.position + pMove + center + up, transform.position + pMove + center - up, radius);
	}

	private bool StartCheckCollisions(Vector3 pCurrPos, Vector3 pMovement, int iterations, out Vector3 resultPos, out Vector3 collisionNormal) // Returns resulting position
	{
		pCurrPos -= CapsuleOffset;
		bool result = CheckCollisions(pCurrPos, pMovement, iterations, out resultPos, out collisionNormal);
		resultPos += CapsuleOffset;
		return result;
	}

	private bool CheckCollisions(Vector3 pCurrPos, Vector3 pMovement, int iterations, out Vector3 resultPos, out Vector3 collisionNormal) // Returns resulting position
	{
		resultPos = pCurrPos + pMovement;
		collisionNormal = Up;

		RaycastHit? nearestHit = null;
		Vector3 up = Up * ((height * 0.5f) - radius);
		foreach (RaycastHit hit in Physics.CapsuleCastAll(pCurrPos + center + up, pCurrPos + center - up, radius, pMovement, pMovement.magnitude, layerMask))
		{
			if (!nearestHit.HasValue || nearestHit.Value.distance > hit.distance)
			{
				nearestHit = hit;
			}
		}
		if (nearestHit.HasValue)
		{
			Vector3 movementToTarget = pMovement.normalized * (nearestHit.Value.distance - Util.NEARZERO);
			resultPos = pCurrPos + movementToTarget;
			collisionNormal = IsValidGround(nearestHit.Value.normal) ? nearestHit.Value.normal : Util.Horizontalize(nearestHit.Value.normal, Up, 1);
			pMovement = Vector3.ProjectOnPlane(pMovement - movementToTarget, collisionNormal);

			Gizmos.DrawLine(pCurrPos, resultPos);
			Util.GizmoCapsule(resultPos + center + up, resultPos + center - up, radius);

			if (iterations > 0)
			{
				CheckCollisions(resultPos, pMovement, --iterations, out resultPos, out _);
			}
		}
		else
		{
			Gizmos.DrawLine(pCurrPos, resultPos);
			Util.GizmoCapsule(resultPos + center + up, resultPos + center - up, radius);
		}
		return nearestHit.HasValue;
	}
}
