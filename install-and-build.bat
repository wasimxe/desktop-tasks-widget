@echo off
echo ================================================
echo Desktop Tasks - Complete Setup Script
echo ================================================
echo.

REM Check if installer exists in current directory
if exist "dotnet-sdk-8.0.415-win-x64.exe" (
    echo Found .NET SDK installer!
    echo.
    echo Installing .NET SDK 8.0...
    echo Please wait, this may take a few minutes...
    echo.

    start /wait dotnet-sdk-8.0.415-win-x64.exe /install /quiet /norestart

    if %errorlevel% equ 0 (
        echo.
        echo ================================================
        echo .NET SDK installed successfully!
        echo ================================================
        echo.
        echo Waiting for PATH to update...
        timeout /t 5 /nobreak >nul

        echo.
        echo Building Desktop Tasks application...
        echo.

        call build.bat

        echo.
        echo ================================================
        echo Setup Complete!
        echo ================================================
        echo.
        echo To run the application, use: run.bat
        echo.
    ) else (
        echo.
        echo ERROR: Installation failed or was cancelled.
        echo Please run the installer manually.
        echo.
    )
) else (
    echo .NET SDK installer not found in current directory.
    echo.
    echo Please run: download-dotnet.bat
    echo.
    echo Or manually download from:
    echo https://dotnet.microsoft.com/download/dotnet/8.0
    echo.
)

pause
