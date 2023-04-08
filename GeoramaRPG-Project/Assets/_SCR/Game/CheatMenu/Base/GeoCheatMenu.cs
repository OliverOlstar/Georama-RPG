using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OkoCheatMenu : CheatMenu
{
#if !RELEASE
	private bool CanUpdate()
	{
		if (IsOpen)
		{
			return false;
		}
		if ((Application.isEditor && !Input.GetMouseButton(1)) ||
			(Application.isMobilePlatform && Input.touchCount < 2))
		{
			return false;
		}
		if (Input.mousePosition.y < 0.75f * Screen.height)
		{
			return false;
		}
		return true;
	}

	private void Update()
	{
		if (!CanUpdate())
		{
			return;
		}
		// Open();
	}
#endif
}