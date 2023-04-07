using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher.Weapon
{
	public abstract class SOWeaponSpreadExpandBase : SOWeaponSpreadBase
	{
		[Range(0.0f, 1.0f)] public float spreadIncrease = 0.4f;
		[Range(Util.NEARZERO, 3)] public float spreadDecrease = 0.6f;

		protected float spread01 = 0.0f;

		public override void OnShoot()
		{
			spread01 = Mathf.Min(1, spread01 + spreadIncrease);
		}

		public override void OnUpdate(in float pDeltaTime)
		{
			spread01 = Mathf.Max(0, spread01 - (Time.deltaTime * spreadDecrease));
		}
	}
}