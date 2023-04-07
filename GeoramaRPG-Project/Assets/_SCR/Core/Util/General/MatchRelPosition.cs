using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchRelPosition : MonoBehaviour
{
	public Transform target = null;

	void LateUpdate()
	{
		if (target != null)
			transform.localPosition = target.localPosition;
	}
}
