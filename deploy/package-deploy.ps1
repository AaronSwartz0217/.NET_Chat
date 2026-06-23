# 打包部署文件 - Windows PowerShell
# 使用方法: .\package-deploy.ps1

Write-Host "=========================================="
Write-Host "  打包部署文件"
Write-Host "=========================================="

$deployPath = "c:\Users\29717\Desktop\Chat_v\deploy"
$publishPath = "$deployPath\publish"
$packagePath = "$deployPath\chat-server-deploy.tar.gz"

# 检查发布目录
if (-not (Test-Path $publishPath)) {
    Write-Host "发布目录不存在，请先运行发布脚本"
    exit 1
}

# 使用tar打包（Windows 10+自带tar）
Write-Host "打包发布文件..."
Set-Location $deployPath
tar -czf chat-server-deploy.tar.gz -C publish .

if (Test-Path $packagePath) {
    $fileSize = (Get-Item $packagePath).Length / 1MB
    Write-Host ""
    Write-Host "=========================================="
    Write-Host "  打包完成！"
    Write-Host "=========================================="
    Write-Host ""
    Write-Host "文件: $packagePath"
    Write-Host "大小: $fileSize MB"
    Write-Host ""
    Write-Host "上传到 Debian Linux："
    Write-Host "  scp chat-server-deploy.tar.gz user@debian-ip:/tmp/"
    Write-Host ""
    Write-Host "在 Debian 上解压："
    Write-Host "  cd /tmp"
    Write-Host "  tar -xzf chat-server-deploy.tar.gz"
    Write-Host "  sudo ./install-debian.sh"
    Write-Host ""
} else {
    Write-Host "打包失败"
    exit 1
}