using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace OliverLoescher.Weapon
{
	public abstract class SOWeaponShootStartBase : ScriptableObject
	{
		[Required, InlineEditor]
		public SOWeaponShootTypeBase shootType = null;

		public virtual void ShootStart() => shootType.ShootStart();
		public virtual void ShootEnd() => shootType.ShootEnd();
		public virtual void OnUpdate(float pDelta) => shootType.OnUpdate(pDelta);
		public virtual void OnShoot() => shootType.OnShoot();

		public virtual SOWeaponShootStartBase Init(Action pShoot)
		{
			shootType = Instantiate(shootType).Init(pShoot);
			return this;
		}
	}
}