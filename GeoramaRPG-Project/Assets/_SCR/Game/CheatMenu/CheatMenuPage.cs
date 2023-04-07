using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class CheatMenuPage
{
	public abstract string Name { get; }

	public virtual void DrawGUI() { }
	public virtual void OnInitialize() { }
	public virtual void OnBecameActive() { }
	public virtual void OnPostClose() { }

	public virtual int Priority => CheatMenuPriority.Default;

	// By default only show cheat pages when we're actually in game, most cheat won't work during the bootflow
	public virtual bool IsAvailable() => GameStateManager.IsInGame();

	public bool CloseMenu;
	protected void Close() { CloseMenu = true; }
	
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

	protected static int IntField(int value)
	{
		int.TryParse(GUILayout.TextField(value.ToString()), out int result);
		return result;
	}

	protected int IntField(string name, int value)
	{
		GUILayout.Label(name);
		int.TryParse(GUILayout.TextField(value.ToString()), out int result);
		return result;
	}
	
	protected int IntField(string name, int value, int maxLength)
	{
		GUILayout.Label(name);
		int.TryParse(GUILayout.TextField(value.ToString(), maxLength), out int result);
		return result;
	}

	protected float FloatField(string name, float value)
	{
		GUILayout.Label(name);
		float.TryParse(GUILayout.TextField(value.ToString()), out float result);
		return result;
	}

	protected int MinMaxIntField(int value, int min, int max, int increment, string label)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label(label + ": " + value);
		if (GUILayout.Button("<"))
		{
			value -= increment;
		}
		if (GUILayout.Button(">"))
		{
			value += increment;
		}
		GUILayout.EndHorizontal();
		return Mathf.Clamp(value, min, max);
	}

	protected float MinMaxFloatField(float value, float min, float max, float increment, string label)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label(label + ": " + value);
		if (GUILayout.Button("<"))
		{
			value -= increment;
		}
		if (GUILayout.Button(">"))
		{
			value += increment;
		}
		GUILayout.EndHorizontal();
		return Mathf.Clamp(value, min, max);
	}

	protected int IntSlider(int value, int min, int max, string label)
	{
		using (new GUILayout.HorizontalScope())
		{
			GUILayout.Label(label);
			value = (int)GUILayout.HorizontalSlider(value, min, max);
		}
		return value;
	}

    protected int DrawTimeBox(string name, int value)
    {
        GUILayout.BeginHorizontal();
        value = IntField(name, value, 6);
        GUILayout.BeginHorizontal();
		float buttonWidth = 32.0f;
		if (GUILayout.Button("+m", GUILayout.Width(buttonWidth)))
        {
            value += 60;
        }
        if (GUILayout.Button("+h", GUILayout.Width(buttonWidth)))
        {
            value += 60 * 60;
        }
        if (GUILayout.Button("+d", GUILayout.Width(buttonWidth)))
        {
            value += 60 * 60 * 24;
        }
        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();
        return value;
    }

	protected void GameObjectOnOff(string name)
	{
		bool on = true;
		if (OnOff(name, ref on))
		{
			GameObject obj = GameObject.Find(name);
			if (obj != null)
			{
				obj.SetActive(on);
			}
		}
	}

	protected void ShaderOnOff(string name)
	{
		bool on = true;
		if (OnOff(name, ref on))
		{
			GameObject obj = GameObject.Find(name);
			if (obj != null)
			{
				Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
				foreach (Renderer renderer in renderers)
				{
					foreach (Material mat in renderer.materials)
					{
						mat.shader = Shader.Find("VertexLit");
					}
				}
			}
		}
	}

	private bool OnOff(string name, ref bool on)
	{
		GUILayout.BeginHorizontal();
		bool changed = false;
		if (!on && GUILayout.Button(name + " On"))
		{
			on = true;
			changed = true;
		}
		else if (on && GUILayout.Button(name + " Off"))
		{
			on = false;
			changed = true;
		}
		GUILayout.EndHorizontal();
		return changed;
	}

	public static void DrawChallenge(in string id, in int progress, in int target, in bool isClaimable, in string propertyID, in string condition01, in string condition02, in Action<string, int> setProgress = null, in Action<string> claimChallenge = null, string additionalText = null)
		=> DrawChallenge(id, id, progress, target, isClaimable, propertyID, condition01, condition02, setProgress, claimChallenge, additionalText);
	public static void DrawChallenge(in string displayID, in string id, in int progress, in int target, in bool isClaimable, in string propertyID, in string condition01, in string condition02, in Action<string, int> setProgress = null, in Action<string> claimChallenge = null, string additionalText = null)
	{
		GUILayout.BeginHorizontal(GUI.skin.box);

		// Label
		GUILayout.BeginVertical();
		GUILayout.Label($"{displayID}: ", SmallStyle);
		if (!string.IsNullOrEmpty(condition01))
		{
			Core.Str.Add($"_{condition01}");
		}
		if (!string.IsNullOrEmpty(condition02))
		{
			Core.Str.Add($"_{condition02}");
		}
		GUILayout.Label($"({propertyID}{Core.Str.Finish()})", SmallStyle);
		if (!string.IsNullOrEmpty(additionalText))
		{
			GUILayout.Label(additionalText, SmallStyle);
		}
		GUILayout.EndVertical();

		// Set
		if (setProgress != null)
		{
			int value = IntField(progress); // Input Field
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

		GUILayout.EndHorizontal();
	}
}