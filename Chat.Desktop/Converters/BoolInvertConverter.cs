using System;
using Avalonia.Data.Converters;
using System.Globalization;

namespace Chat.Desktop;

/// <summary>
/// 布尔值取反转换器
/// </summary>
public class BoolInvertConverter : IValueConverter
{
    public static readonly BoolInvertConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b) return !b;
        return true;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
