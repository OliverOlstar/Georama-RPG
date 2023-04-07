using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
	public class VerticalScroller : Scroller
	{
		public VerticalScroller(ScrollRect scrollRect)
			: base(scrollRect)
		{
		}

		public VerticalScroller(ScrollRect scrollRect, float duration, float maxDelta)
			: base(scrollRect, duration, maxDelta)
		{
		}

		protected override bool AllowHorizontal => false;
		protected override bool AllowVertical => true;

		protected override Vector2 GetScrollPosition(RectTransform transform, Alignment alignment, float distance)
		{
			switch (alignment)
			{
				case Alignment.Top:
				case Alignment.Bottom:
				case Alignment.Middle:
					break;
				case Alignment.Left:
				case Alignment.Right:
				default:
					throw new ArgumentException($"Invalid Aligment '{alignment}' for {nameof(VerticalScroller)}");
			}
			float contentHeight = ScrollRect.content.rect.height;
			float viewportHeight = ScrollRect.viewport.rect.height;
			if (viewportHeight >= contentHeight)
			{
				return new Vector2(0, distance);
			}
			float childY = ScrollRect.viewport.InverseTransformPoint(transform.position).y;
			float contentY = ScrollRect.viewport.InverseTransformPoint(ScrollRect.content.position).y;
			float x = ScrollRect.content.anchoredPosition.x;
			float y = contentY - childY;
			if (alignment == Alignment.Top && LayoutGroup)
			{
				y -= LayoutGroup.padding.top;
			}
			else if (alignment == Alignment.Middle)
			{
				y -= (viewportHeight * 0.5f) - (transform.rect.height * 0.5f);
			}
			else if (alignment == Alignment.Bottom)
			{
				y -= viewportHeight - transform.rect.height;
			}
			y = Mathf.Clamp(y, 0f, contentHeight - viewportHeight);
			return new Vector2(x, y + distance);
		}
	}
	
}
