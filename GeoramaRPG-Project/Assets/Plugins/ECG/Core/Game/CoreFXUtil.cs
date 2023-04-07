
using UnityEngine;

namespace Core
{
	public static class FXUtil
	{
		public static void Scale(GameObject obj, float scale)
		{
			obj.transform.localScale = scale * obj.transform.localScale;
			ParticleSystem[] fxs = obj.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem fx in fxs)
			{
				// This seems to be the only way to reset the particle system so the sizing change takes effect
				fx.Simulate(0.0f);
				ParticleSystem.MainModule main = fx.main;
				main.startSizeMultiplier *= scale;
				fx.Play();
			}
		}

		public static void SetColor(GameObject obj, Color color)
		{
			ParticleSystem[] fxs = obj.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem fx in fxs)
			{
				ParticleSystem.MainModule main = fx.main;
				main.startColor = color;
			}
			Util.ColorRenderer(obj, color);
		}

		public static float GetParticleSystemMinMaxCurveDuration(ParticleSystem particleSystem, ParticleSystem.MinMaxCurve minMaxCurve)
		{
			float duration = 0.0f;
			switch (minMaxCurve.mode)
			{
				case ParticleSystemCurveMode.Constant:
					duration = minMaxCurve.constant;
					break;
				case ParticleSystemCurveMode.TwoConstants:
					duration = minMaxCurve.constantMax;
					break;
				case ParticleSystemCurveMode.Curve:
					for (int i = 0; i < minMaxCurve.curve.keys.Length; i++)
					{
						duration = Mathf.Max(duration, minMaxCurve.curve.keys[i].value);
					}
					break;
				case ParticleSystemCurveMode.TwoCurves:
					for (int i = 0; i < minMaxCurve.curveMax.keys.Length; i++)
					{
						duration = Mathf.Max(duration, minMaxCurve.curveMax.keys[i].value);
					}
					break;
				default:
					Debug.LogError(Core.Str.Build(
						"GameUtil.ParticleSystemMinMaxCurveDuration: MinMaxCurve on particle system named \"", particleSystem.gameObject.name,
						"\" has the unrecognized mode ", minMaxCurve.mode.ToString(), "."));
					break;
			}
			return duration;
		}


		public static float CalculateParticalSystemLifetime(ParticleSystem particleSystem)
		{
			return CalculateParticalSystemLifetime(particleSystem, -1.0f);
		}

		public static float CalculateParticalSystemLifetime(ParticleSystem particleSystem, float ignoreBeyondThisLimit)
		{
			float duration = 0.0f;
			if (Util.Approximately(GetParticleSystemMinMaxCurveDuration(particleSystem, particleSystem.emission.rateOverTime.constant), 0.0f))
			{
				// Bursts only.
				for (int j = 0; j < particleSystem.emission.burstCount; j++)
				{
					ParticleSystem.Burst burst = particleSystem.emission.GetBurst(j);
					float particleDuration = burst.time + GetParticleSystemMinMaxCurveDuration(particleSystem, particleSystem.main.startLifetime);
					if (ignoreBeyondThisLimit < 0.0f || particleDuration < ignoreBeyondThisLimit)
					{
						duration = Mathf.Max(duration, particleDuration);
					}
				}
			}
			else
			{
				float particleDuration =
					GetParticleSystemMinMaxCurveDuration(particleSystem, particleSystem.main.startDelay)
					+ particleSystem.main.duration
					+ GetParticleSystemMinMaxCurveDuration(particleSystem, particleSystem.main.startLifetime);
				if (ignoreBeyondThisLimit < 0.0f || particleDuration < ignoreBeyondThisLimit)
				{
					duration = Mathf.Max(duration, particleDuration);
				}
			}
			return duration;
		}
	}
}
