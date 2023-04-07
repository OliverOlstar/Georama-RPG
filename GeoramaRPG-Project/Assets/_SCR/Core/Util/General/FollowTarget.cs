using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher
{
	public class FollowTarget : MonoBehaviour
	{
		[SerializeField]
		private MonoUtil.Updateable updateable = new MonoUtil.Updateable(MonoUtil.UpdateType.Default, MonoUtil.Priorities.Camera);

		[Header("Position")]
		public Transform posTarget = null;
		public Vector3 posOffset = new Vector3();
		[Min(0)] public float posDampening = 0.0f;

		[Header("Rotation")]
		public Transform rotTarget = null;
		public Vector3 rotOffset = new Vector3();
		[Min(0)] public float rotDampening = 0.0f;

		private void Start()
		{
			updateable.Register(Tick);
		}

		private void OnDestroy()
		{
			updateable.Deregister();
		}

		private void Tick(float pDeltaTime) 
		{
			if (posTarget != null)
			{
				Vector3 pos = posTarget.position + posOffset;
				if (posDampening == 0.0f)
				{
					transform.position = pos;
				}
				else
				{
					transform.position = Vector3.Lerp(transform.position, pos, pDeltaTime * posDampening);
				}
			}
			
			if (rotTarget != null)
			{
				Quaternion rot = rotTarget.rotation * Quaternion.Euler(posOffset);
				if (rotDampening == 0.0f)
				{
					transform.rotation = rot;
				}
				else
				{
					transform.rotation = Quaternion.Lerp(transform.rotation, rot, pDeltaTime * rotDampening);
				}
			}
		}

		private void OnDrawGizmosSelected()
		{
			if (Application.isPlaying)
			{
				return;
			}

			if (posTarget != null)
			{
				transform.position = posTarget.position + posOffset;
			}
			if (rotTarget != null)
			{
				transform.rotation = rotTarget.rotation * Quaternion.Euler(posOffset);
			}
		}
	}
}