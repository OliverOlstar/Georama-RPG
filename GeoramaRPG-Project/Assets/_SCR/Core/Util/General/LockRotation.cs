using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour
{
	public bool LockXRotation = false;
	public bool LockYRotation = false;
	public bool LockZRotation = false;
	Vector3 initialRotation;

	void Start() 
	{
		initialRotation = transform.eulerAngles;
	}

	void lockRotation() 
	{
		if (LockXRotation || LockYRotation || LockZRotation) 
		{
			Vector3 currentRotation = transform.eulerAngles;
			transform.eulerAngles = new Vector3(LockXRotation ? initialRotation.x : currentRotation.x, LockYRotation ? initialRotation.y : currentRotation.y, LockZRotation ? initialRotation.z : currentRotation.z);
		}
	}

	private void FixedUpdate() 
	{
		lockRotation();
	}

	private void Update() 
	{
		lockRotation();
	}
}