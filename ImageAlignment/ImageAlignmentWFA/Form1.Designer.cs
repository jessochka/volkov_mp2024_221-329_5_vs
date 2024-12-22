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
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tbAngle).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.Location = new Point(271, 10);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(862, 600);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // btnLoadImage
            // 
            btnLoadImage.Location = new Point(10, 10);
            btnLoadImage.Name = "btnLoadImage";
            btnLoadImage.Size = new Size(150, 44);
            btnLoadImage.TabIndex = 1;
            btnLoadImage.Text = "Загрузить";
            // 
            // btnReset
            // 
            btnReset.Location = new Point(10, 60);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(150, 30);
            btnReset.TabIndex = 2;
            btnReset.Text = "Сбросить";
            // 
            // tbAngle
            // 
            tbAngle.Location = new Point(10, 120);
            tbAngle.Maximum = 180;
            tbAngle.Minimum = -180;
            tbAngle.Name = "tbAngle";
            tbAngle.Size = new Size(160, 69);
            tbAngle.TabIndex = 3;
            // 
            // txtAngle
            // 
            txtAngle.Location = new Point(176, 120);
            txtAngle.Name = "txtAngle";
            txtAngle.Size = new Size(60, 31);
            txtAngle.TabIndex = 4;
            txtAngle.Text = "0";
            // 
            // btnRotate90
            // 
            btnRotate90.Location = new Point(10, 210);
            btnRotate90.Name = "btnRotate90";
            btnRotate90.Size = new Size(75, 30);
            btnRotate90.TabIndex = 5;
            btnRotate90.Text = "+90°";
            // 
            // btnRotateMinus90
            // 
            btnRotateMinus90.Location = new Point(95, 210);
            btnRotateMinus90.Name = "btnRotateMinus90";
            btnRotateMinus90.Size = new Size(75, 30);
            btnRotateMinus90.TabIndex = 6;
            btnRotateMinus90.Text = "-90°";
            // 
            // chkGuidelines
            // 
            chkGuidelines.Location = new Point(10, 260);
            chkGuidelines.Name = "chkGuidelines";
            chkGuidelines.Size = new Size(226, 35);
            chkGuidelines.TabIndex = 7;
            chkGuidelines.Text = "Направляющие линии";
            // 
            // chkDiagonals
            // 
            chkDiagonals.Location = new Point(10, 301);
            chkDiagonals.Name = "chkDiagonals";
            chkDiagonals.Size = new Size(270, 24);
            chkDiagonals.TabIndex = 8;
            chkDiagonals.Text = "Направляющие диагонали";
            // 
            // btnSaveImage
            // 
            btnSaveImage.Location = new Point(10, 340);
            btnSaveImage.Name = "btnSaveImage";
            btnSaveImage.Size = new Size(150, 30);
            btnSaveImage.TabIndex = 9;
            btnSaveImage.Text = "Сохранить";
            // 
            // Form1
            // 
            ClientSize = new Size(1143, 630);
            Controls.Add(pictureBox1);
            Controls.Add(btnLoadImage);
            Controls.Add(btnReset);
            Controls.Add(tbAngle);
            Controls.Add(txtAngle);
            Controls.Add(btnRotate90);
            Controls.Add(btnRotateMinus90);
            Controls.Add(chkGuidelines);
            Controls.Add(chkDiagonals);
            Controls.Add(btnSaveImage);
            Name = "Form1";
            Text = "Выравнивание (WFA)";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)tbAngle).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
