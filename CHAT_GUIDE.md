# Chat_v 实时聊天系统 - 使用指南

> **⚠️ 版本状态：后端完成版（Backend Complete）**
> - ✅ **后端服务**：REST API + WebSocket 已完成并通过测试
> - 🔧 **前端客户端**：开发中（Android/Desktop/Web）
> - 📅 更新时间：2026-06-13
>
> **本项目为 .NET Chat 应用的后端服务端，提供完整的 RESTful API 和 WebSocket 实时通信能力。**

---

## 📋 系统架构

```
┌─────────────────────────────────────────────────────┐
│              后端服务 (Chat.Server) ✅ 完成            │
│  ┌───────────────────┐    ┌──────────────────────┐   │
│  │   REST API:5002   │    │   WebSocket:5003     │   │
│  │  (HTTP/JSON接口)   │    │  (实时通信)           │   │
│  └───────────────────┘    └──────────────────────┘   │
│                                                         │
│  技术栈：                                                │
│  • .NET 9.0 + Furion 框架                               │
│  • SqlSugar ORM + MySQL                                 │
│  • TouchSocket 4.0.1 (WebSocket)                        │
│  • JWT Bearer Token 认证                                │
└─────────────────────────────────────────────────────┘
         ↓                              ↓
    API调用                        WebSocket连接
         ↓                              ↓
┌─────────────────────────────────────────────────────┐
│              前端客户端 🔧 开发中                       │
│  • Android (Kotlin/Java) - 进行中                     │
│  • Desktop (Avalonia/C#) - 基础框架                   │
│  • Web (待定)                                         │
└─────────────────────────────────────────────────────┘
```

---

## 🚀 快速启动

### 方式1：命令行启动（推荐）

```powershell
# 1. 进入后端目录
cd c:\Users\29717\Desktop\Chat_v\Chat.Server

# 2. 启动服务（REST API + WebSocket）
dotnet run

# 服务启动后会显示：
# ✅ REST API: http://0.0.0.0:5002
# ✅ WebSocket: ws://0.0.0.0:5003
```

### 方式2：IDE启动（Visual Studio / Rider）

1. 打开 `c:\Users\29717\Desktop\Chat_v\Chat.Server`
2. 按 **F5** 或点击运行按钮
3. 控制台输出：
   ```
   Now listening on: http://0.0.0.0:5002
   服务已启动 - REST API:5002 | WebSocket:5003
   ```

---

## 📡 验证服务状态

### 1. 检查端口监听

```powershell
netstat -ano | findstr "5002 5003"
```

**预期输出：**
```
TCP    0.0.0.0:5002    LISTENING    <PID>
TCP    0.0.0.0:5003    LISTENING    <PID>
```

### 2. 测试REST API

```powershell
# 测试健康检查
curl http://localhost:5002/api/auth/login -X POST -H "Content-Type: application/json" -d "{\"username\":\"admin\",\"password\":\"123456\"}"
```

### 3. 测试WebSocket连接

```javascript
// 在浏览器控制台执行
const ws = new WebSocket('ws://localhost:5003');
ws.onopen = () => console.log('✅ WebSocket连接成功');
ws.onerror = (e) => console.log('❌ 连接失败', e);
```

---

## 🔧 API接口列表

### 认证接口

| 方法 | 路径 | 说明 |
|------|------|------|
| POST | `/api/auth/register` | 用户注册 |
| POST | `/api/auth/login` | 用户登录（返回JWT） |

**登录示例：**
```bash
curl -X POST http://localhost:5002/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"123456"}'
```

**响应格式（Furion标准）：**
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

### 用户接口（需JWT认证）

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/users` | 获取所有用户 |
| GET | `/api/users/{id}` | 获取单个用户信息 |

**请求头：**
```
Authorization: Bearer <your_jwt_token>
```

### 帖子接口（需JWT认证）

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/posts` | 获取帖子列表 |
| POST | `/api/posts` | 创建帖子 |
| PUT | `/api/posts/{id}` | 更新帖子 |
| DELETE | `/api/posts/{id}` | 删除帖子 |
| GET | `/api/posts/my` | 我的帖子（谁发了什么） |

**我的帖子查询示例：**
```bash
curl -X GET http://localhost:5002/api/posts/my \
  -H "Authorization: Bearer eyJhbGci..."
```

### 评论、频道等其他接口

详见各Controller文件。

---

## 💬 WebSocket实时通信

### 连接地址

```
ws://localhost:5003        # 本地测试
ws://10.0.2.2:5003         # Android模拟器
wss://your-domain.com:5003 # 生产环境（需配置SSL）
```

### 消息协议

#### 1. 客户端 → 服务端（发送消息）

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

**消息类型（type）：**
- `chat` - 聊天消息
- `auth` - 身份认证
- `ping` - 心跳检测

**动作（action）：**
- `send` - 发送消息
- `join` - 加入频道
- `leave` - 离开频道

#### 2. 服务端 → 客户端（接收消息）

```json
{
  "type": "chat",
  "fromUser": {
    "id": 1,
    "nickname": "张三"
  },
  "channelId": 1,
  "message": "你好！",
  "timestamp": "2026-06-13T14:20:00+08:00"
}
```

### 前端使用示例（JavaScript）

```javascript
class ChatClient {
  constructor(url, token) {
    this.url = url;
    this.token = token;
    this.ws = null;
    this.reconnectAttempts = 0;
    this.maxReconnectAttempts = 3;
  }

  connect() {
    this.ws = new WebSocket(this.url);

    this.ws.onopen = () => {
      console.log('✅ WebSocket已连接');
      this.reconnectAttempts = 0;

      // 发送认证消息
      this.send({
        type: 'auth',
        action: 'login',
        payload: { token: this.token }
      });
    };

    this.ws.onmessage = (event) => {
      const data = JSON.parse(event.data);
      this.handleMessage(data);
    };

    this.ws.onclose = () => {
      console.log('❌ WebSocket已断开');
      this.handleReconnect();
    };

    this.ws.onerror = (error) => {
      console.error('⚠️ WebSocket错误:', error);
    };
  }

  send(message) {
    if (this.ws && this.ws.readyState === WebSocket.OPEN) {
      this.ws.send(JSON.stringify(message));
    }
  }

  sendChatMessage(channelId, message) {
    this.send({
      type: 'chat',
      action: 'send',
      payload: { channelId, message }
    });
  }

  handleMessage(data) {
    switch (data.type) {
      case 'chat':
        console.log(`[${data.fromUser.nickname}]: ${data.message}`);
        // 更新UI显示消息
        break;
      case 'auth':
        console.log('认证成功:', data.payload);
        break;
      default:
        console.log('未知消息类型:', data);
    }
  }

  handleReconnect() {
    if (this.reconnectAttempts < this.maxReconnectAttempts) {
      this.reconnectAttempts++;
      setTimeout(() => {
        console.log(`尝试重连 (${this.reconnectAttempts}/${this.maxReconnectAttempts})...`);
        this.connect();
      }, 2000 * this.reconnectAttempts);
    } else {
      console.error('重连失败次数已达上限，请手动刷新页面');
    }
  }
}

// 使用示例
const client = new ChatClient('ws://localhost:5003', 'your_jwt_token');
client.connect();

// 发送消息
client.sendChatMessage(1, '大家好！');
```

---

## 🧪 测试流程

### 1. 基础功能测试

```bash
# 步骤1：启动服务
dotnet run

# 步骤2：验证端口
netstat -ano | findstr "5002 5003"

# 步骤3：获取JWT Token
TOKEN=$(curl -s -X POST http://localhost:5002/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"123456"}' | \
  jq -r '.data.token')

echo "Token: $TOKEN"

# 步骤4：测试API
curl -X GET http://localhost:5002/api/users \
  -H "Authorization: Bearer $TOKEN"

# 步骤5：测试WebSocket（浏览器控制台）
# 复制上面的JS代码到浏览器F12控制台
```

### 2. 多人聊天测试

打开两个浏览器窗口，分别：

1. **用户A**：登录 → 获取Token → 连接WS → 发送"Hello from A"
2. **用户B**：登录 → 获取Token → 连接WS → 发送"Hello from B"

**预期结果：** 双方都能看到对方的消息。

### 3. 断线重连测试

1. 连接WebSocket
2. 关闭后端服务（Ctrl+C）
3. 观察前端自动重连逻辑
4. 重新启动后端
5. 验证是否恢复连接

---

## ⚙️ 配置说明

### 默认账户

| 用户名 | 密码 | 角色 |
|--------|------|------|
| admin | 123456 | 管理员 |

### 端口配置

| 端口 | 用途 | 修改位置 |
|------|------|----------|
| 5002 | REST API | [Program.cs:84](file:///c:/Users/29717/Desktop/Chat_v/Chat.Server/Program.cs#L84) |
| 5003 | WebSocket | [Program.cs:64](file:///c:/Users/29717/Desktop/Chat_v/Chat.Server/Program.cs#L64) |

**修改端口：**
```csharp
// Program.cs 最后一行
}, urls: "http://0.0.0.0:<你的API端口>");

// WebSocket端口
.SetListenIPHosts(<你的WS端口>)
```

### JWT配置

位置：[JwtSettings](file:///c:/Users/29717/Desktop/Chat_v/Chat.Core/Models/JwtSettings.cs)

```csharp
public class JwtSettings
{
    public string Issuer { get; set; } = "ChatServer";
    public string Audience { get; set; } = "ChatClient";
    public string SecretKey { get; set; } = "YourSuperSecretKey123!@#";
    public int ExpireMinutes { get; set; } = 1440; // 24小时
}
```

---

## 🛠️ 故障排查

### 问题1：端口被占用

**错误：** `Address already in use`

**解决：**
```powershell
# 查找占用进程
netstat -ano | findstr :5002
netstat -ano | findstr :5003

# 终止进程（替换<PID>为上面查到的进程ID）
taskkill /PID <PID> /F
```

### 问题2：WebSocket连接失败

**可能原因：**

1. **后端未启动**
   ```bash
   # 检查服务是否运行
   netstat -ano | findstr 5003
   ```

2. **防火墙阻止**
   ```powershell
   # Windows防火墙添加例外
   New-NetFirewallRule -DisplayName "Chat WebSocket" `
     -Direction Inbound -LocalPort 5003 -Protocol TCP -Action Allow
   ```

3. **地址错误**
   ```
   ❌ 错误：ws://127.0.0.1:5003（仅本机访问）
   ✅ 正确：ws://0.0.0.0:5003（允许外部访问）
   ```

### 问题3：认证失败

**错误：** `401 Unauthorized`

**检查项：**
- Token是否过期（默认24小时）
- Header格式：`Bearer <token>`（注意空格）
- Token是否正确复制（无多余空格）

### 问题4：数据库连接失败

**错误：** `Unable to connect to MySQL server`

**检查：**
1. MySQL服务是否启动
2. 连接字符串是否正确（[appsettings.json](file:///c:/Users/29717/Desktop/Chat_v/Chat.Server/appsettings.json)）
3. 数据库是否存在（首次运行会自动创建）

---

## 📊 性能监控

### 在线用户统计

Worker每10秒输出一次在线人数：

```
[Worker] 运行中 | 在线用户: 5 | 时间: 06/13/2026 14:20:00 +08:00
```

### 日志查看

```powershell
# 实时查看日志（Ctrl+C退出）
dotnet run 2>&1 | Select-String -Pattern "info|error|warn"

# 或者直接看控制台输出
```

---

## 🔄 停止服务

### 方式1：命令行停止

在运行服务的终端按 **Ctrl+C**

### 方式2：查找并终止进程

```powershell
# 查找进程
Get-Process dotnet | Where-Object {$_.CommandLine -like "*Chat.Server*"}

# 终止进程
taskkill /IM dotnet.exe /F
```

---

## 📝 开发笔记

### 技术栈

- **后端框架：** .NET 9.0 + Furion
- **ORM：** SqlSugarCore
- **数据库：** MySQL
- **WebSocket：** TouchSocket 4.0.1
- **认证：** JWT Bearer Token

### 核心文件

| 文件 | 功能 |
|------|------|
| [Program.cs](file:///c:/Users/29717/Desktop/Chat_v/Chat.Server/Program.cs) | 应用入口、服务配置 |
| [Worker.cs](file:///c:/Users/29717/Desktop/Chat_v/Chat.Server/Worker.cs) | 后台服务、生命周期管理 |
| [ChatWebSocketPlugin.cs](file:///c:/Users/29717/Desktop/Chat_v/Chat.Server/ChatWebSocketPlugin.cs) | WebSocket核心逻辑 |
| Controllers/* | REST API控制器 |
| Services/* | 业务逻辑层 |

### 已实现功能 ✅

- [x] 用户注册/登录（JWT认证）
- [x] RESTful API（用户、帖子、评论、频道等）
- [x] WebSocket实时通信（TouchSocket 4.0.1）
- [x] 在线状态管理
- [x] 群聊消息广播
- [x] 私聊功能
- [x] 消息持久化（数据库存储）
- [x] 消息已读回执
- [x] 输入状态通知（正在输入）
- [x] 断线重连机制
- [x] Furion标准响应格式

### 下一步优化建议

- [ ] 支持图片/文件传输
- [ ] 添加消息撤回功能
- [ ] 实现群组管理（创建/解散/踢人）
- [ ] 前端客户端完善（Android/Desktop/Web）

---

## 📞 联系与支持

遇到问题？请检查：

1. **日志输出** - 查看控制台错误信息
2. **端口状态** - `netstat -ano | findstr 5003`
3. **防火墙设置** - 确保5003端口开放
4. **版本兼容性** - TouchSocket >= 4.0.1

---

## 🏷️ 版本信息

| 项目 | 状态 |
|------|------|
| **版本号** | v1.0.0-beta (后端完成版) |
| **发布日期** | 2026-06-13 |
| **后端服务** | ✅ 完成 |
| **REST API** | ✅ 完成 |
| **WebSocket** | ✅ 完成 |
| **前端 Android** | 🔧 开发中 |
| **前端 Desktop** | 🔧 开发中 |
| **前端 Web** | 📋 规划中 |

> **声明：本项目为 .NET Chat 应用的后端服务，已通过完整测试。前端客户端正在开发中。**

---

**最后更新时间：** 2026-06-13
**当前版本：** v1.0.0-beta
