using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEditor;

namespace OliverLoescher.Weapon
{
	public class Weapon : MonoBehaviour
	{
		public enum MultiMuzzleType
		{
			FirstOnly,
			Loop,
			PingPong,
			Random,
			RandomNotOneAfterItself,
			RandomAllOnce
		}

		[SerializeField, Required] 
		private SOWeapon data = null;
		public SOTeam team = null;
		[ShowIf("@muzzlePoints.Length > 1"), SerializeField] 
		protected MultiMuzzleType multiMuzzleType = MultiMuzzleType.RandomNotOneAfterItself;
		public bool canShoot = true;

		[Header("References")]
		[SerializeField] 
		protected Transform[] muzzlePoints = new Transform[1];
		[SerializeField] 
		private ParticleSystem muzzleFlash = null;
		[ShowIf("@muzzleFlash != null"), SerializeField] 
		private Vector3 muzzleFlashRelOffset = new Vector3();

		[Space]
		public GameObject sender = null;
		[SerializeField] private Rigidbody recoilBody = null;
		public AudioSourcePool sourcePool = null;

		[FoldoutGroup("Unity Events")] public UnityEvent OnShoot;
		[FoldoutGroup("Unity Events")] public UnityEvent OnFailedShoot;

		private SOWeaponShootStartBase shootStart = null;
		private SOWeaponSpreadBase spread = null;
		public bool isShooting { get; private set; } = false;

		private void Start() 
		{
			if (data == null)
			{
				return;
			}

			shootStart = Instantiate(data.shootStart).Init(Shoot);
			spread = Instantiate(data.spread).Init();

			Init();
		}

		private void Reset()
		{
			sender = gameObject;
		}

		protected virtual void Init() { }

		public SOWeapon Data => data;
		public void SetData(SOWeapon pData)
		{
			data = pData;
			Start();
		}

		public void ShootStart()
		{
			shootStart.ShootStart();
		}

		public void ShootEnd()
		{
			shootStart.ShootEnd();
		}

		private void Update()
		{
			if (shootStart == null || spread == null)
			{
				return;
			}
			shootStart.OnUpdate(Time.deltaTime);
			spread.OnUpdate(Time.deltaTime);
		}

		public void Shoot()
		{
			if (canShoot)
			{
				shootStart.OnShoot();

				// Bullet
				Transform muzzle = GetMuzzle();
				SpawnShot(muzzle);

				// Recoil
				if (recoilBody != null && data.recoilForce != Vector3.zero)
				{
					recoilBody.AddForceAtPosition(muzzle.TransformDirection(data.recoilForce), muzzle.position, ForceMode.VelocityChange);
				}

				// Spread
				spread.OnShoot();

				// Particles
				if (muzzleFlash != null)
				{
					if (muzzleFlash.transform.parent != muzzle)
					{
						muzzleFlash.transform.SetParent(muzzle);
						muzzleFlash.transform.localPosition = muzzleFlashRelOffset;
						muzzleFlash.transform.localRotation = Quaternion.identity;
					}
					muzzleFlash.Play();
				}

				// Audio
				data.shotSound.Play(sourcePool);

				// Event
				OnShoot?.Invoke();
			}
			else
			{
				OnShootFailed();
			}
		}

		protected virtual void SpawnShot(Transform pMuzzle)
		{
			for (int i = 0; i < data.projectilesPerShot; i++)
			{
				if (data.bulletType == SOWeapon.BulletType.Raycast)
				{
					SpawnRaycast(pMuzzle.position, pMuzzle.forward);
				}
				else
				{
					Vector3 dir = spread.ApplySpread(pMuzzle.forward);
					SpawnProjectile(pMuzzle.position, dir);
				}
			}
		}

		protected virtual void OnShootFailed()
		{
			// Audio
			data.failedShotSound.Play(sourcePool);

			// Event
			OnFailedShoot?.Invoke();
		}

		protected virtual Projectile SpawnProjectile(Vector3 pPoint, Vector3 pDirection)
		{
			// Spawn projectile
			GameObject projectile;
			projectile = ObjectPoolDictionary.Get(data.projectilePrefab);
			projectile.SetActive(true);

			Projectile projectileScript = projectile.GetComponentInChildren<Projectile>();
			projectileScript.Init(pPoint, pDirection, sender);

			// Audio
			data.shotSound.Play(sourcePool); // TODO Move this incase bulletsPerShot > 1
			OnShoot?.Invoke();

			return projectileScript;
		}

		protected virtual void SpawnRaycast(Vector3 pPoint, Vector3 pForward)
		{
			Vector3 dir = spread.ApplySpread(pForward);
			if (Physics.Raycast(pPoint, dir, out RaycastHit hit, data.range, data.layerMask, QueryTriggerInteraction.Ignore)) 
			{
				ApplyParticleFX(hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal), hit.collider);

				// push object if rigidbody
				Rigidbody hitRb = hit.collider.attachedRigidbody;
				// if (hitRb != null)
				//	 hitRb.AddForceAtPosition(data.hitForce * dir, hit.point);

				// Damage my script if possible
				IDamageable a = hit.collider.GetComponent<IDamageable>();
				// if (a != null)
				//	 a.Damage(data.damage, sender, hit.point, hit.normal);
			}
		}

		public virtual void ApplyParticleFX(Vector3 position, Quaternion rotation, Collider attachTo) 
		{
			if (data.hitFXPrefab) 
			{
				GameObject impact = Instantiate(data.hitFXPrefab, position, rotation) as GameObject;
			}
		}

		private int lastMuzzleIndex = 0;
		private bool muzzlePingPongDirection = true;
		private List<int> muzzleIndexList = new List<int>();
		protected Transform GetMuzzle()
		{
			switch (multiMuzzleType)
			{
				case MultiMuzzleType.Loop: // Loop ////////////////////////////////////////
					lastMuzzleIndex++;
					if (lastMuzzleIndex == muzzlePoints.Length)
						lastMuzzleIndex = 0;
					return muzzlePoints[lastMuzzleIndex];
					
				case MultiMuzzleType.PingPong: // PingPong ////////////////////////////////
					if (muzzlePingPongDirection)
					{
						lastMuzzleIndex++; // Forward
						if (lastMuzzleIndex == muzzlePoints.Length - 1)
							muzzlePingPongDirection = false;
					}
					else
					{
						lastMuzzleIndex--; // Back
						if (lastMuzzleIndex == 0)
							muzzlePingPongDirection = true;
					}
					return muzzlePoints[lastMuzzleIndex];

				case MultiMuzzleType.Random: // Random ////////////////////////////////////
					return muzzlePoints[Random.Range(0, muzzlePoints.Length)];

				case MultiMuzzleType.RandomNotOneAfterItself: // RandomNotOneAfterItself //
					int i = Random.Range(0, muzzlePoints.Length);
					if (i == lastMuzzleIndex)
					{
						// If is previous offset to new index
						i += Random.Range(1, muzzlePoints.Length);
						// If past max, loop back around
						if (i >= muzzlePoints.Length)
							i -= muzzlePoints.Length;
					}
					lastMuzzleIndex = i;
					return muzzlePoints[i];

				case MultiMuzzleType.RandomAllOnce: // RandomAllOnce //////////////////////
					if (muzzleIndexList.Count == 0)
					{
						// If out of indexes, refill
						for (int z = 0; z < muzzlePoints.Length; z++)
							muzzleIndexList.Add(z);
					}
					
					// Get random index from list of unused indexes
					int a = Random.Range(0, muzzleIndexList.Count);
					int b = muzzleIndexList[a];
					muzzleIndexList.RemoveAt(a);
					return muzzlePoints[b];

				default: // First Only ////////////////////////////////////////////////////
					return muzzlePoints[0];
			}
		}

		private void OnDrawGizmos() 
		{
	#if UNITY_EDITOR
			if (data == null)
				return;

			foreach (Transform m in muzzlePoints)
			{
				if (m == null)
					continue;
				(spread == null ? data.spread : spread).DrawGizmos(transform, m);
			}
	#endif
		}

		#region Helpers
		protected void Log(string pMessage) => Debug.Log($"[{GetType().Name}] {pMessage}", this);
		protected void LogError(string pMessage) => Debug.LogError($"[{GetType().Name}] {pMessage}", this);
		#endregion
	}
}