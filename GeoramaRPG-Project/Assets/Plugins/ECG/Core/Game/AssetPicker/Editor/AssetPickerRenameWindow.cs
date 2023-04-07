using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetPickerRenameWindow : EditorWindow
{
	public static void Open(UnityEngine.Object obj, System.Type assetType, Vector2 position)
	{
		AssetPickerRenameWindow window = ScriptableObject.CreateInstance<AssetPickerRenameWindow>();

		window.m_Path = AssetDatabase.GetAssetPath(obj);
		window.m_NewName = Path.GetFileNameWithoutExtension(window.m_Path);

		string[] paths = Core.AssetDatabaseUtil.Find(assetType);
		List<UnityEngine.Object> assets = new List<Object>();
		Core.AssetDatabaseUtil.LoadAll(assetType, assets);
		string folder = Path.GetDirectoryName(window.m_Path);
		folder = folder.Replace('\\', '/');

		float width = 100.0f;
		for (int i = 0; i < assets.Count; i++)
		{
			UnityEngine.Object asset = assets[i];
			string path = AssetDatabase.GetAssetPath(asset);
			if (path.StartsWith(folder))
			{
				GUIContent content = EditorGUIUtility.ObjectContent(asset, asset.GetType());
				window.m_OtherAssets.Add(content);
				window.m_OtherAssets2.Add((path, content.text, content.image));
				float w = EditorStyles.label.CalcSize(content).x;
				width = Mathf.Max(width, w);
			}
		}

		window.ShowUtility();

		position.x -= 0.5f * width; // Center window

		Rect r = window.position;
		r.position = position;
		r.width = width;
		window.position = r;
	}

	private List<GUIContent> m_OtherAssets = new List<GUIContent>();
	private List<(string, string, Texture)> m_OtherAssets2 = new List<(string, string, Texture)>();
	private string m_Path = string.Empty;
	private string m_NewName = string.Empty;
	private Vector2 m_ScrollPos = Vector2.zero;
	private bool m_Focus = false;
	private bool m_ScrollTo = true;

	private void OnGUI()
	{
		string folder = Path.GetDirectoryName(m_Path);
		UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(folder);
		GUIContent c = EditorGUIUtility.ObjectContent(obj, obj.GetType());
		c.text = folder.Replace('\\','/').Replace("Assets/", "");
		EditorGUILayout.LabelField(c);

		Rect scrollViewRect = EditorGUILayout.GetControlRect(GUILayout.ExpandHeight(true));
		Rect scrollContentRect = scrollViewRect;
		scrollContentRect.height = m_OtherAssets2.Count * EditorGUIUtility.singleLineHeight;
		scrollContentRect.width -= GUI.skin.verticalScrollbar.fixedWidth;

		m_ScrollPos = GUI.BeginScrollView(scrollViewRect, m_ScrollPos, scrollContentRect);

		Rect itemRect = scrollContentRect;
		itemRect.height = EditorGUIUtility.singleLineHeight;

		EditorGUI.indentLevel++;
		itemRect = EditorGUI.IndentedRect(itemRect);
		EditorGUI.indentLevel--;

		foreach ((string, string, Texture) content in m_OtherAssets2)
		{
			if (m_Path == content.Item1)
			{
				if (m_ScrollTo)
				{
					GUI.ScrollTo(itemRect);
					m_ScrollTo = false;
				}
				Rect r1 = itemRect;
				r1.width = r1.height;
				GUI.DrawTexture(r1, content.Item3);
				Rect r2 = itemRect;
				r2.x += r1.width;
				r2.width -= r1.width;
				GUI.SetNextControlName("AssetPickerRenameTextField");
				m_NewName = GUI.TextField(r2, m_NewName);
			}
			else
			{
				EditorGUI.LabelField(itemRect, new GUIContent(content.Item2, content.Item3));
			}
			itemRect.y += itemRect.height;
		}

		//EditorGUI.DrawRect(scrollContentRect, Color.green);

		GUI.EndScrollView();

		//EditorGUI.indentLevel++;
		//m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
		//foreach ((string, string, Texture) content in m_OtherAssets2)
		//{
		//	if (m_Path == content.Item1)
		//	{
		//		EditorGUILayout.BeginHorizontal();
		//		//GUILayout.Label(new GUIContent(content.Item3), GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight), GUILayout.ExpandWidth(false));
		//		Rect r = EditorGUILayout.GetControlRect();
		//		r = EditorGUI.IndentedRect(r);
		//		//EditorGUI.DrawRect(r, Color.green);
		//		Rect r1 = r;
		//		r1.width = r1.height;
		//		GUI.DrawTexture(r1, content.Item3);
		//		Rect r2 = r;
		//		r2.x += r1.width;
		//		r2.width -= r1.width;
		//		//EditorGUI.DrawRect(r2, Color.green);
		//		GUI.SetNextControlName("AssetPickerRenameTextField");
		//		m_NewName = GUI.TextField(r2, m_NewName);
		//		//EditorGUI.DrawTextureTransparent(r1, content.Item3);
		//		//GUI.DrawTexture(r, content.Item3);
		//		//m_NewName = EditorGUILayout.TextField(m_NewName);
		//		EditorGUILayout.EndHorizontal();
		//	}
		//	else
		//	{
		//		EditorGUILayout.LabelField(new GUIContent(content.Item2, content.Item3));
		//	}
		//}
		//EditorGUILayout.EndScrollView();
		//EditorGUI.indentLevel--;

		if (!m_Focus)
		{
			m_Focus = true;
			GUI.FocusControl("AssetPickerRenameTextField");
		}

		string originalName = Path.GetFileNameWithoutExtension(m_Path);
		string newPath = m_Path.Replace(originalName, m_NewName);

		bool exists = File.Exists(newPath);
		if (exists)
		{
			EditorGUILayout.HelpBox("At asset with that name alread exists", MessageType.Warning);
			GUI.enabled = false;
			GUILayout.Button("Ok");
			GUI.enabled = true;
			return;
		}

		Event e = Event.current;
		if (e.keyCode == KeyCode.Return)
		{
			string error = AssetDatabase.RenameAsset(m_Path, m_NewName);
			Debug.Log(m_Path + " >>> " + newPath + "\nerror:" + error);
			Close();
		}

		if (GUILayout.Button("Ok"))
		{
			string error = AssetDatabase.RenameAsset(m_Path, m_NewName);
			Debug.Log(m_Path + " >>> " + newPath + "\nerror:" + error);
			Close();
		}
	}
}
