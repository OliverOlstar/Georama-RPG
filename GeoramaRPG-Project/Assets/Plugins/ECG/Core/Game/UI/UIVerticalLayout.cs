using UnityEngine;
using System.Collections.Generic;

namespace Core
{
	namespace UI
	{
		[ExecuteInEditMode]
		[System.Obsolete("Use UILayout instead.")]
		public class UIVerticalLayout : MonoBehaviour
		{
			public enum Alignment
			{
				Center = 0,
				Bottom,
				Top,
				Stretch
			}

			public Alignment m_Alignment = Alignment.Center;
			public bool m_SizeBased = false;
			public bool m_IgnoreDisabled = false;
			public bool m_ReverseOrder = false;
			public float m_Padding = 0.0f;
			public float m_Spacing = 0.0f;

			float m_Offset = 0.0f;
			float m_TotalHeight = 0.0f;

			void OnEnable()
			{
				LateUpdate();
			}

			void LateUpdate()
			{
				if (m_Alignment == Alignment.Stretch)
				{
					m_SizeBased = false;
				}

				m_Offset = m_Padding;
				m_TotalHeight = m_Padding * 2.0f;

				List<RectTransform> childRectTransforms = new List<RectTransform>();
				foreach (Transform child in transform)
				{
					if (!child.gameObject.activeSelf && m_IgnoreDisabled)
					{
						continue;
					}
					RectTransform childRectTransform = child.GetComponent<RectTransform>();
					if (childRectTransform != null)
					{
						childRectTransforms.Add(childRectTransform);
						m_TotalHeight += childRectTransform.rect.height + m_Spacing;
					}
				}
				for (int i = 0; i < childRectTransforms.Count; i++)
				{
					int index = i;
					if (m_ReverseOrder)
					{
						index = childRectTransforms.Count - i - 1;
					}
					if (m_Alignment == Alignment.Stretch)
					{
						Stretch((float)index, (float)childRectTransforms.Count, childRectTransforms[i]);
					}
					else if (m_SizeBased)
					{
						AlignSizeBased((float)childRectTransforms.Count, childRectTransforms[index]);
					}
					else
					{
						Align((float)index, (float)childRectTransforms.Count, childRectTransforms[i]);
					}
				}
			}

			void Stretch(float floatIndex, float floatChildCount, RectTransform childRectTransform)
			{
				Vector2 pivot = childRectTransform.pivot;
				Vector2 anchorMin = childRectTransform.anchorMin;
				Vector2 anchorMax = childRectTransform.anchorMax;
				Vector2 offsetMin = childRectTransform.offsetMin;
				Vector2 offsetMax = childRectTransform.offsetMax;

				float anchorMinValue = (floatIndex) / (floatChildCount);
				float anchorMaxValue = (floatIndex + 1.0f) / (floatChildCount);

				pivot.y = 0.5f;
				anchorMin.y = anchorMinValue;
				anchorMax.y = anchorMaxValue;
				offsetMin.y = 0.0f;
				offsetMax.y = 0.0f;

				childRectTransform.pivot = pivot;
				childRectTransform.anchorMin = anchorMin;
				childRectTransform.anchorMax = anchorMax;
				childRectTransform.offsetMin = offsetMin;
				childRectTransform.offsetMax = offsetMax;
			}

			void AlignSizeBased(float floatChildCount, RectTransform childRectTransform)
			{
				Vector2 pivot = childRectTransform.pivot;
				Vector2 anchorMin = childRectTransform.anchorMin;
				Vector2 anchorMax = childRectTransform.anchorMax;
				Vector2 anchoredPosition = childRectTransform.anchoredPosition;

				float childHeight = childRectTransform.rect.height + m_Spacing;

				if (m_Alignment == Alignment.Center)
				{
					pivot.y = 0.5f;
					anchorMin.y = 0.5f;
					anchorMax.y = 0.5f;
					anchoredPosition.y = (m_TotalHeight * -0.5f) + m_Offset + (childHeight * 0.5f);
				}
				else if (m_Alignment == Alignment.Bottom)
				{
					pivot.y = 0.0f;
					anchorMin.y = 0.0f;
					anchorMax.y = 0.0f;
					anchoredPosition.y = m_Offset;
				}
				else if (m_Alignment == Alignment.Top)
				{
					pivot.y = 1.0f;
					anchorMin.y = 1.0f;
					anchorMax.y = 1.0f;
					anchoredPosition.y = -m_Offset;
				}

				childRectTransform.pivot = pivot;
				childRectTransform.anchorMin = anchorMin;
				childRectTransform.anchorMax = anchorMax;
				childRectTransform.anchoredPosition = anchoredPosition;

				m_Offset += childHeight;
			}

			void Align(float floatIndex, float floatChildCount, RectTransform childRectTransform)
			{
				Vector2 pivot = childRectTransform.pivot;
				Vector2 anchorMin = childRectTransform.anchorMin;
				Vector2 anchorMax = childRectTransform.anchorMax;
				Vector2 anchoredPosition = childRectTransform.anchoredPosition;

				float parentHeight = GetComponent<RectTransform>().rect.height;

				if (m_Alignment == Alignment.Center)
				{
					float anchorMidValue = (floatIndex + floatIndex + 1.0f) / (2.0f * floatChildCount);

					pivot.y = 0.5f;
					anchorMin.y = anchorMidValue;
					anchorMax.y = anchorMidValue;
					anchoredPosition.y = (floatIndex + 0.5f - (floatChildCount * 0.5f)) * m_Spacing;
				}
				else if (m_Alignment == Alignment.Bottom)
				{
					float anchorMinValue = floatIndex / floatChildCount;

					pivot.y = 0.0f;
					anchorMin.y = anchorMinValue;
					anchorMax.y = anchorMinValue;
					anchoredPosition.y = m_Padding + (floatIndex * m_Spacing);
				}
				else if (m_Alignment == Alignment.Top)
				{
					float anchorMaxValue = (floatIndex + 1.0f) / floatChildCount;

					pivot.y = 1.0f;
					anchorMin.y = anchorMaxValue;
					anchorMax.y = anchorMaxValue;
					anchoredPosition.y = -m_Padding - ((floatChildCount - floatIndex - 1.0f) * m_Spacing);
				}

				childRectTransform.pivot = pivot;
				childRectTransform.anchorMin = anchorMin;
				childRectTransform.anchorMax = anchorMax;
				childRectTransform.anchoredPosition = anchoredPosition;
			}
		}
	}
}
