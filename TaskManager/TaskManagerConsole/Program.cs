using System;
using System.Globalization;
using System.IO;
using TaskManagerLibrary;

namespace TaskManagerConsole
{
    internal class Program
    {
        private static TaskManager manager = new TaskManager();
        private static bool exit = false;

        static void Main(string[] args)
        {
            Console.WriteLine("=== Консольный Task Manager с дедлайнами и файлами ===");

            while(!exit)
            {
                ShowMenu();
                Console.Write("Введите команду: ");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1": ShowAllTasks(); break;
                    case "2": AddNewTask(); break;
                    case "3": RemoveTask(); break;
                    case "4": CompleteTask(); break;
                    case "5": ReopenTask(); break;
                    case "6": ShowTaskDetails(); break;
                    case "7": EditTask(); break;
                    case "8": exit=true; break;
                    case "9": LoadFromFile(); break;
                    case "10": SaveToFile(); break;
                    default:
                        Console.WriteLine("Неизвестная команда");
                        break;
                }
            }

            Console.WriteLine("Программа завершена. Нажмите любую клавишу...");
            Console.ReadKey();
        }

        static void ShowMenu()
        {
            Console.WriteLine("\n--- МЕНЮ ---");
            Console.WriteLine("1) Показать все задачи");
            Console.WriteLine("2) Добавить задачу");
            Console.WriteLine("3) Удалить задачу");
            Console.WriteLine("4) Завершить задачу");
            Console.WriteLine("5) Сделать задачу незавершённой");
            Console.WriteLine("6) Детали задачи");
            Console.WriteLine("7) Редактировать задачу");
            Console.WriteLine("8) Выход");
            Console.WriteLine("9) Загрузить задачи из файла");
            Console.WriteLine("10) Сохранить задачи в файл");
        }

        static void ShowAllTasks()
        {
            var tasks = manager.GetAllTasks();
            if(tasks.Count==0)
            {
                Console.WriteLine("Список пуст");
                return;
            }
            Console.WriteLine("\n--- Список задач ---");
            foreach(var t in tasks)
            {
                Console.WriteLine(t);
            }
        }

        static void AddNewTask()
        {
            Console.Write("Заголовок: ");
            string? title= Console.ReadLine();
            if(string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Пустой заголовок");
                return;
            }
            Console.Write("Описание: ");
            string? desc= Console.ReadLine()??"";

            var task = manager.AddTask(title,desc);

            Console.Write("Дедлайн (dd.MM.yyyy) или Enter: ");
            string? dlStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(dlStr))
            {
                if (DateTime.TryParseExact(dlStr, "dd.MM.yyyy",
                    CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime dt))
                {
                    task.SetDeadline(dt);
                }
                else
                {
                    Console.WriteLine("Неверный формат. Пример: 26.05.2004");
                }
            }
            Console.WriteLine($"Добавлена задача: {task}");
        }

        static void RemoveTask()
        {
            ShowAllTasks();
            Console.Write("\nID для удаления: ");
            string? input= Console.ReadLine();
            if(!Guid.TryParse(input, out Guid id))
            {
                Console.WriteLine("Неверный Guid");
                return;
            }
            bool ok= manager.RemoveTask(id);
            Console.WriteLine(ok?"Удалено":"Не найдено");
        }

        static void CompleteTask()
        {
            ShowAllTasks();
            Console.Write("\nID задачи для завершения: ");
            string? input= Console.ReadLine();
            if(!Guid.TryParse(input, out Guid id))
            {
                Console.WriteLine("Неверный Guid");
                return;
            }
            var t= manager.FindTask(id);
            if(t==null)
            {
                Console.WriteLine("Не найдена");
                return;
            }
            t.Complete();
            Console.WriteLine("Завершена");
        }

        static void ReopenTask()
        {
            ShowAllTasks();
            Console.Write("\nID задачи для отмены завершения: ");
            string? input= Console.ReadLine();
            if(!Guid.TryParse(input, out Guid id))
            {
                Console.WriteLine("Неверный Guid");
                return;
            }
            var t= manager.FindTask(id);
            if(t==null)
            {
                Console.WriteLine("Не найдена");
                return;
            }
            t.Reopen();
            Console.WriteLine("Теперь незавершённая");
        }

        static void ShowTaskDetails()
        {
            ShowAllTasks();
            Console.Write("\nID задачи: ");
            string? input= Console.ReadLine();
            if(!Guid.TryParse(input, out Guid id))
            {
                Console.WriteLine("Неверный Guid");
                return;
            }
            var t= manager.FindTask(id);
            if(t==null)
            {
                Console.WriteLine("Не найдена");
                return;
            }
            Console.WriteLine("\n--- Детали ---");
            Console.WriteLine($"ID: {t.Id}");
            Console.WriteLine($"Title: {t.Title}");
            Console.WriteLine($"Desc: {t.Description}");
            Console.WriteLine($"Done?: {(t.IsCompleted?"Да":"Нет")}");
            if(t.Deadline.HasValue)
                Console.WriteLine($"Deadline: {t.Deadline.Value:dd.MM.yyyy}");
            else
                Console.WriteLine("Deadline: нет");
        }

        static void EditTask()
        {
            ShowAllTasks();
            Console.Write("\nID задачи для редактирования: ");
            string? input= Console.ReadLine();
            if(!Guid.TryParse(input, out Guid id))
            {
                Console.WriteLine("Неверный Guid");
                return;
            }
            var t= manager.FindTask(id);
            if(t==null)
            {
                Console.WriteLine("Не найдена");
                return;
            }

            Console.WriteLine($"Текущий заголовок: {t.Title}");
            Console.Write("Новый заголовок (Enter, если без изменений): ");
            string? newTitle= Console.ReadLine();
            if(!string.IsNullOrWhiteSpace(newTitle))
                t.Title= newTitle;

            Console.WriteLine($"Текущее описание: {t.Description}");
            Console.Write("Новое описание (Enter, если без изменений): ");
            string? newDesc= Console.ReadLine();
            if(newDesc!=null)
                t.Description= newDesc;
            if (t.Deadline.HasValue)
                Console.WriteLine($"Текущий дедлайн: {t.Deadline.Value:dd.MM.yyyy}");
            else
                Console.WriteLine("Дедлайн не установлен");

            Console.Write("Новый дедлайн (dd.MM.yyyy), '-'=очистить, Enter=не менять: ");
            string? dl = Console.ReadLine();
            if (dl == "-")
            {
                t.ClearDeadline();
                Console.WriteLine("Дедлайн очищен");
            }
            else if (!string.IsNullOrWhiteSpace(dl))
            {
                if (DateTime.TryParseExact(dl, "dd.MM.yyyy",
                    CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime dt))
                {
                    t.SetDeadline(dt);
                    Console.WriteLine("Дедлайн обновлён");
                }
                else
                {
                    Console.WriteLine("Неверный формат (ожидается dd.MM.yyyy)");
                }
            }
            Console.WriteLine("Изменения внесены");
        }

        static void LoadFromFile()
        {
            Console.Write("Введите путь к файлу JSON: ");
            string? path = Console.ReadLine();
            if(string.IsNullOrWhiteSpace(path)) 
            {
                Console.WriteLine("Путь не может быть пустым");
                return;
            }

            try
            {
                manager.LoadFromFile(path);
                Console.WriteLine("Файл успешно загружен");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Ошибка: "+ex.Message);
            }
        }

        static void SaveToFile()
        {
            Console.Write("Введите путь для сохранения JSON: ");
            string? path= Console.ReadLine();
            if(string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine("Путь не может быть пустым");
                return;
            }

            try
            {
                manager.SaveToFile(path);
                Console.WriteLine("Файл сохранён");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Ошибка: "+ex.Message);
            }
        }
    }
}
