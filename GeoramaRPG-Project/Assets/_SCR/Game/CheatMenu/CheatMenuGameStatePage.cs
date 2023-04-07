using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatMenuGameStatePage : CheatMenuPage
{
	public override string Name => "Game State";

	public override void DrawGUI()
	{
		foreach (GameState gameState in Enum.GetValues(typeof(GameState)))
		{
			if (GUILayout.Button(gameState.ToString()))
			{
				GameStateManager.RequestState(gameState);
				Close();
			}
		}
		if (GUILayout.Button("Endless Loading Screen"))
		{
			//TODO implement an alternative 
			//SplashScreenPlayer.ShowDebugRotation(LoadScreenType.Default);
			throw new NotImplementedException();
		}
	}
}
