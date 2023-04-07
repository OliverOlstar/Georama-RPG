using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher.Weapon
{
	[CreateAssetMenu(menuName = "ScriptableObject/Weapon/ShootType/Auto")]
	public class SOWeaponShootTypeAuto : SOWeaponShootTypeBase
	{
		[SerializeField, Min(0.0f)]
		private float secondsBetweenShots = 0.1f;

		private bool isShooting = false;

		public override void ShootStart()
		{
			isShooting = true;
			shoot.Invoke();
		}

		public override void ShootEnd()
		{
			isShooting = false;
		}

		public override void OnUpdate(float pDeltaTime)
		{
			if (isShooting && Time.time >= nextCanShootTime)
			{
				shoot.Invoke();
			}
		}

		public override void OnShoot()
		{
			nextCanShootTime = Time.time + secondsBetweenShots;
		}
	}
}