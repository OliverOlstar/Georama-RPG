using UnityEngine;

namespace UI
 {
 	[CreateAssetMenu(fileName = "NewRotationAnimation", menuName = "UIAnimation/RotationAnimation", order = -1)]
 	public class UIRotationAnimation : UICustomAnimation
 	{
 		public AnimationCurve m_ZRotationCurve = null;
 	}
 }
