using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System.IO;

public class UberPickerSearchWindowProvider : ScriptableObject, ISearchWindowProvider
{
	private const string NULLICONPATH = "Assets/Editor/Textures/SearchWindowNoneIcon.png";
	private const string NULLITEMNAME = "None";

	private static readonly Core.AssetDatabaseDependentValues<string, UberPickerSearchWindowProvider> s_CachedSearchWindows =
		new Core.AssetDatabaseDependentValues<string, UberPickerSearchWindowProvider>();

	public static UberPickerSearchWindowProvider GetOrCreate(
		SerializedProperty property,
		UberPickerPathCache pickerPaths,
		System.Action<SerializedProperty, string> onSelected)
	{
		string key = UberPickerPathCache.GetPropertyCacheKey(property);
		if (!s_CachedSearchWindows.TryGet(key, out UberPickerSearchWindowProvider provider))
		{
			provider = CreateInstance<UberPickerSearchWindowProvider>();
			provider.Init(property, pickerPaths);
			s_CachedSearchWindows.Set(key, provider);
		}
		provider.RegisterOnSelected(onSelected, property);
		return provider;
	}

	private List<SearchTreeEntry> m_Entries = null;
	private SerializedProperty m_SerializedProperty;
	private System.Action<SerializedProperty, string> m_OnSelected;

	private void RegisterOnSelected(System.Action<SerializedProperty, string> onSelected, SerializedProperty property)
	{
		m_OnSelected = onSelected;
		m_SerializedProperty = property;
	}
	private UberPickerPathCache m_Paths;
	private GUIContent m_NoneEntryContent = null;

	private float m_Width = 0.0f;
	public float Width => m_Width;

	private float m_Height = 0.0f;
	public float Height => m_Height;

	public void Init(SerializedProperty serializedProperty, UberPickerPathCache paths)
	{
		m_SerializedProperty = serializedProperty;
		m_Paths = paths;

		Texture tex = AssetDatabase.GetCachedIcon(NULLICONPATH);
		m_NoneEntryContent = new GUIContent(NULLITEMNAME, tex);

		m_Entries = new List<SearchTreeEntry>(m_Paths.Names.Length);
		for (int i = 0; i < m_Paths.Names.Length; i++)
		{
			string name = m_Paths.Names[i];
			string path = m_Paths.Paths[i];
			int level = m_Paths.ItemLevels[i];
			if (path == string.Empty)
			{
				m_Entries.Add(new SearchTreeGroupEntry(new GUIContent(name), level));
			}
			else if (name == UberPickerPathCache.NULL_ITEM_NAME)
			{
				SearchTreeEntry entry = new SearchTreeEntry(m_NoneEntryContent);
				entry.userData = null;
				entry.level = level;
				m_Entries.Add(entry);
			}
			else
			{
				GUIContent content = Path.HasExtension(path) ? // If this is an asset path, then get it's icon
					new GUIContent(name, AssetDatabase.GetCachedIcon(path)) :
					new GUIContent(name);
				SearchTreeEntry entry = new SearchTreeEntry(content);
				entry.userData = path;
				entry.level = level;
				m_Entries.Add(entry);
			}

			// Calculate content width
			// Note: Groups are very special case-y as they don't have icons, but the drawer draws a blank space there as if there was one, 
			// also need to account for the '>' symbol after the group name
			float width =
				string.IsNullOrEmpty(path) ? EditorStyles.label.CalcSize(new GUIContent(name, tex)).x + 13.0f : // Group
				Path.HasExtension(path) ? EditorStyles.label.CalcSize(new GUIContent(name, tex)).x : // Item is an Asset
				EditorStyles.label.CalcSize(new GUIContent(name)).x; // Item is not an asset
			width += 3.0f; // Small magic number to make things fit more comfortably
			m_Width = Mathf.Max(m_Width, width);
		}

		int largestGroupCount = 0;
		int largestGroupLevel = 0;
		for (int groupLevel = 1; groupLevel < 100; groupLevel++)
		{
			bool found = false;
			int groupCount = 0;
			//string groupStart = string.Empty;
			for (int i = 0; i < m_Paths.Names.Length; i++)
			{
				int level = m_Paths.ItemLevels[i];
				if (level < groupLevel)
				{
					//if (groupCount > largestGroup)
					//{
					//	Debug.LogError("GWAR1 " + groupStart + " " + groupCount);
					//}
					largestGroupCount = Mathf.Max(largestGroupCount, groupCount);
					largestGroupLevel = level;
					groupCount = 0;
				}
				else if (level == groupLevel)
				{
					found = true;
					groupCount++;
					//groupStart = m_Paths.Names[i];
				}
			}
			//if (groupCount > largestGroup)
			//{
			//	Debug.LogError("GWAR2 " + groupStart + " " + groupCount);
			//}
			largestGroupCount = Mathf.Max(largestGroupCount, groupCount);
			largestGroupLevel = m_Paths.ItemLevels[m_Paths.ItemLevels.Length - 1];
			if (!found)
			{
				break;
			}
		}
		float x = 1.11f;
		float y = 1.6f;
		m_Height = (x * largestGroupCount * EditorGUIUtility.singleLineHeight) + (y * 2 * EditorGUIUtility.singleLineHeight);
		// Adjust height based on screen size so we can make the window as large as possible without it opening in a weird position
		float maxHeight = Screen.currentResolution.height * 0.45f;
		if (m_Height > maxHeight)
		{
			m_Height = maxHeight;
			m_Width += GUI.skin.verticalScrollbar.fixedWidth; // We know we're going to have to scroll so make space to accodate bar
		}
	}

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
		return m_Entries;
    }
    
    public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
		m_OnSelected.Invoke(m_SerializedProperty, searchTreeEntry.userData as string);
        return true;
    }

    private void OutputTree(List<SearchTreeEntry> list, SearchWindowStyle style)
    {
        string o = "[SearchWindow] " + style.ToString() + ":\n";
        for (int i = 0; i < list.Count; i++)
        {
            for (int z = 0; z < list[i].level; z++)
            {
                o += "|  ";
            }
            o += list[i].name + "\n";
        }        
        UnityEngine.Debug.Log(o);
    }
}
