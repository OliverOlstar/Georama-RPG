using UnityEditor;
using UnityEngine;

namespace Core
{
	public class DebugOptionsWindow : EditorWindow
	{
		[MenuItem("Window/Debug/Options")]
		static void CreateWizard()
		{
			DebugOptionsWindow window = EditorWindow.GetWindow<DebugOptionsWindow>("Debug Options");
			window.Show();
		}

		private Vector2 m_ScrollPos = Vector2.zero;

		private void OnGUI()
		{
			//System.Type t = typeof(DebugOptions);
			//FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.Static);
			//List<System.Type> types = (from domainAssembly in System.AppDomain.CurrentDomain.GetAssemblies()
			//						   from assemblyType in domainAssembly.GetExportedTypes()
			//						   where assemblyType.IsSubclassOf(typeof(Gwar))
			//						   select assemblyType).ToList();

			//System.Type t = typeof(DebugOptions);
			//FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.Static);

			m_ScrollPos = GUILayout.BeginScrollView(m_ScrollPos);
			foreach (string group in DebugOption.GetGroupNames())
			{
				EditorGUILayout.LabelField(group, EditorStyles.boldLabel);
				foreach (DebugOption op in DebugOption.GetGroupOptions(group))
				{
					if (op.NeverShow())
					{
						continue;
					}
					GUI.enabled = op.CanShow();
					DrawDebugOption(op);
					GUI.enabled = true;
				}
			}

			GUILayout.Space(16.0f);
			if (GUILayout.Button("Reset Debug Options"))
			{
				DebugOption.Reset();
			}
			GUILayout.EndScrollView();
		}

		public static void DrawDebugOption(DebugOption op, string overrideName = null)
		{
			EditorGUILayout.BeginHorizontal();

			bool set = op.IsSet();
			string name = string.IsNullOrEmpty(overrideName) ? op.Name : overrideName;
			string label = string.IsNullOrEmpty(op.Tooltip) ? $"   {name}" : $"\u2055 {name}";
			if (EditorGUILayout.Toggle(new GUIContent(label, op.Tooltip), set) != set)
			{
				if (set)
				{
					op.Clear();
				}
				else
				{
					op.Set();
				}
			}

			string arg = DebugOption.GetArg(op);
			int intArg = DebugOption.GetInt(op);
			string newArg = arg;

			switch (op)
			{
				case DebugOption.Toggle _:
					EditorGUILayout.Space();
					break;

				case DebugOption.String _:
					newArg = EditorGUILayout.TextField(arg);
					break;

				case DebugOption.Int opInt:
					opInt.GetIntItems(out intArg);
					newArg = EditorGUILayout.IntField(intArg).ToString();
					break;

				case DebugOption.Slider opSlider:
					opSlider.GetSliderItems(out int min, out int max);
					newArg = EditorGUILayout.IntSlider(intArg, min, max).ToString();
					break;

				case DebugOption.StringWithDropdown opStringDrop:
					newArg = EditorGUILayout.TextField(arg);
					if (opStringDrop.TryGetDropdownItems(out string[] argPresets, out int currentIndex))
					{
						int index = EditorGUILayout.Popup(currentIndex, argPresets, GUILayout.Width(20));
						if (index >= 0 && currentIndex != index)
						{
							newArg = argPresets[index];
						}
					}
					break;

				case DebugOption.Dropdown opDrop:
					if (opDrop.TryGetDropdownItems(out argPresets, out currentIndex))
					{
						int index = EditorGUILayout.Popup(currentIndex, argPresets);
						if (index >= 0 && currentIndex != index)
						{
							newArg = argPresets[index];
						}
					}
					else
					{
						Debug.LogWarning($"DebugOptions {op.Name} is of type {nameof(DebugOption.Dropdown)} but arg presets is null or empty. This shouldn't happen");
						newArg = EditorGUILayout.TextField(arg);
					}
					break;
			}

			if (!Str.Equals(arg, newArg))
			{
				DebugOption.SetArg(op, newArg);
			}

			EditorGUILayout.EndHorizontal();
		}
	}
}
