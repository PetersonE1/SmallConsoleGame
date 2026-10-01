using System.Drawing;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Text.Json;
using AStar;
using AStar.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmallConsoleGame.Tiles;
using SmallConsoleGame.TileTypes;
using Void = SmallConsoleGame.Tiles.Void;

namespace SmallConsoleGame;

public class Map(int width, int height, int rockCount, int goblinCount, int spriteCount)
{
	private readonly ITile[] _referenceMap = new ITile[3 + goblinCount + spriteCount];
	private readonly int[,] _grid = new int[width, height];

	public List<ActiveEntity<Goblin>> Goblins { get; } = [];
	public List<ActiveEntity<SunSprite>> Sprites { get; } = [];
	public ActiveEntity<Player>? Player = null;

	public string PrintMap()
	{
		StringBuilder sb = new();
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				sb.Append(_referenceMap[_grid[x,y]].Char);
			}

			sb.AppendLine();
		}
		return sb.ToString();
	}

	public string RawMap()
	{
		JObject map = new JObject();
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				ITile tile = _referenceMap[_grid[x, y]];
				switch (tile)
				{
					case IEnemy enemy:
						map.Add($"({x},{y})", JObject.FromObject(enemy));
						break;
					case Player player:
						map.Add($"({x},{y})", JObject.FromObject(player));
						break;
					default:
						map.Add($"({x},{y})", JObject.FromObject(tile));
						break;
				}
			}
		}
		return map.ToString();
	}

	public Map(int width, int height, int rockCount, int goblinCount, int spriteCount, int maxGenerationAttempts = 10)
		: this(width, height, rockCount, goblinCount, spriteCount)
	{
		GenerateReferenceMap();
		int iter = 0;
		while (!GenerateMap() && iter++ < maxGenerationAttempts) ;
		if (iter >= maxGenerationAttempts)
			throw new Exception("Max generation attempts exceeded");
	}

	private void GenerateReferenceMap()
	{
		_referenceMap[GetGround()] = new Ground();
		_referenceMap[GetRock()] = new Rock();
		_referenceMap[GetPlayer()] = new Player();
		for (int i = 0; i < goblinCount; i++)
			_referenceMap[GetGoblin(i)] = new Goblin();
		for (int i = 0; i < spriteCount; i++)
			_referenceMap[GetSunSprite(i)] = new SunSprite();
	}

	private bool GenerateMap()
	{
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				_grid[x, y] = GetGround();
			}
		}

		for (int i = 0; i < rockCount; i++)
		{
			(int x, int y) = GetEmptySpace();
			_grid[x, y] = GetRock();
		}

		for (int i = 0; i < goblinCount; i++)
		{
			(int x, int y) = GetEmptySpace();
			int id = GetGoblin(i);
			_grid[x, y] = id;
			Goblins.Add(new ActiveEntity<Goblin>((_referenceMap[id] as Goblin)!, id, x, y));
		}

		for (int i = 0; i < spriteCount; i++)
		{
			(int x, int y) = GetEmptySpace();
			int id = GetSunSprite(i);
			_grid[x, y] = id;
			Sprites.Add(new ActiveEntity<SunSprite>((_referenceMap[id] as SunSprite)!, id, x, y));
		}
		
		return ValidateMap();
	}

	public bool PlacePlayer(int x, int y, out string result)
	{
		if (Player is not null)
		{
			result = "Player already exists.";
			return false;
		}
		if (x < 0 || x >= width || y < 0 || y >= height)
		{
			result = "Can't place player out of bounds.";
			return false;
		}
		ITile tile = _referenceMap[_grid[x, y]];
		if (tile.IsObstacle || tile is IEnemy)
		{
			result = $"Tile is occupied by {tile.Name}";
			return false;
		}

		int id = GetPlayer();
		_grid[x, y] = id;
		Player = new ActiveEntity<Player>((_referenceMap[id] as Player)!, id, x, y);
		result = "Successfully placed player.";
		return true;
	}

	public bool TryMovePlayer(short dir, out Point target)
	{
		target = new Point();
		if (Player is null) return false;
		Point offset = dir switch
		{
			1 => new Point( 0,  1),
			2 => new Point( 1,  0),
			3 => new Point( 0, -1),
			4 => new Point(-1,  0),
			_ => throw new ArgumentOutOfRangeException(nameof(dir), dir, "Player attempted to move off the 2D plane")
		};
		target = Player.Position;
		target.Offset(offset);
		if (target.X >= width || target.Y >= height || target.X < 0 || target.Y < 0) return false;
		if (_grid[target.X, target.Y] != GetGround()) return false;
		_grid[target.X, target.Y] = GetPlayer();
		_grid[Player.Position.X, Player.Position.Y] = GetGround();
		Player.Position = target;
		return true;
	}

	public ITile AttackWithPlayer(short dir, bool arrow)
	{
		if (Player is null) return new Ground();
		Point offset = dir switch
		{
			1 => new Point( 0,  1),
			2 => new Point( 1,  0),
			3 => new Point( 0, -1),
			4 => new Point(-1,  0),
			_ => throw new ArgumentOutOfRangeException(nameof(dir), dir, "Player attempted to attack off the 2D plane")
		};
		Point target = Player.Position;
		if (!arrow)
			target.Offset(offset);
		else
		{
			Player.Tile.ArrowCount--;
			while (!GetTileAtPos(target).IsObstacle && GetTileAtPos(target) is not IEnemy)
			{
				target.Offset(offset);
				if (target.X >= width || target.Y >= height || target.X < 0 || target.Y < 0)
					break;
			}
		}
		
		ITile hit = GetTileAtPos(target);
		if (hit is not IEnemy enemy) return hit;
		
		enemy.Health -= 1;
		if (enemy.Health > 0) return hit;
		switch (enemy)
		{
			case Goblin:
				Goblins.RemoveAll(g => g.Position == target);
				break;
			case SunSprite:
				Sprites.RemoveAll(s => s.Position == target);
				break;
		}
		_grid[target.X, target.Y] = GetGround();
		return hit;
	}

	public bool CanAttackOtherwiseStepTowardPlayer<T>(ActiveEntity<T> enemy, out string movedTo) where T : IEnemy
	{
		movedTo = "Error";
		if (Player is null) return false;
		PathFinderOptions options = new()
		{
			PunishChangeDirection = true,
			UseDiagonals = false
		};
		
		short[,] tiles = new short[height, width];
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				ITile reference = _referenceMap[_grid[x, y]];
				tiles[y, x] = reference.IsObstacle ? (short)0 : (short)1;
			}
		}

		WorldGrid grid = new(tiles);
		PathFinder pathfinder = new(grid, options);
		Point[] path = pathfinder.FindPath(enemy.Position, Player.Position);
		
		if (path.Length - 1 <= enemy.Tile.Range) return true;

		if (path.Length < 2) return false;
		
		Point nextStep = path[1];
		if (_grid[nextStep.X, nextStep.Y] != GetGround())
		{
			movedTo = "... blocked! Couldn't move.";
			return false;
		}
		
		_grid[nextStep.X, nextStep.Y] = enemy.ReferenceId;
		_grid[enemy.Position.X, enemy.Position.Y] = GetGround();
		enemy.Position = nextStep;
		movedTo = enemy.Position.ToString();
		return false;
	}

	public ITile GetTileAtPos(Point point)
	{
		if (point.X < 0 || point.Y < 0 || point.X >= width || point.Y >= height) return new Void();
		return _referenceMap[_grid[point.X, point.Y]];
	}

	public void KillPlayer()
	{
		if (Player is null) return;
		_grid[Player.Position.X, Player.Position.Y] = GetGround();
	}

	private (int x, int y) GetEmptySpace()
	{
		int x = RandomNumberGenerator.GetInt32(width);
		int y = RandomNumberGenerator.GetInt32(height);
		while (_grid[x, y] != GetGround())
		{
			x = RandomNumberGenerator.GetInt32(width);
			y = RandomNumberGenerator.GetInt32(height);
		}
		return (x, y);
	}

	private bool ValidateMap()
	{
		int walkableTiles = 0;
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				if (!_referenceMap[_grid[x, y]].IsObstacle)
					walkableTiles++;
			}
		}
		
		List<(int x, int y)> checkedValues = [];
		(int x, int y) startPos = (-1, -1);
		
		for (int y = 0; y < height; y++)
		{
			bool broken = false;
			for (int x = 0; x < width; x++)
			{
				if (_referenceMap[_grid[x, y]].IsObstacle) continue;
				startPos = (x, y);
				broken = true;
				break;
			}

			if (broken)
				break;
		}
		
		int reachableTiles = CheckMap(startPos.x, startPos.y, ref checkedValues);
		return reachableTiles == walkableTiles;
	}

	private int CheckMap(int x, int y, ref List<(int x, int y)> checkedValues)
	{
		if (checkedValues.Contains((x, y))) return 0;
		checkedValues.Add((x, y));

		ITile tile = _referenceMap[_grid[x, y]];
		if (tile.IsObstacle)
			return 0;

		int runningCount = 1;
		runningCount += CheckMap(Math.Max(x - 1, 0), y, ref checkedValues);
		runningCount += CheckMap(Math.Min(x + 1, width - 1), y, ref checkedValues);
		runningCount += CheckMap(x, Math.Max(y - 1, 0), ref checkedValues);
		runningCount += CheckMap(x, Math.Min(y + 1, height - 1), ref checkedValues);
		
		return runningCount;
	}

	private int GetGround() => 0;
	private int GetRock() => 1;
	private int GetPlayer() => 2;
	private int GetGoblin(int index) => index + 3;
	private int GetSunSprite(int index) => index + 3 + goblinCount;
}