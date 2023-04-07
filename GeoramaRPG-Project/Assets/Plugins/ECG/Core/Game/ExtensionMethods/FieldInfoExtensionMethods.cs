using System;
using System.Reflection;

public static class FieldInfoExtensionMethods
{
	public static bool TryGetCustomAttribute<T>(this FieldInfo self, out T attribute)
		where T : Attribute
	{
		self = self ?? throw new ArgumentNullException(nameof(self));
		attribute = self.GetCustomAttribute<T>();
		return attribute != null;
	}
}
