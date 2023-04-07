using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
	[SerializeField, Min(0)] private float seconds = 1.0f;

	void Start()
	{
		Destroy(gameObject, seconds);
	}
}
