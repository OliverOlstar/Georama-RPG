using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverLoescher;

public interface ICharacterBehaviour
{
	float Priority { get; } // -1 comes before 1
	void Initalize(Character pCharacter);
}

public abstract class CharacterBehaviour : MonoBehaviour, ICharacterBehaviour
{
	[SerializeField]
	private Updateable updateable = new Updateable(MonoUtil.UpdateType.Default, MonoUtil.Priorities.Default);

	private Character m_Character;
	public Character Character => m_Character;
	public virtual float Priority => 0.0f;

	public void Initalize(Character pCharacter)
	{
		m_Character = pCharacter;
		OnInitalize();
		OnEnable();
	}

	private void OnDestroy()
	{
		OnDestroyed();
	}

	private void OnEnable()
	{
		if (m_Character == null)
		{
			return;
		}
		updateable.Register(Tick);
		OnEnabled();
	}

	private void OnDisable()
	{
		updateable.Deregister();
		OnDisabled();
	}

	protected virtual void OnInitalize() { }
	protected virtual void OnDestroyed() { }
	protected virtual void OnEnabled() { }
	protected virtual void OnDisabled() { }
	protected abstract void Tick(float pDeltaTime);

	public static int Compare(ICharacterBehaviour pA, ICharacterBehaviour pB)
	{
		return pA.Priority.CompareTo(pB.Priority);
	}
}
