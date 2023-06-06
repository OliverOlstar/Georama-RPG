using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using OliverLoescher;

public class CharacterInteractions : CharacterBehaviour
{
	[Header("Targeting"), SerializeField]
	private TargetableManager.Team m_Team = TargetableManager.Team.NotPlayer;
	[SerializeField, Min(Util.NEARZERO)]
	private float m_Radius = 5.0f;
	[SerializeField]
	private Vector3 m_Offset = Vector3.zero;
	[SerializeField, Min(0.1f)]
	private float m_MaxDistance = 5.0f;
	[SerializeField]
	private LayerMask m_TerrianLayer = new LayerMask();

	public UnityEvent<ITargetable> OnTargetChanged = new UnityEvent<ITargetable>();

	private ITargetable m_CurrentTarget = null;
	private TargetableManager.Context m_Context = new TargetableManager.Context();
	

	protected override void OnEnabled()
	{
		Character.Input.LockOn.onPerformed.AddListener(OnLockOnPerformed);
		Character.Input.LockOn.onCanceled.AddListener(OnLockOnCanceled);
	}

	protected override void OnDisabled()
	{
		Character.Input.LockOn.onPerformed.RemoveListener(OnLockOnPerformed);
		Character.Input.LockOn.onCanceled.RemoveListener(OnLockOnCanceled);
	}

	public void GetTargetables(ref List<ITargetable> result)
	{
		Transform camera = MainCamera.Camera.transform;
		m_Context.Team = m_Team;
		m_Context.Point = camera.position + camera.TransformVector(m_Offset);
		m_Context.Radius = m_Radius;
		Core.Director.GetOrCreate<TargetableManager>().Query(ref result, m_Context);
	}

	protected override void Tick(float pDeltaTime)
	{
		if (m_CurrentTarget != null)
		{
			if (Util.DistanceGreaterThan(m_CurrentTarget.Transform.position, transform.position, m_MaxDistance) || !IsPointVisible(m_CurrentTarget.Transform.position))
			{
				m_CurrentTarget = null;
				Character.Input.ClearLockOnIfIsToggle();
				OnTargetChanged.Invoke(m_CurrentTarget);
			}
		}
	}

	private void OnLockOnPerformed()
	{
		List<ITargetable> targets = Core.ListPool<ITargetable>.Request();
		GetTargetables(ref targets);
		m_CurrentTarget = null;
		float highestScore = int.MinValue;
		foreach (ITargetable target in targets)
		{
			if (!IsPointVisible(target.Transform.position))
			{
				continue;
			}
			float score = PointScore(target.Transform.position);
			if (score > highestScore)
			{
				m_CurrentTarget = target;
				highestScore = score;
			}
		}
		if (m_CurrentTarget != null)
		{
			OnTargetChanged.Invoke(m_CurrentTarget);
		}
		Core.ListPool<ITargetable>.Return(targets);
	}

	private void OnLockOnCanceled()
	{
		if (m_CurrentTarget != null)
		{
			m_CurrentTarget = null;
			OnTargetChanged.Invoke(m_CurrentTarget);
		}
	}

	private bool IsPointVisible(Vector3 pPoint)
	{
		Vector3 position = MainCamera.Camera.WorldToViewportPoint(pPoint);
		if (!(position.z > 0 && position.x > 0 && position.x < 1 && position.y > 0 && position.y < 1))
		{
			return false; // Not on screen
		}
		if (Physics.Linecast(MainCamera.Camera.transform.position, pPoint, m_TerrianLayer))
		{
			return false; // Is occluded
		}
		return true;
	}

	private float PointScore(Vector3 pPoint)
	{
		Transform camera = MainCamera.Camera.transform;
		Vector3 direction = pPoint - camera.position;
		float distance = direction.sqrMagnitude;
		float angle = Vector3.Angle(camera.forward, direction);
		// Debug.Log($"Score - Distance {distance}, Angle {angle}");
		return -(distance * 0.01f) + -angle;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		Transform camera = MainCamera.Camera ? MainCamera.Camera.transform : Camera.main.transform;
		Gizmos.DrawWireSphere(camera.position + camera.TransformVector(m_Offset), m_Radius);
		Gizmos.color = Color.white;
	}
}
