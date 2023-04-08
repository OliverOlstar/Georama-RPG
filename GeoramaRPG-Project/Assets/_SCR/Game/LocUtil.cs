using I2.Loc;
using UnityEngine;
using UnityEngine.UI;
using Core;
using System;

public static class LocUtil
{
	public static readonly string ENGLISH = "English";
	public static readonly string KOREAN = "Korean";

	public static readonly string THAI_LANGUAGE_CODE = "th";

	public static readonly string BUNDLE_NAME = "language_source";
	public static readonly string ASSET_NAME = "I2LanguagesBundled";

	public static readonly string TIMER_LOC_DAYS = "Menu_Timer_Short_Days";
	public static readonly string TIMER_LOC_HOURS = "Menu_Timer_Short_Hours";
	public static readonly string TIMER_LOC_MINUTES = "Menu_Timer_Short_Minutes";
	public static readonly string TIMER_LOC_SECONDS = "Menu_Timer_Short_Seconds";

	public static string CurrentLanguage => I2.Loc.LocalizationManager.CurrentLanguageCode;
	public static bool IsDebugLoc => ECGDebugOptions.ForceDebugLoc.IsSet() && (!ECGDebugOptions.ForceDebugLoc.IsStringSet(out string language) || string.IsNullOrEmpty(language));
	public static string DebugLanguage
	{
		get
		{
			string rt = null;
			if (ECGDebugOptions.ForceDebugLoc.IsSet() && ECGDebugOptions.ForceDebugLoc.IsStringSet(out string language))
			{
				rt = language;
			}

			return rt;
		}
	}

	public static string SavedLanguage
	{
		get
		{
			string language = null;
			language = PlayerPrefs.GetString("SelectedLanguage", null);
			return language;
		}
	}
	
	public static string Localize(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return Core.Str.EMPTY;
		}
		if (IsDebugLoc)
		{
			return key;
		}
		if (I2.Loc.LocalizationManager.TryGetTranslation(key, out string ret))
		{
			ret = ApplyTagFixes(ret);
			ret = ApplyLanguageSpecificFixes(ret);
			return ret;
		}
		return ErrorString(key);
	}

	private static string ApplyTagFixes(string ret)
	{
		// Remove Unsupported Unity Rich Text Tags; should consider some Data Structure with Unsupported Tags and their Replacements
		ret = ret.Replace("<br>", "\n");
		return ret; 
	}

	private static string ApplyLanguageSpecificFixes(string ret)
	{
		switch(LocalizationManager.CurrentLanguage)
		{
			// case string thaiLanguage when LocalizationManager.CurrentLanguageCode.Equals(THAI_LANGUAGE_CODE, System.StringComparison.OrdinalIgnoreCase):
			// 	ret = ThaiFontAdjuster.Adjust(ret);
			// 	break;
		}

		return ret;
	}

	public static bool TryLocalize(string key, out string text)
	{
		if (string.IsNullOrEmpty(key))
		{
			text = Core.Str.EMPTY;
			return false;
		}
		if (IsDebugLoc)
		{
			text = key;
			return true;
		}
		if (I2.Loc.LocalizationManager.TryGetTranslation(key, out string ret))
		{
			text = ret;
			return true;
		}
		text = ErrorString(key);
		return false;
	}

	public static string ErrorString(string str)
	{
		// If something goes wrong return the empty string in release, this is probably the least offensive looking option
		// In dev add some mark ups because sometimes people pass in already localized text as the key so it's obvious when this happens
		return Core.Util.IsRelease() ? Core.Str.EMPTY : Core.Str.Build("!!!", str, "!!!");
	}

	public static string Localize(string key, I2.Loc.Localize.TermModification modification)
	{
		string localizedString = Localize(key);
		if (modification == I2.Loc.Localize.TermModification.ToUpper)
		{
			localizedString = localizedString.ToUpper();
		}
		else if (modification == I2.Loc.Localize.TermModification.ToLower)
		{
			localizedString = localizedString.ToLower();
		}
		return localizedString;
	}

	public static string Localize(string key, params object[] args)
	{
		return SafeFormat(Localize(key), args);
	}

	public static string SafeFormat(string format, params object[] args)
	{
		try
		{
			return string.Format(format, args);
		}
		catch (System.Exception e)
		{
			Debug.LogWarning(Core.Str.Build("LocUtil.Localize: String with loc id '", format,
				"' is not formatted to accept ", args.Length.ToString(), " argument(s).\n", e.ToString()));
			return ErrorString(format);
		}
	}

	public static string LocalizeEnum(string e)
	{
		return Localize("Data_ENUM_" + e);
	}

	public static string LocalizeEnum(System.Enum e)
	{
		return LocalizeEnum(e.ToString());
	}

	public static string LocalizeEnum(System.Enum e, params object[] args)
	{
		return SafeFormat(LocalizeEnum(e), args);
	}

	public static string LocalizeGeneric(string s)
	{
		// Check if this is a number
		int i = 0;
		float f = 0.0f;
		if (int.TryParse(s, out i) || float.TryParse(s, out f))
		{
			return s;
		}
		// Otherwise asume it's a string that needs localization
		return LocUtil.Localize(s);
	}

	public static void AssignText(Text ui, string text)
	{
		ui.text = text;
		I2.Loc.Localize loc = ui.GetComponent<I2.Loc.Localize>();
		if (loc != null)
		{
			loc.enabled = false;
		}
	}

	public static void LocalizeAndAssignText(Text text, I2.Loc.Localize.TermModification modification, string key)
	{
		string localizedString = Localize(key);
		if (modification == I2.Loc.Localize.TermModification.ToUpper)
		{
			localizedString = localizedString.ToUpper();
		}
		else if (modification == I2.Loc.Localize.TermModification.ToLower)
		{
			localizedString = localizedString.ToLower();
		}

		text.text = localizedString;
		I2.Loc.Localize loc = text.GetComponent<I2.Loc.Localize>();
		if (loc != null)
		{
			loc.enabled = false;
		}
	}

	public static void LocalizeAndAssignText(Text text, string key)
	{
		LocalizeAndAssignText(text, I2.Loc.Localize.TermModification.DontModify, key);
	}

	public static void LocalizeAndAssignText(Text text, I2.Loc.Localize.TermModification modification, string key, params object[] args)
	{
		string localizedString = Localize(key, args);
		if (modification == I2.Loc.Localize.TermModification.ToUpper)
		{
			localizedString = localizedString.ToUpper();
		}
		else if (modification == I2.Loc.Localize.TermModification.ToLower)
		{
			localizedString = localizedString.ToLower();
		}

		text.text = localizedString;
		I2.Loc.Localize loc = text.GetComponent<I2.Loc.Localize>();
		if (loc != null)
		{
			loc.enabled = false;
		}
	}

	public static void LocalizeAndAssignText(Text text, string key, params object[] args)
	{
		LocalizeAndAssignText(text, I2.Loc.Localize.TermModification.DontModify, key, args);
	}

	public static void LocalizeAndAssignText(Text text, I2.Loc.Localize.TermModification modification, System.Enum e)
	{
		string localizedString = LocalizeEnum(e);
		if (modification == I2.Loc.Localize.TermModification.ToUpper)
		{
			localizedString = localizedString.ToUpper();
		}
		else if (modification == I2.Loc.Localize.TermModification.ToLower)
		{
			localizedString = localizedString.ToLower();
		}

		text.text = localizedString;
		I2.Loc.Localize loc = text.GetComponent<I2.Loc.Localize>();
		if (loc != null)
		{
			loc.enabled = false;
		}
	}

	public static void LocalizeAndAssignText(Text text, System.Enum e)
	{
		LocalizeAndAssignText(text, I2.Loc.Localize.TermModification.DontModify, e);
	}

	public class LocKeyAttribute : PropertyAttribute {}

	public static void UpdateSources(LanguageSourceAsset source)
	{
		if (source == null)
		{
			Debug.LogError("LocUtil.UpdateSources() Language source is null");
			return;
		}

		if (!LocalizationManager.Sources.Contains(source.SourceData))
		{
			LocalizationManager.AddSource(source.SourceData);
		}

		LocalizationManager.UpdateSources();
	}
}
