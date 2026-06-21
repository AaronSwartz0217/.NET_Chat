using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Chat.Desktop;

/// <summary>
/// Tab内容转换器 - 根据选中索引返回对应的ViewModel
/// MultiBinding参数: [0]=SelectedTabIndex, [1]=SearchVM, [2]=NotificationVM, [3]=ProfileVM, [4]=ChatVM, [5]=ForumVM
/// 索引: 0=搜索, 1=通知, 2=我的, 3=聊天室, 4=论坛
/// </summary>
public class TabContentConverter : IMultiValueConverter
{
    public static readonly TabContentConverter Instance = new();

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count >= 5 && values[0] is int index)
        {
            return index switch
            {
                0 => values[1], // SearchVM
                1 => values[2], // NotificationVM
                2 => values[3], // ProfileVM
                3 => values[4], // ChatVM
                4 => values[5], // ForumVM
                _ => values[1]  // 默认搜索
            };
        }
        return null;
    }

    public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
