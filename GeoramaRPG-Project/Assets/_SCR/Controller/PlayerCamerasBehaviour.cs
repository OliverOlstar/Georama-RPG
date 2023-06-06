using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverLoescher;

public class PlayerCamerasBehaviour : CharacterBehaviour
{
	[SerializeField]
	private ThirdPersonCamera m_DefaultCamera = null;
	[SerializeField]
	private ThirdPersonTargetingCamera m_LockOnCamera = null;

	protected override void OnEnabled()
	{
		Character.GetBehaviour<CharacterInteractions>().OnTargetChanged.AddListener(OnTargetChanged);
		OnTargetChanged(null);
	}

	protected override void OnDisabled()
	{
		Character.GetBehaviour<CharacterInteractions>().OnTargetChanged.RemoveListener(OnTargetChanged);
	}

	protected override void Tick(float pDeltaTime) { }

	private void OnTargetChanged(ITargetable target)
	{
		bool hasTarget = target != null;
		if (hasTarget)
		{
			m_LockOnCamera.transform.forward = m_DefaultCamera.transform.forward;
			m_LockOnCamera.target = target.Transform;
		}
		else
		{
			m_DefaultCamera.transform.forward = m_LockOnCamera.transform.forward;
			m_LockOnCamera.target = null;
		}
		m_DefaultCamera.gameObject.SetActive(!hasTarget);
		m_LockOnCamera.gameObject.SetActive(hasTarget);
	}
}
