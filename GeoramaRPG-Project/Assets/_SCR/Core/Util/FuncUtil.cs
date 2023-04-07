using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher 
{
	public static partial class Util
	{
		#region Application
		public static bool IsApplicationQuitting = false;
		static void Quit() => IsApplicationQuitting = true;

		[RuntimeInitializeOnLoadMethod]
		static void RunOnStart() => Application.quitting += Quit;
		#endregion Application

		public static float SmoothStep(float pMin, float pMax, float pIn)
		{
			return Mathf.Clamp01((pIn - pMin) / (pMax - pMin));
		}
		public static float SmoothStep(Vector2 pMinMax, float pIn) => SmoothStep(pMinMax.x, pMinMax.y, pIn);

		public static float SafeAngle(float pAngle)
		{
			if (pAngle > 180)
			{
				pAngle -= 360;
			}
			return pAngle;
		}

		public static bool TryGetAndRemove<TKey, TValue>(ref Dictionary<TKey, TValue> pDictionary, TKey pKey, out TValue pValue)
		{
			if (pDictionary.TryGetValue(pKey, out pValue) && pDictionary.Remove(pKey))
			{
				return true;
			}
			return false;
		}

		public static TValue GetAndRemove<TKey, TValue>(ref Dictionary<TKey, TValue> pDictionary, TKey pKey)
		{
			pDictionary.TryGetValue(pKey, out TValue pValue);
			pDictionary.Remove(pKey);
			return pValue;
		}
	}
}