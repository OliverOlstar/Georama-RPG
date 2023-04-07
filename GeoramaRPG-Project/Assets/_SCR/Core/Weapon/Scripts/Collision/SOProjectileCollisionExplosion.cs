using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher.Weapon
{
	[CreateAssetMenu(menuName = "ScriptableObject/Weapon/Collision/Explosion")]
	public class SOProjectileCollisionExplosion : SOProjectileCollisionBase
	{
		[Space, SerializeField, Min(0.1f)]
		private float explosionRadius = 5.0f;
		[SerializeField, Min(0)]
		private int explosionDamage = 5;
		[SerializeField, Min(0.0f)]
		protected float explosionForce = 2000.0f;
		[SerializeField, Min(0.0f)]
		private float explosiveUpwardsModifier = 1;

		public override bool DoCollision(Projectile pProjectile, Collider pOther, ref bool canDamage, ref bool activeSelf)
		{
			pProjectile.rigidbody.isKinematic = true;
			canDamage = false;
			base.DoCollision(pProjectile, pOther, ref canDamage, ref activeSelf);
			Explode(pProjectile.transform.position, pProjectile);
			return true;
		}

		public void Explode(Vector3 pPoint, Projectile pProjectile)
		{
			Collider[] hits = Physics.OverlapSphere(pPoint, explosionRadius);
			foreach (var hit in hits)
			{
				Rigidbody rb = hit.GetComponent<Rigidbody>();
				if (rb != null)
				{
					rb.AddExplosionForce(explosionForce, pPoint, explosionRadius, explosiveUpwardsModifier);
				}
				IDamageable damageable = hit.GetComponent<IDamageable>();
				List<IDamageable> hitDamagables = ListPool<IDamageable>.Request();
				if (damageable != null && !hitDamagables.Contains(damageable.GetParentDamageable()))
				{
					hitDamagables.Add(damageable.GetParentDamageable());
					damageable.Damage(explosionDamage, pProjectile.sender, pPoint, (hit.transform.position - pPoint).normalized);
				}
				ListPool<IDamageable>.Return(hitDamagables);
			}
		}

		public override void DrawGizmos(Projectile pProjectile)
		{
			Gizmos.DrawWireSphere(pProjectile.transform.position, explosionRadius);
		}
	}
}
