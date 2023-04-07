using System.Collections.Generic;

public static class ListExtensionMethods
{
	public static bool AddUniqueItem<T>(this List<T> self, T item)
	{
		if (self.Contains(item))
		{
			return false;
		}
		self.Add(item);
		return true;
	}
}