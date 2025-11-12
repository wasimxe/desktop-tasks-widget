$ErrorActionPreference = "Stop"

Write-Host "Attempting to download .NET SDK 8.0..." -ForegroundColor Yellow

try {
    $url = "https://download.visualstudio.microsoft.com/download/pr/b8cf3a22-c62d-4ac2-a835-c03b60cf65db/f14c00b4ec7f5c1d7085abf0e62bb9ee/dotnet-sdk-8.0.415-win-x64.exe"
    $output = "dotnet-sdk-installer.exe"

    Write-Host "URL: $url"
    Write-Host "Output: $output"

    # Method 1: Try WebClient
    Write-Host "`nTrying WebClient..." -ForegroundColor Cyan
    $webClient = New-Object System.Net.WebClient
    $webClient.DownloadFile($url, $output)

    if (Test-Path $output) {
        $size = (Get-Item $output).Length
        Write-Host "SUCCESS! Downloaded $size bytes" -ForegroundColor Green
        exit 0
    }
}
catch {
    Write-Host "Method 1 failed: $_" -ForegroundColor Red

    try {
        # Method 2: Try Invoke-WebRequest
        Write-Host "`nTrying Invoke-WebRequest..." -ForegroundColor Cyan
        Invoke-WebRequest -Uri $url -OutFile $output -UseBasicParsing

        if (Test-Path $output) {
            $size = (Get-Item $output).Length
            Write-Host "SUCCESS! Downloaded $size bytes" -ForegroundColor Green
            exit 0
        }
    }
    catch {
        Write-Host "Method 2 failed: $_" -ForegroundColor Red
        Write-Host "`nAll download methods failed." -ForegroundColor Red
        exit 1
    }
}
