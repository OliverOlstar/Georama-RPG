using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class AssetPickerPathSource : IAssetPickerPathSource
{
	private System.Type m_Type = null;
	private System.Type m_TypeToLoad = null;
	private string m_PathPrefix = null;
	private readonly bool m_CanBeNested;

	public AssetPickerPathSource(System.Type type, string limitPathsByPrefix = null, bool canBesNested = true)
	{
		if (type.IsArray) // Attribute could be attached to an Array element
		{
			type = type.GetElementType(); // Attribute could be attached to a List element
		}
		else if (type.IsGenericType)
		{
			type = type.GetGenericArguments()[0];
		}
		m_Type = type;

		bool isComponent = typeof(Component).IsAssignableFrom(type);
		if (isComponent)
		{
			type = typeof(GameObject);
		}
		m_TypeToLoad = type;

		m_CanBeNested = canBesNested;
		m_PathPrefix = limitPathsByPrefix;
	}

	string IAssetPickerPathSource.GetSearchWindowTitle() => m_Type.Name;

	char[] IAssetPickerPathSource.GetPathSperators() => new char[] { '/', '\\' };

	List<string> IAssetPickerPathSource.GetPaths()
	{
		string[] paths = Core.AssetDatabaseUtil.Find(m_TypeToLoad, m_CanBeNested);
		if (string.IsNullOrEmpty(m_PathPrefix))
		{
			return new List<string>(paths);
		}

		List<string> validPaths = new List<string>(paths.Length);
		foreach (string path in paths)
		{
			if (path.StartsWith(m_PathPrefix))
			{
				validPaths.Add(path);
			}
		}
		return validPaths;
	}

	bool IAssetPickerPathSource.TryGetUnityObjectType(out System.Type type) { type = m_TypeToLoad; return true; }
}
