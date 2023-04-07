using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher 
{
	public static partial class Util
	{
		public static float Range(Vector2 pRange) => Random.Range(pRange.x, pRange.y);
		public static int Range(Vector2Int pRange) => Random.Range(pRange.x, pRange.y);
		public static float Range(float pRange) => Random.Range(-pRange, pRange);
		public static int Range(int pRange) => Random.Range(-pRange, pRange);

		public static Vector2 GetRandomPointInEllipse(float ellipse_width, float ellipse_height)
		{
			float t = 2 * Mathf.PI * Random.value;
			float u = Random.value + Random.value;
			float r;
			if (u > 1)
			{
				r = 2 - u;
			}
			else
			{
				r = u;
			}
			return new Vector2(ellipse_width * r * Mathf.Cos(t) / 2, ellipse_height * r * Mathf.Sin(t) / 2);
		}
		public static Vector2 GetRandomPointOnCircle(float pRadius) => GetRandomPointInEllipse(pRadius, pRadius);
	}
}