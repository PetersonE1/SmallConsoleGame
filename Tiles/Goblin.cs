using SmallConsoleGame.TileTypes;

namespace SmallConsoleGame.Tiles;

public class Goblin : IEnemy
{
    public char Char => 'ᴥ';
    public string Name => "Goblin";
    public bool IsObstacle => false;
    public int Range => 1;
    public int Damage => 1;
    public int Health { get; set; } = 1;
}