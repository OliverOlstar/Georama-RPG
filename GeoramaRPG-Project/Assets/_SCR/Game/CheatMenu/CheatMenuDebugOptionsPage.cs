using Core;
using UnityEngine;

public class CheatMenuDebugOptionsPage : CheatMenuPage
{
	public override CheatMenuGroup Group => CheatMenuOkoGroups.DEBUG;
	public override string Name => "Options";
	public override int Priority => -1;
	public override bool IsAvailable() => true;

	private DebugOption m_ActiveTooltip = null;
	private DebugOption m_ActivePresets = null;

	public override void DrawGUI()
	{
		// Timescale
		GUILayout.BeginHorizontal();
		if (TimeScaleManager.Get() != null)
		{
			if (GUILayout.Button(" < "))
			{
				TimeScaleManager.Get().EditorDec();
			}
			GUILayout.Label("Time Scale " + TimeScaleManager.Get().GetEditorSlowMo() + "%");
			if (GUILayout.Button(" > "))
			{
				TimeScaleManager.Get().EditorInc();
			}
		}
		GUILayout.EndHorizontal();

		GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
		titleStyle.fontStyle = FontStyle.Bold;

		GUIStyle toggleStyle = new GUIStyle(GUI.skin.label);
		toggleStyle.fontSize = 24;
		toggleStyle.alignment = TextAnchor.MiddleLeft;

		GUIStyle toggleTipStyle = new GUIStyle(toggleStyle);
		toggleTipStyle.fontSize = 16;

		GUIStyle buttonStyle = new GUIStyle(GUI.skin.label);
		buttonStyle.alignment = TextAnchor.MiddleLeft;

		// Debug Options
		foreach (string groupName in Core.DebugOption.GetGroupNames())
		{
			if (Core.DebugOption.Group.EditorOnlyGroups.Contains(groupName))
			{
				continue;
			}

			GUILayout.Label(groupName, titleStyle);
			foreach (Core.DebugOption op in Core.DebugOption.GetGroupOptions(groupName))
			{
				if (!op.CanShow())
				{
					continue;
				}

				bool hasTooltip = !string.IsNullOrEmpty(op.Tooltip);
				bool isTooltipOpen = m_ActiveTooltip == op;

				// Big button
				bool set = Core.DebugOption.IsSet(op);
				Rect r = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.button);
				if (hasTooltip)
				{
					r.width -= 20f;
				}
				if (GUI.Button(r, "", GUI.skin.box))
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

				// Big button Icon
				Rect r1 = r;
				r1.y -= 2.0f;
				r1.width = 16.0f;
				r1.x += 4.0f;
				GUI.Label(r1, set ? "\u25cf" : "\u25cb", toggleStyle);
				float toggleOffset = r1.x + r1.width;

				// Tooltip Button
				if (hasTooltip)
				{
					Rect r3 = r;
					r3.width = 20.0f;
					r3.x += r.width;
					if (GUI.Button(r3, isTooltipOpen ? "\u25bc" : "\u25c0", toggleTipStyle))
					{
						m_ActiveTooltip = isTooltipOpen ? null : op;
						isTooltipOpen = !isTooltipOpen;
					}
				}

				// Big button Label
				Rect r2 = r;
				r2.x += toggleOffset;
				r2.width -= toggleOffset;
				GUI.Label(r2, op.Name , buttonStyle);

				string arg = DebugOption.GetArg(op);
				int intArg = DebugOption.GetInt(op); 
				string newArg = arg;

				string[] argPresets;
				int currentIndex;

				if (set)
				{
					switch (op)
					{
						case DebugOption.String _:
							OnGUIString(arg, ref newArg, toggleOffset, 0.0f);
							break;

						case DebugOption.Int _:
							OnGUIInt(intArg, ref newArg, toggleOffset, 0.0f);
							break;

						case DebugOption.Slider opSlider:
							OnGUISlider(opSlider, intArg, ref newArg);
							break;

						case DebugOption.StringWithDropdown opStringDrop:
							bool hasArgPresets = opStringDrop.TryGetDropdownItems(out argPresets, out currentIndex);
							r = OnGUIString(arg, ref newArg, toggleOffset, hasArgPresets ? 25.0f : 0.0f);

							if (hasArgPresets)
							{
								r.x += r.width;
								r.width = 25.0f;
								bool isPresetsOpen = m_ActivePresets == op;
								if (GUI.Button(r, "-"))
								{
									m_ActivePresets = isPresetsOpen ? null : op;
									isPresetsOpen = !isPresetsOpen;
								}
								if (isPresetsOpen)
								{
									OnGUIDropdown(ref newArg, argPresets, currentIndex, 7);
								}
							}
							break;

						case DebugOption.Dropdown opDrop:
							if (!opDrop.TryGetDropdownItems(out argPresets, out currentIndex))
							{
								Debug.LogWarning($"DebugOptions {op.Name} is of type {nameof(DebugOption.Dropdown)} but arg presets is null or empty. This shouldn't happen");
								OnGUIString(arg, ref newArg, toggleOffset, 0.0f);
								break;
							}
							OnGUIDropdown(ref newArg, argPresets, currentIndex, 9);
							break;
					}
				}

				if (!Str.Equals(arg, newArg))
				{
					DebugOption.SetArg(op, newArg);
				}

				// Tooltip
				if (isTooltipOpen)
				{
					r = GUILayoutUtility.GetRect(new GUIContent(op.Tooltip), SmallStyle);
					r.x += 4.0f;
					r.width -= 4.0f;
					GUI.Label(r, op.Tooltip, SmallStyle);
				}
			}
		}

		if (GUILayout.Button("Reset all options"))
		{
			DebugOption.Reset();
		}
	}

	private Rect OnGUIString(in string arg, ref string newArg, in float xOffset, in float widthOffset)
	{
		Rect r = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.textField);
		r.x += xOffset;
		r.width -= xOffset + widthOffset;
		newArg = GUI.TextField(r, arg);
		return r;
	}

	private void OnGUIInt(in int intArg, ref string newArg, in float xOffset, in float widthOffset)
	{
		Rect r = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.textField);
		r.x += xOffset;
		r.width -= xOffset + widthOffset;
		newArg = GUI.TextField(r, intArg.ToString());
	}

	private void OnGUISlider(in DebugOption.Slider slider, in int intArg, ref string newArg)
	{
		Rect r = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.textField);
		r.width -= 45.0f;
		slider.GetSliderItems(out int min, out int max);
		newArg = Mathf.RoundToInt(GUI.HorizontalSlider(r, intArg, min, max)).ToString();
		r.x += r.width;
		r.width = 45.0f;
		newArg = GUI.TextField(r, newArg);
	}

	private void OnGUIDropdown(ref string newArg, string[] argPresets, int currentIndex, int itemsPerRow = -1)
	{
		int rows = 1;
		if (itemsPerRow <= 0)
		{
			itemsPerRow = argPresets.Length;
		}
		else
		{
			// Calculate rows required then rebalance items per row
			rows = Mathf.CeilToInt((float)argPresets.Length / itemsPerRow);
			itemsPerRow = Mathf.CeilToInt((float)argPresets.Length / rows);
		}

		for (int row = 0; row < rows; row++)
		{
			Rect r = GUILayoutUtility.GetRect(new GUIContent(), ExtraSmallButtonStyle);
			int startIndex = row * itemsPerRow;
			int endIndex = Mathf.Min(startIndex + itemsPerRow, argPresets.Length);
			r.width = r.width / (endIndex - startIndex);
			for (int i = startIndex; i < endIndex; i++)
			{
				if (currentIndex == i)
				{
					GUI.color = Color.yellow;
				}
				if (GUI.Button(r, argPresets[i], ExtraSmallButtonStyle))
				{
					newArg = argPresets[i];
				}
				GUI.color = Color.white;
				r.x += r.width;
			}
		}
	}
}