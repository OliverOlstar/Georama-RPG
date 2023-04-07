using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace OliverLoescher.Weapon
{
	[RequireComponent(typeof(Weapon))]
	public class WeaponAmmo : MonoBehaviour
	{
		private Weapon weapon = null;

		private int totalAmmo;
		private int clipAmmo;
		private Coroutine chargeRoutine = null;

		[FoldoutGroup("Unity Events")] public UnityEvent OnReload;
		[FoldoutGroup("Unity Events")] public UnityEvent OnStartOverHeat;
		[FoldoutGroup("Unity Events")] public UnityEvent OnEndOverHeat;
		[FoldoutGroup("Unity Events")] public UnityEvent OnOutOfAmmo;

		private void Start() 
		{
			weapon = GetComponent<Weapon>();
			weapon.OnShoot.AddListener(OnShoot);

			clipAmmo = weapon.Data.clipAmmo;
			totalAmmo = weapon.Data.totalAmmo - clipAmmo;
		}

		public void OnShoot()
		{
			if (weapon.Data.ammoType == SOWeapon.AmmoType.Null)
				return;

			// Ammo
			clipAmmo = Mathf.Max(0, clipAmmo - 1);
			if (clipAmmo == 0)
			{
				weapon.canShoot = false;

				// Audio
				weapon.Data.outOfAmmoSound.Play(weapon.sourcePool);

				OnStartOverHeat.Invoke();
			}

			if (weapon.Data.ammoType == SOWeapon.AmmoType.Limited && totalAmmo <= 0)
			{
				// If totally out of ammo
				if (clipAmmo <= 0)
				{
					// If out of all ammo
					OnOutOfAmmo.Invoke();
				}
			}
			else
			{
				// Recharge
				if (chargeRoutine != null)
					StopCoroutine(chargeRoutine);
				chargeRoutine = StartCoroutine(AmmoRoutine());
			}
		}

		private IEnumerator AmmoRoutine()
		{
			yield return new WaitForSeconds(Mathf.Max(0, weapon.Data.reloadDelaySeconds - weapon.Data.reloadIntervalSeconds));

			while (clipAmmo < weapon.Data.clipAmmo && (totalAmmo > 0 || weapon.Data.ammoType == SOWeapon.AmmoType.Unlimited))
			{
				yield return new WaitForSeconds(weapon.Data.reloadIntervalSeconds);

				// Clip Ammo
				clipAmmo++;

				// Total Ammo
				if (weapon.Data.ammoType == SOWeapon.AmmoType.Limited)
				{
					totalAmmo--;
				}

				// Audio
				weapon.Data.reloadSound.Play(weapon.sourcePool);

				OnReload.Invoke();
			}

			if (weapon.canShoot == false)
			{
				weapon.canShoot = true;

				// Audio
				weapon.Data.onReloadedSound.Play(weapon.sourcePool);

				// Events
				OnEndOverHeat.Invoke();
			}
		}

		public void ModifyAmmo(int pValue)
		{
			// Modify Ammo
			totalAmmo += pValue;

			// Check for recharge
			if (totalAmmo > 0 && clipAmmo < weapon.Data.clipAmmo)
			{
				if (chargeRoutine != null)
					StopCoroutine(chargeRoutine);
				chargeRoutine = StartCoroutine(AmmoRoutine());
			}
		}
	}
}