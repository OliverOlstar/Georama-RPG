using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace OliverLoescher.UI
{
	[RequireComponent(typeof(Slider))]
	public class SliderText : MonoBehaviour
	{
		[SerializeField] 
		private TMP_Text text = null;
		private Slider slider = null;

		[Header("Text")]
		// https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-3.0/dwhawy9k(v=vs.85)?redirectedfrom=MSDN
		[Tooltip("Xn - n number of decimal place \nX format - F (fixed-point), D (decimal), C (currency) P (percent), etc")]
		public string textFormat = "F2";
		public string preText = "";
		public string postText = "";
		public bool addPlusIfPositive = false;

		void Awake()
		{
			slider = GetComponent<Slider>();
			slider.onValueChanged.AddListener(OnValueChanged);
			OnValueChanged(slider.value);
		}

		private void OnDestroy()
		{
			slider.onValueChanged.RemoveListener(OnValueChanged);
		}

		private void OnValidate()
		{
			if (slider == null && !TryGetComponent(out slider))
			{
				return;
			}
			OnValueChanged(slider.value);
		}

		public void OnValueChanged(float pValue)
		{
			string plus = string.Empty;
			if (addPlusIfPositive && pValue > 0.0f)
			{
				plus = "+";
			}
			text.text = $"{preText}{plus}{pValue.ToString(textFormat)}{postText}";
		}
	}
}