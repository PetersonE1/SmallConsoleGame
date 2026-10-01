using System.Drawing;
using SmallConsoleGame.TileTypes;

namespace SmallConsoleGame;

public class ActiveEntity<T>(T tile, int referenceId, int x, int y)
    where T : ITile
{
    public T Tile { get; } = tile;
    public Point Position { get; set; } = new(x, y);
    public int ReferenceId { get; } = referenceId;
}