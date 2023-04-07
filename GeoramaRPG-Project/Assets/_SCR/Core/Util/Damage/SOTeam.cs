using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace OliverLoescher
{
	[CreateAssetMenu(menuName = "ScriptableObject/Weapon/Team Data")]
	public class SOTeam : ScriptableObject
	{
		public bool ignoreTeamCollisions = true;
		[ShowIf(@"ignoreTeamCollisions")] public bool teamDamage = false;

		public static bool Compare(SOTeam teamA, SOTeam teamB)
		{
			if (teamA == null || teamB == null)
			{
				return false;
			}
			return teamA == teamB;
		}
	}
}