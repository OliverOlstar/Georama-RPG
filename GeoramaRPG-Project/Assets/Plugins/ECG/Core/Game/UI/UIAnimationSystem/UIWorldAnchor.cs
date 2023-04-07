using UnityEngine;

namespace UI
{
	[RequireComponent(typeof(RectTransform))]
	public class UIWorldAnchor : MonoBehaviour
	{
		public Transform m_Parent = null;
		public Vector3 m_LocalPosition = Vector3.zero;
		public Vector3 m_WorldOffset = Vector3.zero;
		public Vector2 m_ScreenOffset = Vector3.zero;

		Canvas m_Canvas = null;
		RectTransform m_RectTransform = null;

		public void Anchor(Vector3 worldPos)
		{
			m_LocalPosition = Vector3.zero;
			m_WorldOffset = worldPos;
			LateUpdate();
		}

		public void Anchor(Transform parent, Vector3 offset)
		{
			m_Parent = parent;
			m_LocalPosition = offset;
			LateUpdate();
		}

		void LateUpdate()
		{
			Camera camera = Camera.main;
			if (camera == null)
			{
				return;
			}
			if (m_Canvas == null)
			{
				m_Canvas = Core.Util.GetComponentInHighestParent<Canvas>(transform);
			}
			if (m_RectTransform == null)
			{
				m_RectTransform = GetComponent<RectTransform>();
			}

			Vector3 worldPosition = m_Parent == null ? m_LocalPosition : m_Parent.TransformPoint(m_LocalPosition);
			Vector2 screenPosition = camera.WorldToScreenPoint(worldPosition + m_WorldOffset);
			Vector2 canvasPosition = screenPosition / m_Canvas.scaleFactor;
			m_RectTransform.anchorMin = Vector2.zero;
			m_RectTransform.anchorMax = Vector2.zero;
			m_RectTransform.anchoredPosition = canvasPosition + m_ScreenOffset;
		}
	}
}
