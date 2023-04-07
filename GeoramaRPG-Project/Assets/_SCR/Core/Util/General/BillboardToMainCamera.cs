using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardToMainCamera : MonoBehaviour
{
	private new Transform camera = null;

	void Start()
	{
		camera = Camera.main.transform;
	}

	void LateUpdate()
	{
		Vector3 dir = transform.position - camera.transform.position;
		dir.y = 0;

		if (dir != Vector3.zero)
			transform.rotation = Quaternion.LookRotation(dir);
	}
}
