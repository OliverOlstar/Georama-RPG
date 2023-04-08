using System;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class CheatMenu : MonoBehaviour
{
	[AttributeUsage(AttributeTargets.Class)]
	public class IgnorePageAttribute : Attribute { }

	[DebugOptionList]
	public static class DebugOptions
	{
		public static DebugOption LogCheatMenu = new DebugOption.Toggle(DebugOption.Group.Log, "Log CheatMenu", DebugOption.DefaultSetting.OnDevice);
	}

	public static float Width => WIDTH * WIDTH_SCALE;
	public static float Height => HEIGHT * HEIGHT_SCALE;
	public static float HalfWidth => Width * 0.5f;
	public static float HalfHeight => Height * 0.5f;

	private const float WIDTH = 315f;
	private const float HEIGHT = (16.0f / 9.0f) * WIDTH;
	private const float WIDTH_SCALE = 1.0f;
	private const float HEIGHT_SCALE = 0.95f;

	private const int BUTTONS_PER_ROW = 3;
	private const float MAX_BUTTON_WIDTH = 99.0f;
	private const int SPACE_PRIORITY_THRESHOLD = 1000;
	private const string CHEAT_MENU_GROUP_PREF = "CHEAT_MENU_GROUP";

#if RELEASE
	public static void Open() { }
	public static void Close() { }
	public static void CloseGroup() { }
	public static bool IsOpen => false;
#else
	private static CheatMenu s_Instance = null;
	
	public static void Open() { if (s_Instance != null) { s_Instance.OpenInternal(); } }
	public static void Close() { if (s_Instance != null) { s_Instance.CloseInternal(); } }
	public static void CloseGroup() { if (s_Instance != null) { s_Instance.CloseGroupInternal(); } }
	public static bool IsOpen => s_Instance != null ? s_Instance.m_IsOpen : false;

	private bool m_IsOpen = false;
	private PlayerPrefsString m_CurrentGroupName = null;
	private Vector2 m_ScrollPosition = Vector2.zero;
	private static CheatMenuGroup[] s_GroupArray = null;
	private static Dictionary<string, CheatMenuGroup> s_GroupDict;
	private int m_TimeScaleHandle = Core.TimeScaleManager.INVALID_HANDLE;
	private readonly Rect m_BoxRect = new Rect(0f, 0f, WIDTH, HEIGHT);
	private GUIStyle m_HeaderStyle = null;

#pragma warning disable CS0414 // No referenced in RELEASE
	[SerializeField]
	private GameObject m_InputBlocker = null;
#pragma warning restore CS0414

	protected virtual void OnAwake() { }
	protected virtual void OnOpened() { }
	protected virtual void OnClosed() { }

	void Awake()
	{
		if (s_Instance != null)
		{
			Core.DebugUtil.DevException("A new CheatMenu was just created but one already existed. CheatMenu is a singleton, this should never happen.");
		}
		s_Instance = this;

		DontDestroyOnLoad(gameObject);
		m_InputBlocker.gameObject.SetActive(false);

		if (s_GroupArray == null)
		{
			s_GroupDict = new Dictionary<string, CheatMenuGroup>();
			List<CheatMenuGroup> groupList = ListPool<CheatMenuGroup>.Request();
			foreach (Type type in Core.TypeUtility.GetTypesDerivedFrom(typeof(CheatMenuPage)))
			{
				if (type.IsAbstract || type.GetCustomAttributes(typeof(IgnorePageAttribute), true).Length > 0)
				{
					continue;
				}
				CheatMenuPage page = Activator.CreateInstance(type) as CheatMenuPage;
				if (!s_GroupDict.TryGetValue(page.Group.Name, out CheatMenuGroup group))
				{
					group = page.Group;
					s_GroupDict.Add(group.Name, group);
					groupList.Add(group);
				}
				group.AddPage(page);
			}
			s_GroupArray = groupList.ToArray();
			ListPool<CheatMenuGroup>.Return(groupList);
			Array.Sort(s_GroupArray, ComparePageGroupsByPriority);
		}

		m_CurrentGroupName = new PlayerPrefsString(CHEAT_MENU_GROUP_PREF);

		foreach (CheatMenuGroup group in s_GroupArray)
		{
			group.OnInitialize();
		}
		OnAwake();
	}

	void OnDestroy()
	{
		if (s_Instance == this)
		{
			s_Instance = null;
		}
		foreach (CheatMenuGroup group in s_GroupArray)
		{
			group.OnDestroy();
		}
	}

	void OnGUI()
	{
		if (!IsOpen)
		{
			return;
		}
		Matrix4x4 matrix = GUI.matrix;
		GUI.matrix = CalculateMatrix();

		// Scale up buttons and text fields to make them more pressable on mobile devices
		GUIStyle originalButton = GUI.skin.button;
		GUIStyle originalTextField = GUI.skin.textField;
		GUIStyle newButton = new GUIStyle(GUI.skin.button);
		GUIStyle newTextField = new GUIStyle(GUI.skin.textField);
		newButton.fixedHeight = 26;
		newTextField.fixedHeight = 26;
		GUI.skin.button = newButton;
		GUI.skin.textField = newTextField;

		if (m_HeaderStyle == null)
		{
			m_HeaderStyle = new GUIStyle(GUI.skin.label);
			m_HeaderStyle.alignment = TextAnchor.LowerCenter;
			m_HeaderStyle.fontStyle = FontStyle.Bold;
		}

		GUI.Box(m_BoxRect, string.Empty); // Background
		Rect rect = CalculateScreenRect();
		using (GUIUtil.UsableArea.Use(rect))
		{
			// Check current group
			bool hasCurrentGroup = false;
			CheatMenuGroup currentGroup = null;
			if (!string.IsNullOrEmpty(m_CurrentGroupName.Value) && s_GroupDict.TryGetValue(m_CurrentGroupName.Value, out currentGroup))
			{
				hasCurrentGroup = currentGroup.UpdateIsAvailable();
				if (!hasCurrentGroup)
				{
					m_CurrentGroupName.Value = string.Empty;
				}
			}

			// Draw
			if (hasCurrentGroup)
			{				
				DrawGroupMenuButtons(currentGroup);
				currentGroup.DrawGUI();
			}
			else
			{
				DrawMenuButtons();
				DrawPageSelection();
			}
		}
		GUI.skin.button = originalButton;
		GUI.skin.textField = originalTextField;
		GUI.matrix = matrix;
	}

	private void DrawGroupMenuButtons(CheatMenuGroup currentGroup)
	{
		using (GUIUtil.UsableHorizontal.Use(GUI.skin.box, GUILayout.ExpandHeight(false)))
		{
			if (GUILayout.Button("Back", GUILayout.Width(85.0f)))
			{
				currentGroup.OnPostClose();
				m_CurrentGroupName.Value = string.Empty;
			}
			GUILayout.Label(string.IsNullOrEmpty(m_CurrentGroupName.Value) ? string.Empty : m_CurrentGroupName.Value, m_HeaderStyle);
			if (GUILayout.Button("Close", GUILayout.Width(85.0f)))
			{
				Close();
			}
		}
	}

	private void DrawMenuButtons()
	{
		using (GUIUtil.UsableHorizontal.Use(GUI.skin.box, GUILayout.ExpandHeight(false)))
		{
			if (GUILayout.Button(GameStateManager.GameState == GameState.None ? "START GAME" : "Close"))
			{
				Close();
			}
		}
	}

	private void DrawPageSelection()
	{
		using (GUIUtil.UsableScrollRect.Use(ref m_ScrollPosition, GUI.skin.box))
		{
			int index = 0;
			bool newSection = false;

			for (int row = 0; index < s_GroupArray.Length && row < 99; row += BUTTONS_PER_ROW)
			{
				using (GUIUtil.UsableHorizontal.Use())
				{
					for (int column = 0; column < BUTTONS_PER_ROW && index < s_GroupArray.Length; column++)
					{
						CheatMenuGroup group = s_GroupArray[index];
						if (index > 0 && !newSection &&
							s_GroupArray[index - 1].Priority < group.Priority - SPACE_PRIORITY_THRESHOLD)
						{
							newSection = true;
							break;
						}
						newSection = false;
						GUI.enabled = group.IsAvailable();
						if (!GUI.enabled && group.HideWhenNotAvailable)
						{
							index++;
							column--;
							continue;
						}
						if (GUILayout.Button(group.Name, GUILayout.Width(MAX_BUTTON_WIDTH)))
						{
							m_CurrentGroupName.Value = group.Name;
							group.OnBecameActive();
							m_ScrollPosition = Vector3.zero;
						}
						index++;
					}
				}
				if (newSection)
				{
					GUILayout.Space(6.0f);
				}
			}
			GUI.enabled = true;
		}
	}

	private Matrix4x4 CalculateMatrix()
	{
		float width = WIDTH;
		float height = HEIGHT;

		if (Screen.width > Screen.height)
		{
			width = HEIGHT;
			height = WIDTH;
		}

		return Matrix4x4.TRS
		(
			Vector3.zero,
			Quaternion.identity,

			new Vector3(Screen.width / width, Screen.height / height, 1.0f)
		);
	}

	private Rect CalculateScreenRect()
	{
		float width = WIDTH;
		float height = HEIGHT;
		float widthScale = WIDTH_SCALE;
		float heightScale = HEIGHT_SCALE;

		if (Screen.width > Screen.height)
		{
			width = HEIGHT;
			height = WIDTH;
			widthScale = HEIGHT_SCALE;
			heightScale = WIDTH_SCALE;
		}

		return new Rect
		(
			0.5f * (1.0f - widthScale) * width,
			0.5f * (1.0f - heightScale) * height,
			widthScale * width,
			heightScale * height
		);
	}

	private void OpenInternal()
	{
		if (IsOpen)
		{
			return;
		}
		Log("Open");
		m_IsOpen = true;
		m_InputBlocker.gameObject.SetActive(true);
		m_TimeScaleHandle = Core.TimeScaleManager.Exists() ? Core.TimeScaleManager.StartTimeEvent(0.0f) : Core.TimeScaleManager.INVALID_HANDLE;
		OnOpened();
		if (!s_GroupDict.TryGetValue(m_CurrentGroupName.Value, out CheatMenuGroup currentGroup))
		{
			return;
		}
		if (!currentGroup.UpdateIsAvailable())
		{
			m_CurrentGroupName.Value = string.Empty;
			return;
		}
		currentGroup.OnBecameActive();
	}

	private void CloseInternal()
	{
		if (!m_IsOpen)
		{
			return;
		}
		Log("Close");
		m_IsOpen = false;
		m_InputBlocker.gameObject.SetActive(false);
		if (m_TimeScaleHandle != Core.TimeScaleManager.INVALID_HANDLE)
		{
			Core.TimeScaleManager.EndTimeEvent(m_TimeScaleHandle);
		}
		OnClosed();
		if (!string.IsNullOrEmpty(m_CurrentGroupName.Value) && s_GroupDict.TryGetValue(m_CurrentGroupName.Value, out CheatMenuGroup currentGroup))
		{
			currentGroup.OnPostClose();
		}
	}

	private void CloseGroupInternal()
	{
		if (!m_IsOpen || string.IsNullOrEmpty(m_CurrentGroupName.Value))
		{
			return;
		}
		if (s_GroupDict.TryGetValue(m_CurrentGroupName.Value, out CheatMenuGroup currentGroup))
		{
			currentGroup.OnPostClose();
		}
		m_CurrentGroupName.Value = string.Empty;
	}

	private int ComparePageGroupsByPriority(CheatMenuGroup x, CheatMenuGroup y)
	{
		if (x.Priority == y.Priority)
		{
			return x.Name.CompareTo(y.Name);
		}
		return x.Priority.CompareTo(y.Priority);
	}

	private void Log(string message)
	{
		if (DebugOptions.LogCheatMenu.IsSet())
		{
			Debug.Log($"[CheatMenu] {message}");
		}
	}
#endif
}
