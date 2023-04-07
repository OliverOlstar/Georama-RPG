
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UnimaTiming))]
public class UnimaTimingDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (property.NextVisible(true)) // Skip the first visible property as this is always the script reference
		{
			int depth = property.depth;
			do
			{
				position.height = EditorGUI.GetPropertyHeight(property, true);
				EditorGUI.PropertyField(position, property, true);
				position.y += position.height;
			}
			while (property.NextVisible(false) && property.depth == depth);
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		float height = 0.0f;
		if (property.NextVisible(true)) // Skip the first visible property as this is always the script reference
		{
			int depth = property.depth;
			do
			{
				height += EditorGUI.GetPropertyHeight(property, true);
			}
			while (property.NextVisible(false) && property.depth == depth);
		}
		return height;
	}
}
