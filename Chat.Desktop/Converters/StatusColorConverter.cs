using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace Chat.Desktop;

/// <summary>
/// 状态消息颜色转换器
/// </summary>
public class StatusColorConverter : IValueConverter
{
    public static readonly StatusColorConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSuccess)
        {
            return isSuccess 
                ? new SolidColorBrush(Color.Parse("#10B981")) // 绿色
                : new SolidColorBrush(Color.Parse("#EF4444")); // 红色
        }
        return new SolidColorBrush(Color.Parse("#6B7280")); // 默认灰色
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
