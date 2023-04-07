using System;
using UnityEngine;


namespace OliverLoescher
{
	public abstract class Singleton<T> where T : class, new()
	{
		private static T _Instance = null;
		private static ISingleton _InstanceInterface = null;

		public static T Instance
		{
			get
			{
				// Create
				if (_Instance == null)
				{
					_Instance = new T();
					_InstanceInterface = (_Instance is ISingleton i) ? i : null;
				}
				// Access
				if (_InstanceInterface != null)
				{
					_InstanceInterface.OnAccessed();
				}
				return _Instance;
			}
		}

		protected static void Log(string pMessage) => Debug.Log($"[{typeof(T).Name}] {pMessage}");
		protected static void LogWarning(string pMessage) => Debug.LogWarning($"[{typeof(T).Name}] {pMessage}");
		protected static void LogError(string pMessage) => Debug.LogError($"[{typeof(T).Name}] {pMessage}");
		protected static void LogExeception(string pMessage) => Debug.LogException(new System.Exception($"[{typeof(T).Name}] {pMessage}"));
	}
}