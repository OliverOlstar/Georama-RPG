using UnityEngine;

public abstract class Unimate<TAnimation, TPlayer> : UnimateBase
	where TAnimation : UnimateBase
	where TPlayer : UnimaPlayer<TAnimation>, new()
{
	public override IUnimaPlayer CreatePlayer(UnimaTiming timing, GameObject gameObject)
	{
		TPlayer player = new TPlayer();
		player.Initialize(this, timing, gameObject);
		return player;
	}
}
