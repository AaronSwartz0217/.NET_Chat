using CommunityToolkit.Mvvm.ComponentModel;

namespace Chat.Desktop.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string greeting = "欢迎进入聊天室";
    }
}
