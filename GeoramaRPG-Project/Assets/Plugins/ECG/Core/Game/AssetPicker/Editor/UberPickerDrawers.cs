
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(UberPicker.AssetAttribute), true)]
public class AssetPickerDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		UberPicker.AssetAttribute at = attribute as UberPicker.AssetAttribute;
		UberPickerGUI.GUIObject(
			property,
			position,
			label,
			fieldInfo,
			at,
			new AssetPickerPathSource(fieldInfo.FieldType, at.PathPrefix, canBesNested: at.CanBeNested));
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => UberPickerGUI.GetPropertyHeight(property);
}

[CustomPropertyDrawer(typeof(UberPicker.AssetNameAttribute))]
public class AssetNamePickerDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		UberPicker.AssetNameAttribute at = attribute as UberPicker.AssetNameAttribute;
		UberPickerGUI.GUIString(
			property,
			position,
			label,
			fieldInfo,
			at,
			new AssetPickerPathSource(at.Type, at.PathPrefix));
	}
}
