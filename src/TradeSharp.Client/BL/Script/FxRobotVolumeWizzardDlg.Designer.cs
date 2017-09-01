namespace TradeSharp.Client.BL.Script
{
    partial class FxRobotVolumeWizzardDlg
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
            this.label5 = new System.Windows.Forms.Label();
            this.tbStartLever = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbRobotsCount = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbMaxLeverage = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbMaxOrders = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblComment = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(520, 63);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(160, 29);
            this.label5.TabIndex = 19;
            this.label5.Text = "старт. плечо";
            // 
            // tbStartLever
            // 
            this.tbStartLever.Location = new System.Drawing.Point(384, 60);
            this.tbStartLever.Name = "tbStartLever";
            this.tbStartLever.Size = new System.Drawing.Size(130, 35);
            this.tbStartLever.TabIndex = 18;
            this.tbStartLever.Text = "1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(111, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(249, 29);
            this.label4.TabIndex = 17;
            this.label4.Text = "количество роботов";
            // 
            // tbRobotsCount
            // 
            this.tbRobotsCount.Enabled = false;
            this.tbRobotsCount.Location = new System.Drawing.Point(12, 60);
            this.tbRobotsCount.Name = "tbRobotsCount";
            this.tbRobotsCount.Size = new System.Drawing.Size(93, 35);
            this.tbRobotsCount.TabIndex = 16;
            this.tbRobotsCount.Text = "4";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(520, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(152, 29);
            this.label3.TabIndex = 15;
            this.label3.Text = "макс. плечо";
            // 
            // tbMaxLeverage
            // 
            this.tbMaxLeverage.Location = new System.Drawing.Point(384, 12);
            this.tbMaxLeverage.Name = "tbMaxLeverage";
            this.tbMaxLeverage.Size = new System.Drawing.Size(130, 35);
            this.tbMaxLeverage.TabIndex = 14;
            this.tbMaxLeverage.Text = "10";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(111, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 29);
            this.label1.TabIndex = 13;
            this.label1.Text = "макс. сделок";
            // 
            // tbMaxOrders
            // 
            this.tbMaxOrders.Enabled = false;
            this.tbMaxOrders.Location = new System.Drawing.Point(12, 12);
            this.tbMaxOrders.Name = "tbMaxOrders";
            this.tbMaxOrders.Size = new System.Drawing.Size(93, 35);
            this.tbMaxOrders.TabIndex = 12;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(12, 249);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(197, 51);
            this.btnOk.TabIndex = 20;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(255, 249);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(197, 51);
            this.btnCancel.TabIndex = 21;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lblComment
            // 
            this.lblComment.AutoSize = true;
            this.lblComment.Location = new System.Drawing.Point(12, 137);
            this.lblComment.Name = "lblComment";
            this.lblComment.Size = new System.Drawing.Size(21, 29);
            this.lblComment.TabIndex = 22;
            this.lblComment.Text = "-";
            // 
            // FxRobotVolumeWizzardDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(937, 312);
            this.Controls.Add(this.lblComment);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbStartLever);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbRobotsCount);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbMaxLeverage);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbMaxOrders);
            this.Name = "FxRobotVolumeWizzardDlg";
            this.Text = "Авто-расчет объема";
            this.Load += new System.EventHandler(this.FxRobotVolumeWizzardDlg_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbStartLever;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbRobotsCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbMaxLeverage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbMaxOrders;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblComment;
    }
}