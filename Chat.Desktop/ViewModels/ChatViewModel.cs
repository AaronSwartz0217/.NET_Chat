using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Chat.Desktop.ViewModels;

public partial class ChatViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? _sendContent;

    [ObservableProperty]
    private ObservableCollection<MessageItem> _messageList = new();

    public ChatViewModel()
    {
        MessageList.Add(new MessageItem { Content = "欢迎进入聊天室！", IsMe = false });
    }

    [RelayCommand]
    public void Send()
    {
        if (!string.IsNullOrWhiteSpace(SendContent))
        {
            MessageList.Add(new MessageItem { Content = SendContent, IsMe = true });
            SendContent = string.Empty;
        }
    }
}

public class MessageItem
{
    public string Content { get; set; } = string.Empty;
    public bool IsMe { get; set; }
}