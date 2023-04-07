using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace OliverLoescher.Weapon
{
	[CreateAssetMenu(menuName = "ScriptableObject/Weapon/Weapon Data")]
	public class SOWeapon : ScriptableObject
	{
		public enum BulletType 
		{
			Projectile,
			RaycastProjectile,
			Raycast
		}

		public enum AmmoType
		{
			Limited,
			Unlimited,
			Null
		}

		[Title("Display")]
		public string displayName = "Untitled";
		[TextArea] public string description = "";

		[Title("Type")]
		public BulletType bulletType =  BulletType.Projectile;

		[Min(1)] public int projectilesPerShot = 1;

		[ShowIfGroup("Proj", Condition = "@bulletType != BulletType.Raycast")]
		[TitleGroup("Proj/Projectile")] public GameObject projectilePrefab = null;

		[ShowIfGroup("Ray", Condition = "@bulletType == BulletType.Raycast")]
		[TitleGroup("Ray/Raycast")] public float range = 5.0f;
		[TitleGroup("Ray/Raycast")] [AssetsOnly] public GameObject hitFXPrefab = null;
		[ShowIf("@bulletType != BulletType.Projectile")] public LayerMask layerMask = new LayerMask();

		[Title("Stats")]
		[Required, InlineEditor]
		public SOWeaponShootStartBase shootStart = null;

		[Header("Spread")]
		[Required, InlineEditor]
		public SOWeaponSpreadBase spread = null;

		[Header("Force")]
		public Vector3 recoilForce = Vector3.zero;

		[Title("Ammo")]
		public AmmoType ammoType = AmmoType.Null;
		[ShowIf("@ammoType == AmmoType.Limited")] [MinValue("@clipAmmo")] public int totalAmmo = 12;
		[ShowIf("@ammoType != AmmoType.Null")] [Min(1)] public int clipAmmo = 4;

		[Space]
		[ShowIf("@ammoType != AmmoType.Null")] [Min(0)] public float reloadDelaySeconds = 0.6f;
		[ShowIf("@ammoType != AmmoType.Null")] [Min(0.01f)] public float reloadIntervalSeconds = 0.2f;

		[Title("Audio")]
		public AudioUtil.AudioPiece shotSound = new AudioUtil.AudioPiece();
		public AudioUtil.AudioPiece failedShotSound = new AudioUtil.AudioPiece();

		[Space]
		[ShowIf("@ammoType != AmmoType.Null")] public AudioUtil.AudioPiece reloadSound = new AudioUtil.AudioPiece();
		[ShowIf("@ammoType != AmmoType.Null")] public AudioUtil.AudioPiece outOfAmmoSound = new AudioUtil.AudioPiece();
		[ShowIf("@ammoType != AmmoType.Null")] public AudioUtil.AudioPiece onReloadedSound = new AudioUtil.AudioPiece();
	}
}