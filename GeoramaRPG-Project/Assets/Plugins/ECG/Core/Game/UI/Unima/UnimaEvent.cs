
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnimaEvent : IUnimaPlayer, System.IComparable
{
	[SerializeField]
	private bool m_NoInvokeOnStop;
	[SerializeField]
	private float m_StartTime = 0.0f;
	public float StartTime => m_StartTime;
	[SerializeField]
	private UnityEvent m_Event = new UnityEvent();
	public UnityEvent Event => m_Event;

	private UnityAction m_RegisteredAction;
	public UnityAction RegisteredAction => m_RegisteredAction;

	public UnimaEvent(float startTime, UnityAction action)
	{
		m_StartTime = startTime;
		m_Event.AddListener(action);
		m_RegisteredAction = action;
	}

	private float m_OffsetStartTime = 0.0f;
	private float m_Timer = 0.0f;
	private bool m_Invoked = false;

	public IUnimaPlayer InstatiatePlayer(GameObject gameObject)
	{
		return this;
	}

	bool IUnimaPlayer.IsPlaying() { return false; }

	public void UnregisterAction(UnityAction action) { m_Event.RemoveListener(action); }

	void IUnimaPlayer.Play(IUnimaContext context, float offsetStartTime)
	{
		m_OffsetStartTime = offsetStartTime;
		float startTime = m_StartTime + m_OffsetStartTime;
		if (Core.Util.Approximately(startTime, 0.0f))
		{
			// Update the flag first. The event could disable us, causing Stop() to be
			// called, which would invoke our event again -- Josh M. 
			m_Invoked = true;
			m_Event.Invoke();
		}
		else
		{
			m_Timer = 0.0f;
			m_Invoked = false;
		}
	}

	void IUnimaPlayer.UpdatePlaying(float deltaTime)
	{
		if (!m_Invoked && m_StartTime > 0.0f)
		{
			m_Timer += deltaTime;
			float startTime = m_StartTime + m_OffsetStartTime;
			if (m_Timer > startTime)
			{
				// Update the flag first. The event could disable us, causing Stop() to be
				// called, which would invoke our event again -- Josh M. 
				m_Invoked = true;
				m_Event.Invoke();
			}
		}
	}

	void IUnimaPlayer.Stop()
	{
		if (!m_Invoked && !m_NoInvokeOnStop)
		{
			m_Event.Invoke();
		}
	}

	public int CompareTo(object obj)
	{
		UnimaEvent timedItem = obj as UnimaEvent;
		if (timedItem == null)
		{
			return 0;
		}
		float thisTiming = m_StartTime < 0.0f ? Mathf.Infinity : m_StartTime;
		float otherTiming = timedItem.m_StartTime < 0.0f ? Mathf.Infinity : timedItem.m_StartTime;
		int comparison = thisTiming.CompareTo(otherTiming);
		return comparison;
	}
}
