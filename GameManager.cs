using System.Collections.Immutable;
using SmallConsoleGame.Tiles;
using SmallConsoleGame.TileTypes;

namespace SmallConsoleGame;

public class GameManager
{
    private readonly ImmutableArray<ITile> _tileTypes = [new Ground(), new Rock(), new Player(), new Goblin(), new SunSprite()];
    private Map? _map = null;

    public void CreateNewMap(int width, int height, int rockCount, int goblinCount, int spriteCount)
    {
        _map = new Map(width, height, rockCount, goblinCount, spriteCount, 10);
    }

    public void PrintMap() => _map?.PrintMap();

    public bool PlacePlayer(int x, int y) => _map?.PlacePlayer(x, y) ?? false;

    public void PrintLegend()
    {
        Console.WriteLine("LEGEND:");
        foreach (ITile tileType in _tileTypes)
            Console.WriteLine($"{tileType.Char} -> {tileType.Name}");
    }
}