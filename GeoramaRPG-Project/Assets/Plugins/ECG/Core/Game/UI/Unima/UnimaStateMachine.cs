
using System.Collections.Generic;
using UnityEngine;

public class UnimaStateMachine<TEnum> where TEnum : System.Enum
{
	private Dictionary<TEnum, UnimaController> m_StateDict = new Dictionary<TEnum, UnimaController>();

	private TEnum m_State = default;
	public TEnum State => m_State;
	private UnimaController m_StateController = null;

	public void AddState(TEnum state, UnimaController controller)
	{
		if (m_StateDict.ContainsKey(state))
		{
			m_StateDict[state] = controller;
			return;
		}
		m_StateDict.Add(state, controller);
	}

	// Utility function
	public void InitializeState(TEnum state, UnimaController controller, GameObject gameObject)
	{
		controller.Initialize(gameObject);
		AddState(state, controller);
	}

	public void SetState(TEnum state, IUnimaContext context = null)
	{
		if (m_StateController != null)
		{
			System.Enum e1 = m_State;
			System.Enum e2 = state;
			if (e1 == e2)
			{
				return;
			}
			m_StateController.Stop();
			m_StateController = null;
		}
		m_State = state;
		if (!m_StateDict.TryGetValue(state, out UnimaController controller))
		{
			return;
		}
		m_StateController = controller;
		m_StateController.Play(context);
	}

	public void Stop()
	{
		if (m_StateController != null)
		{
			m_StateController.Stop();
			m_StateController = null;
		}
	}
}
