using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher.Debug2
{
	public abstract class GizmoBase : MonoBehaviour
	{
		[SerializeField] private Color color = new Color(0, 0.5f, 1, 1);
		[SerializeField] private bool alwaysShow = false;

		private void Awake()
		{
			if (!Application.isEditor)
			{
				Debug.LogWarning($"This {GetType()} exist, destory it. Please clean these up. {Util.GetPath(transform)}");
				DestroyImmediate(this);
			}
		}

		protected virtual void OnDrawGizmos()
		{
			if (alwaysShow == true)
			{
				DrawGizmos();
			}
		}

		protected virtual void OnDrawGizmosSelected()
		{
			if (alwaysShow == false)
			{
				DrawGizmos();
			}
		}

		protected virtual void DrawGizmos()
		{
			Gizmos.color = color;
		}
	}
}