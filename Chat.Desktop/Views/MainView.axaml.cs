using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chat.Desktop.ViewModels;

namespace Chat.Desktop;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        this.DataContext = new MainViewModel();
    }
}
