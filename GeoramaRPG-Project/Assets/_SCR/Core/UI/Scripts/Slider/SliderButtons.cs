using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OliverLoescher.UI
{
	[RequireComponent(typeof(Slider))]
    public class SliderButtons : MonoBehaviour
    {
		private Slider slider = null;

		[SerializeField, Min(Util.NEARZERO)]
		private float increaseDelta = 0.1f;
		[SerializeField, Min(Util.NEARZERO)]
		private float decreaseDelta = 0.1f;

		[SerializeField]
		private Button buttonIncrease = null;
		[SerializeField]
		private Button buttonDecrease = null;

		private void Awake()
		{
			buttonIncrease.onClick.AddListener(OnIncreaseClicked);
			buttonDecrease.onClick.AddListener(OnDecreaseClicked);
			slider = GetComponent<Slider>();
		}

		private void OnDestroy()
		{
			buttonIncrease.onClick.RemoveListener(OnIncreaseClicked);
			buttonDecrease.onClick.RemoveListener(OnDecreaseClicked);
		}

		protected virtual void OnIncreaseClicked() => slider.value += increaseDelta;
		protected virtual void OnDecreaseClicked() => slider.value -= decreaseDelta;

		public void SetDelta(float pIncrease, float pDecrease)
		{
			increaseDelta = Mathf.Max(0.0f, pIncrease);
			decreaseDelta = Mathf.Max(0.0f, pDecrease);
		}
	}
}
