# Chat Server 后端部署指南 - Debian Linux 13

## 📋 部署概述

本指南帮助你将 Chat Server 后端部署到 Debian Linux 13 虚拟机。

---

## 🚀 快速部署（推荐）

### 步骤1：在Windows上发布后端

```powershell
# 进入部署目录
cd c:\Users\29717\Desktop\Chat_v\deploy

# 运行发布脚本
.\publish-server.ps1
```

发布完成后，`publish` 目录包含：
- Chat.Server.dll 及所有依赖
- chat-server.service（systemd服务配置）
- install-debian.sh（安装脚本）
- chat_db_backup.sql（数据库备份）

---

### 步骤2：上传到Debian虚拟机

```powershell
# 使用SCP上传（Windows PowerShell）
scp -r c:\Users\29717\Desktop\Chat_v\deploy\publish user@debian-ip:/tmp/

# 或使用WinSCP、FileZilla等工具上传
```

---

### 步骤3：在Debian上安装

```bash
# SSH登录到Debian
ssh user@debian-ip

# 进入上传目录
cd /tmp/publish

# 给脚本执行权限
sudo chmod +x install-debian.sh

# 运行安装脚本
sudo ./install-debian.sh
```

安装脚本会自动：
- ✅ 更新系统包
- ✅ 安装 .NET 9.0 Runtime
- ✅ 安装 MySQL/MariaDB
- ✅ 创建数据库和用户
- ✅ 配置防火墙

---

### 步骤4：部署后端服务

```bash
# 创建服务目录
sudo mkdir -p /opt/chat-server

# 复制发布文件
sudo cp -r /tmp/publish/* /opt/chat-server/

# 安装systemd服务
sudo cp /opt/chat-server/chat-server.service /etc/systemd/system/

# 重载systemd
sudo systemctl daemon-reload

# 启用并启动服务
sudo systemctl enable chat-server
sudo systemctl start chat-server

# 检查服务状态
sudo systemctl status chat-server
```

---

### 步骤5：导入数据库数据

```bash
# 导入数据库备份（如果有）
mysql -u chat_user -p'Chat@2026#Secure' chat_db < /opt/chat-server/chat_db_backup.sql

# 验证数据
mysql -u chat_user -p'Chat@2026#Secure' chat_db -e "SHOW TABLES;"
```

---

## 🔧 手动部署（可选）

### 1. 安装系统依赖

```bash
# 更新系统
sudo apt update && sudo apt upgrade -y

# 安装必要工具
sudo apt install -y curl wget unzip

# 安装 .NET 9.0
wget https://dot.net/v1/dotnet-install.sh -O /tmp/dotnet-install.sh
chmod +x /tmp/dotnet-install.sh
sudo /tmp/dotnet-install.sh --channel 9.0 --runtime aspnetcore --install-dir /usr/share/dotnet
sudo ln -sf /usr/share/dotnet/dotnet /usr/bin/dotnet

# 安装 MySQL
sudo apt install -y mariadb-server mariadb-client
sudo systemctl start mariadb
sudo systemctl enable mariadb
```

### 2. 创建数据库

```bash
sudo mysql <<EOF
CREATE DATABASE chat_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE USER 'chat_user'@'localhost' IDENTIFIED BY 'Chat@2026#Secure';
GRANT ALL PRIVILEGES ON chat_db.* TO 'chat_user'@'localhost';
FLUSH PRIVILEGES;
EOF
```

### 3. 配置防火墙

```bash
sudo apt install -y ufw
sudo ufw allow 5002/tcp
sudo ufw --force enable
```

---

## 📊 服务管理命令

| 操作 | 命令 |
|------|------|
| **启动服务** | `sudo systemctl start chat-server` |
| **停止服务** | `sudo systemctl stop chat-server` |
| **重启服务** | `sudo systemctl restart chat-server` |
| **查看状态** | `sudo systemctl status chat-server` |
| **查看日志** | `sudo journalctl -u chat-server -f` |
| **查看端口** | `sudo lsof -i :5002` |

---

## ✅ 验证部署

```bash
# 检查服务运行
sudo systemctl status chat-server

# 测试API
curl http://localhost:5002/api/auth/login

# 测试登录API
curl -X POST http://localhost:5002/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"userName": "test", "password": "123456"}'

# 检查数据库
mysql -u chat_user -p'Chat@2026#Secure' chat_db -e "SELECT * FROM users;"
```

---

## 🔐 数据库信息

| 配置项 | 值 |
|--------|-----|
| **主机** | localhost |
| **端口** | 3306 |
| **数据库** | chat_db |
| **用户** | chat_user |
| **密码** | Chat@2026#Secure |

---

## 🌐 客户端连接

部署完成后，修改客户端API地址：

```
原地址: http://127.0.0.1:5002
新地址: http://<Debian虚拟机IP>:5002
```

---

## ❗ 常见问题

### 问题1：服务无法启动

```bash
# 检查日志
sudo journalctl -u chat-server -n 50

# 检查.NET版本
dotnet --version

# 检查端口占用
sudo lsof -i :5002
```

### 问题2：数据库连接失败

```bash
# 检查MySQL服务
sudo systemctl status mariadb

# 测试连接
mysql -u chat_user -p'Chat@2026#Secure' chat_db

# 检查用户权限
sudo mysql -e "SHOW GRANTS FOR 'chat_user'@'localhost';"
```

### 问题3：防火墙阻止访问

```bash
# 检查防火墙状态
sudo ufw status

# 开放端口
sudo ufw allow 5002/tcp

# 或临时关闭防火墙测试
sudo ufw disable
```

---

## 📁 文件结构

```
/opt/chat-server/
├── Chat.Server.dll          # 主程序
├── Chat.Core.dll            # 核心库
├── Chat.Application.dll     # 应用层
├── chat-server.service      # 服务配置
├── install-debian.sh        # 安装脚本
├── chat_db_backup.sql       # 数据库备份
└── ... (其他依赖文件)
```

---

## 🔄 更新部署

```bash
# 1. 停止服务
sudo systemctl stop chat-server

# 2. 备份旧版本
sudo mv /opt/chat-server /opt/chat-server-backup

# 3. 上传新版本
sudo cp -r /tmp/publish-new /opt/chat-server

# 4. 启动服务
sudo systemctl start chat-server

# 5. 验证
sudo systemctl status chat-server
```

---

## 📞 技术支持

如有问题，请检查：
1. 服务日志：`sudo journalctl -u chat-server -f`
2. 系统日志：`sudo tail -f /var/log/syslog`
3. 数据库日志：`sudo tail -f /var/log/mysql/error.log`