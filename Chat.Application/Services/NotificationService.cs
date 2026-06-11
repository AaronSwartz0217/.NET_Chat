using Chat.Application.Dtos;
using Chat.Core.Models;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chat.Application.Services;

public class NotificationService : INotificationService
{
    private readonly ISqlSugarClient _db;

    public NotificationService(ISqlSugarClient db)
    {
        _db = db;
        _db.CodeFirst.InitTables<Notification>();
    }

    public async Task<List<NotificationDto>> GetNotificationsAsync(int userId)
    {
        var notifications = await _db.Queryable<Notification>()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedTime)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Type = (int)n.Type,
                Title = n.Title,
                Content = n.Content,
                RelatedId = n.RelatedId,
                Read = n.Read,
                CreatedTime = n.CreatedTime
            })
            .ToListAsync();

        return notifications;
    }

    public async Task<SimpleResponse> MarkAsReadAsync(int userId, int notificationId)
    {
        var result = await _db.Updateable<Notification>()
            .SetColumns(n => n.Read == true)
            .Where(n => n.Id == notificationId && n.UserId == userId)
            .ExecuteCommandAsync();

        return result > 0
            ? new SimpleResponse { Success = true, Message = "已标记为已读" }
            : new SimpleResponse { Success = false, Message = "标记失败" };
    }

    public async Task<SimpleResponse> MarkAllAsReadAsync(int userId)
    {
        await _db.Updateable<Notification>()
            .SetColumns(n => n.Read == true)
            .Where(n => n.UserId == userId && !n.Read)
            .ExecuteCommandAsync();

        return new SimpleResponse { Success = true, Message = "全部已标记为已读" };
    }

    public async Task CreateNotificationAsync(int userId, NotificationType type, string title, string content, int? relatedId = null)
    {
        await _db.Insertable(new Notification
        {
            UserId = userId,
            Type = type,
            Title = title,
            Content = content,
            RelatedId = relatedId
        }).ExecuteCommandAsync();
    }
}
