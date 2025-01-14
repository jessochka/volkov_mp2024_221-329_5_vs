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

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                imageProcessor?.Dispose(); // освобождение ресурсов
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnLoadImage = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.tbAngle = new System.Windows.Forms.TrackBar();
            this.txtAngle = new System.Windows.Forms.TextBox();
            this.btnRotate90 = new System.Windows.Forms.Button();
            this.btnRotateMinus90 = new System.Windows.Forms.Button();
            this.chkGuidelines = new System.Windows.Forms.CheckBox();
            this.chkDiagonals = new System.Windows.Forms.CheckBox();
            this.btnSaveImage = new System.Windows.Forms.Button();
            this.btnAutoAlign = new System.Windows.Forms.Button();
            this.btnToggleContour = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAngle)).BeginInit();
            this.SuspendLayout();
            //
            // pictureBox1
            //
            this.pictureBox1.Anchor =
                ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                                                      | System.Windows.Forms.AnchorStyles.Left)
                                                      | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(271, 10);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(862, 600);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            //
            // btnLoadImage
            //
            this.btnLoadImage.Location = new System.Drawing.Point(10, 10);
            this.btnLoadImage.Name = "btnLoadImage";
            this.btnLoadImage.Size = new System.Drawing.Size(150, 44);
            this.btnLoadImage.TabIndex = 1;
            this.btnLoadImage.Text = "Загрузить";
            this.btnLoadImage.UseVisualStyleBackColor = true;
            //
            // btnReset
            //
            this.btnReset.Location = new System.Drawing.Point(10, 60);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(150, 30);
            this.btnReset.TabIndex = 2;
            this.btnReset.Text = "Сбросить";
            this.btnReset.UseVisualStyleBackColor = true;
            //
            // tbAngle
            //
            this.tbAngle.Location = new System.Drawing.Point(10, 120);
            this.tbAngle.Maximum = 180;
            this.tbAngle.Minimum = -180;
            this.tbAngle.Name = "tbAngle";
            this.tbAngle.Size = new System.Drawing.Size(160, 45);
            this.tbAngle.TabIndex = 3;
            //
            // txtAngle
            //
            this.txtAngle.Location = new System.Drawing.Point(176, 120);
            this.txtAngle.Name = "txtAngle";
            this.txtAngle.Size = new System.Drawing.Size(60, 23);
            this.txtAngle.TabIndex = 4;
            this.txtAngle.Text = "0";
            //
            // btnRotate90
            //
            this.btnRotate90.Location = new System.Drawing.Point(10, 210);
            this.btnRotate90.Name = "btnRotate90";
            this.btnRotate90.Size = new System.Drawing.Size(75, 30);
            this.btnRotate90.TabIndex = 5;
            this.btnRotate90.Text = "+90°";
            this.btnRotate90.UseVisualStyleBackColor = true;
            //
            // btnRotateMinus90
            //
            this.btnRotateMinus90.Location = new System.Drawing.Point(95, 210);
            this.btnRotateMinus90.Name = "btnRotateMinus90";
            this.btnRotateMinus90.Size = new System.Drawing.Size(75, 30);
            this.btnRotateMinus90.TabIndex = 6;
            this.btnRotateMinus90.Text = "-90°";
            this.btnRotateMinus90.UseVisualStyleBackColor = true;
            //
            // chkGuidelines
            //
            this.chkGuidelines.Location = new System.Drawing.Point(10, 260);
            this.chkGuidelines.Name = "chkGuidelines";
            this.chkGuidelines.Size = new System.Drawing.Size(226, 35);
            this.chkGuidelines.TabIndex = 7;
            this.chkGuidelines.Text = "Направляющие линии";
            this.chkGuidelines.UseVisualStyleBackColor = true;
            //
            // chkDiagonals
            //
            this.chkDiagonals.Location = new System.Drawing.Point(10, 301);
            this.chkDiagonals.Name = "chkDiagonals";
            this.chkDiagonals.Size = new System.Drawing.Size(270, 24);
            this.chkDiagonals.TabIndex = 8;
            this.chkDiagonals.Text = "Направляющие диагонали";
            this.chkDiagonals.UseVisualStyleBackColor = true;
            //
            // btnSaveImage
            //
            this.btnSaveImage.Location = new System.Drawing.Point(10, 340);
            this.btnSaveImage.Name = "btnSaveImage";
            this.btnSaveImage.Size = new System.Drawing.Size(150, 30);
            this.btnSaveImage.TabIndex = 9;
            this.btnSaveImage.Text = "Сохранить";
            this.btnSaveImage.UseVisualStyleBackColor = true;
            //
            // btnAutoAlign
            //
            this.btnAutoAlign.Location = new System.Drawing.Point(10, 174);
            this.btnAutoAlign.Name = "btnAutoAlign";
            this.btnAutoAlign.Size = new System.Drawing.Size(160, 30);
            this.btnAutoAlign.TabIndex = 10;
            this.btnAutoAlign.Text = "Автовыравнивание";
            this.btnAutoAlign.UseVisualStyleBackColor = true;
            //
            // btnToggleContour
            //
            this.btnToggleContour.Location = new System.Drawing.Point(10, 420);
            this.btnToggleContour.Name = "btnToggleContour";
            this.btnToggleContour.Size = new System.Drawing.Size(160, 30);
            this.btnToggleContour.TabIndex = 12;
            this.btnToggleContour.Text = "Скрыть контур";
            this.btnToggleContour.UseVisualStyleBackColor = true;
            //
            // Form1
            //
            this.ClientSize = new System.Drawing.Size(1143, 630);
            this.Controls.Add(this.btnToggleContour);
            this.Controls.Add(this.btnAutoAlign);
            this.Controls.Add(this.btnSaveImage);
            this.Controls.Add(this.chkDiagonals);
            this.Controls.Add(this.chkGuidelines);
            this.Controls.Add(this.btnRotateMinus90);
            this.Controls.Add(this.btnRotate90);
            this.Controls.Add(this.txtAngle);
            this.Controls.Add(this.tbAngle);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnLoadImage);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Выравнивание (WFA)";

            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbAngle)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
