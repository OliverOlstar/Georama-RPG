using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System.IO;

public class AssetSearchWindowProvider : ScriptableObject, ISearchWindowProvider
{
    protected const string NULLICONPATH = "Assets/Editor/Textures/SearchWindowNoneIcon.png";
    protected const string NULLITEMNAME = "None";

	protected SerializedProperty m_SerializedProperty;
	protected ISearchWindowParameters m_Parameters;
	protected IAssetPickerPathSource m_PathSource;

    public void Init( // Switch from constructor to Init because Unity was giving me warnings for not using ScriptableObject.CreateInstance()
        SerializedProperty serializedProperty,
		ISearchWindowParameters parameters,
		IAssetPickerPathSource pathSource)
    {
		m_SerializedProperty = serializedProperty;
		m_Parameters = parameters;
		m_PathSource = pathSource;
	}

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> list = new List<SearchTreeEntry>();
        switch (m_Parameters.Style)
        {
            case SearchWindowStyle.Full:
                Full(ref list);
                break;
                
            case SearchWindowStyle.Compact:
                Compact(ref list);
                break;
                
            case SearchWindowStyle.Flatten:
                Flatten(ref list);
                break;
                
            case SearchWindowStyle.NoGroups:
                NoGroups(ref list);
                break;
        }

        // OutputTree(list, m_Style);

        if (m_Parameters.AllowNull)
        {
            AddNoneItem(ref list, 1);
        }

        return list;
    }
    
    public virtual bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        m_SerializedProperty.objectReferenceValue = (UnityEngine.Object)searchTreeEntry.userData;
        m_SerializedProperty.serializedObject.ApplyModifiedProperties();
        return true;
    }

    protected virtual void AddItem(ref List<SearchTreeEntry> list, string name, string path, int level)
    {
		UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(name, EditorGUIUtility.ObjectContent(obj, obj.GetType()).image));
        entry.userData = obj;
        entry.level = level;
        list.Add(entry);
    }

	protected void AddNoneItem(ref List<SearchTreeEntry> list, int level, int index = 1)
    {
        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(NULLICONPATH);
        SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(NULLITEMNAME, EditorGUIUtility.ObjectContent(obj, obj.GetType()).image));
        entry.userData = null;
        entry.level = level;
        list.Insert(index, entry);
    }

	private void Full(ref List<SearchTreeEntry> list)
    {
		List<string> paths = m_PathSource.GetPaths();
		list.Add(new SearchTreeGroupEntry(new GUIContent(m_PathSource.GetSearchWindowTitle())));
        List<string> groupsList = new List<string>();
        int lowestGroupCount = 0;
        foreach (string path in paths)
        {
            string[] groups = path.Split(m_PathSource.GetPathSperators());
			string name = Path.GetFileNameWithoutExtension(path);
			string groupName = "";
            for (int i = 0; i < groups.Length - 1; i++)
            {
                groupName += groups[i];
                if (!groupsList.Contains(groupName))
                {
                    list.Add(new SearchTreeGroupEntry(new GUIContent(groups[i]), i + 1));
                    groupsList.Add(groupName);
                    if (i == 0)
                    {
                        lowestGroupCount++;
                    }
                }
				groupName += '/';
            }
            AddItem(ref list, name, path, groups.Length);
            if (groups.Length == 1)
            {
                lowestGroupCount++;
            }
        }

        // If only one group at level 1, this is meant to hide the "Assets" folder but not prevent other file structure from breaking
        if (lowestGroupCount == 1)
        {
            list.RemoveAt(1); // First group, other than root
            ModifyLevel(list, 1, list.Count);
        }
    }

    private void Compact(ref List<SearchTreeEntry> list)
    {
		AssetSearchWindowUtil.GetOptimizedPaths(
			m_PathSource,
			m_Parameters.TreeSearchParameters,
			out List<string> itemNames, 
			out List<string> itemPaths, 
			out List<int> itemLevels);
        for (int i = 0; i < itemNames.Count; i++)
        {
            if (itemPaths[i] == string.Empty)
            {
                list.Add(new SearchTreeGroupEntry(new GUIContent(itemNames[i]), itemLevels[i]));
            }
            else
            {
                AddItem(ref list, itemNames[i], itemPaths[i], itemLevels[i]);
            }
        }
    }

    private void Flatten(ref List<SearchTreeEntry> list)
    {
		List<string> paths = m_PathSource.GetPaths();
		list.Add(new SearchTreeGroupEntry(new GUIContent(m_PathSource.GetSearchWindowTitle())));
        List<string> groupsList = new List<string>();
        foreach (string path in paths)
        {
            string[] groups = path.Split(m_PathSource.GetPathSperators());
			string name = Path.GetFileNameWithoutExtension(path);
			if (groups.Length > 1)
            {
                string groupName = groups[groups.Length - 2];
                if (!groupsList.Contains(groupName))
                {
                    list.Add(new SearchTreeGroupEntry(new GUIContent(groupName), 1));
                    groupsList.Add(groupName);
                }
                AddItem(ref list, name, path, 2);
            }
            else
            {
                AddItem(ref list, name, path, 1); // Not in a groups
            }
        }

        // If only one group exists, remove it
        if (groupsList.Count == 1)
        {
            list.RemoveAt(1); // First group, other than root
            ModifyLevel(list, 1, list.Count);
        }
    }

    private void NoGroups(ref List<SearchTreeEntry> list)
    {
		List<string> paths = m_PathSource.GetPaths();
		list.Add(new SearchTreeGroupEntry(new GUIContent(m_PathSource.GetSearchWindowTitle())));
        foreach (string path in paths)
        {
			string name = Path.GetFileNameWithoutExtension(path);
            AddItem(ref list, name, path, 1);
        }
    }

    private void ModifyLevel(in List<SearchTreeEntry> list, int fromInclusive, int toExclusive, int amount = -1)
    {
        for (int i = fromInclusive; i < toExclusive; i++)
        {
            list[i].level += amount;
        }
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
