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
    
    public bool EnemiesRemain => _map?.Goblins.Count > 0 || _map?.Sprites.Count > 0;
    public bool PlayerDead => _map?.Player?.Tile.IsDead ?? false;

    public void CreateNewMap(int width, int height, int rockCount, int goblinCount, int spriteCount)
    {
        _map = new Map(width, height, rockCount, goblinCount, spriteCount, 10);
    }

    public string PrintMap() => _map?.PrintMap() ?? "";

    public bool PlacePlayer(int x, int y) => _map?.PlacePlayer(x, y) ?? false;

    public string PrintLegend()
    {
        StringBuilder sb = new();
        sb.AppendLine("=== LEGEND ===");
        foreach (ITile tileType in _tileTypes)
            sb.AppendLine($"{tileType.Char} -> {tileType.Name}");
        return sb.ToString();
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
            sb.AppendLine($"{_map.Player.Tile.Health} health remaining!");
        }

        foreach (ActiveEntity<SunSprite> sprite in _map.Sprites.Where(sprite =>
                     _map.CanAttackOtherwiseStepTowardPlayer(sprite)))
        {
            _map.Player.Tile.Damage(sprite.Tile.Damage);
            sb.AppendLine($"{sprite.Tile.Name} dealt {sprite.Tile.Damage} damage!");
            sb.AppendLine($"{_map.Player.Tile.Health} health remaining!");
        }
        
        return sb.ToString();
    }

    /// <summary>
    /// Takes a player action
    /// </summary>
    /// <param name="attack">attack if true, move if false</param>
    /// <param name="dir">0,1,2,3,4 -> stay,down,right,up,left</param>
    /// <param name="arrow">attack using arrow if true, sword if false, unused when moving</param>
    /// <returns>Commentary on results</returns>
    private string PlayerAction(bool attack, short dir, bool arrow)
    {
        if (_map?.Player is null) return "No active player";
        StringBuilder sb = new();
        if (dir == 0) return "Waited";
        string dirString = dir switch
        {
            1 => "down",
            2 => "right",
            3 => "up",
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
        if (arrow)
        {
            _map.Player.Tile.ArrowCount--;
            if (_map.Player.Tile.ArrowCount <= 0)
                sb.AppendLine("No arrows left!");
            else
                sb.AppendLine($"{_map.Player.Tile.ArrowCount} arrows remaining.");
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
                {
                    sb.AppendLine($"Killed a {enemy.Name}!");
                    switch (enemy)
                    {
                        case Goblin:
                            _map.Player.Tile.ArrowCount++;
                            sb.AppendLine("Gained an arrow!");
                            break;
                        case SunSprite:
                            _map.Player.Tile.Health++;
                            sb.AppendLine("Gained one health!");
                            break;
                    }
                }
            } break;
        }
        return sb.ToString();
    }

    public string FullStep(bool attack, short dir, bool arrow)
    {
        if (_map?.Player is null) return "No active player";
        StringBuilder sb = new();
        sb.AppendLine("===== PLAYER TURN =====");
        sb.AppendLine(PlayerAction(attack, dir, arrow));
        sb.AppendLine("===== ENEMY  TURN =====");
        sb.AppendLine(Step());

        if (_map.Player.Tile.IsDead)
        {
            _map.KillPlayer();
            sb.AppendLine("\n You died!");
            sb.AppendLine("=====  GAME OVER  =====");
        }
        return sb.ToString();
    }

    public string Wait() => FullStep(false, 0, false);
    
    public string MoveDown() => FullStep(false, 1, false);
    public string MoveRight() => FullStep(false, 2, false);
    public string MoveUp() => FullStep(false, 3, false);
    public string MoveLeft() => FullStep(false, 4, false);
    
    public string StabDown() => FullStep(true, 1, false);
    public string StabRight() => FullStep(true, 2, false);
    public string StabUp() => FullStep(true, 3, false);
    public string StabLeft() => FullStep(true, 4, false);
    
    public string ShootDown() => FullStep(true, 1, true);
    public string ShootRight() => FullStep(true, 2, true);
    public string ShootUp() => FullStep(true, 3, true);
    public string ShootLeft() => FullStep(true, 4, true);
}