namespace SmallConsoleGame.TileTypes;

public interface ITile
{
	public char Char { get; }
	public bool IsObstacle { get; }
}