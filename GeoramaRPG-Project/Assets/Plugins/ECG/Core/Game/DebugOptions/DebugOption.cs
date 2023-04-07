using System;
using UnityEngine;

namespace Core
{
	public class DebugOptionList : System.Attribute {}

	public partial class DebugOption : IComparable<DebugOption>
	{
		public enum DefaultSetting
		{
			Off = 0,
			On,
			OnInEditor,
			OnDevice
		}

		public enum ReleaseSetting
		{
			AlwaysOff = 0,
			AlwaysOn,
			Setable
		}

		public enum ShowIfType
		{
			Always = 0,
			IfInEditor,
			IfNotInEditor,
			IfIsPlaying,
			IfNotIsPlaying,
			Never
		}

		private string m_Group;
		private string m_Name;
		private string m_ArgKey;
		(string, int)? m_Arg;
		private bool? m_Set;
		private DefaultSetting m_Default;
		private ShowIfType m_ShowIfType;
		private ReleaseSetting m_Release;
		private string m_Tooltip;

		public string Name => m_Name;
		public string GroupName => m_Group;
		public string Tooltip => m_Tooltip;

		private DebugOption(
			string group,
			string name,
			DefaultSetting defaultSetting = DefaultSetting.Off,
			ReleaseSetting releaseSetting = ReleaseSetting.AlwaysOff,
			ShowIfType showIfType = ShowIfType.Always,
			string tooltip = null)
		{
			m_Group = group;
			m_Name = Core.Util.IsRelease() ? $"Release_{name}" : name; // Prevent bugs when installing a release build over top of a dev build
			m_ArgKey = m_Name + "Arg";
			m_Default = defaultSetting;
			m_Release = releaseSetting;
			m_ShowIfType = showIfType;
			m_Tooltip = tooltip;
			m_Set = null;
			m_Arg = null;
			IsSet();
			GetArgInternal();
			RegisterOption(this);
		}

		protected virtual void ResetInternal()
		{
			SetArgInternal(null);
			SetDefault();
		}

		public int CompareTo(DebugOption obj)
		{
			return m_Name.CompareTo(obj.m_Name);
		}

		public bool IsSet()
		{
			if (Util.IsRelease())
			{
				switch (m_Release)
				{
					case ReleaseSetting.AlwaysOff:
						return false;
					case ReleaseSetting.AlwaysOn:
						return true;
				}
			}

			if (m_Set.HasValue)
			{
				return m_Set.Value;
			}
			bool defaultValue = SetDefault();
#if UNITY_EDITOR
			m_Set = UnityEditor.EditorPrefs.GetBool(m_Name, defaultValue);
#else
			m_Set = PlayerPrefs.GetInt(m_Name, defaultValue ? 1 : 0) > 0;
#endif
			return m_Set.Value;
		}
		private bool SetDefault()
		{
			bool defaultValue = false;
			switch (m_Default)
			{
				case DefaultSetting.Off:
					defaultValue = false;
					break;
				case DefaultSetting.On:
					defaultValue = true;
					break;
				case DefaultSetting.OnInEditor:
					defaultValue = Application.isEditor ? true : false;
					break;
				case DefaultSetting.OnDevice:
					defaultValue = Application.isEditor ? false : true;
					break;
			}
			m_Set = defaultValue;
			return defaultValue;
		}
		public void Set()
		{
			m_Set = true;
#if UNITY_EDITOR
			if (Application.isEditor)
			{
				UnityEditor.EditorPrefs.SetBool(m_Name, true);
			}
			else
#endif
			{
				PlayerPrefs.SetInt(m_Name, 1);
			}
		}
		public void Clear()
		{
			m_Set = false;
#if UNITY_EDITOR
			if (Application.isEditor)
			{
				UnityEditor.EditorPrefs.SetBool(m_Name, false);
			}
			else
#endif
			{
				PlayerPrefs.SetInt(m_Name, 0);
			}
		}

		public void SetString(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				Clear();
				SetArgInternal(string.Empty);
			}
			else
			{
				Set();
				SetArgInternal(s);
			}
		}
		public void SetInt(int i)
		{
			Set();
			SetArgInternal(i.ToString());
		}

		protected void SetArgInternal(string s)
		{
			m_Arg = (s, int.TryParse(s, out int i) ? i : 0);
#if UNITY_EDITOR
			UnityEditor.EditorPrefs.SetString(m_ArgKey, s);
#else
			PlayerPrefs.SetString(m_ArgKey, s);
#endif
		}

		private (string, int) GetArgInternal()
		{
			if (m_Arg.HasValue)
			{
				return m_Arg.Value;
			}
#if UNITY_EDITOR
			string arg = UnityEditor.EditorPrefs.GetString(m_ArgKey, Core.Str.EMPTY);
#else
			string arg = PlayerPrefs.GetString(m_ArgKey, Core.Str.EMPTY);
#endif
			m_Arg = (arg, int.TryParse(arg, out int i) ? i : 0);
			return m_Arg.Value;
		}

		public bool IsStringSet(out string arg)
		{
			if (!IsSet())
			{
				arg = null;
				return false;
			}
			arg = GetArgInternal().Item1;
			return !string.IsNullOrEmpty(arg);
		}

		public bool IsIntSet(out int arg)
		{
			if (!IsSet())
			{
				arg = 0;
				return false;
			}
			arg = GetArgInternal().Item2;
			return true;
		}

		public bool IsEnumSet<T>(out T e, bool throwExceptionOnParseFail = false) where T : struct, System.Enum
		{
			if (!IsSet())
			{
				e = default;
				return false;
			}
			string arg = GetArgInternal().Item1;
			if (string.IsNullOrEmpty(arg))
			{
				e = default;
				if (throwExceptionOnParseFail)
				{
					Core.DebugUtil.DevException($"Debug Option '{m_Name}' string argument cannot be empty, " +
						$"it must be a valid entry in Enum of type {typeof(T).Name}: {string.Join(", ", System.Enum.GetNames(typeof(T)))}");
				}
				return false;
			}
			if (System.Enum.TryParse(arg, out e))
			{
				return true;
			}
			if (throwExceptionOnParseFail)
			{
				Core.DebugUtil.DevException($"Debug Option '{m_Name}' cannot parse '{arg}' to Enum of type {typeof(T).Name}: {string.Join(", ", System.Enum.GetNames(typeof(T)))}");
			}
			return false;
		}

		public bool CanShow()
		{
			switch (m_ShowIfType)
			{
				case ShowIfType.IfInEditor:
					return Application.isEditor ? true : false;
				case ShowIfType.IfNotInEditor:
					return Application.isEditor ? false : true;
				case ShowIfType.IfIsPlaying:
					return Application.isPlaying ? true : false;
				case ShowIfType.IfNotIsPlaying:
					return Application.isPlaying ? false : true;
				case ShowIfType.Never:
					return false;
				default: // ShowIfType.Always
					return true;
			}
		}

		public bool NeverShow() => m_ShowIfType == ShowIfType.Never;
	}
}
