#!/bin/bash
# 数据库连接配置修改脚本
# 使用方法: ./update-db-config.sh

# 数据库配置
DB_HOST="localhost"
DB_PORT="3306"
DB_NAME="chat_db"
DB_USER="chat_user"
DB_PASS="Chat@2026#Secure"

# 后端配置文件路径
CONFIG_FILE="/opt/chat-server/Chat.Core.dll"

echo "=========================================="
echo "  数据库连接配置"
echo "=========================================="
echo "主机: $DB_HOST"
echo "端口: $DB_PORT"
echo "数据库: $DB_NAME"
echo "用户: $DB_USER"
echo "密码: $DB_PASS"
echo ""

# 注意：SqlSugar配置在代码中，需要修改DbContext.cs后重新发布
# 或者使用环境变量覆盖

echo "提示：数据库连接信息已配置在代码中"
echo "如果需要修改，请："
echo "1. 修改 Chat.Core/DbContext.cs 中的连接字符串"
echo "2. 重新发布后端：dotnet publish -c Release -r linux-x64"
echo "3. 上传新的发布文件到 /opt/chat-server"
echo ""

# 测试数据库连接
echo "测试数据库连接..."
mysql -h "$DB_HOST" -P "$DB_PORT" -u "$DB_USER" -p"$DB_PASS" -e "SELECT 1;" && echo "数据库连接成功！" || echo "数据库连接失败！请检查配置"