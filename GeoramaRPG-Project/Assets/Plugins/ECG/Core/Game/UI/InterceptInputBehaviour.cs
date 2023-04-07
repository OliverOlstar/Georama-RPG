using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Core
{
	namespace UI
	{
		public class InterceptInputBehaviour : MonoBehaviour
		{
			static bool s_IgnoreAll = false;
			public static void SetIgnoreAll(bool ignore) { s_IgnoreAll = ignore; }

			static bool s_Tutorial = false;
			public static bool IsTutorial() { return s_Tutorial; }
			public static void Tutorial(bool tutorial) { s_Tutorial = tutorial; }

			static List<RectTransform> s_InputBlockers = new List<RectTransform>();

			public static bool IgnoreInput()
			{
				return s_IgnoreAll || s_Tutorial;
			}

			public static bool IgnoreInput(Vector2 screenPosition)
			{
				if (s_IgnoreAll || s_Tutorial)
				{
					return true;
				}
				for (int i = 0; i < s_InputBlockers.Count; i++)
				{
					if (s_InputBlockers[i] == null)
					{
						continue;
					}
						
					Rect blockingRect = Util.ScreenRectForRectTransform(s_InputBlockers[i]);
					if (blockingRect.Contains(screenPosition))
					{
						return true;
					}
				}
				return false;
			}

			void OnEnable()
			{
				s_InputBlockers.Add(transform.GetComponent<RectTransform>());
			}

			void OnDisable()
			{
				s_InputBlockers.Remove(transform.GetComponent<RectTransform>());
			}
		}
	}
}
