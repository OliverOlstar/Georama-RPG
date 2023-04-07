using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSelf : MonoBehaviour
{
	[SerializeField] private Vector3 rotateSpeed = new Vector3(0, 1, 0);

	private void LateUpdate()
	{
		transform.Rotate(rotateSpeed * Time.deltaTime, Space.Self);
	}
}
