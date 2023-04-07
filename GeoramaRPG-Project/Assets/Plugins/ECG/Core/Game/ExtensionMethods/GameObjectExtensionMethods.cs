using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensionMethods
{
	public static bool TryGetComponents<T>(this GameObject self, out T[] components)
	{
		components = self.GetComponents<T>();
		return components.Length > 0;
	}

	public static bool TryGetComponents<T>(this GameObject self, List<T> components)
	{
		components = components ?? throw new ArgumentNullException("components");
		components.Clear();
		components.AddRange(self.GetComponents<T>());
		return components.Count > 0;
	}

	public static bool TryGetComponentInChildren<T>(this GameObject self, out T component)
	{
		return TryGetComponentInChildren(self, false, out component);
	}

	public static bool TryGetComponentInChildren<T>(this GameObject self, bool includeInactive, out T component)
	{
		component = self.GetComponentInChildren<T>(includeInactive);
		return component != null;
	}

	public static bool TryGetComponentInChildren(this GameObject self, Type type, out Component component)
	{
		return TryGetComponentInChildren(self, type, false, out component);
	}

	public static bool TryGetComponentInChildren(this GameObject self, Type type, bool includeInactive, out Component component)
	{
		component = self.GetComponentInChildren(type, includeInactive);
		return component != null;
	}

	public static bool TryGetComponentsInChildren<T>(this GameObject self, out T[] components)
	{
		return TryGetComponentsInChildren(self, false, out components);
	}

	public static bool TryGetComponentsInChildren<T>(this GameObject self, bool includeInactive, out T[] components)
	{
		components = self.GetComponentsInChildren<T>(includeInactive);
		return components.Length > 0;
	}

	public static bool TryGetComponentsInChildren<T>(this GameObject self, List<T> components)
	{
		return TryGetComponentsInChildren(self, false, components);
	}

	public static bool TryGetComponentsInChildren<T>(this GameObject self, bool includeInactive, List<T> components)
	{
		components = components ?? throw new ArgumentNullException("components");
		components.Clear();
		self.GetComponentsInChildren(includeInactive, components);
		return components.Count > 0;
	}

	public static bool TryGetComponentInParent<T>(this GameObject self, out T component)
	{
		component = self.GetComponentInParent<T>();
		return component != null;
	}

	public static bool TryGetComponentsInParent<T>(this GameObject self, out T[] components)
	{
		components = self.GetComponentsInParent<T>();
		return components.Length > 0;
	}
}
