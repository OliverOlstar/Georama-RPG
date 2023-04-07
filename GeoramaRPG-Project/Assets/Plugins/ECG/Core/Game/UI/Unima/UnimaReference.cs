
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnimaDurationType
{
	Arbitrary = 0,
	Infinite,
	Fixed
}

[System.Serializable]
public class UnimaTiming
{
	public float m_StartTime = 0.0f;
	public bool m_WaitToFinish = true;
}

public abstract class UnimaTimedItem
{
	[SerializeField]
	protected UnimaTiming m_Timing = new UnimaTiming();
	public UnimaTiming Timing => m_Timing;

	public abstract IUnimaPlayer InstatiatePlayer(GameObject gameObject);

	public int CompareTo(object obj)
	{
		UnimaTimedItem timedItem = obj as UnimaTimedItem;
		if (timedItem == null)
		{
			return 0;
		}
		float thisTiming = m_Timing.m_StartTime < 0.0f ? Mathf.Infinity : m_Timing.m_StartTime;
		float otherTiming = timedItem.m_Timing.m_StartTime < 0.0f ? Mathf.Infinity : timedItem.m_Timing.m_StartTime;
		int comparison = thisTiming.CompareTo(otherTiming);
		return comparison;
	}
}

[System.Serializable]
public class UnimaReference : UnimaTimedItem
{
	[SerializeField, UberPicker.AssetNonNull]
	private UnimateBase m_Animation = null;
	public UnimateBase Animation => m_Animation;

	public override IUnimaPlayer InstatiatePlayer(GameObject gameObject)
	{
		return m_Animation != null ? m_Animation.CreatePlayer(m_Timing, gameObject) : null;
	}
}

[System.Serializable]
public class UnimaSet : IEnumerable<UnimaReference>
{
	[SerializeField]
	private List<UnimaReference> m_Animations = new List<UnimaReference>();

	public int Count => m_Animations.Count;

	public UnimaReference this[int index] => m_Animations[index];

	public IEnumerator<UnimaReference> GetEnumerator()
	{
		return m_Animations.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return m_Animations.GetEnumerator();
	}

	public List<IUnimaPlayer> InstantiatePlayers(GameObject gameObject)
	{
		List<IUnimaPlayer> players = new List<IUnimaPlayer>(m_Animations.Count);
		foreach (UnimaReference anim in m_Animations)
		{
			IUnimaPlayer player = anim.InstatiatePlayer(gameObject);
			if (player != null)
			{
				players.Add(player);
			}
		}
		return players;
	}
}
