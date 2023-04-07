using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;

public class StringSearchWindowProvider : AssetSearchWindowProvider
{
    new protected const string NULLITEMNAME = "Clear";

    protected virtual string ICONPATH => "Assets/Editor/Textures/SearchWindowStringIcon.png";

    public override bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        m_SerializedProperty.stringValue = (string)searchTreeEntry.userData;
        m_SerializedProperty.serializedObject.ApplyModifiedProperties();
        return true;
    }

    protected override void AddItem(ref List<SearchTreeEntry> list, string name, string path, int level)
    {
        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(ICONPATH);
        SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(name, EditorGUIUtility.ObjectContent(obj, obj.GetType()).image));
        entry.userData = name;
        entry.level = level;
        list.Add(entry);
    }
}
