using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace OliverLoescher.UI
{
	[RequireComponent(typeof(Button))]
	public class ButtonText : MonoBehaviour
	{
		[SerializeField] private TMP_Text text = null;
		private Button button;

		[SerializeField] private string[] strings = new string[1];
		private int index = 0;

		private void Awake()
		{
			button = GetComponent<Button>();
			button.onClick.AddListener(OnClick);
		}

		public void Reset()
		{
			if (text != null && strings.Length > 0)
			{
				text.text = strings[strings.Length - 1];
			}
		}

		private void OnDestroy()
		{
			button.onClick.RemoveListener(OnClick);
		}

		public void OnClick()
		{
			index++;
			if (index == strings.Length)
				index = 0;

			text.text = strings[index];
		}
	}
}