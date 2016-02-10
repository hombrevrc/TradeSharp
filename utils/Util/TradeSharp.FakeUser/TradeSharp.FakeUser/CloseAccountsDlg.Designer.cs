namespace TradeSharp.FakeUser
{
    partial class CloseAccountsDlg
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
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblCompleted = new System.Windows.Forms.Label();
            this.btnCloseAccounts = new System.Windows.Forms.Button();
            this.tbAccounts = new System.Windows.Forms.RichTextBox();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.lblCompleted);
            this.panelTop.Controls.Add(this.btnCloseAccounts);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(419, 48);
            this.panelTop.TabIndex = 0;
            // 
            // lblCompleted
            // 
            this.lblCompleted.AutoSize = true;
            this.lblCompleted.Location = new System.Drawing.Point(189, 17);
            this.lblCompleted.Name = "lblCompleted";
            this.lblCompleted.Size = new System.Drawing.Size(10, 13);
            this.lblCompleted.TabIndex = 1;
            this.lblCompleted.Text = "-";
            // 
            // btnCloseAccounts
            // 
            this.btnCloseAccounts.Location = new System.Drawing.Point(12, 12);
            this.btnCloseAccounts.Name = "btnCloseAccounts";
            this.btnCloseAccounts.Size = new System.Drawing.Size(156, 23);
            this.btnCloseAccounts.TabIndex = 0;
            this.btnCloseAccounts.Text = "Закрыть счета";
            this.btnCloseAccounts.UseVisualStyleBackColor = true;
            this.btnCloseAccounts.Click += new System.EventHandler(this.btnCloseAccounts_Click);
            // 
            // tbAccounts
            // 
            this.tbAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbAccounts.Location = new System.Drawing.Point(0, 48);
            this.tbAccounts.Name = "tbAccounts";
            this.tbAccounts.Size = new System.Drawing.Size(419, 238);
            this.tbAccounts.TabIndex = 1;
            this.tbAccounts.Text = "";
            // 
            // CloseAccountsDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 286);
            this.Controls.Add(this.tbAccounts);
            this.Controls.Add(this.panelTop);
            this.Name = "CloseAccountsDlg";
            this.Text = "Закрыть счета";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CloseAccountsDlg_FormClosing);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnCloseAccounts;
        private System.Windows.Forms.RichTextBox tbAccounts;
        private System.Windows.Forms.Label lblCompleted;
    }
}