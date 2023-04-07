using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher.Weapon
{
	public class ProjectileSFX : MonoBehaviour
	{
		[SerializeField] private AudioClip[] hitClips = new AudioClip[0];
		[SerializeField] private Vector2 hitPitch = new Vector2(0.9f, 1.2f);
		[SerializeField] private AudioClip[] explodeClips = new AudioClip[0];
		[SerializeField] private Vector2 explodePitch = new Vector2(0.9f, 1.2f);
		private AudioSource[] sources = new AudioSource[0];
		int index = 0;

		void Start()
		{
			sources = GetComponents<AudioSource>();
		}

		public AudioSource GetSource()
		{
			index++;
			if (index == sources.Length)
			{
				index = 0;
			}
			return sources[index];
		}

		public void OnCollision()
		{
			AudioUtil.PlayOneShotRandomClip(GetSource(), hitClips, hitPitch);
		}

		public void OnExplode()
		{
			AudioUtil.PlayOneShotRandomClip(GetSource(), explodeClips, explodePitch);
		}
	}
}