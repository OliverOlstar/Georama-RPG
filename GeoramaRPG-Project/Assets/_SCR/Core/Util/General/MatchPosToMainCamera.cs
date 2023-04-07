using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchPosToMainCamera : MonoBehaviour
{
	private Transform mainCamera;
	[SerializeField] private Vector3 offset = new Vector3();

	void Start()
	{
		mainCamera = Camera.main.transform;
	}

	void FixedUpdate()
	{
		transform.position = mainCamera.position + offset;
	}
}
