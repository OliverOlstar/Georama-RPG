using UnityEngine;

namespace UI
{
	public class UICustomAnimationHandler<T, U> : UIBaseAnimationHandler
		where T : UICustomAnimation
		where U : Component
	{
		[SerializeField]
		protected T m_Animation = null;
		[SerializeField]
		protected U m_Target = null;

		[SerializeField]
		bool m_Loop = false;

		protected float m_ContentDuration = 0.0f;

		float m_ContentTimer = 0.0f;

		public override bool IsContentLooped()
		{
			return m_Loop;
		}

		public void SetLooping(bool looping)
		{
			 m_Loop = looping;
		}

		public override float GetContentDuration()
		{
			if (m_Loop)
			{
				return Mathf.Infinity;
			}
			if (m_Animation == null)
			{
				return 0.0f;
			}
			if (IsPlaying())
			{
				return m_ContentDuration;
			}
			return m_Animation.m_ContentDuration;
		}

		public void SetAnimation(T animation)
		{
			KillAnimation();
			m_Animation = animation;
		}

		public U GetTarget()
		{
			return m_Target;
		}

		public void SetTarget(U target)
		{
			KillAnimation();
			m_Target = target;
		}

		protected virtual void InitializeCustomAnimation() { }

		protected virtual void AnimateValues(float delta) { }

		protected override string GetContentName()
		{
			if (m_Animation != null)
			{
				return m_Animation.name;
			}
			return string.Empty;
		}

		protected override void InitializeAnimation()
		{
			if (m_Animation == null)
			{
				Debug.LogError(gameObject.name + "UIScaleAnimationHandler.StartAnimation: No animation set.", gameObject);
				KillAnimation();
				return;
			}
			if (m_Target == null)
			{
				Debug.LogError(gameObject.name + ".UIScaleAnimationHandler.StartAnimation: No target set.", gameObject);
				KillAnimation();
				return;
			}

			m_ContentDuration = m_Animation.m_ContentDuration;
			InitializeCustomAnimation();
		}

		protected override void StartAnimation()
		{
			if (m_Animation == null)
			{
				return;
			}
			if (m_Target == null)
			{
				return;
			}

			m_ContentTimer = 0.0f;
			AnimateValues(0.0f);
		}

		protected override void OnAnimationUpdate()
		{
			if (m_Animation == null)
			{
				return;
			}
			if (m_Target == null)
			{
				return;
			}

			if (!m_UseUnScaledTime)
			{
				m_ContentTimer += Mathf.Min(Core.TimeScaleManager.GetRealDeltaTime(), Core.Util.SPF30);
			}
			else
			{
				m_ContentTimer += Time.unscaledDeltaTime;
			}

			if (m_Loop)
			{
				m_ContentTimer = m_ContentTimer % m_ContentDuration;
			}

			AnimateValues(Mathf.Clamp01(m_ContentTimer / m_ContentDuration));
		}

		private void OnDisable()
		{
			ResetTarget();
		}

		public virtual void ResetTarget()
		{
			
		}
	}
}
