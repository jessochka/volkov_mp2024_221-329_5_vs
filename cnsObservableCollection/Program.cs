using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace cnsObservableCollection
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var cities = new ObservableCollection<string>() { "Москва" };

            cities.CollectionChanged += Cities_CollectionChanged;

            Console.WriteLine("Команды:");
            Console.WriteLine("+ <город> - добавить город");
            Console.WriteLine("- <город> - удалить город");
            Console.WriteLine("?         - показать все города");
            Console.WriteLine("[Enter]   - выход");

            while (true)
            {
                var line = Console.ReadLine();
                if (String.IsNullOrEmpty(line))
                    break;
                var cmd = line.Split(' ');

                switch (cmd[0])
                {
                    case "+":
                        if (String.IsNullOrEmpty(cmd[1]))
                        {
                            Console.WriteLine("Название города не может быть пустой строкой!");
                            break;
                        }
                        cities.Add(cmd[1]);
                        break;
                    case "-":
                        if (String.IsNullOrEmpty(cmd[1]))
                        {
                            Console.WriteLine("Название города не может быть пустой строкой!");
                            break;
                        }
                        cities.Remove(cmd[1]);
                        break;
                    case "?":
                        Console.WriteLine($"Список городов ({cities.Count}):");
                        Console.WriteLine(String.Join(Environment.NewLine, cities));
                        break;
                    default:
                        Console.WriteLine("Неизвестная комада");
                        break;
                }
            }
        }

        private static void Cities_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Console.WriteLine($"Город {e.NewItems?[0]} был успешно добавлен");
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Console.WriteLine($"Город {e.OldItems?[0]} был успешно удален");
                    break;
                default:
                    Console.WriteLine("Какое-то непонятное что-то произошло со списком городов");
                    break;
            }
        }
    }
}
