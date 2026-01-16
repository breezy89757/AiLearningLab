using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace AiLearningLab.Services;

/// <summary>
/// MCP 服務 - 展示 Model Context Protocol
/// </summary>
public class McpService : IAsyncDisposable
{
    private readonly ILogger<McpService> _logger;
    private readonly IConfiguration _configuration;
    private McpClient? _mcpClient;
    private bool _isConnected;

    public bool IsConnected => _isConnected;
    public string? ServerName { get; private set; }
    public List<string> AvailableTools { get; private set; } = new();

    public McpService(ILogger<McpService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// 連接到 MCP Server
    /// </summary>
    public async Task<bool> ConnectAsync(string? serverCommand = null, string[]? serverArgs = null)
    {
        try
        {
            var labConfig = _configuration.GetSection("LearningLab");
            var command = serverCommand ?? labConfig["McpServerCommand"] ?? "npx";
            var args = serverArgs ?? labConfig.GetSection("McpServerArgs").Get<string[]>() 
                ?? new[] { "-y", "@anthropic/mcp-server-filesystem", "C:\\temp" };

            _logger.LogInformation("Connecting to MCP Server: {Command} {Args}", 
                command, string.Join(" ", args));

            _mcpClient = await McpClient.CreateAsync(new StdioClientTransport(new()
            {
                Name = "LearningLabMCP",
                Command = command,
                Arguments = args
            }));

            // 取得可用工具列表
            var tools = await _mcpClient.ListToolsAsync();
            AvailableTools = tools.Select(t => t.Name).ToList();
            
            ServerName = command + " " + string.Join(" ", args);
            _isConnected = true;
            
            _logger.LogInformation("MCP connected. Available tools: {Tools}", 
                string.Join(", ", AvailableTools));
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to MCP Server");
            _isConnected = false;
            return false;
        }
    }

    /// <summary>
    /// 取得 MCP 工具作為 AITool 列表
    /// </summary>
    public async Task<IEnumerable<AITool>> GetToolsAsync()
    {
        if (_mcpClient == null || !_isConnected)
        {
            return Enumerable.Empty<AITool>();
        }

        var tools = await _mcpClient.ListToolsAsync();
        return tools.Cast<AITool>();
    }

    /// <summary>
    /// 斷開連接
    /// </summary>
    public async Task DisconnectAsync()
    {
        if (_mcpClient != null)
        {
            await _mcpClient.DisposeAsync();
            _mcpClient = null;
        }
        _isConnected = false;
        AvailableTools.Clear();
        ServerName = null;
    }

    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();
    }
}
