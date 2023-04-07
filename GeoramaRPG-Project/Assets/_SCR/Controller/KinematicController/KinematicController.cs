using OliverLoescher;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Text.RegularExpressions;
using RootMotion.Dynamics;

[InfoBox("1. IsGround could be done better\n2. Moving object will pass through instead of push\n3. Doesn't move with moving objects\n4. No good solution to make getting over little ledges")]
public class KinematicController : MonoBehaviour
{
	[Header("Movement")]
	[SerializeField, Min(0.0f)]
	private float gravity = 9.81f;
	[SerializeField, Range(-1.0f, 1.0f)]
	private float groundAllowDot = 0.5f;
	[SerializeField]
	private int collisionIterations = 3;
	[SerializeField, Min(0)]
	private float stepUpHeight = 0.1f;
	[SerializeField, Min(0)]
	private float stepDownHeight = 0.1f;
	[SerializeField, Min(0)]
	private float groundedDistance = 0.05f;

	[Header("Capsule")]
	[SerializeField]
	private Capsule capsule = new Capsule();
	[SerializeField]
	private LayerMask layerMask = new LayerMask();

	protected bool isGrounded = false;
	protected float verticalVelocity = 0.0f;
	protected Vector3 groundNormal = Vector3.up;
	private Vector3 storedMovement = Vector3.zero;

	public bool IsGrounded => isGrounded;
	public Capsule Capsule => capsule;
	private bool IsValidGround(in Vector3 normal) => Vector3.Dot(capsule.Up, normal) > groundAllowDot;

	public void Move(Vector3 pMove)
	{
		if (isGrounded)
		{
			float y = pMove.y;
			pMove.y = 0;
			pMove = Util.Horizontalize(pMove, groundNormal, pMove.magnitude);
			pMove.y += y;
		}
		storedMovement += pMove; // Cache until next fixed update
	}

	private void FixedUpdate()
	{
		if (isGrounded)
		{
			verticalVelocity = Mathf.Max(verticalVelocity, 0.0f);
		}
		else
		{
			verticalVelocity += -gravity * Time.fixedDeltaTime;
			storedMovement += new Vector3(0.0f, verticalVelocity, 0.0f);
		}

		transform.position = TryGetMoveResult(storedMovement, transform.position, collisionIterations, layerMask, stepDownHeight, stepUpHeight);
		storedMovement = Vector3.zero;
	}

	private Vector3 TryGetMoveResult(Vector3 pMovement, Vector3 pPosition, int pBounces, LayerMask pLayerMask, float pSnapDownHeight, float pSnapUpHeight)
	{
		Vector3 resultPosition = pPosition;
		Vector3 collisionNormal = groundNormal;
		bool canSnapUp = true;

		while (true)
		{
			if (!capsule.CheckCollisions(pMovement, pPosition, pLayerMask, out resultPosition, out collisionNormal))
			{
				// No Collision
				if (isGrounded && !CheckSnap(pLayerMask, pSnapDownHeight, 0.01f, ref resultPosition, ref collisionNormal))
				{
					isGrounded = false; // Failed snap down
				}
				break; // Exit having not collided
			}
			Vector3 movementToTarget = resultPosition - pPosition;

			// TODO Fix trying to snap up at times that don't make sense!!!!!!!!!
			// Collision but maybe can snap up
			if (canSnapUp && CheckMoveSnap(pMovement, pLayerMask, 0.0f, pSnapUpHeight, ref resultPosition, ref collisionNormal))
			{
				// Snap and continue original movement
				pMovement -= movementToTarget;
				pPosition = resultPosition;
				canSnapUp = false;
				continue;
			}
			canSnapUp = true;

			// Out of bounces
			if (pBounces <= 0)
			{
				isGrounded = IsValidGround(collisionNormal); // Reached ground
				break; // Exit having collided
			}

			// Collision but couldn't snap, then Bounce
			pMovement = Vector3.ProjectOnPlane(pMovement - movementToTarget, collisionNormal);
			pPosition = resultPosition;
			pBounces--;
		}

		if (!isGrounded && CheckSnap(pLayerMask, groundedDistance, 0.01f, ref resultPosition, ref collisionNormal))
		{
			isGrounded = true; // Snap to ground
		}
		groundNormal = isGrounded ? collisionNormal : capsule.Up;
		return resultPosition;
	}

	private bool CheckSnap(LayerMask pLayerMask, float pSnapDownHeight, float pSnapUpHeight, ref Vector3 resultPosition, ref Vector3 collisionNormal)
	{
		if (pSnapDownHeight < 0 && pSnapUpHeight < 0)
		{
			return false;
		}

		Vector3 snapMovement = new Vector3(0.0f, -(pSnapDownHeight + pSnapUpHeight), 0.0f);
		Vector3 snapPosition = resultPosition + new Vector3(0.0f, pSnapUpHeight, 0.0f);

		if (capsule.CheckCollisions(snapMovement, snapPosition, pLayerMask, out Vector3 snapResultPosition, out Vector3 snapCollisionNormal) &&
			(snapResultPosition.y - resultPosition.y) <= pSnapUpHeight && // Block snapping above snap height
			IsValidGround(snapCollisionNormal)) // Only snap to valid ground
		{
			resultPosition = snapResultPosition;
			collisionNormal = snapCollisionNormal;
			return true;
		}
		return false;
	}

	private bool CheckMoveSnap(Vector3 pMovement, LayerMask pLayerMask, float pSnapDownHeight, float pSnapUpHeight, ref Vector3 resultPosition, ref Vector3 collisionNormal)
	{
		float magnitude = pMovement.magnitude;
		pMovement = Util.Horizontalize(pMovement);
		if (magnitude <= Util.NEARZERO)
		{
			return false;
		}

		Vector3 offset = pMovement * (Mathf.Min(capsule.radius, magnitude) + Util.NEARZERO);
		resultPosition += offset;
		bool snapped = CheckSnap(pLayerMask, pSnapDownHeight, pSnapUpHeight, ref resultPosition, ref collisionNormal);
		resultPosition -= offset;
		return snapped;
	}

	private void OnDrawGizmos()
	{
		capsule.DrawGizmos(transform.position);
		Gizmos.DrawLine(transform.position, transform.position - (capsule.Up * groundedDistance));
	}
}