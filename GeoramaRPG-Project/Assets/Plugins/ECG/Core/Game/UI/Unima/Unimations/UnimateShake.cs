using UnityEngine;

[CreateAssetMenu(menuName = "Unimate/Core/Shake")]
public class UnimateShake : UnimateTween<UnimateShake, UnimateShake.Player>
{
	[SerializeField]
	private AnimationCurve m_ScaleCurve = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(1.0f, 1.0f));
	[SerializeField]
	private Vector3 m_Distance = Vector3.one;
	[SerializeField]
	private float m_Duration = 1.0f;
	public override float Duration => m_Duration;
	[SerializeField]
	private bool m_Loop = false;
	public override bool Loop => m_Loop;
	[SerializeField]
	public override bool UpdateBeforeStart => false;

	protected override string OnEditorValidate(GameObject gameObject)
	{
		float lastKey = m_ScaleCurve.keys[m_ScaleCurve.length - 1].time;
		if (!Core.Util.Approximately(lastKey, 1.0f))
		{
			return "Curve keys should be in the range 0 to 1";
		}
		return null;
	}

	public class Player : UnimaTweenPlayer<UnimateShake>
	{
		private Vector3 m_Position = Vector3.zero;

		protected override void OnInitialize()
		{
			m_Position = Transform.localPosition;
		}

		protected override void OnUpdateTween(float normalizedTime, float loopingTime)
		{
			Vector3 offset = new Vector3(
				Random.value,
				Random.value,
				Random.value);
			offset = Core.Util.Vector3Mul(offset, Animation.m_Distance);
			float scale = Animation.m_ScaleCurve.Evaluate(normalizedTime);
			offset *= scale;
			Transform.localPosition = m_Position + offset;
		}

		protected override void OnEndTween(bool interrupted)
		{
			Transform.localPosition = m_Position;
		}
	}
}
