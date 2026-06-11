using Chat.Application.Services;
using Chat.Core;
using Chat.Server;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TouchSocket.Core;
using TouchSocket.Http;

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
    var wsConfig = new TouchSocketConfig()
        .SetListenIPHosts(new IPHost[] { new IPHost(5003) })  // WS专用端口
        .ConfigureContainer(a =>
        {
            a.AddConsoleLogger();
        })
        .ConfigurePlugins(a =>
        {
            a.UseWebSocket();           // 启用WebSocket协议
            a.Add<ChatWebSocketPlugin>(); // 添加聊天插件
        });

    var httpService = new HttpService();
    httpService.Setup(wsConfig);
    services.AddSingleton(httpService);

    services.AddHostedService<Worker>();

}, urls: "http://0.0.0.0:5002");  // REST API端口不变
