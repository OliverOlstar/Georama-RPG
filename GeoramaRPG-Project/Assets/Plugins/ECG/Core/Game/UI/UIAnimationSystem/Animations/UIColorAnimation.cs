using UnityEngine;

namespace UI
{
	[CreateAssetMenu(fileName = "NewColorAnimation", menuName = "UIAnimation/ColorAnimation", order = -1)]
	public class UIColorAnimation : UICustomAnimation
	{
		public Gradient m_ColorGradient = null;
		public AnimationCurve m_SourceColorCurve = null;
	}
}
