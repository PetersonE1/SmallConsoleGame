using SmallConsoleGame.TileTypes;

namespace SmallConsoleGame.Tiles;

public class Player : ITile
{
    public char Char => '☺';
    public string Name => "Player";
    public bool IsObstacle => false;

    public int ArrowCount { get; set; } = 1;
    public bool IsDead { get; set; } = false;
    public int Health { get; set; } = 5;
    public void Damage(int damage)
    {
        Health -= damage;
        if (Health < 0)
            Health = 0;
        if (Health <= 0)
            IsDead = true;
    }
}