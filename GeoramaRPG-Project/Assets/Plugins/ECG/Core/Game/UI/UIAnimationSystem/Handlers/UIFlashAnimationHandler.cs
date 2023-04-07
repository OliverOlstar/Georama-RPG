using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class UIFlashAnimationHandler : UICustomAnimationHandler<UIColorAnimation, Image>
	{
		[SerializeField]
		Transform m_FlashParent = null;

		GameObject m_Flash = null;
		Image m_FlashImage = null;

		public void SetFlashParent(Transform flashParent)
		{
			KillAnimation();
			m_FlashParent = flashParent;
		}

		protected override void InitializeCustomAnimation()
		{
			if (m_Target == null)
			{
				return;
			}

			// instantiate flash mask and setup its image to match the target image.
			m_Flash = new GameObject(gameObject.name + "_Flash", typeof(RectTransform), typeof(Image), typeof(Mask));
			m_Flash.GetComponent<Image>().sprite = m_Target.sprite;
			m_Flash.GetComponent<Image>().type = m_Target.type;
			m_Flash.gameObject.GetComponent<Mask>().showMaskGraphic = false;

			// position flash directly overtop of target and size it to cover target.
			RectTransform flashRectTransform = m_Flash.GetComponent<RectTransform>();
			RectTransform targetRectTransform = m_Target.GetComponent<RectTransform>();
			flashRectTransform.SetParent(m_FlashParent == null ? transform : m_FlashParent, false);
			flashRectTransform.pivot = targetRectTransform.pivot;
			flashRectTransform.position = targetRectTransform.position;
			flashRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetRectTransform.rect.width);
			flashRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetRectTransform.rect.height);

			// instantiate the flash image and set its sprite.
			m_FlashImage = new GameObject("FlashImage", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
			m_FlashImage.sprite = null;

			// set the flash image as a child of the flash mask and set it to fill its parent.
			RectTransform flashImageRectTransform = m_FlashImage.GetComponent<RectTransform>();
			flashImageRectTransform.SetParent(m_Flash.transform, false);
			flashImageRectTransform.anchorMin = Vector2.zero;
			flashImageRectTransform.anchorMax = Vector2.one;
			flashImageRectTransform.offsetMin = Vector2.zero;
			flashImageRectTransform.offsetMax = Vector2.zero;
		}

		protected override void AnimateValues(float delta)
		{
			Color gradientColor = m_Animation.m_ColorGradient.Evaluate(delta);
			m_FlashImage.color = Color.Lerp(m_Target.color, gradientColor, m_Animation.m_SourceColorCurve.Evaluate(delta));
		}

		protected override void OnAnimationEnd(bool interrupted)
		{
			m_FlashImage = null;
			Destroy(m_Flash);
		}
	}
}
