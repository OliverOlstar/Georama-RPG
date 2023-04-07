
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Unimate/Core/Graphic Material Property")]
public class UnimateGraphicMaterialProperty : UnimateTween<UnimateGraphicMaterialProperty, UnimateGraphicMaterialProperty.Player>
{
	[SerializeField]
	private string m_PropertyName = null;

	[SerializeField]
	private AnimationCurve m_PropertyValue = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));

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
	private bool m_RestoreInitialValueOnEnd = false;
	[SerializeField]
	private bool m_Recursive = false;

	protected override string OnEditorValidate(GameObject gameObject)
	{
		if (string.IsNullOrEmpty(m_PropertyName))
		{
			return "Property Name cannot be empty";
		}
		float lastKey = m_PropertyValue.keys[m_PropertyValue.length - 1].time;
		if (!Core.Util.Approximately(lastKey, 1.0f))
		{
			return "Curve keys should be in the range 0 to 1";
		}
		List<Graphic> graphics = new List<Graphic>();
		if (m_Recursive)
		{
			gameObject.GetComponentsInChildren(graphics);
		}
		else
		{
			gameObject.GetComponents(graphics);
		}
		if (graphics.Count < 1)
		{
			return "Requires Graphic component";
		}
		foreach (Graphic graphic in graphics)
		{
			if (graphic.material == null)
			{
				return $"{graphic.name} has no material";
			}
			if (!graphic.material.HasProperty(m_PropertyName))
			{
				return $"{graphic.name} material doesn't have property {m_PropertyName}";
			}
		}
		return null;
	}

	public class Player : UnimaTweenPlayer<UnimateGraphicMaterialProperty>
	{
		private List<Graphic> m_AllGraphics = new List<Graphic>();
		private List<Graphic> m_Graphics = null;
		private float[] m_InitialValues = null;
		private int m_PropertyID = 0;

		protected override void OnInitialize()
		{
			m_PropertyID = Shader.PropertyToID(Animation.m_PropertyName);
			if (Animation.m_Recursive)
			{
				GameObject.GetComponentsInChildren(m_AllGraphics);
			}
			else
			{
				GameObject.GetComponents(m_AllGraphics);
			}
			m_InitialValues = new float[m_AllGraphics.Count];
			m_Graphics = new List<Graphic>(m_AllGraphics.Count);
		}

		protected override bool TryPlay(IUnimaContext context)
		{
			m_Graphics.Clear();
			foreach (Graphic graphic in m_AllGraphics)
			{
				if (graphic != null &&
					graphic.material != null &&
					graphic.material.HasProperty(m_PropertyID))
				{
					GraphicMaterialInstance.InstatiateCopy(graphic);
					m_Graphics.Add(graphic);
					continue;
				}
			}
			return m_Graphics.Count > 0;
		}

		protected override void OnStartTween()
		{
			for (int i = 0; i < m_Graphics.Count; i++)
			{
				m_InitialValues[i] = m_Graphics[i].materialForRendering.GetFloat(m_PropertyID);
			}
		}

		protected override void OnUpdateTween(float normalizedTime, float loopingTime)
		{
			float value = Animation.m_PropertyValue.Evaluate(normalizedTime);
			for (int i = 0; i < m_Graphics.Count; i++)
			{
				m_Graphics[i].materialForRendering.SetFloat(m_PropertyID, value);
			}
		}

		protected override void OnEndTween(bool interrupted)
		{
			if (Animation.m_RestoreInitialValueOnEnd)
			{
				for (int i = 0; i < m_Graphics.Count; i++)
				{
					m_Graphics[i].materialForRendering.SetFloat(m_PropertyID, m_InitialValues[i]);
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
