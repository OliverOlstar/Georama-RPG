using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher
{
	public interface IPrimitive
	{
		public abstract bool PointIntersects(Vector3 pPoint, Vector3 pPosition);
		public abstract bool CheckCollisions(Vector3 pMovement, Vector3 pPosition, int pBounces, LayerMask pLayerMask, out Vector3 resultPosition, out Vector3 collisionNormal);

		public abstract void DrawGizmos(Vector3 pPosition);
	}
}
