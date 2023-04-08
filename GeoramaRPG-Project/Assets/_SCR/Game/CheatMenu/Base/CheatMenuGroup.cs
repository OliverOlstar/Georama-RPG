using System.Drawing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class CheatMenuGroup
{
	public string Name { get; private set; }
	public int Priority { get; private set; }
	public bool HideWhenNotAvailable { get; private set; }
	private List<CheatMenuPage> m_Pages = new List<CheatMenuPage>();
	private List<CheatMenuPage> m_AvailablePages = new List<CheatMenuPage>();
	private string[] m_PageNames;
	private Func<bool> m_IsAvaliable;

	public CheatMenuGroup(string name, int priority = 0, Func<bool> isAvaliable = null, bool hideWhenNotAvaliable = false)
	{
		Name = name;
		Priority = priority;
		m_IsAvaliable = isAvaliable;
		HideWhenNotAvailable = hideWhenNotAvaliable;
	}

#if !RELEASE
	private int m_CurrentIndex = -1;
	private bool m_IsInitalized = false;
	private Vector2 m_ScrollPosition = Vector2.zero;
	
	public bool IsAvailable()
	{
		if (m_IsAvaliable != null && !m_IsAvaliable.Invoke())
		{
			return false;
		}
		foreach (CheatMenuPage page in m_Pages)
		{
			if (page.IsAvailable())
			{
				return true;
			}
		}
		return false;
	}

	public void AddPage(CheatMenuPage page)
	{
		m_Pages.Add(page);
		if (m_IsInitalized)
		{
			page.Initialize();
			m_Pages.Sort(ComparePagesByPriority);
		}
	}

	public void OnInitialize()
	{
		foreach (CheatMenuPage page in m_Pages)
		{
			page.Initialize();
		}
		m_Pages.Sort(ComparePagesByPriority);
		m_IsInitalized = true;
	}

	public void OnDestroy()
	{
		foreach (CheatMenuPage page in m_Pages)
		{
			page.OnDestroy();
		}
	}

	public void OnBecameActive()
	{
		UpdateIsAvailable(true);
	}

	public bool UpdateIsAvailable(bool forceBecomeActive = false)
	{
		if (m_IsAvaliable != null && !m_IsAvaliable.Invoke())
		{
			return false;
		}

		CheatMenuPage lastPage = m_AvailablePages.Count > m_CurrentIndex && m_CurrentIndex >= 0 ? m_AvailablePages[m_CurrentIndex] : null;

		// IsAvailable
		m_AvailablePages.Clear();
		foreach (CheatMenuPage page in m_Pages)
		{
			if (page.IsAvailable())
			{
				m_AvailablePages.Add(page);
			}
		}
		if (m_AvailablePages.Count == 0)
		{
			Log($"Closing group, no available pages.");
			m_CurrentIndex = -1;
			CheatMenu.CloseGroup(); // Exit
			return false;
		}

		// Build PageNames
		m_CurrentIndex = 0;
		if (m_PageNames == null || m_PageNames.Length != m_AvailablePages.Count)
		{
			m_PageNames = new string[m_AvailablePages.Count];
		}
		for (int i = 0; i < m_AvailablePages.Count; i++)
		{
			m_PageNames[i] = m_AvailablePages[i].Name;
			if (m_AvailablePages[i] == lastPage)
			{
				m_CurrentIndex = i;
			}
		}

		// OnBecameActive
		CheatMenuPage currentPage = m_AvailablePages[m_CurrentIndex];
		if (forceBecomeActive)
		{
			Log($"Opening page '{m_AvailablePages[m_CurrentIndex].Name}'");
			currentPage.OnBecameActive();
			return true;
		}
		if (currentPage != lastPage)
		{
			Log($"Page availablity changed -> closing page '{(lastPage != null ? lastPage.Name : "")}' & opening page '{currentPage.Name}'");
			if (lastPage != null)
			{
				lastPage.OnPostClose();
			}
			currentPage.OnBecameActive();
		}
		return true;
	}

	public void DrawGUI()
	{
		if (m_AvailablePages.Count == 0)
		{
			GUILayout.Label($"No Available Pages");
			return;
		}

		// Tabs
		int tabCount = GetTabCount();
		if (tabCount > 0)
		{
			using (GUIUtil.UsableVertical.Use(GUI.skin.box))
			{
				int index = GUILayout.SelectionGrid(m_CurrentIndex, m_PageNames, tabCount);
				if (index != m_CurrentIndex)
				{
					Log($"Switching page -> closing page '{m_AvailablePages[m_CurrentIndex].Name}' & opening page '{m_AvailablePages[index].Name}'");
					m_AvailablePages[m_CurrentIndex].OnPostClose();
					m_CurrentIndex = index;
					m_AvailablePages[m_CurrentIndex].OnBecameActive();
				}
			}
		}

		// Page
		using (GUIUtil.UsableScrollRect.Use(ref m_ScrollPosition))
		{
			m_AvailablePages[m_CurrentIndex].DrawGUI();
		}
	}

	private int GetTabCount()
	{
		switch (m_AvailablePages.Count)
		{
			case 1:
				return 0;
			case 2:
				return 2;
			case 3:
			case 5:
			case 6:
			case 9:
				return 3;
			case 4:
			default:
				return 4;
		}
	}

	public void OnPostClose()
	{
		if (m_CurrentIndex >= 0 && m_CurrentIndex < m_AvailablePages.Count)
		{
			Log($"Closing page '{m_AvailablePages[m_CurrentIndex].Name}'");
			m_AvailablePages[m_CurrentIndex].OnPostClose();
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

	private void Log(string message)
	{
		if (CheatMenu.DebugOptions.LogCheatMenu.IsSet())
		{
			Debug.Log($"[CheatMenu][{Name}] {message}");
		}
	}
#endif
}
