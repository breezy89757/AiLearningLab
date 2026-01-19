using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using AiLearningLab.Plugins;

namespace AiLearningLab.Services;

/// <summary>
/// Agent 服務 - 展示 Tool Calling 和 Agent 能力
/// </summary>
public class AgentService
{
    private readonly ChatClient _chatClient;
    private readonly ILogger<AgentService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public AgentService(
        ChatClient chatClient, 
        ILogger<AgentService> logger,
        IServiceProvider serviceProvider)
    {
        _chatClient = chatClient;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Level 6: Function Calling - 單次工具呼叫
    /// </summary>
    public async Task<(string Response, List<ToolCallInfo> ToolCalls)> ChatWithToolsAsync(
        string userMessage,
        string systemPrompt,
        bool enableOrderTools = true,
        bool enableSearch = false,
        CancellationToken ct = default)
    {
        _logger.LogInformation("[Level 6] Chat with Tools");
        
        var toolCalls = new List<ToolCallInfo>();
        var tools = new List<AITool>();
        
        var demoPlugin = new DemoPlugin();

        if (enableOrderTools)
        {
            tools.Add(AIFunctionFactory.Create(demoPlugin.GetOrderStatus));
            tools.Add(AIFunctionFactory.Create(demoPlugin.SubmitReturnRequest));
        }
        if (enableSearch)
        {
            tools.Add(AIFunctionFactory.Create(demoPlugin.SearchDocuments));
        }

        // 設置工具呼叫追蹤
        demoPlugin.OnToolCalled = info => toolCalls.Add(info);

        var agent = _chatClient.CreateAIAgent(
            instructions: systemPrompt,
            tools: tools);

        var messages = new List<Microsoft.Extensions.AI.ChatMessage>
        {
            new(Microsoft.Extensions.AI.ChatRole.System, systemPrompt),
            new(Microsoft.Extensions.AI.ChatRole.User, userMessage)
        };

        var result = await agent.RunAsync(messages, cancellationToken: ct);
        var responseText = result?.Text ?? "無法取得回應";

        return (responseText, toolCalls);
    }

    /// <summary>
    /// Level 7: Agent - 多步驟自主推理
    /// </summary>
    public async Task<(string Response, List<ToolCallInfo> ToolCalls)> RunAgentAsync(
        string userMessage,
        string systemPrompt,
        List<(string Role, string Content)> history,
        CancellationToken ct = default)
    {
        _logger.LogInformation("[Level 7] Agent with multi-step reasoning");
        
        var toolCalls = new List<ToolCallInfo>();
        var demoPlugin = new DemoPlugin();
        demoPlugin.OnToolCalled = info => toolCalls.Add(info);

        var tools = new List<AITool>
        {
            AIFunctionFactory.Create(demoPlugin.GetOrderStatus),
            AIFunctionFactory.Create(demoPlugin.SubmitReturnRequest),
            AIFunctionFactory.Create(demoPlugin.SearchDocuments),
            AIFunctionFactory.Create(demoPlugin.GetCurrentTime)
        };

        var agent = _chatClient.CreateAIAgent(
            instructions: systemPrompt,
            tools: tools);

        var messages = new List<Microsoft.Extensions.AI.ChatMessage>
        {
            new(Microsoft.Extensions.AI.ChatRole.System, systemPrompt)
        };

        // 加入歷史
        foreach (var (role, content) in history)
        {
            var chatRole = role == "user" 
                ? Microsoft.Extensions.AI.ChatRole.User 
                : Microsoft.Extensions.AI.ChatRole.Assistant;
            messages.Add(new Microsoft.Extensions.AI.ChatMessage(chatRole, content));
        }

        messages.Add(new Microsoft.Extensions.AI.ChatMessage(
            Microsoft.Extensions.AI.ChatRole.User, userMessage));

        var result = await agent.RunAsync(messages, cancellationToken: ct);
        var responseText = result?.Text ?? "無法取得回應";

        return (responseText, toolCalls);
    }
}

/// <summary>
/// 工具呼叫資訊
/// </summary>
public class ToolCallInfo
{
    public string ToolName { get; set; } = "";
    public string Arguments { get; set; } = "";
    public string Result { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
