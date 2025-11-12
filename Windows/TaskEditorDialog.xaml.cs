using System;
using System.Windows;
using DesktopTasks.Models;

namespace DesktopTasks.Windows
{
    public partial class TaskEditorDialog : Window
    {
        public TaskItem? TaskResult { get; private set; }
        private TaskItem? _existingTask;

        public TaskEditorDialog()
        {
            InitializeComponent();
            TitleTextBox.Focus();
        }

        public TaskEditorDialog(TaskItem existingTask) : this()
        {
            _existingTask = existingTask;
            Title = "Edit Task";
            HeaderText.Text = "Edit Task";

            // Populate fields with existing task data
            TitleTextBox.Text = existingTask.Title;
            DescriptionTextBox.Text = existingTask.Description;
            DueDatePicker.SelectedDate = existingTask.DueDate;

            // Set priority ComboBox
            PriorityComboBox.SelectedIndex = existingTask.Priority switch
            {
                TaskPriority.Low => 0,
                TaskPriority.High => 2,
                _ => 1
            };
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Validate title
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Please enter a task title.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TitleTextBox.Focus();
                return;
            }

            // Map ComboBox index to TaskPriority enum
            TaskPriority priority = PriorityComboBox.SelectedIndex switch
            {
                0 => TaskPriority.Low,
                2 => TaskPriority.High,
                _ => TaskPriority.Normal
            };

            // Create or update task item
            TaskResult = new TaskItem
            {
                Id = _existingTask?.Id ?? Guid.NewGuid(),
                Title = TitleTextBox.Text.Trim(),
                Description = DescriptionTextBox.Text.Trim(),
                Priority = priority,
                DueDate = DueDatePicker.SelectedDate,
                CreatedAt = _existingTask?.CreatedAt ?? DateTime.Now,
                IsCompleted = _existingTask?.IsCompleted ?? false
            };

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
