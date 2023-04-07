using UnityEngine;

[CreateAssetMenu(menuName = "Unimate/Core/Scale")]
public class UnimateScale : UnimateTween<UnimateScale, UnimateScale.Player>
{
	public enum ScaleType
	{
		RelativeOriginal = 0,
		OverwriteOriginal,
	}
	public enum OnEnd
	{
		None = 0,
		RestoreOriginal,
	}

	[SerializeField]
	private AnimationCurve m_ScaleCurve = null;
	[SerializeField]
	private ScaleType m_ScaleType = ScaleType.RelativeOriginal;
	[SerializeField]
	private OnEnd m_ScaleOnEnd = OnEnd.None;
	[SerializeField]
	private float m_Duration = 1.0f;
	public override float Duration => m_Duration;
	[SerializeField]
	private bool m_Loop = false;
	public override bool Loop => m_Loop;
	[SerializeField]
	private bool m_UpdateBeforeStart = false;

	[SerializeField]
	private float m_Multiplier = 1;
	public override bool UpdateBeforeStart => m_UpdateBeforeStart;

	protected override string OnEditorValidate(GameObject gameObject)
	{
		float lastKey = m_ScaleCurve.keys[m_ScaleCurve.length - 1].time;
		if (!Core.Util.Approximately(lastKey, 1.0f))
		{
			return "Curve keys should be in the range 0 to 1";
		}
		return null;
	}

	public class Player : UnimaTweenPlayer<UnimateScale>
	{
		private Vector3 m_InitialScale = Vector3.one;
		private Vector3 m_BaseScale = Vector3.one;

		protected override void OnInitialize()
		{
			m_InitialScale = Transform.localScale;
			m_BaseScale = Animation.m_ScaleType == ScaleType.RelativeOriginal ? Transform.localScale : Vector3.one;
		}

		protected override void OnUpdateTween(float normalizedTime, float loopingTime)
		{
			float scale = Animation.m_ScaleCurve.Evaluate(normalizedTime);
			Transform.localScale = (Animation.m_Multiplier * scale) * m_BaseScale;
		}

		protected override void OnEndTween(bool interrupted)
		{
			switch (Animation.m_ScaleOnEnd)
			{
				case OnEnd.RestoreOriginal:
					Transform.localScale = m_InitialScale;
					break;
			}
		}
	}
}
