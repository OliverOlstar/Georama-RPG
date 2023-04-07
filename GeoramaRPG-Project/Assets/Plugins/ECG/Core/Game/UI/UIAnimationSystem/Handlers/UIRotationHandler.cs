using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
	public class UIRotationHandler : UICustomAnimationHandler<UIRotationAnimation, Transform>
	{
		public float m_Magnitude = 1.0f;

		private Vector3? m_StartingEulers = null;
		
		protected override void AnimateValues(float delta)
		{
			//first pass set the base value
			if (m_StartingEulers.HasValue)
			{
				m_StartingEulers = m_Target.eulerAngles;
			}
			
			Vector3 initialRotation = new Vector3(m_Target.eulerAngles.x,
				m_Target.eulerAngles.y,
				m_Animation.m_ZRotationCurve.Evaluate(0.0f));
			Vector3 finalRotation = new Vector3(m_Target.eulerAngles.x,
				m_Target.eulerAngles.y,
				m_Animation.m_ZRotationCurve.Evaluate(1.0f));
			Vector3 rotProgress = Vector3.Lerp(initialRotation, finalRotation, delta);

			Vector3 rotation = new Vector3(m_Target.eulerAngles.x,
				m_Target.eulerAngles.y,
				m_Animation.m_ZRotationCurve.Evaluate(delta));
			m_Target.eulerAngles = Vector3.LerpUnclamped(rotProgress, rotation, m_Magnitude);
		}

		public override void ResetTarget()
		{
			if (m_StartingEulers.HasValue)
			{
				m_Target.eulerAngles = m_StartingEulers.Value;
			}
		}
	}
}
