using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DesktopTasks.Models;
using DesktopTasks.Services;

namespace DesktopTasks.Windows
{
    public partial class TaskManagerWindow : Window
    {
        private readonly TaskStorageService _taskStorage;
        private readonly DesktopOverlay _desktopOverlay;
        private TaskItem? _currentTask;

        public TaskManagerWindow(TaskStorageService taskStorage, DesktopOverlay desktopOverlay)
        {
            InitializeComponent();
            _taskStorage = taskStorage;
            _desktopOverlay = desktopOverlay;

            // Setup priority combo box
            PriorityComboBox.ItemsSource = Enum.GetValues(typeof(TaskPriority));
            PriorityComboBox.SelectedIndex = 1; // Default to Normal

            LoadTasks();
            ClearForm();
        }

        private void LoadTasks()
        {
            TasksListView.ItemsSource = null;
            TasksListView.ItemsSource = _taskStorage.GetAllTasks();
        }

        private void TasksListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TasksListView.SelectedItem is TaskItem task)
            {
                LoadTaskIntoEditor(task);
                ShowEditor();
            }
        }

        private void LoadTaskIntoEditor(TaskItem task)
        {
            _currentTask = task;
            TitleTextBox.Text = task.Title;
            DescriptionTextBox.Text = task.Description;
            PriorityComboBox.SelectedItem = task.Priority;
            DueDatePicker.SelectedDate = task.DueDate;
        }

        private void ShowEditor()
        {
            TitleTextBox.IsEnabled = true;
            DescriptionTextBox.IsEnabled = true;
            PriorityComboBox.IsEnabled = true;
            DueDatePicker.IsEnabled = true;
            NoSelectionMessage.Visibility = Visibility.Collapsed;
        }

        private void HideEditor()
        {
            TitleTextBox.IsEnabled = false;
            DescriptionTextBox.IsEnabled = false;
            PriorityComboBox.IsEnabled = false;
            DueDatePicker.IsEnabled = false;
            NoSelectionMessage.Visibility = Visibility.Visible;
        }

        private void ClearForm()
        {
            _currentTask = null;
            TitleTextBox.Clear();
            DescriptionTextBox.Clear();
            PriorityComboBox.SelectedIndex = 1;
            DueDatePicker.SelectedDate = null;
            TasksListView.SelectedItem = null;
            HideEditor();
        }

        private void NewTask_Click(object sender, RoutedEventArgs e)
        {
            _currentTask = null;
            TitleTextBox.Clear();
            DescriptionTextBox.Clear();
            PriorityComboBox.SelectedIndex = 1;
            DueDatePicker.SelectedDate = null;
            TasksListView.SelectedItem = null;
            ShowEditor();
            TitleTextBox.Focus();
        }

        private void SaveTask_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Please enter a task title.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_currentTask == null)
            {
                // Create new task
                var newTask = new TaskItem
                {
                    Title = TitleTextBox.Text.Trim(),
                    Description = DescriptionTextBox.Text.Trim(),
                    Priority = (TaskPriority)(PriorityComboBox.SelectedItem ?? TaskPriority.Normal),
                    DueDate = DueDatePicker.SelectedDate
                };
                _taskStorage.AddTask(newTask);
                MessageBox.Show("Task created successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Update existing task
                _currentTask.Title = TitleTextBox.Text.Trim();
                _currentTask.Description = DescriptionTextBox.Text.Trim();
                _currentTask.Priority = (TaskPriority)(PriorityComboBox.SelectedItem ?? TaskPriority.Normal);
                _currentTask.DueDate = DueDatePicker.SelectedDate;
                _taskStorage.UpdateTask(_currentTask);
                MessageBox.Show("Task updated successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }

            LoadTasks();
            ClearForm();
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (TasksListView.SelectedItem is TaskItem task)
            {
                var result = MessageBox.Show($"Are you sure you want to delete '{task.Title}'?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _taskStorage.DeleteTask(task.Id);
                    LoadTasks();
                    ClearForm();
                }
            }
            else
            {
                MessageBox.Show("Please select a task to delete.", "No Selection",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void TaskCompleted_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is TaskItem task)
            {
                _taskStorage.ToggleTaskCompletion(task.Id);
                LoadTasks();
            }
        }

        private void ClearForm_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }
    }
}
