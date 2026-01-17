# AI 學習實驗室 🧪

一個互動式 Blazor 網頁應用程式，用於教學 AI 技術演進 — 從基本 LLM 呼叫到 MCP (Model Context Protocol)。

[🇺🇸 English](../en/README.md)

## 📊 AI 技術演進

![AI Technology Evolution](../../docs/ai_evolution.png)

## 🎯 特色功能

- **7 個漸進層級** — 每個層級解決上一層的問題
- **並列比較** — 看看每種技術如何改善 AI 回應
- **可收合程式碼** — 學習每個層級需要什麼程式碼
- **即時演示** — 實際呼叫 Azure OpenAI 看結果
- **多語系** — 支援英文和繁體中文

## 🏗️ 技術層級

| 層級 | 名稱 | 新增功能 | 解決問題 |
|------|------|----------|----------|
| L1 | 純 LLM | 基本對話 | - |
| L2 | System Prompt | 角色/身份 | 沒有一致性 |
| L3 | Few-shot | 範例輸出 | 格式不可預測 |
| L4 | 對話記憶 | 對話歷史 | AI 健忘 |
| L5 | RAG | 讀取文件 | AI 沒有私有知識 |
| L6 | Function Calling | 工具呼叫 | 沒有即時資料 |
| L7 | Agent | 多步驟推理 | 需手動協調 |
| L8 | MCP | 標準協議 | 工具整合麻煩 |
| L9 | LangGraph | 圖形化流程 | 複雜任務不可控 |
| L10 | Observability | 觀測與除錯 | AI 行為黑盒子 |

## 🚀 快速開始

### 前置需求

- .NET 10.0 SDK
- Azure OpenAI 或 LiteLLM 端點

### 設定

1. 編輯 `appsettings.json`：

```json
{
  "LlmProvider": {
    "Provider": "AzureOpenAI"
  },
  "AzureOpenAI": {
    "Endpoint": "https://your-resource.openai.azure.com/",
    "ApiKey": "your-api-key",
    "DeploymentName": "gpt-4o"
  }
}
```

2. 執行：

```bash
dotnet run
```

3. 開啟 http://localhost:5062

## 💡 建議測試案例

| 層級 | 輸入 | 預期行為 |
|------|------|----------|
| L1 vs L2 | "退貨流程是什麼" | L1 = 通用回答, L2 = 品牌專屬 |
| L3 | "這產品很爛" | 回傳 JSON: `{"sentiment":"negative"}` |
| L4 | "我叫小明" 然後 "我叫什麼" | 記得「小明」 |
| L6 | "台北天氣" | 呼叫天氣工具 |
| L7 | "查天氣，晴天就預約會議室" | 多步驟推理 |

## 📂 專案結構

```
AiLearningLab/
├── Components/
│   ├── Layout/MainLayout.razor    # 側邊欄含語言切換
│   └── Pages/Home.razor           # 單頁含所有層級
├── Services/
│   ├── LlmService.cs              # L1-L4 實作
│   ├── AgentService.cs            # L6-L7 實作
│   └── McpService.cs              # L8 MCP 連線
├── Resources/                     # 在地化檔案
│   ├── SharedResource.en.resx     # 英文
│   └── SharedResource.zh-TW.resx  # 繁體中文
├── Plugins/
│   └── DemoPlugin.cs              # 範例工具
└── Models/
    └── LearningLevel.cs           # 層級中繼資料
```

## 🛠️ 技術堆疊

- **前端**: Blazor Server (.NET 10)
- **AI**: Azure OpenAI / LiteLLM
- **框架**: Microsoft.Extensions.AI, Microsoft.Agents.AI

## 📚 保持更新的資源

這個專案涵蓋的 AI 技術正在快速演進，以下是保持最新資訊的資源：

### 官方來源
- [Azure OpenAI Service 文件](https://learn.microsoft.com/azure/ai-services/openai/) — 最新 API 和功能
- [Microsoft.Extensions.AI](https://aka.ms/meai) — 統一 AI 抽象層
- [MCP 官方規範](https://modelcontextprotocol.io/) — Model Context Protocol 標準

### 社群和最新消息
- [Awesome MCP Servers](https://github.com/punkpeye/awesome-mcp-servers) — MCP 伺服器整理列表
- [Azure AI Blog](https://techcommunity.microsoft.com/t5/ai-azure-ai-services-blog/bg-p/Azure-AI-Services-blog) — 官方公告
- [LangChain Updates](https://blog.langchain.dev/) — AI 應用開發趨勢

### 即時追蹤
- Twitter/X: [@Azure](https://twitter.com/Azure), [@OpenAI](https://twitter.com/OpenAI)
- Discord: [LangChain](https://discord.gg/langchain), [MCP Community](https://discord.gg/mcp)

## 📝 授權

MIT
