using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using DesktopTasks.Helpers;
using DesktopTasks.Windows;
using Microsoft.Win32;
using Application = System.Windows.Application;

namespace DesktopTasks
{
    public partial class App : Application
    {
        private NotifyIcon? _notifyIcon;
        private DesktopOverlay? _desktopOverlay;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Create desktop overlay window
            _desktopOverlay = new DesktopOverlay();
            _desktopOverlay.Show();

            // Create system tray icon
            SetupSystemTray();
        }

        private void SetupSystemTray()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = IconHelper.CreateDefaultIcon(),
                Visible = true,
                Text = "Desktop Widget - Click to Lock/Unlock"
            };

            // Create context menu
            var contextMenu = new ContextMenuStrip();

            var lockMenuItem = new ToolStripMenuItem("Lock/Unlock");
            lockMenuItem.Click += (s, e) => ToggleLock();
            contextMenu.Items.Add(lockMenuItem);

            var alwaysOnTopMenuItem = new ToolStripMenuItem("Always on Top");
            alwaysOnTopMenuItem.CheckOnClick = true;
            alwaysOnTopMenuItem.Click += (s, e) =>
            {
                _desktopOverlay?.ToggleAlwaysOnTop();
                UpdateTrayIcon();
            };
            contextMenu.Items.Add(alwaysOnTopMenuItem);

            var minimalModeMenuItem = new ToolStripMenuItem("Minimal Mode");
            minimalModeMenuItem.CheckOnClick = true;
            minimalModeMenuItem.Click += (s, e) =>
            {
                _desktopOverlay?.ToggleMinimalMode();
                UpdateTrayIcon();
            };
            contextMenu.Items.Add(minimalModeMenuItem);

            var startWithWindowsMenuItem = new ToolStripMenuItem("Start with Windows");
            startWithWindowsMenuItem.CheckOnClick = true;
            startWithWindowsMenuItem.Click += (s, e) =>
            {
                ToggleStartWithWindows();
            };
            contextMenu.Items.Add(startWithWindowsMenuItem);

            contextMenu.Items.Add(new ToolStripSeparator());

            var exitMenuItem = new ToolStripMenuItem("Exit");
            exitMenuItem.Click += (s, e) => Shutdown();
            contextMenu.Items.Add(exitMenuItem);

            _notifyIcon.ContextMenuStrip = contextMenu;

            // Update menu checkmarks and labels before opening
            contextMenu.Opening += (s, e) =>
            {
                // Update lock menu item text
                bool isLocked = _desktopOverlay?.IsLocked ?? true;
                lockMenuItem.Text = isLocked ? "Unlock Now" : "Lock Now";

                alwaysOnTopMenuItem.Checked = _desktopOverlay?.IsAlwaysOnTop ?? false;
                minimalModeMenuItem.Checked = _desktopOverlay?.IsMinimalMode ?? false;
                startWithWindowsMenuItem.Checked = IsStartupEnabled();
            };

            UpdateTrayIcon();
        }

        private void ToggleLock()
        {
            _desktopOverlay?.ToggleLock();
            UpdateTrayIcon();
        }

        private void UpdateTrayIcon()
        {
            if (_notifyIcon != null && _desktopOverlay != null)
            {
                _notifyIcon.Text = _desktopOverlay.IsLocked
                    ? "🔒 Widget Locked - Click to Edit"
                    : "🔓 Widget Unlocked - Click to Lock";
            }
        }

        private void ToggleStartWithWindows()
        {
            bool isEnabled = IsStartupEnabled();
            SetStartup(!isEnabled);
        }

        private bool IsStartupEnabled()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", false);
                return key?.GetValue("DesktopTasks") != null;
            }
            catch
            {
                return false;
            }
        }

        private void SetStartup(bool enable)
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                if (key != null)
                {
                    if (enable)
                    {
                        // Get the path to the executable
                        string exePath = Process.GetCurrentProcess().MainModule?.FileName ?? Assembly.GetExecutingAssembly().Location;

                        // For .NET apps, we need the exe path, not the dll
                        if (exePath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                        {
                            exePath = exePath.Replace(".dll", ".exe");
                        }

                        key.SetValue("DesktopTasks", $"\"{exePath}\"");
                    }
                    else
                    {
                        key.DeleteValue("DesktopTasks", false);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Failed to update startup settings: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _notifyIcon?.Dispose();
        }
    }
}
