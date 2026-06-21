using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Chat.Desktop;

/// <summary>
/// 类型选择背景转换器
/// </summary>
public class TypeBgConverter : IMultiValueConverter
{
    public static readonly TypeBgConverter Instance = new();

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count >= 2 && values[0] is string selectedType && values[1] is string type)
        {
            if (selectedType == type)
            {
                return new SolidColorBrush(Color.Parse("#00A6FF")); // 选中状态 - 品牌蓝
            }
        }
        return new SolidColorBrush(Color.Parse("#6B7280")); // 默认状态 - 灰色
    }
}

/// <summary>
/// 是否有更多数据转换器
/// </summary>
public class HasMoreConverter : IValueConverter
{
    public static readonly HasMoreConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int currentPage)
        {
            return currentPage > 1; // 假设最大页数为1
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 数量可见性转换器
/// </summary>
public class CountVisibilityConverter : IValueConverter
{
    public static readonly CountVisibilityConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int count)
        {
            return count > 0;
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 是否有未读转换器
/// </summary>
public class HasUnreadConverter : IValueConverter
{
    public static readonly HasUnreadConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int count)
        {
            return count > 0;
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 通知背景转换器
/// </summary>
public class NotificationBgConverter : IMultiValueConverter
{
    public static readonly NotificationBgConverter Instance = new();

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count >= 1 && values[0] is bool isRead)
        {
            if (!isRead)
            {
                // 未读状态 - 淡蓝色背景
                return new SolidColorBrush(Color.Parse("#EFF6FF"));
            }
        }
        // 已读状态 - 默认卡片背景
        return new SolidColorBrush(Colors.Transparent);
    }
}

/// <summary>
/// 通知图标转换器
/// </summary>
public class NotificationIconConverter : IValueConverter
{
    public static readonly NotificationIconConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string icon)
        {
            return icon;
        }
        // 默认通知图标
        return "\xE7E3"; // Bell icon
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
