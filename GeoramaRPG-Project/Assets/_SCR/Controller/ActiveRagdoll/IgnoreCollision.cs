using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollision : MonoBehaviour
{
	[SerializeField]
	private Collider myCollider = null;

	[SerializeField]
	private Collider[] otherColliders = new Collider[0];

	private void Start()
	{
		foreach (Collider c in otherColliders)
		{
			Physics.IgnoreCollision(c, myCollider);
		}
		DestroyImmediate(this);
	}

	private void Reset()
	{
		TryGetComponent(out myCollider);
	}
}
