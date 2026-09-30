using SmallConsoleGame.TileTypes;

namespace SmallConsoleGame.Tiles;

public class Goblin : IEnemy
{
    public char Char => 'ᴥ';
    public bool IsObstacle => false;
    public int Range => 1;
    public int Damage => 1;
    public int Health => 1;
}