using System;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
	private Dictionary<System.Type, ICharacterBehaviour> m_Behaviours = new Dictionary<System.Type, ICharacterBehaviour>();

	[SerializeField]
	private InputBridge_KinematicController m_Input = null;
	[SerializeField]
	private Transform m_ModelForward = null;
	[SerializeField]
	private Transform m_CameraForward = null;

	private CharacterMoveState m_MoveState;
	private CharacterOnGround m_OnGround;
	private CharacterMoveManager m_Controller;
	private ModelAnimController m_AnimController;
	private ModelAnimEvents m_AnimEvents;
	private ModelMovementController m_ModelMovement;

	public InputBridge_KinematicController Input => m_Input;
	public CharacterMoveState MoveState => m_MoveState;
	public CharacterOnGround OnGround => m_OnGround;
	public CharacterMoveManager Controller => m_Controller;
	public ModelAnimController AnimController => m_AnimController;
	public ModelAnimEvents AnimEvents => m_AnimEvents;
	public ModelMovementController ModelMovement => m_ModelMovement;

	public Transform Model => m_ModelForward;
	public Transform Camera => m_CameraForward;

	private void Awake()
	{
		ICharacterBehaviour[] behaviours = GetComponentsInChildren<ICharacterBehaviour>();
		Array.Sort(behaviours, CharacterBehaviour.Compare);
		foreach (ICharacterBehaviour behaviour in behaviours)
		{
			m_Behaviours.Add(behaviour.GetType(), behaviour);
		}
		GetBehaviour(out m_MoveState);
		GetBehaviour(out m_OnGround);
		GetBehaviour(out m_Controller);
		GetBehaviour(out m_AnimController);
		GetBehaviour(out m_AnimEvents);
		GetBehaviour(out m_ModelMovement);
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

	public void GetBehaviour<T>(out T pBehaviour) where T : ICharacterBehaviour
	{
		if (!TryGetBehaviour(out pBehaviour))
		{
			Core.DebugUtil.DevException($"Could not find required character behaviour {typeof(T).Name}");
		}
	}
}
