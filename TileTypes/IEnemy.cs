namespace SmallConsoleGame.TileTypes;

public interface IEnemy : ITile
{
    public int Range { get; }
    public int Damage { get; }
    public int Health { get; set; }
}