using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace OliverLoescher
{
	public static partial class Util
	{
		#region Logs
		public static void DevException<T>(T pException) where T : Exception
		{
#if RELEASE
			Debug.LogException(exception.Message);
#else
			throw pException;
#endif
		}

		public static void DevException(string pFormattedString)
		{
#if RELEASE
			Debug.LogException(new System.InvalidOperationException(pFormattedString));
#else
			throw new System.InvalidOperationException(pFormattedString);
#endif
		}

		public static void DevException(string pFormattedString, UnityEngine.Object pObject)
		{
#if RELEASE
			Debug.LogException(new System.InvalidOperationException(pFormattedString), pObject);
#else
			throw new System.InvalidOperationException(pFormattedString);
#endif
		}

		public static void Log<TKey, TValue>(string pMessage, Dictionary<TKey, TValue> pDictionary)
		{
			string message = string.Empty;
			foreach (KeyValuePair<TKey, TValue> value in pDictionary)
			{
				message += $"{value.Key}: {value.Value}, ";
			}
			Debug.Log($"{pMessage} [{message.Remove(message.Length - 2, 2)}]");
		}

		public static void Log<TValue>(string pMessage, IEnumerable<TValue> pValues)
		{
			string message = string.Empty;
			foreach (TValue value in pValues)
			{
				message += $"{value}, ";
			}
			Debug.Log($"{pMessage} [{message.Remove(message.Length - 2, 2)}]");
		}

		public static void Log<TValue>(string pMessage, TValue[,] pValues)
		{
			string message = string.Empty;
			int xLength = pValues.GetLength(0);
			int yLength = pValues.GetLength(1);
			for (int x = 0; x < xLength; x++)
			{
				message += "{ ";
				for (int y = 0; y < yLength; y++)
				{
					if (y == yLength - 1)
					{
						message += $"{pValues[x, y]}" + "}";
					}
					else
					{
						message += $"{pValues[x, y]}, ";
					}
				}
				if (x < xLength - 1)
				{
					message += ", ";
				}
			}
			Debug.Log(pMessage + " { " + message + " }");
		}
		#endregion Logs

		public static string GetPath(Transform transform)
		{
			if (transform.parent == null)
			{
				return transform.name;
			}
			return $"{transform.name}/{GetPath(transform.parent)}";
		}

		#region Gizmos
		public static void GizmoCapsule(Vector3 pVectorA, Vector3 pVectorB, float pRadius)
		{
			Gizmos.DrawWireSphere(pVectorA, pRadius);
			Gizmos.DrawLine(pVectorA + (Vector3.forward * pRadius), pVectorB + (Vector3.forward * pRadius));
			Gizmos.DrawLine(pVectorA + (Vector3.left * pRadius), pVectorB + (Vector3.left * pRadius));
			Gizmos.DrawLine(pVectorA + (Vector3.right * pRadius), pVectorB + (Vector3.right * pRadius));
			Gizmos.DrawLine(pVectorA + (Vector3.back * pRadius), pVectorB + (Vector3.back * pRadius));
			Gizmos.DrawWireSphere(pVectorB, pRadius);
		}
		public static void GizmoCapsule(Vector3 pCenter, float pRadius, float pHeight)
		{
			pHeight -= pRadius * 2.0f;
			if (pHeight <= 0)
			{
				Gizmos.DrawWireSphere(pCenter, pRadius);
				return;
			}
			Vector3 top = pCenter + (Vector3.up * pHeight * 0.5f);
			Vector3 bottem = pCenter + (Vector3.down * pHeight * 0.5f);
			GizmoCapsule(top, bottem, pRadius);
		}
		public static void GizmoCapsule(Vector3 pVectorA, Vector3 pVectorB, float pRadius, Matrix4x4 pMatrix)
		{
			Gizmos.matrix = pMatrix;
			GizmoCapsule(pVectorA, pVectorB, pRadius);
			Gizmos.matrix = Matrix4x4.identity;
		}
		public static void GizmoCapsule(Vector3 pCenter, float pRadius, float pHeight, Matrix4x4 pMatrix)
		{
			Gizmos.matrix = pMatrix;
			GizmoCapsule(pCenter, pRadius, pHeight);
			Gizmos.matrix = Matrix4x4.identity;
		}
		#endregion Gizmos
	}
}
