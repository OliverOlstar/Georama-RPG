using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UnityEventWithKeyDown : UnityEventWithButton
{
	[SerializeField] private KeyCode keyDown = KeyCode.None;
	[SerializeField] private bool executeInEditMode = false;

	private void Update() 
	{
		if (Input.GetKeyDown(keyDown) && (executeInEditMode || Application.isPlaying))
		{
			InvokeEvent();
		}
	}
}
