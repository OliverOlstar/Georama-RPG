using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnimateTween<TAnimation, TPlayer> : Unimate<TAnimation, TPlayer>, IUnimateTween
	where TAnimation : UnimateBase
	where TPlayer : UnimaPlayer<TAnimation>, new()
{
	public abstract float Duration { get; }
	public abstract bool Loop { get; }
	public abstract bool UpdateBeforeStart { get; }

	public override UnimaDurationType GetEditorDuration(out float seconds)
	{
		if (Loop)
		{
			seconds = -1.0f;
			return UnimaDurationType.Infinite;
		}
		seconds = Duration;
		return UnimaDurationType.Fixed;
	}
}
