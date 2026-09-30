using SmallConsoleGame.Tiles;

namespace SmallConsoleGame;

public class Map(int Width, int Height)
{
	public ITile[,] Grid = new ITile[Height,Width];

	public void PrintMap()
	{
		for (int y = 0; y < Height; y++)
		{
			for (int x = 0; x < Width; x++)
			{
				Console.Write(Grid[y,x].Char);
			}
			Console.WriteLine();
		}
	}

	public void Init()
	{
		for (int y = 0; y < Height; y++)
		{
			for (int x = 0; x < Width; x++)
			{
				Grid[y,x] = new Ground();
			}
		}
		Grid[0,3] = new Rock();
		Grid[1,1] = new Rock();
		Grid[3,2] = new Rock();
	}
}