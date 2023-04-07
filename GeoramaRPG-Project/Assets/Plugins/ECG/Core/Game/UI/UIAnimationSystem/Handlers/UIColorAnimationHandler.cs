using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UI
{
	public class UIColorAnimationHandler : UICustomAnimationHandler<UIColorAnimation, Image>
	{
		protected override void AnimateValues(float delta)
		{
			float prog = m_Animation.m_SourceColorCurve.Evaluate(delta);
			m_Target.color = m_Animation.m_ColorGradient.Evaluate(prog);
		}
	}
}
