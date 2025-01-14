using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using TaskManagerLibrary;
using System.Windows.Controls;
using System.Windows.Media;

namespace TaskManagerWPF
{
    public partial class MainWindow : Window
    {
        private TaskManager manager;
        private bool isDarkTheme = false; // флаг текущей темы

        public MainWindow()
        {
            InitializeComponent();
            manager = new TaskManager();
            RefreshList();
        }

        // Темы
        private void btnToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            isDarkTheme = !isDarkTheme;
            if (isDarkTheme)
            {
                // Установить тёмную тему
                Resources["WindowBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(40, 40, 40));
                Resources["WindowForegroundBrush"] = new SolidColorBrush(Colors.White);
                Resources["PanelBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(60, 60, 60));
                Resources["PanelForegroundBrush"] = new SolidColorBrush(Colors.White);

                btnToggleTheme.Content = "Светлая тема";
            }
            else
            {
                // Вернуть светлую тему
                Resources["WindowBackgroundBrush"] = new SolidColorBrush(Colors.White);
                Resources["WindowForegroundBrush"] = new SolidColorBrush(Colors.Black);
                Resources["PanelBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(238, 238, 238));
                Resources["PanelForegroundBrush"] = new SolidColorBrush(Colors.Black);

                btnToggleTheme.Content = "Тёмная тема";
            }
        }

        // Кнопки файлов
        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "JSON files|*.json|All files|*.*"
            };
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    manager.LoadFromFile(ofd.FileName);
                    RefreshList();
                    ClearForm();
                    MessageBox.Show("Файл загружен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                Filter = "JSON files|*.json|All files|*.*",
                FileName = "tasks.json"
            };
            if (sfd.ShowDialog() == true)
            {
                try
                {
                    manager.SaveToFile(sfd.FileName);
                    MessageBox.Show("Файл сохранён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка сохранения: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Выбор картинки
        private void btnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            // Выбираем картинку
            var ofd = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.png;*.jpeg;*.bmp|All files|*.*"
            };
            if (ofd.ShowDialog() == true)
            {
                txtImagePath.Text = ofd.FileName;
            }
        }

        // Добавление задачи
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string title = txtTitle.Text.Trim();
            string desc = txtDescription.Text.Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Введите заголовок задачи", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ImportanceLevel importance = ParseImportanceCombo();
            var newTask = manager.AddTask(title, desc, importance);

            if (dpDeadline.SelectedDate.HasValue)
            {
                newTask.SetDeadline(dpDeadline.SelectedDate.Value);
            }

            // Запишем путь к картинке, если выбран
            if (!string.IsNullOrEmpty(txtImagePath.Text))
            {
                newTask.ImagePath = txtImagePath.Text;
            }

            RefreshList();
            ClearForm();
        }

        // Изменение задачи
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var sel = listTasks.SelectedItem as TaskItem;
            if (sel == null)
            {
                MessageBox.Show("Выберите задачу", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            sel.Title = txtTitle.Text.Trim();
            sel.Description = txtDescription.Text.Trim();

            if (dpDeadline.SelectedDate.HasValue)
            {
                sel.SetDeadline(dpDeadline.SelectedDate.Value);
            }
            else
            {
                sel.ClearDeadline();
            }

            sel.Importance = ParseImportanceCombo();

            // Обновим изображение
            sel.ImagePath = string.IsNullOrEmpty(txtImagePath.Text) ? null : txtImagePath.Text;

            RefreshList();
        }

        // Выполнение задачи
        private void btnComplete_Click(object sender, RoutedEventArgs e)
        {
            var sel = listTasks.SelectedItem as TaskItem;
            if (sel == null)
            {
                MessageBox.Show("Выберите задачу", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            sel.Complete();
            RefreshList();
        }

        // Не выполнение
        private void btnReopen_Click(object sender, RoutedEventArgs e)
        {
            var sel = listTasks.SelectedItem as TaskItem;
            if (sel == null)
            {
                MessageBox.Show("Выберите задачу", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            sel.Reopen();
            RefreshList();
        }

        // Удаление задачи
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var sel = listTasks.SelectedItem as TaskItem;
            if (sel == null)
            {
                MessageBox.Show("Выберите задачу", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var result = MessageBox.Show($"Удалить задачу '{sel.Title}'?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                manager.RemoveTask(sel.Id);
                RefreshList();
                ClearForm();
            }
        }

        // Филтр
        private void btnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            RefreshList();
        }

        // Раскрытие деталей
        private void listTasks_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var sel = listTasks.SelectedItem as TaskItem;
            if (sel != null)
            {
                TaskDetailsWindow detailsWindow = new TaskDetailsWindow(sel);
                detailsWindow.Owner = this;
                detailsWindow.ShowDialog();
            }
        }

        // Добавление характеристики задачи в главном меню
        private void listTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sel = listTasks.SelectedItem as TaskItem;
            if (sel != null)
            {
                txtTitle.Text = sel.Title;
                txtDescription.Text = sel.Description;
                dpDeadline.SelectedDate = sel.Deadline;

                var tag = sel.Importance.ToString(); // "High"/"Medium"/"Low"
                foreach (ComboBoxItem item in cmbImportance.Items)
                {
                    if ((string)item.Tag == tag)
                    {
                        cmbImportance.SelectedItem = item;
                        break;
                    }
                }

                // Покажем путь картинки
                txtImagePath.Text = sel.ImagePath ?? "";
            }
            else
            {
                ClearForm();
            }
        }

        // Обновление списка
        public void RefreshList()
        {
            listTasks.ItemsSource = null;
            var tasks = manager.GetAllTasks();

            // Фильтр по важности
            if (cmbFilterImportance.SelectedItem is ComboBoxItem filterItem)
            {
                string tag = filterItem.Tag as string;
                if (tag != "All")
                {
                    if (Enum.TryParse<ImportanceLevel>(tag, out var importance))
                    {
                        tasks = tasks.Where(t => t.Importance == importance).ToList();
                    }
                }
            }

            // Показ/скрыть выполненные
            if (chkShowCompleted.IsChecked == false)
            {
                tasks = tasks.Where(t => !t.IsCompleted).ToList();
            }

            listTasks.ItemsSource = tasks;
        }

        // Очистка формы
        private void ClearForm()
        {
            txtTitle.Text = "";
            txtDescription.Text = "";
            dpDeadline.SelectedDate = null;
            cmbImportance.SelectedIndex = 1; // Средняя
            txtImagePath.Text = "";
            listTasks.SelectedItem = null;
        }

        // Сортировка по важности
        private ImportanceLevel ParseImportanceCombo()
        {
            if (cmbImportance.SelectedItem is ComboBoxItem selItem)
            {
                string tag = selItem.Tag as string;
                return tag switch
                {
                    "High" => ImportanceLevel.High,
                    "Medium" => ImportanceLevel.Medium,
                    "Low" => ImportanceLevel.Low,
                    _ => ImportanceLevel.Medium
                };
            }
            return ImportanceLevel.Medium;
        }
    }
}
