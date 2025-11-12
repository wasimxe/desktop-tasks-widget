using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using DesktopTasks.Models;

namespace DesktopTasks.Services
{
    public class TaskStorageService
    {
        private readonly string _dataFilePath;
        private List<TaskItem> _tasks;

        public event EventHandler? TasksChanged;

        public TaskStorageService()
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DesktopTasks"
            );
            Directory.CreateDirectory(appDataPath);
            _dataFilePath = Path.Combine(appDataPath, "tasks.json");
            _tasks = LoadTasks();
        }

        private List<TaskItem> LoadTasks()
        {
            if (!File.Exists(_dataFilePath))
                return new List<TaskItem>();

            try
            {
                var json = File.ReadAllText(_dataFilePath);
                return JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
            }
            catch
            {
                return new List<TaskItem>();
            }
        }

        private void SaveTasks()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_tasks, options);
            File.WriteAllText(_dataFilePath, json);
            TasksChanged?.Invoke(this, EventArgs.Empty);
        }

        public List<TaskItem> GetAllTasks() => _tasks.ToList();

        public List<TaskItem> GetActiveTasks() => _tasks.Where(t => !t.IsCompleted).OrderBy(t => t.Priority).ToList();

        public void AddTask(TaskItem task)
        {
            _tasks.Add(task);
            SaveTasks();
        }

        public void UpdateTask(TaskItem task)
        {
            var index = _tasks.FindIndex(t => t.Id == task.Id);
            if (index >= 0)
            {
                _tasks[index] = task;
                SaveTasks();
            }
        }

        public void DeleteTask(Guid taskId)
        {
            _tasks.RemoveAll(t => t.Id == taskId);
            SaveTasks();
        }

        public void ToggleTaskCompletion(Guid taskId)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                task.IsCompleted = !task.IsCompleted;
                SaveTasks();
            }
        }
    }
}
