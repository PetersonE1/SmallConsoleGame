namespace SmallConsoleGame;

public static class Program
{
	public static void Main(string[] args)
	{
		Map map = new(5, 5);
		map.Init();
		map.PrintMap();
	}
}