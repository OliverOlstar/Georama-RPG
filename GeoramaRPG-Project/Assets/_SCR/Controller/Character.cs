using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OliverLoescher;

public class Character : MonoBehaviour
{
	private Dictionary<System.Type, ICharacterBehaviour> m_Behaviours = new Dictionary<System.Type, ICharacterBehaviour>();

	[SerializeField]
	private KinematicForceController m_Controller = null;
	[SerializeField]
	private InputBridge_KinematicController m_Input = null;
	private CharacterMoveState m_MoveState;

	public Vector3 Forward => m_Controller.Forward();
	public Vector3 Right => m_Controller.Right();
	public Vector3 Up => m_Controller.Capsule.Up;

	public KinematicForceController Controller => m_Controller;
	public Vector3 Velocity => m_Controller.Velocity;
	public bool IsGrounded => m_Controller.IsGrounded;

	public InputBridge_KinematicController Input => m_Input;
	public CharacterMoveState MoveState => m_MoveState;

	private void Awake()
	{
		ICharacterBehaviour[] behaviours = GetComponentsInChildren<ICharacterBehaviour>();
		Array.Sort(behaviours, CharacterBehaviour.Compare);
		foreach (ICharacterBehaviour behaviour in behaviours)
		{
			m_Behaviours.Add(behaviour.GetType(), behaviour);
		}
		TryGetBehaviour(out m_MoveState);
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
		return  default(T);
	}
}
