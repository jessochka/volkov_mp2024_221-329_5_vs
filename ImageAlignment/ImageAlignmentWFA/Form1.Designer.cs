namespace ImageAlignmentWFA
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            chkGuidelines = new CheckBox();
            chkDiagonals = new CheckBox();
            chkAutoCrop = new CheckBox();
            btnRotate90 = new Button();
            btnRotateMinus90 = new Button();
            btnAutoAlign = new Button();
            btnLoadImage = new Button();
            pictureBox1 = new PictureBox();
            tbAngle = new TrackBar();
            txtAngle = new TextBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tbAngle).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(100, 25);
            label1.TabIndex = 0;
            label1.Text = "Настройки";
            // 
            // chkGuidelines
            // 
            chkGuidelines.AutoSize = true;
            chkGuidelines.Location = new Point(12, 46);
            chkGuidelines.Name = "chkGuidelines";
            chkGuidelines.Size = new Size(221, 29);
            chkGuidelines.TabIndex = 1;
            chkGuidelines.Text = "Направляющие линии";
            chkGuidelines.UseVisualStyleBackColor = true;
            // 
            // chkDiagonals
            // 
            chkDiagonals.AutoSize = true;
            chkDiagonals.Location = new Point(12, 81);
            chkDiagonals.Name = "chkDiagonals";
            chkDiagonals.Size = new Size(257, 29);
            chkDiagonals.TabIndex = 2;
            chkDiagonals.Text = "Направляющие диагонали";
            chkDiagonals.UseVisualStyleBackColor = true;
            // 
            // chkAutoCrop
            // 
            chkAutoCrop.AutoSize = true;
            chkAutoCrop.Location = new Point(12, 116);
            chkAutoCrop.Name = "chkAutoCrop";
            chkAutoCrop.Size = new Size(199, 29);
            chkAutoCrop.TabIndex = 3;
            chkAutoCrop.Text = "Автовыравнивание";
            chkAutoCrop.UseVisualStyleBackColor = true;
            // 
            // btnRotate90
            // 
            btnRotate90.Location = new Point(35, 166);
            btnRotate90.Name = "btnRotate90";
            btnRotate90.Size = new Size(176, 34);
            btnRotate90.TabIndex = 4;
            btnRotate90.Text = "+90";
            btnRotate90.UseVisualStyleBackColor = true;
            // 
            // btnRotateMinus90
            // 
            btnRotateMinus90.Location = new Point(35, 206);
            btnRotateMinus90.Name = "btnRotateMinus90";
            btnRotateMinus90.Size = new Size(176, 34);
            btnRotateMinus90.TabIndex = 5;
            btnRotateMinus90.Text = "-90";
            btnRotateMinus90.UseVisualStyleBackColor = true;
            // 
            // btnAutoAlign
            // 
            btnAutoAlign.Location = new Point(35, 246);
            btnAutoAlign.Name = "btnAutoAlign";
            btnAutoAlign.Size = new Size(176, 34);
            btnAutoAlign.TabIndex = 6;
            btnAutoAlign.Text = "авто";
            btnAutoAlign.UseVisualStyleBackColor = true;
            // 
            // btnLoadImage
            // 
            btnLoadImage.Location = new Point(35, 328);
            btnLoadImage.Name = "btnLoadImage";
            btnLoadImage.Size = new Size(176, 34);
            btnLoadImage.TabIndex = 7;
            btnLoadImage.Text = "Загрузить";
            btnLoadImage.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(287, 9);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(854, 363);
            pictureBox1.TabIndex = 8;
            pictureBox1.TabStop = false;
            // 
            // tbAngle
            // 
            tbAngle.Location = new Point(12, 406);
            tbAngle.Name = "tbAngle";
            tbAngle.Size = new Size(1016, 69);
            tbAngle.TabIndex = 9;
            // 
            // txtAngle
            // 
            txtAngle.Location = new Point(1044, 419);
            txtAngle.Name = "txtAngle";
            txtAngle.Size = new Size(83, 31);
            txtAngle.TabIndex = 10;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1153, 487);
            Controls.Add(txtAngle);
            Controls.Add(tbAngle);
            Controls.Add(pictureBox1);
            Controls.Add(btnLoadImage);
            Controls.Add(btnAutoAlign);
            Controls.Add(btnRotateMinus90);
            Controls.Add(btnRotate90);
            Controls.Add(chkAutoCrop);
            Controls.Add(chkDiagonals);
            Controls.Add(chkGuidelines);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)tbAngle).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private CheckBox chkGuidelines;
        private CheckBox chkDiagonals;
        private CheckBox chkAutoCrop;
        private Button btnRotate90;
        private Button btnRotateMinus90;
        private Button btnAutoAlign;
        private Button btnLoadImage;
        private PictureBox pictureBox1;
        private TrackBar tbAngle;
        private TextBox txtAngle;
    }
}
