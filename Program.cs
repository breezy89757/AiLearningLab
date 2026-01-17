using AiLearningLab.Components;
using AiLearningLab.Services;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Localization;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// Configure LLM provider settings (AzureOpenAI / LiteLLM)
var providerConfig = builder.Configuration.GetSection("LlmProvider");
var provider = providerConfig["Provider"] ?? "LiteLLM";


ChatClient chatClient;

if (string.Equals(provider, "AzureOpenAI", StringComparison.OrdinalIgnoreCase))
{
    var aoaiConfig = builder.Configuration.GetSection("AzureOpenAI");
    var endpoint = aoaiConfig["Endpoint"] ?? throw new InvalidOperationException("AzureOpenAI:Endpoint is required");
    var apiKey = aoaiConfig["ApiKey"] ?? throw new InvalidOperationException("AzureOpenAI:ApiKey is required");
    var deploymentName = aoaiConfig["DeploymentName"] ?? "gpt-4o";


    var azureClient = new AzureOpenAIClient(new Uri(endpoint), new Azure.AzureKeyCredential(apiKey));
    chatClient = azureClient.GetChatClient(deploymentName);
}
else
{
    var liteLlmConfig = builder.Configuration.GetSection("LiteLLM");
    var endpoint = liteLlmConfig["Endpoint"] ?? "http://localhost:4000/v1";
    var apiKey = liteLlmConfig["ApiKey"] ?? "YOUR_API_KEY";
    var chatModel = liteLlmConfig["ChatModel"] ?? "gpt-4.1";

    var openAiClient = new OpenAIClient(
        new ApiKeyCredential(apiKey),
        new OpenAIClientOptions { Endpoint = new Uri(endpoint) });
    chatClient = openAiClient.GetChatClient(chatModel);
}

// Register ChatClient
builder.Services.AddSingleton(chatClient);

// Register Learning Lab services
builder.Services.AddScoped<LlmService>();
builder.Services.AddScoped<AgentService>();
builder.Services.AddSingleton<McpService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

// Add localization middleware
var supportedCultures = new[] { "en", "zh-TW" };
app.UseRequestLocalization(options =>
{
    options.SetDefaultCulture("zh-TW");
    options.AddSupportedCultures(supportedCultures);
    options.AddSupportedUICultures(supportedCultures);
    options.ApplyCurrentCultureToResponseHeaders = true;
    options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
});

app.MapControllers();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
