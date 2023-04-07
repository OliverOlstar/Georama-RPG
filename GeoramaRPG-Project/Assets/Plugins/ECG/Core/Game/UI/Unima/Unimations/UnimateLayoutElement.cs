using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Unimate/Core/Layout Element")]
public class UnimateLayoutElement : UnimateTween<UnimateLayoutElement, UnimateLayoutElement.Player>
{
	public enum OriginCache
	{
		Initialize,
		TweenStart,
	}

	[SerializeField]
	private float m_Duration = 1.0f;
	[SerializeField]
	private AnimationCurve m_AnimationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
	[SerializeField]
	private LayoutElementValues m_TargetValues = new LayoutElementValues();
	[SerializeField]
	private OriginCache m_OriginCache = OriginCache.Initialize;

	public override bool Loop => false;
	public override bool UpdateBeforeStart => false;
	public override float Duration => m_Duration;

	protected override string OnEditorValidate(GameObject gameObject)
	{
		if (!gameObject.TryGetComponent<LayoutElement>(out _))
		{
			return $"Requires {nameof(LayoutElement)} component";
		}
		return null;
	}

	public class Player : UnimaTweenPlayer<UnimateLayoutElement>
	{
		private LayoutElement m_LayoutElement;
		private LayoutElementValues m_InitialValues;

		protected override bool TryPlay(IUnimaContext context) => m_LayoutElement != null;

		protected override void OnInitialize()
		{
			m_LayoutElement = GameObject.GetComponent<LayoutElement>();
			if (Animation.m_OriginCache == OriginCache.Initialize && m_LayoutElement)
			{
				m_InitialValues = new LayoutElementValues(m_LayoutElement);
			}
		}

		protected override void OnUpdateTween(float normalizedTime, float loopingTime)
		{
			if (Animation.m_OriginCache == OriginCache.TweenStart && m_LayoutElement)
			{
				m_InitialValues = new LayoutElementValues(m_LayoutElement);
			}
			if (m_InitialValues == null)
			{
				return;
			}
			AnimationCurve curve = Animation.m_AnimationCurve;
			LayoutElementValues targetValues = Animation.m_TargetValues;
			float time = curve.keys[curve.length - 1].time * normalizedTime;
			float eval = curve.Evaluate(time);
			if (targetValues.PreferredWidth >= 0f)
			{
				m_LayoutElement.preferredWidth = Mathf.LerpUnclamped(m_InitialValues.PreferredWidth, targetValues.PreferredWidth, eval);
			}
			if (targetValues.PreferredHeight >= 0f)
			{
				m_LayoutElement.preferredHeight = Mathf.LerpUnclamped(m_InitialValues.PreferredHeight, targetValues.PreferredHeight, eval);
			}
		}
	}

	[Serializable]
	private class LayoutElementValues
	{
		[SerializeField, Min(-1f)]
		private float m_PreferredWidth;
		[SerializeField, Min(-1f)]
		private float m_PreferredHeight;

		public float PreferredWidth => m_PreferredWidth;
		public float PreferredHeight => m_PreferredHeight;

		public LayoutElementValues()
		{
			m_PreferredWidth = 1f;
			m_PreferredHeight = 1f;
		}

		public LayoutElementValues(LayoutElement layoutElement)
		{
			m_PreferredWidth = layoutElement.preferredWidth;
			m_PreferredHeight = layoutElement.preferredHeight;
		}
	}
}
