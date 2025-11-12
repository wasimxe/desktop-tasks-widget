# 📋 Desktop Tasks Widget

<div align="center">

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)
![WPF](https://img.shields.io/badge/WPF-Windows-0078D4?style=for-the-badge&logo=windows)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

**A beautiful, feature-rich desktop task management widget for Windows that stays visible on your desktop and resists Show Desktop (Win+D).**

[Features](#-features) • [Installation](#-installation) • [Usage](#-usage) • [Building](#-building-from-source) • [License](#-license)

</div>

---

## 🌟 Features

### Core Functionality
- ✅ **Desktop Widget** - Stays visible on your desktop, integrated seamlessly
- 🔒 **Lock/Unlock Modes** - Switch between view-only and interactive modes
- 🎯 **Task Management** - Create, edit, delete tasks with priorities and due dates
- 📌 **Show Desktop Resistance** - Uniquely resists Win+D even without "Always on Top"
- 🎨 **Minimal Mode** - Clean, transparent background showing only tasks
- 🚀 **Auto-Start** - Optional start with Windows on login

### Advanced Features
- **Smart Lock System**
  - 🔒 Locked: Click-through, read-only, resists Show Desktop
  - 🔓 Unlocked: Drag, resize, edit, fully interactive
  - Visual indicator (🔒/🔓) always visible in header

- **WinEvent Hook Technology**
  - Detects Show Desktop activation in real-time
  - Temporarily becomes topmost ONLY during Show Desktop
  - Returns to normal z-order when you click any window
  - Perfect balance: not permanently on top, but never minimizes

- **Task Organization**
  - Priority levels: Low, Normal, High (color-coded borders)
  - Due dates with visual indicators
  - Completion tracking with progress display
  - Automatic sorting (incomplete → priority → due date)
  - Click to edit (when unlocked)

---

## 🚀 Installation

### Prerequisites
- **Windows 10** or **Windows 11**
- **.NET 8.0 Runtime (Desktop)** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)

### Quick Start

1. **Download** the latest release from [Releases](../../releases)
2. **Extract** `DesktopTasks.zip` to a folder
3. **Run** `DesktopTasks.exe`
4. **Configure** via system tray icon (blue circle in bottom-right)

---

## 📖 Usage

### System Tray Menu

Right-click the tray icon for these options:

| Menu Item | Description |
|-----------|-------------|
| **Lock Now / Unlock Now** | Toggle between view and edit modes |
| **Always on Top** | Keep widget above all windows |
| **Minimal Mode** | Hide header, transparent background |
| **Start with Windows** | Auto-launch on system startup |
| **Exit** | Close the application |

### Lock vs Unlock Modes

#### 🔒 Locked Mode
- Click-through to desktop
- No interactive elements visible
- Resists Show Desktop (Win+D)
- Perfect for viewing while working

#### 🔓 Unlocked Mode
- Fully interactive
- Drag from header to move
- Resize from any edge
- Click tasks to edit
- Add/delete tasks

---

## 🛠️ Building from Source

### Prerequisites
- **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Windows 10/11**

### Build Steps

```bash
# Clone the repository
git clone https://github.com/YOUR-USERNAME/desktop-tasks.git
cd desktop-tasks

# Build in Release mode
dotnet build -c Release

# Run the application
dotnet run
```

---

## 🧪 Technical Highlights

### Show Desktop Resistance Solution
Uses `SetWinEventHook` to detect when Show Desktop is activated, temporarily making the window topmost only during Show Desktop, then reverting to normal z-order.

### Key Technologies
- **C# 12** with .NET 8.0
- **WPF** for UI
- **Win32 API Interop** for window management
- **WinEvent Hooks** for Show Desktop detection

---

## 📄 License

This project is licensed under the MIT License.

---

## 👨‍💻 Author

**TanxeStudio**
- 📧 Email: wasimxe@gmail.com
- 💬 WhatsApp: +92 345 540 7008
- 🐙 GitHub: [@TanxeStudio](https://github.com/TanxeStudio)

*Built with ❤️ and C#*

---

<div align="center">

⭐ **Star this repo if you find it useful!** ⭐

</div>
