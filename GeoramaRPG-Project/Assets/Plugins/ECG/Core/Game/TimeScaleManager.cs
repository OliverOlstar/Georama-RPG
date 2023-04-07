
using UnityEngine;

namespace Core
{
	public class TimeScaleManager : MonoBehaviour 
	{
		public class TimeEvent
		{
			public float mTimer;

			int mHandle;
			float mScale;
			float mTime;

			bool mTimed;
			bool mScaleTimer;
			bool mAffectsAudio;
			bool mFadeOut;

			public TimeEvent(int handle, float scale, bool affectsAudio = false)
			{
				mHandle = handle;
				mScale = scale;
				mTimed = false;
				mScaleTimer = false;
				mTimer = -1.0f;
				mAffectsAudio = affectsAudio;
				mFadeOut = false;
			}

			public TimeEvent(int handle, float scale, float duration, bool scaleTimer, bool affectsAudio = false, bool fadeOut = false)
			{
				mHandle = handle;
				mScale = scale;
				mTimed = true;
				mScaleTimer = scaleTimer;
				mTime = mTimer = duration;
				mAffectsAudio = affectsAudio;
				mFadeOut = fadeOut;
			}

			public int GetHandle()
			{
				return mHandle;
			}

			public float GetScale()
			{
				if (mFadeOut)
				{
					float delta = 1.0f - Mathf.Clamp01(mTimer / mTime);
					delta *= delta;
					delta = 1.0f - delta;
					return Mathf.Lerp(1.0f, mScale, delta);
				}
				return mScale;
			}

			public bool IsTimed()
			{
				return mTimed;
			}

			public bool IsTimerScaled()
			{
				return mScaleTimer;
			}

			public bool ShouldAffectAudio()
			{
				return mAffectsAudio;
			}
		}

		public static readonly int INVALID_HANDLE = -11;

		static TimeScaleManager s_Singleton = null;

		// time scale at index 0 is the one currently active
		TimeEvent[] m_CurrentTimeEvents = new TimeEvent[10];
		int m_EndIndex = -1;
		int m_FreeHandles = 0;
		float m_PreviousRealTimeSinceStartup = 0.0f;
		float m_RealDeltaTime = 0.0f;
		float m_BaseTimeScale = 1.0f;
		bool m_Paused = false;
		bool m_FirstFramePaused = false;
		bool m_AffectAudio = false;
		
		int m_EditorSlowMo = 100;
		public int GetEditorSlowMo() { return m_EditorSlowMo; }

		public static TimeScaleManager Get()
		{
			return s_Singleton;
		}

		public static bool Exists()
		{
			return s_Singleton != null;
		}

		public void Awake()
		{
			if (s_Singleton == null)
			{
				s_Singleton = this;
			}
		}

		void Start()
		{
			m_PreviousRealTimeSinceStartup = Time.realtimeSinceStartup;
		}

		void OnDestroy()
		{
			s_Singleton = null;
		}
		
		static void PressedResume()
		{
			TimeScaleManager tem = TimeScaleManager.Get();
			if (tem == null)
			{
				return;
			}
			tem.UnPause();
		}
		
		static void PressedPause()
		{
			TimeScaleManager tem = TimeScaleManager.Get();
			if (tem == null)
			{
				return;
			}
			tem.Pause();
		}

		public void EditorStep()
		{
			if (m_Paused)
			{
				return;
			}
		}

		public void EditorInc()
		{
			if (m_EditorSlowMo >= 100)
			{
				m_EditorSlowMo += 100;
			}
			else if (m_EditorSlowMo >= 10)
			{
				m_EditorSlowMo += 10;
			}
			else if (m_EditorSlowMo >= 5)
			{
				m_EditorSlowMo += 5;
			}
			else if (m_EditorSlowMo >= 1)
			{
				m_EditorSlowMo += 1;
			}
		}

		public void EditorDec()
		{
			if (m_EditorSlowMo > 100)
			{
				m_EditorSlowMo -= 100;
			}
			else if (m_EditorSlowMo > 10)
			{
				m_EditorSlowMo -= 10;
			}
			else if (m_EditorSlowMo > 5)
			{
				m_EditorSlowMo -= 5;
			}
			else if (m_EditorSlowMo > 2)
			{
				m_EditorSlowMo -= 1;
			}
		}
		
		void Update()
		{
			m_FirstFramePaused = false;

			for(int i = 0; i <= m_EndIndex; i++)
			{
				if (m_CurrentTimeEvents[i].IsTimed())
				{
					if (m_CurrentTimeEvents[i].IsTimerScaled())
					{
						m_CurrentTimeEvents[i].mTimer -= Time.deltaTime;
					}
					else
					{
						m_CurrentTimeEvents[i].mTimer -= RealDeltaTime();
					}
					if (m_CurrentTimeEvents[i].mTimer <= 0.0f)
					{
						EndTimeEvent(m_CurrentTimeEvents[i].GetHandle());
					}
				}
			}

			ResetTimeScale();
			
			m_RealDeltaTime = (Time.realtimeSinceStartup - m_PreviousRealTimeSinceStartup) * m_BaseTimeScale;
			m_PreviousRealTimeSinceStartup = Time.realtimeSinceStartup;
		}
		
		public float RealDeltaTime()
		{
			if (m_Paused && !m_FirstFramePaused)
			{
				return 0.0f;
			}
			return m_RealDeltaTime;
		}
		
		public static float GetRealDeltaTime()
		{
			TimeScaleManager tem = TimeScaleManager.Get();
				
			if (tem == null)
			{
				return Time.deltaTime;
			}
			
			return tem.RealDeltaTime();
		}
		
		public void Pause()
		{
			m_Paused = true;
			ActualPause();
		}
		
		public void UnPause()
		{
			m_Paused = false;
			ResetTimeScale();
		}

		void ActualPause()
		{
			m_FirstFramePaused = true;
			Time.timeScale = 0.0f;
		}

		void ResetTimeScale()
		{			
			if (m_Paused)
			{
				m_AffectAudio = false;
				return;
			}
			
			float timeScale = m_EndIndex == -1 ? 1.0f : m_CurrentTimeEvents[0].GetScale();
			m_AffectAudio = m_EndIndex == -1 ? false : m_CurrentTimeEvents[0].ShouldAffectAudio();
			// Go with slowest time event
			for (int i = 1; i <= m_EndIndex; i++)
			{
				if (m_CurrentTimeEvents[i].GetScale() < timeScale)
				{
					timeScale = m_CurrentTimeEvents[i].GetScale();
					m_AffectAudio = m_CurrentTimeEvents[i].ShouldAffectAudio();
				}
			}

			timeScale *= m_BaseTimeScale;
			timeScale *= m_EditorSlowMo / 100.0f;
			Time.timeScale = timeScale;
		}
		
		public static int InvalidHandle()
		{
			return INVALID_HANDLE;
		}
		
		public int GetHandle()
		{
			for (int i = 0; i < 10; i++)
			{
				int mask = 1 << i;
				if ((mask & m_FreeHandles) == 0)
				{
					m_FreeHandles = mask | m_FreeHandles;
					return -1 - i;
				}
			}
			
			Debug.LogError("TimeScaleManager.GetHandle() there are no free handles");
			return INVALID_HANDLE;
		}

		public static void SetBaseTimeScale(float timeScale)
		{
			if (timeScale < Core.Util.LOW_PRECISION_EPSILON)
			{
				Debug.LogError("TimeScaleManager.SetBaseTimeScale: Time scale " + timeScale + " is invalid. Base time scale cannot be <= 0.");
			}
			TimeScaleManager tem = TimeScaleManager.Get();
			if (tem == null)
			{
				Debug.LogError("TimeScaleManager.SetBaseTimeScale: TimeScaleManager is null.");
				return;
			}

			tem.m_BaseTimeScale = timeScale;
			tem.ResetTimeScale();
		}

		public static float GetBaseTimeScale()
		{
			TimeScaleManager tem = TimeScaleManager.Get();
			if (tem == null)
			{
				Debug.LogError("TimeScaleManager.GetBaseTimeScale: TimeScaleManager is null.");
				return 1.0f;
			}

			return tem.m_BaseTimeScale;
		}

		public static float GetAudioTimeScale()
		{
			TimeScaleManager tem = TimeScaleManager.Get();
			if ( tem == null )
			{
				Debug.LogError( "TimeScaleManager.GetAudioTimeScale: TimeScaleManager is null." );
				return 1.0f;
			}
			if (tem.m_AffectAudio)
			{				
				return Time.timeScale;
			} else
			{
				return tem.m_BaseTimeScale * (tem.m_EditorSlowMo / 100.0f);
			}			
		}

		public static int StartTimeEvent(float timeScale, bool affectsAudio = false)
		{
			TimeScaleManager tem = TimeScaleManager.Get();
			if (tem == null)
			{
				Debug.LogError("TimeScaleManager.StartTimeEvent: TimeScaleManager is null.");
				return INVALID_HANDLE;
			}
			
			int handle = tem.GetHandle();
			
			TimeEvent timeEvent = new TimeEvent(handle, timeScale, affectsAudio);
			CreateTimeEvent(timeEvent);
			Debug.Log($"[{nameof(TimeScaleManager)}] Starting time event: {handle}");
			return handle;
		}

		public static int StartTimeEvent(float timeScale, float duration, bool scaleTimer, bool affectsAudio = false, bool fadeOut = false)
		{
			TimeScaleManager tem = TimeScaleManager.Get();
			if (tem == null)
			{
				Debug.LogError("TimeScaleManager.StartTimeEvent: TimeScaleManager is null.");
				return INVALID_HANDLE;
			}

			int handle = tem.GetHandle();

			TimeEvent timeEvent = new TimeEvent(handle, timeScale, duration, scaleTimer, affectsAudio, fadeOut);
			CreateTimeEvent(timeEvent);
			Debug.Log($"[{nameof(TimeScaleManager)}] Starting time event: {handle}");
			return handle;
		}

		public static void UpdateTimeEvent(int handle, float newTimeScale, bool affectAudio = false)
		{
			TimeScaleManager tem = TimeScaleManager.Get();
			if (tem == null)
			{
				Debug.LogError("TimeScaleManager.StartTimeEvent: TimeScaleManager is null.");
				return;
			}

			if (tem.m_EndIndex == -1)
			{
				return;
			}
			int index = -1;
			for (int i = 0; i <= tem.m_EndIndex; i++)
			{
				if (tem.m_CurrentTimeEvents[i].GetHandle() == handle)
				{
					index = i;
					break;
				}
			}
			if (index == -1)
			{
				return;
			}
			tem.m_CurrentTimeEvents[index] = new TimeEvent(handle, newTimeScale, affectAudio);
		}

		public static void EndTimeEvent(int handle)
		{
			TimeScaleManager tem = TimeScaleManager.Get();
			if (tem == null)
			{
				Debug.LogError("TimeScaleManager.StartTimeEvent: TimeScaleManager is null.");
				return;
			}

			if (tem.m_EndIndex == -1)
			{
				return;
			}
			int index = -1;
			for (int i = 0; i <= tem.m_EndIndex; i++)
			{
				if (tem.m_CurrentTimeEvents[i].GetHandle() == handle)
				{
					index = i;
					break;
				}
			}
			if (index == -1)
			{
				return;
			}
			for (int i = index; i < tem.m_EndIndex; i++)
			{
				tem.m_CurrentTimeEvents[i] = tem.m_CurrentTimeEvents[i + 1];
			}		
			tem.m_EndIndex--;

			if (handle < 0)
			{
				int mask = 1 << Mathf.Abs(handle + 1);
				tem.m_FreeHandles = tem.m_FreeHandles ^ mask;
			}

			tem.ResetTimeScale();
			Debug.Log($"[{nameof(TimeScaleManager)}] Ending time event: {handle}");
		}

		public static void EndAllTimeEvents()
		{
			TimeScaleManager tem = TimeScaleManager.Get();
			if (tem == null)
			{
				Debug.LogError("TimeScaleManager.StartTimeEvent: TimeScaleManager is null.");
				return;
			}

			if (tem.m_EndIndex == -1)
			{
				return;
			}
			for (int i = tem.m_EndIndex; i >= 0; i--)
			{
				EndTimeEvent(tem.m_CurrentTimeEvents[i].GetHandle());
			}
		}

		static void CreateTimeEvent(TimeEvent timeEvent)
		{
			TimeScaleManager tem = TimeScaleManager.Get();
			if (tem == null)
			{
				Debug.LogError("TimeScaleManager.StartTimeEvent: TimeScaleManager is null.");
				return;
			}

			if (tem.m_EndIndex == tem.m_CurrentTimeEvents.Length - 1)
			{
				Debug.LogError("TimeScaleManager.StartTimeEvent() attempting to start new timeevent " + timeEvent.GetHandle() + " when max timeevents are active");
				return;
			}
			if (timeEvent == null)
			{
				Debug.LogError("TimeScaleManager.StartTimeEvent() time event that is being started is null");
				return;
			}

			for (int i = tem.m_EndIndex; i >= 0; i--)
			{
				tem.m_CurrentTimeEvents[i + 1] = tem.m_CurrentTimeEvents[i];
			}

			tem.m_CurrentTimeEvents[0] = timeEvent;
			tem.m_EndIndex++;

			tem.ResetTimeScale();
		}
	}
}

