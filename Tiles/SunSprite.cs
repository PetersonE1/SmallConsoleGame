using SmallConsoleGame.TileTypes;

namespace SmallConsoleGame.Tiles;

public class SunSprite : IEnemy
{
    public char Char => '☼';
    public bool IsObstacle => false;
    public int Range => 5;
    public int Damage => 2;
    public int Health => 1;
}