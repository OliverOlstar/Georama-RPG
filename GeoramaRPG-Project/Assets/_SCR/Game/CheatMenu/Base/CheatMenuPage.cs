using System;
using System.Collections.Generic;
using UnityEngine;
using Core;

public abstract class CheatMenuPage
{
	public virtual string Name => "Untitled";
	public virtual int Priority => 0;
	public abstract CheatMenuGroup Group { get; }

	public void Initialize()
	{
		OnInitialize();
	}

	protected virtual void OnInitialize() { }
	public virtual void OnDestroy() { }
	public virtual void OnBecameActive() { }
	public abstract void DrawGUI();
	public virtual void OnPostClose() { }


	// By default only show cheat pages when we're actually in game, most cheats won't work during the bootflow
	public virtual bool IsAvailable() => GameStateManager.IsInGame();

	protected void Close() { CheatMenu.Close(); }

	// TODO Move this out
	private static GUIStyle m_SmallStyle = null;
	public static GUIStyle SmallStyle 
	{
		get
		{
			if (m_SmallStyle == null)
			{
				m_SmallStyle = new GUIStyle();
				m_SmallStyle.fontSize = 10;
				m_SmallStyle.alignment = TextAnchor.MiddleLeft;
				m_SmallStyle.wordWrap = true;
				m_SmallStyle.normal.textColor = Color.white;
			}
			return m_SmallStyle;
		}
	}

	private static GUIStyle m_SmallButtonStyle = null;
	public static GUIStyle SmallButtonStyle 
	{
		get
		{
			if (m_SmallButtonStyle == null)
			{
				m_SmallButtonStyle = GUI.skin.button;
				m_SmallButtonStyle.fontSize = 10;
				m_SmallButtonStyle.wordWrap = true;
				m_SmallButtonStyle.normal.textColor = Color.white;
			}
			return m_SmallButtonStyle;
		}
	}

	private static GUIStyle m_ExtraSmallButtonStyle = null;
	public static GUIStyle ExtraSmallButtonStyle
	{
		get
		{
			if (m_ExtraSmallButtonStyle == null)
			{
				m_ExtraSmallButtonStyle = SmallButtonStyle;
				m_ExtraSmallButtonStyle.fontSize = 8;
			}
			return m_ExtraSmallButtonStyle;
		}
	}

	public static void DrawChallenge(in string id, in int progress, in int target, in bool isClaimable, in string propertyID, in string condition01, in string condition02, in Action<string, int> setProgress = null, in Action<string> claimChallenge = null, string additionalText = null)
		=> DrawChallenge(id, id, progress, target, isClaimable, propertyID, condition01, condition02, setProgress, claimChallenge, additionalText);
	public static void DrawChallenge(in string displayID, in string id, in int progress, in int target, in bool isClaimable, in string propertyID, in string condition01, in string condition02, in Action<string, int> setProgress = null, in Action<string> claimChallenge = null, string additionalText = null)
	{
		using (GUIUtil.UsableHorizontal.Use(GUI.skin.box))
		{
			// Label
			using (GUIUtil.UsableVertical.Use())
			{
				GUILayout.Label($"{displayID}: ", SmallStyle);
				if (!string.IsNullOrEmpty(condition01))
				{
					Str.Add($"_{condition01}");
				}
				if (!string.IsNullOrEmpty(condition02))
				{
					Str.Add($"_{condition02}");
				}
				GUILayout.Label($"({propertyID}{Str.Finish()})", SmallStyle);
				if (!string.IsNullOrEmpty(additionalText))
				{
					GUILayout.Label(additionalText, SmallStyle);
				}
			}
			// Set
			if (setProgress != null)
			{
				int value = GUIUtil.IntField(progress); // Input Field
				if (value != progress)
				{
					setProgress.Invoke(id, value);
				}
				GUILayout.Label($"/{target}", SmallStyle);
			}
			else
			{
				GUILayout.Label($"{progress}/{target}", SmallStyle);
			}
			// Claim
			GUI.enabled = isClaimable;
			if (claimChallenge != null && GUILayout.Button("Claim", SmallButtonStyle))
			{
				claimChallenge.Invoke(id);
			}
			GUI.enabled = true;
		}
	}
}