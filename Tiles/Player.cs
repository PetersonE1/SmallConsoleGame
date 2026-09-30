using SmallConsoleGame.TileTypes;

namespace SmallConsoleGame.Tiles;

public class Player : ITile
{
    public char Char => '☺';
    public string Name => "Player";
    public bool IsObstacle => false;
}