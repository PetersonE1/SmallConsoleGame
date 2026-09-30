namespace SmallConsoleGame.Tiles;

public class Ground : ITile
{
	public char Char => '░';
	public bool IsObstacle => false;
}