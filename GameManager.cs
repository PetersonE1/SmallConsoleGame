using System.Collections.Immutable;
using System.Drawing;
using System.Text;
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

    public string Step()
    {
        if (_map is null) return "No active map";
        if (_map.Player is null) return "No active player";
        StringBuilder sb = new();
        foreach (ActiveEntity<Goblin> goblin in _map.Goblins.Where(goblin =>
                     _map.CanAttackOtherwiseStepTowardPlayer(goblin)))
        {
            _map.Player.Tile.Damage(goblin.Tile.Damage);
            sb.AppendLine($"{goblin.Tile.Name} dealt {goblin.Tile.Damage} damage!");
        }

        foreach (ActiveEntity<SunSprite> sprite in _map.Sprites.Where(sprite =>
                     _map.CanAttackOtherwiseStepTowardPlayer(sprite)))
        {
            _map.Player.Tile.Damage(sprite.Tile.Damage);
            sb.AppendLine($"{sprite.Tile.Name} dealt {sprite.Tile.Damage} damage!");
        }
        
        return sb.ToString();
    }

    /// <summary>
    /// Takes a player action
    /// </summary>
    /// <param name="attack">attack if true, move if false</param>
    /// <param name="dir">0,1,2,3,4 -> stay,up,right,down,left</param>
    /// <param name="arrow">attack using arrow if true, sword if false, unused when moving</param>
    /// <returns>Commentary on results</returns>
    private string PlayerAction(bool attack, short dir, bool arrow)
    {
        if (_map?.Player is null) return "No active player";
        StringBuilder sb = new();
        if (dir == 0) return "Waited";
        string dirString = dir switch
        {
            1 => "up",
            2 => "right",
            3 => "down",
            4 => "left",
            _ => "unknown"
        };
        if (!attack)
        {
            sb.Append($"Moving {dirString}... ");
            if (!_map.TryMovePlayer(dir, out Point target))
            {
                sb.Append("failed! ");
                sb.AppendLine($"Tile already occupied by {_map.GetTileAtPos(target).Name}");
            }
            return sb.ToString();
        }

        sb.Append($"Attacking {dirString} with {(arrow ? "arrow" : "sword")}... ");
        if (arrow && _map.Player.Tile.ArrowCount <= 0)
        {
            sb.AppendLine("No arrows left!");
            return sb.ToString();
        }

        ITile hit = _map.AttackWithPlayer(dir, arrow);
        switch (hit)
        {
            case Ground: sb.AppendLine("hit nothing!"); break;
            case Rock: sb.AppendLine("hit a rock!"); break;
            case IEnemy enemy:
            {
                sb.AppendLine($"hit a {enemy.Name}!");
                if (enemy.Health <= 0)
                    sb.AppendLine($"Killed a {enemy.Name}!");
            } break;
        }
        return sb.ToString();
    }
}