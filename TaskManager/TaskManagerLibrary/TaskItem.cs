using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TaskManagerLibrary
{
    public enum ImportanceLevel
    {
        High,
        Medium,
        Low
    }

    public class TaskItem
    {
        public Guid Id { get; private set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; private set; }
        public string? ImagePath { get; set; } = null;

        /// Дедлайн в формате (дд.мм.гггг)
        public DateTime? Deadline { get; private set; }

        /// Важность задачи
        public ImportanceLevel Importance { get; set; }

        // Конструктор для новых задач
        public TaskItem(string title, string description, ImportanceLevel importance = ImportanceLevel.Medium)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            IsCompleted = false;
            Deadline = null;
            Importance = importance;
        }

        // Пустой конструктор (для JSON-сериализации)
        public TaskItem()
        {
            Id = Guid.NewGuid();
            Title = "";
            Description = "";
            Importance = ImportanceLevel.Medium;
        }

        public void Complete() => IsCompleted = true;
        public void Reopen() => IsCompleted = false;

        public void SetDeadline(DateTime date) => Deadline = date;
        public void ClearDeadline() => Deadline = null;

        public override string ToString()
        {
            string doneMark = IsCompleted ? "X" : " ";
            string dd = Deadline.HasValue
                ? $" (Deadline: {Deadline.Value:dd.MM.yyyy})"
                : "";
            return $"[{doneMark}] {Title}{dd}";
        }
    }

    public class TaskManager
    {
        private List<TaskItem> tasks = new List<TaskItem>();

        public TaskItem AddTask(string title, string description, ImportanceLevel importance = ImportanceLevel.Medium)
        {
            var t = new TaskItem(title, description, importance);
            tasks.Add(t);
            return t;
        }

        public bool RemoveTask(Guid id)
        {
            var t = tasks.Find(x => x.Id == id);
            if (t != null)
            {
                tasks.Remove(t);
                return true;
            }
            return false;
        }

        public TaskItem? FindTask(Guid id)
        {
            return tasks.Find(x => x.Id == id);
        }

        public List<TaskItem> GetAllTasks()
        {
            return new List<TaskItem>(tasks);
        }

        // Загрузка списка задач из JSON-файла
        public void LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден", filePath);

            string json = File.ReadAllText(filePath);
            var loaded = JsonSerializer.Deserialize<List<TaskItem>>(json);
            if (loaded != null)
            {
                tasks = loaded;
            }
        }

        // Сохранение списка задач в JSON-файл
        public void SaveToFile(string filePath)
        {
            var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(filePath, json);
        }
    }
}
