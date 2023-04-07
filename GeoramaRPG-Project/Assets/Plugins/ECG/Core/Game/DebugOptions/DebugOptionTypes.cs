using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core
{
	public partial class DebugOption : IComparable<DebugOption>
	{
		public class Toggle : DebugOption
		{
			public Toggle(
				string group,
				string name,
				DefaultSetting defaultSetting = DefaultSetting.Off,
				ReleaseSetting releaseSetting = ReleaseSetting.AlwaysOff,
				ShowIfType showIfType = ShowIfType.Always,
				string tooltip = null) :
					base(group, name, defaultSetting, releaseSetting, showIfType, tooltip)
			{ }
		}

		public class String : DebugOption
		{
			public String(
				string group,
				string name,
				DefaultSetting defaultSetting = DefaultSetting.Off,
				ReleaseSetting releaseSetting = ReleaseSetting.AlwaysOff,
				ShowIfType showIfType = ShowIfType.Always,
				string tooltip = null) :
					base(group, name, defaultSetting, releaseSetting, showIfType, tooltip)
			{ }
		}

		public class Int : DebugOption
		{
			public Int(
				string group,
				string name,
				DefaultSetting defaultSetting = DefaultSetting.Off,
				ReleaseSetting releaseSetting = ReleaseSetting.AlwaysOff,
				ShowIfType showIfType = ShowIfType.Always,
				string tooltip = null) :
					base(group, name, defaultSetting, releaseSetting, showIfType, tooltip)
			{ }

			public void GetIntItems(out int arg) => arg = GetArgInternal().Item2;
		}

		public class Slider : DebugOption
		{
			private int m_Min;
			private int m_Max;

			public Slider(
				string group,
				string name,
				int min,
				int max,
				DefaultSetting defaultSetting = DefaultSetting.Off,
				ReleaseSetting releaseSetting = ReleaseSetting.AlwaysOff,
				ShowIfType showIfType = ShowIfType.Always,
				string tooltip = null) :
					base(group, name, defaultSetting, releaseSetting, showIfType, tooltip)
			{
				m_Min = min;
				m_Max = max;
			}

			public void GetSliderItems(out int min, out int max)
			{
				int arg = GetArgInternal().Item2;
				min = Mathf.Min(arg, m_Min);
				max = Mathf.Max(arg, m_Max);
			}
		}

		public class Dropdown : DebugOption
		{
			protected string[] m_ArgPresets = null;
			protected string[] m_ArgPresetsAndCurrent = null;

			public Dropdown(
				string group,
				string name,
				DefaultSetting defaultSetting = DefaultSetting.Off,
				ReleaseSetting releaseSetting = ReleaseSetting.AlwaysOff,
				ShowIfType showIfType = ShowIfType.Always,
				params string[] argPresets) :
					base(group, name, defaultSetting, releaseSetting, showIfType, null)
			{
				SetArgPresets(argPresets);
			}

			public Dropdown(
				string group,
				string name,
				string[] argPresets,
				DefaultSetting defaultSetting = DefaultSetting.Off,
				ReleaseSetting releaseSetting = ReleaseSetting.AlwaysOff,
				ShowIfType showIfType = ShowIfType.Always,
				string tooltip = null) :
					base(group, name, defaultSetting, releaseSetting, showIfType, tooltip)
			{
				SetArgPresets(argPresets);
			}

			public Dropdown(
				string group,
				string name,
				IReadOnlyCollection<string> argPresets,
				DefaultSetting defaultSetting = DefaultSetting.Off,
				ReleaseSetting releaseSetting = ReleaseSetting.AlwaysOff,
				ShowIfType showIfType = ShowIfType.Always,
				string tooltip = null) :
					base(group, name, defaultSetting, releaseSetting, showIfType, tooltip)
			{
				SetArgPresets(argPresets.ToArray());
			}

			private void SetArgPresets(in string[] args)
			{
				m_ArgPresets = args;
				if (m_ArgPresets == null)
				{
					m_ArgPresetsAndCurrent = null;
					return;
				}
				m_ArgPresetsAndCurrent = new string[args.Length + 1];
				for (int i = 0; i < args.Length; i++)
				{
					m_ArgPresetsAndCurrent[i] = args[i];
				}
			}

			public bool TryGetDropdownItems(out string[] argPresets, out int currentIndex)
			{
				argPresets = m_ArgPresets;
				if (argPresets == null)
				{
					currentIndex = -1;
					return false;
				}
				string arg = GetArgInternal().Item1;
				if (argPresets.Length > 0 && !string.IsNullOrEmpty(arg) && !m_ArgPresets.Contains(arg))
				{
					m_ArgPresetsAndCurrent[m_ArgPresetsAndCurrent.Length - 1] = arg;
					argPresets = m_ArgPresetsAndCurrent;
				}
				currentIndex = GetCurrentArgIndex();
				return argPresets.Length > 0;
			}

			public int GetCurrentArgIndex()
			{
				string arg = GetArgInternal().Item1;
				if (!string.IsNullOrEmpty(arg))
				{
					return Array.IndexOf(m_ArgPresetsAndCurrent, arg);
				}
				return -1;
			}
		}

		public class StringWithDropdown : Dropdown
		{
			public StringWithDropdown(
				string group,
				string name,
				DefaultSetting defaultSetting = DefaultSetting.Off,
				ReleaseSetting releaseSetting = ReleaseSetting.AlwaysOff,
				ShowIfType showIfType = ShowIfType.Always,
				params string[] argPresets) :
					base(group, name, argPresets, defaultSetting, releaseSetting, showIfType, null)
			{ }

			public StringWithDropdown(
				string group,
				string name,
				string[] argPresets,
				DefaultSetting defaultSetting = DefaultSetting.Off,
				ReleaseSetting releaseSetting = ReleaseSetting.AlwaysOff,
				ShowIfType showIfType = ShowIfType.Always,
				string tooltip = null) :
					base(group, name, argPresets, defaultSetting, releaseSetting, showIfType, tooltip)
			{ }
		}

		public class Bool : Dropdown
		{
			private static readonly string[] ARGUMENTS = new string[] { "true", "false" };

			public Bool(
				string group,
				string name,
				DefaultSetting defaultSetting = DefaultSetting.Off,
				ReleaseSetting releaseSetting = ReleaseSetting.AlwaysOff,
				ShowIfType showIfType = ShowIfType.Always,
				string tooltip = null) :
					base(group, name, ARGUMENTS, defaultSetting, releaseSetting, showIfType, tooltip)
			{ }
		}

		public class Enum<TEnum> : Dropdown where TEnum : Enum
		{
			public Enum(
				string group,
				string name,
				DefaultSetting defaultSetting = DefaultSetting.Off,
				ReleaseSetting releaseSetting = ReleaseSetting.AlwaysOff,
				ShowIfType showIfType = ShowIfType.Always,
				string tooltip = null) :
					base(group, name, Enum.GetNames(typeof(TEnum)), defaultSetting, releaseSetting, showIfType, tooltip)
			{

			}
		}
	}
}