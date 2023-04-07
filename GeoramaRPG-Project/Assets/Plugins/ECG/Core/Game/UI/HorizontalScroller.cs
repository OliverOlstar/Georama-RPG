using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
	public class HorizontalScroller : Scroller
	{
		public HorizontalScroller(ScrollRect scrollRect)
			: base(scrollRect)
		{
		}

		public HorizontalScroller(ScrollRect scrollRect, float duration, float maxDelta)
			: base(scrollRect, duration, maxDelta)
		{
		}

		protected override bool AllowHorizontal => true;
		protected override bool AllowVertical => false;

		protected override Vector2 GetScrollPosition(RectTransform transform, Alignment alignment, float distance)
		{
			switch (alignment)
			{
				case Alignment.Left:
				case Alignment.Middle:
				case Alignment.Right:
					break;
				case Alignment.Top:
				case Alignment.Bottom:
				default:
					throw new ArgumentException($"Invalid Aligment '{alignment}' for {nameof(HorizontalScroller)}");
			}
			float contentWidth = ScrollRect.content.rect.width;
			float viewportWidth = ScrollRect.viewport.rect.width;
			if (viewportWidth >= contentWidth)
			{
				return new Vector2(distance, 0);
			}
			float childX = ScrollRect.viewport.InverseTransformPoint(transform.position).x;
			float contentX = ScrollRect.viewport.InverseTransformPoint(ScrollRect.content.position).x;
			float y = ScrollRect.content.anchoredPosition.y;
			float x = contentX - childX;
			if (alignment == Alignment.Left && LayoutGroup)
			{
				x -= LayoutGroup.padding.left;
			}
			else if (alignment == Alignment.Middle)
			{
				//x -= (viewportWidth * 0.5f) - (transform.rect.width * 0.5f);
			}
			else if (alignment == Alignment.Right)
			{
				x -= viewportWidth - transform.rect.width;
			}
			return new Vector2(x + distance, y);
		}
	}
}
