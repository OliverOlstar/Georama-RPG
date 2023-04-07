using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

[CustomPropertyDrawer(typeof(StringSearchWindowAttribute), true)]
public class StringSearchWindowDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        StringSearchWindowAttribute values = (StringSearchWindowAttribute)attribute;

        position.width -= 60;
        if (values.UseTextField)
        {
            string input = EditorGUI.TextField(position, label, property.stringValue);
            if (values.AllowNull || !string.IsNullOrEmpty(input))
            {
                property.stringValue = input;
            }
        }
        else
        {
            EditorGUI.TextField(position, label, property.stringValue);
        }

        position.x += position.width;
        position.width = 60;
        if (GUI.Button(position, new GUIContent("Pick")))
        {
            SearchWindowContext context = new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition));
            StringSearchWindowProvider provider = ScriptableObject.CreateInstance(typeof(StringSearchWindowProvider)) as StringSearchWindowProvider;
            provider.Init(property, values, values);
            SearchWindow.Open(context, provider);
        }
    }
}
