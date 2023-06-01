using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetableBehaviour : MonoBehaviour, ITargetable
{
	[SerializeField]
	private TargetableManager.Team m_Team = TargetableManager.Team.NotPlayer;

	Transform ITargetable.Transform => transform;

	void Start()
	{
		Core.Director.GetOrCreate<TargetableManager>().AddTarget(this, m_Team);
	}

	void OnDestroy()
	{
		Core.Director.Get<TargetableManager>().RemoveTarget(this, m_Team);
	}
}
