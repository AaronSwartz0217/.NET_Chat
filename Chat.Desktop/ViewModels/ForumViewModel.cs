using Avalonia.Collections;
using Chat.Desktop.Models;
using Chat.Desktop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace Chat.Desktop.ViewModels;

/// <summary>
/// 论坛ViewModel
/// 管理帖子列表、详情、创建、评论等
/// </summary>
public partial class ForumViewModel : ViewModelBase
{
    private readonly ForumApiService _apiService;

    [ObservableProperty]
    private string? _statusText = "论坛";

    /// <summary>
    /// 帖子列表
    /// </summary>
    public AvaloniaList<PostModel> Posts { get; } = [];

    /// <summary>
    /// 是否有帖子（用于控制空状态提示）
    /// </summary>
    public bool HasPosts => Posts.Count > 0;

    /// <summary>
    /// 当前选中的帖子（查看详情）
    /// </summary>
    [ObservableProperty]
    private PostModel? _selectedPost;

    /// <summary>
    /// 当前是否在查看帖子详情
    /// </summary>
    [ObservableProperty]
    private bool _isViewingDetail = false;

    // ===== 评论相关 =====

    /// <summary>
    /// 当前帖子的评论列表
    /// </summary>
    public AvaloniaList<CommentModel> Comments { get; } = [];

    [ObservableProperty]
    private string? _newCommentContent = string.Empty;

    [ObservableProperty]
    private bool _isLoadingComments = false;

    /// <summary>
    /// 是否正在加载（帖子或评论）
    /// </summary>
    public bool IsBusy => IsLoading || IsLoadingComments;

    /// <summary>
    /// 当前用户ID
    /// </summary>
    public int? CurrentUserId { get; set; }

    // ===== 创建帖子的字段 =====

    [ObservableProperty]
    private string? _newPostTitle = string.Empty;

    [ObservableProperty]
    private string? _newPostContent = string.Empty;

    [ObservableProperty]
    private bool _isCreatingPost = false;

    // ===== 分页 =====

    [ObservableProperty]
    private int _currentPage = 1;

    [ObservableProperty]
    private int _totalPages = 1;

    public bool CanPreviousPage => CurrentPage > 1;
    public bool CanNextPage => CurrentPage < TotalPages;

    [ObservableProperty]
    private bool _isLoading = false;

    // ===== 排序 =====

    [ObservableProperty]
    private string _sortBy = "time"; // time, hot

    public ForumViewModel()
    {
        _apiService = new ForumApiService();
    }

    /// <summary>
    /// 设置认证Token
    /// </summary>
    public void SetToken(string token)
    {
        _apiService.SetToken(token);
    }

    /// <summary>
    /// 加载帖子列表
    /// </summary>
    [RelayCommand]
    public async Task LoadPostsAsync()
    {
        if (IsLoading) return;

        IsLoading = true;
        StatusText = "加载中...";
        System.Diagnostics.Debug.WriteLine($"[ForumVM] 开始加载帖子: page={CurrentPage}");

        try
        {
            var (posts, totalCount, totalPages) = await _apiService.GetPostsAsync(
                CurrentPage, 10, SortBy);

            System.Diagnostics.Debug.WriteLine($"[ForumVM] API返回: posts={posts?.Count ?? 0}, total={totalCount}, pages={totalPages}");

            if (posts != null)
            {
                Posts.Clear();
                foreach (var post in posts)
                {
                    Posts.Add(post);
                }
                TotalPages = totalPages;
            }

            OnPropertyChanged(nameof(HasPosts));
            StatusText = posts != null ? $"论坛 ({totalCount} 篇帖子)" : "暂无帖子";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ForumVM] 加载失败: {ex.Message}");
            StatusText = $"加载失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
            System.Diagnostics.Debug.WriteLine($"[ForumVM] 加载完成, IsLoading={IsLoading}");
        }
    }

    /// <summary>
    /// 刷新帖子列表
    /// </summary>
    [RelayCommand]
    private async Task RefreshAsync()
    {
        CurrentPage = 1;
        await LoadPostsAsync();
    }

    /// <summary>
    /// 上一页
    /// </summary>
    [RelayCommand]
    private async Task PreviousPageAsync()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            await LoadPostsAsync();
        }
    }

    /// <summary>
    /// 下一页
    /// </summary>
    [RelayCommand]
    private async Task NextPageAsync()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
            await LoadPostsAsync();
        }
    }

    /// <summary>
    /// 查看帖子详情（含评论）
    /// </summary>
    [RelayCommand]
    private async Task ViewPostAsync(PostModel? post)
    {
        if (post == null) return;

        IsLoading = true;
        var detail = await _apiService.GetPostAsync(post.Id);
        if (detail != null)
        {
            SelectedPost = detail;
            IsViewingDetail = true;
            NewCommentContent = string.Empty;
            // 自动加载评论
            await LoadCommentsAsync();
        }
        IsLoading = false;
    }

    /// <summary>
    /// 返回列表
    /// </summary>
    [RelayCommand]
    private void BackToList()
    {
        SelectedPost = null;
        IsViewingDetail = false;
        Comments.Clear();
    }

    /// <summary>
    /// 删除帖子
    /// </summary>
    [RelayCommand]
    private async Task DeletePostAsync(PostModel? post)
    {
        if (post == null) return;

        var success = await _apiService.DeletePostAsync(post.Id);
        if (success)
        {
            Posts.Remove(post);
            OnPropertyChanged(nameof(HasPosts));
            StatusText = "删除成功";
        }
        else
        {
            StatusText = "删除失败";
        }
    }

    /// <summary>
    /// 点赞/取消点赞
    /// </summary>
    [RelayCommand]
    private async Task ToggleLikeAsync(PostModel? post)
    {
        if (post == null) return;

        var success = await _apiService.ToggleLikeAsync(post.Id, post.IsLiked);
        if (success)
        {
            post.IsLiked = !post.IsLiked;
            post.LikeCount += post.IsLiked ? 1 : -1;
        }
    }

    /// <summary>
    /// 收藏/取消收藏
    /// </summary>
    [RelayCommand]
    private async Task ToggleFavoriteAsync(PostModel? post)
    {
        if (post == null) return;

        var success = await _apiService.ToggleFavoriteAsync(post.Id, post.IsFavorited);
        if (success)
        {
            post.IsFavorited = !post.IsFavorited;
        }
    }

    // ===== 评论功能 =====

    /// <summary>
    /// 加载当前帖子的评论
    /// </summary>
    [RelayCommand]
    private async Task LoadCommentsAsync()
    {
        if (SelectedPost == null || IsLoadingComments) return;

        IsLoadingComments = true;
        try
        {
            var (comments, _, _) = await _apiService.GetCommentsAsync(SelectedPost.Id);
            Comments.Clear();
            if (comments != null)
            {
                foreach (var c in comments)
                    Comments.Add(c);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ForumVM] 加载评论失败: {ex.Message}");
        }
        finally
        {
            IsLoadingComments = false;
        }
    }

    /// <summary>
    /// 发表评论
    /// </summary>
    [RelayCommand]
    private async Task SubmitCommentAsync()
    {
        if (SelectedPost == null || string.IsNullOrWhiteSpace(NewCommentContent)) return;

        var success = await _apiService.AddCommentAsync(SelectedPost.Id, NewCommentContent.Trim());
        if (success)
        {
            NewCommentContent = string.Empty;
            SelectedPost.CommentCount++;
            await LoadCommentsAsync();
        }
        else
        {
            StatusText = "评论发表失败";
        }
    }

    /// <summary>
    /// 删除评论
    /// </summary>
    [RelayCommand]
    private async Task DeleteCommentAsync(CommentModel? comment)
    {
        if (comment == null) return;

        var success = await _apiService.DeleteCommentAsync(comment.Id);
        if (success)
        {
            Comments.Remove(comment);
            if (SelectedPost != null && SelectedPost.CommentCount > 0)
                SelectedPost.CommentCount--;
        }
    }

    // ===== 发帖功能 =====

    /// <summary>
    /// 显示创建帖子界面
    /// </summary>
    [RelayCommand]
    private void ShowCreatePost()
    {
        NewPostTitle = string.Empty;
        NewPostContent = string.Empty;
        IsCreatingPost = true;
    }

    /// <summary>
    /// 取消创建帖子
    /// </summary>
    [RelayCommand]
    private void CancelCreatePost()
    {
        IsCreatingPost = false;
        NewPostTitle = string.Empty;
        NewPostContent = string.Empty;
    }

    /// <summary>
    /// 提交创建帖子
    /// </summary>
    [RelayCommand]
    private async Task SubmitCreatePostAsync()
    {
        if (string.IsNullOrWhiteSpace(NewPostTitle) || string.IsNullOrWhiteSpace(NewPostContent))
            return;

        StatusText = "发布中...";

        var post = await _apiService.CreatePostAsync(NewPostTitle.Trim(), NewPostContent.Trim());
        if (post != null)
        {
            // 发布成功，刷新列表
            IsCreatingPost = false;
            NewPostTitle = string.Empty;
            NewPostContent = string.Empty;
            await RefreshAsync();
        }
        else
        {
            StatusText = "发布失败";
        }
    }

    /// <summary>
    /// 按时间排序
    /// </summary>
    [RelayCommand]
    private async Task SortByTimeAsync()
    {
        SortBy = "time";
        await RefreshAsync();
    }

    /// <summary>
    /// 按热度排序
    /// </summary>
    [RelayCommand]
    private async Task SortByHotAsync()
    {
        SortBy = "hot";
        await RefreshAsync();
    }

    // ===== 属性变化通知 =====

    partial void OnIsLoadingChanged(bool value) => OnPropertyChanged(nameof(IsBusy));
    partial void OnIsLoadingCommentsChanged(bool value) => OnPropertyChanged(nameof(IsBusy));
    partial void OnCurrentPageChanged(int value)
    {
        OnPropertyChanged(nameof(CanPreviousPage));
        OnPropertyChanged(nameof(CanNextPage));
    }
    partial void OnTotalPagesChanged(int value)
    {
        OnPropertyChanged(nameof(CanPreviousPage));
        OnPropertyChanged(nameof(CanNextPage));
    }
}
