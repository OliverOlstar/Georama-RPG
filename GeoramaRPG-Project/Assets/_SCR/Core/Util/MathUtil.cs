using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher
{
    public static partial class Util
    {
		public const float NEARZERO = 0.0001f;

		public static Vector3 Horizontal(Vector3 pVector) => new Vector3(pVector.x, 0.0f, pVector.z);
		public static Vector3 Horizontal(Vector3 pVector, Vector3 pUp) => Vector3.ProjectOnPlane(pVector, pUp);

		public static Vector3 Horizontalize(Vector3 pVector) => new Vector3(pVector.x, 0.0f, pVector.z).normalized;
		public static Vector3 Horizontalize(Vector3 pVector, float pMagnitude) => new Vector3(pVector.x, 0.0f, pVector.z).normalized * pMagnitude;

		public static Vector3 Horizontalize(Vector3 pVector, Vector3 pUp) => Vector3.ProjectOnPlane(pVector, pUp).normalized;
		public static Vector3 Horizontalize(Vector3 pVector, Vector3 pUp, float pMagnitude) => Vector3.ProjectOnPlane(pVector, pUp).normalized * pMagnitude;

		public static Vector3 Inverse(in Vector3 pVector) => new Vector3(1.0f / pVector.x, 1.0f / pVector.y, 1.0f / pVector.z);

		public static float Clamp(float pValue, Vector2 pClamp) => Mathf.Clamp(pValue, pClamp.x, pClamp.y);
		 
		public static Vector3 Mult(Vector3 pA, Vector3 pB) => new Vector3(pA.x * pB.x, pA.y * pB.y, pA.z * pB.z);
		public static Vector3 Add(Vector3 pA, Vector3 pB) => new Vector3(pA.x + pB.x, pA.y + pB.y, pA.z + pB.z);
		public static Vector3 Add(params Vector3[] pVectors)
		{
			Vector3 vector = Vector3.zero;
			foreach (Vector3 v in pVectors)
			{
				vector = Add(vector, v);
			}
			return vector;
		}
		public static Vector3 Add(IEnumerable<Vector3> pVectors)
		{
			Vector3 vector = Vector3.zero;
			foreach (Vector3 v in pVectors)
			{
				vector = Add(vector, v);
			}
			return vector;
		}

		public static float AddPercents(IEnumerable<float> pValues)
		{
			float value = 1;
			foreach (float v in pValues)
			{
				if (v > 0)
				{
					value += v;
				}
			}
			foreach (float v in pValues)
			{
				if (v < 0)
				{
					value *= Mathf.Clamp01(1 - Mathf.Abs(v));
				}
			}
			return value;
		}

		public static Quaternion Difference(this Quaternion to, Quaternion from) => to * Quaternion.Inverse(from);
		public static Quaternion Add(this Quaternion start, Quaternion diff) => diff * start;

		#region Compare
		public static bool DistanceEqual(Vector3 pA, Vector3 pB, float pDistance) => (pA - pB).sqrMagnitude == Mathf.Pow(pDistance, 2);
		public static bool DistanceGreaterThan(Vector3 pA, Vector3 pB, float pDistance) => (pA - pB).sqrMagnitude > Mathf.Pow(pDistance, 2);
		public static bool DistanceEqualGreaterThan(Vector3 pA, Vector3 pB, float pDistance) => (pA - pB).sqrMagnitude >= Mathf.Pow(pDistance, 2);
		public static bool DistanceLessThan(Vector3 pA, Vector3 pB, float pDistance) => (pA - pB).sqrMagnitude < Mathf.Pow(pDistance, 2);
		public static bool DistanceEqualLessThan(Vector3 pA, Vector3 pB, float pDistance) => (pA - pB).sqrMagnitude <= Mathf.Pow(pDistance, 2);

		public static bool DistanceOnPlaneEqual(Vector3 pA, Vector3 pB, float pDistance, Vector3 pPlaneNormal) =>
			Vector3.ProjectOnPlane(pA - pB, pPlaneNormal).sqrMagnitude == Mathf.Pow(pDistance, 2);
		public static bool DistanceOnPlaneGreaterThan(Vector3 pA, Vector3 pB, float pDistance, Vector3 pPlaneNormal) =>
			Vector3.ProjectOnPlane(pA - pB, pPlaneNormal).sqrMagnitude > Mathf.Pow(pDistance, 2);
		public static bool DistanceOnPlaneEqualGreaterThan(Vector3 pA, Vector3 pB, float pDistance, Vector3 pPlaneNormal) =>
			Vector3.ProjectOnPlane(pA - pB, pPlaneNormal).sqrMagnitude >= Mathf.Pow(pDistance, 2);
		public static bool DistanceOnPlaneLessThan(Vector3 pA, Vector3 pB, float pDistance, Vector3 pPlaneNormal) =>
			Vector3.ProjectOnPlane(pA - pB, pPlaneNormal).sqrMagnitude < Mathf.Pow(pDistance, 2);
		public static bool DistanceOnPlaneEqualLessThan(Vector3 pA, Vector3 pB, float pDistance, Vector3 pPlaneNormal) =>
			Vector3.ProjectOnPlane(pA - pB, pPlaneNormal).sqrMagnitude <= Mathf.Pow(pDistance, 2);

		public static float DistanceOnPlane(Vector3 pA, Vector3 pB, Vector3 pPlaneNormal) => Vector3.ProjectOnPlane(pA - pB, pPlaneNormal).magnitude;
		#endregion Compare
	}
}