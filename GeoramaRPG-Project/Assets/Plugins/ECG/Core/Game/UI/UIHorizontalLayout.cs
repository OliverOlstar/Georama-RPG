using UnityEngine;
using System.Collections.Generic;

namespace Core
{
	namespace UI
	{
		[ExecuteInEditMode]
		[System.Obsolete("Use UILayout instead.")]
		public class UIHorizontalLayout : MonoBehaviour
		{
			public enum Alignment
			{
				Center = 0,
				Left,
				Right,
				Stretch
			}

			public Alignment m_Alignment = Alignment.Center;
			public bool m_SizeBased = false;
			public bool m_IgnoreDisabled = false;
			public bool m_ReverseOrder = false;
			public float m_Padding = 0.0f;
			public float m_Spacing = 0.0f;

			float m_Offset = 0.0f;
			float m_TotalWidth = 0.0f;

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
				m_TotalWidth = m_Padding * 2.0f;

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
						m_TotalWidth += childRectTransform.rect.width + m_Spacing;
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

				pivot.x = 0.5f;
				anchorMin.x = anchorMinValue;
				anchorMax.x = anchorMaxValue;
				offsetMin.x = 0.0f;
				offsetMax.x = 0.0f;

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

				float childWidth = childRectTransform.rect.width + m_Spacing;

				if (m_Alignment == Alignment.Center)
				{
					pivot.x = 0.5f;
					anchorMin.x = 0.5f;
					anchorMax.x = 0.5f;
					anchoredPosition.x = (m_TotalWidth * -0.5f) + m_Offset + (childWidth * 0.5f);
				}
				else if (m_Alignment == Alignment.Left)
				{
					pivot.x = 0.0f;
					anchorMin.x = 0.0f;
					anchorMax.x = 0.0f;
					anchoredPosition.x = m_Offset;
				}
				else if (m_Alignment == Alignment.Right)
				{
					pivot.x = 1.0f;
					anchorMin.x = 1.0f;
					anchorMax.x = 1.0f;
					anchoredPosition.x = -m_Offset;
				}

				childRectTransform.pivot = pivot;
				childRectTransform.anchorMin = anchorMin;
				childRectTransform.anchorMax = anchorMax;
				childRectTransform.anchoredPosition = anchoredPosition;

				m_Offset += childWidth;
			}

			void Align(float floatIndex, float floatChildCount, RectTransform childRectTransform)
			{
				Vector2 pivot = childRectTransform.pivot;
				Vector2 anchorMin = childRectTransform.anchorMin;
				Vector2 anchorMax = childRectTransform.anchorMax;
				Vector2 anchoredPosition = childRectTransform.anchoredPosition;

				if (m_Alignment == Alignment.Center)
				{
					float anchorMidValue = (floatIndex + floatIndex + 1.0f) / (2.0f * floatChildCount);

					pivot.x = 0.5f;
					anchorMin.x = anchorMidValue;
					anchorMax.x = anchorMidValue;
					anchoredPosition.x = (floatIndex + 0.5f - (floatChildCount * 0.5f)) * m_Spacing;
				}
				else if (m_Alignment == Alignment.Left)
				{
					float anchorMinValue = floatIndex / floatChildCount;

					pivot.x = 0.0f;
					anchorMin.x = anchorMinValue;
					anchorMax.x = anchorMinValue;
					anchoredPosition.x = m_Padding + (floatIndex * m_Spacing);
				}
				else if (m_Alignment == Alignment.Right)
				{
					float anchorMaxValue = (floatIndex + 1.0f) / floatChildCount;

					pivot.x = 1.0f;
					anchorMin.x = anchorMaxValue;
					anchorMax.x = anchorMaxValue;
					anchoredPosition.x = -m_Padding - ((floatChildCount - floatIndex - 1.0f) * m_Spacing);
				}

				childRectTransform.pivot = pivot;
				childRectTransform.anchorMin = anchorMin;
				childRectTransform.anchorMax = anchorMax;
				childRectTransform.anchoredPosition = anchoredPosition;
			}
		}
	}
}
