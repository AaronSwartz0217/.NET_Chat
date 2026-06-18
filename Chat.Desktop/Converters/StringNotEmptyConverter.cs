using System;
using Avalonia.Data.Converters;
using System.Globalization;

namespace Chat.Desktop;

/// <summary>
/// 字符串非空转换器
/// </summary>
public class StringNotEmptyConverter : IValueConverter
{
    public static readonly StringNotEmptyConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return !string.IsNullOrWhiteSpace(value as string);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
