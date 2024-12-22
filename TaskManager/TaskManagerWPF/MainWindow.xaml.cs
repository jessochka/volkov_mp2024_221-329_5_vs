using System;
using System.Globalization;
using System.Windows;
using Microsoft.Win32;
using TaskManagerLibrary;

namespace TaskManagerWPF
{
    public partial class MainWindow : Window
    {
        private TaskManager manager;

        public MainWindow()
        {
            InitializeComponent();
            manager = new TaskManager();
            RefreshList();
        }

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
                    MessageBox.Show("Файл загружен");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки: " + ex.Message);
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
                    MessageBox.Show("Файл сохранён");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка сохранения: " + ex.Message);
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string title = txtTitle.Text.Trim();
            string desc = txtDescription.Text.Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Введите заголовок задачи");
                return;
            }
            var newTask = manager.AddTask(title, desc);

            string dlStr = txtDeadline.Text.Trim();
            if (!string.IsNullOrEmpty(dlStr))
            {
                if (DateTime.TryParseExact(dlStr, "dd.MM.yyyy",
                    CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime dt))
                {
                    newTask.SetDeadline(dt);
                }
                else
                {
                    MessageBox.Show("Неверный формат даты. Используйте дд.мм.гггг");
                }
            }

            RefreshList();
            ClearForm();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var sel = listTasks.SelectedItem as TaskItem;
            if (sel == null)
            {
                MessageBox.Show("Выберите задачу");
                return;
            }

            sel.Title = txtTitle.Text.Trim();
            sel.Description = txtDescription.Text.Trim();

            string dlStr = txtDeadline.Text.Trim();
            if (dlStr == "-")
            {
                sel.ClearDeadline();
            }
            else if (!string.IsNullOrWhiteSpace(dlStr))
            {
                if (DateTime.TryParseExact(dlStr, "dd.MM.yyyy",
                    CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime dt))
                {
                    sel.SetDeadline(dt);
                }
                else
                {
                    MessageBox.Show("Неверный формат даты, дедлайн не изменён");
                }
            }
            // Если поле пустое, оставляем прежний дедлайн

            RefreshList();
        }

        private void btnComplete_Click(object sender, RoutedEventArgs e)
        {
            var sel = listTasks.SelectedItem as TaskItem;
            if (sel == null)
            {
                MessageBox.Show("Выберите задачу");
                return;
            }
            sel.Complete();
            RefreshList();
        }

        private void btnReopen_Click(object sender, RoutedEventArgs e)
        {
            var sel = listTasks.SelectedItem as TaskItem;
            if (sel == null)
            {
                MessageBox.Show("Выберите задачу");
                return;
            }
            sel.Reopen();
            RefreshList();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var sel = listTasks.SelectedItem as TaskItem;
            if (sel == null)
            {
                MessageBox.Show("Выберите задачу");
                return;
            }
            manager.RemoveTask(sel.Id);
            RefreshList();
            ClearForm();
        }

        private void listTasks_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var sel = listTasks.SelectedItem as TaskItem;
            if (sel != null)
            {
                txtTitle.Text = sel.Title;
                txtDescription.Text = sel.Description;
                txtDeadline.Text = sel.Deadline.HasValue
                    ? sel.Deadline.Value.ToString("dd.MM.yyyy")
                    : "";
            }
        }

        private void RefreshList()
        {
            listTasks.ItemsSource = null;
            listTasks.ItemsSource = manager.GetAllTasks();
        }

        private void ClearForm()
        {
            txtTitle.Text = "";
            txtDescription.Text = "";
            txtDeadline.Text = "";
            listTasks.SelectedItem = null;
        }
    }
}
