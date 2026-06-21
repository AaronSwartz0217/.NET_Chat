# .NET Chat - 实时聊天系统

<p align="center">
  <strong>基于 .NET 9.0 + Furion 的实时聊天应用（后端完成版）</strong>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-9.0-purple" alt=".NET 9.0" />
  <img src="https://img.shields.io/badge/Furion-Framework-blue" alt="Furion" />
  <img src="https://img.shields.io/badge/MySQL-Database-orange" alt="MySQL" />
  <img src="https://img.shields.io/badge/TouchSocket-4.0.1-green" alt="TouchSocket" />
  <img src="https://img.shields.io/badge/JWT-Auth-red" alt="JWT" />
</p>

---

## ⚠️ 项目状态：前端开发完成版 (Frontend Complete)

> **声明：本项目后端服务和前端客户端均已完成并通过测试！**

| 模块 | 状态 | 说明 |
|------|------|------|
| **REST API** | ✅ 完成 | 用户、帖子、评论、频道等完整接口 |
| **WebSocket** | ✅ 完成 | 实时聊天、在线状态、消息广播 |
| **认证授权** | ✅ 完成 | JWT Bearer Token 认证 |
| **数据库** | ✅ 完成 | MySQL + SqlSugar ORM |
| **Android 前端** | ✅ 开发成功 | [NTU_DigitalTwin](https://github.com/AaronSwartz0217/NTU_DigitalTwin) |
| **Desktop 前端** | ✅ 开发成功 | Avalonia/C# |
| **Web 前端** | 📋 规划中 | - |

---

## 功能特性

### 已实现功能

- **用户系统**：注册、登录、JWT认证、角色权限
- **帖子管理**：创建、编辑、删除、查询（我的帖子）
- **评论系统**：CRUD 操作
- **频道/群组**：创建频道、成员管理
- **实时聊天**：
  - 群聊消息广播
  - 私聊功能
  - 在线用户列表
  - 上线/下线通知
  - 输入状态通知（正在输入）
  - 消息已读回执
  - 消息持久化存储
- **断线重连**：自动重连机制（最多3次）

### 技术架构

```
┌─────────────────────────────────────────────────────────┐
│                    客户端层 (开发中)                       │
│         Android / Desktop (Avalonia) / Web              │
└─────────────────────────┬───────────────────────────────┘
                          │ HTTP / WebSocket
          ┌───────────────┼───────────────┐
          ↓               ↓               ↓
┌─────────────────┐ ┌───────────┐ ┌──────────────────┐
│  REST API:5002   │ │ WebSocket │ │   JWT Auth       │
│  (Furion API)    │ │  :5003    │ │  (Bearer Token)  │
└────────┬─────────┘ └─────┬─────┘ └────────┬─────────┘
         │                 │                │
         └────────┬────────┴────────────────┘
                  ↓
┌─────────────────────────────────────────────────────────┐
│                   服务层 (Services)                       │
│  AuthService | UserService | PostService | ChannelService│
└─────────────────────────┬───────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│                  数据访问层 (SqlSugar)                    │
│                      MySQL Database                      │
└─────────────────────────────────────────────────────────┘
```

### 技术栈

| 技术 | 版本 | 用途 |
|------|------|------|
| .NET | 9.0 | 运行时框架 |
| Furion | Latest | 应用框架（API标准格式） |
| SqlSugarCore | Latest | ORM 数据库操作 |
| TouchSocket | 4.0.1 | WebSocket 服务端 |
| Microsoft.AspNetCore.Authentication.JwtBearer | 9.0.0 | JWT 认证 |
| MySQL | 8.x | 关系型数据库 |

---

## 快速开始

### 环境要求

- .NET 9.0 SDK
- MySQL 8.x
- Visual Studio 2022 或 Rider

### 安装步骤

```bash
# 1. 克隆项目
git clone git@github.com:AaronSwartz0217/.NET_Chat.git
cd .NET_Chat

# 2. 配置数据库连接字符串
# 编辑 Chat.Server/appsettings.json（如需要）

# 3. 还原依赖
cd Chat.Server
dotnet restore

# 4. 启动服务
dotnet run
```

### 启动成功标志

```
✅ 管理员账户已存在
✅ 服务已启动 - REST API:5002 | WebSocket:5003
✅ Now listening on: http://0.0.0.0:5002
✅ Application started. Press Ctrl+C to shut down.
```

### 默认账户

| 用户名 | 密码 | 角色 |
|--------|------|------|
| admin | 123456 | 管理员 |

---

## API 接口文档

### 认证接口

| 方法 | 路径 | 说明 |
|------|------|------|
| POST | `/api/auth/register` | 用户注册 |
| POST | `/api/auth/login` | 用户登录（返回JWT） |

### 用户接口（需认证）

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/users` | 获取所有用户 |
| GET | `/api/users/{id}` | 获取单个用户信息 |

### 帖子接口（需认证）

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/posts` | 获取帖子列表 |
| POST | `/api/posts` | 创建帖子 |
| PUT | `/api/posts/{id}` | 更新帖子 |
| DELETE | `/api/posts/{id}` | 删除帖子 |
| GET | `/api/posts/my` | 我的帖子 |

### WebSocket 连接

```
地址：ws://localhost:5003
协议：WebSocket (RFC 6455)
认证：发送 {"type":"auth","token":"<jwt_token>"}
```

详细使用指南请查看 [CHAT_GUIDE.md](./CHAT_GUIDE.md)

---

## 项目结构

```
.NET_Chat/
├── Chat.Core/                 # 核心模型和配置
│   ├── Models/                # 实体类（User, Post, Message...）
│   └── Enums/                 # 枚举定义
├── Chat.Application/          # 应用层
│   ├── Services/              # 业务逻辑服务
│   ├── Dtos/                  # 数据传输对象
│   └── Interfaces/            # 服务接口定义
├── Chat.Server/               # 后端服务（主项目）
│   ├── Controllers/           # REST API 控制器
│   ├── Program.cs             # 应用入口
│   ├── Worker.cs              # 后台服务
│   └── ChatWebSocketPlugin.cs # WebSocket 核心逻辑
├── Chat.Desktop/              # 桌面客户端（Avalonia，开发中）
│   ├── Services/              # 客户端服务
│   ├── ViewModels/            # 视图模型
│   └── Views/                 # UI 视图
├── CHAT_GUIDE.md              # 详细使用指南
└── README.md                  # 本文件
```

---

## 端口说明

| 端口 | 协议 | 用途 |
|------|------|------|
| 5002 | HTTP/REST | RESTful API 接口 |
| 5003 | WebSocket | 实时通信服务 |

---

## 开发计划

### 当前版本 v1.0.0（前端开发完成）

- [x] REST API 完整实现
- [x] WebSocket 实时通信
- [x] JWT 认证与授权
- [x] 消息持久化
- [x] 在线状态管理
- [x] 断线重连机制
- [x] Android 前端客户端 ✅
- [x] Desktop 前端客户端 ✅

### 下一步计划

- [ ] Web 前端开发
- [ ] 图片/文件传输支持
- [ ] 消息撤回功能
- [ ] 群组高级管理

---

## 故障排查

常见问题及解决方案请查看 [CHAT_GUIDE.md](./CHAT_GUIDE.md) 的 **故障排查** 章节。

---

## 许可证

MIT License

---

## 联系方式

- **GitHub**: [AaronSwartz0217/.NET_Chat](https://github.com/AaronSwartz0217/.NET_Chat)
- **Issues**: 欢迎提交 Issue 反馈问题

---

<p align="center">
  <sub>Made with ❤️ using .NET 9.0 + Furion</sub>
</p>
