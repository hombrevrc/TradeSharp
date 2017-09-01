namespace TradeSharp.Client.BL.Script
{
    partial class FxRobotVolumeSetupDlg
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
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnRecalcVolumes = new System.Windows.Forms.Button();
            this.tbMaxOrders = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblCaption = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbStartVolume = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbRobotsCount = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbVolumeStep = new System.Windows.Forms.TextBox();
            this.tbMessage = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(17, 371);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(181, 54);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "Принять";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(239, 371);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(181, 54);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnRecalcVolumes
            // 
            this.btnRecalcVolumes.Location = new System.Drawing.Point(17, 299);
            this.btnRecalcVolumes.Name = "btnRecalcVolumes";
            this.btnRecalcVolumes.Size = new System.Drawing.Size(403, 54);
            this.btnRecalcVolumes.TabIndex = 2;
            this.btnRecalcVolumes.Text = "Рассчитать объемы ...";
            this.btnRecalcVolumes.UseVisualStyleBackColor = true;
            this.btnRecalcVolumes.Click += new System.EventHandler(this.btnRecalcVolumes_Click);
            // 
            // tbMaxOrders
            // 
            this.tbMaxOrders.Location = new System.Drawing.Point(12, 77);
            this.tbMaxOrders.Name = "tbMaxOrders";
            this.tbMaxOrders.Size = new System.Drawing.Size(93, 35);
            this.tbMaxOrders.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(111, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 29);
            this.label1.TabIndex = 4;
            this.label1.Text = "макс. сделок";
            // 
            // lblCaption
            // 
            this.lblCaption.AutoSize = true;
            this.lblCaption.Location = new System.Drawing.Point(12, 24);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(79, 29);
            this.lblCaption.TabIndex = 5;
            this.lblCaption.Text = "label2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(520, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(223, 29);
            this.label3.TabIndex = 7;
            this.label3.Text = "начальный объем";
            // 
            // tbStartVolume
            // 
            this.tbStartVolume.Location = new System.Drawing.Point(384, 77);
            this.tbStartVolume.Name = "tbStartVolume";
            this.tbStartVolume.Size = new System.Drawing.Size(130, 35);
            this.tbStartVolume.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(111, 128);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(108, 29);
            this.label4.TabIndex = 9;
            this.label4.Text = "роботов";
            // 
            // tbRobotsCount
            // 
            this.tbRobotsCount.Location = new System.Drawing.Point(12, 125);
            this.tbRobotsCount.Name = "tbRobotsCount";
            this.tbRobotsCount.Size = new System.Drawing.Size(93, 35);
            this.tbRobotsCount.TabIndex = 8;
            this.tbRobotsCount.Text = "4";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(520, 128);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(150, 29);
            this.label5.TabIndex = 11;
            this.label5.Text = "шаг объема";
            // 
            // tbVolumeStep
            // 
            this.tbVolumeStep.Location = new System.Drawing.Point(384, 125);
            this.tbVolumeStep.Name = "tbVolumeStep";
            this.tbVolumeStep.Size = new System.Drawing.Size(130, 35);
            this.tbVolumeStep.TabIndex = 10;
            // 
            // tbMessage
            // 
            this.tbMessage.Enabled = false;
            this.tbMessage.Location = new System.Drawing.Point(19, 189);
            this.tbMessage.Name = "tbMessage";
            this.tbMessage.Size = new System.Drawing.Size(877, 96);
            this.tbMessage.TabIndex = 12;
            this.tbMessage.Text = "";
            // 
            // FxRobotVolumeSetupDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(908, 440);
            this.Controls.Add(this.tbMessage);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbVolumeStep);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbRobotsCount);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbStartVolume);
            this.Controls.Add(this.lblCaption);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbMaxOrders);
            this.Controls.Add(this.btnRecalcVolumes);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Name = "FxRobotVolumeSetupDlg";
            this.Text = "Настройка робота FX";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnRecalcVolumes;
        private System.Windows.Forms.TextBox tbMaxOrders;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbStartVolume;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbRobotsCount;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbVolumeStep;
        private System.Windows.Forms.RichTextBox tbMessage;
    }
}