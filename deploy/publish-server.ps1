# Chat Server 后端发布脚本 - Windows PowerShell
# 使用方法: .\publish-server.ps1

Write-Host "=========================================="
Write-Host "  Chat Server 后端发布脚本"
Write-Host "  目标平台: Linux x64"
Write-Host "=========================================="

$ErrorActionPreference = "Stop"

# 项目路径
$projectPath = "c:\Users\29717\Desktop\Chat_v\Chat.Server"
$outputPath = "c:\Users\29717\Desktop\Chat_v\deploy\publish"

# 清理旧的发布文件
Write-Host "[1/4] 清理旧的发布文件..."
if (Test-Path $outputPath) {
    Remove-Item -Path $outputPath -Recurse -Force
}
New-Item -ItemType Directory -Path $outputPath -Force | Out-Null

# 发布后端
Write-Host "[2/4] 发布后端服务..."
Set-Location $projectPath
dotnet publish -c Release -r linux-x64 --self-contained true -o $outputPath

# 复制部署脚本到发布目录
Write-Host "[3/4] 复制部署脚本..."
$deployPath = "c:\Users\29717\Desktop\Chat_v\deploy"
Copy-Item -Path "$deployPath\chat-server.service" -Destination $outputPath -Force
Copy-Item -Path "$deployPath\install-debian.sh" -Destination $outputPath -Force
Copy-Item -Path "$deployPath\update-db-config.sh" -Destination $outputPath -Force

# 导出数据库
Write-Host "[4/4] 导出数据库备份..."
$dbBackupPath = "$outputPath\chat_db_backup.sql"
$mysqlDump = "mysqldump"
$dbUser = "root"
$dbPass = "Z2971762643z"
$dbName = "chat_db"

try {
    & $mysqlDump -u $dbUser -p$dbPass $dbName | Out-File -FilePath $dbBackupPath -Encoding UTF8
    Write-Host "数据库备份完成: $dbBackupPath"
} catch {
    Write-Host "数据库备份失败，请手动导出"
    Write-Host "命令: mysqldump -u root -p chat_db > chat_db_backup.sql"
}

Write-Host ""
Write-Host "=========================================="
Write-Host "  发布完成！"
Write-Host "=========================================="
Write-Host ""
Write-Host "发布目录: $outputPath"
Write-Host ""
Write-Host "下一步操作："
Write-Host "1. 将 publish 目录上传到 Debian Linux 虚拟机"
Write-Host "   scp -r publish user@debian-ip:/tmp/"
Write-Host ""
Write-Host "2. 在 Debian 上运行安装脚本"
Write-Host "   cd /tmp/publish"
Write-Host "   sudo chmod +x install-debian.sh"
Write-Host "   sudo ./install-debian.sh"
Write-Host ""
Write-Host "3. 安装服务"
Write-Host "   sudo cp chat-server.service /etc/systemd/system/"
Write-Host "   sudo systemctl daemon-reload"
Write-Host "   sudo systemctl enable chat-server"
Write-Host "   sudo systemctl start chat-server"
Write-Host ""
Write-Host "4. 导入数据库（如果有备份）"
Write-Host "   mysql -u chat_user -p chat_db < chat_db_backup.sql"
Write-Host ""
Write-Host "5. 检查服务状态"
Write-Host "   sudo systemctl status chat-server"
Write-Host "   curl http://localhost:5002/api/auth/login"
Write-Host ""