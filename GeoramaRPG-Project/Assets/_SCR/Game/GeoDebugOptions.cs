using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DebugOptionList]
public class GeoDebugOptions : MonoBehaviour
{
	public static class Group
	{
		public const string Boot = "Boot";
		public const string Battle = "Battle";
		public const string Cheat = "Cheat";
		public const string Seed = "Seed";
		public const string FTE = "FTE";
		public const string Quality = "Quality";
		public const string Store = "Store";
	}

	//public static readonly DebugOption PlayerInvincible = new DebugOption.Toggle(Group.Cheat, "Player Invincible");
	//public static readonly DebugOption EnemyInvincible = new DebugOption.Toggle(Group.Cheat, "Mob Invincible");
	//public static readonly DebugOption DisableKnockbackEffect = new DebugOption.Toggle(Group.Cheat, "Disable Knockback");
	//public static readonly DebugOption DisableAITeamA = new DebugOption.Toggle(Group.Cheat, "Player Disable AI");
	//public static readonly DebugOption DisableAITeamB = new DebugOption.Toggle(Group.Cheat, "Mob Disable AI");
	//public static readonly DebugOption CheatPlayerOneShotKills = new DebugOption.Toggle(Group.Cheat, "Player One Shot Kills");
	//public static readonly DebugOption CheatPlayerOneShotDeaths = new DebugOption.Toggle(Group.Cheat, "Player One Shot Deaths");

	//public static readonly DebugOption DefaultSeed = new DebugOption.Toggle(Group.Seed, "Use Default", releaseSetting: DebugOption.ReleaseSetting.Setable);
	//public static readonly DebugOption OverrideSeed = new DebugOption.Int(Group.Seed, "Override");

	//public static readonly DebugOption TutorialSkipAll = new DebugOption.Toggle(Group.FTE, "FTE Tutorial Skip All");

	//public static readonly DebugOption QualityOverride = new DebugOption.Dropdown(Group.Quality, "Quality Override", QualitySettingData.VALIDATE_IDS_EXIST);
	//public static readonly DebugOption QualityResolution = new DebugOption.Enum<QualitySettingData.Resolution>(Group.Quality, "Quality Resolution");
	//public static readonly DebugOption QualityShaders = new DebugOption.Enum<QualitySettingData.Shaders>(Group.Quality, "Quality Shaders");
	//public static readonly DebugOption QualityFrameRateCap = new DebugOption.Enum<QualitySettingData.FrameRateCap>(Group.Quality, "Quality Frame Rate Cap",
	//	tooltip: "CappedMax: Set to vsync as high as possible\nOff: Disable vsync");
	//public static readonly DebugOption QualityPost = new DebugOption.Enum<QualitySettingData.Post>(Group.Quality, "Quality Post");
	//public static readonly DebugOption QualityShadows = new DebugOption.Enum<QualitySettingData.Shadows>(Group.Quality, "Quality Shadows");
	//public static readonly DebugOption QualityEnvironmentFX = new DebugOption.Enum<QualitySettingData.EnvironmentFX>(Group.Quality, "Quality Environment FX");
	//public static readonly DebugOption QualityDynamicBones = new DebugOption.Slider(Group.Quality, "Quality Dynamic Bones", 0, 120,
	//	tooltip: "Set dynamic bones update rate. Requires hotfix.");

	//public static readonly DebugOption SkipToBattle = new DebugOption.Toggle(Group.Boot, "Boot to Battle");
	//public static readonly DebugOption SkipToSandbox = new DebugOption.Toggle(Group.Boot, "Boot to Sandbox");
	public static readonly DebugOption OpenCheatMenuOnBoot = new DebugOption.Toggle(Group.Boot, "Boot Open Cheat Menu", DebugOption.DefaultSetting.OnDevice);

	public static readonly DebugOption BackgroundAppInEditor = new DebugOption.Toggle(DebugOption.Group.Editor, "Background App in Editor");

	public static readonly DebugOption LogsFilter = new DebugOption.String(DebugOption.Group.Misc, "Logs Filter", DebugOption.DefaultSetting.Off); // TODO Move into cheatMenu itself
}