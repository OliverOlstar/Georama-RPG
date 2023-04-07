using System;
using System.Collections.Generic;
using UnityEngine;
using OliverLoescher;

public class CheatMenu : MonoBehaviour
{
	[AttributeUsage(AttributeTargets.Class)]
	public class IgnorePageAttribute : Attribute
	{
	}

	public static float Width => WIDTH* WIDTH_SCALE;
	public static float Height => HEIGHT * HEIGHT_SCALE;
	public static float HalfWidth => Width * 0.5f;
	public static float HalfHeight => Height * 0.5f;

	private const float WIDTH = 315f;
	private const float HEIGHT = (16.0f / 9.0f) * WIDTH;
	private const float WIDTH_SCALE = 1.0f;
	private const float HEIGHT_SCALE = 0.95f;
	private const float SPACE_SIZE = 5f;
	private const int BUTTONS_PER_PAGE = 3;

	public bool IsOpen { get; private set; }

#pragma warning disable CS0414 // No referenced in RELEASE
	[SerializeField]
	private GameObject m_InputBlocker = null;
#pragma warning restore CS0414

#if !RELEASE
	private PlayerPrefsString m_CurrentPageName;
	private Vector2 m_ScrollPosition = Vector2.zero;
	private List<CheatMenuPage> m_PageList = null;
	private Dictionary<string, CheatMenuPage> m_PageDict = null;
	private List<string> m_AvailablePageNames = null;
	private int m_TimeScaleHandle = Core.TimeScaleManager.INVALID_HANDLE;
	private int m_NavButtonGroupIndex = -1;
	private bool m_ExpandNavButtons = false;

	private readonly Rect m_BoxRect;

	CheatMenu()
	{
		m_PageList = new List<CheatMenuPage>();
		m_PageDict = new Dictionary<string, CheatMenuPage>();
		m_AvailablePageNames = new List<string>();
		foreach (Type type in Core.TypeUtility.GetTypesDerivedFrom(typeof(CheatMenuPage)))
		{
			if (type.IsAbstract || type.GetCustomAttributes(typeof(IgnorePageAttribute), true).Length > 0)
			{
				continue;
			}
			CheatMenuPage page = Activator.CreateInstance(type) as CheatMenuPage;
			m_PageList.Add(page);
			m_PageDict.Add(page.Name, page);
		}
		m_PageList.Sort(ComparePagesByPriority);
		m_BoxRect = new Rect(0f, 0f, WIDTH, HEIGHT);
	}

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		m_CurrentPageName = new PlayerPrefsString("IN_GAME_CHEAT_MENU_PAGE");
		for (int i = 0; i < m_PageList.Count; ++i)
		{
			Type t = m_PageList[i].GetType();
			m_PageList[i].OnInitialize();
		}
		m_InputBlocker.gameObject.SetActive(false);
		if (string.IsNullOrEmpty(m_CurrentPageName.Get()))
		{
			m_CurrentPageName.Set(m_PageList[0].Name);
		}
	}

	void Update()
	{
		//if (!IsOpen && Input.GetKeyDown(KeyCode.Tilde))
		//{
		//	Open();
		//}
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

		GUI.Box(m_BoxRect, string.Empty);
		Rect rect = CalculateScreenRect();
		GUILayout.BeginArea(rect);
		
		int selectedindex = -1;
		m_AvailablePageNames.Clear();
		for (int i = 0; i < m_PageList.Count; i++)
		{
			CheatMenuPage page = m_PageList[i];
			if (!page.IsAvailable())
			{
				continue;
			}
			m_AvailablePageNames.Add(page.Name);
			if (selectedindex < 0 && page.Name == m_CurrentPageName.Get())
			{
				selectedindex = m_AvailablePageNames.Count - 1;
			}
		}
		if (selectedindex < 0) // Page must have become unavailable
		{
			selectedindex = 0;
			m_CurrentPageName.Set(m_AvailablePageNames[0]);
			CheatMenuPage page = m_PageDict[m_AvailablePageNames[0]];
			page.OnBecameActive();
			m_ScrollPosition = Vector3.zero;
			m_NavButtonGroupIndex = -1;
		}
		GUILayout.BeginVertical(GUI.skin.box);
		if (m_ExpandNavButtons)
		{
			DrawExpandedButtons(selectedindex);
		}
		else
		{
			DrawCollapsedButtons(selectedindex);
		}
		GUILayout.EndVertical();

		GUI.color = Color.white;
		GUI.enabled = true;
		GUILayout.BeginVertical(GUI.skin.box);
		m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);
		if (m_PageDict.TryGetValue(m_CurrentPageName.Get(), out CheatMenuPage currentPage))
		{
			currentPage.DrawGUI();
			if (currentPage.CloseMenu)
			{
				currentPage.CloseMenu = false;
				Close();
			}
		}
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
		GUILayout.BeginVertical(GUI.skin.box);
		if (GUILayout.Button(GameStateManager.GameState == GameState.None ? "START GAME" : "EXIT"))
		{
			Close();
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
		GUI.skin.button = originalButton;
		GUI.skin.textField = originalTextField;
		GUI.matrix = matrix;
	}

	private void DrawCollapsedButtons(int selectedindex)
	{
		int buttonPageCount = m_AvailablePageNames.Count / BUTTONS_PER_PAGE;
		if (m_NavButtonGroupIndex < 0)
		{
			m_NavButtonGroupIndex = selectedindex / BUTTONS_PER_PAGE;
		}
		m_NavButtonGroupIndex = Mathf.Clamp(m_NavButtonGroupIndex, 0, m_AvailablePageNames.Count);
		GUILayout.BeginHorizontal();
		GUI.enabled = m_NavButtonGroupIndex > 0;
		if (GUILayout.Button("<<<"))
		{
			m_NavButtonGroupIndex--;
		}
		GUI.enabled = true;
		if (GUILayout.Button("Expand"))
		{
			m_ExpandNavButtons = true;
		}
		GUI.enabled = m_NavButtonGroupIndex < buttonPageCount;
		if (GUILayout.Button(">>>"))
		{
			m_NavButtonGroupIndex++;
		}
		GUILayout.EndHorizontal();
		GUI.enabled = true;

		
		int start = m_NavButtonGroupIndex * BUTTONS_PER_PAGE;
		int count = Mathf.Min(BUTTONS_PER_PAGE, m_AvailablePageNames.Count - start);

		string[] names = new string[count];
		m_AvailablePageNames.CopyTo(start, names, 0, count);
		int localSelectedIndex = selectedindex < start || selectedindex >= start + count ? -1 : selectedindex % BUTTONS_PER_PAGE;
		int newSelection = GUILayout.SelectionGrid(localSelectedIndex, names, count);
		if (newSelection != localSelectedIndex)
		{
			string pageName = names[newSelection];
			m_PageDict[pageName].OnBecameActive();
			m_CurrentPageName.Set(pageName);
			m_ScrollPosition = Vector3.zero;
		}
	}

	private void DrawExpandedButtons(int selectedIndex)
	{
		GUI.enabled = true;
		GUI.color = Color.white;
		if (GUILayout.Button("Collapse"))
		{
			m_ExpandNavButtons = false;
		}
		int newSelection = GUILayout.SelectionGrid(selectedIndex, m_AvailablePageNames.ToArray(), BUTTONS_PER_PAGE);
		if (newSelection != selectedIndex)
		{
			string newPageName = m_AvailablePageNames[newSelection];
			m_CurrentPageName.Set(newPageName);
			CheatMenuPage page = m_PageDict[newPageName];
			page.OnBecameActive();
			m_ScrollPosition = Vector3.zero;
			m_ExpandNavButtons = false;
			m_NavButtonGroupIndex = -1;
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

	public void ForceOpen()
	{
		Open();
	}

	private void Open()
	{
		IsOpen = true;
		m_InputBlocker.gameObject.SetActive(true);
		m_TimeScaleHandle = Core.TimeScaleManager.Exists() ? Core.TimeScaleManager.StartTimeEvent(0.0f) : Core.TimeScaleManager.INVALID_HANDLE;
		m_NavButtonGroupIndex = -1;
		if (!m_PageDict.TryGetValue(m_CurrentPageName.Get(), out CheatMenuPage page))
		{
			return;
		}
		if (!page.IsAvailable())
		{
			m_CurrentPageName.Set(string.Empty);
			return;
		}
		page.OnBecameActive();
	}

	public void Close()
	{
		Debug.Log("InGameCheatMenu: Close");
		IsOpen = false;
		m_InputBlocker.gameObject.SetActive(false);
		if (m_TimeScaleHandle != Core.TimeScaleManager.INVALID_HANDLE)
		{
			Core.TimeScaleManager.EndTimeEvent(m_TimeScaleHandle);
		}

		for (int i = 0; i < m_PageList.Count; ++i)
		{
			m_PageList[i].OnPostClose();
		}
	}

	private int ComparePagesByPriority(CheatMenuPage x, CheatMenuPage y)
	{
		if (x.Priority == y.Priority)
		{
			return x.Name.CompareTo(y.Name);
		}
		return x.Priority.CompareTo(y.Priority);
	}
#endif
}
