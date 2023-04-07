using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher.Weapon
{
	[CreateAssetMenu(menuName = "ScriptableObject/Weapon/Spread/None")]
	public class SOWeaponSpreadNone : SOWeaponSpreadBase
	{
		public override Vector3 ApplySpread(Vector3 pDirection) => pDirection;
		public override void OnShoot() { }
		public override void OnUpdate(in float pDeltaTime) { }
	}
}
