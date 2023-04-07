using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace OliverLoescher.UI
{
	[RequireComponent(typeof(Button))]
	public class ButtonToggleEvent : MonoBehaviour
	{
		private Button button;
		public UnityEventsUtil.BoolEvent onToggle;
		public UnityEvent onDotoggle;
		public UnityEvent onUntoggle;
		private bool toggle = false;

		private void Awake()
		{
			button = GetComponent<Button>();
			button.onClick.AddListener(OnClick);
		}

		private void OnDestroy()
		{
			button.onClick.RemoveListener(OnClick);
		}

		public void OnClick()
		{
			toggle = !toggle;
			onToggle.Invoke(toggle);
			if (toggle)
			{
				onDotoggle.Invoke();
			}
			else
			{
				onUntoggle.Invoke();
			}
		}
	}
}
