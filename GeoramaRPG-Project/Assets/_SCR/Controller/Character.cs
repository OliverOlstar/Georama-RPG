using System;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
	private Dictionary<System.Type, ICharacterBehaviour> m_Behaviours = new Dictionary<System.Type, ICharacterBehaviour>();

	[SerializeField]
	private InputBridge_KinematicController m_Input = null;

	private CharacterMoveState m_MoveState;
	private CharacterOnGround m_OnGround;
	private KinematicForceController m_Controller;

	public InputBridge_KinematicController Input => m_Input;
	public CharacterMoveState MoveState => m_MoveState;
	public CharacterOnGround OnGround => m_OnGround;
	public KinematicForceController Controller => m_Controller;

	private void Awake()
	{
		ICharacterBehaviour[] behaviours = GetComponentsInChildren<ICharacterBehaviour>();
		Array.Sort(behaviours, CharacterBehaviour.Compare);
		foreach (ICharacterBehaviour behaviour in behaviours)
		{
			m_Behaviours.Add(behaviour.GetType(), behaviour);
		}
		TryGetBehaviourRequired(out m_MoveState);
		TryGetBehaviourRequired(out m_OnGround);
		TryGetBehaviourRequired(out m_Controller);
		foreach (ICharacterBehaviour behaviour in behaviours)
		{
			behaviour.Initalize(this);
		}
	}

	public bool TryGetBehaviour<T>(out T pBehaviour) where T : ICharacterBehaviour
	{
		if (m_Behaviours.TryGetValue(typeof(T), out ICharacterBehaviour behaviour) && behaviour is T tBehaviour)
		{
			pBehaviour = tBehaviour;
			return true;
		}
		pBehaviour = default(T);
		return false;
	}

	public T GetBehaviour<T>() where T : ICharacterBehaviour
	{
		if (m_Behaviours.TryGetValue(typeof(T), out ICharacterBehaviour behaviour) && behaviour is T tBehaviour)
		{
			return tBehaviour;
		}
		Core.DebugUtil.DevException($"Could not find character behaviour {typeof(T).Name}");
		return default(T);
	}

	public bool TryGetBehaviourRequired<T>(out T pBehaviour) where T : ICharacterBehaviour
	{
		if (!TryGetBehaviour(out pBehaviour))
		{
			Core.DebugUtil.DevException($"Could not find required character behaviour {typeof(T).Name}");
			return false;
		}
		return true;
	}
}
