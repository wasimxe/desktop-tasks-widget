@echo off
echo ================================
echo Desktop Tasks - Build Script
echo ================================
echo.

echo Checking for .NET SDK...
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: .NET SDK is not installed!
    echo Please follow the instructions in INSTALL_DOTNET.md
    echo.
    pause
    exit /b 1
)

echo .NET SDK found!
echo.

echo Restoring NuGet packages...
dotnet restore
if %errorlevel% neq 0 (
    echo ERROR: Failed to restore packages
    pause
    exit /b 1
)

echo.
echo Building project...
dotnet build -c Release
if %errorlevel% neq 0 (
    echo ERROR: Build failed
    pause
    exit /b 1
)

echo.
echo ================================
echo Build completed successfully!
echo ================================
echo.
echo To run the application:
echo   dotnet run
echo.
echo Or run the executable directly from:
echo   bin\Release\net8.0-windows\DesktopTasks.exe
echo.
pause
