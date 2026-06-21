using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Chat.Desktop.Models;
using Chat.Desktop.Services;

namespace Chat.Desktop.ViewModels;

/// <summary>
/// 搜索视图ViewModel
/// </summary>
public partial class SearchViewModel : ViewModelBase
{
    private readonly SearchApiService _searchService;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private string _selectedType = "posts";

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private bool _hasSearched = false;

    [ObservableProperty]
    private bool _hasResults = false;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private int _totalResults = 0;

    [ObservableProperty]
    private int _currentPage = 1;

    [ObservableProperty]
    private int _totalPages = 1;

    /// <summary>
    /// 是否有更多结果可加载
    /// </summary>
    public bool HasMoreResults => HasResults && CurrentPage < TotalPages;

    /// <summary>
    /// 是否无结果（已搜索但没有结果）
    /// </summary>
    public bool HasNoResults => HasSearched && !HasResults && !IsLoading;

    public ObservableCollection<PostModel> SearchResults { get; } = new();

    public ObservableCollection<string> SearchTypes { get; } = new()
    {
        { "posts" },
        { "users" }
    };

    private string? _accessToken;

    public SearchViewModel()
    {
        _searchService = new SearchApiService();
    }

    /// <summary>
    /// 设置认证Token
    /// </summary>
    public void SetToken(string token)
    {
        _accessToken = token;
        _searchService.SetToken(token);
    }

    /// <summary>
    /// 执行搜索
    /// </summary>
    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            StatusMessage = "请输入搜索关键词";
            return;
        }

        IsLoading = true;
        HasSearched = true;
        SearchResults.Clear();
        CurrentPage = 1;

        try
        {
            var result = await _searchService.SearchAsync(SearchQuery, SelectedType, 1, 20);

            if (result != null)
            {
                TotalResults = result.TotalCount;
                TotalPages = result.TotalPages;
                HasResults = result.Posts?.Count > 0;

                if (result.Posts != null)
                {
                    foreach (var post in result.Posts)
                    {
                        SearchResults.Add(post);
                    }
                }

                if (TotalResults > 0)
                {
                    StatusMessage = $"找到 {TotalResults} 条结果";
                }
                else
                {
                    StatusMessage = "未找到相关结果";
                }
            }
            else
            {
                StatusMessage = "搜索失败，请稍后重试";
                HasResults = false;
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"搜索失败：{ex.Message}";
            System.Diagnostics.Debug.WriteLine($"[Search] 搜索异常: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// 加载更多结果
    /// </summary>
    [RelayCommand]
    private async Task LoadMoreAsync()
    {
        if (IsLoading || CurrentPage >= TotalPages)
            return;

        IsLoading = true;
        CurrentPage++;

        try
        {
            var result = await _searchService.SearchAsync(SearchQuery, SelectedType, CurrentPage, 20);

            if (result?.Posts != null)
            {
                foreach (var post in result.Posts)
                {
                    SearchResults.Add(post);
                }
                StatusMessage = $"已加载 {SearchResults.Count}/{TotalResults} 条结果";
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Search] 加载更多异常: {ex.Message}");
            CurrentPage--; // 回退页码
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// 清除搜索
    /// </summary>
    [RelayCommand]
    private void ClearSearch()
    {
        SearchQuery = string.Empty;
        SearchResults.Clear();
        HasSearched = false;
        HasResults = false;
        StatusMessage = string.Empty;
        TotalResults = 0;
        CurrentPage = 1;
        TotalPages = 1;
    }

    /// <summary>
    /// 切换搜索类型
    /// </summary>
    [RelayCommand]
    private async Task SwitchTypeAsync(string type)
    {
        if (SelectedType == type)
            return;

        SelectedType = type;
        SearchResults.Clear();

        if (HasSearched)
        {
            await SearchAsync();
        }
    }
}
