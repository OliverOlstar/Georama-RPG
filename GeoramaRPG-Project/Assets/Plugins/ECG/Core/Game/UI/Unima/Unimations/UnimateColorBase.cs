
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UnimateColorBase<TComponent> : 
	UnimateTween<UnimateColorBase<TComponent>, UnimateColorBase<TComponent>.Player<TComponent>>
		where TComponent : MaskableGraphic
{
	[SerializeField]
	private Gradient m_ColorOverTime = null;
	[SerializeField]
	private AnimationCurve m_FromSourceToColor = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));
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
	private bool m_RestoreSourceOnEnd = false;
	[SerializeField]
	private bool m_Recursive = false;

	protected override string OnEditorValidate(GameObject gameObject)
	{
		float lastKey = m_FromSourceToColor.keys[m_FromSourceToColor.length - 1].time;
		if (!Core.Util.Approximately(lastKey, 1.0f))
		{
			return "Curve keys should be in the range 0 to 1";
		}
		bool valid = m_Recursive ?
			gameObject.GetComponentsInChildren<TComponent>().Length > 0 :
			gameObject.GetComponent<TComponent>() != null;
		return valid ? null : $"Requires {typeof(TComponent).Name} component";
	}

	public class Player<TPlayerComponent> : UnimaTweenPlayer<UnimateColorBase<TPlayerComponent>> 
		where TPlayerComponent : MaskableGraphic
	{
		private List<TPlayerComponent> m_Components = new List<TPlayerComponent>();
		private Color[] m_SourceColors = null;

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
			m_SourceColors = new Color[m_Components.Count];
		}

		protected override bool TryPlay(IUnimaContext context)
		{
			return m_Components.Count > 0;
		}

		protected override void OnStartTween()
		{
			for (int i = 0; i < m_Components.Count; i++)
			{
				m_SourceColors[i] = m_Components[i].color;
			}
		}

		protected override void OnUpdateTween(float normalizedTime, float loopingTime)
		{
			float src = Animation.m_FromSourceToColor.Evaluate(normalizedTime);
			Color color = Animation.m_ColorOverTime.Evaluate(normalizedTime);
			for (int i = 0; i < m_Components.Count; i++)
			{
				m_Components[i].color = Color.Lerp(m_SourceColors[i], color, src);
			}
		}

		protected override void OnEndTween(bool interrupted)
		{
			if (Animation.m_RestoreSourceOnEnd)
			{
				for (int i = 0; i < m_Components.Count; i++)
				{
					m_Components[i].color = m_SourceColors[i];
				}
			}
			else if (interrupted)
			{
				// Sample end of animation so color doesn't get stuck at some random mid point
				OnUpdateTween(1.0f, Animation.Duration);
			}
		}
	}
}
