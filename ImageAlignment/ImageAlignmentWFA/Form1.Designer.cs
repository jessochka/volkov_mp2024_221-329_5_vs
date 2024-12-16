namespace ImageAlignmentWFA
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Очистка ресурсов
        /// </summary>
        /// <param name="disposing">Указывает, нужно ли очищать управляемые ресурсы</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                imageProcessor?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        private void InitializeComponent()
        {
            pictureBox1 = new PictureBox();
            btnLoadImage = new Button();
            btnAutoAlign = new Button();
            tbAngle = new TrackBar();
            txtAngle = new TextBox();
            btnRotate90 = new Button();
            btnRotateMinus90 = new Button();
            chkGuidelines = new CheckBox();
            chkDiagonals = new CheckBox();
            btnSaveImage = new Button();
            btnApplyCrop = new Button();
            chkAutoCrop = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tbAngle).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.Location = new Point(250, 10);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(800, 600);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // btnLoadImage
            // 
            btnLoadImage.Location = new Point(10, 10);
            btnLoadImage.Name = "btnLoadImage";
            btnLoadImage.Size = new Size(100, 30);
            btnLoadImage.TabIndex = 1;
            btnLoadImage.Text = "Загрузить";
            btnLoadImage.UseVisualStyleBackColor = true;
            // 
            // btnAutoAlign
            // 
            btnAutoAlign.Location = new Point(10, 200);
            btnAutoAlign.Name = "btnAutoAlign";
            btnAutoAlign.Size = new Size(150, 30);
            btnAutoAlign.TabIndex = 2;
            btnAutoAlign.Text = "Сбросить изображение";
            btnAutoAlign.UseVisualStyleBackColor = true;
            // 
            // tbAngle
            // 
            tbAngle.Location = new Point(10, 250);
            tbAngle.Maximum = 180;
            tbAngle.Minimum = -180;
            tbAngle.Name = "tbAngle";
            tbAngle.Size = new Size(160, 45);
            tbAngle.TabIndex = 3;
            // 
            // txtAngle
            // 
            txtAngle.Location = new Point(10, 300);
            txtAngle.Name = "txtAngle";
            txtAngle.Size = new Size(60, 23);
            txtAngle.TabIndex = 4;
            txtAngle.Text = "0";
            // 
            // btnRotate90
            // 
            btnRotate90.Location = new Point(10, 340);
            btnRotate90.Name = "btnRotate90";
            btnRotate90.Size = new Size(75, 30);
            btnRotate90.TabIndex = 5;
            btnRotate90.Text = "+90°";
            btnRotate90.UseVisualStyleBackColor = true;
            // 
            // btnRotateMinus90
            // 
            btnRotateMinus90.Location = new Point(95, 340);
            btnRotateMinus90.Name = "btnRotateMinus90";
            btnRotateMinus90.Size = new Size(75, 30);
            btnRotateMinus90.TabIndex = 6;
            btnRotateMinus90.Text = "-90°";
            btnRotateMinus90.UseVisualStyleBackColor = true;
            // 
            // chkGuidelines
            // 
            chkGuidelines.AutoSize = true;
            chkGuidelines.Location = new Point(10, 420);
            chkGuidelines.Name = "chkGuidelines";
            chkGuidelines.Size = new Size(152, 19);
            chkGuidelines.TabIndex = 8;
            chkGuidelines.Text = "Направляющие линии";
            chkGuidelines.UseVisualStyleBackColor = true;
            // 
            // chkDiagonals
            // 
            chkDiagonals.AutoSize = true;
            chkDiagonals.Location = new Point(10, 450);
            chkDiagonals.Name = "chkDiagonals";
            chkDiagonals.Size = new Size(175, 19);
            chkDiagonals.TabIndex = 9;
            chkDiagonals.Text = "Направляющие диагонали";
            chkDiagonals.UseVisualStyleBackColor = true;
            // 
            // btnSaveImage
            // 
            btnSaveImage.Location = new Point(10, 535);
            btnSaveImage.Name = "btnSaveImage";
            btnSaveImage.Size = new Size(173, 48);
            btnSaveImage.TabIndex = 11;
            btnSaveImage.Text = "Сохранить изображение";
            btnSaveImage.UseVisualStyleBackColor = true;
            // 
            // btnApplyCrop
            // 
            btnApplyCrop.Location = new Point(10, 475);
            btnApplyCrop.Name = "btnApplyCrop";
            btnApplyCrop.Size = new Size(150, 24);
            btnApplyCrop.TabIndex = 12;
            btnApplyCrop.Text = "Применить обрезку";
            btnApplyCrop.UseVisualStyleBackColor = true;
            // 
            // chkAutoCrop
            // 
            chkAutoCrop.AutoSize = true;
            chkAutoCrop.Location = new Point(10, 395);
            chkAutoCrop.Name = "chkAutoCrop";
            chkAutoCrop.Size = new Size(72, 19);
            chkAutoCrop.TabIndex = 13;
            chkAutoCrop.Text = "Обрезка";
            chkAutoCrop.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            ClientSize = new Size(1060, 630);
            Controls.Add(chkAutoCrop);
            Controls.Add(btnApplyCrop);
            Controls.Add(btnSaveImage);
            Controls.Add(chkDiagonals);
            Controls.Add(chkGuidelines);
            Controls.Add(btnRotateMinus90);
            Controls.Add(btnRotate90);
            Controls.Add(txtAngle);
            Controls.Add(tbAngle);
            Controls.Add(btnAutoAlign);
            Controls.Add(btnLoadImage);
            Controls.Add(pictureBox1);
            Name = "Form1";
            Text = "Автовыравнивание изображения";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)tbAngle).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnLoadImage;
        private System.Windows.Forms.Button btnAutoAlign;
        private System.Windows.Forms.TrackBar tbAngle;
        private System.Windows.Forms.TextBox txtAngle;
        private System.Windows.Forms.Button btnRotate90;
        private System.Windows.Forms.Button btnRotateMinus90;
        private System.Windows.Forms.CheckBox chkGuidelines;
        private System.Windows.Forms.CheckBox chkDiagonals;
        private System.Windows.Forms.Button btnSaveImage;
        private Button btnApplyCrop;
        private CheckBox chkAutoCrop;
    }
}