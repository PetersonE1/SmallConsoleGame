namespace SmallConsoleGame;

public static class Program
{
	public static void Main(string[] args)
	{
		Console.OutputEncoding = System.Text.Encoding.UTF8;
		GameManager manager = new();
		manager.CreateNewMap(5, 5, 3, 2, 1);
		manager.PrintMap();
		Console.WriteLine("Placing player at (0, 0)... " + (manager.PlacePlayer(0, 0) ? "Success" : "Failed"));
		manager.PrintMap();
		manager.PrintLegend();
		Console.WriteLine(manager.Step());
		manager.PrintMap();
	}
}