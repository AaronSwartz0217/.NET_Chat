using Chat.Application.Dtos;
using Chat.Core.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Application.Services;

public class ChannelService : IChannelService
{
    private readonly ISqlSugarClient _db;

    public ChannelService(ISqlSugarClient db)
    {
        _db = db;
        _db.CodeFirst.InitTables<Channel>();
        _db.CodeFirst.InitTables<ChannelMember>();
        _db.CodeFirst.InitTables<Message>();
    }

    public async Task<List<ChannelDto>> GetChannelsAsync(int userId)
    {
        var memberChannels = await _db.Queryable<ChannelMember>()
            .Where(cm => cm.UserId == userId)
            .Select(cm => cm.ChannelId)
            .ToListAsync();

        var channels = await _db.Queryable<Channel>()
            .Where(c => memberChannels.Contains(c.Id))
            .ToListAsync();

        var result = new List<ChannelDto>();
        foreach (var channel in channels)
        {
            var lastMessage = await _db.Queryable<Message>()
                .Where(m => m.ChannelId == channel.Id && !m.Recalled)
                .OrderByDescending(m => m.CreatedTime)
                .FirstAsync();

            var member = await _db.Queryable<ChannelMember>()
                .Where(cm => cm.ChannelId == channel.Id && cm.UserId == userId)
                .FirstAsync();

            var unreadCount = await _db.Queryable<Message>()
                .Where(m => m.ChannelId == channel.Id && !m.Recalled)
                .Where(m => member.LastReadTime == null || m.CreatedTime > member.LastReadTime)
                .CountAsync();

            string? channelName = null;
            string? channelAvatar = null;

            if (channel.Type == ChannelType.Private)
            {
                var otherMember = await _db.Queryable<ChannelMember>()
                    .Where(cm => cm.ChannelId == channel.Id && cm.UserId != userId)
                    .FirstAsync();

                if (otherMember != null)
                {
                    var otherUser = await _db.Queryable<User>()
                        .FirstAsync(u => u.Id == otherMember.UserId);
                    channelName = otherUser?.Nickname ?? otherUser?.UserName;
                    channelAvatar = otherUser?.Avatar;
                }
            }
            else
            {
                channelName = channel.Name;
                channelAvatar = channel.Avatar;
            }

            result.Add(new ChannelDto
            {
                Id = channel.Id,
                Type = (int)channel.Type,
                Name = channelName,
                Avatar = channelAvatar,
                LastMessage = lastMessage?.Content,
                LastMessageTime = lastMessage?.CreatedTime,
                UnreadCount = unreadCount,
                CreatedTime = channel.CreatedTime
            });
        }

        return result.OrderByDescending(c => c.LastMessageTime).ToList();
    }

    public async Task<ChannelDto?> CreateChannelAsync(int userId, CreateChannelRequest request)
    {
        var channel = new Channel
        {
            Type = (ChannelType)request.Type,
            Name = request.Name
        };

        var channelId = await _db.Insertable(channel).ExecuteReturnIdentityAsync();

        await _db.Insertable(new ChannelMember
        {
            ChannelId = (int)channelId,
            UserId = userId,
            Role = "admin"
        }).ExecuteCommandAsync();

        if (request.MemberIds != null)
        {
            foreach (var memberId in request.MemberIds)
            {
                if (memberId != userId)
                {
                    await _db.Insertable(new ChannelMember
                    {
                        ChannelId = (int)channelId,
                        UserId = memberId,
                        Role = "member"
                    }).ExecuteCommandAsync();
                }
            }
        }

        return new ChannelDto
        {
            Id = (int)channelId,
            Type = request.Type,
            Name = request.Name,
            CreatedTime = DateTime.UtcNow
        };
    }

    public async Task<PaginatedResponse<MessageDto>> GetMessagesAsync(int channelId, int userId, int pageIndex = 1, int pageSize = 50)
    {
        var isMember = await _db.Queryable<ChannelMember>()
            .AnyAsync(cm => cm.ChannelId == channelId && cm.UserId == userId);

        if (!isMember)
        {
            return new PaginatedResponse<MessageDto>();
        }

        var query = _db.Queryable<Message>()
            .Where(m => m.ChannelId == channelId && !m.Recalled)
            .OrderByDescending(m => m.CreatedTime)
            .LeftJoin<User>((m, u) => m.UserId == u.Id)
            .Select((m, u) => new MessageDto
            {
                Id = m.Id,
                ChannelId = m.ChannelId,
                UserId = m.UserId,
                UserName = u.UserName,
                Avatar = u.Avatar,
                Content = m.Content,
                Type = (int)m.Type,
                Recalled = m.Recalled,
                ReplyTo = m.ReplyTo,
                CreatedTime = m.CreatedTime
            });

        var totalCount = await query.CountAsync();
        var data = await query.ToPageListAsync(pageIndex, pageSize);

        return new PaginatedResponse<MessageDto>
        {
            Data = data,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<MessageDto?> SendMessageAsync(int channelId, int userId, SendMessageRequest request)
    {
        var isMember = await _db.Queryable<ChannelMember>()
            .AnyAsync(cm => cm.ChannelId == channelId && cm.UserId == userId);

        if (!isMember)
        {
            return null;
        }

        var message = new Message
        {
            ChannelId = channelId,
            UserId = userId,
            Content = request.Content,
            Type = (MessageType)request.Type,
            ReplyTo = request.ReplyTo
        };

        var messageId = await _db.Insertable(message).ExecuteReturnIdentityAsync();

        var user = await _db.Queryable<User>().FirstAsync(u => u.Id == userId);
        return new MessageDto
        {
            Id = (int)messageId,
            ChannelId = channelId,
            UserId = userId,
            UserName = user.UserName,
            Avatar = user.Avatar,
            Content = request.Content,
            Type = request.Type,
            Recalled = false,
            ReplyTo = request.ReplyTo,
            CreatedTime = DateTime.UtcNow
        };
    }

    public async Task<SimpleResponse> RecallMessageAsync(int messageId, int userId)
    {
        var message = await _db.Queryable<Message>().FirstAsync(m => m.Id == messageId);
        if (message == null)
        {
            return new SimpleResponse { Success = false, Message = "消息不存在" };
        }
        if (message.UserId != userId)
        {
            return new SimpleResponse { Success = false, Message = "无权撤回此消息" };
        }

        var timeDiff = DateTime.UtcNow - message.CreatedTime;
        if (timeDiff.TotalMinutes > 2)
        {
            return new SimpleResponse { Success = false, Message = "超过2分钟无法撤回" };
        }

        await _db.Updateable<Message>()
            .SetColumns(m => m.Recalled == true)
            .Where(m => m.Id == messageId)
            .ExecuteCommandAsync();

        return new SimpleResponse { Success = true, Message = "撤回成功" };
    }

    public async Task<SimpleResponse> MarkAsReadAsync(int channelId, int userId)
    {
        var result = await _db.Updateable<ChannelMember>()
            .SetColumns(cm => cm.LastReadTime == DateTime.UtcNow)
            .Where(cm => cm.ChannelId == channelId && cm.UserId == userId)
            .ExecuteCommandAsync();

        return result > 0
            ? new SimpleResponse { Success = true, Message = "已标记为已读" }
            : new SimpleResponse { Success = false, Message = "标记失败" };
    }
}
