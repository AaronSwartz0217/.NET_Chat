using CommunityToolkit.Mvvm.ComponentModel;

namespace Chat.Desktop.Models;

public class NewsModel : ObservableObject
{
    public int Index { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Source { get; set; } = "腾讯新闻";
    public string Category { get; set; } = string.Empty;
    public string HotValue { get; set; } = string.Empty;
}
