# Chat_v 项目架构文档

> **版本：** v1.0.0
> **更新时间：** 2026-06-22
> **项目状态：** 后端完成 + 前端完成

---

## 📋 目录

- [项目概述](#项目概述)
- [技术栈](#技术栈)
- [架构设计](#架构设计)
- [模块详解](#模块详解)
- [服务器设计](#服务器设计)
- [通信协议](#通信协议)
- [数据库设计](#数据库设计)
- [部署架构](#部署架构)

---

## 项目概述

Chat_v 是一个基于 .NET 9.0 的实时聊天系统，采用前后端分离架构，支持多端客户端（Desktop、Android、Web）。系统提供完整的用户认证、实时聊天、论坛功能、学生数据管理等功能。

### 核心特性

- ✅ **实时通信**：基于 WebSocket 的高性能实时消息传输
- ✅ **JWT 认证**：安全的用户身份验证和授权
- ✅ **多端支持**：Windows Desktop、Linux Desktop、Android、Web
- ✅ **论坛系统**：帖子发布、评论、点赞等功能
- ✅ **学生管理**：完整的学生信息 CRUD 操作
- ✅ **搜索功能**：全文搜索支持
- ✅ **断线重连**：自动重连机制，保证连接稳定性

---

## 技术栈

### 后端技术栈

| 技术 | 版本 | 用途 |
|------|------|------|
| .NET | 9.0 | 运行时框架 |
| Furion | 5.0.0-preview.1.20240813.1 | 应用框架（API标准格式、依赖注入） |
| SqlSugarCore | 5.1.4.142 | ORM 数据库操作 |
| MySqlConnector | 2.3.2 | MySQL 数据库驱动 |
| TouchSocket | 4.0.1 | WebSocket 服务端 |
| Microsoft.AspNetCore.Authentication.JwtBearer | 9.0.0 | JWT 认证 |
| System.IdentityModel.Tokens.Jwt | 8.0.1 | JWT Token 处理 |
| Microsoft.Extensions.Hosting | 10.0.5 | 后台服务框架 |

### 前端技术栈（Desktop）

| 技术 | 版本 | 用途 |
|------|------|------|
| Avalonia | 12.0.2 | 跨平台 UI 框架 |
| Avalonia.Themes.Fluent | 12.0.2 | Fluent 设计主题 |
| CommunityToolkit.Mvvm | 8.4.1 | MVVM 框架 |
| Semi.Avalonia | 12.0.1 | Semi Design 主题 |
| Irihi.Ursa | 2.0.0 | UI 组件库 |
| TouchSocket.Http | 2.0.2 | HTTP 客户端 |
| Mapster | 7.5.0 | 对象映射 |

### 数据库

- **MySQL 8.x**：关系型数据库，存储用户、消息、帖子等数据

---

## 架构设计

### 整体架构

```
┌─────────────────────────────────────────────────────────────────┐
│                         客户端层 (Client Layer)                    │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐           │
│  │  Desktop     │  │   Android    │  │     Web      │           │
│  │  (Avalonia)  │  │  (Kotlin)    │  │   (Blazor)   │           │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘           │
└─────────┼─────────────────┼─────────────────┼───────────────────┘
          │                 │                 │
          └─────────────────┼─────────────────┘
                            │
          ┌─────────────────┼─────────────────┐
          ↓                 ↓                 ↓
    ┌──────────┐      ┌──────────┐      ┌──────────┐
    │   HTTP   │      │WebSocket │      │  HTTPS   │
    │  :5002   │      │  :5003   │      │  :443    │
    └────┬─────┘      └────┬─────┘      └────┬─────┘
         │                 │                 │
         └─────────────────┼─────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────────────┐
│                      服务端层 (Server Layer)                      │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │                    Chat.Server                            │   │
│  │  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │   │
│  │  │   REST API   │  │   WebSocket  │  │  JWT Auth    │  │   │
│  │  │  Controller  │  │   Plugin     │  │  Middleware  │  │   │
│  │  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘  │   │
│  └─────────┼─────────────────┼─────────────────┼───────────┘   │
└────────────┼─────────────────┼─────────────────┼───────────────┘
             │                 │                 │
             └─────────────────┼─────────────────┘
                               ↓
┌─────────────────────────────────────────────────────────────────┐
│                   应用层 (Application Layer)                     │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐           │
│  │  AuthService │  │UserService   │  │PostService   │           │
│  └──────────────┘  └──────────────┘  └──────────────┘           │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐           │
│  │ChannelService│  │CommentService│  │SearchService │           │
│  └──────────────┘  └──────────────┘  └──────────────┘           │
└─────────────────────────────────────────────────────────────────┘
                               ↓
┌─────────────────────────────────────────────────────────────────┐
│                   数据访问层 (Data Access Layer)                  │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │                    SqlSugar ORM                           │   │
│  │  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │   │
│  │  │   User Repo  │  │  Post Repo   │  │ Message Repo │  │   │
│  │  └──────────────┘  └──────────────┘  └──────────────┘  │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                               ↓
┌─────────────────────────────────────────────────────────────────┐
│                      数据层 (Data Layer)                         │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │                    MySQL Database                         │   │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐  │   │
│  │  │  users   │ │  posts   │ │messages  │ │channels  │  │   │
│  │  └──────────┘ └──────────┘ └──────────┘ └──────────┘  │   │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐  │   │
│  │  │comments  │ │ students │ │  forums  │ │ accounts │  │   │
│  │  └──────────┘ └──────────┘ └──────────┘ └──────────┘  │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

### 分层架构说明

#### 1. 客户端层 (Client Layer)
- **Desktop 客户端**：基于 Avalonia 的跨平台桌面应用
- **Android 客户端**：基于 Kotlin 的移动应用
- **Web 客户端**：基于 Blazor 的 Web 应用（规划中）

#### 2. 服务端层 (Server Layer)
- **REST API**：提供标准的 HTTP/JSON 接口
- **WebSocket**：提供实时双向通信
- **JWT 认证**：统一的身份认证和授权

#### 3. 应用层 (Application Layer)
- **业务逻辑服务**：处理具体的业务逻辑
- **DTO 映射**：使用 Mapster 进行对象映射
- **服务接口**：定义服务契约

#### 4. 数据访问层 (Data Access Layer)
- **SqlSugar ORM**：提供数据库操作抽象
- **仓储模式**：封装数据访问逻辑

#### 5. 数据层 (Data Layer)
- **MySQL 数据库**：持久化存储所有数据

---

## 模块详解

### 1. Chat.Core（核心模块）

**职责：** 定义核心实体、数据模型和基础配置

**主要组件：**

```
Chat.Core/
├── Models/                    # 数据模型
│   ├── User.cs               # 用户实体
│   ├── Students.cs           # 学生实体
│   ├── ForumModels.cs        # 论坛相关实体
│   └── JwtSettings.cs        # JWT 配置
├── DbContext.cs              # 数据库上下文
└── Chat.Core.csproj          # 项目文件
```

**核心实体：**

```csharp
// 用户实体
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Nickname { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; }
}

// 学生实体
public class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string StudentId { get; set; }
    public string Class { get; set; }
    public string Major { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}

// 帖子实体
public class Post
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

**技术特点：**
- 使用 SqlSugar 的 `[SugarTable]` 特性映射数据库表
- 使用 `[SugarColumn]` 特性配置列属性
- 支持数据库自动迁移

---

### 2. Chat.Application（应用模块）

**职责：** 实现业务逻辑和服务接口

**主要组件：**

```
Chat.Application/
├── Services/                  # 业务服务
│   ├── AuthService.cs        # 认证服务
│   ├── UserService.cs        # 用户服务
│   ├── PostService.cs        # 帖子服务
│   ├── CommentService.cs     # 评论服务
│   ├── ChannelService.cs     # 频道服务
│   ├── StudentService.cs     # 学生服务
│   ├── SearchService.cs      # 搜索服务
│   └── JwtService.cs         # JWT 服务
├── Dtos/                      # 数据传输对象
│   ├── UserDto.cs            # 用户 DTO
│   ├── PostDto.cs            # 帖子 DTO
│   ├── StudentDto.cs         # 学生 DTO
│   ├── ForumDto.cs           # 论坛 DTO
│   └── WsMessageDto.cs       # WebSocket 消息 DTO
├── Mapper.cs                  # Mapster 映射配置
└── Chat.Application.csproj    # 项目文件
```

**核心服务：**

#### AuthService（认证服务）
```csharp
public interface IAuthService
{
    Task<string> LoginAsync(string username, string password);
    Task<User> RegisterAsync(RegisterDto dto);
    Task<User> ValidateTokenAsync(string token);
}

// 功能：
// - 用户登录（验证密码，生成 JWT Token）
// - 用户注册（创建新用户）
// - Token 验证（解析和验证 JWT）
```

#### UserService（用户服务）
```csharp
public interface IUserService
{
    Task<List<User>> GetAllUsersAsync();
    Task<User> GetUserByIdAsync(int id);
    Task<User> UpdateUserAsync(int id, UpdateUserDto dto);
    Task<bool> DeleteUserAsync(int id);
}

// 功能：
// - 获取所有用户
// - 获取单个用户
// - 更新用户信息
// - 删除用户
```

#### PostService（帖子服务）
```csharp
public interface IPostService
{
    Task<List<Post>> GetAllPostsAsync();
    Task<Post> GetPostByIdAsync(int id);
    Task<Post> CreatePostAsync(CreatePostDto dto);
    Task<Post> UpdatePostAsync(int id, UpdatePostDto dto);
    Task<bool> DeletePostAsync(int id);
    Task<List<Post>> GetMyPostsAsync(int userId);
}

// 功能：
// - 获取所有帖子
// - 获取单个帖子
// - 创建帖子
// - 更新帖子
// - 删除帖子
// - 获取我的帖子
```

#### StudentService（学生服务）
```csharp
public interface IStudentService
{
    Task<List<Student>> GetAllStudentsAsync();
    Task<Student> GetStudentByIdAsync(int id);
    Task<Student> CreateStudentAsync(CreateStudentDto dto);
    Task<Student> UpdateStudentAsync(int id, UpdateStudentDto dto);
    Task<bool> DeleteStudentAsync(int id);
}

// 功能：
// - 获取所有学生
// - 获取单个学生
// - 创建学生
// - 更新学生信息
// - 删除学生
```

**技术特点：**
- 使用依赖注入（DI）管理服务生命周期
- 使用 Mapster 进行 DTO 映射
- 异步编程（async/await）
- 异常处理和日志记录

---

### 3. Chat.Server（服务器模块）

**职责：** 提供 REST API 和 WebSocket 服务

**主要组件：**

```
Chat.Server/
├── Controllers/               # REST API 控制器
│   ├── AuthController.cs     # 认证接口
│   ├── UserController.cs     # 用户接口
│   ├── PostController.cs     # 帖子接口
│   ├── CommentController.cs  # 评论接口
│   ├── ChannelController.cs  # 频道接口
│   ├── StudentController.cs  # 学生接口
│   ├── SearchController.cs   # 搜索接口
│   └── AccountController.cs  # 账户接口
├── Program.cs                 # 应用入口
├── Worker.cs                  # 后台服务
├── ChatWebSocketPlugin.cs     # WebSocket 插件
├── appsettings.json           # 配置文件
└── Chat.Server.csproj         # 项目文件
```

#### REST API 控制器

**AuthController（认证控制器）**
```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        // 登录逻辑
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        // 注册逻辑
    }
}
```

**UserController（用户控制器）**
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        // 获取所有用户
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        // 获取单个用户
    }
}
```

#### WebSocket 服务

**ChatWebSocketPlugin（WebSocket 插件）**
```csharp
public class ChatWebSocketPlugin : WebSocketPluginBase
{
    // 连接管理
    private readonly ConcurrentDictionary<int, WebSocketClient> _clients;

    // 连接事件
    protected override async Task OnConnectedAsync(WebSocketClient client)
    {
        // 处理连接
    }

    // 消息接收
    protected override async Task OnReceivedAsync(WebSocketClient client, WebSocketData data)
    {
        // 处理消息
    }

    // 断开连接
    protected override async Task OnDisconnectedAsync(WebSocketClient client)
    {
        // 处理断开
    }

    // 广播消息
    public async Task BroadcastAsync(WsMessageDto message)
    {
        // 广播给所有客户端
    }
}
```

**技术特点：**
- 使用 Furion 框架统一 API 响应格式
- JWT Bearer Token 认证
- TouchSocket 4.0.1 实现 WebSocket
- 后台服务（Worker）监控在线状态
- CORS 跨域支持

---

### 4. Chat.Desktop（桌面客户端模块）

**职责：** 提供跨平台桌面用户界面

**主要组件：**

```
Chat.Desktop/
├── Views/                     # 视图（UI）
│   ├── MainWindow.axaml      # 主窗口
│   ├── LoginWindow.axaml     # 登录窗口
│   ├── RegisterWindow.axaml  # 注册窗口
│   ├── MainView.axaml        # 主界面
│   ├── ChatView.axaml        # 聊天界面
│   ├── ForumView.axaml       # 论坛界面
│   ├── ProfileView.axaml     # 个人资料界面
│   └── SearchView.axaml      # 搜索界面
├── ViewModels/                # 视图模型（MVVM）
│   ├── MainWindowViewModel.cs
│   ├── LoginWindowViewModel.cs
│   ├── RegisterViewModel.cs
│   ├── MainViewModel.cs
│   ├── ChatViewModel.cs
│   ├── ForumViewModel.cs
│   ├── ProfileViewModel.cs
│   └── SearchViewModel.cs
├── Services/                  # 客户端服务
│   ├── ChatWebSocketService.cs  # WebSocket 客户端
│   ├── ForumApiService.cs       # 论坛 API 服务
│   ├── ProfileApiService.cs     # 个人资料 API 服务
│   ├── RegisterApiService.cs    # 注册 API 服务
│   └── SearchApiService.cs      # 搜索 API 服务
├── Models/                    # 客户端模型
│   ├── ChatModel.cs
│   ├── NewsModel.cs
│   └── PostModel.cs
├── Converters/               # 值转换器
│   ├── BoolInvertConverter.cs
│   ├── BoolRunConverter.cs
│   └── FeatureConverters.cs
├── AppConfig.cs              # 应用配置
├── App.axaml                 # 应用入口
├── Program.cs                # 程序入口
└── Chat.Desktop.csproj       # 项目文件
```

#### MVVM 架构

**ViewModelBase（视图模型基类）**
```csharp
public abstract class ViewModelBase : ObservableObject
{
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
```

**ChatViewModel（聊天视图模型）**
```csharp
public class ChatViewModel : ViewModelBase
{
    private readonly ChatWebSocketService _wsService;

    public ObservableCollection<ChatMessage> Messages { get; }

    public ChatViewModel()
    {
        _wsService = new ChatWebSocketService();
        Messages = new ObservableCollection<ChatMessage>();
    }

    public async Task ConnectAsync(string token)
    {
        await _wsService.ConnectAsync(token, _userId);
    }

    public async Task SendMessageAsync(string message)
    {
        await _wsService.SendMessageAsync(message);
    }
}
```

#### WebSocket 客户端服务

**ChatWebSocketService（WebSocket 客户端）**
```csharp
public class ChatWebSocketService
{
    private readonly WebSocketClient _client;

    public async Task ConnectAsync(string token, int userId)
    {
        _client = new WebSocketClient();
        await _client.ConnectAsync(AppConfig.WsUrl);

        // 发送认证消息
        await SendAuthAsync(token);
    }

    public async Task SendMessageAsync(string message)
    {
        var wsMessage = new WsMessageDto
        {
            Type = "chat",
            Action = "send",
            Payload = new
            {
                message = message
            }
        };

        await _client.SendAsync(JsonSerializer.Serialize(wsMessage));
    }

    private async Task HandleReconnectAsync()
    {
        // 自动重连逻辑
    }
}
```

**技术特点：**
- MVVM 架构模式
- CommunityToolkit.Mvvm 提供数据绑定
- Avalonia 跨平台 UI 框架
- Semi.Avalonia 现代化 UI 主题
- 异步编程和错误处理
- 自动重连机制

---

## 服务器设计

### 服务器架构

```
┌─────────────────────────────────────────────────────────┐
│                    Chat.Server                            │
│  ┌───────────────────────────────────────────────────┐  │
│  │              ASP.NET Core Host                     │  │
│  │  ┌──────────────┐  ┌──────────────┐               │  │
│  │  │  Kestrel     │  │   IIS        │               │  │
│  │  │  (HTTP/WS)   │  │  (可选)      │               │  │
│  │  └──────┬───────┘  └──────┬───────┘               │  │
│  └─────────┼─────────────────┼───────────────────────┘  │
│            │                 │                           │
│  ┌─────────┼─────────────────┼───────────────────────┐  │
│  │         ↓                 ↓                           │  │
│  │  ┌──────────────┐  ┌──────────────┐               │  │
│  │  │   REST API   │  │   WebSocket  │               │  │
│  │  │  Middleware  │  │    Plugin    │               │  │
│  │  └──────┬───────┘  └──────┬───────┘               │  │
│  └─────────┼─────────────────┼───────────────────────┘  │
│            │                 │                           │
│  ┌─────────┼─────────────────┼───────────────────────┐  │
│  │         ↓                 ↓                           │  │
│  │  ┌──────────────┐  ┌──────────────┐               │  │
│  │  │  Controllers │  │  WebSocket   │               │  │
│  │  │     (API)    │  │   Handlers   │               │  │
│  │  └──────┬───────┘  └──────┬───────┘               │  │
│  └─────────┼─────────────────┼───────────────────────┘  │
│            │                 │                           │
│            └────────┬────────┘                           │
│                     ↓                                    │
│  ┌───────────────────────────────────────────────────┐  │
│  │              Services (DI)                         │  │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────┐          │  │
│  │  │   Auth   │ │   User   │ │   Post   │          │  │
│  │  └──────────┘ └──────────┘ └──────────┘          │  │
│  └───────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
```

### 启动流程

```csharp
// Program.cs
var builder = Host.CreateApplicationBuilder(args);

// 1. 添加 Furion 框架
builder.Services.AddFurion();

// 2. 添加数据库
builder.Services.AddDatabase();

// 3. 添加服务
builder.Services.AddServices();

// 4. 添加 JWT 认证
builder.Services.AddJwtAuthentication();

// 5. 添加 WebSocket
builder.Services.AddWebSocket();

// 6. 添加 Worker
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

// 7. 初始化数据库
await host.InitDatabaseAsync();

// 8. 启动服务器
await host.RunAsync();
```

### 端口配置

| 端口 | 协议 | 用途 |
|------|------|------|
| 5002 | HTTP | REST API |
| 5003 | WebSocket | 实时通信 |

### 性能优化

1. **连接池管理**
   - 数据库连接池
   - HTTP 连接池

2. **缓存策略**
   - 内存缓存（用户信息）
   - Redis 缓存（规划中）

3. **异步处理**
   - 所有 I/O 操作异步化
   - WebSocket 消息异步处理

4. **日志记录**
   - 结构化日志
   - 性能监控

---

## 通信协议

### REST API 协议

#### 请求格式

```http
POST /api/auth/login HTTP/1.1
Host: localhost:5002
Content-Type: application/json
Authorization: Bearer <token>

{
  "username": "admin",
  "password": "123456"
}
```

#### 响应格式（Furion 标准）

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "user": {
      "id": 1,
      "username": "admin",
      "nickname": "管理员"
    }
  }
}
```

#### 状态码

| 状态码 | 说明 |
|--------|------|
| 200 | 成功 |
| 400 | 请求参数错误 |
| 401 | 未授权 |
| 403 | 禁止访问 |
| 404 | 资源不存在 |
| 500 | 服务器错误 |

### WebSocket 协议

#### 连接地址

```
ws://localhost:5003
```

#### 消息格式

**客户端 → 服务端**

```json
{
  "type": "chat",
  "action": "send",
  "payload": {
    "channelId": 1,
    "message": "Hello World!",
    "token": "jwt_token_here"
  }
}
```

**服务端 → 客户端**

```json
{
  "type": "chat",
  "fromUser": {
    "id": 1,
    "nickname": "张三"
  },
  "channelId": 1,
  "message": "你好！",
  "timestamp": "2026-06-22T14:20:00+08:00"
}
```

#### 消息类型

| Type | Action | 说明 |
|------|--------|------|
| auth | login | 身份认证 |
| chat | send | 发送消息 |
| chat | join | 加入频道 |
| chat | leave | 离开频道 |
| ping | - | 心跳检测 |

#### 心跳机制

```json
// 客户端发送
{
  "type": "ping",
  "timestamp": "2026-06-22T14:20:00+08:00"
}

// 服务端响应
{
  "type": "pong",
  "timestamp": "2026-06-22T14:20:00+08:00"
}
```

---

## 数据库设计

### 数据库表结构

#### users（用户表）

| 字段 | 类型 | 说明 |
|------|------|------|
| id | INT | 主键 |
| username | VARCHAR(50) | 用户名 |
| password | VARCHAR(255) | 密码（加密） |
| nickname | VARCHAR(50) | 昵称 |
| email | VARCHAR(100) | 邮箱 |
| role | VARCHAR(20) | 角色 |
| created_at | DATETIME | 创建时间 |

#### students（学生表）

| 字段 | 类型 | 说明 |
|------|------|------|
| id | INT | 主键 |
| name | VARCHAR(50) | 姓名 |
| student_id | VARCHAR(20) | 学号 |
| class | VARCHAR(50) | 班级 |
| major | VARCHAR(50) | 专业 |
| phone | VARCHAR(20) | 电话 |
| email | VARCHAR(100) | 邮箱 |
| created_at | DATETIME | 创建时间 |

#### posts（帖子表）

| 字段 | 类型 | 说明 |
|------|------|------|
| id | INT | 主键 |
| user_id | INT | 用户ID（外键） |
| title | VARCHAR(200) | 标题 |
| content | TEXT | 内容 |
| created_at | DATETIME | 创建时间 |
| updated_at | DATETIME | 更新时间 |

#### messages（消息表）

| 字段 | 类型 | 说明 |
|------|------|------|
| id | INT | 主键 |
| from_user_id | INT | 发送者ID |
| to_user_id | INT | 接收者ID |
| channel_id | INT | 频道ID |
| content | TEXT | 消息内容 |
| created_at | DATETIME | 发送时间 |

#### channels（频道表）

| 字段 | 类型 | 说明 |
|------|------|------|
| id | INT | 主键 |
| name | VARCHAR(50) | 频道名称 |
| description | TEXT | 描述 |
| created_at | DATETIME | 创建时间 |

### 数据库初始化

```csharp
// DbContext.cs
public class DbContext
{
    public static async Task InitDatabaseAsync()
    {
        // 创建数据库连接
        var db = new SqlSugarClient(new ConnectionConfig()
        {
            ConnectionString = "server=localhost;Database=chat_db;Uid=root;Pwd=123456;",
            DbType = DbType.MySql,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute
        });

        // 创建表结构
        db.CodeFirst.InitTables(
            typeof(User),
            typeof(Student),
            typeof(Post),
            typeof(Message),
            typeof(Channel)
        );

        // 初始化管理员账户
        await InitAdminUserAsync(db);
    }
}
```

---

## 部署架构

### 开发环境

```
┌─────────────────────────────────────────────────┐
│              开发环境 (本地)                      │
│  ┌──────────────┐  ┌──────────────┐            │
│  │   Desktop    │  │   Server     │            │
│  │  (localhost) │  │  (localhost) │            │
│  └──────┬───────┘  └──────┬───────┘            │
│         │                 │                     │
│         └────────┬────────┘                     │
│                  ↓                              │
│         ┌──────────────┐                        │
│         │   MySQL      │                        │
│         │  (localhost) │                        │
│         └──────────────┘                        │
└─────────────────────────────────────────────────┘
```

### 生产环境

```
┌─────────────────────────────────────────────────────────┐
│              生产环境 (Linux 服务器)                       │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │   Nginx      │  │   Server     │  │    MySQL     │  │
│  │  (反向代理)   │  │  (.NET 9.0)  │  │   (数据库)    │  │
│  │  :80/:443    │  │ :5002/:5003  │  │    :3306     │  │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘  │
│         │                 │                 │           │
│         └────────┬────────┴─────────────────┘           │
│                  ↓                                      │
│         ┌──────────────┐                                │
│         │   Redis      │  (可选，缓存)                  │
│         │    :6379     │                                │
│         └──────────────┘                                │
└─────────────────────────────────────────────────────────┘
```

### 部署步骤

#### 1. 服务器部署

```bash
# 1. 安装 .NET 9.0 Runtime
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x ./dotnet-install.sh
./dotnet-install.sh --channel 9.0

# 2. 安装 MySQL
sudo apt-get install mysql-server

# 3. 发布应用
dotnet publish Chat.Server/Chat.Server.csproj -c Release -r linux-x64 --self-contained

# 4. 配置服务
sudo cp chat-server.service /etc/systemd/system/
sudo systemctl enable chat-server
sudo systemctl start chat-server
```

#### 2. 客户端部署

**Windows：**
```powershell
# 直接运行
.\Chat.Desktop.exe
```

**Linux：**
```bash
# 添加执行权限
chmod +x Chat.Desktop

# 运行
./Chat.Desktop
```

**Android：**
```bash
# 构建 APK
./gradlew assembleDebug

# 安装
adb install app/build/outputs/apk/debug/app-debug.apk
```

---

## 安全设计

### 认证与授权

1. **JWT 认证**
   - Token 有效期：24 小时
   - 签名算法：HS256
   - 存储方式：客户端本地存储

2. **密码加密**
   - 使用 BCrypt 加密
   - 盐值自动生成

3. **角色权限**
   - Admin：管理员权限
   - User：普通用户权限

### 数据安全

1. **SQL 注入防护**
   - 使用参数化查询
   - ORM 框架防护

2. **XSS 防护**
   - 输入验证
   - 输出编码

3. **CORS 配置**
   - 限制允许的域名
   - 配置允许的方法

---

## 性能优化

### 服务器优化

1. **连接池**
   - 数据库连接池
   - HTTP 连接池

2. **缓存**
   - 内存缓存
   - Redis 缓存（规划中）

3. **异步处理**
   - 所有 I/O 操作异步化

### 客户端优化

1. **数据绑定优化**
   - 虚拟化列表
   - 延迟加载

2. **网络优化**
   - 请求合并
   - 数据压缩

---

## 监控与日志

### 日志记录

```csharp
// 使用 Furion 日志
Log.Information("用户登录: {Username}", username);
Log.Warning("连接失败: {Error}", error);
Log.Error("系统错误: {Error}", exception);
```

### 性能监控

```csharp
// Worker 定期输出状态
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    while (!stoppingToken.IsCancellationRequested)
    {
        var onlineCount = _wsService.GetOnlineCount();
        Log.Information("在线用户: {Count}", onlineCount);

        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
    }
}
```

---

## 总结

Chat_v 是一个功能完整的实时聊天系统，采用现代化的技术栈和架构设计：

- **前后端分离**：清晰的模块划分和职责分离
- **跨平台支持**：支持 Windows、Linux、Android、Web
- **实时通信**：基于 WebSocket 的高性能实时消息传输
- **安全可靠**：JWT 认证、密码加密、权限控制
- **易于扩展**：模块化设计，便于功能扩展
- **高性能**：异步处理、连接池、缓存优化

项目已完成核心功能开发，可以用于生产环境部署。

---

**文档版本：** v1.0.0
**最后更新：** 2026-06-22
**维护者：** Chat_v 开发团队