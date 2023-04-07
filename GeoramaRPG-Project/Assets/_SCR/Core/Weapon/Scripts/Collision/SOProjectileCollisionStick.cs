using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher.Weapon
{
	[CreateAssetMenu(menuName = "ScriptableObject/Weapon/Collision/Stick")]
	public class SOProjectileCollisionStick : SOProjectileCollisionBase
	{
		public override bool DoCollision(Projectile pProjectile, Collider pOther, ref bool canDamage, ref bool activeSelf)
		{
			pProjectile.rigidbody.isKinematic = true;
			if (pOther.gameObject.isStatic == false)
				pProjectile.transform.SetParent(pOther.transform);
			canDamage = false;
			activeSelf = false;
			base.DoCollision(pProjectile, pOther, ref canDamage, ref activeSelf);

			return false;
		}
	}
}
