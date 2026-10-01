using System.ComponentModel;
using System.Text;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace SmallConsoleGame.GameTools;

[McpServerToolType]
public static class InfoTools
{
    [McpServerTool, Description("Get the game explanation.")]
    public static async Task<string> GetExplanation()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Welcome to Small Console Game!");
        sb.AppendLine("The goal is to survive and defeat every enemy.");
        sb.AppendLine(
            "After generating a map, place your character down in a spot of your choosing. You will act first.");
        sb.AppendLine(
            "Each turn, you can move or attack in any of the four cardinal directions. Arrows have infinite range, but you have a limited supply.");
        sb.AppendLine(
            "After your turn, enemies will each attempt to move one step closer to you until they are in attack range.");
        sb.AppendLine(
            "Once an enemy is capable of reaching you in a number of turns less than or equal to their range, they can and will attack you.");
        sb.AppendLine("Enemies chase using the A* pathfinding algorithm.");
        sb.AppendLine(
            "Defeating a goblin will reward you one arrow. Defeating a sun sprite will increase your health by one.");
        sb.AppendLine(
            "To get started, use the CreateGameMap tool to create a new map (re)starting the game.");
        return sb.ToString();
    }
    
    [McpServerTool, Description("Get the game map.")]
    public static async Task<string> GetMap(GameManager manager)
    {
        return manager.PrintMap();
    }
    
    [McpServerTool, Description("Get the game map as raw data.")]
    public static async Task<string> GetRawData(GameManager manager)
    {
        return manager.RawMap();
    }

    [McpServerTool, Description("Get the legend.")]
    public static async Task<string> GetLegend(GameManager manager)
    {
        return manager.PrintLegend();
    }
}