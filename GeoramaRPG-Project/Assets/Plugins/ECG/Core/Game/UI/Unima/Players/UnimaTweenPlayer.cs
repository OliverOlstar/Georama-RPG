
using System.Collections.Generic;
using UnityEngine;

public interface IUnimateTween
{
	float Duration { get; }
	bool Loop { get; }
	bool UpdateBeforeStart { get; }
}

public abstract class UnimaTweenPlayer<TAnimation> : UnimaPlayer<TAnimation>
	where TAnimation : UnimateBase, IUnimateTween
{
	protected override void OnPreStart()
	{
		if (Animation.UpdateBeforeStart)
		{
			OnStartTween();
		}
	}

	protected override void OnPreUpdate(float deltaTime)
	{
		if (Animation.UpdateBeforeStart)
		{
			OnUpdateTween(0.0f, 0.0f);
		}
	}

	protected override void OnStart()
	{
		if (!Animation.UpdateBeforeStart)
		{
			OnStartTween();
		}
	}

	protected override bool OnUpdate(float deltaTime)
	{
		float loopingTimer = Timer;
		float normalizedTime = 1.0f;
		float duration = Animation.Duration;
		if (duration > Core.Util.EPSILON)
		{
			if (Animation.Loop)
			{
				loopingTimer %= duration;
				normalizedTime = Mathf.Clamp01(loopingTimer / duration);
			}
			else
			{
				normalizedTime = Mathf.Clamp01(Timer / duration);
			}
		}
		loopingTimer = Mathf.Min(loopingTimer, duration);
		OnUpdateTween(normalizedTime, loopingTimer);
		if (!Animation.Loop && Core.Util.Approximately(normalizedTime, 1.0f))
		{
			return false;
		}
		return true;
	}

	protected override void OnStop(bool interrupted)
	{
		OnEndTween(interrupted);
	}

	protected virtual void OnStartTween() { }

	protected virtual void OnUpdateTween(float normalizedTime, float loopingTime) { }

	protected virtual void OnEndTween(bool interrupted) { }
}
