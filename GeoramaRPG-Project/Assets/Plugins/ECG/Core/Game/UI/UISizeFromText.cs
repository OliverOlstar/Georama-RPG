using UnityEngine;
using UnityEngine.UI;

namespace Core
{
	namespace UI
	{
		[ExecuteInEditMode]
		[RequireComponent(typeof(RectTransform))]
		public class UISizeFromText : MonoBehaviour
		{
			[SerializeField]
			Text m_SizeFromText = null;
			[SerializeField]
			bool m_SetWidth = false;
			[SerializeField]
			bool m_SetHeight = false;
			[SerializeField]
			float m_MinWidth = 0.0f;
			[SerializeField]
			float m_MinHeight = 0.0f;
			[SerializeField]
			float m_MaxWidth = -1.0f;
			[SerializeField]
			float m_MaxHeight = -1.0f;
			[SerializeField]
			float m_AddedWidth = 0.0f;
			[SerializeField]
			float m_AddedHeight = 0.0f;
			[SerializeField]
			bool m_AutoUpdateLayout = true;

			RectTransform m_RectTransform = null;
			RectTransform m_TextRectTransform = null;
			TextGenerator m_TextGenerator = null;
			string m_CachedText = null;
			int m_CachedFontSize = -1;

			public bool IsInitialized()
			{
				return m_SizeFromText != null && m_RectTransform != null && m_TextRectTransform != null && m_TextGenerator != null;
			}

			public void SetSizeFromText()
			{
				Initialize();
				if (!IsInitialized())
				{
					return;
				}

				float canvasScale = Core.Util.GetCanvasScale(m_RectTransform);
				if (m_SetWidth)
				{
					TextGenerationSettings textGenerationSettings = m_SizeFromText.GetGenerationSettings(new Vector2(m_MinWidth, m_RectTransform.rect.height));
					float preferredWidth = m_TextGenerator.GetPreferredWidth(m_SizeFromText.text, textGenerationSettings);
					float canvasWidth = Mathf.Max(m_MinWidth, preferredWidth / canvasScale);
					float width = canvasWidth + m_AddedWidth;
					if (m_MaxWidth >= 0.0f && width > m_MaxWidth)
					{
						width = m_MaxWidth;
					}
					m_RectTransform.SetSizeWithCurrentAnchors(
						RectTransform.Axis.Horizontal, 
						width);
				}
				if (m_SetHeight)
				{
					TextGenerationSettings textGenerationSettings = m_SizeFromText.GetGenerationSettings(new Vector2(m_TextRectTransform.rect.width, m_MinHeight));
					float preferredHeight = m_TextGenerator.GetPreferredHeight(m_SizeFromText.text, textGenerationSettings);
					float canvasHeight = Mathf.Max(m_MinHeight, preferredHeight / canvasScale);
					float height = canvasHeight + m_AddedHeight;
					if (m_MaxHeight >= 0.0f && height > m_MaxHeight)
					{
						height = m_MaxHeight;
					}
					m_RectTransform.SetSizeWithCurrentAnchors(
						RectTransform.Axis.Vertical, 
						height);
				}

				CacheValues();
			}

			public void SetSetWidth(bool setWidth)
			{
				m_SetWidth = setWidth;
				SetSizeFromText();
			}

			public void SetSetHeight(bool setHeight)
			{
				m_SetHeight = setHeight;
				SetSizeFromText();
			}

			public void SetMinWidth(float minWidth)
			{
				m_MinWidth = minWidth;
				SetSizeFromText();
			}

			public void SetMinHeight(float minHeight)
			{
				m_MinHeight = minHeight;
				SetSizeFromText();
			}

			public void SetMaxWidth(float maxWidth)
			{
				m_MaxWidth = maxWidth;
				SetSizeFromText();
			}

			public void SetMaxHeight(float maxHeight)
			{
				m_MaxHeight = maxHeight;
				SetSizeFromText();
			}

			public void SetAddedWidth(float addedWidth)
			{
				m_AddedWidth = addedWidth;
				SetSizeFromText();
			}

			public void SetAddedHeight(float addedHeight)
			{
				m_AddedHeight = addedHeight;
				SetSizeFromText();
			}

			void Initialize()
			{
				if (m_SizeFromText == null)
				{
					m_SizeFromText = GetComponent<Text>();
				}
				if (m_SizeFromText == null)
				{
					return;
				}

				if (m_RectTransform == null)
				{
					m_RectTransform = GetComponent<RectTransform>();
				}
				if (m_TextRectTransform == null)
				{
					m_TextRectTransform = m_SizeFromText.GetComponent<RectTransform>();
				}
				if (m_TextGenerator == null)
				{
					m_TextGenerator = new TextGenerator();
				}
			}

			bool ValuesChanged()
			{
				if (m_SizeFromText == null)
				{
					return false;
				}
				return !Core.Str.Equals(m_CachedText, m_SizeFromText.text) 
					|| m_CachedFontSize != m_SizeFromText.fontSize;
			}

			void CacheValues()
			{
				if (m_SizeFromText == null)
				{
					return;
				}
				m_CachedText = m_SizeFromText.text;
				m_CachedFontSize = m_SizeFromText.fontSize;
			}

			void OnEnable()
			{
				SetSizeFromText();
			}

			void LateUpdate()
			{
				Initialize();
				if (!IsInitialized())
				{
					return;
				}

				if (m_AutoUpdateLayout && ValuesChanged())
				{
					SetSizeFromText();
				}
			}

			void OnValidate()
			{
				if (m_MaxWidth < 0.0f)
				{
					m_MaxWidth = -1.0f;
				}
				if (m_MaxHeight < 0.0f)
				{
					m_MaxHeight = -1.0f;
				}
			}
		}
	}
}