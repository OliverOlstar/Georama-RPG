
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnimaTreeNode : System.IComparable
{
	public UnimaController m_Handler = null;
	public bool m_Guaranteed = false;
	public bool m_AfterStartDelay = false;
	[Delayed]
	public float m_Timing = -1.0f;

	[System.NonSerialized]
	public bool m_Played = false;

	public int CompareTo(object obj)
	{
		UnimaTreeNode otherNode = obj as UnimaTreeNode;
		if (otherNode == null)
		{
			return 0;
		}
		float thisTiming = m_Timing < 0.0f ? Mathf.Infinity : m_Timing;
		float otherTiming = otherNode.m_Timing < 0.0f ? Mathf.Infinity : otherNode.m_Timing;
		int comparison = thisTiming.CompareTo(otherTiming);
		return comparison;
	}
}
