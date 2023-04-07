using UnityEngine;

[CreateAssetMenu(menuName = "Unimate/Core/ScaleXYZ")]
public class UnimateScaleXYZ : UnimateTween<UnimateScaleXYZ, UnimateScaleXYZ.Player>
{
	[SerializeField]
	private AnimationCurve m_XScaleCurve = null;
	[SerializeField]
	private AnimationCurve m_YScaleCurve = null;
	[SerializeField]
	private AnimationCurve m_ZScaleCurve = null;
	[SerializeField]
	private float m_Duration = 1.0f;
	[SerializeField]
	private Vector3 m_ScaleFactor = Vector3.one;
	public override float Duration => m_Duration;
	[SerializeField]
	private bool m_Loop = false;
	public override bool Loop => m_Loop;
	[SerializeField]
	private bool m_UpdateBeforeStart = false;
	public override bool UpdateBeforeStart => m_UpdateBeforeStart;

	protected override string OnEditorValidate(GameObject gameObject)
	{
		float lastKey = m_XScaleCurve.keys[m_XScaleCurve.length - 1].time;
		if (!Core.Util.Approximately(lastKey, 1.0f))
		{
			return "X Scale Curve keys should be in the range 0 to 1";
		}
		lastKey = m_YScaleCurve.keys[m_YScaleCurve.length - 1].time;
		if (!Core.Util.Approximately(lastKey, 1.0f))
		{
			return "Y Scale Curve keys should be in the range 0 to 1";
		}
		lastKey = m_ZScaleCurve.keys[m_ZScaleCurve.length - 1].time;
		if (!Core.Util.Approximately(lastKey, 1.0f))
		{
			return "Z Scale Curve keys should be in the range 0 to 1";
		}
		return null;
	}

	public class Player : UnimaTweenPlayer<UnimateScaleXYZ>
	{
		private Vector3 m_InitialScale = Vector3.one;

		protected override void OnInitialize()
		{
			m_InitialScale = Transform.localScale;
		}

		protected override void OnUpdateTween(float normalizedTime, float loopingTime)
		{
			Vector3 scale = new Vector3(
				Animation.m_XScaleCurve.Evaluate(normalizedTime) * Animation.m_ScaleFactor.x,
				Animation.m_YScaleCurve.Evaluate(normalizedTime) * Animation.m_ScaleFactor.y,
				Animation.m_ZScaleCurve.Evaluate(normalizedTime) * Animation.m_ScaleFactor.z);
			Transform.localScale = Core.Util.Vector3Mul(m_InitialScale, scale);
		}
	}
}
