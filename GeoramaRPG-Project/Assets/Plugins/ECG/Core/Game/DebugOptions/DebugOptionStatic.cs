
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Core
{
	public partial class DebugOption
	{
		public static class Group
		{
			public const string Log = "Log";
			public const string Draw = "Draw";
			public const string Editor = "Editor";
			public const string Data = "Data";
			public const string Build = "Build";
			public const string Misc = "Misc";

			public static readonly HashSet<string> EditorOnlyGroups = new HashSet<string>
			{
				Draw,
				Editor
			};
		}

		private static bool s_Initialized = false;
		//private static HashSet<string> s_OptionNames = new HashSet<string>();
		//private static List<DebugOption> s_Options = new List<DebugOption>();

		private static SortedDictionary<string, SortedDictionary<string, DebugOption>> s_Options =
			new SortedDictionary<string, SortedDictionary<string, DebugOption>>();

		private static void Initialize()
		{
			if (s_Initialized)
			{
				return;
			}
			// Static constructor get's called when the game runs, but this coded is needed to reload
			// the debug options after code compiles while out of play mode
			foreach (System.Type type in TypeUtility.GetTypesWithAttribute(typeof(DebugOptionList)))
			{
				// Calling static constructor for class should register all options
				RuntimeHelpers.RunClassConstructor(type.TypeHandle);
			}
			s_Initialized = true;
		}

		public static IEnumerable<DebugOption> GetAllOptions()
		{
			Initialize();
			foreach (SortedDictionary<string, DebugOption> group in s_Options.Values)
			{
				foreach (DebugOption op in group.Values)
				{
					yield return op;
				}
			}
		}

		public static IEnumerable<string> GetGroupNames()
		{
			Initialize();
			return s_Options.Keys;
		}

		public static IEnumerable<DebugOption> GetGroupOptions(string groupName)
		{
			Initialize();
			if (!s_Options.TryGetValue(groupName, out SortedDictionary<string, DebugOption> group))
			{
				yield break;
			}
			foreach (DebugOption option in group.Values)
			{
				yield return option;
			}
		}

		public static void Reset()
		{
			foreach (DebugOption option in GetAllOptions())
			{
				option.ResetInternal();
			}
		}

		public static void RegisterOption(DebugOption option)
		{
			if (!s_Options.TryGetValue(option.GroupName, out SortedDictionary<string, DebugOption> group))
			{
				group = new SortedDictionary<string, DebugOption>();
				s_Options.Add(option.GroupName, group);
			}
			if (!group.ContainsKey(option.Name))
			{
				group.Add(option.Name, option);
			}
			else
			{
				Debug.LogError($"DebugOptionsManager.RegisterOption() {option.Name} is already registered");
			}
		}

		public static bool IsAnyOptionInGroupSet(string groupName)
		{
			Initialize();
			if (!s_Options.TryGetValue(groupName, out SortedDictionary<string, DebugOption> group))
			{
				return false;
			}
			foreach (DebugOption op in group.Values)
			{
				if (op.IsSet())
				{
					return true;
				}
			}
			return false;
		}

		// DEPRECATED: This static function is just maintained for legacy code
		public static bool IsSet(DebugOption option) => option.IsSet();
		public static void SetArg(DebugOption option, string arg) => option.SetArgInternal(arg);
		public static string GetArg(DebugOption option) => option.GetArgInternal().Item1;
		public static int GetInt(DebugOption option) => option.GetArgInternal().Item2;
	}
}
