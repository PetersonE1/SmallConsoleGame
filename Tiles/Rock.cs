using SmallConsoleGame.TileTypes;

namespace SmallConsoleGame.Tiles;

public class Rock : ITile
{
	public char Char => '▲';
	public string Name => "Rock";
	public bool IsObstacle => true;
}