
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Unimate/Core/Greyscale")]
public class UnimateGreyscale : UnimateTween<UnimateGreyscale, UnimateGreyscale.Player>
{
	[SerializeField]
	private AnimationCurve m_GreyScaleAmount = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(1.0f, 1.0f));
	[SerializeField]
	private float m_Duration = 1.0f;
	public override float Duration => m_Duration;
	[SerializeField]
	private bool m_Loop = false;
	public override bool Loop => m_Loop;
	[SerializeField]
	private bool m_UpdateBeforeStart = false;
	public override bool UpdateBeforeStart => m_UpdateBeforeStart;
	[SerializeField]
	private bool m_Recursive = false;

	protected override string OnEditorValidate(GameObject gameObject)
	{
		float lastKey = m_GreyScaleAmount.keys[m_GreyScaleAmount.length - 1].time;
		if (!Core.Util.Approximately(lastKey, 1.0f))
		{
			return "Curve keys should be in the range 0 to 1";
		}
		bool valid = m_Recursive ?
			gameObject.GetComponentsInChildren<Image>().Length > 0 :
			gameObject.GetComponent<Image>() != null;
		return valid ? null : $"Requires {typeof(Image).Name} component";
	}

	public class Player : UnimaTweenPlayer<UnimateGreyscale>
	{
		private List<Image> m_Components = new List<Image>();

		protected override void OnInitialize()
		{
			if (Animation.m_Recursive)
			{
				GameObject.GetComponentsInChildren(m_Components);
			}
			else
			{
				GameObject.GetComponents(m_Components);
			}
		}

		protected override bool TryPlay(IUnimaContext context)
		{
			return m_Components.Count > 0;
		}

		protected override void OnUpdateTween(float normalizedTime, float loopingTime)
		{
			float amount = Animation.m_GreyScaleAmount.Evaluate(normalizedTime);
			for (int i = 0; i < m_Components.Count; i++)
			{
				Core.UIUtil.SetGreyscale(m_Components[i], amount);
			}
		}

		protected override void OnEndTween(bool interrupted)
		{
			for (int i = 0; i < m_Components.Count; i++)
			{
				Core.UIUtil.SetGreyscale(m_Components[i], false);
			}
		}
	}
}
