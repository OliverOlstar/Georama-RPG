using System;

public static class TypeExtensionMethods
{
	public static bool Is(this Type type, Type other)
	{
		if (type == null || other == null)
		{
			return false;
		}
		return other == type ||
			   other.IsSubclassOf(type) ||
			   other.IsAssignableFrom(type);
	}

	public static bool Is<T>(this Type type)
	{
		return Is(type, typeof(T));
	}

	public static bool TryGetCustomAttribute<T>(this Type type, out T attribute)
		where T : Attribute
	{
		type = type ?? throw new ArgumentNullException(nameof(type));
		object[] attributes = type.GetCustomAttributes(typeof(T), false);
		if (attributes.Length <= 0)
		{
			attribute = null;
			return false;
		}
		attribute = attributes[0] as T;
		return attribute != null;
	}

	public static T GetCustomAttribute<T>(this Type type) 
		where T : Attribute
	{
		TryGetCustomAttribute<T>(type, out T attribute);
		return attribute;
	}
}
