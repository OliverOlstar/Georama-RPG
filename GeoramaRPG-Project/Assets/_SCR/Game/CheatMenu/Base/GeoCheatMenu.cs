using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeoCheatMenu : CheatMenu
{
	[SerializeField]
	private InputBridge_Debug input = null;

#if !RELEASE
	protected override void OnAwake()
	{
		input.CheatMenu.onPerformed.AddListener(ToggleMenu);
	}

	private void ToggleMenu()
	{
		if (IsOpen)
		{
			Close();
		}
		else
		{
			Open();
		}	
	}

	protected override void CalculateScreenRect(ref Rect rect)
	{
		rect.x = Screen.width * 0.01f;
		rect.y = Screen.height * 0.01f;
		rect.width = Screen.width * 0.3f;
		rect.height = Screen.height * 0.95f;
	}
#endif
}