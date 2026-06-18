using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Chat.Desktop;

/// <summary>
/// Tab内容转换器 - 根据选中索引返回对应的ViewModel
/// MultiBinding参数: [0]=SelectedTabIndex, [1]=ProfileVM, [2]=ChatVM, [3]=ForumVM
/// </summary>
public class TabContentConverter : IMultiValueConverter
{
    public static readonly TabContentConverter Instance = new();

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count >= 4 && values[0] is int index)
        {
            return index switch
            {
                0 => values[1], // ProfileVM
                1 => values[2], // ChatVM
                2 => values[3], // ForumVM
                _ => values[1]
            };
        }
        return null;
    }

    public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
