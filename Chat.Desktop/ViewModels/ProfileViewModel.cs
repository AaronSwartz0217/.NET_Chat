using Chat.Desktop.Models;
using Chat.Desktop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace Chat.Desktop.ViewModels;

/// <summary>
/// 个人中心ViewModel
/// 管理用户资料查看、编辑、修改密码
/// </summary>
public partial class ProfileViewModel : ViewModelBase
{
    private readonly ProfileApiService _apiService = new();

    [ObservableProperty]
    private UserProfileModel? _profile;

    [ObservableProperty]
    private string? _statusText = "个人中心";

    [ObservableProperty]
    private bool _isLoading = false;

    // ===== 编辑资料字段 =====

    [ObservableProperty]
    private string? _editNickname = string.Empty;

    [ObservableProperty]
    private string? _editSignature = string.Empty;

    [ObservableProperty]
    private bool _isEditingProfile = false;

    // ===== 学生档案编辑字段 =====
    [ObservableProperty]
    private string? _editNo = string.Empty;           // 学号
    [ObservableProperty]
    private string? _editName = string.Empty;         // 真实姓名
    [ObservableProperty]
    private string? _editIdNumber = string.Empty;
    [ObservableProperty]
    private int? _editGender = null;                   // 0=未选, 1=男, 2=女（可空）
    [ObservableProperty]
    private int? _editEthnicGroup = null;
    [ObservableProperty]
    private string? _editNativePlace = string.Empty;
    [ObservableProperty]
    private DateTime? _editBirthday = null;
    [ObservableProperty]
    private int? _editWeight = null;
    [ObservableProperty]
    private decimal? _editHeight = null;

    // ===== 修改密码字段 =====

    [ObservableProperty]
    private string? _oldPassword = string.Empty;

    [ObservableProperty]
    private string? _newPassword = string.Empty;

    [ObservableProperty]
    private string? _confirmPassword = string.Empty;

    [ObservableProperty]
    private bool _isChangingPassword = false;

    /// <summary>
    /// 是否已加载资料
    /// </summary>
    public bool HasProfile => Profile != null;

    public ProfileViewModel() { }

    /// <summary>
    /// 设置认证Token
    /// </summary>
    public void SetToken(string token)
    {
        _apiService.SetToken(token);
    }

    /// <summary>
    /// 加载用户资料
    /// </summary>
    [RelayCommand]
    public async Task LoadProfileAsync()
    {
        if (IsLoading) return;

        IsLoading = true;
        StatusText = "加载中...";

        try
        {
            var profile = await _apiService.GetCurrentUserProfileAsync();
            if (profile != null)
            {
                Profile = profile;
                EditNickname = profile.Nickname ?? "";
                EditSignature = profile.Signature ?? "";
                // 加载学生档案
                EditNo = profile.No ?? "";
                EditName = profile.Name ?? "";
                EditIdNumber = profile.IdNumber ?? "";
                EditGender = profile.Gender;
                EditEthnicGroup = profile.EthnicGroup;
                EditNativePlace = profile.NativePlace ?? "";
                EditBirthday = profile.Birthday;
                EditWeight = profile.Weight;
                EditHeight = profile.Height;
                StatusText = $"个人中心 - {profile.DisplayName}";
            }
            else
            {
                StatusText = "加载失败";
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProfileVM] 加载失败: {ex.Message}");
            StatusText = $"加载失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
            OnPropertyChanged(nameof(HasProfile));
        }
    }

    /// <summary>
    /// 开始编辑资料
    /// </summary>
    [RelayCommand]
    private void StartEditProfile()
    {
        if (Profile == null) return;
        EditNickname = Profile.Nickname ?? "";
        EditSignature = Profile.Signature ?? "";
        // 加载学生档案到编辑字段
        EditNo = Profile.No ?? "";
        EditName = Profile.Name ?? "";
        EditIdNumber = Profile.IdNumber ?? "";
        EditGender = Profile.Gender;
        EditEthnicGroup = Profile.EthnicGroup;
        EditNativePlace = Profile.NativePlace ?? "";
        EditBirthday = Profile.Birthday;
        EditWeight = Profile.Weight;
        EditHeight = Profile.Height;
        IsEditingProfile = true;
    }

    /// <summary>
    /// 取消编辑
    /// </summary>
    [RelayCommand]
    private void CancelEditProfile()
    {
        IsEditingProfile = false;
        EditNickname = Profile?.Nickname ?? "";
        EditSignature = Profile?.Signature ?? "";
        // 恢复学生档案
        EditNo = Profile?.No ?? "";
        EditName = Profile?.Name ?? "";
        EditIdNumber = Profile?.IdNumber ?? "";
        EditGender = Profile?.Gender ?? 0;
        EditEthnicGroup = Profile?.EthnicGroup ?? 0;
        EditNativePlace = Profile?.NativePlace ?? "";
        EditBirthday = Profile?.Birthday;
        EditWeight = Profile?.Weight ?? 0;
        EditHeight = Profile?.Height ?? 0;
    }

    /// <summary>
    /// 保存资料
    /// </summary>
    [RelayCommand]
    private async Task SaveProfileAsync()
    {
        if (string.IsNullOrWhiteSpace(EditNickname))
        {
            StatusText = "昵称不能为空";
            return;
        }

        IsLoading = true;
        StatusText = "保存中...";

        var success = await _apiService.UpdateProfileAsync(
            EditNickname.Trim(),
            string.IsNullOrWhiteSpace(EditSignature) ? null : EditSignature.Trim(),
            // 学生档案
            string.IsNullOrWhiteSpace(EditNo) ? null : EditNo.Trim(),
            string.IsNullOrWhiteSpace(EditName) ? null : EditName.Trim(),
            string.IsNullOrWhiteSpace(EditIdNumber) ? null : EditIdNumber.Trim(),
            EditGender,
            EditEthnicGroup,
            string.IsNullOrWhiteSpace(EditNativePlace) ? null : EditNativePlace.Trim(),
            EditBirthday,
            EditWeight,
            EditHeight);

        if (success)
        {
            if (Profile != null)
            {
                Profile.Nickname = EditNickname.Trim();
                Profile.Signature = string.IsNullOrWhiteSpace(EditSignature) ? null : EditSignature.Trim();
                // 更新学生档案
                Profile.No = EditNo?.Trim();
                Profile.Name = EditName?.Trim();
                Profile.IdNumber = EditIdNumber?.Trim();
                Profile.Gender = EditGender;
                Profile.EthnicGroup = EditEthnicGroup;
                Profile.NativePlace = EditNativePlace?.Trim();
                Profile.Birthday = EditBirthday;
                Profile.Weight = EditWeight;
                Profile.Height = EditHeight;
            }
            IsEditingProfile = false;
            StatusText = "保存成功";
            OnPropertyChanged(nameof(Profile));
        }
        else
        {
            StatusText = "保存失败";
        }

        IsLoading = false;
    }

    /// <summary>
    /// 开始修改密码
    /// </summary>
    [RelayCommand]
    private void StartChangePassword()
    {
        OldPassword = "";
        NewPassword = "";
        ConfirmPassword = "";
        IsChangingPassword = true;
    }

    /// <summary>
    /// 取消修改密码
    /// </summary>
    [RelayCommand]
    private void CancelChangePassword()
    {
        IsChangingPassword = false;
        OldPassword = "";
        NewPassword = "";
        ConfirmPassword = "";
    }

    /// <summary>
    /// 提交修改密码
    /// </summary>
    [RelayCommand]
    private async Task SubmitChangePasswordAsync()
    {
        if (string.IsNullOrWhiteSpace(OldPassword) || string.IsNullOrWhiteSpace(NewPassword))
        {
            StatusText = "请填写完整";
            return;
        }

        if (NewPassword != ConfirmPassword)
        {
            StatusText = "两次输入的密码不一致";
            return;
        }

        if (NewPassword.Length < 6)
        {
            StatusText = "新密码至少6位";
            return;
        }

        IsLoading = true;
        StatusText = "提交中...";

        var (success, message) = await _apiService.ChangePasswordAsync(OldPassword, NewPassword);

        if (success)
        {
            IsChangingPassword = false;
            OldPassword = "";
            NewPassword = "";
            ConfirmPassword = "";
            StatusText = message;
        }
        else
        {
            StatusText = message;
        }

        IsLoading = false;
    }
}
