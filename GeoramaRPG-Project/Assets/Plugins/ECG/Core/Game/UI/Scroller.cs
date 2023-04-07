using OliverLoescher;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
	public abstract class Scroller
	{
		public const float DefaultDuration = 0.35f;
		public const float DefaultMaxDelta = 1000f;

		public enum Alignment
		{
			Top,
			Bottom,
			Left,
			Right,
			Middle,
		}

		public bool IsScrolling => m_Registered;

		public Vector2 Velocity => ScrollRect.velocity;

		protected ScrollRect ScrollRect { get; private set; }
		public bool IsDragging => ScrollRect.IsDragging();
		public Vector2 AnchoredPos
		{
			get
			{
				if (ScrollRect == null)
				{
					return Vector2.zero;
				}
				return ScrollRect.content.anchoredPosition;
			}
		}
		private MonoUtil.Updateable m_Updateable = new MonoUtil.Updateable(MonoUtil.UpdateType.Default, MonoUtil.Priorities.UI);
		protected LayoutGroup LayoutGroup { get; private set; }
		private float m_Duration;
		private float m_MaxDelta;
		private Vector2 m_CurrentVelocity;
		private RectTransform m_Target;
		private Vector2 m_TargetPosition;
		private Alignment m_Alignment;
		private float m_Distance = 0;
		private bool m_Registered;

		protected abstract bool AllowHorizontal { get; }
		protected abstract bool AllowVertical { get; }
		protected abstract Vector2 GetScrollPosition(RectTransform transform, Alignment alignment, float distance);

		public Scroller(ScrollRect scrollRect)
			: this(scrollRect, DefaultDuration, DefaultMaxDelta)
		{
		}

		public Scroller(ScrollRect scrollRect, float duration, float maxDelta)
		{
			if (!scrollRect) throw new ArgumentNullException(nameof(scrollRect));
			ScrollRect = scrollRect;
			ScrollRect.horizontal = AllowHorizontal;
			ScrollRect.vertical = AllowVertical;
			LayoutGroup = ScrollRect.GetComponentInChildren<LayoutGroup>();
			m_Duration = duration;
			m_MaxDelta = maxDelta;
			m_CurrentVelocity = Vector2.zero;
			m_TargetPosition = Vector2.zero;
		}
		
		public void SetScrollDuration(float duration)
		{
			if (duration < 0f) throw new ArgumentException($"{nameof(duration)} must be non-negative", nameof(duration));
			m_Duration = duration;
		}

		public void ScrollTo(RectTransform child, Alignment alignment, float distance = 0)
		{
			m_Target = child;
			m_Distance = distance;
			m_Alignment = alignment;
			ScrollRect.velocity = Vector2.zero;
			m_TargetPosition = GetScrollPosition(m_Target, alignment, distance);
			if (!m_Registered)
			{
				m_Registered = true;
				m_Updateable.Register(OnUpdate);
			}
		}

		public void SnapTo(RectTransform child, Alignment alignment, float distance = 0)
		{
			ScrollRect.content.anchoredPosition = GetScrollPosition(child, alignment, 0);
			Stop();
			ScrollRect.velocity = Vector2.zero;
		}

		public void Stop()
		{
			m_TargetPosition = Vector2.zero;
			m_CurrentVelocity = Vector2.zero;
			if (m_Registered)
			{
				m_Registered = false;
				m_Updateable.Deregister();
			}
		}

		void OnUpdate(float deltaTime)
		{
			if (ScrollRect.IsDragging())
			{
				Stop();
				return;
			}
			if (m_Target)
			{
				m_TargetPosition = GetScrollPosition(m_Target, m_Alignment, m_Distance);
			}
			Vector2 position = Vector2.SmoothDamp(ScrollRect.content.anchoredPosition, m_TargetPosition, ref m_CurrentVelocity, m_Duration, m_MaxDelta, (float)deltaTime);
			ScrollRect.content.anchoredPosition = position;
			if (Util.Approximately(m_TargetPosition, position))
			{
				Stop();
			}
		}
		
		protected Vector2 GetPosition(RectTransform child, Alignment alignment)
		{
			float axisVal = 0;

			float contSize = AllowHorizontal ? ScrollRect.content.rect.width : ScrollRect.content.rect.height;
			float viewSize = AllowHorizontal ? ScrollRect.viewport.rect.width : ScrollRect.viewport.rect.height;		
			float dif = contSize - viewSize;
		
			if (dif > 0.0f)
			{
				Vector3 childPoint = ScrollRect.viewport.InverseTransformPoint(child.position);
				Vector3 contentPoint = ScrollRect.viewport.InverseTransformPoint(ScrollRect.content.position);
			
				float childs = AllowHorizontal ? childPoint.x : childPoint.y;
				float contents = AllowHorizontal ? contentPoint.x : contentPoint.y;
				axisVal = contents - childs;
				
				//because of how scrollers is made this assumes the pivot of the children is at the upper middle of the object
				axisVal = Mathf.Clamp(axisVal, 0, dif);
			}
		
			return new Vector2(AllowHorizontal ? axisVal: ScrollRect.content.anchoredPosition.x , AllowVertical ? axisVal: ScrollRect.content.anchoredPosition.y);
		}
		
		public int FindClosest()
		{
			if (ScrollRect == null)
			{
				return 0;
			}
			
			int closest = -1;
			float minDist = -1;

			int skipped = 0;
			
			//have to calc this here just so we can tell if the scroll is at the end
			float contSize = AllowHorizontal ? ScrollRect.content.rect.width : ScrollRect.content.rect.height;
			float viewSize = AllowHorizontal ? ScrollRect.viewport.rect.width : ScrollRect.viewport.rect.height;		
			float dif = (int)(contSize - viewSize);
			int currentPos = (int)(AllowHorizontal ? ScrollRect.content.anchoredPosition.x : ScrollRect.content.anchoredPosition.y);
			
			for (int i = 0; i < ScrollRect.content.childCount; ++i)
			{
				Transform child = ScrollRect.content.GetChild(i);
				LayoutElement element = child.GetComponent<LayoutElement>();
				if (child.gameObject.activeSelf && (element == null || !element.ignoreLayout))
				{
					Vector2 loc = GetPosition(child.GetComponent<RectTransform>(), m_Alignment);
					float dist = Vector2.Distance(ScrollRect.content.anchoredPosition, loc);

					if (minDist < 0 || dist < minDist || currentPos >= dif) //last check is in case the scroll is at the very bottom becuase anchoring
					{
						closest = i - skipped;
						minDist = dist;
					}
				}
				else
				{
					++skipped;
				}
			}

			//on init before everything is sized right it has a problem finding the first entry without delaying frames. we can assumed if its at the beginning its the first
			if (currentPos <= 0.0f)
			{
				closest = 0;
			}
			
			return closest;
		}
	}
}
