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
}