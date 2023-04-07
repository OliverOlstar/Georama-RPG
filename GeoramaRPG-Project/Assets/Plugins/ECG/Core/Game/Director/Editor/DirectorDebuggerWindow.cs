
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Core
{
	public class DirectorDebuggerWindow : EditorWindow
	{
		[MenuItem("Window/Debug/Director Debugger")]
		private static void CreateWizard()
		{
			DirectorDebuggerWindow window = GetWindow<DirectorDebuggerWindow>("Directors");
			window.Show();
		}

		private Vector2 m_Scroll = Vector2.zero;

		private void OnInspectorUpdate()
		{
			Repaint();
		}

		private void OnGUI()
		{
			List<IDirector> directors = ListPool<IDirector>.Request();
			Director.GetAll(directors);
			directors.Sort(Compare);
			m_Scroll = GUILayout.BeginScrollView(m_Scroll);
			foreach (IDirector dir in directors)
			{
				GUILayout.Label(dir.GetType().Name + "-" + (Director.IsPersistent(dir) ? "Persistent" : "Transient"));
			}
			GUILayout.EndScrollView();
			ListPool<IDirector>.Return(directors);
		}

		private static int Compare(IDirector a, IDirector b)
		{
			return a.GetType().Name.CompareTo(b.GetType().Name);
		}
	}
}
