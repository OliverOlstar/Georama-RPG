using UnityEngine;

namespace UI
{
	[CreateAssetMenu(fileName = "NewScaleAnimation", menuName = "UIAnimation/ScaleAnimation", order = -1)]
	public class UIScaleAnimation : UICustomAnimation
	{
		public AnimationCurve m_XScaleCurve = null;
		public AnimationCurve m_YScaleCurve = null;
		public AnimationCurve m_ZScaleCurve = null;
	}
}
