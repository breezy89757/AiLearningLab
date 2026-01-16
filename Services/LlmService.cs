using OpenAI.Chat;

namespace AiLearningLab.Services;

/// <summary>
/// 基礎 LLM 服務 - 展示不同層級的對話能力
/// </summary>
public class LlmService
{
    private readonly ChatClient _chatClient;
    private readonly ILogger<LlmService> _logger;

    public LlmService(ChatClient chatClient, ILogger<LlmService> logger)
    {
        _chatClient = chatClient;
        _logger = logger;
    }

    /// <summary>
    /// Level 1: 純 LLM 對話 - 無 System Prompt
    /// </summary>
    public async Task<string> ChatPureAsync(string userMessage, CancellationToken ct = default)
    {
        _logger.LogInformation("[Level 1] Pure LLM chat: {Message}", userMessage);
        
        var messages = new List<OpenAI.Chat.ChatMessage>
        {
            new UserChatMessage(userMessage)
        };

        var response = await _chatClient.CompleteChatAsync(messages, cancellationToken: ct);
        return response.Value.Content[0].Text ?? "無法取得回應";
    }

    /// <summary>
    /// Level 2: 帶 System Prompt 的對話
    /// </summary>
    public async Task<string> ChatWithSystemPromptAsync(
        string userMessage, 
        string systemPrompt, 
        CancellationToken ct = default)
    {
        _logger.LogInformation("[Level 2] Chat with System Prompt");
        
        var messages = new List<OpenAI.Chat.ChatMessage>
        {
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(userMessage)
        };

        var response = await _chatClient.CompleteChatAsync(messages, cancellationToken: ct);
        return response.Value.Content[0].Text ?? "無法取得回應";
    }

    /// <summary>
    /// Level 3: Few-shot Learning - 帶範例的對話
    /// </summary>
    public async Task<string> ChatWithFewShotAsync(
        string userMessage,
        string systemPrompt,
        List<(string User, string Assistant)> examples,
        CancellationToken ct = default)
    {
        _logger.LogInformation("[Level 3] Chat with Few-shot examples ({Count} examples)", examples.Count);
        
        var messages = new List<OpenAI.Chat.ChatMessage>
        {
            new SystemChatMessage(systemPrompt)
        };

        // 加入範例對話
        foreach (var (user, assistant) in examples)
        {
            messages.Add(new UserChatMessage(user));
            messages.Add(new AssistantChatMessage(assistant));
        }

        messages.Add(new UserChatMessage(userMessage));

        var response = await _chatClient.CompleteChatAsync(messages, cancellationToken: ct);
        return response.Value.Content[0].Text ?? "無法取得回應";
    }

    /// <summary>
    /// Level 4: 多輪對話 - 帶歷史記錄
    /// </summary>
    public async Task<string> ChatWithHistoryAsync(
        string userMessage,
        string systemPrompt,
        List<(string Role, string Content)> history,
        CancellationToken ct = default)
    {
        _logger.LogInformation("[Level 4] Chat with history ({Count} messages)", history.Count);
        
        var messages = new List<OpenAI.Chat.ChatMessage>
        {
            new SystemChatMessage(systemPrompt)
        };

        // 加入歷史對話
        foreach (var (role, content) in history)
        {
            if (role == "user")
                messages.Add(new UserChatMessage(content));
            else if (role == "assistant")
                messages.Add(new AssistantChatMessage(content));
        }

        messages.Add(new UserChatMessage(userMessage));

        var response = await _chatClient.CompleteChatAsync(messages, cancellationToken: ct);
        return response.Value.Content[0].Text ?? "無法取得回應";
    }

    /// <summary>
    /// Level 5: RAG - 帶檢索結果的對話 (簡化版)
    /// </summary>
    public async Task<string> ChatWithRagAsync(
        string userMessage,
        string systemPrompt,
        List<string> retrievedDocuments,
        CancellationToken ct = default)
    {
        _logger.LogInformation("[Level 5] RAG chat with {Count} documents", retrievedDocuments.Count);
        
        // 建構帶有檢索結果的 prompt
        var ragPrompt = $@"{systemPrompt}

以下是與問題相關的文件內容，請根據這些內容回答：

{string.Join("\n\n---\n\n", retrievedDocuments)}";

        var messages = new List<OpenAI.Chat.ChatMessage>
        {
            new SystemChatMessage(ragPrompt),
            new UserChatMessage(userMessage)
        };

        var response = await _chatClient.CompleteChatAsync(messages, cancellationToken: ct);
        return response.Value.Content[0].Text ?? "無法取得回應";
    }
}
