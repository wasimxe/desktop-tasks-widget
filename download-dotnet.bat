@echo off
echo ================================================
echo .NET SDK 8.0 Download Helper
echo ================================================
echo.
echo Opening download page in your default browser...
echo.
echo The download should start automatically.
echo File: dotnet-sdk-8.0.415-win-x64.exe
echo.

start https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-8.0.415-windows-x64-installer

echo.
echo If the download doesn't start:
echo 1. Click the download link on the page that opened
echo 2. Save the file to this directory:
echo    %CD%
echo.
echo After download completes:
echo 1. Run the installer: dotnet-sdk-8.0.415-win-x64.exe
echo 2. Follow the installation wizard
echo 3. After installation, run: build.bat
echo.
pause
