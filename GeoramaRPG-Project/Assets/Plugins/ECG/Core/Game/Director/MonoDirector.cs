
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
	public static class MonoDirector
	{
		private static Dictionary<System.Type, HashSet<UnityEngine.Object>> s_Directors = new Dictionary<System.Type, HashSet<Object>>();

		public static void Register(UnityEngine.Object obj)
		{
			System.Type key = obj.GetType();
			if (!s_Directors.TryGetValue(key, out HashSet<UnityEngine.Object> set))
			{
				set = new HashSet<UnityEngine.Object>();
				s_Directors.Add(key, set);
			}
			if (!set.Contains(obj))
			{
				set.Add(obj);
			}
		}

		public static void Deregister(UnityEngine.Object obj)
		{
			System.Type key = obj.GetType();
			if (!s_Directors.TryGetValue(key, out HashSet<UnityEngine.Object> set))
			{
				return;
			}
			if (!set.Contains(obj))
			{
				return;
			}
			set.Remove(obj);
		}

		public static bool TryGet<T>(out T obj) where T : UnityEngine.Object
		{
			obj = null;
			if (s_Directors.TryGetValue(typeof(T), out HashSet<UnityEngine.Object> set))
			{
				foreach (UnityEngine.Object i in set)
				{
					obj = i as T;
					if (obj != null)
					{
						return true;
					}
				}
			}
			return obj != null;
		}

		public static T Get<T>() where T : UnityEngine.Object
		{
			return TryGet(out T obj) ? obj : throw new System.ArgumentNullException($"Core.MonoDirector.Get() No director of type {typeof(T).Name} exists");
		}

		public static void Get<T>(List<T> list) where T : UnityEngine.Object
		{
			if (s_Directors.TryGetValue(typeof(T), out HashSet<UnityEngine.Object> set))
			{
				foreach (UnityEngine.Object obj in set)
				{
					if (obj is T objOfType)
					{
						list.Add(objOfType);
					}
				}
			}
		}

		public static bool TryGetRecursive<T>(out T obj) where T : UnityEngine.Object
		{
			obj = null;
			System.Type key = typeof(T);
			foreach (KeyValuePair<System.Type, HashSet<UnityEngine.Object>> pair in s_Directors)
			{
				if (key.IsAssignableFrom(pair.Key))
				{
					obj = pair.Value.GetEnumerator().Current as T;
					if (obj != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static T GetRecursive<T>() where T : UnityEngine.Object
		{
			return TryGetRecursive(out T obj) ? obj : throw new System.ArgumentNullException($"Core.GetRecursive.Get() No director assignable from type {typeof(T).Name} exists");
		}

		public static void GetRecursive<T>(List<T> list) where T : UnityEngine.Object
		{
			System.Type key = typeof(T);
			foreach (KeyValuePair<System.Type, HashSet<UnityEngine.Object>> pair in s_Directors)
			{
				if (key.IsAssignableFrom(pair.Key))
				{
					foreach (UnityEngine.Object obj in pair.Value)
					{
						if (obj is T objOfType)
						{
							list.Add(objOfType);
						}
					}
				}
			}
		}
	}
}

