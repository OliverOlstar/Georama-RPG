using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OliverLoescher.UI
{
    public class ButtonFloatEvent : MonoBehaviour
	{
		[SerializeField]
		private float value = 0.0f;
		[SerializeField, Min(Util.NEARZERO)]
		private float increaseDelta = 0.1f;
		[SerializeField, Min(Util.NEARZERO)]
		private float decreaseDelta = 0.1f;
		[SerializeField]
		private Vector2 clamp = new Vector2(0.0f, 1.0f);

		[Space, SerializeField]
		private Button buttonIncrease = null;
		[SerializeField]
		private Button buttonDecrease = null;

		[Space]
		public UnityEventsUtil.FloatEvent onChange;

		public float Value => value;

		protected virtual void Awake()
		{
			buttonIncrease.onClick.AddListener(OnIncreaseClicked);
			buttonDecrease.onClick.AddListener(OnDecreaseClicked);
		}

		protected virtual void OnDestroy()
		{
			buttonIncrease.onClick.RemoveListener(OnIncreaseClicked);
			buttonDecrease.onClick.RemoveListener(OnDecreaseClicked);
		}

		protected virtual void OnValidate()
		{
			value = Util.Clamp(value, clamp);

		}

		protected virtual void OnIncreaseClicked() => ModifyValue(increaseDelta);
		protected virtual void OnDecreaseClicked() => ModifyValue(-decreaseDelta);
		protected virtual void ModifyValue(float pDelta)
		{
			value = Util.Clamp(value + pDelta, clamp);
			onChange.Invoke(value);
		}
	}
}
