using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class CharacterOnGround : CharacterBehaviour
{
	public enum State
	{
		Airborne,
		Grounded,
		Sliding
	}

	[System.Serializable]
	private class SphereCast
	{
		[SerializeField]
		private Vector3 center = new Vector3();
		[SerializeField]
		private float radius = 0.5f;
		[SerializeField]
		private float distance = 1.0f;

		public RaycastHit m_HitInfo = new RaycastHit();

		public RaycastHit Hit => m_HitInfo;

		public bool Check(Transform pTransform, LayerMask pLayerMask)
		{
			Vector3 origin = pTransform.TransformPoint(center);
			return Physics.SphereCast(origin, radius, -pTransform.up, out m_HitInfo, distance, pLayerMask);
		}

		public void OnDrawGizmos(Transform pTransform, LayerMask pLayerMask)
		{
			Vector3 origin = pTransform.TransformPoint(center);
			Vector3 end = origin - (pTransform.up * distance);
			Color color = Check(pTransform, pLayerMask) ? Color.green : Color.red;
			Core.DebugUtil.DrawCapsule(origin, end, radius, color);
		}
	}

	[SerializeField]
	private SphereCast[] m_Spheres = new SphereCast[1];
	[SerializeField]
	private LayerMask m_LayerMask = new LayerMask();
	[SerializeField, Range(0.0f, 180.0f)]
	private float m_SlopeLimit = 45.0f;
	[SerializeField]
	private bool m_FollowGround = false;
	[SerializeField]
	private float m_DisconnectVelocityScalar = 10.0f;

	[FoldoutGroup("Events")]
	public UnityEvent<State> OnStateChanged;

	[SerializeField, HideInEditorMode, DisableInPlayMode]
	private State m_State = State.Airborne;
	private Transform m_FollowTransform = null;
	private Vector3 m_FollowPosition = Vector3.zero;
	private bool m_IsFollowingGround = false;

	public bool IsGrounded => m_State == State.Grounded;
	public bool IsSliding => m_State == State.Sliding;
	public bool IsAirborne => m_State == State.Airborne;
	public State GroundedState => m_State;

	protected override void OnInitalize()
	{
		m_FollowTransform = new GameObject($"{gameObject.name}-GroundFollower").transform;
	}

	protected override void Tick(float pDeltaTime)
	{
		State state = CalculateGrounded();
		if (state != m_State)
		{
			m_State = state;
			OnStateChanged.Invoke(m_State);
		}
		FollowGround();
	}

	private State CalculateGrounded()
	{
		if (Character.Controller.Velocity.y > 0.0f)
		{
			return State.Airborne;
		}
		// Find Ground
		foreach (SphereCast sphere in m_Spheres)
		{
			if (!sphere.Check(transform, m_LayerMask))
			{
				continue;
			}
			// Check Angle
			if (Vector3.Angle(GetAverageNormal(), Vector3.up) > m_SlopeLimit) // Found Ground
			{
				return State.Sliding;
			}
			return State.Grounded;
		}

		return State.Airborne;
	}

	private void StartFollowingGround(Transform pTransform, Vector3 pPoint)
	{
		m_FollowTransform.SetParent(pTransform);
		m_FollowTransform.position = pPoint;
		m_FollowPosition = pPoint;
		m_IsFollowingGround = true;
	}

	private void StopFollowingGround()
	{
		m_FollowTransform.SetParent(null);
		m_IsFollowingGround = false;

		Vector3 displacement = m_FollowTransform.position - m_FollowPosition;
		Character.Controller.AddVelocity(displacement * Time.fixedDeltaTime * m_DisconnectVelocityScalar);
	}

	private void FollowGround()
	{
		if (m_State == State.Airborne || !TryGetNonStaticGroundTransform(out Transform transform, out Vector3 point))
		{
			if (m_IsFollowingGround)
			{
				StopFollowingGround();
			}
			return;
		}

		if (!m_IsFollowingGround)
		{
			StartFollowingGround(transform, point);
		}
		Vector3 displacement = m_FollowTransform.position - m_FollowPosition;
		Character.Controller.Move(displacement);
		StartFollowingGround(transform, point);
	}

	public Vector3 GetAverageNormal()
	{
		if (m_Spheres.Length == 1)
		{
			return m_Spheres[0].Hit.normal;
		}

		Vector3 total = Vector3.zero;
		foreach (SphereCast sphere in m_Spheres)
		{
			total += sphere.Hit.normal;
		}
		return total / m_Spheres.Length;
	}

	public Vector3 GetGroundPoint()
	{
		bool first = true;
		Vector3 point = Vector3.zero;
		foreach (SphereCast sphere in m_Spheres)
		{
			if (first || sphere.Hit.point.y > point.y)
			{
				point = sphere.Hit.point;
			}
			first = false;
		}
		return point;
	}

	public bool TryGetNonStaticGroundTransform(out Transform pTransform, out Vector3 pPoint)
	{
		foreach (SphereCast sphere in m_Spheres)
		{
			if (sphere.Hit.transform != null && !sphere.Hit.transform.gameObject.isStatic)
			{
				pTransform = sphere.Hit.transform;
				pPoint = sphere.Hit.point;
				return true;
			}
		}
		pTransform = null;
		pPoint = Vector3.zero;
		return false;
	}

	private void OnDrawGizmosSelected()
	{
		foreach (SphereCast line in m_Spheres)
		{
			line.OnDrawGizmos(transform, m_LayerMask);
		}
	}
}
