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

    [Description("查詢訂單狀態")]
    public string GetOrderStatus(
        [Description("訂單編號，例如：ORD-12345")] string orderId)
    {
        // 模擬訂單資料
        var orders = new Dictionary<string, (string Status, string Item, bool CanReturn)>
        {
            ["ORD-2024001"] = ("已完成", "藍色襯衫", true),
            ["ORD-2024002"] = ("運送中", "無線耳機", false),
            ["ORD-2024003"] = ("已完成", "生鮮食品", false) // 生鮮不可退
        };

        if (orders.TryGetValue(orderId, out var info))
        {
            var result = $"{{\"orderId\": \"{orderId}\", \"item\": \"{info.Item}\", \"status\": \"{info.Status}\", \"canReturn\": {info.CanReturn.ToString().ToLower()}}}";
            RecordToolCall("GetOrderStatus", $"orderId: {orderId}", result);
            return result;
        }
        else
        {
            var error = $"{{\"error\": \"找不到訂單 {orderId}\"}}";
            RecordToolCall("GetOrderStatus", $"orderId: {orderId}", error);
            return error;
        }
    }

    [Description("提交退貨申請")]
    public string SubmitReturnRequest(
        [Description("訂單編號")] string orderId,
        [Description("退貨原因")] string reason)
    {
        var result = $"{{\"success\": true, \"orderId\": \"{orderId}\", \"message\": \"退貨申請已受理，請於 3 日內將商品寄回。退貨原因：{reason}\"}}";
        RecordToolCall("SubmitReturnRequest", $"orderId: {orderId}, reason: {reason}", result);
        return result;
    }

    [Description("搜尋內部文件庫 (模擬 RAG)")]
    public string SearchDocuments(
        [Description("搜尋關鍵字")] string query)
    {
        // 模擬文件搜尋結果 (退貨相關)
        var docs = new Dictionary<string, string>
        {
            ["退貨政策"] = "本公司提供 7 天鑑賞期。生鮮食品、衛生用品若拆封概不接受退貨。一般商品需保持包裝完整。",
            ["退款流程"] = "退貨申請核准後，款項將於 3-5 個工作天內刷退至原信用卡。若為貨到付款，將匯款至指定帳戶。",
            ["運費說明"] = "退貨產生的運費由買家自行負擔，除非商品有瑕疵或寄錯商品。滿 2000 元訂單享有免費到府收件服務。",
            ["換貨須知"] = "若商品尺寸不合，可申請換貨一次。請在訂單頁面選擇「申請換貨」並註明正確尺寸。"
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
}
