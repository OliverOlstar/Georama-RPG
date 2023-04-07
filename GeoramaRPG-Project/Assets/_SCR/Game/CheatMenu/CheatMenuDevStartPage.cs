using Core;
using UnityEngine;

public class CheatMenuDevStartPage : CheatMenuPage
{
	private string[] m_PlayfabIDs = new[] {"6F007", "778C6", "31349", "60E2D"};
	private string[] m_Enviros = new[] {"Dev", "QA", "Stable", "Prod"};
	
	public override string Name => "Dev Start";
	public override int Priority => CheatMenuPriority.DevStart;

	public override bool IsAvailable() => GameStateManager.IsPreGame();

	//[SerializeField]
	//private int m_Index = -1;
	
	public override void DrawGUI()
	{
		/*if (m_Index < 0 || m_Index > m_PlayfabIDs.Length)
		{
			for(int i = 0; i < m_PlayfabIDs.Length; ++i)
			{
				if (m_PlayfabIDs[i] == PlayFabConfiguration.Settings.TitleId)
				{
					m_Index = i;
				}
			}
		}

		if (m_Index < 0 || m_Index > m_Enviros.Length)
		{
			GUILayout.Label("Unable to find default playfab titleID please fix the list inside InGameCheatMenuDevStartPage or the id in the titan hub");
			return;
		}*/
		
		base.DrawGUI();
		
		GUILayout.BeginVertical();

		GUILayout.EndVertical();

		/*GUILayout.Space(15);
		
		GUILayout.BeginVertical();
		GUILayout.Label($"Current Selected Enviro: {m_Enviros[m_Index]}");
		GUILayout.EndVertical();
		
		for(int i = 0; i < m_Enviros.Length; ++i)
		{
			GUILayout.BeginVertical();

			if (GUILayout.Button(m_Enviros[i]))
			{
				PlayFabConfiguration.Settings.TitleId = m_PlayfabIDs[i];
				m_Index = i;
			}

			GUILayout.EndVertical();
		}*/
	}
}