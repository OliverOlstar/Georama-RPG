using UnityEngine;

namespace UI
{
	[CreateAssetMenu(fileName = "NewAlphaAnimation", menuName = "UIAnimation/AlphaAnimation", order = -1)]
	public class UIAlphaAnimation : UICustomAnimation
	{
		public AnimationCurve m_AlphaCurve = null;
	}
}
