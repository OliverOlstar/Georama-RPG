using System;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentExtensionMethods
{
	public static bool TryGetComponents<T>(this Component self, out T[] components)
	{
		components = self.GetComponents<T>();
		return components.Length > 0;
	}

	public static bool TryGetComponents<T>(this Component self, List<T> components)
	{
		components = components ?? throw new ArgumentNullException("components");
		components.Clear();
		components.AddRange(self.GetComponents<T>());
		return components.Count > 0;
	}

	public static bool TryGetComponentInChildren<T>(this Component self, out T component)
	{
		return TryGetComponentInChildren(self, false, out component);
	}

	public static bool TryGetComponentInChildren<T>(this Component self, bool includeInactive, out T component)
	{
		component = self.GetComponentInChildren<T>(includeInactive);
		return component != null;
	}

	public static bool TryGetComponentsInChildren<T>(this Component self, out T[] components)
	{
		return TryGetComponentsInChildren(self, false, out components);
	}

	public static bool TryGetComponentsInChildren<T>(this Component self, bool includeInactive, out T[] components)
	{
		components = self.GetComponentsInChildren<T>(includeInactive);
		return components.Length > 0;
	}

	public static bool TryGetComponentsInChildren<T>(this Component self, List<T> components)
	{
		return TryGetComponentsInChildren(self, false, components);
	}

	public static bool TryGetComponentsInChildren<T>(this Component self, bool includeInactive, List<T> components)
	{
		components = components ?? throw new ArgumentNullException("components");
		components.Clear();
		self.GetComponentsInChildren(includeInactive, components);
		return components.Count > 0;
	}

	public static bool TryGetComponentInParent<T>(this Component self, out T component)
	{
		component = self.GetComponentInParent<T>();
		return component != null;
	}

	public static bool TryGetComponentsInParent<T>(this Component self, out T[] components)
	{
		components = self.GetComponentsInParent<T>();
		return components.Length > 0;
	}
}
