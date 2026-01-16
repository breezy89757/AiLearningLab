using System.ComponentModel;
using AiLearningLab.Services;

namespace AiLearningLab.Plugins;

/// <summary>
/// Demo 工具集 - 用於展示 Function Calling 和 Agent
/// </summary>
public class DemoPlugin
{
    public Action<ToolCallInfo>? OnToolCalled { get; set; }

    private void RecordToolCall(string name, string args, string result)
    {
        OnToolCalled?.Invoke(new ToolCallInfo
        {
            ToolName = name,
            Arguments = args,
            Result = result,
            Timestamp = DateTime.Now
        });
    }

    [Description("取得指定城市的天氣資訊")]
    public string GetWeather(
        [Description("城市名稱，例如：台北、東京、紐約")] string city)
    {
        // 模擬天氣資料
        var weathers = new Dictionary<string, (int Temp, string Condition)>
        {
            ["台北"] = (25, "晴天 ☀️"),
            ["東京"] = (18, "多雲 ⛅"),
            ["紐約"] = (12, "陰天 🌥️"),
            ["倫敦"] = (8, "小雨 🌧️"),
            ["北京"] = (15, "霧霾 🌫️")
        };

        var (temp, condition) = weathers.GetValueOrDefault(city, (20, "晴天 ☀️"));
        var result = $"{{\"city\": \"{city}\", \"temperature\": {temp}, \"condition\": \"{condition}\"}}";
        
        RecordToolCall("GetWeather", $"city: {city}", result);
        return result;
    }

    [Description("計算數學表達式")]
    public string Calculate(
        [Description("數學表達式，例如：2+2 或 (10*5)/2")] string expression)
    {
        try
        {
            // 簡易計算器 (實際應用應使用安全的表達式計算庫)
            var result = new System.Data.DataTable().Compute(expression, null);
            var output = $"{{\"expression\": \"{expression}\", \"result\": {result}}}";
            
            RecordToolCall("Calculate", $"expression: {expression}", output);
            return output;
        }
        catch
        {
            var error = $"{{\"error\": \"無法計算表達式: {expression}\"}}";
            RecordToolCall("Calculate", $"expression: {expression}", error);
            return error;
        }
    }

    [Description("搜尋內部文件庫 (模擬 RAG)")]
    public string SearchDocuments(
        [Description("搜尋關鍵字")] string query)
    {
        // 模擬文件搜尋結果
        var docs = new Dictionary<string, string>
        {
            ["請假"] = "根據公司規定，員工每年有 14 天特休假。請假需提前 3 天申請，緊急情況除外。",
            ["報帳"] = "報帳流程：1. 填寫報帳單 2. 附上收據 3. 主管簽核 4. 送財務部審核。",
            ["會議室"] = "會議室預約請使用內部系統，最多可預約 2 週內的時段，單次最長 2 小時。",
            ["加班"] = "加班需事先申請，平日加班費為時薪 1.33 倍，假日為 2 倍。"
        };

        var results = docs
            .Where(d => d.Key.Contains(query) || d.Value.Contains(query))
            .Select(d => d.Value)
            .ToList();

        var output = results.Any() 
            ? $"{{\"found\": {results.Count}, \"documents\": [\"{string.Join("\", \"", results)}\"]}}"
            : "{\"found\": 0, \"documents\": []}";
        
        RecordToolCall("SearchDocuments", $"query: {query}", output);
        return output;
    }

    [Description("取得目前時間")]
    public string GetCurrentTime()
    {
        var now = DateTime.Now;
        var result = $"{{\"datetime\": \"{now:yyyy-MM-dd HH:mm:ss}\", \"dayOfWeek\": \"{now:dddd}\"}}";
        
        RecordToolCall("GetCurrentTime", "", result);
        return result;
    }

    [Description("預約會議室")]
    public string BookMeeting(
        [Description("會議室名稱")] string room,
        [Description("日期，格式 YYYY-MM-DD")] string date,
        [Description("開始時間，格式 HH:MM")] string startTime,
        [Description("會議時長（小時）")] int durationHours)
    {
        // 模擬預約邏輯
        var random = new Random();
        var success = random.Next(100) > 30; // 70% 成功率

        string result;
        if (success)
        {
            result = $"{{\"success\": true, \"room\": \"{room}\", \"date\": \"{date}\", \"time\": \"{startTime}\", \"duration\": {durationHours}, \"confirmationCode\": \"MTG-{random.Next(1000, 9999)}\"}}";
        }
        else
        {
            result = $"{{\"success\": false, \"reason\": \"會議室 {room} 在 {date} {startTime} 已被預約\"}}";
        }
        
        RecordToolCall("BookMeeting", $"room: {room}, date: {date}, time: {startTime}, duration: {durationHours}h", result);
        return result;
    }
}
