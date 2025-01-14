using System;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using TaskManagerLibrary;

namespace TaskManagerWPF
{
    public partial class TaskDetailsWindow : Window
    {
        private TaskItem task;
        private bool editMode = false; // Режим редактирования и просмотра

        public TaskDetailsWindow(TaskItem task)
        {
            InitializeComponent();
            this.task = task;
            DisplayTaskDetails();
        }

        // Отобразить поля в режиме чтения
        private void DisplayTaskDetails()
        {
            editMode = false;
            ToggleEditFields(false);

            txtTitleBlock.Text = task.Title;
            txtDescriptionBlock.Text = task.Description;
            txtDeadlineBlock.Text = task.Deadline.HasValue
                ? task.Deadline.Value.ToString("dd.MM.yyyy")
                : "(нет дедлайна)";
            txtImportanceBlock.Text = task.Importance.ToString();
            txtStatusBlock.Text = task.IsCompleted ? "Выполнено" : "Не выполнено";

            if (!string.IsNullOrEmpty(task.ImagePath))
            {
                try
                {
                    var bitmap = new BitmapImage(new Uri(task.ImagePath));
                    imgPreview.Source = bitmap;
                }
                catch
                {
                    imgPreview.Source = null;
                }
            }
            else
            {
                imgPreview.Source = null;
            }
        }

        // Переключение элементов режима редактирования
        private void ToggleEditFields(bool editing)
        {
            // Title
            txtTitleBlock.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            txtTitleEdit.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;

            // Description
            txtDescriptionBlock.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            txtDescriptionEdit.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;

            // Deadline
            txtDeadlineBlock.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            dpDeadlineEdit.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;

            // Importance
            txtImportanceBlock.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            cmbImportanceEdit.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;

            // Image
            imgPreview.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            spImageEditPanel.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;

            // Buttons
            btnEdit.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
            btnSave.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
            btnCancel.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
        }

        // Переключение в сам режим редактирования
        private void SwitchToEditMode()
        {
            editMode = true;
            ToggleEditFields(true);

            // Заполняем поля редактирования
            txtTitleEdit.Text = task.Title;
            txtDescriptionEdit.Text = task.Description;
            dpDeadlineEdit.SelectedDate = task.Deadline;

            var tag = task.Importance.ToString(); // "High"/"Medium"/"Low"
            foreach (var item in cmbImportanceEdit.Items)
            {
                if (item is System.Windows.Controls.ComboBoxItem cbi && (string)cbi.Tag == tag)
                {
                    cmbImportanceEdit.SelectedItem = cbi;
                    break;
                }
            }

            txtImageEdit.Text = task.ImagePath ?? "";
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            SwitchToEditMode();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Сохранить изменения обратно в task
            task.Title = txtTitleEdit.Text.Trim();
            task.Description = txtDescriptionEdit.Text.Trim();

            if (dpDeadlineEdit.SelectedDate.HasValue)
                task.SetDeadline(dpDeadlineEdit.SelectedDate.Value);
            else
                task.ClearDeadline();

            if (cmbImportanceEdit.SelectedItem is System.Windows.Controls.ComboBoxItem cbi)
            {
                var tg = (string)cbi.Tag;
                if (tg == "High") task.Importance = ImportanceLevel.High;
                else if (tg == "Medium") task.Importance = ImportanceLevel.Medium;
                else if (tg == "Low") task.Importance = ImportanceLevel.Low;
            }

            task.ImagePath = string.IsNullOrEmpty(txtImageEdit.Text) ? null : txtImageEdit.Text;

            // Возвращаемся в режим просмотра
            DisplayTaskDetails();

            // Обновление списка 
            if (this.Owner is MainWindow mainWin)
            {
                mainWin.RefreshList();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DisplayTaskDetails();
        }

        private void btnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.png;*.jpeg;*.bmp|All files|*.*"
            };
            if (ofd.ShowDialog() == true)
            {
                txtImageEdit.Text = ofd.FileName;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            // Обновляем список на главном окне при изменениях

            if (this.Owner is MainWindow mainWin)
            {
                mainWin.RefreshList();
            }

            this.Close();
        }
    }
}
