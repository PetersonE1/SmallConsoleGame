namespace SmallConsoleGame;

public static class Program
{
	public static void Main(string[] args)
	{
		Console.OutputEncoding = System.Text.Encoding.UTF8;
		Map map = new(5, 5, 3, 2, 1, 10);
		map.PrintMap();
	}
}