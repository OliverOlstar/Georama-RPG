using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher.Weapon
{
	[CreateAssetMenu(menuName = "ScriptableObject/Weapon/ShootType/Burst")]
	public class SOWeaponShootTypeBurst : SOWeaponShootTypeBase
	{
		[SerializeField, Min(2)]
		private int shootCount = 3;
		[SerializeField, Min(0.0f)]
		private float secondsBetweenShots = 0.1f;
		[SerializeField, Min(0.0f)]
		private float secondsBetweenBurstShots = 0.1f;

		private bool isShooting = false;
		private int activeCount = 0;

		public override void ShootStart()
		{
			if (isShooting)
				return;
			isShooting = true;

			activeCount = shootCount;
			shoot.Invoke();
		}

		public override void ShootEnd()
		{

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
			activeCount--;
			if (activeCount <= 0)
			{
				nextCanShootTime = Time.time + secondsBetweenShots;
				isShooting = false;
			}
			else
			{
				nextCanShootTime = Time.time + secondsBetweenBurstShots;
			}
		}
	}
}