
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Unimate/Core/Animation Clip")]
public class UnimateAnimationClip : UnimateTween<UnimateAnimationClip, UnimateAnimationClip.Player>
{
	[SerializeField]
	private AnimationClip m_Clip = null;
	[SerializeField]
	private float m_Speed = 1.0f;
	[SerializeField]
	private bool m_Loop = false;
	public override bool Loop => m_Loop;
	[SerializeField]
	private bool m_UpdateBeforeStart = false;
	public override bool UpdateBeforeStart => m_UpdateBeforeStart;
	[SerializeField]
	private bool m_CompleteOnInterupt = false;
	public bool CompleteOnInterupt => m_CompleteOnInterupt;

	public override float Duration => m_Clip == null ? 0.0f : m_Clip.length / m_Speed;

	protected override string OnEditorValidate(GameObject gameObject)
	{
		if (m_Clip == null)
		{
			return "Animation clip is null";
		}
		return null;
	}

	public class Player : UnimaTweenPlayer<UnimateAnimationClip>
	{
		private Core.SampleAnimatorClip m_Play = null;

		protected override void OnInitialize()
		{
			if (Animation.m_Clip == null)
			{
				return;
			}
			if (!Animation.m_Clip.legacy)
			{
				m_Play = Core.Util.GetOrAddComponent<Core.SampleAnimatorClip>(GameObject);
			}
		}

		protected override bool TryPlay(IUnimaContext context)
		{
			return Animation.m_Clip != null;
		}

		protected override void OnStartTween()
		{
			if (m_Play != null)
			{
				m_Play.SetClip(Animation.m_Clip);
			}
		}

		protected override void OnUpdateTween(float normalizedTime, float loopingTime)
		{
			// Note: It seems we can get undefined behaviour if due to float point precision time exceedes m_Clip.length
			// subtracting EPSILON should guarentee we never go over the end
			SetTime(normalizedTime * (Animation.m_Clip.length - Core.Util.EPSILON));
		}

		protected override void OnEndTween(bool interrupted)
		{
			if (interrupted && Animation.CompleteOnInterupt)
			{
				SetTime(float.MaxValue);
			}
		}

		private void SetTime(float time)
		{
			if (m_Play != null)
			{
				m_Play.SampleClip(Animation.m_Clip, time);
			}
			else
			{
				Animation.m_Clip.SampleAnimation(GameObject, time);
			}
		}
	}
}
