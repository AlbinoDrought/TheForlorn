namespace TheForlorn
{
    partial class WatchForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnScreenshot = new System.Windows.Forms.Button();
            this.tmrScreenshot = new System.Windows.Forms.Timer(this.components);
            this.pbScreenshot = new System.Windows.Forms.PictureBox();
            this.tbFrequency = new System.Windows.Forms.TrackBar();
            this.nudQuality = new System.Windows.Forms.NumericUpDown();
            this.cbFormat = new System.Windows.Forms.ComboBox();
            this.chkInput = new System.Windows.Forms.CheckBox();
            this.tmrScreenshotTimeout = new System.Windows.Forms.Timer(this.components);
            this.txtKeyboard = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbScreenshot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbFrequency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudQuality)).BeginInit();
            this.SuspendLayout();
            // 
            // btnScreenshot
            // 
            this.btnScreenshot.Location = new System.Drawing.Point(12, 12);
            this.btnScreenshot.Name = "btnScreenshot";
            this.btnScreenshot.Size = new System.Drawing.Size(75, 23);
            this.btnScreenshot.TabIndex = 0;
            this.btnScreenshot.Text = "Toggle Screenshots";
            this.btnScreenshot.UseVisualStyleBackColor = true;
            this.btnScreenshot.Click += new System.EventHandler(this.btnScreenshot_Click);
            // 
            // tmrScreenshot
            // 
            this.tmrScreenshot.Interval = 1000;
            this.tmrScreenshot.Tick += new System.EventHandler(this.tmrScreenshot_Tick);
            // 
            // pbScreenshot
            // 
            this.pbScreenshot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbScreenshot.Location = new System.Drawing.Point(12, 41);
            this.pbScreenshot.Name = "pbScreenshot";
            this.pbScreenshot.Size = new System.Drawing.Size(591, 474);
            this.pbScreenshot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbScreenshot.TabIndex = 1;
            this.pbScreenshot.TabStop = false;
            this.pbScreenshot.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbScreenshot_MouseClick);
            this.pbScreenshot.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbScreenshot_MouseMove);
            // 
            // tbFrequency
            // 
            this.tbFrequency.Location = new System.Drawing.Point(93, 12);
            this.tbFrequency.Maximum = 5000;
            this.tbFrequency.Minimum = 10;
            this.tbFrequency.Name = "tbFrequency";
            this.tbFrequency.Size = new System.Drawing.Size(104, 45);
            this.tbFrequency.SmallChange = 125;
            this.tbFrequency.TabIndex = 2;
            this.tbFrequency.TickFrequency = 500;
            this.tbFrequency.Value = 1000;
            this.tbFrequency.Scroll += new System.EventHandler(this.tbFrequency_Scroll);
            // 
            // nudQuality
            // 
            this.nudQuality.Increment = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.nudQuality.Location = new System.Drawing.Point(203, 12);
            this.nudQuality.Name = "nudQuality";
            this.nudQuality.Size = new System.Drawing.Size(46, 20);
            this.nudQuality.TabIndex = 3;
            this.nudQuality.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // cbFormat
            // 
            this.cbFormat.FormattingEnabled = true;
            this.cbFormat.Items.AddRange(new object[] {
            "jpg",
            "gif",
            "png"});
            this.cbFormat.Location = new System.Drawing.Point(255, 11);
            this.cbFormat.Name = "cbFormat";
            this.cbFormat.Size = new System.Drawing.Size(47, 21);
            this.cbFormat.TabIndex = 4;
            this.cbFormat.Text = "jpg";
            // 
            // chkInput
            // 
            this.chkInput.AutoSize = true;
            this.chkInput.Location = new System.Drawing.Point(308, 13);
            this.chkInput.Name = "chkInput";
            this.chkInput.Size = new System.Drawing.Size(92, 17);
            this.chkInput.TabIndex = 5;
            this.chkInput.Text = "Input Enabled";
            this.chkInput.UseVisualStyleBackColor = true;
            // 
            // tmrScreenshotTimeout
            // 
            this.tmrScreenshotTimeout.Tick += new System.EventHandler(this.tmrScreenshotTimeout_Tick);
            // 
            // txtKeyboard
            // 
            this.txtKeyboard.Location = new System.Drawing.Point(406, 11);
            this.txtKeyboard.Name = "txtKeyboard";
            this.txtKeyboard.Size = new System.Drawing.Size(100, 20);
            this.txtKeyboard.TabIndex = 6;
            this.txtKeyboard.TextChanged += new System.EventHandler(this.txtKeyboard_TextChanged);
            // 
            // WatchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 527);
            this.Controls.Add(this.txtKeyboard);
            this.Controls.Add(this.chkInput);
            this.Controls.Add(this.cbFormat);
            this.Controls.Add(this.nudQuality);
            this.Controls.Add(this.tbFrequency);
            this.Controls.Add(this.pbScreenshot);
            this.Controls.Add(this.btnScreenshot);
            this.Name = "WatchForm";
            this.Text = "WatchForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WatchForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.WatchForm_FormClosed);
            this.Load += new System.EventHandler(this.WatchForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbScreenshot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbFrequency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudQuality)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnScreenshot;
        private System.Windows.Forms.Timer tmrScreenshot;
        private System.Windows.Forms.PictureBox pbScreenshot;
        private System.Windows.Forms.TrackBar tbFrequency;
        private System.Windows.Forms.NumericUpDown nudQuality;
        private System.Windows.Forms.ComboBox cbFormat;
        private System.Windows.Forms.CheckBox chkInput;
        private System.Windows.Forms.Timer tmrScreenshotTimeout;
        private System.Windows.Forms.TextBox txtKeyboard;
    }
}