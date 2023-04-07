
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
	public sealed class Director
	{
		private static Dictionary<Type, IDirector> s_Directors;

		static Director()
		{
			s_Directors = new Dictionary<Type, IDirector>();
		}

		[RuntimeInitializeOnLoadMethod]
		private static void Initialize()
		{
			foreach (Type directorType in TypeUtility.GetTypesImplementingInterface(typeof(IDirector)))
			{
				if (directorType.IsAbstract || directorType.IsGenericType)
				{
					continue;
				}
				if (!directorType.IsDefined(typeof(AutoDirectorAttribute), true))
				{
					continue;
				}
				if (s_Directors.ContainsKey(directorType))
				{
					continue;
				}
				CreateInternal(directorType);
			}
		}

		public static T Get<T>() where T : class, IDirector
		{
			TryGet(out T director);
			return director;
		}

		public static void GetAll<T>(List<T> directors) where T : IDirector
		{
			foreach (IDirector director in s_Directors.Values)
			{
				if (director is T directorOfType)
				{
					directors.Add(directorOfType);
				}
			}
		}

		public static bool TryGet<T>(out T director) where T : class, IDirector
		{
			bool result = s_Directors.TryGetValue(typeof(T), out IDirector value);
			director = value as T;
			return result;
		}

		public static bool TryGetOfType<T>(out T director) where T : class, IDirector
		{
			foreach (IDirector d in s_Directors.Values)
			{
				director = d as T;
				if (director != null)
				{
					return true;
				}
			}
			director = null;
			return false;
		}

		public static void Create(Type directorType)
		{
			if (s_Directors.ContainsKey(directorType))
			{
				return;
			}
			CreateInternal(directorType);
		}

		public static void Create<T>() where T : class, IDirector
		{
			bool exists = TryGet<T>(out _);
			if (exists)
			{
				return;
			}
			CreateInternal(typeof(T));
		}

		public static T GetOrCreate<T>() where T : class, IDirector
		{
			if (TryGet(out T director))
			{
				return director;
			}
			else
			{
				return CreateInternal(typeof(T)) as T;
			}
		}

		public static IDirector GetOrCreate(Type directorType)
		{
			if (s_Directors.TryGetValue(directorType, out IDirector director))
			{
				return director;
			}
			else
			{
				return CreateInternal(directorType);
			}
		}

		public static void GetOrCreateOfType<T>(List<T> directors) where T : class, IDirector
		{
			GetOrCreateOfTypeWithPredicate(directors, True);
		}

		public static void GetOrCreateOfTypeWithPredicate<T>(List<T> directors, Predicate<Type> predicate) where T : class, IDirector
		{
			directors = directors ?? throw new ArgumentException("directors must not be null", "directors");
			directors.Clear();
			foreach (Type directorType in TypeUtility.GetTypesImplementingInterfaceOrDerivedFrom(typeof(T)))
			{
				if (directorType.IsAbstract || directorType.IsGenericType)
				{
					continue;
				}
				if (!predicate?.Invoke(directorType) ?? false)
				{
					continue;
				}
				T director = GetOrCreate(directorType) as T;
				directors.Add(director);
			}
		}

		public static void GetMatching(List<IDirector> directors, Predicate<IDirector> predicate)
		{
			directors = directors ?? throw new ArgumentNullException(nameof(directors));
			directors.Clear();
			foreach (KeyValuePair<Type, IDirector> kvp in s_Directors)
			{
				if (predicate?.Invoke(kvp.Value) ?? false)
				{
					directors.Add(kvp.Value);
				}
			}
		}

		public static bool Exists<T>() where T : IDirector
		{
			return s_Directors.ContainsKey(typeof(T));
		}

		public static bool Destroy<T>() where T : class, IDirector
		{
			if (!TryGet(out T director))
			{
				return false;
			}
			director.OnDestroy();
			s_Directors.Remove(typeof(T));
			return true;
		}

		public static void DestroyAll()
		{
			foreach (IDirector director in s_Directors.Values)
			{
				director.OnDestroy();
			}
			s_Directors.Clear();
		}

		public static void DestroyPersistent()
		{
			DestroyMatching(IsPersistent);
		}

		public static void DestroyTransient()
		{
			DestroyMatching(IsTransient);
		}

		public static void DestroyMatching(Predicate<IDirector> predicate)
		{
			if (predicate == null)
			{
				return;
			}
			List<Type> keys = new List<Type>();
			foreach (KeyValuePair<Type, IDirector> kvp in s_Directors)
			{
				if (predicate.Invoke(kvp.Value))
				{
					keys.Add(kvp.Key);
				}
			}
			for (int i = 0; i < keys.Count; ++i)
			{
				s_Directors[keys[i]].OnDestroy();
				s_Directors.Remove(keys[i]);
			}
		}

		private static IDirector CreateInternal(Type directorType)
		{
			if (!typeof(IDirector).IsAssignableFrom(directorType))
			{
				return null;
			}
			IDirector director = (IDirector)Activator.CreateInstance(directorType);
			s_Directors[directorType] = director;
			director.OnCreate();
			return director;
		}

		public static bool IsPersistent(IDirector director)
		{
			return director.GetType().IsDefined(typeof(PersistentDirectorAttribute), true);
		}

		public static bool IsTransient(IDirector director)
		{
			return !IsPersistent(director);
		}

		private static bool True(Type directorType)
		{
			return true;
		}

		private Director()
		{
		}
	}
}
