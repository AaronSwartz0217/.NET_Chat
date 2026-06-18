using System;
using Avalonia.Data.Converters;
using System.Globalization;

namespace Chat.Desktop;

/// <summary>
/// 整数大于零转换器
/// </summary>
public class IntGreaterThanZeroConverter : IValueConverter
{
    public static readonly IntGreaterThanZeroConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is int i && i > 0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
