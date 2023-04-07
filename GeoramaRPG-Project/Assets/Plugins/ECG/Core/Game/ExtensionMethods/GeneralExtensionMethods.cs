using System;
using System.Collections.Generic;

public static class GeneralExtensionMethods  {
    public static bool IsNumber(this object value)
    {
        return value is sbyte
                || value is byte
                || value is short
                || value is ushort
                || value is int
                || value is uint
                || value is long
                || value is ulong
                || value is float
                || value is double
                || value is decimal;
    }

	public static void Foreach<T>(this IEnumerable<T> self, Action action)
	{
		foreach (var element in self)
		{
			action?.Invoke();
		}
	}

	public static void Foreach<T>(this IEnumerable<T> self, Action<T> action, Func<T, bool> predic)
	{
		foreach (var element in self)
		{
			if (predic(element))
			{
				action?.Invoke(element);
			}
		}
	}

	public static void Foreach<T>(this IEnumerable<T> self, Predicate<T> predic, Action<T> action)
	{
		foreach (var element in self)
		{
			if(predic(element))
			{
				action?.Invoke(element);
			}
		}
	}

	public static void Foreach<T>(this IEnumerable<T> self, Action<T> action)
	{
		foreach (var element in self)
		{
			action?.Invoke(element);
		}
	}

	public static void Foreach<T>(this IEnumerable<T> self, Action<T, int> action)
	{
		int index = 0;
		foreach (var element in self)
		{
			action?.Invoke(element, index++);
		}
	}

	public static void Foreach<T>(this IList<T> self, Action<T> action)
	{
		for (int index = 0; index < self.Count; index++)
		{
			action?.Invoke(self[index]);
		}
	}

	public static void Foreach<T>(this IList<T> self, Action<T, int> action)
	{
		for(int index = 0; index < self.Count; index++)
		{
			action?.Invoke(self[index], index);
		}
	}
}
