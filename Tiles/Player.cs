using SmallConsoleGame.TileTypes;

namespace SmallConsoleGame.Tiles;

public class Player : ITile
{
    public char Char => '☺';
    public bool IsObstacle => false;
}