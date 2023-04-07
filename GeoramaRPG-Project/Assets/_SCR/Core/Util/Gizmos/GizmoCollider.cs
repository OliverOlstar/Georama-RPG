using OliverLoescher.Debug2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher
{
	[RequireComponent(typeof(Collider))]
    public class GizmoCollider : GizmoBase
	{
		[SerializeField]
		private bool wireframe = false;

		private new Collider collider = null;

		protected override void DrawGizmos()
		{
			base.DrawGizmos();
			if (collider == null && !TryGetComponent(out collider))
			{
				return;
			}
			Gizmos.matrix = transform.localToWorldMatrix;
			if (wireframe)
			{
				switch (collider)
				{
					case BoxCollider box:
						Gizmos.DrawWireCube(box.center, box.size);
						break;
					case CapsuleCollider capsule:
						Util.GizmoCapsule(capsule.center, capsule.radius, capsule.height);
						break;
					case SphereCollider sphere:
						Gizmos.DrawWireSphere(sphere.center, sphere.radius);
						break;
					case MeshCollider mesh:
						Gizmos.DrawWireMesh(mesh.sharedMesh);
						break;
				}
			}
			else
			{
				switch (collider)
				{
					case BoxCollider box:
						Gizmos.DrawCube(box.center, box.size);
						break;
					case CapsuleCollider capsule:
						Gizmos.DrawMesh(Util.GetPrimitiveMesh(PrimitiveType.Capsule), capsule.center, Quaternion.identity, new Vector3(capsule.radius * 2.0f, capsule.height * 0.5f, capsule.radius * 2.0f));
						break;
					case SphereCollider sphere:
						Gizmos.DrawSphere(sphere.center, sphere.radius);
						break;
					case MeshCollider mesh:
						Gizmos.DrawMesh(mesh.sharedMesh);
						break;
				}
			}
			Gizmos.matrix = Matrix4x4.identity;
		}
	}
}
