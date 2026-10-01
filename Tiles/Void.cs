using SmallConsoleGame.TileTypes;

namespace SmallConsoleGame.Tiles;

public class Void : ITile
{
    public char Char => ' ';
    public string Name => "Void";
    public bool IsObstacle => true;
}