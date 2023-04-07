using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher.Weapon
{
	public abstract class SOWeaponSpreadBase : ScriptableObject
	{
		public abstract Vector3 ApplySpread(Vector3 pDirection);
		public abstract void OnShoot();
		public abstract void OnUpdate(in float pDeltaTime);

		public virtual void DrawGizmos(in Transform pTransform, in Transform pMuzzle) { }

		public virtual SOWeaponSpreadBase Init() => this;
	}
}
