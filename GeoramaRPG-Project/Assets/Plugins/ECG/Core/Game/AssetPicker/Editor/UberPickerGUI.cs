using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public static class UberPickerGUI
{
	public static readonly float UTILITY_BUTTON_WIDTH = 18.0f;
	public static readonly string POINTER_UNICODE = "\u261B";
	public static readonly string HAMBURGER_UNICODE = "\u2261";

	private static readonly float NESTED_INSPECTOR_BUFFER_HEIGHT = 4.0f;

	public static bool AttachAssetSelectButton(ref Rect attachToRect)
	{
		attachToRect.width -= UTILITY_BUTTON_WIDTH;
		Rect r = new Rect(attachToRect.x + attachToRect.width, attachToRect.y, UTILITY_BUTTON_WIDTH, EditorGUIUtility.singleLineHeight);
		return UnityEngine.GUI.Button(r, POINTER_UNICODE, UnityEngine.GUI.skin.label);
	}

	public static void AttachNullWarning(ref Rect position)
	{
		EditorGUI.DrawRect(position, new Color32(117, 6, 0, 255));
		Rect error = position;
		error.width = 56.0f;
		position.width -= error.width;
		error.x += position.width;
		EditorGUI.HelpBox(error, "NULL", MessageType.Error);
	}

	public static void GUIObject(
		SerializedProperty property,
		Rect position,
		GUIContent label,
		FieldInfo fieldInfo,
		IAssetPickerAttribute attribute,
		IAssetPickerPathSource pathSource)
	{
		string selectedPath = AssetDatabase.GetAssetPath(property.objectReferenceValue);
		GUIInternal(
			property,
			position,
			label,
			fieldInfo,
			attribute,
			pathSource,
			UberPickerPathCache.Select.Path,
			selectedPath,
			OnSelectedObject);
	}
	private static void OnSelectedObject(SerializedProperty property, string path)
	{
		if (!string.IsNullOrEmpty(path))
		{
			property.objectReferenceValue = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
		}
		else
		{
			property.objectReferenceValue = null;
		}
		property.serializedObject.ApplyModifiedProperties();
	}

	public static void GUIString(
		SerializedProperty property,
		Rect position,
		GUIContent label,
		FieldInfo fieldInfo,
		IAssetPickerAttribute attribute,
		IAssetPickerPathSource pathSource)
	{
		GUIInternal(
			property,
			position,
			label,
			fieldInfo,
			attribute,
			pathSource,
			UberPickerPathCache.Select.Name,
			property.stringValue,
			OnSelectedString);
	}
	private static void OnSelectedString(SerializedProperty property, string path)
	{
		if (!string.IsNullOrEmpty(path))
		{
			property.stringValue = Path.GetFileNameWithoutExtension(path);
		}
		else
		{
			property.stringValue = string.Empty;
		}
		property.serializedObject.ApplyModifiedProperties();
	}

	private static void GUIInternal(
		SerializedProperty property,
		Rect position,
		GUIContent label,
		FieldInfo fieldInfo,
		IAssetPickerAttribute attribute,
		IAssetPickerPathSource pathSource,
		UberPickerPathCache.Select selectionType,
		string selection,
		System.Action<SerializedProperty, string> onSelected)
	{
		Rect originalPosition = position;
		position.height = EditorGUIUtility.singleLineHeight;

		// If we have a label the widget takes up a full line, otherwise if there's no label we just take up a specific area
		// ie. Stats append a small picker to the end of another property drawer
		bool hasLabel = !string.IsNullOrEmpty(label.text);

		UberPickerPathCache pickerPaths = UberPickerPathCache.GetPaths(property, attribute, pathSource);
		int? selectedIndex = pickerPaths.TryGetIndexForSelection(selection, selectionType);
		string selectedPath = selectedIndex.HasValue ? pickerPaths.Paths[selectedIndex.Value] : null;
		if (pathSource.TryGetUnityObjectType(out System.Type objectType))
		{
			if (DragAndDrop.objectReferences.Length > 0)
			{
				string dragObjectPath = AssetDatabase.GetAssetPath(DragAndDrop.objectReferences[0]);
				int? dragIndex = pickerPaths.TryGetIndexForSelection(dragObjectPath, UberPickerPathCache.Select.Path);
				if (dragIndex.HasValue)
				{
					if (hasLabel)
					{
						TryDrawFoldoutInspector(property, label, position, originalPosition, out _);
					}
					UnityEngine.Object selected = string.IsNullOrEmpty(selectedPath) ? null :
						AssetDatabase.LoadAssetAtPath(selectedPath, DragAndDrop.objectReferences[0].GetType());
					UnityEngine.Object newSelection = hasLabel ?
						EditorGUI.ObjectField(position, " ", selected, DragAndDrop.objectReferences[0].GetType(), false) : 
						EditorGUI.ObjectField(position, selected, DragAndDrop.objectReferences[0].GetType(), false);
					if (newSelection != selected)
					{
						onSelected.Invoke(property, AssetDatabase.GetAssetPath(newSelection));
					}
					return;
				}
			}
			if (!string.IsNullOrEmpty(selectedPath) && AttachAssetSelectButton(ref position))
			{
				EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(selectedPath, typeof(UnityEngine.Object)));
			}
			AssetPickerUtilityMenu.TryAttachMenu(ref position, property, objectType, selectedPath, onSelected);
		}

		if (!attribute.AllowNull && !selectedIndex.HasValue)
		{
			AttachNullWarning(ref position);
		}

		Rect labelPosition = Rect.zero;
		if (hasLabel)
		{
			TryDrawFoldoutInspector(property, label, position, originalPosition, out labelPosition);
		}
		if (pickerPaths.ItemLevels == null)
		{
			if (!string.IsNullOrEmpty(attribute.OverrideFirstName))
			{
				pickerPaths.Names[0] = selectedIndex.HasValue ? UberPickerPathCache.NULL_ITEM_NAME : attribute.OverrideFirstName;
			}
			int index =
				selectedIndex.HasValue ? selectedIndex.Value :
				attribute.AllowNull ? 0 :
				-1;
			int newIndex = hasLabel ?
				EditorGUI.Popup(position, " ", index, pickerPaths.Names) : 
				EditorGUI.Popup(position, index, pickerPaths.Names);
			if (newIndex != index)
			{
				onSelected.Invoke(property, pickerPaths.Paths[newIndex]);
			}
		}
		else
		{
			// Title is always index 0, "None" element is index 1 when allowing nulls
			string name =
				selectedIndex.HasValue ? pickerPaths.Names[selectedIndex.Value] :
				!attribute.AllowNull ? "" :
				!string.IsNullOrEmpty(attribute.OverrideFirstName) ? attribute.OverrideFirstName :
				"";
			Rect buttonPosition = position;
			buttonPosition.xMin += labelPosition.width;
			if (GUI.Button(buttonPosition, name, EditorStyles.popup))
			{
				UberPickerSearchWindowProvider pickerWindow = UberPickerSearchWindowProvider.GetOrCreate(property, pickerPaths, onSelected);
				SearchWindowContext context = new SearchWindowContext(
					GUIUtility.GUIToScreenPoint(Event.current.mousePosition),
					pickerWindow.Width,
					pickerWindow.Height);
				SearchWindow.Open(context, pickerWindow);
			}
		}
	}

	public static void TryDrawFoldoutInspector(SerializedProperty property, GUIContent label, Rect position, Rect originalPosition, out Rect labelPosition)
	{
		labelPosition = position;
		labelPosition.width = EditorGUIUtility.labelWidth;
		if (property.propertyType != SerializedPropertyType.ObjectReference || property.objectReferenceValue == null || !(property.objectReferenceValue is ScriptableObject))
		{
			EditorGUI.LabelField(labelPosition, label);
			return;
		}
		SerializedObject obj = new SerializedObject(property.objectReferenceValue);
		SerializedProperty prop = obj.GetIterator();
		if (!prop.NextVisible(true) || !prop.NextVisible(false)) // You need to call Next (true) on the first element to get to the first element
		{
			EditorGUI.LabelField(labelPosition, label);
			return;
		}
		property.isExpanded = EditorGUI.Foldout(labelPosition, property.isExpanded, label, true);
		if (!property.isExpanded)
		{
			return;
		}

		Rect p = originalPosition;
		p.height = labelPosition.height;
		float startingY = p.y + p.height + NESTED_INSPECTOR_BUFFER_HEIGHT;
		p.y = startingY;
		EditorGUI.indentLevel++;
		prop.Reset();
		if (prop.NextVisible(true)) // You need to call Next (true) on the first element to get to the first element
		{
			do
			{
				p.height = EditorGUI.GetPropertyHeight(prop, true);
				EditorGUI.PropertyField(p, prop, true);
				p.y += p.height;
			}
			while (prop.NextVisible(false));
		}
		EditorGUI.indentLevel--;

		Rect r = new Rect(
			p.xMin,
			startingY,
			2.0f,
			p.yMin - startingY);
		EditorGUI.DrawRect(r, new Color32(15, 128, 190, 255));

		if (obj.hasModifiedProperties)
		{
			obj.ApplyModifiedProperties();
		}
	}

	public static float GetPropertyHeight(SerializedProperty property)
	{
		float height = EditorGUIUtility.singleLineHeight;
		if (!property.isExpanded)
		{
			return height;
		}
		if (property.objectReferenceValue == null || !(property.objectReferenceValue is ScriptableObject))
		{
			return height;
		}
		SerializedObject obj = new SerializedObject(property.objectReferenceValue);
		SerializedProperty prop = obj.GetIterator();
		if (!prop.NextVisible(true) || !prop.NextVisible(false)) // You need to call Next (true) on the first element to get to the first element
		{
			return height;
		}
		height += 2.0f * NESTED_INSPECTOR_BUFFER_HEIGHT;
		prop.Reset();
		if (prop.NextVisible(true)) // Skip the first visible property as this is always the script reference
		{
			do
			{
				height += EditorGUI.GetPropertyHeight(prop, true);
			}
			while (prop.NextVisible(false));
		}
		return height;
	}
}
