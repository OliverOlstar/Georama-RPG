using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher.Weapon
{
	[CreateAssetMenu(menuName = "ScriptableObject/Weapon/Start/Charge")]
	public class SOWeaponShootStartCharge : SOWeaponShootStartBase
	{
		[SerializeField]
		private float chargeSeconds = 0;

		private Coroutine coroutine = null;

		public override void ShootStart()
		{
			MonoUtil.Stop(ref coroutine);
			coroutine = MonoUtil.Start(ShootDelayed());
		}
		public override void ShootEnd()
		{
			MonoUtil.Stop(ref coroutine);
		}

		public IEnumerator ShootDelayed()
		{
			yield return new WaitForSeconds(chargeSeconds);
			shootType.ShootStart();
		}
	}
}