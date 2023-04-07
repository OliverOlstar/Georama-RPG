using UnityEngine;

namespace UI
{
	[CreateAssetMenu(fileName = "NewTranslateToAnimation", menuName = "UIAnimation/TranslateToAnimation", order = -1)]
	public class UITranslateToAnimation : UICustomAnimation
	{
		public float m_Speed = -1.0f;
		public float m_MinArcHeight = 0.0f;
		public float m_MaxArcHeight = 0.0f;
		public float m_MinArcApexPosition = 0.5f;
		public float m_MaxArcApexPosition = 0.0f;
		public AnimationCurve m_MovementCurve = null;

		void OnValidate()
		{
			UIBaseAnimationHandler.ValidateTimeValue(m_ContentDuration);
			if (m_ContentDuration < -0.5f)
			{
				m_Speed = Mathf.Max(m_Speed, 0.0f);
			}
			else
			{
				m_Speed = -1.0f;
			}
		}
	}
}
