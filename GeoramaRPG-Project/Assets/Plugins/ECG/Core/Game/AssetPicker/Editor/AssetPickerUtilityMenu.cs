using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class AssetPickerUtilityMenu
{
	public class Context
	{
		public Vector2 Position;
		public System.Type Type;
		public UnityEngine.Object Object;
		public System.Action<SerializedProperty, string> OnSelected;
		public SerializedProperty Property;
	}

	public static void TryAttachMenu(
		ref Rect attachToRect,
		SerializedProperty property,
		System.Type objectType,
		string selectedPath = null,
		System.Action<SerializedProperty, string> onSelected = null)
	{
		if (!typeof(ScriptableObject).IsAssignableFrom(objectType))
		{
			return;
		}

		attachToRect.width -= UberPickerGUI.UTILITY_BUTTON_WIDTH;
		Rect r = new Rect(attachToRect.x + attachToRect.width, attachToRect.y, UberPickerGUI.UTILITY_BUTTON_WIDTH, EditorGUIUtility.singleLineHeight);
		GUI.Label(r, UberPickerGUI.HAMBURGER_UNICODE);

		Event e = Event.current;
		if (e.type == EventType.MouseDown &&
			e.button == 0 &&
			r.Contains(e.mousePosition))
		{
			GenericMenu menu = new GenericMenu();
			Vector2 p = GUIUtility.GUIToScreenPoint(e.mousePosition);
			if (!string.IsNullOrEmpty(selectedPath))
			{
				UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(selectedPath, objectType);
				if (obj != null)
				{
					Context context = new Context
					{
						Type = objectType,
						Object = obj,
						Property = property,
						OnSelected = onSelected,
						Position = p,
					};
					menu.AddItem(new GUIContent("Rename"), false, OnRename, context);
					menu.AddItem(new GUIContent($"Duplicate ({obj.GetType().Name})"), false, OnDuplicate, context);
				}
			}
			foreach (System.Type type in Core.TypeUtility.GetMatchingTypes(objectType, IsTypeMatching))
			{
				Context context = new Context
				{
					Type = type,
					Property = property,
					OnSelected = onSelected,
					Position = p,
				};
				menu.AddItem(new GUIContent("New " + type.Name), false, OnCreate, context);
			}
			menu.ShowAsContext();
		}
	}

	private static bool IsTypeMatching(System.Type type, System.Type baseType)
	{
		return baseType.IsAssignableFrom(type) &&
			!type.IsGenericTypeDefinition &&
			!type.IsAbstract &&
			!type.IsDefined(typeof(System.ObsoleteAttribute), false);
	}

	public static void OnDuplicate(object obj)
	{
		Context context = obj as Context;
		UnityEngine.Object objectToDuplicate = context.Object;
		UnityEngine.Object createdObject = UnityEngine.Object.Instantiate(objectToDuplicate);
		string sourcePath = AssetDatabase.GetAssetPath(objectToDuplicate);
		HandleCreatedObject(createdObject, sourcePath, context);
	}

	public static void OnCreate(object obj)
	{
		Context context = obj as Context;
		UnityEngine.Object createdObject = ScriptableObject.CreateInstance(context.Type);
		createdObject.name = $"New {context.Type.Name}";
		string sourcePath = null;
		string[] paths = Core.AssetDatabaseUtil.Find(context.Type);
		if (paths.Length != 0)
		{
			sourcePath = paths[0];
		}
		HandleCreatedObject(createdObject, sourcePath, context);
	}

	private static void HandleCreatedObject(
		UnityEngine.Object createdObject,
		string sourcePath,
		Context context)
	{
		string createdObjectPath;
		if (!string.IsNullOrEmpty(sourcePath))
		{
			createdObjectPath = $"{System.IO.Path.GetDirectoryName(sourcePath)}/{createdObject.name}{System.IO.Path.GetExtension(sourcePath)}";
		}
		else
		{
			createdObjectPath = $"Assets/ScriptableObjects/{createdObject.GetType().Name}";
			System.IO.Directory.CreateDirectory(createdObjectPath);
			createdObjectPath += $"/{createdObject.name}.asset";
		}
		createdObjectPath = AssetDatabase.GenerateUniqueAssetPath(createdObjectPath);
		AssetDatabase.CreateAsset(createdObject, createdObjectPath);
		EditorGUIUtility.PingObject(createdObject);
		AssetPickerRenameWindow.Open(createdObject, context.Type, context.Position);
		context.OnSelected?.Invoke(context.Property, createdObjectPath);
	}

	public static void OnRename(object obj)
	{
		Context context = obj as Context;
		EditorGUIUtility.PingObject(context.Object);
		AssetPickerRenameWindow.Open(context.Object, context.Type, context.Position);
	}
}
