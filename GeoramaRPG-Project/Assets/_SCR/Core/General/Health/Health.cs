using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

namespace OliverLoescher
{
	public class Health : CharacterValue, IDamageable
	{
		public delegate void DamageEvent(float pValue, GameObject pSender, Vector3 pPoint, Vector3 pDirection);
		public DamageEvent onDamageEvent;

		[SerializeField] private SOTeam team = null;

		[Header("Death")]
		[SerializeField] private bool disableCollidersOnDeath = true;
		private Collider[] colliders = new Collider[0];

		[Space]
		[ColorPalette("UI")] public Color deathColor = Color.grey;
		[ColorPalette("UI")] public Color damageColor = Color.red;
		[ColorPalette("UI")] public Color healColor = Color.green;

		protected override void Start() 
		{
			base.Start();

			if (disableCollidersOnDeath)
			{
				colliders = GetComponentsInChildren<Collider>();
			}

			onValueOut.AddListener(Death);

			Init();
		}

		protected virtual void Init() { }

		public virtual void Damage(float pValue, GameObject pSender, Vector3 pPoint, Vector3 pDirection, Color pColor) => Damage(pValue, pSender, pPoint, pDirection); // IDamageable
		[Button()] 
		public virtual void Damage(float pValue, GameObject pSender, Vector3 pPoint, Vector3 pDirection) // IDamageable
		{
			Modify(-pValue);
			onDamageEvent?.Invoke(pValue, pSender, pPoint, pDirection);
		}

		public virtual void Death() 
		{
			if (disableCollidersOnDeath)
			{
				foreach (Collider c in colliders)
				{
					c.enabled = false;
				}
			}
		}

		GameObject IDamageable.GetGameObject() => gameObject;
		IDamageable IDamageable.GetParentDamageable() => this;
		SOTeam IDamageable.GetTeam() => team;

		public void Respawn()
		{
			value = maxValue;
			foreach (BarValue bar in UIBars)
				bar.InitValue(1);
			OnValueIn();
		}
	}
}