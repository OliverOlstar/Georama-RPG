using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher.Debug2
{
	public class GizmoForward : GizmoBase
	{
		[SerializeField] private float magnitude = 1;

		protected override void DrawGizmos()
		{
			base.DrawGizmos();

			Gizmos.DrawLine(transform.position, transform.position + (transform.forward * magnitude));
		}
	}
}