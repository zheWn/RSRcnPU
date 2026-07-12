using Newtonsoft.Json;

namespace RotationSolver.Localization;

/// <summary>
/// 轻量级本地化引擎。
/// 从 JSON 字典文件加载翻译，提供 Get() 方法。
/// 字典中找不到 key 时自动返回 fallback（原文），不会崩溃。
/// </summary>
public static class Loc
{
    private static Dictionary<string, string> _dict = new(StringComparer.OrdinalIgnoreCase);
    private static bool _initialized = false;

    /// <summary> 初始化：从 JSON 文件加载字典 </summary>
    public static void Initialize(string jsonPath)
    {
        if (!File.Exists(jsonPath))
        {
            _initialized = true;
            return;
        }
        try
        {
            var json = File.ReadAllText(jsonPath);
            _dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json)
                    ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        catch { }
        _initialized = true;
    }

    /// <summary> 翻译：查找 key，找不到返回 fallback；fallback 为空时返回 key 本身 </summary>
    public static string Get(string key, string fallback = "")
    {
        if (_dict.TryGetValue(key, out var value)) return value;
        return string.IsNullOrEmpty(fallback) ? key : fallback;
    }

    /// <summary> 翻译（带格式化参数），支持 {0} {1} 占位符 </summary>
    public static string Format(string key, string fallback, params object[] args)
    {
        var template = Get(key, fallback);
        return string.Format(template, args);
    }

    /// <summary> 给 UiString.GetDescription() 使用的拦截方法 </summary>
    internal static string GetUiString(string enumName, string fallback)
    {
        if (!_initialized) return fallback;
        if (_dict.TryGetValue(enumName, out var localized)) return localized;
        return fallback;
    }
}
