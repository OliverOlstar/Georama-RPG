using UnityEngine;

[CreateAssetMenu(menuName = "Unimate/Core/Position XYZ")]
public class UnimatePositionXYZ : UnimateTween<UnimatePositionXYZ, UnimatePositionXYZ.Player>
{
	public enum OriginCache
	{
		Initialize,
		TweenStart,
	}

	public enum OnEnd
	{
		None = 0,
		RestoreOriginal,
	}

	[SerializeField]
	private AnimationCurve m_XPositionCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 0.0f);
	[SerializeField]
	private AnimationCurve m_YPositionCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 0.0f);
	[SerializeField]
	private AnimationCurve m_ZPositionCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 0.0f);

	[SerializeField]
	private Vector3 m_Distance = Vector3.one;
	[SerializeField]
	private OriginCache m_OriginCache = OriginCache.Initialize;
	[SerializeField]
	private OnEnd m_PositionOnEnd = OnEnd.None;

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
		float lastKey = m_XPositionCurve.keys[m_XPositionCurve.length - 1].time;
		if (!Core.Util.Approximately(lastKey, 1.0f))
		{
			return "X Position Curve keys should be in the range 0 to 1";
		}
		lastKey = m_YPositionCurve.keys[m_YPositionCurve.length - 1].time;
		if (!Core.Util.Approximately(lastKey, 1.0f))
		{
			return "Y Position Curve keys should be in the range 0 to 1";
		}
		lastKey = m_ZPositionCurve.keys[m_ZPositionCurve.length - 1].time;
		if (!Core.Util.Approximately(lastKey, 1.0f))
		{
			return "Z Position Curve keys should be in the range 0 to 1";
		}
		return null;
	}

	public class Player : UnimaTweenPlayer<UnimatePositionXYZ>
	{
		private Vector3 m_Origin = Vector3.zero;

		protected override void OnInitialize()
		{
			if (Animation.m_OriginCache == OriginCache.Initialize)
			{
				m_Origin = Transform.localPosition;
			}
		}

		protected override void OnStartTween()
		{
			if (Animation.m_OriginCache == OriginCache.TweenStart)
			{
				m_Origin = Transform.localPosition;
			}
		}

		protected override void OnUpdateTween(float normalizedTime, float loopingTime)
		{
			Vector3 offset = new Vector3(
				Animation.m_XPositionCurve.Evaluate(normalizedTime),
				Animation.m_YPositionCurve.Evaluate(normalizedTime),
				Animation.m_ZPositionCurve.Evaluate(normalizedTime));
			offset = Core.Util.Vector3Mul(offset, Animation.m_Distance);
			Transform.localPosition = m_Origin + offset;
		}

		protected override void OnEndTween(bool interrupted)
		{
			switch (Animation.m_PositionOnEnd)
			{
				case OnEnd.RestoreOriginal:
					Transform.localPosition = m_Origin;
					break;
			}
		}
	}
}
