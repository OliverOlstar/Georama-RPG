
using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
	public static class WorldUtil
	{
		public static bool IsTransformMyChild(Transform parent, Transform child)
		{
			while (child.parent != null)
			{
				if (child.parent.GetInstanceID() == parent.GetInstanceID())
				{
					return true;
				}
				child = child.parent;
			}
			return false;
		}

		public static float FindGround(Ray worldRay, out Vector3 normal, Transform ignoreCollisions, List<MeshFilter> filters = null)
		{
			normal = Vector3.up;
			float dist = float.MaxValue;
			if (filters == null)
			{
				filters = new List<MeshFilter>(GameObject.FindObjectsOfType<MeshFilter>());
			}
			foreach (MeshFilter filter in filters)
			{
				Mesh mesh = filter.sharedMesh;
				if (mesh == null)
				{
					continue;
				}
				if (ignoreCollisions != null &&
					(filter.transform.GetInstanceID() == ignoreCollisions.GetInstanceID() ||
					 IsTransformMyChild(ignoreCollisions, filter.transform)))
				{
					continue;
				}
				Matrix4x4 worldToLocal = filter.transform.worldToLocalMatrix;
				Ray localRay = new Ray(
					worldToLocal.MultiplyPoint(worldRay.origin),
					worldToLocal.MultiplyVector(worldRay.direction));
				float boundsDist = float.MaxValue;
				if (!mesh.bounds.IntersectRay(localRay, out boundsDist))
				{
					continue;
				}

				int[] tris = mesh.triangles;
				Vector3[] verts = mesh.vertices;
				for (int i = 0; i < tris.Length; i += 3)
				{
					Vector3 p0 = verts[tris[i]];
					Vector3 p1 = verts[tris[i + 1]];
					Vector3 p2 = verts[tris[i + 2]];

					Vector3 v0 = p1 - p0;
					Vector3 v1 = p2 - p0;
					Vector3 p = Vector3.Cross(localRay.direction, v1);
					float det = Vector3.Dot(v0, p);
					if (Core.Util.Approximately(det, 0.0f))
					{
						continue;
					}

					float invDet = 1.0f / det;
					Vector3 t = localRay.origin - p0;
					float u = Vector3.Dot(t, p) * invDet;
					if (u < 0.0f || u > 1.0f + Core.Util.LOW_PRECISION_EPSILON)
					{
						continue;
					}

					Vector3 q = Vector3.Cross(t, v0);
					float v = Vector3.Dot(localRay.direction, q) * invDet;
					if (v < 0.0f || u + v > 1.0f + Core.Util.LOW_PRECISION_EPSILON)
					{
						continue;
					}

					float w = Vector3.Dot(v1, q) * invDet;
					if (w < Core.Util.LOW_PRECISION_EPSILON)
					{
						continue;
					}

					Matrix4x4 localToWorld = filter.transform.localToWorldMatrix;
					float newDist = localToWorld.MultiplyVector(w * localRay.direction).magnitude;
					if (newDist < dist)
					{
						normal = Vector3.Normalize(localToWorld.MultiplyVector(Vector3.Cross(p1 - p0, p2 - p0)));
						dist = newDist;
					}
				}
			}

			return dist;
		}

		[MenuItem("Core/World Util/Snap Position To Ground &h")] // alt + g
		static void SnapPositionToGround()
		{
			Transform transform = Selection.activeTransform;
			if (transform == null)
			{
				return;
			}
			Ray worldRay = new Ray(transform.position + 0.5f * Vector3.up, Vector3.down);
			Vector3 normal = Vector3.up;
			float dist = FindGround(worldRay, out normal, transform);
			if (dist > 10.0f)
			{
				return;
			}
			Undo.RecordObject(transform, "Snap Position To Ground");
			transform.position = worldRay.origin + dist * worldRay.direction;
		}

		[MenuItem("Core/World Util/Snap Rotation To Ground &#h")] // alt + shft + h
		static void SnapRotationToGround()
		{
			Transform transform = Selection.activeTransform;
			if (transform == null)
			{
				return;
			}
			Ray worldRay = new Ray(transform.position + 0.5f * Vector3.up, Vector3.down);
			Vector3 normal = Vector3.up;
			float dist = FindGround(worldRay, out normal, transform);
			if (dist > 10.0f)
			{
				return;
			}
			Undo.RecordObject(transform, "Snap Rotation To Ground");
			transform.rotation = Quaternion.FromToRotation(transform.up, normal) * transform.rotation;
			transform.position = worldRay.origin + dist * worldRay.direction;
		}

		[MenuItem("Core/World Util/Snap To Camera Look At %&h")] // ctrl + alt + h
		static void SnapToLookAt()
		{
			Transform transform = Selection.activeTransform;
			if (transform == null)
			{
				return;
			}
			Transform cameraTransform = SceneView.lastActiveSceneView.camera.transform;
			Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
			Vector3 normal = Vector3.up;
			float dist = FindGround(ray, out normal, transform);
			if (dist > 100.0f)
			{
				return;
			}
			Undo.RecordObject(transform, "Snap To Camera Look At");
			transform.position = ray.origin + dist * ray.direction;
		}
	}
}
