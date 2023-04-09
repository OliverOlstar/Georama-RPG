using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatMenuGameStatePage : CheatMenuPage
{
	public override CheatMenuGroup Group => GeoCheatMenuGroups.GAME_STATE;
	public override bool IsAvailable() => base.IsAvailable() || GameStateManager.GameState == GameState.Testing;

	public override void DrawGUI()
	{
		GUILayout.Label("GameStates");
		foreach (GameState gameState in System.Enum.GetValues(typeof(GameState)))
		{
			if (GUILayout.Button(gameState.ToString()))
			{
				GameStateManager.RequestState(gameState);
				Close();
			}
		}
		GUILayout.Space(8.0f);
		GUILayout.Label("Reboot To");
		foreach (GeoDebugOptions.BootTo bootTo in System.Enum.GetValues(typeof(GeoDebugOptions.BootTo)))
		{
			if (GUILayout.Button(bootTo.ToString()))
			{
				GeoDebugOptions.BootToState.SetString(bootTo);
				GameStateManager.RequestState(GameState.BootFlow);
				Close();
			}
		}
	}
}
