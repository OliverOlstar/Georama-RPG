using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class ModelAnimEvents : CharacterBehaviour
{
	protected override void Tick(float pDeltaTime) { }

	[FoldoutGroup("Dodge"), SerializeField]
	private UnityEvent m_DodgeEndEvent = new UnityEvent();
	[FoldoutGroup("Dodge"), SerializeField]
	private UnityEvent<float> m_DodgeSpeedCurveEvent = new UnityEvent<float>();

	public UnityEvent DodgeEndEvent => m_DodgeEndEvent;
	public UnityEvent<float> DodgeSpeedCurveEvent => m_DodgeSpeedCurveEvent;

	private void DodgeEnd() => m_DodgeEndEvent.Invoke();
	private void DodgeSpeedCurve(float pValue) => m_DodgeSpeedCurveEvent.Invoke(pValue);

	private void FootR() { } // SUPPRESS ANIM EVENTS
	private void FootL() { }
	private void Hit() { }
	private void Land() { }
}
