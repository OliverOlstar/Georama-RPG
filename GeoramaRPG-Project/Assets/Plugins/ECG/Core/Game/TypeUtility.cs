using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;

namespace Core
{
	public static class TypeUtility
	{
		public delegate bool Predicate(Type type, Type matchingType);

		private static List<Type> s_Types;

		[RuntimeInitializeOnLoadMethod]
		static void Initialize()
		{
			// This is just here to trigger the static ctor on init instead 
			// of lazy loading it the first time this class is used
			// Please do not remove this method -- Josh M. 
		}

		static TypeUtility()
		{
			s_Types = new List<Type>();
			System.Reflection.Assembly[] assemblies = null; ;
			if (AppDomain.CurrentDomain == null)
			{
				Debug.LogError("TypeUtility CurrentDomaing is null");
				return;
			}
			try
			{
				assemblies = AppDomain.CurrentDomain.GetAssemblies();
			}
			catch(Exception e)
			{
				Core.DebugUtil.DevException($"TypeUtility constructor had exception: {e} could not get assemblies");
			}
			if (assemblies == null)
			{
				return;
			}
			foreach (System.Reflection.Assembly assembly in assemblies)
			{
				if (assembly == null)
				{
					Debug.LogError("TypeUtility contains a null assembly");
					continue;
				}
				try
				{
					s_Types.AddRange(assembly.GetTypes());
				}
				catch (ReflectionTypeLoadException e)
				{
					int initalCount = s_Types.Count;
					s_Types.AddRange(e.Types.Where(t => t != null));
					Debug.LogError($"TypeUtility constructor had exception on the assembly '{assembly.FullName}'. " +
						$"Things happen I guess, we tried to recover. We gather {s_Types.Count - initalCount} types from the assembly out of the possible {e.Types.Length}\n{e}");
				}
			}
		}

		public static Type GetTypeByName(string typeName)
		{
			if (string.IsNullOrEmpty(typeName))
			{
				return null;
			}
			for (int i = 0; i < s_Types.Count; ++i)
			{
				if (s_Types[i].Name == typeName)
				{
					return s_Types[i];
				}
			}
			return null;
		}

		public static IEnumerable<Type> GetAllTypes()
		{
			for (int i = 0; i < s_Types.Count; ++i)
			{
				yield return s_Types[i];
			}
		}

		public static IEnumerable<Type> GetMatchingTypes(Type typeToMatch, Predicate predicate)
		{
			for (int i = 0; i < s_Types.Count; ++i)
			{
				if (predicate.Invoke(s_Types[i], typeToMatch))
				{
					yield return s_Types[i];
				}
			}
		}

		public static IEnumerable<Type> GetTypesImplementingInterface(Type interfaceType)
		{
			return GetMatchingTypes(interfaceType, ImplementsInterface);
		}

		public static IEnumerable<Type> GetTypesDerivedFrom(Type baseType)
		{
			return GetMatchingTypes(baseType, IsDerivedFrom);
		}

		public static IEnumerable<Type> GetTypesImplementingInterfaceOrDerivedFrom(Type type)
		{
			return GetMatchingTypes(type, ImplementsInterfaceOrIsDerivedFrom);
		}

		public static IEnumerable<Type> GetTypesWithAttribute(Type attributeType)
		{
			return GetMatchingTypes(attributeType, HasAttribute);
		}

		public static bool ImplementsInterface(Type type, Type interfaceType)
		{
			return interfaceType.IsInterface && !type.IsAbstract && interfaceType.IsAssignableFrom(type);
		}

		public static bool IsDerivedFrom(Type type, Type baseType)
		{
			return type.IsSubclassOf(baseType);
		}

		public static bool HasAttribute(Type type, Type attributeType)
		{
			if (!attributeType.IsSubclassOf(typeof(Attribute)))
			{
				return false;
			}
			return type.IsDefined(attributeType, false);
		}

		private static bool ImplementsInterfaceOrIsDerivedFrom(Type type, Type compareType)
		{
			return ImplementsInterface(type, compareType) || IsDerivedFrom(type, compareType);
		}
	}
}
