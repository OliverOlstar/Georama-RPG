using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
	public class UIJitterAnimationHandler : UICustomAnimationHandler<UIJitterAnimation, Transform>
	{
		public float m_Magnitude = 1.0f;

		private Vector3? m_StartingPosition = null;
		
		protected override void AnimateValues(float delta)
		{
			//first pass set the base value
			if (m_StartingPosition.HasValue)
			{
				m_StartingPosition = m_Target.eulerAngles;
			}
			
			Vector3 initialPos = new Vector3(
				m_Animation.m_XPosCurve.Evaluate(0.0f),
				m_Animation.m_YPosCurve.Evaluate(0.0f),
				m_Target.localPosition.z);
			Vector3 finalPos = new Vector3(
				m_Animation.m_XPosCurve.Evaluate(1.0f),
				m_Animation.m_YPosCurve.Evaluate(1.0f),
				m_Target.localPosition.z);
			Vector3 pivotScale = Vector3.Lerp(initialPos, finalPos, delta);

			Vector3 pos = new Vector3(
				m_Animation.m_XPosCurve.Evaluate(delta),
				m_Animation.m_YPosCurve.Evaluate(delta),
				m_Target.localPosition.z);
			m_Target.localPosition = Vector3.LerpUnclamped(pivotScale, pos, m_Magnitude);
		}
		
		public override void ResetTarget()
		{
			if (m_StartingPosition.HasValue)
			{
				m_Target.localPosition = m_StartingPosition.Value;
			}
		}
	}
}
