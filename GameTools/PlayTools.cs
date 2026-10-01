using System.ComponentModel;
using ModelContextProtocol.Server;

namespace SmallConsoleGame.GameTools;

[McpServerToolType]
public class PlayTools
{
    [McpServerTool, Description("Create a new game map and view the result.")]
    public static async Task<string> CreateGameMap(
        GameManager manager,
        [Description("The width of the map in units.")] int width,
        [Description("The height of the map in units.")] int height,
        [Description("The number of rocks on the map.")] int rockCount,
        [Description("The number of goblins on the map.")] int goblinCount,
        [Description("The number of solar sprites on the map.")] int spriteCount)
    {
        manager.CreateNewMap(width, height, rockCount, goblinCount, spriteCount);
        return manager.PrintMap();
    }

    [McpServerTool, Description("Place the player on the map.")]
    public static async Task<string> PlacePlayer(
        GameManager manager,
        [Description("The X coordinate (left to right).")]
        int x,
        [Description("The Y coordinate (top to bottom).")]
        int y)
    {
        manager.PlacePlayer(x, y, out string result);
        return result;
    }

    [McpServerTool, Description("Wait a turn.")]
    public static async Task<string> Wait(GameManager manager) => manager.Wait();
    
    [McpServerTool, Description("Move one unit up.")]
    public static async Task<string> MoveUp(GameManager manager) => manager.MoveUp();
    
    [McpServerTool, Description("Move one unit right.")]
    public static async Task<string> MoveRight(GameManager manager) => manager.MoveRight();

    [McpServerTool, Description("Move one unit down.")]
    public static async Task<string> MoveDown(GameManager manager) => manager.MoveDown();
    
    [McpServerTool, Description("Move one unit left.")]
    public static async Task<string> MoveLeft(GameManager manager) => manager.MoveLeft();
    
    [McpServerTool, Description("Attack directly above you with your sword.")]
    public static async Task<string> StabUp(GameManager manager) => manager.StabUp();
    
    [McpServerTool, Description("Attack directly to the right of you with your sword.")]
    public static async Task<string> StabRight(GameManager manager) => manager.StabRight();
    
    [McpServerTool, Description("Attack directly below you with your sword.")]
    public static async Task<string> StabDown(GameManager manager) => manager.StabDown();
    
    [McpServerTool, Description("Attack directly to the left of you with your sword.")]
    public static async Task<string> StabLeft(GameManager manager) => manager.StabLeft();
    
    [McpServerTool, Description("Attack above you with your bow.")]
    public static async Task<string> ShootUp(GameManager manager) => manager.ShootUp();
    
    [McpServerTool, Description("Attack to the right of you with your bow.")]
    public static async Task<string> ShootRight(GameManager manager) => manager.ShootRight();
    
    [McpServerTool, Description("Attack below you with your bow.")]
    public static async Task<string> ShootDown(GameManager manager) => manager.ShootDown();
    
    [McpServerTool, Description("Attack to the left of you with your bow.")]
    public static async Task<string> ShootLeft(GameManager manager) => manager.ShootLeft();
}