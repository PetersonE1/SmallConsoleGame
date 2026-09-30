using SmallConsoleGame.TileTypes;

namespace SmallConsoleGame.Tiles;

public class Rock : ITile
{
	public char Char => '▲';
	public bool IsObstacle => true;
}