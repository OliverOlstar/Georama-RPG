using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class UberPickerPathCache
{
	public const int MAX_ITEMS_IN_DROPDOWN = 30;
	public const string NULL_ICON_PATH = "Assets/Editor/Textures/SearchWindowNoneIcon.png";
	public const string NULL_ITEM_NAME = "None";

	private static Core.AssetDatabaseDependentValues<string, UberPickerPathCache> s_Cache = 
		new Core.AssetDatabaseDependentValues<string, UberPickerPathCache>();

	public static string GetPropertyCacheKey(SerializedProperty property) => 
		Core.Str.Build(property.serializedObject.targetObject.GetType().Name, ".", property.propertyPath);

	public static UberPickerPathCache GetPaths(
		SerializedProperty property,
		IAssetPickerAttribute attribute,
		IAssetPickerPathSource pathSource)
	{
		string key = GetPropertyCacheKey(property);
		if (s_Cache.TryGet(key, out UberPickerPathCache pickerPaths))
		{
			return pickerPaths;
		}
		List<string> pathList = pathSource.GetPaths();
		if (pathList.Count <= MAX_ITEMS_IN_DROPDOWN || attribute.ForceFlatten)
		{
			List<string> nameList = new List<string>(pathList.Count);
			foreach (string path in pathList)
			{
				string name = Path.GetFileNameWithoutExtension(path);
				nameList.Add(name);
			}
			if (attribute.AllowNull)
			{
				pathList.Insert(0, string.Empty);
				nameList.Insert(0, NULL_ITEM_NAME);
			}
			pickerPaths = new UberPickerPathCache(nameList.ToArray(), pathList.ToArray(), null);
			s_Cache.Set(key, pickerPaths);
		}
		else
		{
			UberPickerPathOptimizer.Calculate(
				pathList,
				pathSource.GetPathSperators(),
				pathSource.GetSearchWindowTitle(),
				out List<string> itemNames,
				out List<string> itemPaths,
				out List<int> itemLevels);
			if (attribute.AllowNull)
			{
				// Index 1 is after title object
				itemPaths.Insert(1, NULL_ICON_PATH);
				itemNames.Insert(1, NULL_ITEM_NAME);
				itemLevels.Insert(1, 1);
			}
			pickerPaths = new UberPickerPathCache(itemNames.ToArray(), itemPaths.ToArray(), itemLevels.ToArray());
			s_Cache.Set(key, pickerPaths);
		}
		return pickerPaths;
	}

	public enum Select
	{
		Path,
		Name,
	}

	public UberPickerPathCache(string[] names, string[] paths, int[] itemLevels)
	{
		Names = names;
		Paths = paths;
		ItemLevels = itemLevels;
	}

	public string[] Names { get; }
	public string[] Paths { get; }
	public int[] ItemLevels { get; }

	public int? TryGetIndexForSelection(string selection, Select selectionType)
	{
		int? selectedIndex = null;
		switch (selectionType)
		{
			case Select.Name:
				for (int i = 0; i < Names.Length; i++)
				{
					if (string.Equals(Names[i], selection))
					{
						selectedIndex = i;
						break;
					}
				}
				break;
			case Select.Path:
				for (int i = 0; i < Paths.Length; i++)
				{
					// Search window adds some items with empty paths that should be selected, ie title is always index 0, also groups
					string path = Paths[i];
					if (!string.IsNullOrEmpty(path) && string.Equals(path, selection))
					{
						selectedIndex = i;
						break;
					}
				}
				break;
		}
		return selectedIndex;
	}
}
