using System;
using UnityEngine;

namespace OliverLoescher
{
	[System.Serializable]
	public class AnimatedValueInOut
	{
		[SerializeField]
		private AnimatedValue m_InAnimation = new AnimatedValue();
		[SerializeField]
		private AnimatedValue m_OutAnimation = new AnimatedValue();

		private Action<float> m_OnValueChanged = null;

		private bool? m_IsIn = false;

		public void StartIn(Action<float> onValueChanged)
		{
			if (m_IsIn.HasValue && m_IsIn.Value)
			{
				return;
			}
			m_IsIn = true;

			m_OutAnimation.Stop();
			m_InAnimation.Start(onValueChanged);
		}

		public void StartOut(Action<float> onValueChanged)
		{
			if (m_IsIn.HasValue && !m_IsIn.Value)
			{
				return;
			}
			m_IsIn = false;

			m_InAnimation.Stop();
			m_OutAnimation.Start(onValueChanged);
		}

		public void Stop()
		{
			m_InAnimation.Stop();
			m_OutAnimation.Stop();
		}

		public void Complete()
		{
			if (m_InAnimation.IsAnimating || m_InAnimation.IsPaused)
			{
				m_InAnimation.Complete();
			}
			else if (m_OutAnimation.IsAnimating || m_OutAnimation.IsPaused)
			{
				m_OutAnimation.Complete();
			}
		}

		public void Resume()
		{
			m_InAnimation.Resume();
			m_OutAnimation.Resume();
		}

		public void Pause()
		{
			m_InAnimation.Pause();
			m_OutAnimation.Pause();
		}
	}
}
