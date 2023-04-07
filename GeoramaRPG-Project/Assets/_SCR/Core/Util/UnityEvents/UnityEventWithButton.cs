using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventWithButton : MonoBehaviour
{
	[SerializeField] private UnityEvent myEvent = null;

	public void InvokeEvent()
	{
		myEvent.Invoke();
	}
}
