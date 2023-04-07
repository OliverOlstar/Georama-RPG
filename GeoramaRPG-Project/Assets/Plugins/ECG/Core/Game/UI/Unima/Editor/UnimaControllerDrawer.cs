
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomPropertyDrawer(typeof(UnimaController))]
public class UnimaControllerDrawer : PropertyDrawer
{
	private const float SPACE = 4.0f;
	private const float PADDING = 2.0f;

	private UnimaController m_Controller = null;

	private float m_Height = 0.0f;

	private void TryGetController2(SerializedProperty property)
	{
		if (m_Controller != null)
		{
			// Trick to keep window updating so we can show the state of the controller at runtime
			foreach (Editor item in ActiveEditorTracker.sharedTracker.activeEditors)
			{
				if (item.serializedObject == property.serializedObject)
				{
					item.Repaint();
					break;
				}
			}
			return;
		}
		if (!Application.isPlaying)
		{
			return;
		}
		if (!(property.serializedObject.targetObject is IUnimaControllerSource source))
		{
			return;
		}
		// Search source for this controller
		System.Type type = source.GetType();
		while (type != null && type != typeof(MonoBehaviour))
		{
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo info in fields)
			{
				if (info.FieldType == typeof(UnimaController) && string.Equals(info.Name, fieldInfo.Name))
				{
					if (info.GetValue(source) is UnimaController controller)
					{
						m_Controller = controller;
						return;
					}
				}
			}
			type = type.BaseType;
		}

		return;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		TryGetController2(property);
		if (m_Controller != null && m_Controller.IsPlaying())
		{
			EditorGUI.DrawRect(position, new Color32(44, 93, 135, 255));
		}
		else
		{
			EditorGUI.HelpBox(position, "", MessageType.None);
		}
		//Color rectColor = m_Controller != null && m_Controller.IsPlaying() ? new Color(0.5f, 0.75f, 0.9f) : 0.1f * Color.white;

		Rect p1 = position;
		p1.y += PADDING;
		p1.width -= PADDING;
		p1.height = EditorGUIUtility.singleLineHeight;

		//EditorGUI.LabelField(p1, property.displayName, EditorStyles.boldLabel);
		float playControlWidth = 20.0f;
		Rect pFoldout = p1;
		pFoldout.width -= playControlWidth;
		property.isExpanded = EditorGUI.Foldout(pFoldout, property.isExpanded, " \u24ca " + property.displayName, true);

		if (m_Controller != null && m_Controller.CanPlay())
		{
			Rect pPlay = p1;
			pPlay.width = playControlWidth;
			pPlay.x = p1.width;
			GUIStyle playButton = new GUIStyle(GUI.skin.label);
			playButton.active.textColor = Color.white;
			if (m_Controller.IsPlaying())
			{
				playButton.fontSize = 16;
				//playButton.padding.bottom = 6;
				if (GUI.Button(pPlay, "\u25a0", playButton))
				{
					m_Controller.Stop();
				}
			}
			else
			{
				playButton.fontSize = 12;
				//playButton.padding.bottom = 3;
				if (GUI.Button(pPlay, "\u25b6", playButton))
				{
					m_Controller.Play();
				}
			}
		}

		if (!property.isExpanded)
		{
			return;
		}

		SerializedProperty mode = property.FindPropertyRelative("m_Mode");
		SerializedProperty alphaID = property.FindPropertyRelative("m_AlphaID");

		p1.y += p1.height + SPACE;
		p1 = UnimaDrawerUtil.AddIndent(p1);
		EditorGUI.PropertyField(p1, mode);
		p1.y += p1.height;

		switch ((UnimaControllerMode)mode.enumValueIndex)
		{
			case UnimaControllerMode.Beta:
				if (property.serializedObject.targetObject is Component component)
				{
					if (GetAlphaIDs(component, alphaID.stringValue, out int index, out string[] ids))
					{
						index = EditorGUI.Popup(p1, alphaID.displayName, index, ids);
						alphaID.stringValue = ids[index];
						p1.y += p1.height;

						if (UnimaUtil.TryGetAlpha(component.gameObject, alphaID.stringValue, out _, out IUnimaControllerSource alphaSource) &&
							alphaSource is Component cp)
						{
							p1 = UnimaDrawerUtil.AddIndent(p1);
							EditorGUI.ObjectField(p1, UnimaControllerMode.Alpha.ToString(), cp, typeof(Component), true);
							p1.y += p1.height;
							p1 = UnimaDrawerUtil.RemoveIndent(p1);
						}
					}
					else if (ids.Length == 0)
					{
						EditorGUI.PropertyField(p1, alphaID);
						p1.y += p1.height;
						EditorGUI.HelpBox(p1, "No alpha controllers found in heirarchy", MessageType.Warning);
						p1.y += p1.height;
					}
					else
					{
						index = EditorGUI.Popup(p1, alphaID.displayName, index, ids);
						alphaID.stringValue = ids[index];
						p1.y += p1.height;
						if (string.IsNullOrEmpty(alphaID.stringValue))
						{
							EditorGUI.HelpBox(p1, "Select a valid Alpha ID", MessageType.Warning);
							p1.y += p1.height;
						}
						else
						{
							EditorGUI.HelpBox(p1, "Could not find an alpha controller with Alpha ID", MessageType.Error);
							p1.y += p1.height;
						}
					}
				}
				else
				{
					EditorGUI.PropertyField(p1, alphaID);
					p1.y += p1.height;
				}
				break;
			case UnimaControllerMode.Alpha:
				alphaID.stringValue = EditorGUI.TextField(p1, new GUIContent(alphaID.displayName), alphaID.stringValue);
				p1.y += p1.height;
				if (string.IsNullOrEmpty(alphaID.stringValue))
				{
					EditorGUI.HelpBox(p1, "Beta controllers must have an Alpha ID", MessageType.Error);
					p1.y += p1.height;
				}
				if (property.serializedObject.targetObject is Component comp)
				{
					List<IUnimaControllerSource> sources = Core.ListPool<IUnimaControllerSource>.Request();
					List<UnimaController> controllers = Core.ListPool<UnimaController>.Request();
					UnimaUtil.GetBetas(comp.gameObject, alphaID.stringValue, sources, controllers);
					p1 = UnimaDrawerUtil.AddIndent(p1);
					foreach (IUnimaControllerSource src in sources)
					{
						if (src is Component cp)
						{
							EditorGUI.ObjectField(p1, UnimaControllerMode.Beta.ToString(), cp, typeof(Component), true);
							p1.y += p1.height;
						}
					}
					p1 = UnimaDrawerUtil.RemoveIndent(p1);
					Core.ListPool<IUnimaControllerSource>.Return(sources);
					Core.ListPool<UnimaController>.Return(controllers);
				}
				break;
			default:
				alphaID.stringValue = string.Empty;
				break;
		}

		if (property.NextVisible(true)) // Skip the first visible property as this is always the script reference
		{
			int depth = property.depth;
			do
			{
				p1.height = EditorGUI.GetPropertyHeight(property, true);
				EditorGUI.PropertyField(p1, property, true);
				p1.y += p1.height;
			}
			while (property.NextVisible(false) && property.depth == depth);
		}

		if (m_Controller != null && GUI.changed)
		{
			m_Controller.EditorSetDirty();
		}

		m_Height = p1.y - position.y;
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		float height = 0.0f;
		if (!property.isExpanded)
		{
			height = EditorGUIUtility.singleLineHeight;
		}
		else if (m_Height > Core.Util.EPSILON)
		{
			height = m_Height;
		}
		else
		{
			height += SPACE * EditorGUIUtility.singleLineHeight;
			if (property.NextVisible(true)) // Skip the first visible property as this is always the script reference
			{
				int depth = property.depth;
				do
				{
					height += EditorGUI.GetPropertyHeight(property, true);
				}
				while (property.NextVisible(false) && property.depth == depth);
			}
		}
		height += 3.0f * PADDING;
		return height;
	}

	private bool GetAlphaIDs(Component comp, string id, out int index, out string[] ids)
	{
		List<IUnimaControllerSource> sources = new List<IUnimaControllerSource>();
		UnimaUtil.FindControllerSources(comp.gameObject, sources);
		List<UnimaController> controllers = new List<UnimaController>(sources.Count);
		List<string> idList = new List<string>(controllers.Count);
		foreach (IUnimaControllerSource source in sources)
		{
			source.AddControllers(controllers);
		}
		for (int i = controllers.Count - 1; i >= 0; i--)
		{
			UnimaController controller = controllers[i];
			if (controller.Mode == UnimaControllerMode.Alpha &&
				!string.IsNullOrEmpty(controller.AlphaID))
			{
				idList.Add(controller.AlphaID);
			}
		}
		if (idList.Count == 0)
		{
			index = -1;
			ids = new string[] { };
			return false;
		}
		for (int i = 0; i < idList.Count; i++)
		{
			if (string.Equals(id, idList[i]))
			{
				index = i;
				ids = idList.ToArray();
				return true;
			}
		}
		index = 0;
		idList.Insert(0, id);
		ids = idList.ToArray();
		return false;
	}
}

