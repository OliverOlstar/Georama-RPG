using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace OliverLoescher.Weapon
{
	public abstract class SOWeaponShootTypeBase : ScriptableObject
	{
		protected Action shoot = null;
		[HideInInspector]
		public float nextCanShootTime { get; protected set; } = 0.0f;

		public abstract void ShootStart();
		public abstract void ShootEnd();
		public abstract void OnUpdate(float pDeltaTime);
		public abstract void OnShoot();

		public virtual SOWeaponShootTypeBase Init(Action pShoot)
		{
			shoot = pShoot;
			return this;
		}
	}
}