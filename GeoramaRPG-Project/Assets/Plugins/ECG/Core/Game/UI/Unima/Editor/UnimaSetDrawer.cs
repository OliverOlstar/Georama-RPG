
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UnimaSet))]
public class UnimaSetDrawer : PropertyDrawer
{
	private static readonly float SPACE = 4.0f;
	private static readonly float PADDING = 2.0f;

	private int m_Errors = 0;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		m_Errors = 0;
		Component component = property.serializedObject.targetObject as Component;
		SerializedProperty animations = property.FindPropertyRelative("m_Animations");
		Rect r = position;
		r.y += PADDING;
		r.height = EditorGUIUtility.singleLineHeight;

		Rect header = r;
		header.xMin -= 2.0f;
		EditorGUI.DrawRect(header, 0.5f * Color.white);
		if (GUI.Button(header, string.Empty, GUIStyle.none))
		{
			animations.InsertArrayElementAtIndex(0);
			SerializedProperty newAnimation = animations.GetArrayElementAtIndex(0);
			SerializedProperty newTiming = newAnimation.FindPropertyRelative("m_Timing");
			newTiming.FindPropertyRelative("m_WaitToFinish").boolValue = true;
		}

		Rect r2 = r;
		r2.width = 20.0f;
		r.width -= r2.width;
		r2.x += r.width;

		GUIStyle style = new GUIStyle(GUI.skin.label);
		style.alignment = TextAnchor.MiddleCenter;
		style.fontSize = 16;

		EditorGUI.LabelField(r, property.displayName);
		EditorGUI.LabelField(r2, "\uE09D", style);
		r.y += r.height;
		r2.y += r.height;
		r = UnimaDrawerUtil.AddIndent(r);
		r.y += SPACE;
		r2.y += SPACE;

		if (animations.arraySize == 0)
		{
			EditorGUI.HelpBox(r, "Empty", MessageType.Warning);
			m_Errors++;
		}

		int removeIndex = -1;
		for (int i = 0; i < animations.arraySize; i++)
		{
			float totalHeight = EditorGUIUtility.singleLineHeight;
			SerializedProperty element = animations.GetArrayElementAtIndex(i);
			SerializedProperty animationRef = element.FindPropertyRelative("m_Animation");
			string error = null;
			UnimateBase animation = animationRef.objectReferenceValue as UnimateBase;
			if (animation == null)
			{
				error = "Unimation reference is missing";
				m_Errors++;
			}
			else if (component != null && animation.EditorValidate(component.gameObject, out error))
			{
				m_Errors++;
			}

			if (element.NextVisible(true)) // Skip the first visible property as this is always the script reference
			{
				int depth = element.depth;
				do
				{
					totalHeight += EditorGUI.GetPropertyHeight(element, true);
				}
				while (element.NextVisible(false) && element.depth == depth);
			}
			Rect bgRect = r;
			bgRect.width += r2.width;
			bgRect.height = totalHeight;
			bgRect.xMin -= 2.0f;
			EditorGUI.DrawRect(bgRect, 0.5f * Color.white);

			if (GUI.Button(r2, "\uE09F", style))
			{
				removeIndex = i;
			}

			element = animations.GetArrayElementAtIndex(i);
			if (element.NextVisible(true)) // Skip the first visible property as this is always the script reference
			{
				int depth = element.depth;
				do
				{
					r.height = EditorGUI.GetPropertyHeight(element, true);
					EditorGUI.PropertyField(r, element, true);
					r.y += r.height;
					r2.y += r.height;
				}
				while (element.NextVisible(false) && element.depth == depth);
			}

			r.height = EditorGUIUtility.singleLineHeight;
			if (string.IsNullOrEmpty(error))
			{
				UnimaDurationType durType = animation.GetEditorDuration(out float duration);
				string durationString =
					durType == UnimaDurationType.Arbitrary ? "?" :
					durType == UnimaDurationType.Infinite ? "\u221e" :
					duration.ToString();
				EditorGUI.LabelField(r, "Duration", durationString);
			}
			else
			{
				EditorGUI.HelpBox(UnimaDrawerUtil.AddIndent(r), error, MessageType.Error);
			}
			r.y += r.height;
			r2.y += r.height;
			r.y += SPACE;
			r2.y += SPACE;
		}
		if (removeIndex >= 0)
		{
			// When an object reference is assigned first delete will set it none,
			// and the second will remove the array element
			int size = animations.arraySize;
			animations.DeleteArrayElementAtIndex(removeIndex);
			if (size == animations.arraySize)
			{
				animations.DeleteArrayElementAtIndex(removeIndex);
			}
		}
	}
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		SerializedProperty animations = property.FindPropertyRelative("m_Animations");
		int count = animations.arraySize;
		float height = EditorGUIUtility.singleLineHeight * Mathf.Max(count + 1, 2);
		height += SPACE * (count + 1);
		for (int i = 0; i < count; i++)
		{
			SerializedProperty element = animations.GetArrayElementAtIndex(i);
			if (element.NextVisible(true)) // Skip the first visible property as this is always the script reference
			{
				int depth = element.depth;
				do
				{
					height += EditorGUI.GetPropertyHeight(element, true);
				}
				while (element.NextVisible(false) && element.depth == depth);
			}
		}
		height += 2.0f * PADDING;
		return height;
	}
}
