using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace UI
{
	public class UIBaseAnimationHandler : MonoBehaviour
	{
		[System.Serializable]
		public class AnimationTreeNode : IComparable
		{
			[HideInInspector]
			public string m_Name = string.Empty;
			public UIBaseAnimationHandler m_Handler = null;
			public bool m_Guaranteed = false;
			public bool m_AfterStartDelay = false;
			[Delayed]
			public float m_Timing = -1.0f;

			[System.NonSerialized]
			public bool m_Played = false;

			public int CompareTo(object obj)
			{
				AnimationTreeNode otherNode = obj as AnimationTreeNode;
				if (otherNode == null)
				{
					return 0;
				}
				float thisTiming = Core.Util.Approximately(m_Timing, DIFFER_DURATION) ? Mathf.Infinity : m_Timing;
				float otherTiming = Core.Util.Approximately(otherNode.m_Timing, DIFFER_DURATION) ? Mathf.Infinity : otherNode.m_Timing;
				int comparison = thisTiming.CompareTo(otherTiming);
				if (comparison == 0)
				{
					if (m_Name == null)
					{
						comparison = -1;
					}
					else
					{
						string otherName = otherNode == null ? null : otherNode.m_Name;
						comparison = m_Name.CompareTo(otherName);
					}
				}
				return comparison;
			}
		}

		[System.Serializable]
		public class AnimationEvent
		{
			public string m_Name = string.Empty;
			public UnityEngine.Events.UnityEvent m_Event = new UnityEngine.Events.UnityEvent();
			public bool m_Guaranteed = false;
			public bool m_AfterStartDelay = false;
			[Delayed]
			public float m_Timing = -1.0f;

			[System.NonSerialized]
			public bool m_Played = false;

			public int CompareTo(object obj)
			{
				AnimationEvent otherNode = obj as AnimationEvent;
				if (otherNode == null)
				{
					return 0;
				}
				float thisTiming = Core.Util.Approximately(m_Timing, DIFFER_DURATION) ? Mathf.Infinity : m_Timing;
				float otherTiming = Core.Util.Approximately(otherNode.m_Timing, DIFFER_DURATION) ? Mathf.Infinity : otherNode.m_Timing;
				int comparison = thisTiming.CompareTo(otherTiming);
				if (comparison == 0)
				{
					comparison = m_Name.CompareTo(otherNode.m_Name);
				}
				return comparison;
			}
		}

		public enum Trigger
		{
			None = 0,
			OnEnable,
		}

		static readonly float DIFFER_DURATION = -1.0f;

		[SerializeField]
		string m_HandlerName = null;
		[SerializeField]
		uint m_StartFrameDelay = 0;
		[SerializeField]
		float m_StartDelay = 0.0f;
		[SerializeField]
		float m_Duration = -1.0f;
		[SerializeField]
		Trigger m_Trigger = Trigger.None;

		[SerializeField]
		protected bool m_UseUnScaledTime = false;

		public Trigger GetTrigger
		{
			get { return m_Trigger; }
		}
		[SerializeField]
		AnimationTreeNode[] m_AnimationTree = {};
		[SerializeField]
		AnimationEvent[] m_AnimationEvents = {};
		[SerializeField]
		private UnityEvent m_OnAnimationStartEvent = new UnityEvent();
		[SerializeField]
		private UnityEvent m_OnAnimationEndEvent = new UnityEvent();

		List<UIBaseAnimationHandler> m_FlattenedTree = null;
		bool m_Playing = false;
		bool m_PlayingTree = false;
		bool m_FrameDelayStart = false;
		bool m_DelayStart = false;
		int m_Counter = 0;
		float m_Timer = 0.0f;

		public UnityEvent OnAnimationStartEvent { get { return m_OnAnimationStartEvent; } }
		public UnityEvent OnAnimationEndEvent { get { return m_OnAnimationEndEvent; } }

		public static float ValidateTimeValue(float value)
		{
			if (value > -0.5f)
			{
				return Mathf.Max(value, 0.0f);
			}
			return DIFFER_DURATION;
		}

		public virtual bool IsContentLooped() { return false; }

		public virtual float GetContentDuration() { return 0.0f; }

		public void PlayAnimation(bool playTree = true)
		{
			if (IsPlaying())
			{
				EndAnimation(true);
			}

			m_Playing = true;
			m_PlayingTree = playTree;
			m_FrameDelayStart = m_StartFrameDelay > 0;
			m_DelayStart = m_StartDelay > Core.Util.EPSILON ? true : false;
			m_Counter = 0;
			m_Timer = 0.0f;

			InitializeAnimation();
			if (!m_FrameDelayStart)
			{
				if (!m_DelayStart)
				{
					StartAnimationInternal();
				}
				// frame delay is used to burn bad frames so dont let the tree start until after the frame delay.
				StartTree();
			}
			StartEvents();
		}

		public void KillAnimation()
		{
			EndAnimation(true);
		}

		public void DestroyMyself()
		{
			Destroy(gameObject);
		}

		public bool IsPlaying()
		{
			return m_Playing;
		}

		public bool IsTreePlaying()
		{
			foreach (UIBaseAnimationHandler handler in FlattenAnimationTree())
			{
				if (handler.IsPlaying())
				{
					return true;
				}
			}
			return false;
		}

		public float GetTotalDuration()
		{
			if (Core.Util.Approximately(m_Duration, DIFFER_DURATION))
			{
				return m_StartDelay + GetContentDuration();
			}
			return m_StartDelay + m_Duration;
		}

		public void SetStartDelay(float startDelay)
		{
			KillAnimation();
			m_StartDelay = startDelay;
		}

		public void AddAnimationEvent(float timing, UnityEngine.Events.UnityAction action)
		{
			AddAnimationEvent(false, timing, action);
		}

		public void AddAnimationEvent(bool afterStart, float timing, UnityEngine.Events.UnityAction action)
		{
			foreach (AnimationEvent animationEvent in m_AnimationEvents)
			{
				if (Core.Util.Approximately(animationEvent.m_Timing, timing))
				{
					animationEvent.m_Event.AddListener(action);
					return;
				}
			}
			AnimationEvent[] temp = m_AnimationEvents;
			m_AnimationEvents = new AnimationEvent[m_AnimationEvents.Length + 1];
			System.Array.Copy(temp, m_AnimationEvents, temp.Length);
			m_AnimationEvents[m_AnimationEvents.Length - 1] = new AnimationEvent();
			m_AnimationEvents[m_AnimationEvents.Length - 1].m_AfterStartDelay = afterStart;
			m_AnimationEvents[m_AnimationEvents.Length - 1].m_Timing = timing;
			m_AnimationEvents[m_AnimationEvents.Length - 1].m_Event.AddListener(action);
			m_AnimationEvents[m_AnimationEvents.Length - 1].m_Guaranteed = true;
			OnValidate();
		}

		public List<UIBaseAnimationHandler> FlattenAnimationTree()
		{
			if (m_FlattenedTree == null)
			{
				List<UIBaseAnimationHandler> flattenedTree = new List<UIBaseAnimationHandler>(m_AnimationTree.Length + 1);
				flattenedTree.Add(this);
				for (int i = 0; i < flattenedTree.Count; i++)
				{
					foreach (AnimationTreeNode node in flattenedTree[i].m_AnimationTree)
					{
						if (!flattenedTree.Contains(node.m_Handler))
						{
							flattenedTree.Add(node.m_Handler);
						}
					}
				}
				m_FlattenedTree = flattenedTree;
			}

			return m_FlattenedTree;
		}

		protected virtual string GetContentName() { return string.Empty; }

		protected virtual void InitializeAnimation() {}

		protected virtual void StartAnimation() {}

		protected virtual void OnAnimationUpdate() {}

		protected virtual void OnAnimationEnd(bool interrupted) {}

		protected virtual void OnContentValidation() {}

		void StartTree()
		{
			foreach (AnimationTreeNode node in m_AnimationTree)
			{
				node.m_Played = (!node.m_AfterStartDelay || !m_DelayStart) && Core.Util.Approximately(node.m_Timing, 0.0f);
				if (node.m_Played)
				{
					node.m_Handler.PlayAnimation();
				}
			}
		}

		void StartEvents()
		{
			foreach (AnimationEvent node in m_AnimationEvents)
			{
				node.m_Played = (!node.m_AfterStartDelay || !m_DelayStart) && Core.Util.Approximately(node.m_Timing, 0.0f);
				if (node.m_Played)
				{
					node.m_Event.Invoke();
				}
			}
		}

        public void ForceAllEvents()
        {
            foreach (AnimationEvent node in m_AnimationEvents)
            {
                if (node.m_Played)
                {
                    node.m_Event.Invoke();
                    node.m_Played = true;
                }
            }
        }

        bool PerformStartFrameDelay()
		{
			if (!m_FrameDelayStart)
			{
				return false;
			}

			if (m_Counter > m_StartFrameDelay)
			{
				m_FrameDelayStart = false;
				StartTree();
				if (!m_DelayStart)
				{
					// only start the animation here if there is no start delay.
					StartAnimationInternal();
				}
			}

			return m_FrameDelayStart;
		}

		bool PerformStartDelay()
		{
			if (!m_DelayStart)
			{
				return false;
			}

			if (m_Timer > m_StartDelay)
			{
				m_DelayStart = false;
				StartAnimationInternal();
			}

			return m_DelayStart;
		}

		float GetTiming(float timing, bool afterStartDelay)
		{
			if (Core.Util.Approximately(timing, DIFFER_DURATION))
			{
				return GetTotalDuration();
			}
			if (afterStartDelay)
			{
				return timing + m_StartDelay;
			}
			return timing;
		}

		void PlayChildAnimations()
		{
			foreach (AnimationTreeNode node in m_AnimationTree)
			{
				if (node.m_Played || node.m_Handler == null)
				{
					continue;
				}

				float timing = GetTiming(node.m_Timing, node.m_AfterStartDelay);
				if (m_Timer + Core.Util.EPSILON > timing)
				{
					node.m_Played = true;
					node.m_Handler.PlayAnimation();
				}
			}
			foreach (AnimationEvent node in m_AnimationEvents)
			{
				if (node.m_Played)
				{
					continue;
				}

				float timing = GetTiming(node.m_Timing, node.m_AfterStartDelay);
				if (m_Timer + Core.Util.EPSILON > timing)
				{
					node.m_Played = true;
					node.m_Event.Invoke();
				}
			}
		}

		void StartAnimationInternal()
		{
			m_OnAnimationStartEvent?.Invoke();
			StartAnimation();
		}

		void EndAnimation(bool interrupted)
		{
			if (!m_Playing)
			{
				return;
			}

			// play unplayed guarenteed nodes.
			foreach (AnimationTreeNode node in m_AnimationTree)
			{
				if (!node.m_Played && node.m_Handler != null && node.m_Guaranteed)
				{
					node.m_Played = true;
					node.m_Handler.PlayAnimation();
				}
			}
			foreach (AnimationEvent node in m_AnimationEvents)
			{
				if (!node.m_Played && node.m_Guaranteed)
				{
					node.m_Played = true;
					node.m_Event.Invoke();
				}
			}

			m_Playing = false;
			m_PlayingTree = false;
			m_DelayStart = false;
			m_Counter = 0;
			m_Timer = 0.0f;
			m_OnAnimationEndEvent?.Invoke();
			OnAnimationEnd(interrupted);
		}

		void OnEnable()
		{
			if (!m_Playing && m_Trigger == Trigger.OnEnable)
			{
				PlayAnimation();
			}
		}

		void OnDisable()
		{
			KillAnimation();
		}

		void Update()
		{
			if (!m_Playing)
			{
				return;
			}

			m_Counter++;

			if (PerformStartFrameDelay())
			{
				return;
			}

			if (!m_UseUnScaledTime)
			{
				m_Timer += Mathf.Min(Core.TimeScaleManager.GetRealDeltaTime(), Core.Util.SPF30);
			}
			else
			{
				m_Timer += Time.unscaledDeltaTime;
			}
			

			if (m_PlayingTree)
			{
				PlayChildAnimations();
			}

			if (PerformStartDelay())
			{
				return;
			}

			OnAnimationUpdate();

			if (m_Timer > GetTotalDuration())
			{
				EndAnimation(false);
			}
		}

		void OnValidate()
		{
			if (string.IsNullOrEmpty(m_HandlerName))
			{
				m_HandlerName = GetContentName();
			}

			m_Duration = ValidateTimeValue(m_Duration);

			foreach (AnimationTreeNode node in m_AnimationTree)
			{
				node.m_Name = node.m_Handler != null ? node.m_Handler.m_HandlerName : "Null";
				node.m_Timing = ValidateTimeValue(node.m_Timing);
			}
			foreach (AnimationEvent node in m_AnimationEvents)
			{
				node.m_Timing = ValidateTimeValue(node.m_Timing);
			}

			Array.Sort(m_AnimationTree);

			OnContentValidation();
		}
	}
}
