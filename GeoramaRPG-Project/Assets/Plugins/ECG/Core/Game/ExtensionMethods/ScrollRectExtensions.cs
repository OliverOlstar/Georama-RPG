using System.Reflection;
using UnityEngine.UI;

namespace Core
{
	public static class ScrollRectExtensions
	{
		private static FieldInfo s_Dragging;

		static ScrollRectExtensions()
		{
			s_Dragging = typeof(ScrollRect).GetField("m_Dragging", BindingFlags.Instance | BindingFlags.NonPublic);
		}

		public static bool IsDragging(this ScrollRect scrollRect)
		{
			return (bool)s_Dragging.GetValue(scrollRect);
		}
	}
}