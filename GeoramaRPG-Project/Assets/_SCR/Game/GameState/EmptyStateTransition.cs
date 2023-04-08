using System.Collections;
using UnityEngine.SceneManagement;

public class EmptyStateTransition : GameStateTransition
{
	public class Context
	{
		public string SceneName;
	}

	public EmptyStateTransition(GameState state) : base(state) { }

	protected override IEnumerator OnLoad(object stateChangeData)
	{
		Context context = stateChangeData as Context;
		SceneManager.LoadScene(context.SceneName);
		yield return null;
		SceneManager.SetActiveScene(SceneManager.GetSceneByName(context.SceneName));
	}

	protected override IEnumerator OnFinish(object stateChangeData)
	{
		yield break;
	}
}