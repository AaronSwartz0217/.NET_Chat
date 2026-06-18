using System;
using Avalonia.Data.Converters;
using System.Globalization;

namespace Chat.Desktop;

/// <summary>
/// 布尔值转文本（参数格式: "真值文本|假值文本"）
/// 用于 Run.Text 绑定
/// </summary>
public class BoolRunConverter : IValueConverter
{
    public static readonly BoolRunConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var param = parameter as string ?? "";
        var parts = param.Split('|');
        if (value is bool b && b)
            return parts.Length > 0 ? parts[0] : "";
        return parts.Length > 1 ? parts[1] : "";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
