public interface ILoadingDirector : IDirector
{
	void StartLoading(GameState gameState, object gameStateContext);
	bool IsLoadingDone();
}
