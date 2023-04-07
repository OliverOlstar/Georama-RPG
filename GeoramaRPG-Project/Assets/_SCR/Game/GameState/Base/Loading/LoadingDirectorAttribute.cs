using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class GameStateDirectorPersistAttribute : Attribute
{
	private GameState m_ValidState;

	public GameStateDirectorPersistAttribute(GameState validState)
	{
		m_ValidState = validState;
	}

	public bool IsValid(GameState gameState)
	{
		return m_ValidState.HasFlag(gameState);
	}
}

public class LoginGameStateDirectorPersist : GameStateDirectorPersistAttribute
{
	public LoginGameStateDirectorPersist() : base(GameState.MainMenu | GameState.InGame) { }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class GameStateDirectorCreateAttribute : Attribute
{
	private GameState m_ValidState;

	public GameStateDirectorCreateAttribute(GameState validState)
	{
		m_ValidState = validState;
	}

	public bool IsValid(GameState gameState)
	{
		return m_ValidState.HasFlag(gameState);
	}
}
