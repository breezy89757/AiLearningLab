namespace AiLearningLab.Models;

/// <summary>
/// 學習層級定義
/// </summary>
public class LearningLevel
{
    public int Level { get; set; }
    public string Title { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Description { get; set; } = "";
    public string WhyNeeded { get; set; } = "";
    public string Route { get; set; } = "";
}

/// <summary>
/// 預設層級資料
/// </summary>
public static class LearningLevels
{
    public static List<LearningLevel> All => new()
    {
        new() { 
            Level = 1, 
            Title = "純 LLM 對話", 
            Icon = "💬",
            Description = "最基礎的 AI 對話，沒有任何指引",
            WhyNeeded = "了解 LLM 的原始能力與限制",
            Route = "/level1"
        },
        new() { 
            Level = 2, 
            Title = "System Prompt", 
            Icon = "🎭",
            Description = "給 AI 角色設定與規則",
            WhyNeeded = "控制 AI 的行為模式，確保一致性",
            Route = "/level2"
        },
        new() { 
            Level = 3, 
            Title = "Few-shot Learning", 
            Icon = "📝",
            Description = "透過範例教 AI 特定格式",
            WhyNeeded = "讓 AI 學會輸出特定格式或風格",
            Route = "/level3"
        },
        new() { 
            Level = 4, 
            Title = "對話記憶", 
            Icon = "🧠",
            Description = "多輪對話的上下文記憶",
            WhyNeeded = "維持對話連貫性，理解前後文",
            Route = "/level4"
        },
        new() { 
            Level = 5, 
            Title = "RAG 檢索增強", 
            Icon = "📚",
            Description = "結合知識庫的智慧問答",
            WhyNeeded = "讓 AI 回答私有/最新知識",
            Route = "/level5"
        },
        new() { 
            Level = 6, 
            Title = "Function Calling", 
            Icon = "🔧",
            Description = "AI 呼叫外部工具",
            WhyNeeded = "擴展 AI 能力到實際操作",
            Route = "/level6"
        },
        new() { 
            Level = 7, 
            Title = "Agent 自主代理", 
            Icon = "🤖",
            Description = "AI 自主規劃並執行多步驟任務",
            WhyNeeded = "處理複雜任務，自動調整策略",
            Route = "/level7"
        },
        new() { 
            Level = 8, 
            Title = "MCP 標準協議", 
            Icon = "🌐",
            Description = "Model Context Protocol 標準化連接",
            WhyNeeded = "統一工具連接方式，像 USB 一樣即插即用",
            Route = "/level8"
        }
    };
}
