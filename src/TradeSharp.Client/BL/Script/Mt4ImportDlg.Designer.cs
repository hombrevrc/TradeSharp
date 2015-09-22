namespace TradeSharp.Client.BL.Script
{
    partial class Mt4ImportDlg
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblOrdersTotal = new System.Windows.Forms.Label();
            this.lblSelected = new System.Windows.Forms.Label();
            this.dpStart = new System.Windows.Forms.DateTimePicker();
            this.dpEnd = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnResetInterval = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "ордеров всего";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "ордеров выбрано";
            // 
            // lblOrdersTotal
            // 
            this.lblOrdersTotal.AutoSize = true;
            this.lblOrdersTotal.Location = new System.Drawing.Point(163, 9);
            this.lblOrdersTotal.Name = "lblOrdersTotal";
            this.lblOrdersTotal.Size = new System.Drawing.Size(10, 13);
            this.lblOrdersTotal.TabIndex = 2;
            this.lblOrdersTotal.Text = "-";
            // 
            // lblSelected
            // 
            this.lblSelected.AutoSize = true;
            this.lblSelected.Location = new System.Drawing.Point(163, 36);
            this.lblSelected.Name = "lblSelected";
            this.lblSelected.Size = new System.Drawing.Size(10, 13);
            this.lblSelected.TabIndex = 3;
            this.lblSelected.Text = "-";
            // 
            // dpStart
            // 
            this.dpStart.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dpStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dpStart.Location = new System.Drawing.Point(166, 66);
            this.dpStart.Name = "dpStart";
            this.dpStart.Size = new System.Drawing.Size(161, 20);
            this.dpStart.TabIndex = 4;
            this.dpStart.ValueChanged += new System.EventHandler(this.dpEnd_ValueChanged);
            // 
            // dpEnd
            // 
            this.dpEnd.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dpEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dpEnd.Location = new System.Drawing.Point(166, 92);
            this.dpEnd.Name = "dpEnd";
            this.dpEnd.Size = new System.Drawing.Size(161, 20);
            this.dpEnd.TabIndex = 5;
            this.dpEnd.ValueChanged += new System.EventHandler(this.dpEnd_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 72);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "начало интервала";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 98);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(93, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "конец интервала";
            // 
            // btnResetInterval
            // 
            this.btnResetInterval.Location = new System.Drawing.Point(12, 130);
            this.btnResetInterval.Name = "btnResetInterval";
            this.btnResetInterval.Size = new System.Drawing.Size(156, 23);
            this.btnResetInterval.TabIndex = 8;
            this.btnResetInterval.Text = "Очистить интервал";
            this.btnResetInterval.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(12, 188);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 9;
            this.btnOk.Text = "ОК";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(110, 188);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // Mt4ImportDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 223);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnResetInterval);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dpEnd);
            this.Controls.Add(this.dpStart);
            this.Controls.Add(this.lblSelected);
            this.Controls.Add(this.lblOrdersTotal);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Mt4ImportDlg";
            this.Text = "Импорт ордеров из MT4";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblOrdersTotal;
        private System.Windows.Forms.Label lblSelected;
        private System.Windows.Forms.DateTimePicker dpStart;
        private System.Windows.Forms.DateTimePicker dpEnd;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnResetInterval;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}