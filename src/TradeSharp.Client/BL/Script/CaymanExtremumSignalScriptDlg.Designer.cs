namespace TradeSharp.Client.BL.Script
{
    partial class CaymanExtremumSignalScriptDlg
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbMarginLevels = new System.Windows.Forms.TextBox();
            this.tbCandlesCount = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbRemoveSigns = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(12, 136);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "ОК";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(129, 136);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Граничный уровень Каймана";
            // 
            // tbMarginLevels
            // 
            this.tbMarginLevels.Location = new System.Drawing.Point(12, 25);
            this.tbMarginLevels.Name = "tbMarginLevels";
            this.tbMarginLevels.Size = new System.Drawing.Size(100, 20);
            this.tbMarginLevels.TabIndex = 3;
            this.tbMarginLevels.Text = "35 - 65";
            // 
            // tbCandlesCount
            // 
            this.tbCandlesCount.Location = new System.Drawing.Point(12, 71);
            this.tbCandlesCount.Name = "tbCandlesCount";
            this.tbCandlesCount.Size = new System.Drawing.Size(100, 20);
            this.tbCandlesCount.TabIndex = 5;
            this.tbCandlesCount.Text = "2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(209, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Вход на N-свече за граничным уровнем";
            // 
            // cbRemoveSigns
            // 
            this.cbRemoveSigns.AutoSize = true;
            this.cbRemoveSigns.Checked = true;
            this.cbRemoveSigns.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRemoveSigns.Location = new System.Drawing.Point(12, 97);
            this.cbRemoveSigns.Name = "cbRemoveSigns";
            this.cbRemoveSigns.Size = new System.Drawing.Size(111, 17);
            this.cbRemoveSigns.TabIndex = 8;
            this.cbRemoveSigns.Text = "удалять отметки";
            this.cbRemoveSigns.UseVisualStyleBackColor = true;
            // 
            // CaymanExtremumSignalScriptDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 169);
            this.Controls.Add(this.cbRemoveSigns);
            this.Controls.Add(this.tbCandlesCount);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbMarginLevels);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Name = "CaymanExtremumSignalScriptDlg";
            this.Text = "Торговля от экстремумов Каймана";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbMarginLevels;
        private System.Windows.Forms.TextBox tbCandlesCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbRemoveSigns;
    }
}