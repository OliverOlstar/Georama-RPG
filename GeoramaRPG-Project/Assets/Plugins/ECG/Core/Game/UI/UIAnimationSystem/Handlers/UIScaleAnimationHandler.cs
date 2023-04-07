using UnityEngine;
using System.Collections;

namespace UI
{
	public class UIScaleAnimationHandler : UICustomAnimationHandler<UIScaleAnimation, Transform>
	{
		public float m_Magnitude = 1.0f;

		protected override void AnimateValues(float delta)
		{
			Vector3 initialScale = new Vector3(
				m_Animation.m_XScaleCurve.Evaluate(0.0f),
				m_Animation.m_YScaleCurve.Evaluate(0.0f),
				m_Target.localScale.z);
			Vector3 finalScale = new Vector3(
				m_Animation.m_XScaleCurve.Evaluate(1.0f),
				m_Animation.m_YScaleCurve.Evaluate(1.0f),
				m_Target.localScale.z);
			Vector3 pivotScale = Vector3.Lerp(initialScale, finalScale, delta);

			Vector3 scale = new Vector3(
				m_Animation.m_XScaleCurve.Evaluate(delta),
				m_Animation.m_YScaleCurve.Evaluate(delta),
				m_Target.localScale.z);
			m_Target.localScale = Vector3.LerpUnclamped(pivotScale, scale, m_Magnitude);
		}
	}
}
