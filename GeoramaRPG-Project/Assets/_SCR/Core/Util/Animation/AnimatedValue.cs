using System;
using UnityEngine;

namespace OliverLoescher
{
	[System.Serializable]
    public class AnimatedValue
    {
		private enum State
		{
			None,
			Running,
			Paused
		}

		[SerializeField]
		private AnimationCurve m_Curve = new AnimationCurve();
		[SerializeField, Min(Util.NEARZERO)]
		private float m_Duration = 1.0f;
		[SerializeField]
		private Vector2 m_Range = Vector2.up;

		[SerializeField]
		private Updateable m_Updateable = new Updateable(MonoUtil.UpdateType.Late, MonoUtil.Priorities.Animation);

		private State m_State = State.None;
		private float m_Progress01 = 0.0f;
		private Action<float> m_OnValueChanged = null;

		public bool IsAnimating => m_State == State.Running;
		public bool IsPaused => m_State == State.Paused;

		public void Start(Action<float> onValueChanged, bool canRestart = false)
		{
			if (!canRestart && m_State != State.None)
			{
				return; // Already started
			}
			if (m_State != State.Running)
			{
				m_Updateable.Register(Tick);
			}
			m_OnValueChanged = onValueChanged;
			m_State = State.Running;
			m_Progress01 = 0.0f;
			UpdateValue();
		}
		
		public void Stop()
		{
			if (m_State == State.Running)
			{
				m_Updateable.Deregister();
			}
			m_State = State.None;
		}

		public void Complete()
		{
			Stop();
			m_Progress01 = 1.0f;
			UpdateValue();
		}

		public void Pause()
		{
			if (m_State != State.Running)
			{
				return;
			}
			m_Updateable.Deregister();
			m_State = State.Paused;
		}

		public void Resume()
		{
			if (m_State != State.Paused)
			{
				return;
			}
			m_Updateable.Register(Tick);
			m_State = State.Running;
		}

		public void Tick(float pDeltaTime)
		{
			m_Progress01 = Mathf.Clamp01(m_Progress01 + (pDeltaTime / m_Duration));
			UpdateValue();
			if (m_Progress01 >= 1.0f)
			{
				Complete();
			}
		}

		private void UpdateValue()
		{
			float value = m_Curve.Evaluate(m_Progress01);
			m_OnValueChanged?.Invoke(value * (m_Range.y - m_Range.x) + m_Range.x);
		}
    }
}
