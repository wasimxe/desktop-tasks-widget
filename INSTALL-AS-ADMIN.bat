@echo off
echo ================================================
echo .NET SDK Installation Launcher
echo ================================================
echo.
echo This will launch the PowerShell installation script
echo with Administrator privileges.
echo.
echo Please click "Yes" when prompted for admin rights.
echo.
pause

powershell -Command "Start-Process PowerShell -ArgumentList '-ExecutionPolicy Bypass -File ""%~dp0Install-DotNet-Admin.ps1""' -Verb RunAs"
