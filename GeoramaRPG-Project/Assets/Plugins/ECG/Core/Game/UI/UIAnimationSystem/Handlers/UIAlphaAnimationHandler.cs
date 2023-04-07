using UnityEngine;

namespace UI
{
	public class UIAlphaAnimationHandler : UICustomAnimationHandler<UIAlphaAnimation, CanvasGroup>
	{
		protected override void AnimateValues(float delta)
		{
			m_Target.alpha = m_Animation.m_AlphaCurve.Evaluate(delta);
		}
	}
}
