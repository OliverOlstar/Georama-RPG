using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher.Weapon
{
	[CreateAssetMenu(menuName = "ScriptableObject/Weapon/Collision/Destroy")]
	public class SOProjectileCollisionDestroy : SOProjectileCollisionBase
	{
		public override bool DoCollision(Projectile projectile, Collider other, ref bool canDamage, ref bool activeSelf)
		{
			projectile.rigidbody.isKinematic = true;
			canDamage = false;

			if (particlePrefab != null)
			{
				ObjectPoolDictionary.Play(particlePrefab, projectile.transform.position, projectile.transform.rotation);
			}
			audio.Play(projectile.audioSources);

			if (other != null)
			{
				Rigidbody rb = other.GetComponent<Rigidbody>();
				if (rb != null)
				{
					rb.AddForce(projectile.transform.forward * knockbackForce, ForceMode.Impulse);
				}
			}
			return true;
		}
	}
}
