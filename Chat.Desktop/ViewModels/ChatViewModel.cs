using CommunityToolkit.Mvvm.ComponentModel;

namespace Chat.Desktop.ViewModels;

public partial class ChatViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? _sendContent = "hello";

    public ChatViewModel()
    {
    }
}
