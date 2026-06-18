using Chat.Application.Services;
using Chat.Core;
using Chat.Server;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TouchSocket.Core;
using TouchSocket.Http;

#pragma warning disable CodeAnalysis0001 // 禁用Setup同步方法警告

await Serve.RunAsync(services =>
{
    services.AddMySqlSetup();

    // 旧服务（兼容）
    services.AddTransient<IUserService, UserService>();
    services.AddTransient<IStudentService, StudentService>();
    services.AddTransient<IJwtService, JwtService>();
    services.AddTransient<JwtSettings>();
    services.AddTransient<IAccountService, AccountService>();
    services.AddTransient<AccountService>();

    // 新服务
    services.AddTransient<IAuthService, AuthService>();
    services.AddTransient<IUserServiceV2, UserServiceV2>();
    services.AddTransient<IPostService, PostService>();
    services.AddTransient<ICommentService, CommentService>();
    services.AddTransient<IChannelService, ChannelService>();
    services.AddTransient<INotificationService, NotificationService>();
    services.AddTransient<ISearchService, SearchService>();

    // JWT配置
    var jwtSettings = new JwtSettings();
    services.AddSingleton(jwtSettings);

    // 添加JWT认证
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
            };
        });

    services.AddAuthorization();

    // 配置Kestrel服务器超时设置
    services.Configure<KestrelServerOptions>(options =>
    {
        options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(5);
        options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5);
    });

    // WebSocket配置（TouchSocket独立服务器，监听5003端口）
    var httpService = new HttpService();
    var wsConfig = new TouchSocketConfig()
        .SetListenIPHosts(5003)  // 监听 0.0.0.0:5003
        .ConfigureContainer(a =>
        {
            a.AddConsoleLogger();
        })
        .ConfigurePlugins(a =>
        {
            // TouchSocket 4.0: 必须使用 options 设置 WS 路径
            a.UseWebSocket(options =>
            {
                options.SetUrl("/");           // 接受根路径的 WS 连接
                options.SetAutoPong(true);      // 自动响应 Ping
            });
            a.Add<ChatWebSocketPlugin>();       // 添加聊天插件
        });

    httpService.Setup(wsConfig);  // 同步Setup（在4.x版本中已优化）
    services.AddSingleton(httpService);

    services.AddHostedService<Worker>();

}, urls: "http://0.0.0.0:5002");  // REST API端口不变
