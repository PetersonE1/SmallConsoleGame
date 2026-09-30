namespace SmallConsoleGame.TileTypes;

public interface ITile
{
	public char Char { get; }
	public string Name { get; }
	public bool IsObstacle { get; }
}