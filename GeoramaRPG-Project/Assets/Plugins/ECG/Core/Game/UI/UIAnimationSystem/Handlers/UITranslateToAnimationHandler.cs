using UnityEngine;

namespace UI
{
	public class UITranslateToAnimationHandler : UICustomAnimationHandler<UITranslateToAnimation, RectTransform>
	{
		[SerializeField]
		RectTransform m_AnimateTo = null;
		public RectTransform GetAnimateTo() { return m_AnimateFrom; }
		[SerializeField]
		RectTransform m_AnimateFrom = null;
		public RectTransform GetAnimateFrom() { return m_AnimateFrom; }

		System.Action callbackDelete = null;

		Vector3 m_OriginPosition = Vector3.zero;
		float m_ArcHeight = 0.0f;
		float m_ArcApexPosition = 0.0f;

		public override float GetContentDuration()
		{
			if (IsPlaying())
			{
				return m_ContentDuration;
			}
			return base.GetContentDuration();
		}

		protected override void InitializeCustomAnimation()
		{
			if (m_AnimateTo == null)
			{
				KillAnimation();
				Debug.LogError(gameObject.name + "UITranslateToAnimationHandler.Initialize: Nothing to animate to.", gameObject);
				return;
			}

			m_OriginPosition = m_Target.position;
			if (m_Animation.m_MinArcHeight < m_Animation.m_MaxArcHeight - Core.Util.EPSILON)
			{
				m_ArcHeight = Random.Range(m_Animation.m_MinArcHeight, m_Animation.m_MaxArcHeight);
			}
			else
			{
				m_ArcHeight = m_Animation.m_MinArcHeight;
			}
			if (m_Animation.m_MinArcApexPosition < m_Animation.m_MaxArcApexPosition - Core.Util.EPSILON)
			{
				m_ArcApexPosition = Random.Range(m_Animation.m_MinArcApexPosition, m_Animation.m_MaxArcApexPosition);
			}
			else
			{
				m_ArcApexPosition = m_Animation.m_MinArcApexPosition;
			}
			if (m_Animation.m_ContentDuration > -0.5f)
			{
				m_ContentDuration = m_Animation.m_ContentDuration;
			}
			else
			{
				m_ContentDuration = Vector2.Distance(GetAnimateFromPosition(), GetAnimateToPosition()) / m_Animation.m_Speed;
			}
		}

		public void SetAnimateTo(RectTransform animateTo)
		{
			KillAnimation();
			m_AnimateTo = animateTo;
		}

		public void SetEndAction(System.Action callbackDelete) => this.callbackDelete = callbackDelete;

		public void SetAnimateFrom(RectTransform animateFrom)
		{
			KillAnimation();
			m_AnimateFrom = animateFrom;
		}

		protected override void AnimateValues(float delta)
		{
			if (m_AnimateTo == null)
			{
				return;
			}

			delta = m_Animation.m_MovementCurve.Evaluate(delta);
			Vector3 position = Core.Util.Lerp2DArc4(
				GetAnimateFromPosition(), 
				GetAnimateToPosition(), 
				m_ArcHeight, 
				m_ArcApexPosition, 
				delta);
			position.z = m_Target.position.z;
			m_Target.position = position;
		}

		protected override void OnAnimationEnd(bool interrupted)
		{
			Vector3 position = GetAnimateToPosition();
			position.z = m_Target.position.z;
			m_Target.position = position;
			callbackDelete?.Invoke();
		}

		Vector2 GetAnimateFromPosition()
		{
			if (m_AnimateFrom == null)
			{
				return m_OriginPosition;
			}
			return m_AnimateFrom.position;
		}

		Vector2 GetAnimateToPosition()
		{
			// this only works if the target and the animate to are on canvases with matching settings.
			// in the future this function may need to convert from a position on one canvas type to a position on another canvas type.
            if(m_AnimateTo == null)
            {
                return Vector3.one;
            }

			return m_AnimateTo.position;
		}
	}
}
