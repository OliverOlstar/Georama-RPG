
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Unimate/Core/Graphic Material Color Array")]
public class UnimateGraphicMaterialColorArray : UnimateTween<UnimateGraphicMaterialColorArray, UnimateGraphicMaterialColorArray.Player>
{
	[SerializeField]
	private string[] m_PropertyName = { };

	[SerializeField]
	private Color[] m_PropertyColor = { };

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
        for(int i = 0; i < m_PropertyName.Length; i++)
        {
		    if (string.IsNullOrEmpty(m_PropertyName[i]))
		    {
			    return "Property Name cannot be empty";
		    }
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
			for (int i = 0; i < m_PropertyName.Length; i++)
            {
				if (!graphic.material.HasProperty(m_PropertyName[i]))
				{
					return $"{graphic.name} material doesn't have property {m_PropertyName[i]}";
				}
            }
		}
		return null;
	}

	public class Player : UnimaTweenPlayer<UnimateGraphicMaterialColorArray>
	{
		private List<Graphic> m_AllGraphics = new List<Graphic>();
		private List<Graphic> m_Graphics = null;
		private Color[,] m_InitialColors = null;
		private int[] m_PropertyID = null;

		protected override void OnInitialize()
		{
             m_PropertyID = new int[Animation.m_PropertyName.Length];
            for (int i = 0; i < Animation.m_PropertyName.Length; i++)
            {
                m_PropertyID[i] = Shader.PropertyToID(Animation.m_PropertyName[i]);

            }
			if (Animation.m_Recursive)
			{
				GameObject.GetComponentsInChildren(m_AllGraphics);
			}
			else
			{
				GameObject.GetComponents(m_AllGraphics);
			}
			m_InitialColors = new Color[m_AllGraphics.Count, m_PropertyID.Length];
			m_Graphics = new List<Graphic>(m_AllGraphics.Count);
		}

		protected override bool TryPlay(IUnimaContext context)
		{
			m_Graphics.Clear();
			foreach (Graphic graphic in m_AllGraphics)
			{
				if (graphic != null &&
					graphic.material != null &&
					graphic.material.HasProperty(m_PropertyID[0]))
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
				Graphic graphic = m_Graphics[i];
                for(int j = 0; j < m_PropertyID.Length; j++)
                {
				    m_InitialColors[i,j] = graphic.materialForRendering.GetColor(m_PropertyID[j]);
                }
			}
		}

		protected override void OnUpdateTween(float normalizedTime, float loopingTime)
		{
			for (int i = 0; i < m_Graphics.Count; i++)
			{
				Graphic graphic = m_Graphics[i];
				for (int j = 0; j < m_PropertyID.Length; j++)
                {
                    Color value = Animation.m_PropertyColor[j];
					graphic.materialForRendering.SetColor(m_PropertyID[j], value);
                }
			}
		}

		protected override void OnEndTween(bool interrupted)
		{
			if (Animation.m_RestoreInitialValueOnEnd)
			{
				for (int i = 0; i < m_Graphics.Count; i++)
				{
					Graphic graphic = m_Graphics[i];
					for (int j = 0; j < m_PropertyID.Length; j++)
                    {
						graphic.materialForRendering.SetColor(m_PropertyID[j], m_InitialColors[i,j]);
                    }
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
