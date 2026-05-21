using CommunityToolkit.Mvvm.Input;
using System;

namespace Chat.Desktop.ViewModels;

public partial class DataEditViewModel : ViewModelBase
{
    public int Id { get; set; }

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
