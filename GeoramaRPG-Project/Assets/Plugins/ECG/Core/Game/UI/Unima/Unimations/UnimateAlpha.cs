
using UnityEngine;

[CreateAssetMenu(menuName = "Unimate/Core/Alpha")]
public class UnimateAlpha : UnimateTween<UnimateAlpha, UnimateAlpha.Player>
{
	[SerializeField]
	private AnimationCurve m_AlphaCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));
	[SerializeField]
	private float m_Duration = 1.0f;
	public override float Duration => m_Duration;
	[SerializeField]
	private bool m_Loop = false;
	public override bool Loop => m_Loop;
	[SerializeField]
	private bool m_UpdateBeforeStart = false;
	public override bool UpdateBeforeStart => m_UpdateBeforeStart;

	protected override string OnEditorValidate(GameObject gameObject)
	{
		float lastKey = m_AlphaCurve.keys[m_AlphaCurve.length - 1].time;
		if (!Core.Util.Approximately(lastKey, 1.0f))
		{
			return "Curve keys should be in the range 0 to 1";
		}
		return null;
	}

	public class Player : UnimaTweenPlayer<UnimateAlpha>
	{
		private CanvasGroup m_Group = null;

		protected override void OnInitialize()
		{
			m_Group = GameObject.GetComponent<CanvasGroup>();
			if (m_Group == null)
			{
				m_Group = GameObject.AddComponent<CanvasGroup>();
			}
		}

		protected override void OnUpdateTween(float normalizedTime, float loopingTime)
		{
			m_Group.alpha = Animation.m_AlphaCurve.Evaluate(normalizedTime);
		}
	}
}
