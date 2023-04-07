using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher.Weapon
{
	public abstract class SOProjectileCollisionBase : ScriptableObject
	{
		[SerializeField]
		protected PoolElement particlePrefab = null;
		[SerializeField]
		protected AudioUtil.AudioPiece audio = null;
		[SerializeField, Min(0.0f)]
		protected float knockbackForce = 300.0f;

		public virtual bool DoCollision(Projectile projectile, Collider other, ref bool canDamage, ref bool activeSelf)
		{
			if (other != null && other.TryGetComponent(out Rigidbody rb))
			{
				rb.AddForce(projectile.transform.forward * knockbackForce, ForceMode.Impulse);
			}
			ObjectPoolDictionary.Play(particlePrefab, projectile.transform.position, projectile.transform.rotation);
			audio.Play(projectile.audioSources);
			return true;
		}
		public virtual void DrawGizmos(Projectile pProjectile) { }
	}
}