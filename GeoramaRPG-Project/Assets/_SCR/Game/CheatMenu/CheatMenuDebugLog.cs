using System;
using System.Collections.Generic;
using UnityEngine;

public class CheatMenuDebugLog : CheatMenuPage
{
	public override string Name => "Logs";
	private static Vector2 m_ScrollPosition = Vector2.zero;
	private static string[] m_Filters = null;
	private static string m_SourceFilters = string.Empty;
	private static readonly char[] SPLIT = new char[] { ',' };
	private static string m_LastLog = Core.Str.EMPTY;
	private static int m_Duplicate = 0;
	private static string m_LastError = Core.Str.EMPTY;
	public static string LastError { get { return m_LastError; } }

	private static Queue<string> m_Logs = new Queue<string>();
	private static bool m_Pause = false;

	public override bool IsAvailable() => true;

	public static void Init()
	{
		Core.Str.Flush();
		Application.logMessageReceived += HandleLog;
	}

	public void OnApplicationQuit()
	{
		Application.logMessageReceived -= HandleLog;
	}

	private static void HandleLog(string logString, string stackTrace, LogType type)
	{
		if (type != LogType.Exception && GeoDebugOptions.LogsFilter.IsStringSet(out string filterString))
		{
			if (!string.Equals(m_SourceFilters, filterString))
			{
				m_Filters = filterString.Split(SPLIT, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < m_Filters.Length; i++)
				{
					m_Filters[i] = m_Filters[i].Trim();
				}
			}
			bool found = false;
			for (int i = 0; i < m_Filters.Length; i++)
			{
				if (logString.Contains(m_Filters[i]))
				{
					found = true;
					break;
				}
			}
			if (!found)
			{
				return;
			}
		}
		if (string.Equals(logString, m_LastLog))
		{
			m_Duplicate++;
			return;
		}
		else if (m_Duplicate > 0)
		{
			string newLogString = "[DUPLICATE] x" + m_Duplicate.ToString() + " " + m_LastLog;
			AddData(newLogString);
			m_Duplicate = 0;
		}
		switch (type)
		{
			case LogType.Log:
				break;
			case LogType.Assert:
				logString = Core.Str.Build("[ERROR] ", logString);
				break;
			case LogType.Error:
				logString = Core.Str.Build("[ERROR] ", logString);
				break;
			case LogType.Exception:
				logString = Core.Str.Build("[EXCEPTION] ", logString);
				break;
			case LogType.Warning:
				m_LastLog = logString;
				break;
		}
		AddData(logString, stackTrace);
	}

	private static void AddData(string log, string stackTrace = "")
	{
		if (m_Pause)
		{
			return;
		}
		if (m_Logs.Count > 100)
		{
			m_Logs.Dequeue();
		}
		m_Logs.Enqueue(log + stackTrace);
	}

	public override void DrawGUI()
	{
		base.DrawGUI();
		string pauseButtonString = m_Pause ? "Resume" : "Pause";
		GUILayout.BeginHorizontal();
		if (GUILayout.Button(pauseButtonString))
		{
			m_Pause = !m_Pause;
		}
		if (GUILayout.Button("Clear"))
		{
			m_Logs.Clear();
		}
		GUILayout.EndHorizontal();
		m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);

		foreach (string member in m_Logs)
		{
			GUILayout.Label(member);
		}
		GUILayout.EndScrollView();
	}
}
