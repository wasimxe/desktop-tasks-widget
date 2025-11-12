# Manual Setup Guide - Desktop Tasks Application

## Network Issue Detected

Automated downloads are being blocked. Please follow these manual steps:

## Step 1: Install .NET SDK 8.0 Manually

### Method 1: Using Your Web Browser (RECOMMENDED)

1. **Open your web browser** (Chrome, Edge, Firefox, etc.)

2. **Go to the official .NET download page:**
   ```
   https://dotnet.microsoft.com/download/dotnet/8.0
   ```

3. **Look for the SDK section** (not Runtime)

4. **Click on "Download .NET SDK x64"** for Windows

5. **Run the downloaded installer** (dotnet-sdk-8.0.xxx-win-x64.exe)

6. **Follow the installation wizard:**
   - Click "Install"
   - Accept the license terms
   - Wait for installation to complete
   - Click "Close"

7. **IMPORTANT: Close and reopen any terminal/command prompt windows**

### Method 2: Direct Download Link

If the above doesn't work, use this direct link:
```
https://download.visualstudio.microsoft.com/download/pr/latest/dotnet-sdk-win-x64.exe
```

## Step 2: Verify Installation

1. Open a **NEW** Command Prompt or PowerShell window

2. Type:
   ```
   dotnet --version
   ```

3. You should see something like `8.0.xxx`

## Step 3: Build the Application

Once .NET SDK is installed:

1. Navigate to the project directory:
   ```
   cd D:\workspace\windows\desktop-tasks
   ```

2. **Option A - Use the build script (Easiest):**
   ```
   build.bat
   ```

3. **Option B - Manual commands:**
   ```
   dotnet restore
   dotnet build
   ```

## Step 4: Run the Application

**Option A - Use the run script:**
```
run.bat
```

**Option B - Manual command:**
```
dotnet run
```

**Option C - Run the compiled executable:**
```
bin\Debug\net8.0-windows\DesktopTasks.exe
```

## What to Expect

When you run the application:

1. **No main window will appear** - this is normal!
2. **Look in the system tray** (bottom-right corner of your screen, near the clock)
3. **You'll see a blue checkmark icon**
4. **Right-click the icon** to access the menu:
   - Manage Tasks
   - Refresh Display
   - Exit

5. **The desktop overlay** will show your active tasks on the desktop

## Troubleshooting

### "dotnet: command not found" after installation

**Solution:**
1. Close ALL terminal windows
2. Open a NEW terminal
3. Try again

If still not working:
- Restart your computer
- Check if .NET is installed in: `C:\Program Files\dotnet\`

### Build errors about missing packages

**Solution:**
```
dotnet restore
dotnet clean
dotnet build
```

### "Cannot find app.ico" or icon errors

**Solution:** This is expected - the app creates icons programmatically. The warning can be ignored.

### Application doesn't start

**Solution:**
1. Check for error messages in the terminal
2. Make sure you're running from the correct directory
3. Try: `dotnet build --verbosity detailed` to see detailed errors

## Need Help?

The complete application source code is in:
```
D:\workspace\windows\desktop-tasks\
```

All files are properly created and ready to build once .NET SDK is installed.
