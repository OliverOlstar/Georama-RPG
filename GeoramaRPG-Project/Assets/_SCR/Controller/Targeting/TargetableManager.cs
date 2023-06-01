using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverLoescher;

public interface ITargetable
{
	Transform Transform { get; }
}

[GameStateDirectorCreate(GameState.InGame), GameStateDirectorPersist(GameState.InGame)]
public class TargetableManager : IDirector
{
	public enum Team // TODO Move to data
	{
		NotPlayer = 0,
		Player,
	}

	public struct Context
	{
		public Team Team;
		public Vector3 Point;
		public float Radius;
	}

	public Dictionary<Team, List<ITargetable>> m_Targets = new Dictionary<Team, List<ITargetable>>();

	void IDirector.OnCreate() { }
	void IDirector.OnDestroy() { }

	public void AddTarget(ITargetable pTarget, Team pTeam)
	{
		if (!m_Targets.TryGetValue(pTeam, out List<ITargetable> targets))
		{
			targets = new List<ITargetable>();
			m_Targets.Add(pTeam, targets);
		}
		targets.Add(pTarget);
	}

	public bool RemoveTarget(ITargetable pTarget, Team pTeam)
	{
		if (!m_Targets.TryGetValue(pTeam, out List<ITargetable> targets))
		{
			return false;
		}
		return targets.Remove(pTarget);
	}

	public void Query<T>(ref List<T> pResult, Context pContext) where T : ITargetable
	{
		pResult.Clear();
		if (!m_Targets.TryGetValue(pContext.Team, out List<ITargetable> targets))
		{
			return;
		}
		foreach (T target in targets)
		{
			if (Util.DistanceEqualLessThan(target.Transform.position, pContext.Point, pContext.Radius))
			{
				pResult.Add(target);
			}
		}
	}
}
