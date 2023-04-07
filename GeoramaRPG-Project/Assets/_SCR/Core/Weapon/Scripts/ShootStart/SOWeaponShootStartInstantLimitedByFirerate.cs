using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher.Weapon
{
	[CreateAssetMenu(menuName = "ScriptableObject/Weapon/Start/InstantLimitedByFirerate")]
	public class SOWeaponShootStartInstantLimitedByFirerate : SOWeaponShootStartBase
	{
		public override void ShootStart()
		{
			if (shootType.nextCanShootTime <= Time.time)
			{
				shootType.ShootStart();
			}
		}
	}
}