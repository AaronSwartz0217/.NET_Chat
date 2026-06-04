using Avalonia.Media;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Chat.Desktop.Converters;

public class BooleanToBrushConverter : IValueConverter
{
    public static readonly BooleanToBrushConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true 
            ? new SolidColorBrush(Color.Parse("#007AFF")) 
            : new SolidColorBrush(Color.Parse("#E5E5EA"));
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}