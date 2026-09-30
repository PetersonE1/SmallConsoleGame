using SmallConsoleGame.TileTypes;

namespace SmallConsoleGame.Tiles;

public class Ground : ITile
{
	public char Char => '░';
	public string Name => "Ground";
	public bool IsObstacle => false;
}