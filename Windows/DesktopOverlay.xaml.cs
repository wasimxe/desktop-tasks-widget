using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using DesktopTasks.Models;

namespace DesktopTasks.Windows
{
    public partial class DesktopOverlay : Window, INotifyPropertyChanged
    {
        private readonly string _tasksFilePath;
        private readonly string _settingsPath;
        private ObservableCollection<TaskItem> _tasks = new ObservableCollection<TaskItem>();
        private bool _isLocked = true;
        private bool _isAlwaysOnTop = false;
        private bool _isMinimalMode = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Properties for XAML binding
        public bool IsInteractive => !_isLocked;
        public Visibility InteractiveElementsVisibility => _isLocked ? Visibility.Collapsed : Visibility.Visible;

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_LAYERED = 0x00080000;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WS_EX_APPWINDOW = 0x00040000;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll")]
        private static extern int GetClassName(IntPtr hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);

        private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        private IntPtr _winEventHook = IntPtr.Zero;
        private WinEventDelegate? _winEventDelegate;
        private bool _temporaryTopmost = false;

        private const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
        private const uint WINEVENT_OUTOFCONTEXT = 0x0000;

        private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;

        // Window message constants
        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MINIMIZE = 0xF020;

        public DesktopOverlay()
        {
            InitializeComponent();

            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DesktopTasks"
            );
            Directory.CreateDirectory(appDataPath);

            _tasksFilePath = Path.Combine(appDataPath, "tasks.json");
            _settingsPath = Path.Combine(appDataPath, "widget-settings.txt");

            Loaded += DesktopOverlay_Loaded;
            Closing += DesktopOverlay_Closing;
            StateChanged += DesktopOverlay_StateChanged;

            LoadTasks();
            LoadSettings();
        }

        private void DesktopOverlay_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).EnsureHandle();

            // Add message hook to intercept minimize commands
            HwndSource? source = HwndSource.FromHwnd(hwnd);
            source?.AddHook(WndProc);

            // Install WinEvent hook to detect Show Desktop
            _winEventDelegate = new WinEventDelegate(WinEventProc);
            _winEventHook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, _winEventDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);

            ApplyAllStates();
            RefreshTaskList();
        }

        private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (!_isLocked || _isAlwaysOnTop) return;

            // Get the class name of the foreground window
            var className = new System.Text.StringBuilder(256);
            GetClassName(hwnd, className, className.Capacity);
            string classNameStr = className.ToString();

            var myHwnd = new WindowInteropHelper(this).Handle;

            // WorkerW is the Show Desktop window
            if (classNameStr == "WorkerW")
            {
                // Show Desktop activated - temporarily set topmost
                if (!_temporaryTopmost)
                {
                    _temporaryTopmost = true;
                    SetWindowPos(myHwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
                    System.Diagnostics.Debug.WriteLine("[WinEvent] Show Desktop detected - set temporary topmost");
                }
            }
            else
            {
                // Other window activated - remove temporary topmost
                if (_temporaryTopmost)
                {
                    _temporaryTopmost = false;
                    SetWindowPos(myHwnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
                    System.Diagnostics.Debug.WriteLine("[WinEvent] Show Desktop ended - removed temporary topmost");
                }
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Intercept minimize command when locked
            if (msg == WM_SYSCOMMAND && _isLocked)
            {
                int command = wParam.ToInt32() & 0xFFF0;
                if (command == SC_MINIMIZE)
                {
                    // Block minimize when locked
                    handled = true;
                    return IntPtr.Zero;
                }
            }
            return IntPtr.Zero;
        }

        private void DesktopOverlay_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            // Unhook WinEvent
            if (_winEventHook != IntPtr.Zero)
            {
                UnhookWinEvent(_winEventHook);
                _winEventHook = IntPtr.Zero;
            }

            SaveTasks();
            SaveSettings();
        }

        private void DesktopOverlay_StateChanged(object? sender, EventArgs e)
        {
            // Prevent maximization
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }

            // Prevent minimization when locked (Show Desktop resistance)
            if (WindowState == WindowState.Minimized && _isLocked)
            {
                WindowState = WindowState.Normal;
            }
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isLocked && e.ClickCount == 1)
            {
                try
                {
                    DragMove();
                    SaveSettings();
                }
                catch { }
            }
        }

        public void ToggleLock()
        {
            _isLocked = !_isLocked;
            ApplyAllStates();
            SaveSettings();
        }

        public bool IsLocked => _isLocked;
        public bool IsAlwaysOnTop => _isAlwaysOnTop;
        public bool IsMinimalMode => _isMinimalMode;

        public void ToggleAlwaysOnTop()
        {
            _isAlwaysOnTop = !_isAlwaysOnTop;
            ApplyAllStates();
            SaveSettings();
        }

        public void ToggleMinimalMode()
        {
            _isMinimalMode = !_isMinimalMode;
            ApplyAllStates();
            SaveSettings();
        }

        /// <summary>
        /// Unified state application method that handles all combinations of states
        /// </summary>
        private void ApplyAllStates()
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            if (hwnd == IntPtr.Zero) return;

            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            System.Diagnostics.Debug.WriteLine($"[ApplyAllStates] Starting - Locked: {_isLocked}, AlwaysOnTop: {_isAlwaysOnTop}, Current ExtStyle: 0x{extendedStyle:X}");

            // ===== LOCK STATE =====
            if (_isLocked)
            {
                // LOCKED: Click-through mode
                TaskViewPanel.IsHitTestVisible = false;
                MainBorder.IsHitTestVisible = false;
                this.IsHitTestVisible = false;
                HeaderBorder.Cursor = System.Windows.Input.Cursors.Arrow;

                // Window styles for click-through + Show Desktop resistance
                // CRITICAL: Remove WS_EX_APPWINDOW and add WS_EX_TOOLWINDOW to prevent Show Desktop minimizing
                int newStyle = (extendedStyle | WS_EX_TRANSPARENT | WS_EX_LAYERED | WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW) & ~WS_EX_APPWINDOW;
                SetWindowLong(hwnd, GWL_EXSTYLE, newStyle);

                int actualStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
                System.Diagnostics.Debug.WriteLine($"[ApplyAllStates] LOCKED - Intended: 0x{newStyle:X}, Actual: 0x{actualStyle:X}");
                System.Diagnostics.Debug.WriteLine($"  WS_EX_TOOLWINDOW: {(actualStyle & WS_EX_TOOLWINDOW) != 0}");
                System.Diagnostics.Debug.WriteLine($"  WS_EX_APPWINDOW: {(actualStyle & WS_EX_APPWINDOW) != 0}");
                System.Diagnostics.Debug.WriteLine($"  WS_EX_TRANSPARENT: {(actualStyle & WS_EX_TRANSPARENT) != 0}");

                ResizeMode = ResizeMode.NoResize;
                Focusable = false;
                ShowInTaskbar = false;
            }
            else
            {
                // UNLOCKED: Interactive mode
                TaskViewPanel.IsHitTestVisible = true;
                MainBorder.IsHitTestVisible = true;
                this.IsHitTestVisible = true;
                HeaderBorder.Cursor = System.Windows.Input.Cursors.SizeAll;

                // Remove click-through styles, keep WS_EX_TOOLWINDOW, remove WS_EX_APPWINDOW
                SetWindowLong(hwnd, GWL_EXSTYLE,
                    (extendedStyle | WS_EX_TOOLWINDOW) & ~WS_EX_TRANSPARENT & ~WS_EX_LAYERED & ~WS_EX_NOACTIVATE & ~WS_EX_APPWINDOW);

                ResizeMode = ResizeMode.CanResize;
                Focusable = true;
                ShowInTaskbar = false;
            }

            // ===== UI VISIBILITY (Based on Lock + Minimal Mode) =====

            // MINIMAL MODE: Only affects header and background, NOT add button
            if (_isMinimalMode)
            {
                // Hide header and make background transparent
                HeaderBorder.Visibility = Visibility.Collapsed;
                MainBorder.Background = System.Windows.Media.Brushes.Transparent;
                MainBorder.BorderThickness = new Thickness(0);
            }
            else
            {
                // Show header and normal background
                HeaderBorder.Visibility = Visibility.Visible;
                MainBorder.Background = new System.Windows.Media.LinearGradientBrush(
                    System.Windows.Media.Color.FromRgb(248, 249, 250),
                    System.Windows.Media.Color.FromRgb(255, 255, 255),
                    90);
                MainBorder.BorderThickness = new Thickness(1);
            }

            // ADD BUTTON VISIBILITY: Based ONLY on lock state, independent of minimal mode
            if (_isLocked)
            {
                AddTaskPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                AddTaskPanel.Visibility = Visibility.Visible;
            }

            // Force update layout
            AddTaskPanel.UpdateLayout();

            // ===== Z-ORDER (ALWAYS ON TOP) =====
            if (_isAlwaysOnTop)
            {
                // Keep on top regardless of lock state
                SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            }
            else
            {
                // Not always on top - just use NOTOPMOST
                // DO NOT use HWND_BOTTOM even when locked - it interferes with WS_EX_TOOLWINDOW
                // and causes Show Desktop to minimize the window
                SetWindowPos(hwnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            }

            // ===== UPDATE LOCK STATUS INDICATOR =====
            if (LockStatusText != null)
            {
                if (_isLocked)
                {
                    LockStatusText.Text = "🔒";
                    LockStatusText.ToolTip = "Locked - Right-click tray icon to unlock";
                }
                else
                {
                    LockStatusText.Text = "🔓";
                    LockStatusText.ToolTip = "Unlocked - Right-click tray icon to lock";
                }
            }

            // Notify property changes for data binding
            OnPropertyChanged(nameof(IsInteractive));
            OnPropertyChanged(nameof(InteractiveElementsVisibility));
        }

        private void SetLockState(bool locked)
        {
            _isLocked = locked;
            ApplyAllStates();
        }

        // Removed: Now using Header_MouseLeftButtonDown for dragging
        // This prevents interference with task clicking and other controls

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new TaskEditorDialog();
            if (dialog.ShowDialog() == true && dialog.TaskResult != null)
            {
                _tasks.Add(dialog.TaskResult);
                SaveTasks();
                RefreshTaskList();
            }
        }

        private void TaskItem_Click(object sender, MouseButtonEventArgs e)
        {
            if (_isLocked) return; // Don't edit when locked

            if (sender is System.Windows.Controls.Border border && border.Tag is TaskItem task)
            {
                var dialog = new TaskEditorDialog(task);
                if (dialog.ShowDialog() == true && dialog.TaskResult != null)
                {
                    // Find and update the task
                    var index = _tasks.IndexOf(task);
                    if (index >= 0)
                    {
                        _tasks[index] = dialog.TaskResult;
                        SaveTasks();
                        RefreshTaskList();
                    }
                }
            }
        }

        private void TaskCheckBox_Click(object sender, RoutedEventArgs e)
        {
            SaveTasks();
            RefreshTaskList();
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is TaskItem task)
            {
                _tasks.Remove(task);
                SaveTasks();
                RefreshTaskList();
            }
        }

        private void RefreshTaskList()
        {
            // Sort tasks: incomplete first, then by priority, then by due date
            var sortedTasks = _tasks
                .OrderBy(t => t.IsCompleted)
                .ThenByDescending(t => (int)t.Priority)
                .ThenBy(t => t.DueDate ?? DateTime.MaxValue)
                .ToList();

            TaskList.ItemsSource = sortedTasks;

            // Update task count
            int totalTasks = _tasks.Count;
            int completedTasks = _tasks.Count(t => t.IsCompleted);
            TaskCountText.Text = $"{completedTasks}/{totalTasks} completed";

            // Create default task if empty
            if (_tasks.Count == 0)
            {
                CreateDefaultTasks();
                RefreshTaskList();
            }
        }

        private void CreateDefaultTasks()
        {
            _tasks.Add(new TaskItem
            {
                Title = "Welcome to Desktop Tasks!",
                Description = "Click the tray icon to lock/unlock",
                Priority = TaskPriority.Normal,
                IsCompleted = false
            });

            _tasks.Add(new TaskItem
            {
                Title = "Add your first task",
                Description = "Click '+ Add Task' to create a new task",
                Priority = TaskPriority.Low,
                IsCompleted = false
            });

            SaveTasks();
        }

        private void LoadTasks()
        {
            try
            {
                if (File.Exists(_tasksFilePath))
                {
                    string json = File.ReadAllText(_tasksFilePath);
                    var tasks = JsonSerializer.Deserialize<TaskItem[]>(json);
                    if (tasks != null)
                    {
                        _tasks = new ObservableCollection<TaskItem>(tasks);
                    }
                }
            }
            catch { }
        }

        private void SaveTasks()
        {
            try
            {
                string json = JsonSerializer.Serialize(_tasks, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(_tasksFilePath, json);
            }
            catch { }
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    string[] lines = File.ReadAllLines(_settingsPath);
                    if (lines.Length >= 5)
                    {
                        Left = double.Parse(lines[0]);
                        Top = double.Parse(lines[1]);
                        Width = double.Parse(lines[2]);
                        Height = double.Parse(lines[3]);
                        _isLocked = bool.Parse(lines[4]);

                        if (lines.Length >= 6)
                            _isAlwaysOnTop = bool.Parse(lines[5]);
                        if (lines.Length >= 7)
                            _isMinimalMode = bool.Parse(lines[6]);
                    }
                }
            }
            catch { }
        }

        private void SaveSettings()
        {
            try
            {
                string[] settings = {
                    Left.ToString(),
                    Top.ToString(),
                    Width.ToString(),
                    Height.ToString(),
                    _isLocked.ToString(),
                    _isAlwaysOnTop.ToString(),
                    _isMinimalMode.ToString()
                };
                File.WriteAllLines(_settingsPath, settings);
            }
            catch { }
        }
    }
}
