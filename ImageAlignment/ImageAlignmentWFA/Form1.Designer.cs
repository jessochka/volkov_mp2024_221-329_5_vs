namespace ImageAlignmentWFA
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnLoadImage;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.TrackBar tbAngle;
        private System.Windows.Forms.TextBox txtAngle;
        private System.Windows.Forms.Button btnRotate90;
        private System.Windows.Forms.Button btnRotateMinus90;
        private System.Windows.Forms.CheckBox chkGuidelines;
        private System.Windows.Forms.CheckBox chkDiagonals;
        private System.Windows.Forms.Button btnSaveImage;
        private System.Windows.Forms.Button btnAutoAlign;
        private System.Windows.Forms.Button btnToggleContour;
        private System.Windows.Forms.Button btnSaveAs;
        private System.Windows.Forms.CheckBox chkAutoCrop;
        private System.Windows.Forms.Label lblOperationTime;
        private System.Windows.Forms.Label lblImagePath;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                imageProcessor?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pictureBox1 = new PictureBox();
            btnLoadImage = new Button();
            btnReset = new Button();
            tbAngle = new TrackBar();
            txtAngle = new TextBox();
            btnRotate90 = new Button();
            btnRotateMinus90 = new Button();
            chkGuidelines = new CheckBox();
            chkDiagonals = new CheckBox();
            btnSaveImage = new Button();
            btnAutoAlign = new Button();
            btnToggleContour = new Button();
            btnSaveAs = new Button();
            chkAutoCrop = new CheckBox();
            lblOperationTime = new Label();
            lblImagePath = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tbAngle).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.Location = new Point(286, 10);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(847, 659);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // btnLoadImage
            // 
            btnLoadImage.Location = new Point(10, 12);
            btnLoadImage.Name = "btnLoadImage";
            btnLoadImage.Size = new Size(150, 42);
            btnLoadImage.TabIndex = 1;
            btnLoadImage.Text = "Загрузить";
            btnLoadImage.UseVisualStyleBackColor = true;
            // 
            // btnReset
            // 
            btnReset.Location = new Point(10, 60);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(150, 42);
            btnReset.TabIndex = 2;
            btnReset.Text = "Сбросить";
            btnReset.UseVisualStyleBackColor = true;
            // 
            // tbAngle
            // 
            tbAngle.Location = new Point(1, 120);
            tbAngle.Maximum = 180;
            tbAngle.Minimum = -180;
            tbAngle.Name = "tbAngle";
            tbAngle.Size = new Size(231, 69);
            tbAngle.TabIndex = 3;
            // 
            // txtAngle
            // 
            txtAngle.Location = new Point(238, 120);
            txtAngle.Name = "txtAngle";
            txtAngle.Size = new Size(41, 31);
            txtAngle.TabIndex = 4;
            txtAngle.Text = "0";
            // 
            // btnRotate90
            // 
            btnRotate90.Location = new Point(27, 174);
            btnRotate90.Name = "btnRotate90";
            btnRotate90.Size = new Size(75, 30);
            btnRotate90.TabIndex = 5;
            btnRotate90.Text = "+90°";
            btnRotate90.UseVisualStyleBackColor = true;
            // 
            // btnRotateMinus90
            // 
            btnRotateMinus90.Location = new Point(123, 174);
            btnRotateMinus90.Name = "btnRotateMinus90";
            btnRotateMinus90.Size = new Size(75, 30);
            btnRotateMinus90.TabIndex = 6;
            btnRotateMinus90.Text = "-90°";
            btnRotateMinus90.UseVisualStyleBackColor = true;
            // 
            // chkGuidelines
            // 
            chkGuidelines.Location = new Point(9, 219);
            chkGuidelines.Name = "chkGuidelines";
            chkGuidelines.Size = new Size(270, 35);
            chkGuidelines.TabIndex = 7;
            chkGuidelines.Text = "Направляющие линии";
            chkGuidelines.UseVisualStyleBackColor = true;
            // 
            // chkDiagonals
            // 
            chkDiagonals.Location = new Point(9, 254);
            chkDiagonals.Name = "chkDiagonals";
            chkDiagonals.Size = new Size(270, 39);
            chkDiagonals.TabIndex = 8;
            chkDiagonals.Text = "Направляющие диагонали";
            chkDiagonals.UseVisualStyleBackColor = true;
            // 
            // btnSaveImage
            // 
            btnSaveImage.Location = new Point(158, 639);
            btnSaveImage.Name = "btnSaveImage";
            btnSaveImage.Size = new Size(122, 30);
            btnSaveImage.TabIndex = 9;
            btnSaveImage.Text = "Сохранить";
            btnSaveImage.UseVisualStyleBackColor = true;
            // 
            // btnAutoAlign
            // 
            btnAutoAlign.Location = new Point(9, 335);
            btnAutoAlign.Name = "btnAutoAlign";
            btnAutoAlign.Size = new Size(188, 44);
            btnAutoAlign.TabIndex = 10;
            btnAutoAlign.Text = "Автовыравнивание";
            btnAutoAlign.UseVisualStyleBackColor = true;
            // 
            // btnToggleContour
            // 
            btnToggleContour.Location = new Point(10, 421);
            btnToggleContour.Name = "btnToggleContour";
            btnToggleContour.Size = new Size(188, 38);
            btnToggleContour.TabIndex = 12;
            btnToggleContour.Text = "Скрыть контур";
            btnToggleContour.UseVisualStyleBackColor = true;
            // 
            // btnSaveAs
            // 
            btnSaveAs.Location = new Point(1, 639);
            btnSaveAs.Name = "btnSaveAs";
            btnSaveAs.Size = new Size(150, 30);
            btnSaveAs.TabIndex = 15;
            btnSaveAs.Text = "Сохранить как";
            btnSaveAs.UseVisualStyleBackColor = true;
            // 
            // chkAutoCrop
            // 
            chkAutoCrop.Location = new Point(10, 299);
            chkAutoCrop.Name = "chkAutoCrop";
            chkAutoCrop.Size = new Size(269, 30);
            chkAutoCrop.TabIndex = 13;
            chkAutoCrop.Text = "Автообрезка";
            chkAutoCrop.UseVisualStyleBackColor = true;
            // 
            // lblOperationTime
            // 
            lblOperationTime.Location = new Point(9, 382);
            lblOperationTime.Name = "lblOperationTime";
            lblOperationTime.Size = new Size(270, 36);
            lblOperationTime.TabIndex = 14;
            // 
            // lblImagePath
            // 
            lblImagePath.Location = new Point(1, 533);
            lblImagePath.Name = "lblImagePath";
            lblImagePath.Size = new Size(250, 103);
            lblImagePath.TabIndex = 16;
            lblImagePath.Text = "Путь к изображению:";
            // 
            // Form1
            // 
            ClientSize = new Size(1143, 689);
            Controls.Add(btnSaveAs);
            Controls.Add(lblOperationTime);
            Controls.Add(chkAutoCrop);
            Controls.Add(chkDiagonals);
            Controls.Add(chkGuidelines);
            Controls.Add(btnRotateMinus90);
            Controls.Add(btnRotate90);
            Controls.Add(txtAngle);
            Controls.Add(tbAngle);
            Controls.Add(btnReset);
            Controls.Add(btnLoadImage);
            Controls.Add(pictureBox1);
            Controls.Add(btnSaveImage);
            Controls.Add(btnAutoAlign);
            Controls.Add(btnToggleContour);
            Controls.Add(lblImagePath);
            Name = "Form1";
            Text = "Выравнивание (WFA)";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)tbAngle).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
