using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher.Weapon
{
	[CreateAssetMenu(menuName = "ScriptableObject/Weapon/Collision/Penetrate")]
	public class SOProjectileCollisionPenetrate : SOProjectileCollisionBase
	{
		public override bool DoCollision(Projectile pProjectile, Collider pOther, ref bool canDamage, ref bool activeSelf)
		{
			if (pOther.gameObject.isStatic)
			{
				pProjectile.rigidbody.isKinematic = true;
				canDamage = false;
				base.DoCollision(pProjectile, pOther, ref canDamage, ref activeSelf);
				return true;
			}
			return false;
		}
	}
}
