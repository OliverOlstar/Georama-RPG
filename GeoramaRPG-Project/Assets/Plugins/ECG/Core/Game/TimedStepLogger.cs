using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Core
{
	public class TimedStepLoggerBase
	{
		private float m_LoadStartTime = 0.0f;
		private float m_LastLogTime = 0.0f;
		private string m_ProcessName = null;
		private List<(string, float)> m_StepDeltaTimes = new List<(string, float)>();

		protected void OnStartInternal(string primaryTerm, string secondaryTerm = null, Color? color = null, string step = "Start")
		{
			m_ProcessName = secondaryTerm == null ? $"[{primaryTerm}] " :
				$"[{primaryTerm}][{secondaryTerm}] ";
			if (color.HasValue)
			{
				m_ProcessName = Core.DebugUtil.ColorString(color.Value, m_ProcessName);
			}
			m_StepDeltaTimes.Clear();
			m_LoadStartTime = m_LastLogTime = Time.realtimeSinceStartup;
			LogInternal(step);
		}

		protected void OnCompleteInternal(string message = null)
		{
			if (message == null)
			{
				LogInternal("Completed");
			}
			else
			{
				LogInternal("Completed", message);
			}
			float total = 0.0f;
			for (int i = 0; i < m_StepDeltaTimes.Count; i++)
			{
				(string, float) step = m_StepDeltaTimes[i];
				Core.Str.AddNewLine(step.Item1, "\t", step.Item2.ToString("0.0"), "s");
				total += step.Item2;
			}
			Core.Str.AddLine();
			Debug.Log($"{m_ProcessName} Timed Results\nTotal Duration: {total.ToString("0.0")}s\n{Core.Str.Finish()}");
		}

		protected void LogInternal(string step, string message)
		{
			float time = Time.realtimeSinceStartup;
			float deltaTime = time - m_LastLogTime;
			float totalTime = time - m_LoadStartTime;
			m_StepDeltaTimes.Add((step, deltaTime));
			Debug.Log($"{m_ProcessName}{step}\n{message}\ntime: {totalTime.ToString("0.0")}s delta: {deltaTime.ToString("0.0")}s");
			m_LastLogTime = time;
		}

		protected void LogInternal(string step)
		{
			float time = Time.realtimeSinceStartup;
			float deltaTime = time - m_LastLogTime;
			float totalTime = time - m_LoadStartTime;
			m_StepDeltaTimes.Add((step, deltaTime));
			Debug.Log($"{m_ProcessName}{step}\ntime: {totalTime.ToString("0.0")}s delta: {deltaTime.ToString("0.0")}s");
			m_LastLogTime = time;
		}

		protected void LogErrorInternal(string step)
		{
			float time = Time.realtimeSinceStartup;
			float deltaTime = time - m_LastLogTime;
			float totalTime = time - m_LoadStartTime;
			Debug.LogError($"{m_ProcessName}{step}\ntime: {totalTime.ToString("0.0")}s delta: {deltaTime.ToString("0.0")}s");
			m_LastLogTime = time;
		}

		protected void LogErrorInternal(string step, string message)
		{
			float time = Time.realtimeSinceStartup;
			float deltaTime = time - m_LastLogTime;
			float totalTime = time - m_LoadStartTime;
			Debug.LogError($"{m_ProcessName}{step}\n{message}\ntime: {totalTime.ToString("0.0")}s delta: {deltaTime.ToString("0.0")}s");
			m_LastLogTime = time;
		}
	}

	public class TimedStepLogger : TimedStepLoggerBase
	{
		// I know this looks silly but I'm not crazy https://stackoverflow.com/questions/8230191/c-sharp-conditional-attribute
		// Conditional attributes do not support !
#if DISABLE_ELOGS
	[System.Diagnostics.Conditional("FALSE")]
#endif
		public void OnStart(string primaryTerm, string secondaryTerm = null, Color? color = null, string step = "Start") =>
			OnStartInternal(primaryTerm, secondaryTerm, color, step);

#if DISABLE_ELOGS
	[System.Diagnostics.Conditional("FALSE")]
#endif
		public void OnComplete(string message = null) => OnCompleteInternal(message);

#if DISABLE_ELOGS
	[System.Diagnostics.Conditional("FALSE")]
#endif
		public void Log(string step) => LogInternal(step);

#if DISABLE_ELOGS
	[System.Diagnostics.Conditional("FALSE")]
#endif
		public void Log(string step, string message) => LogInternal(step, message);

#if DISABLE_ELOGS
	[System.Diagnostics.Conditional("FALSE")]
#endif
		public void LogError(string step) => LogErrorInternal(step);

#if DISABLE_ELOGS
	[System.Diagnostics.Conditional("FALSE")]
#endif
		public void LogError(string step, string message) => LogErrorInternal(step, message);
	}

	public class TimedStepLoggerEditor : TimedStepLoggerBase
	{
		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		public void OnStart(string primaryTerm, string secondaryTerm = null, Color? color = null, string step = "Start") =>
			OnStartInternal(primaryTerm, secondaryTerm, color, step);

		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		public void OnComplete(string message = null) => OnCompleteInternal(message);

		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		public void Log(string step) => LogInternal(step);

		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		public void Log(string step, string message) => LogInternal(step, message);

		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		public void LogError(string step) => LogErrorInternal(step);

		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		public void LogError(string step, string message) => LogErrorInternal(step, message);
	}
}
