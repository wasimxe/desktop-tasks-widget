# .NET SDK 8.0 Installation Script
# Run this as Administrator

Write-Host "========================================" -ForegroundColor Cyan
Write-Host ".NET SDK 8.0 Installation Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if running as Administrator
$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "ERROR: This script must be run as Administrator!" -ForegroundColor Red
    Write-Host ""
    Write-Host "To run as Administrator:" -ForegroundColor Yellow
    Write-Host "1. Right-click on PowerShell" -ForegroundColor Yellow
    Write-Host "2. Select 'Run as Administrator'" -ForegroundColor Yellow
    Write-Host "3. Navigate to this directory and run the script again" -ForegroundColor Yellow
    Write-Host ""
    pause
    exit 1
}

Write-Host "Administrator privileges confirmed!" -ForegroundColor Green
Write-Host ""

# Download URL
$sdkUrl = "https://download.visualstudio.microsoft.com/download/pr/b8cf3a22-c62d-4ac2-a835-c03b60cf65db/f14c00b4ec7f5c1d7085abf0e62bb9ee/dotnet-sdk-8.0.415-win-x64.exe"
$installerPath = "$env:TEMP\dotnet-sdk-8.0.415-win-x64.exe"

Write-Host "Downloading .NET SDK 8.0.415..." -ForegroundColor Yellow
Write-Host "URL: $sdkUrl" -ForegroundColor Gray
Write-Host ""

try {
    # Download with progress
    $webClient = New-Object System.Net.WebClient
    $webClient.DownloadFile($sdkUrl, $installerPath)

    Write-Host "Download completed!" -ForegroundColor Green
    Write-Host ""

    # Run installer
    Write-Host "Starting installer..." -ForegroundColor Yellow
    Write-Host "Please wait, this may take a few minutes..." -ForegroundColor Gray
    Write-Host ""

    $process = Start-Process -FilePath $installerPath -ArgumentList "/install", "/quiet", "/norestart" -Wait -PassThru

    if ($process.ExitCode -eq 0) {
        Write-Host "========================================" -ForegroundColor Green
        Write-Host ".NET SDK installed successfully!" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Green
        Write-Host ""
        Write-Host "Cleaning up installer..." -ForegroundColor Gray
        Remove-Item $installerPath -Force -ErrorAction SilentlyContinue

        Write-Host ""
        Write-Host "Next steps:" -ForegroundColor Cyan
        Write-Host "1. Close this window" -ForegroundColor White
        Write-Host "2. Open a NEW PowerShell or Command Prompt" -ForegroundColor White
        Write-Host "3. Navigate to: D:\workspace\windows\desktop-tasks" -ForegroundColor White
        Write-Host "4. Run: build.bat" -ForegroundColor White
        Write-Host ""
    } else {
        Write-Host "Installation failed with exit code: $($process.ExitCode)" -ForegroundColor Red
        Write-Host "Installer location: $installerPath" -ForegroundColor Yellow
        Write-Host "You can try running it manually." -ForegroundColor Yellow
    }
}
catch {
    Write-Host "ERROR: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Manual installation required:" -ForegroundColor Yellow
    Write-Host "1. Open your browser" -ForegroundColor White
    Write-Host "2. Go to: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor White
    Write-Host "3. Download and install .NET SDK 8.0" -ForegroundColor White
}

Write-Host ""
pause
