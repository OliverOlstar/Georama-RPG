using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace OliverLoescher
{
	[RequireComponent(typeof(ParticleSystem))]
	public class ParticlePoolElement : PoolElement
	{
		[InfoBox("Ensure particle stopping action is Callback")]
		private ParticleSystem particle = null;

		public override void Init(string pPoolKey, Transform pParent)
		{
			base.Init(pPoolKey, pParent);
			particle = GetComponent<ParticleSystem>();
			SetStoppingAction(particle);
		}

		private void Reset()
		{
			SetStoppingAction(GetComponent<ParticleSystem>());
		}

		private void SetStoppingAction(in ParticleSystem pParticle)
		{
			ParticleSystem.MainModule main = pParticle.main;
			main.stopAction = ParticleSystemStopAction.Callback;
		}

		public void OnParticleSystemStopped()
		{
			ReturnToPool();
		}

		public override void ReturnToPool()
		{
			base.ReturnToPool();
			particle.Stop();
		}

		public override void OnExitPool()
		{
			base.OnExitPool();
		}
	}
}