#!/bin/bash
# Chat Server 后端部署脚本 - Debian Linux 13
# 使用方法: sudo ./install-debian.sh

set -e

echo "=========================================="
echo "  Chat Server 后端部署脚本"
echo "  目标系统: Debian Linux 13"
echo "=========================================="

# 检查是否以root运行
if [ "$EUID" -ne 0 ]; then
    echo "请使用 root 权限运行此脚本"
    echo "sudo ./install-debian.sh"
    exit 1
fi

# 1. 更新系统
echo "[1/6] 更新系统包..."
apt update && apt upgrade -y

# 2. 安装必要依赖
echo "[2/6] 安装必要依赖..."
apt install -y curl wget unzip gnupg2 apt-transport-https

# 3. 安装 .NET 9.0 Runtime
echo "[3/6] 安装 .NET 9.0 Runtime..."
if ! command -v dotnet &> /dev/null; then
    wget https://dot.net/v1/dotnet-install.sh -O /tmp/dotnet-install.sh
    chmod +x /tmp/dotnet-install.sh
    /tmp/dotnet-install.sh --channel 9.0 --runtime aspnetcore --install-dir /usr/share/dotnet
    
    # 创建符号链接
    ln -sf /usr/share/dotnet/dotnet /usr/bin/dotnet
    
    # 设置环境变量
    echo 'export DOTNET_ROOT=/usr/share/dotnet' >> /etc/profile
    echo 'export PATH=$PATH:$DOTNET_ROOT' >> /etc/profile
fi

# 验证 .NET 安装
echo "验证 .NET 安装..."
dotnet --version || echo ".NET 安装完成，请重新登录后验证"

# 4. 安装 MySQL
echo "[4/6] 安装 MySQL Server..."
if ! command -v mysql &> /dev/null; then
    apt install -y mariadb-server mariadb-client
    
    # 启动MySQL
    systemctl start mariadb
    systemctl enable mariadb
    
    # 安全配置（可选）
    # mysql_secure_installation
fi

# 5. 创建数据库
echo "[5/6] 创建数据库..."
read -p "请输入MySQL root密码（默认为空，直接回车）: " mysql_root_password

if [ -z "$mysql_root_password" ]; then
    mysql -u root <<EOF
CREATE DATABASE IF NOT EXISTS chat_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE USER IF NOT EXISTS 'chat_user'@'localhost' IDENTIFIED BY 'Chat@2026#Secure';
GRANT ALL PRIVILEGES ON chat_db.* TO 'chat_user'@'localhost';
FLUSH PRIVILEGES;
EOF
else
    mysql -u root -p"$mysql_root_password" <<EOF
CREATE DATABASE IF NOT EXISTS chat_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE USER IF NOT EXISTS 'chat_user'@'localhost' IDENTIFIED BY 'Chat@2026#Secure';
GRANT ALL PRIVILEGES ON chat_db.* TO 'chat_user'@'localhost';
FLUSH PRIVILEGES;
EOF
fi

echo "数据库创建完成！"
echo "数据库: chat_db"
echo "用户: chat_user"
echo "密码: Chat@2026#Secure"

# 6. 配置防火墙
echo "[6/6] 配置防火墙..."
if command -v ufw &> /dev/null; then
    ufw allow 5002/tcp comment 'Chat Server API'
    ufw --force enable
    echo "防火墙已配置，端口 5002 已开放"
else
    echo "未检测到 ufw，请手动配置防火墙"
fi

echo ""
echo "=========================================="
echo "  系统依赖安装完成！"
echo "=========================================="
echo ""
echo "下一步操作："
echo "1. 上传后端发布文件到 /opt/chat-server"
echo "2. 导入数据库数据（如果有备份）"
echo "3. 修改数据库连接配置"
echo "4. 启动服务"
echo ""