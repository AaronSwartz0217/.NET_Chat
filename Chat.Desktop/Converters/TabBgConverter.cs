using System;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using System.Globalization;

namespace Chat.Desktop;

/// <summary>
/// Tab背景色转换器
/// 参数为目标索引，选中时返回选中色，否则透明
/// </summary>
public class TabBgConverter : IValueConverter
{
    public static readonly TabBgConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int current && parameter is string s && int.TryParse(s, out int target))
        {
            return current == target
                ? new SolidColorBrush(Color.Parse("#00A6FF"))  // 选中：QQ蓝
                : Brushes.Transparent;                        // 未选中：透明
        }
        return Brushes.Transparent;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
