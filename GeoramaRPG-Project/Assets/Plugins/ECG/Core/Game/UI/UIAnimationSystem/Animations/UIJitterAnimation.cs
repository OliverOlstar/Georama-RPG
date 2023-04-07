using UnityEngine;

namespace UI
{
	[CreateAssetMenu(fileName = "NewJitterAnimation", menuName = "UIAnimation/JitterAnimation", order = -1)]
	public class UIJitterAnimation : UICustomAnimation
	{
		public AnimationCurve m_XPosCurve = null;
		public AnimationCurve m_YPosCurve = null;
	}
}
