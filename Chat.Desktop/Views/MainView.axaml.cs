using Avalonia.Controls;

namespace Chat.Desktop;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        // DataContext 由 MainWindow / App.axaml.cs 设置
    }
}
