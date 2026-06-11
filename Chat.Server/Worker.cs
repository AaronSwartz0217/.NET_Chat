using Chat.Core.Models;
using Microsoft.Extensions.Logging;
using SqlSugar;
using TouchSocket.Http;

namespace Chat.Server;

public class Worker : BackgroundService
{
    private readonly HttpService _httpService;
    private readonly ILogger<Worker> _logger;
    private readonly ISqlSugarClient _db;

    public Worker(ILogger<Worker> logger, HttpService httpService, ISqlSugarClient db)
    {
        _logger = logger;
        _httpService = httpService;
        _db = db;
    }

    /// <summary>
    /// 服务启动
    /// </summary>
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            // 初始化管理员账户
            await InitializeAdminAccount();

            // 启动HttpService（WebSocket专用服务器）
            await _httpService.StartAsync();
            _logger.LogInformation("服务已启动 - REST API:5002 | WebSocket:5003");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "启动服务失败");
        }

        await base.StartAsync(cancellationToken);
    }

    /// <summary>
    /// 初始化管理员账户
    /// </summary>
    private async Task InitializeAdminAccount()
    {
        try
        {
            var adminUser = await _db.Queryable<User>()
                .FirstAsync(u => u.UserName == "admin");

            if (adminUser == null)
            {
                var user = new User
                {
                    UserName = "admin",
                    Password = "123456",
                    Nickname = "管理员",
                    Role = "admin",
                    LastLoginTime = DateTime.UtcNow,
                    CreatedTime = DateTime.UtcNow
                };

                await _db.Insertable(user).ExecuteCommandAsync();
                _logger.LogInformation("管理员账户已创建: admin / 123456");
            }
            else
            {
                _logger.LogInformation("管理员账户已存在");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "初始化管理员账户失败");
        }
    }

    /// <summary>
    /// 服务停止
    /// </summary>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _httpService.StopAsync();
            _logger.LogInformation("服务已停止");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "停止服务失败");
        }

        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information))
            {
                _logger.LogInformation("[Worker] 运行中 | 在线用户: {Count} | 时间: {time}",
                    ChatWebSocketPlugin.GetOnlineCount(), DateTimeOffset.Now);
            }
            await Task.Delay(10000, stoppingToken);
        }
    }
}
