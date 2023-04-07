
using System.Collections.Generic;
using UnityEngine;

public class UnimaBehaviour : MonoBehaviour, IUnimaControllerSource
{
	public enum TriggerType
	{
		None = 0,
		OnEnable,
	}

	[SerializeField]
	private TriggerType m_Trigger = TriggerType.None;
	public TriggerType Trigger => m_Trigger;

	[SerializeField]
	private UnimaController m_Controller = new UnimaController();
	public UnimaController Controller => m_Controller;

	public bool CanHaveTrigger() { return m_Controller.CanPlay(); }

	private void Awake()
	{
		m_Controller.Initialize(gameObject);
	}

	public void Init()
    {
		m_Controller.Initialize(gameObject);
	}

	private void OnEnable()
	{
		if (m_Trigger == TriggerType.OnEnable)
		{
			m_Controller.Play();
		}
		if (m_Controller.Mode == UnimaControllerMode.Beta)
		{
			if (UnimaUtil.TryGetAlpha(gameObject, m_Controller.AlphaID, out UnimaController parent, out _))
			{
				parent._InternalAddBeta(m_Controller);
			}
		}
	}

	private void OnDisable()
	{
		m_Controller.Stop();
	}

	private void OnValidate()
	{
		if (!CanHaveTrigger())
		{
			m_Trigger = TriggerType.None;
		}
		m_Controller.OnValidate();
	}

	public void Play()
	{
		m_Controller.Play();
	}
	
	public void Stop()
	{
		m_Controller.Stop();
	}

	void IUnimaControllerSource.AddControllers(List<UnimaController> controllers)
	{
		controllers.Add(m_Controller);
	}
}

