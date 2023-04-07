using UnityEngine;
using System.Collections.Generic;
namespace Core
{
	namespace UI
	{
		[ExecuteInEditMode]
		[DisallowMultipleComponent]
		[RequireComponent(typeof(RectTransform))]
		public class UILayout : MonoBehaviour
		{
			public enum Alignment
			{
				None = 0,
				Horizontal_Center = 1,
				Horizontal_Left = 2,
				Horizontal_Right = 3,
				Horizontal_Stretch = 4,
				Horizontal_Ends = 5,
				Vertical_Center = -1,
				Vertical_Bottom = -2,
				Vertical_Top = -3,
				Vertical_Stretch = -4,
				Vertical_Ends = -5,
			}

			public Alignment m_Alignment = Alignment.None;
			public bool m_SizeBased = true;
			public bool m_SizeBasedResizing = false;
			public bool m_IgnoreDisabled = true;
			public bool m_IgnoreScale = false;
			public bool m_ReverseOrder = false;
			public float m_Padding = 0.0f;
			public float m_Spacing = 0.0f;
			public bool m_AutoUpdateLayout = true;

			RectTransform m_RectTransform = null;
			float m_Offset = 0.0f;
			float m_SummedAlignmentDimension = 0.0f;
			List<RectTransform> m_ChildRectTransforms = new List<RectTransform>(50);

			public static void ForceLayoutUpdate(GameObject gameObject)
			{
				if (gameObject == null)
				{
					return;
				}
				UILayout layout = gameObject.GetComponent<UILayout>();
				if (layout != null)
				{
					layout.ApplyLayout();
				}
			}

			public void ApplyLayout()
			{
				if (m_RectTransform == null)
				{
					m_RectTransform = transform as RectTransform;
				}
				if (m_Alignment == Alignment.None)
				{
					return;
				}

				if (m_Alignment == Alignment.Horizontal_Stretch || m_Alignment == Alignment.Vertical_Stretch)
				{
					m_SizeBased = false;
					m_SizeBasedResizing = false;
				}

				m_Offset = m_Padding;
				UpdateChildRectTransforms();
				int count = m_ChildRectTransforms.Count;
				for (int i = 0; i < count; i++)
				{
					int index = i;
					if (m_ReverseOrder)
					{
						index = m_ChildRectTransforms.Count - i - 1;
					}
					if (m_Alignment == Alignment.Horizontal_Stretch || m_Alignment == Alignment.Vertical_Stretch)
					{
						Stretch((float)index, (float)m_ChildRectTransforms.Count, m_ChildRectTransforms[i]);
					}
					else if (m_SizeBased)
					{
						AlignSizeBased((float)m_ChildRectTransforms.Count, m_ChildRectTransforms[index]);
					}
					else
					{
						Align((float)index, (float)m_ChildRectTransforms.Count, m_ChildRectTransforms[i]);
					}
				}

				if (m_SizeBased && m_SizeBasedResizing)
				{
					if ((int)m_Alignment > 0)
					{
						m_RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_SummedAlignmentDimension);
					}
					else
					{
						m_RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_SummedAlignmentDimension);
					}
				}
			}

			float GetRectAlignmentDimension(RectTransform rectTransform)
			{
				if ((int)m_Alignment > 0)
				{
					return m_IgnoreScale ? rectTransform.rect.width : rectTransform.rect.width * rectTransform.localScale.x;
				}
				else
				{
					return m_IgnoreScale ? rectTransform.rect.height : rectTransform.rect.height * rectTransform.localScale.y;
				}
			}

			float GetVectorAlignmentDimension(Vector2 vector)
			{
				if ((int)m_Alignment > 0)
				{
					return vector.x;
				}
				else
				{
					return vector.y;
				}
			}

			void SetVectorAlignmentDimension(ref Vector2 vector, float value)
			{
				if ((int)m_Alignment > 0)
				{
					vector.x = value;
				}
				else
				{
					vector.y = value;
				}
			}

			float GetPivotAdjustment(RectTransform rectTransform)
			{
				if (m_Alignment == Alignment.Horizontal_Center || m_Alignment == Alignment.Vertical_Center)
				{
					return (GetVectorAlignmentDimension(rectTransform.pivot) - 0.5f) * GetRectAlignmentDimension(rectTransform);
				}
				else if (m_Alignment == Alignment.Horizontal_Left || m_Alignment == Alignment.Vertical_Bottom)
				{
					return GetVectorAlignmentDimension(rectTransform.pivot) * GetRectAlignmentDimension(rectTransform);
				}
				else if (m_Alignment == Alignment.Horizontal_Right || m_Alignment == Alignment.Vertical_Top)
				{
					return (1.0f - GetVectorAlignmentDimension(rectTransform.pivot)) * -GetRectAlignmentDimension(rectTransform);
				}
				return 0.0f;
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

				SetVectorAlignmentDimension(ref anchorMin, anchorMinValue);
				SetVectorAlignmentDimension(ref anchorMax, anchorMaxValue);
				SetVectorAlignmentDimension(ref offsetMin, 0.0f);
				SetVectorAlignmentDimension(ref offsetMax, 0.0f);

				childRectTransform.anchorMin = anchorMin;
				childRectTransform.anchorMax = anchorMax;
				childRectTransform.offsetMin = offsetMin;
				childRectTransform.offsetMax = offsetMax;
			}

			void AlignSizeBased(float floatChildCount, RectTransform childRectTransform)
			{
				Vector2 anchorMin = childRectTransform.anchorMin;
				Vector2 anchorMax = childRectTransform.anchorMax;
				Vector2 anchoredPosition = childRectTransform.anchoredPosition;

				float addedOffset = GetRectAlignmentDimension(childRectTransform);
				if (m_Alignment == Alignment.Horizontal_Center || m_Alignment == Alignment.Vertical_Center
					|| m_Alignment == Alignment.Horizontal_Ends || m_Alignment == Alignment.Vertical_Ends)
				{
					float positionValue = (m_SummedAlignmentDimension * -0.5f)
						+ m_Offset + (addedOffset * 0.5f)
						+ GetPivotAdjustment(childRectTransform);
					
					SetVectorAlignmentDimension(ref anchorMin, 0.5f);
					SetVectorAlignmentDimension(ref anchorMax, 0.5f);
					SetVectorAlignmentDimension(ref anchoredPosition, positionValue);
				}
				else if (m_Alignment == Alignment.Horizontal_Left || m_Alignment == Alignment.Vertical_Bottom)
				{
					float positionValue = m_Offset + GetPivotAdjustment(childRectTransform);
					
					SetVectorAlignmentDimension(ref anchorMin, 0.0f);
					SetVectorAlignmentDimension(ref anchorMax, 0.0f);
					SetVectorAlignmentDimension(ref anchoredPosition, positionValue);
				}
				else if (m_Alignment == Alignment.Horizontal_Right || m_Alignment == Alignment.Vertical_Top)
				{
					float positionValue = -m_Offset + GetPivotAdjustment(childRectTransform);
					
					SetVectorAlignmentDimension(ref anchorMin, 1.0f);
					SetVectorAlignmentDimension(ref anchorMax, 1.0f);
					SetVectorAlignmentDimension(ref anchoredPosition, positionValue);
				}

				childRectTransform.anchorMin = anchorMin;
				childRectTransform.anchorMax = anchorMax;
				childRectTransform.anchoredPosition = anchoredPosition;

				m_Offset += addedOffset + m_Spacing;
			}

			void Align(float floatIndex, float floatChildCount, RectTransform childRectTransform)
			{
				Vector2 anchorMin = childRectTransform.anchorMin;
				Vector2 anchorMax = childRectTransform.anchorMax;
				Vector2 anchoredPosition = childRectTransform.anchoredPosition;

				if (m_Alignment == Alignment.Horizontal_Center || m_Alignment == Alignment.Vertical_Center)
				{
					float anchorMidValue = (floatIndex + floatIndex + 1.0f) / (2.0f * floatChildCount);
					float positionValue = ((floatIndex + 0.5f - (floatChildCount * 0.5f)) * m_Spacing)
						+ GetPivotAdjustment(childRectTransform);

					SetVectorAlignmentDimension(ref anchorMin, anchorMidValue);
					SetVectorAlignmentDimension(ref anchorMax, anchorMidValue);
					SetVectorAlignmentDimension(ref anchoredPosition, positionValue);
				}
				else if (m_Alignment == Alignment.Horizontal_Left || m_Alignment == Alignment.Vertical_Bottom)
				{
					float anchorMinValue = floatIndex / floatChildCount;
					float positionValue = m_Padding 
						+ (floatIndex * m_Spacing) 
						+ GetPivotAdjustment(childRectTransform);

					SetVectorAlignmentDimension(ref anchorMin, anchorMinValue);
					SetVectorAlignmentDimension(ref anchorMax, anchorMinValue);
					SetVectorAlignmentDimension(ref anchoredPosition, positionValue);
				}
				else if (m_Alignment == Alignment.Horizontal_Right || m_Alignment == Alignment.Vertical_Top)
				{
					float anchorMaxValue = (floatIndex + 1.0f) / floatChildCount;
					float positionValue = -m_Padding 
						- ((floatChildCount - floatIndex - 1.0f) * m_Spacing) 
						+ GetPivotAdjustment(childRectTransform);

					SetVectorAlignmentDimension(ref anchorMin, anchorMaxValue);
					SetVectorAlignmentDimension(ref anchorMax, anchorMaxValue);
					SetVectorAlignmentDimension(ref anchoredPosition, positionValue);
				}
				else if (m_Alignment == Alignment.Horizontal_Ends || m_Alignment == Alignment.Vertical_Ends)
				{
					if (Core.Util.Approximately(floatChildCount, 1.0f))
					{
						float anchorMidValue = 0.5f;
						float positionValue = 0.0f;

						SetVectorAlignmentDimension(ref anchorMin, anchorMidValue);
						SetVectorAlignmentDimension(ref anchorMax, anchorMidValue);
						SetVectorAlignmentDimension(ref anchoredPosition, positionValue);
					}
					else
					{
						float anchorMidValue = floatIndex / (floatChildCount - 1.0f);
						float positionValue = ((floatIndex + 0.5f - (floatChildCount * 0.5f)) * m_Spacing)
							+ GetPivotAdjustment(childRectTransform);

						SetVectorAlignmentDimension(ref anchorMin, anchorMidValue);
						SetVectorAlignmentDimension(ref anchorMax, anchorMidValue);
						SetVectorAlignmentDimension(ref anchoredPosition, positionValue);
					}
				}

				childRectTransform.anchorMin = anchorMin;
				childRectTransform.anchorMax = anchorMax;
				childRectTransform.anchoredPosition = anchoredPosition;
			}

			List<RectTransform> UpdateChildRectTransforms()
			{
				m_ChildRectTransforms.Clear();

				m_SummedAlignmentDimension = 0.0f;
				int childCount = transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					Transform child = transform.GetChild(i);
					if (!child.gameObject.activeSelf && m_IgnoreDisabled || UIIgnoreLayout.IsIgnoringLayout(child))
					{
						continue;
					}
					RectTransform childRectTransform = child.GetComponent<RectTransform>();
					if (childRectTransform != null)
					{
						m_ChildRectTransforms.Add(childRectTransform);
						m_SummedAlignmentDimension += GetRectAlignmentDimension(childRectTransform);
					}
				}
				int count = m_ChildRectTransforms.Count;
				if (count > 0)
				{
					m_SummedAlignmentDimension += m_Padding * 2.0f;
					m_SummedAlignmentDimension += m_Spacing * Mathf.Max(count - 1.0f, 0.0f);
				}

				return m_ChildRectTransforms;
			}

			void OnEnable()
			{
				if (m_AutoUpdateLayout)
				{
					ApplyLayout();
				}
			}

			void LateUpdate()
			{
				if (m_AutoUpdateLayout)
				{
					ApplyLayout();
				}
			}
		}
	}
}
