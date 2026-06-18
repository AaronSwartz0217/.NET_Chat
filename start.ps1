# Chat_v Startup Script
# Start backend (5002+5003) and frontend desktop app together

$ErrorActionPreference = "Stop"
$rootPath = Split-Path -Parent $MyInvocation.MyCommand.Path

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "   Chat_v Real-time Chat System" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

# Check port usage
$port5002 = Get-NetTCPConnection -LocalPort 5002 -ErrorAction SilentlyContinue
$port5003 = Get-NetTCPConnection -LocalPort 5003 -ErrorAction SilentlyContinue

if ($port5002) {
    Write-Host "[WARN] Port 5002 is in use (PID: $($port5002.OwningProcess))" -ForegroundColor Yellow
}
if ($port5003) {
    Write-Host "[WARN] Port 5003 is in use (PID: $($port5003.OwningProcess))" -ForegroundColor Yellow
}

if ($port5002 -or $port5003) {
    $choice = Read-Host "Kill the process? (Y/N)"
    if ($choice -eq 'Y' -or $choice -eq 'y') {
        if ($port5002) { Stop-Process -Id $port5002.OwningProcess -Force -ErrorAction SilentlyContinue }
        if ($port5003) { Stop-Process -Id $port5003.OwningProcess -Force -ErrorAction SilentlyContinue }
        Start-Sleep -Seconds 1
        Write-Host "[OK] Ports released" -ForegroundColor Green
    } else {
        Write-Host "[EXIT] Please release ports manually" -ForegroundColor Red
        exit 1
    }
}

Write-Host ""
Write-Host "[1/2] Starting backend server..." -ForegroundColor Yellow
Write-Host "       REST API: http://localhost:5002"
Write-Host "       WebSocket: ws://localhost:5003"

# Start backend (background)
$serverJob = Start-Job -ScriptBlock {
    Set-Location "$using:rootPath\Chat.Server"
    dotnet run 2>&1
}

# Wait for backend to start
Start-Sleep -Seconds 3

$jobOutput = Receive-Job $serverJob -ErrorAction SilentlyContinue
if ($jobOutput -match "error|Error") {
    Write-Host "[ERROR] Backend failed to start!" -ForegroundColor Red
    Write-Host $jobOutput
    Stop-Job $serverJob -ErrorAction SilentlyContinue
    Remove-Job $serverJob -Force -ErrorAction SilentlyContinue
    exit 1
}

Write-Host "[OK] Backend started" -ForegroundColor Green
Write-Host ""
Write-Host "[2/2] Starting frontend desktop app..." -ForegroundColor Yellow

# Start frontend (foreground)
Set-Location "$rootPath\Chat.Desktop"
dotnet run 2>&1

# Cleanup after frontend closes
Write-Host ""
Write-Host "[Cleanup] Stopping backend..." -ForegroundColor Gray
Stop-Job $serverJob -ErrorAction SilentlyContinue
Remove-Job $serverJob -Force -ErrorAction SilentlyContinue
Write-Host "[DONE] All services stopped" -ForegroundColor Green
