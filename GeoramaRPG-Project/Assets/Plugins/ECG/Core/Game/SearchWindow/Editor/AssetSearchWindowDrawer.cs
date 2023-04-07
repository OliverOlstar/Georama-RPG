using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System;

[CustomPropertyDrawer(typeof(SearchWindowAttribute), true)]
public class AssetSearchWindowDrawer : PropertyDrawer
{
    private bool m_ShowHelpBox = false;
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SearchWindowAttribute values = (SearchWindowAttribute)attribute;
        UpdateHelpBox(ref position, property, label, values); // Show error if null && null is not allowed

        position.width -= 60;
        UnityEngine.Object obj = EditorGUI.ObjectField(position, label, property.objectReferenceValue, fieldInfo.FieldType, false);
        if (values.AllowNull || obj != null)
        {
            property.objectReferenceValue = obj;
        }

        position.x += position.width;
        position.width = 60;
        if (GUI.Button(position, new GUIContent("Find")))
        {
            SearchWindowContext context = new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition));
            AssetSearchWindowProvider provider = ScriptableObject.CreateInstance(typeof(AssetSearchWindowProvider)) as AssetSearchWindowProvider;
			provider.Init(
				property, 
				values, 
				new AssetPickerPathSource(
					values.CustomType != null ? values.CustomType : fieldInfo.FieldType, 
					values.PathPrefix));
            SearchWindow.Open(context, provider);
        }


		// Playing around with some different styles

		//Rect r1 = position;
		//r1.width = EditorGUIUtility.labelWidth;

		//EditorGUI.LabelField(r1, label);

		//Rect r2 = position;
		//r2.width -= r1.width;
		//r2.x += r1.width;

		//if (GUI.Button(r2, property.objectReferenceValue == null ? "None" : property.objectReferenceValue.name, EditorStyles.popup))
		//{
		//	SearchWindowContext context = new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition));
		//	AssetSearchWindowProvider provider = ScriptableObject.CreateInstance(typeof(AssetSearchWindowProvider)) as AssetSearchWindowProvider;
		//	provider.Init(fieldInfo, property, values);
		//	SearchWindow.Open(context, provider);
		//}
	}

	private void UpdateHelpBox(ref Rect position, in SerializedProperty property, in GUIContent label, in ISearchWindowParameters values)
    {
        if (!values.AllowNull && property.objectReferenceValue == null)
        {
            if (m_ShowHelpBox)
            {
                position.height *= 0.5f;
                position.y += position.height;
                EditorGUI.HelpBox(position, label.text + " can not be NULL", MessageType.Error);
                position.y -= position.height;
            }
            m_ShowHelpBox = true;
        }
        else
        {
            m_ShowHelpBox = false;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) * (m_ShowHelpBox ? 2 : 1);
    }
}