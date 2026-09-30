using System.Security.Cryptography;
using System.Linq;
using SmallConsoleGame.Tiles;
using SmallConsoleGame.TileTypes;

namespace SmallConsoleGame;

public class Map(int width, int height, int rockCount, int goblinCount, int spriteCount)
{
	private readonly ITile[] _referenceMap = new ITile[3 + goblinCount + spriteCount];
	private readonly int[,] _grid = new int[width, height];

	public void PrintMap()
	{
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				Console.Write(_referenceMap[_grid[x,y]].Char);
			}
			Console.WriteLine();
		}
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
			_grid[x, y] = GetGoblin(i);
		}

		for (int i = 0; i < spriteCount; i++)
		{
			(int x, int y) = GetEmptySpace();
			_grid[x, y] = GetSunSprite(i);
		}
		
		return ValidateMap();
	}

	public bool PlacePlayer(int x, int y)
	{
		ITile tile = _referenceMap[_grid[x, y]];
		if (tile.IsObstacle || tile is IEnemy)
			return false;
		
		_grid[x, y] = GetPlayer();
		return true;
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

		if (_referenceMap[_grid[x, y]].IsObstacle)
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