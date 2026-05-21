using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace Chat.Desktop.ViewModels;

public partial class LoginWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [RelayCommand]
    private void OkClick()
    {
        RequestClose?.Invoke(this, true);
    }

    [RelayCommand]
    private void CancelClick()
    {
        RequestClose?.Invoke(this, false);
    }

    public event EventHandler<object?>? RequestClose;
    public void Close()
    {
        RequestClose?.Invoke(this, false);
    }
}
