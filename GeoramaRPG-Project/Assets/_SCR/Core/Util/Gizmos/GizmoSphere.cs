using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher.Debug2
{
	public class GizmoSphere : GizmoBase
	{
		[SerializeField, Min(Util.NEARZERO)] private float radius = 1;

		protected override void DrawGizmos()
		{
			base.DrawGizmos();

			Gizmos.DrawWireSphere(transform.position, radius);
		}
	}
}