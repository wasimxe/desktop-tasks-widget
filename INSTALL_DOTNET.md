# How to Install .NET SDK 8.0

## Option 1: Direct Download (Recommended)

1. Open your web browser
2. Go to: **https://dotnet.microsoft.com/download/dotnet/8.0**
3. Under "SDK 8.0.x", click "Download .NET SDK x64" for Windows
4. Run the downloaded installer
5. Follow the installation wizard
6. Restart your terminal/command prompt after installation

## Option 2: Using Windows Package Manager (winget)

Open PowerShell or Command Prompt as Administrator and run:

```bash
winget install Microsoft.DotNet.SDK.8
```

## Option 3: Using Chocolatey (if installed)

```bash
choco install dotnet-sdk
```

## Verify Installation

After installation, open a **new** terminal and run:

```bash
dotnet --version
```

You should see something like `8.0.xxx`

## Next Steps

Once .NET SDK is installed, return to the project directory and run:

```bash
cd D:\workspace\windows\desktop-tasks
dotnet restore
dotnet build
dotnet run
```

## Troubleshooting

If `dotnet` command is not found after installation:
1. Close all terminal windows
2. Open a new terminal window
3. Try the command again

If still not working, you may need to add .NET to your PATH manually:
- .NET is typically installed to: `C:\Program Files\dotnet\`
