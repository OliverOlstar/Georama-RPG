using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace OliverLoescher.Weapon
{
	public class Projectile : PoolElement
	{
		[Required]
		public SOProjectile data = null;

		public new Rigidbody rigidbody = null;
		public Collider hitboxCollider = null;
		public Collider physicsCollider = null;
		public AudioSourcePool audioSources = null;
		public GameObject sender = null;
		private SOTeam team = null;

		protected bool canDamage = true;
		protected bool activeSelf = true;
		private int currentFrame = 0;
		private int lastHitFrame = 0;
		private Collider lastHitCollider = null;

		[Header("Floating Numbers")]
		[ColorPalette("UI"), SerializeField]
		private Color hitColor = new Color(1, 0, 0, 1);
		[ColorPalette("UI"), SerializeField]
		private Color critColor = new Color(1, 1, 0, 1);

		private Vector3 startPos = new Vector3();
		private Vector3 previousPosition = new Vector3();

		[SerializeField]
		private float spawnOffsetZ = 0;

		public override void ReturnToPool()
		{
			activeSelf = false;
			CancelInvoke();
			base.ReturnToPool();
		}

		public override void OnExitPool()
		{
			currentFrame = 0;
			rigidbody.isKinematic = false;
			rigidbody.useGravity = false;
			canDamage = true;
			lastHitCollider = null;
			hitboxCollider.enabled = true;
			if (physicsCollider != null)
			{
				physicsCollider.enabled = false;
			}
			activeSelf = true;

			base.OnExitPool();
		}

		public void Init(Vector3 pPosition, Vector3 pDirection, GameObject pSender, SOTeam pTeam = null)
		{
			transform.position = pPosition;
			transform.rotation = Quaternion.LookRotation(pDirection);

			rigidbody.velocity = pDirection.normalized * Util.Range(data.shootForce);
			transform.position += transform.forward * spawnOffsetZ;

			startPos = transform.position;
			previousPosition = transform.position;

			sender = pSender;
			team = pTeam;
			Invoke(nameof(DoLifeEnd), Util.Range(data.lifeTime));
		}

		private void FixedUpdate() 
		{
			if (!activeSelf)
			{
				return;
			}

			bool updateRot = false;
			if (data.bulletGravity > 0)
			{
				rigidbody.AddForce(Vector3.down * data.bulletGravity * Time.fixedDeltaTime, ForceMode.VelocityChange);
				updateRot = true;
			}

			if (updateRot)
			{
				transform.rotation = Quaternion.LookRotation(rigidbody.velocity);
			}
		}

		protected virtual void Update() 
		{
			if (!activeSelf || !canDamage)
			{
				return;
			}

			currentFrame++; // Used to ignore collision on first two frames
			if (currentFrame >= -1 && data.useRaycast && // Raycast Projectile
				Physics.Linecast(previousPosition, transform.position, out RaycastHit hit, data.layerMask, QueryTriggerInteraction.Ignore))
			{
				DoHitOther(hit.collider, hit.point);
			}
			previousPosition = transform.position;
		}

		protected virtual void OnTriggerEnter(Collider other) 
		{
			if (!activeSelf)
			{
				return;
			}
			DoHitOther(other, transform.position);
		}

	#region Hit/Damage
		private void DoHitOther(Collider pOther, Vector3 pPoint)
		{
			if (canDamage == false || currentFrame < 1 || pOther.isTrigger || pOther == lastHitCollider || IsSender(pOther.transform))
			{
				return;
			}

			bool didDamage = false;
			IDamageable damageable = pOther.GetComponent<IDamageable>();
			if (damageable != null)
			{
				bool isSameTeam = SOTeam.Compare(damageable.GetTeam(), team);
				if (isSameTeam && team.ignoreTeamCollisions)
				{
					return;
				}
				if (!isSameTeam || team.teamDamage)
				{
					Debug.Log($"[{nameof(Projectile)}] {nameof(DamageOther)}({pOther.name}, {damageable.GetGameObject().name}, {(damageable.GetTeam() == null ? "No Team" : damageable.GetTeam().name)})", pOther);
					DamageOther(damageable, pPoint);
					didDamage = true;
				}
			}

			lastHitFrame = currentFrame;
			lastHitCollider = pOther;
			DoHitOtherInternal(pOther, didDamage);
		}

		protected virtual bool DoHitOtherInternal(Collider pOther, bool pDidDamage)
		{
			if ((pDidDamage ? data.projectileDamagableCollision : data.projectileEnviromentCollision).DoCollision(this, pOther, ref canDamage, ref activeSelf))
			{
				ReturnToPool();
				return true;
			}
			return false;
		}

		private void DamageOther(IDamageable damageable, Vector3 point)
		{
			// Rigidbody otherRb = other.GetComponentInParent<Rigidbody>();
			// if (otherRb != null)
			//	 otherRb.AddForceAtPosition(rigidbody.velocity.normalized * data.hitForce, point);
			
			if (Random.value > data.critChance01)
			{
				damageable.Damage(data.damage, sender, transform.position, rigidbody.velocity);
			}
			else
			{
				damageable.Damage(Mathf.RoundToInt(data.critDamageMultiplier * data.damage), sender, transform.position, rigidbody.velocity, critColor);
			}
		}

		protected virtual void DoLifeEnd()
		{
			data.projectileLifeEnd.DoCollision(this, null, ref canDamage, ref activeSelf);
			ReturnToPool();
		}

		private bool IsSender(Transform other)
		{
			if (sender == null)
			{
				return false;
			}
			if (other == sender.transform)
			{
				return true;
			}
			if (other.parent == null)
			{
				return false;
			}
			return IsSender(other.parent);
		}
		#endregion

		private void PlayParticle(ParticleSystem pParticle, Vector3 pPosition)
		{
			if (pParticle != null)
			{
				pParticle.gameObject.SetActive(true);
				pParticle.Play();
				pParticle.transform.position = pPosition;
				pParticle.transform.SetParent(null);
			}
		}

		private void OnDrawGizmosSelected() 
		{
			Gizmos.color = Color.green;
			Vector3 endPos = transform.position + (transform.forward * -spawnOffsetZ);
			Gizmos.DrawLine(transform.position, endPos);
			Gizmos.DrawWireSphere(endPos, 0.01f);

			if (data == null)
			{
				return;
			}
			if (data.projectileEnviromentCollision != null)
			{
				data.projectileEnviromentCollision.DrawGizmos(this);
			}
			if (data.projectileDamagableCollision != null)
			{
				data.projectileDamagableCollision.DrawGizmos(this);
			}
			if (data.projectileLifeEnd != null)
			{
				data.projectileLifeEnd.DrawGizmos(this);
			}
		}
	}
}