using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Server;

namespace SmallConsoleGame;

public static class Program
{
	private static readonly GameManager Manager = new();
	public static void Main(string[] args)
	{
		Console.OutputEncoding = System.Text.Encoding.UTF8;
		
		switch (args.Length)
		{
			case 0:
				HumanPlayer();
				break;
			case 1:
				switch (args[0])
				{
					case "-h": PrintInstructions(); break;
					case "--help": PrintInstructions(); break;
					case "--mcp": McpPlayer(); break;
					case "-t": BasicTestRun(); break;
					default: Console.WriteLine("Invalid argument"); break;
				}
				break;
		}
	}

	private static void PrintInstructions() => Console.WriteLine("Pass no arguments to play as a human! Pass the `--mcp` option to start an MCP server.");

	private static void HumanPlayer()
	{
		while (true)
		{
			Console.Write("Enter map width: ");
			int width = int.Parse(Console.ReadLine() ?? "");
			Console.Write("Enter map height: ");
			int height = int.Parse(Console.ReadLine() ?? "");
			Console.Write("Enter rock count: ");
			int rockCount = int.Parse(Console.ReadLine() ?? "");
			Console.Write("Enter goblin count: ");
			int goblinCount = int.Parse(Console.ReadLine() ?? "");
			Console.Write("Enter Sun Sprite count: ");
			int spriteCount = int.Parse(Console.ReadLine() ?? "");
			Manager.CreateNewMap(width, height, rockCount, goblinCount, spriteCount);
			Console.WriteLine(Manager.PrintMap());
			Console.Write("Choose an (x, y) coordinate pair to place your character, separated by a space: ");
			int[] coords = Console.ReadLine()?.Split(' ').Select(int.Parse).ToArray() ?? [];

			while (!Manager.PlacePlayer(coords[0], coords[1], out string result))
			{
				Console.Write($"{result}, choose again: ");
				coords = Console.ReadLine()?.Split(' ').Select(int.Parse).ToArray() ?? [];
			}
			
			Console.WriteLine(Manager.PrintMap());
			while (Manager.EnemiesRemain)
			{
				Console.WriteLine("Select an action:");
				Console.WriteLine("1. Wait");
				Console.WriteLine("2. Move");
				Console.WriteLine("3. Attack");
				int choice = int.Parse(Console.ReadLine() ?? "");
				if (choice == 1)
				{
					Console.WriteLine(Manager.Wait());
					Console.WriteLine(Manager.PrintMap());
					if (Manager.PlayerDead) break;
					continue;
				}
				Console.WriteLine("Choose a direction:");
				Console.WriteLine("1. Down");
				Console.WriteLine("2. Right");
				Console.WriteLine("3. Up");
				Console.WriteLine("4. Left");
				short direction = short.Parse(Console.ReadLine() ?? "");
				bool useBow = false;
				if (choice == 3)
				{
					Console.WriteLine("Use bow? Y/N: ");
					useBow = (Console.ReadLine()?.ToLower() ?? "") == "y";
				}
				Console.WriteLine(Manager.FullStep(choice == 3, direction, useBow));
				if (Manager.PlayerDead)
				{
					break;
				}
				Console.WriteLine(Manager.PrintMap());
			}
			Console.Write("Play again? Y/N: ");
			string input = Console.ReadLine()?.ToLower() ?? "";
			if (input == "n") break;
		}
		Console.WriteLine("Thank you for playing!");
	}

	private static void McpPlayer()
	{
		HostApplicationBuilder builder = Host.CreateEmptyApplicationBuilder(settings: null);

		builder.Services.AddMcpServer()
			.WithStdioServerTransport()
			.WithToolsFromAssembly();
		
		builder.Services.AddSingleton(Manager);
		
		IHost app = builder.Build();
		app.Run();
	}

	private static void BasicTestRun()
	{
		Manager.CreateNewMap(5, 5, 3, 2, 1);
		Console.WriteLine(Manager.PrintMap());
		Console.WriteLine("Placing player at (0, 0)... " + (Manager.PlacePlayer(0, 0, out string result) ? "Success" : "Failed"));
		Console.WriteLine(Manager.PrintMap());
		Console.WriteLine(Manager.PrintLegend());
		Console.WriteLine(Manager.Step());
		Console.WriteLine(Manager.PrintMap());
		Console.WriteLine(Manager.RawMap());
	}
}