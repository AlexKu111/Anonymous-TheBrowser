namespace TheBrowser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            btnConnect = new Button();
            lblIpAddress = new Label();
            txtIpAddress = new TextBox();
            richTextBoxLog = new RichTextBox();
            lblStatus = new Label();
            SuspendLayout();
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(12, 12);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(120, 35);
            btnConnect.TabIndex = 0;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += BtnConnect_Click;
            // 
            // lblIpAddress
            // 
            lblIpAddress.AutoSize = true;
            lblIpAddress.Location = new Point(150, 20);
            lblIpAddress.Name = "lblIpAddress";
            lblIpAddress.Size = new Size(64, 15);
            lblIpAddress.TabIndex = 1;
            lblIpAddress.Text = "IP Address:";
            // 
            // txtIpAddress
            // 
            txtIpAddress.Location = new Point(218, 17);
            txtIpAddress.Name = "txtIpAddress";
            txtIpAddress.ReadOnly = true;
            txtIpAddress.Size = new Size(150, 23);
            txtIpAddress.TabIndex = 2;
            txtIpAddress.Text = "Waiting for connection...";
            // 
            // richTextBoxLog
            // 
            richTextBoxLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            richTextBoxLog.Location = new Point(12, 78);
            richTextBoxLog.Name = "richTextBoxLog";
            richTextBoxLog.ReadOnly = true;
            richTextBoxLog.Size = new Size(787, 394);
            richTextBoxLog.TabIndex = 3;
            richTextBoxLog.Text = "";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(12, 50);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(42, 15);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "Status:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(811, 484);
            Controls.Add(lblStatus);
            Controls.Add(richTextBoxLog);
            Controls.Add(txtIpAddress);
            Controls.Add(lblIpAddress);
            Controls.Add(btnConnect);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "TheBrowser";
            FormClosing += Form1_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnConnect;
        private Label lblIpAddress;
        private TextBox txtIpAddress;
        private RichTextBox richTextBoxLog;
        private Label lblStatus;
    }
}
