using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourcePool : MonoBehaviour
{
	private AudioSource[] sources = new AudioSource[0];
	[SerializeField] [Min(1)] private int count = 1;
	int index = 0;

	void Start()
	{
		sources = new AudioSource[count];
		sources[0] = GetComponent<AudioSource>();
		for (int i = 1; i < count; i++)
		{
			sources[i] = DuplicateAudioSource(sources[0], gameObject);
		}
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

	private AudioSource DuplicateAudioSource(AudioSource pSource, GameObject pGameObject)
	{
		AudioSource s = pGameObject.AddComponent<AudioSource>();

		s.clip = pSource.clip;
		s.outputAudioMixerGroup = pSource.outputAudioMixerGroup;
		s.mute = pSource.mute;
		s.bypassEffects = pSource.bypassEffects;
		s.bypassReverbZones = pSource.bypassReverbZones;
		s.playOnAwake = pSource.playOnAwake;
		s.loop = pSource.loop;
		s.priority = pSource.priority;
		s.volume = pSource.volume;
		s.pitch = pSource.pitch;
		s.panStereo = pSource.panStereo;
		s.spatialBlend = pSource.spatialBlend;
		s.reverbZoneMix = pSource.reverbZoneMix;
		s.dopplerLevel = pSource.dopplerLevel;
		s.spread = pSource.spread;
		s.minDistance = pSource.minDistance;
		s.maxDistance = pSource.maxDistance;
		s.rolloffMode = pSource.rolloffMode;
		s.SetCustomCurve(AudioSourceCurveType.CustomRolloff, pSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff));

		return s;
	}
}
