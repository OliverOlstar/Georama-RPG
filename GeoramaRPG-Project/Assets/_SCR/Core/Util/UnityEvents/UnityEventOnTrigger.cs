using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class UnityEventOnTrigger : MonoBehaviour
{
	[SerializeField] private LayerMask allowedLayers = new LayerMask();
	[SerializeField] private string[] allowedTags = new string[0];

	[Header("Events")]
	public UnityEvent onTriggerEnter;

	private void OnTriggerEnter(Collider other) 
	{
		if (IsValid(other))
		{
			onTriggerEnter.Invoke();
		}
	}

	private bool IsValid(Collider other)
	{
		// Other is trigger
		if (other.isTrigger)
		{
			return false;
		}

		// Layers
		if ((allowedLayers | (1 << other.gameObject.layer)) != 0)
		{
			return true;
		}

		// Tags
		foreach (string tag in allowedTags)
		{
			if (other.tag == tag)
			{
				return true;
			}
		}

		// Default
		return false;
	}
}
