// Form1.cs
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using ImageAlignmentLibrary;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace ImageAlignmentWFA
{
    public partial class Form1 : Form
    {
        private ImageProcessor? imageProcessor;
        private Image<Rgba32>? originalImage; // Хранение оригинального изображения для сброса

        public Form1()
        {
            InitializeComponent();

            // Подключение обработчиков событий
            btnLoadImage.Click += BtnLoadImage_Click;
            btnAutoAlign.Click += BtnAutoAlign_Click;
            tbAngle.Scroll += TbAngle_Scroll;
            btnRotate90.Click += BtnRotate90_Click;
            btnRotateMinus90.Click += BtnRotateMinus90_Click;
            chkAutoCrop.CheckedChanged += ChkAutoCrop_CheckedChanged;
            btnApplyCrop.Click += BtnApplyCrop_Click;
            chkGuidelines.CheckedChanged += ChkGuidelines_CheckedChanged;
            chkDiagonals.CheckedChanged += ChkDiagonals_CheckedChanged;
            pictureBox1.Paint += PictureBox1_Paint;
            txtAngle.KeyPress += TxtAngle_KeyPress;
            btnSaveImage.Click += BtnSaveImage_Click; // Новая кнопка сохранения
            this.Resize += Form1_Resize; // Обработчик изменения размера формы
        }

        /// <summary>
        /// Обработчик события загрузки изображения
        /// </summary>
        private void BtnLoadImage_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                imageProcessor?.Dispose();
                imageProcessor = new ImageProcessor();
                imageProcessor.LoadImage(ofd.FileName);
                originalImage = imageProcessor.OriginalImage.Clone(); // Сохраняем оригинал для сброса
                UpdatePicture();
                ResetControls();
            }
        }

        /// <summary>
        /// Обработчик события сброса изображения в исходное положение
        /// </summary>
        private void BtnAutoAlign_Click(object sender, EventArgs e)
        {
            if (imageProcessor == null || originalImage == null)
            {
                MessageBox.Show("Загрузите изображение сначала.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Сброс изображения к оригиналу
                imageProcessor.ResetImage(originalImage);
                tbAngle.Value = 0;
                txtAngle.Text = "0";
                UpdatePicture();
                MessageBox.Show("Изображение сброшено в исходное положение.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сбросе изображения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Обработчик события изменения трекбара угла поворота
        /// </summary>
        private void TbAngle_Scroll(object sender, EventArgs e)
        {
            if (imageProcessor == null)
                return;

            int angle = tbAngle.Value;
            txtAngle.Text = angle.ToString();
            imageProcessor.RotateImage(angle);
            UpdatePicture();
        }

        /// <summary>
        /// Обработчик события поворота на +90 градусов
        /// </summary>
        private void BtnRotate90_Click(object sender, EventArgs e)
        {
            if (imageProcessor == null)
                return;

            int currentAngle = tbAngle.Value;
            int newAngle = currentAngle + 90;

            if (newAngle > 180)
                newAngle -= 360;
            if (newAngle < -180)
                newAngle += 360;

            tbAngle.Value = newAngle;
            txtAngle.Text = newAngle.ToString();
            imageProcessor.RotateImage(newAngle);
            UpdatePicture();
        }

        /// <summary>
        /// Обработчик события поворота на -90 градусов
        /// </summary>
        private void BtnRotateMinus90_Click(object sender, EventArgs e)
        {
            if (imageProcessor == null)
                return;

            int currentAngle = tbAngle.Value;
            int newAngle = currentAngle - 90;

            if (newAngle > 180)
                newAngle -= 360;
            if (newAngle < -180)
                newAngle += 360;

            tbAngle.Value = newAngle;
            txtAngle.Text = newAngle.ToString();
            imageProcessor.RotateImage(newAngle);
            UpdatePicture();
        }

        /// <summary>
        /// Обработчик события изменения состояния чекбокса автообрезания
        /// </summary>
        private void ChkAutoCrop_CheckedChanged(object sender, EventArgs e)
        {
            if (imageProcessor != null)
            {
                imageProcessor.AutoCrop = chkAutoCrop.Checked;
            }
        }

        /// <summary>
        /// Обработчик события изменения состояния чекбоксов направляющих линий и диагоналей
        /// </summary>
        private void ChkGuidelines_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();
        }

        private void ChkDiagonals_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();
        }

        /// <summary>
        /// Обработчик события применения автообрезания
        /// </summary>
        private void BtnApplyCrop_Click(object sender, EventArgs e)
        {
            if (imageProcessor == null || imageProcessor.AlignedImage == null)
            {
                MessageBox.Show("Сначала загрузите и выровняйте изображение.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                imageProcessor.ApplyAutoCrop(); // Вызов метода автообрезки
                UpdatePicture();
                MessageBox.Show("Автообрезание выполнено успешно.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при автообрезании: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Обработчик события отрисовки PictureBox.
        /// Рисует направляющие линии и диагонали при включённых чекбоксах
        /// </summary>
        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (chkGuidelines.Checked)
            {
                DrawGuidelines(e.Graphics);
            }

            if (chkDiagonals.Checked)
            {
                DrawDiagonals(e.Graphics);
            }
        }

        /// <summary>
        /// Рисует направляющие линии
        /// </summary>
        /// <param name="graphics">Graphics объект</param>
        private void DrawGuidelines(Graphics graphics)
        {
            using (Pen pen = new Pen(System.Drawing.Color.Red, 1))
            {
                int centerX = pictureBox1.Width / 2;
                int centerY = pictureBox1.Height / 2;

                // Вертикальная линия
                graphics.DrawLine(pen, centerX, 0, centerX, pictureBox1.Height);
                // Горизонтальная линия
                graphics.DrawLine(pen, 0, centerY, pictureBox1.Width, centerY);
            }
        }

        /// <summary>
        /// Рисует диагональные направляющие линии
        /// </summary>
        /// <param name="graphics">Graphics объект</param>
        private void DrawDiagonals(Graphics graphics)
        {
            using (Pen pen = new Pen(System.Drawing.Color.Blue, 1))
            {
                graphics.DrawLine(pen, 0, 0, pictureBox1.Width, pictureBox1.Height);
                graphics.DrawLine(pen, pictureBox1.Width, 0, 0, pictureBox1.Height);
            }
        }

        /// <summary>
        /// Обработчик события ввода угла вручную в текстбокс
        /// </summary>
        private void TxtAngle_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешить только цифры, знак минус и клавишу Enter
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '-' && e.KeyChar != '\b')
            {
                e.Handled = true;
            }

            // Если нажата клавиша Enter, применить введённый угол
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (int.TryParse(txtAngle.Text, out int angle))
                {
                    // Ограничить значение трекбара
                    if (angle < tbAngle.Minimum) angle = tbAngle.Minimum;
                    if (angle > tbAngle.Maximum) angle = tbAngle.Maximum;

                    tbAngle.Value = angle;
                    if (imageProcessor != null)
                    {
                        imageProcessor.RotateImage(angle);
                        UpdatePicture();
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, введите корректный угол в градусах", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                e.Handled = true;
            }
        }

        /// <summary>
        /// Обновляет изображение в PictureBox
        /// </summary>
        private void UpdatePicture()
        {
            if (imageProcessor == null || imageProcessor.AlignedImage == null)
                return;

            // Освободить предыдущий Bitmap
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }

            // Преобразовать ImageSharp Image в Bitmap
            using var ms = new MemoryStream();
            imageProcessor.AlignedImage.SaveAsBmp(ms);
            ms.Seek(0, SeekOrigin.Begin);
            pictureBox1.Image = new Bitmap(ms);
        }

        /// <summary>
        /// Сбрасывает значения контролов после загрузки нового изображения
        /// </summary>
        private void ResetControls()
        {
            tbAngle.Value = 0;
            txtAngle.Text = "0";
            chkAutoCrop.Checked = false;
            chkGuidelines.Checked = false;
            chkDiagonals.Checked = false;
        }

        /// <summary>
        /// Обработчик события кнопки сохранения изображения
        /// </summary>
        private void BtnSaveImage_Click(object sender, EventArgs e)
        {
            if (imageProcessor == null || imageProcessor.AlignedImage == null)
            {
                MessageBox.Show("Нет обработанного изображения для сохранения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using var sfd = new SaveFileDialog();
            sfd.Filter = "JPEG Image|*.jpg|PNG Image|*.png|Bitmap Image|*.bmp";
            sfd.Title = "Сохранить обработанное изображение";
            sfd.FileName = "AlignedImage";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    imageProcessor.SaveAlignedImage(sfd.FileName);
                    MessageBox.Show("Изображение успешно сохранено.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении изображения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Обработчик события изменения размера формы для масштабирования интерфейса
        /// </summary>
        private void Form1_Resize(object sender, EventArgs e)
        {
            // Автоматическое масштабирование элементов управлени
            // Можно дополнительно настроить при необходимости
            pictureBox1.Width = this.ClientSize.Width - pictureBox1.Left - 10;
            pictureBox1.Height = this.ClientSize.Height - pictureBox1.Top - 10;
        }
    }
}