namespace QuoteManager
{
    partial class MainForm
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.pageTotal = new System.Windows.Forms.TabPage();
            this.splitContainerInfo = new System.Windows.Forms.SplitContainer();
            this.boxInfo = new System.Windows.Forms.RichTextBox();
            this.gridInfo = new FastGrid.FastGrid();
            this.contextMenuInfo = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuitemTrimHistory = new System.Windows.Forms.ToolStripMenuItem();
            this.menuitemMakeIndexes = new System.Windows.Forms.ToolStripMenuItem();
            this.panelInfoTop = new System.Windows.Forms.Panel();
            this.btnBrowseQuoteFolder = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbQuoteFolder = new System.Windows.Forms.TextBox();
            this.pageGaps = new System.Windows.Forms.TabPage();
            this.tabDaysOff = new System.Windows.Forms.TabPage();
            this.tbFilterDaysOffResult = new System.Windows.Forms.RichTextBox();
            this.panelDaysOffTop = new System.Windows.Forms.Panel();
            this.btnBuildSQLFilter = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tbDaysOffFilterStartTime = new System.Windows.Forms.TextBox();
            this.btnFilterDaysOff = new System.Windows.Forms.Button();
            this.btnBrowseDaysOffFilterQuoteFolder = new System.Windows.Forms.Button();
            this.tbDaysOffQuoteFolder = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbDaysOffDuration = new System.Windows.Forms.TextBox();
            this.tbStartHourOff = new System.Windows.Forms.TextBox();
            this.tbStartDayOff = new System.Windows.Forms.TextBox();
            this.dpMergeEndDate = new System.Windows.Forms.DateTimePicker();
            this.tbMergeQuotePath = new System.Windows.Forms.TextBox();
            this.btnMergeBrowseQuote = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnMergeBrowseCsv = new System.Windows.Forms.Button();
            this.tbMergeCsvPath = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnMergeQuotes = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.tbMergeTimeframe = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbMergeDigits = new System.Windows.Forms.TextBox();
            this.tabControl.SuspendLayout();
            this.pageTotal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerInfo)).BeginInit();
            this.splitContainerInfo.Panel1.SuspendLayout();
            this.splitContainerInfo.Panel2.SuspendLayout();
            this.splitContainerInfo.SuspendLayout();
            this.contextMenuInfo.SuspendLayout();
            this.panelInfoTop.SuspendLayout();
            this.pageGaps.SuspendLayout();
            this.tabDaysOff.SuspendLayout();
            this.panelDaysOffTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.pageTotal);
            this.tabControl.Controls.Add(this.pageGaps);
            this.tabControl.Controls.Add(this.tabDaysOff);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(598, 447);
            this.tabControl.TabIndex = 0;
            // 
            // pageTotal
            // 
            this.pageTotal.Controls.Add(this.splitContainerInfo);
            this.pageTotal.Controls.Add(this.panelInfoTop);
            this.pageTotal.Location = new System.Drawing.Point(4, 22);
            this.pageTotal.Name = "pageTotal";
            this.pageTotal.Padding = new System.Windows.Forms.Padding(3);
            this.pageTotal.Size = new System.Drawing.Size(590, 421);
            this.pageTotal.TabIndex = 0;
            this.pageTotal.Text = "Информация";
            this.pageTotal.UseVisualStyleBackColor = true;
            // 
            // splitContainerInfo
            // 
            this.splitContainerInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerInfo.Location = new System.Drawing.Point(3, 54);
            this.splitContainerInfo.Name = "splitContainerInfo";
            // 
            // splitContainerInfo.Panel1
            // 
            this.splitContainerInfo.Panel1.Controls.Add(this.boxInfo);
            // 
            // splitContainerInfo.Panel2
            // 
            this.splitContainerInfo.Panel2.Controls.Add(this.gridInfo);
            this.splitContainerInfo.Size = new System.Drawing.Size(584, 364);
            this.splitContainerInfo.SplitterDistance = 194;
            this.splitContainerInfo.TabIndex = 1;
            // 
            // boxInfo
            // 
            this.boxInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.boxInfo.Location = new System.Drawing.Point(0, 0);
            this.boxInfo.Name = "boxInfo";
            this.boxInfo.Size = new System.Drawing.Size(194, 364);
            this.boxInfo.TabIndex = 2;
            this.boxInfo.Text = "";
            // 
            // gridInfo
            // 
            this.gridInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gridInfo.CaptionHeight = 20;
            this.gridInfo.CellEditMode = FastGrid.FastGrid.CellEditModeTrigger.LeftClick;
            this.gridInfo.CellHeight = 18;
            this.gridInfo.CellPadding = 5;
            this.gridInfo.ColorAltCellBackground = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(247)))), ((int)(((byte)(247)))));
            this.gridInfo.ColorAnchorCellBackground = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.gridInfo.ColorCellBackground = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(247)))), ((int)(((byte)(247)))));
            this.gridInfo.ColorCellFont = System.Drawing.Color.Black;
            this.gridInfo.ColorCellOutlineLower = System.Drawing.Color.White;
            this.gridInfo.ColorCellOutlineUpper = System.Drawing.Color.DarkGray;
            this.gridInfo.ColorSelectedCellBackground = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(247)))), ((int)(((byte)(227)))));
            this.gridInfo.ColorSelectedCellFont = System.Drawing.Color.Black;
            this.gridInfo.ContextMenuStrip = this.contextMenuInfo;
            this.gridInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridInfo.FitWidth = false;
            this.gridInfo.FontAnchoredRow = null;
            this.gridInfo.FontCell = null;
            this.gridInfo.FontHeader = null;
            this.gridInfo.FontSelectedCell = null;
            this.gridInfo.Location = new System.Drawing.Point(0, 0);
            this.gridInfo.MinimumTableWidth = null;
            this.gridInfo.MultiSelectEnabled = false;
            this.gridInfo.Name = "gridInfo";
            this.gridInfo.SelectEnabled = true;
            this.gridInfo.Size = new System.Drawing.Size(386, 364);
            this.gridInfo.StickFirst = false;
            this.gridInfo.StickLast = false;
            this.gridInfo.TabIndex = 0;
            // 
            // contextMenuInfo
            // 
            this.contextMenuInfo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuitemTrimHistory,
            this.menuitemMakeIndexes});
            this.contextMenuInfo.Name = "contextMenuInfo";
            this.contextMenuInfo.Size = new System.Drawing.Size(191, 48);
            // 
            // menuitemTrimHistory
            // 
            this.menuitemTrimHistory.Name = "menuitemTrimHistory";
            this.menuitemTrimHistory.Size = new System.Drawing.Size(190, 22);
            this.menuitemTrimHistory.Text = "Вырезать историю...";
            this.menuitemTrimHistory.Click += new System.EventHandler(this.MenuitemTrimHistoryClick);
            // 
            // menuitemMakeIndexes
            // 
            this.menuitemMakeIndexes.Name = "menuitemMakeIndexes";
            this.menuitemMakeIndexes.Size = new System.Drawing.Size(190, 22);
            this.menuitemMakeIndexes.Text = "Валютные индексы...";
            this.menuitemMakeIndexes.Click += new System.EventHandler(this.MenuitemMakeIndexesClick);
            // 
            // panelInfoTop
            // 
            this.panelInfoTop.Controls.Add(this.btnBrowseQuoteFolder);
            this.panelInfoTop.Controls.Add(this.label1);
            this.panelInfoTop.Controls.Add(this.tbQuoteFolder);
            this.panelInfoTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelInfoTop.Location = new System.Drawing.Point(3, 3);
            this.panelInfoTop.Name = "panelInfoTop";
            this.panelInfoTop.Size = new System.Drawing.Size(584, 51);
            this.panelInfoTop.TabIndex = 0;
            // 
            // btnBrowseQuoteFolder
            // 
            this.btnBrowseQuoteFolder.Location = new System.Drawing.Point(413, 15);
            this.btnBrowseQuoteFolder.Name = "btnBrowseQuoteFolder";
            this.btnBrowseQuoteFolder.Size = new System.Drawing.Size(27, 23);
            this.btnBrowseQuoteFolder.TabIndex = 2;
            this.btnBrowseQuoteFolder.Text = "...";
            this.btnBrowseQuoteFolder.UseVisualStyleBackColor = true;
            this.btnBrowseQuoteFolder.Click += new System.EventHandler(this.BtnBrowseQuoteFolderClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "каталог котировок";
            // 
            // tbQuoteFolder
            // 
            this.tbQuoteFolder.Location = new System.Drawing.Point(5, 17);
            this.tbQuoteFolder.Name = "tbQuoteFolder";
            this.tbQuoteFolder.Size = new System.Drawing.Size(402, 20);
            this.tbQuoteFolder.TabIndex = 0;
            // 
            // pageGaps
            // 
            this.pageGaps.Controls.Add(this.label9);
            this.pageGaps.Controls.Add(this.tbMergeDigits);
            this.pageGaps.Controls.Add(this.label8);
            this.pageGaps.Controls.Add(this.tbMergeTimeframe);
            this.pageGaps.Controls.Add(this.btnMergeQuotes);
            this.pageGaps.Controls.Add(this.label7);
            this.pageGaps.Controls.Add(this.label6);
            this.pageGaps.Controls.Add(this.btnMergeBrowseCsv);
            this.pageGaps.Controls.Add(this.tbMergeCsvPath);
            this.pageGaps.Controls.Add(this.label5);
            this.pageGaps.Controls.Add(this.btnMergeBrowseQuote);
            this.pageGaps.Controls.Add(this.tbMergeQuotePath);
            this.pageGaps.Controls.Add(this.dpMergeEndDate);
            this.pageGaps.Location = new System.Drawing.Point(4, 22);
            this.pageGaps.Name = "pageGaps";
            this.pageGaps.Padding = new System.Windows.Forms.Padding(3);
            this.pageGaps.Size = new System.Drawing.Size(590, 421);
            this.pageGaps.TabIndex = 1;
            this.pageGaps.Text = "Гэпы";
            this.pageGaps.UseVisualStyleBackColor = true;
            // 
            // tabDaysOff
            // 
            this.tabDaysOff.Controls.Add(this.tbFilterDaysOffResult);
            this.tabDaysOff.Controls.Add(this.panelDaysOffTop);
            this.tabDaysOff.Location = new System.Drawing.Point(4, 22);
            this.tabDaysOff.Name = "tabDaysOff";
            this.tabDaysOff.Padding = new System.Windows.Forms.Padding(3);
            this.tabDaysOff.Size = new System.Drawing.Size(590, 421);
            this.tabDaysOff.TabIndex = 2;
            this.tabDaysOff.Text = "Фильтр выходных";
            this.tabDaysOff.UseVisualStyleBackColor = true;
            // 
            // tbFilterDaysOffResult
            // 
            this.tbFilterDaysOffResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbFilterDaysOffResult.Location = new System.Drawing.Point(3, 126);
            this.tbFilterDaysOffResult.Name = "tbFilterDaysOffResult";
            this.tbFilterDaysOffResult.Size = new System.Drawing.Size(584, 292);
            this.tbFilterDaysOffResult.TabIndex = 1;
            this.tbFilterDaysOffResult.Text = "";
            // 
            // panelDaysOffTop
            // 
            this.panelDaysOffTop.Controls.Add(this.btnBuildSQLFilter);
            this.panelDaysOffTop.Controls.Add(this.label4);
            this.panelDaysOffTop.Controls.Add(this.tbDaysOffFilterStartTime);
            this.panelDaysOffTop.Controls.Add(this.btnFilterDaysOff);
            this.panelDaysOffTop.Controls.Add(this.btnBrowseDaysOffFilterQuoteFolder);
            this.panelDaysOffTop.Controls.Add(this.tbDaysOffQuoteFolder);
            this.panelDaysOffTop.Controls.Add(this.label3);
            this.panelDaysOffTop.Controls.Add(this.label2);
            this.panelDaysOffTop.Controls.Add(this.tbDaysOffDuration);
            this.panelDaysOffTop.Controls.Add(this.tbStartHourOff);
            this.panelDaysOffTop.Controls.Add(this.tbStartDayOff);
            this.panelDaysOffTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelDaysOffTop.Location = new System.Drawing.Point(3, 3);
            this.panelDaysOffTop.Name = "panelDaysOffTop";
            this.panelDaysOffTop.Size = new System.Drawing.Size(584, 123);
            this.panelDaysOffTop.TabIndex = 0;
            // 
            // btnBuildSQLFilter
            // 
            this.btnBuildSQLFilter.Location = new System.Drawing.Point(153, 94);
            this.btnBuildSQLFilter.Name = "btnBuildSQLFilter";
            this.btnBuildSQLFilter.Size = new System.Drawing.Size(106, 23);
            this.btnBuildSQLFilter.TabIndex = 11;
            this.btnBuildSQLFilter.Text = "SQL";
            this.btnBuildSQLFilter.UseVisualStyleBackColor = true;
            this.btnBuildSQLFilter.Click += new System.EventHandler(this.btnBuildSQLFilter_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(209, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "начать с";
            // 
            // tbDaysOffFilterStartTime
            // 
            this.tbDaysOffFilterStartTime.Location = new System.Drawing.Point(212, 42);
            this.tbDaysOffFilterStartTime.Name = "tbDaysOffFilterStartTime";
            this.tbDaysOffFilterStartTime.Size = new System.Drawing.Size(114, 20);
            this.tbDaysOffFilterStartTime.TabIndex = 9;
            this.tbDaysOffFilterStartTime.Text = "2015-10-31 00:00";
            // 
            // btnFilterDaysOff
            // 
            this.btnFilterDaysOff.Location = new System.Drawing.Point(16, 94);
            this.btnFilterDaysOff.Name = "btnFilterDaysOff";
            this.btnFilterDaysOff.Size = new System.Drawing.Size(106, 23);
            this.btnFilterDaysOff.TabIndex = 8;
            this.btnFilterDaysOff.Text = "Фильтровать";
            this.btnFilterDaysOff.UseVisualStyleBackColor = true;
            this.btnFilterDaysOff.Click += new System.EventHandler(this.btnFilterDaysOff_Click);
            // 
            // btnBrowseDaysOffFilterQuoteFolder
            // 
            this.btnBrowseDaysOffFilterQuoteFolder.Location = new System.Drawing.Point(408, 66);
            this.btnBrowseDaysOffFilterQuoteFolder.Name = "btnBrowseDaysOffFilterQuoteFolder";
            this.btnBrowseDaysOffFilterQuoteFolder.Size = new System.Drawing.Size(27, 23);
            this.btnBrowseDaysOffFilterQuoteFolder.TabIndex = 7;
            this.btnBrowseDaysOffFilterQuoteFolder.Text = "...";
            this.btnBrowseDaysOffFilterQuoteFolder.UseVisualStyleBackColor = true;
            this.btnBrowseDaysOffFilterQuoteFolder.Click += new System.EventHandler(this.btnBrowseDaysOffFilterQuoteFolder_Click);
            // 
            // tbDaysOffQuoteFolder
            // 
            this.tbDaysOffQuoteFolder.Location = new System.Drawing.Point(16, 68);
            this.tbDaysOffQuoteFolder.Name = "tbDaysOffQuoteFolder";
            this.tbDaysOffQuoteFolder.Size = new System.Drawing.Size(386, 20);
            this.tbDaysOffQuoteFolder.TabIndex = 6;
            this.tbDaysOffQuoteFolder.Text = "C:\\Sources\\github\\TradeSharp\\src\\TradeSharp.Client\\bin\\Debug\\quotes";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(162, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(12, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "ч";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(162, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(12, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "ч";
            // 
            // tbDaysOffDuration
            // 
            this.tbDaysOffDuration.Location = new System.Drawing.Point(94, 42);
            this.tbDaysOffDuration.Name = "tbDaysOffDuration";
            this.tbDaysOffDuration.Size = new System.Drawing.Size(62, 20);
            this.tbDaysOffDuration.TabIndex = 3;
            // 
            // tbStartHourOff
            // 
            this.tbStartHourOff.Location = new System.Drawing.Point(94, 16);
            this.tbStartHourOff.Name = "tbStartHourOff";
            this.tbStartHourOff.Size = new System.Drawing.Size(62, 20);
            this.tbStartHourOff.TabIndex = 2;
            // 
            // tbStartDayOff
            // 
            this.tbStartDayOff.Location = new System.Drawing.Point(16, 16);
            this.tbStartDayOff.Name = "tbStartDayOff";
            this.tbStartDayOff.Size = new System.Drawing.Size(62, 20);
            this.tbStartDayOff.TabIndex = 0;
            // 
            // dpMergeEndDate
            // 
            this.dpMergeEndDate.CustomFormat = "yyyy-MM-dd";
            this.dpMergeEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dpMergeEndDate.Location = new System.Drawing.Point(8, 116);
            this.dpMergeEndDate.Name = "dpMergeEndDate";
            this.dpMergeEndDate.Size = new System.Drawing.Size(124, 20);
            this.dpMergeEndDate.TabIndex = 0;
            // 
            // tbMergeQuotePath
            // 
            this.tbMergeQuotePath.Location = new System.Drawing.Point(8, 16);
            this.tbMergeQuotePath.Name = "tbMergeQuotePath";
            this.tbMergeQuotePath.Size = new System.Drawing.Size(391, 20);
            this.tbMergeQuotePath.TabIndex = 1;
            // 
            // btnMergeBrowseQuote
            // 
            this.btnMergeBrowseQuote.Location = new System.Drawing.Point(405, 15);
            this.btnMergeBrowseQuote.Name = "btnMergeBrowseQuote";
            this.btnMergeBrowseQuote.Size = new System.Drawing.Size(26, 21);
            this.btnMergeBrowseQuote.TabIndex = 2;
            this.btnMergeBrowseQuote.Text = "...";
            this.btnMergeBrowseQuote.UseVisualStyleBackColor = true;
            this.btnMergeBrowseQuote.Click += new System.EventHandler(this.btnMergeBrowseQuote_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(156, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "заменить котировки в файле";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 53);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(124, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "котировками из файла";
            // 
            // btnMergeBrowseCsv
            // 
            this.btnMergeBrowseCsv.Location = new System.Drawing.Point(405, 65);
            this.btnMergeBrowseCsv.Name = "btnMergeBrowseCsv";
            this.btnMergeBrowseCsv.Size = new System.Drawing.Size(26, 21);
            this.btnMergeBrowseCsv.TabIndex = 5;
            this.btnMergeBrowseCsv.Text = "...";
            this.btnMergeBrowseCsv.UseVisualStyleBackColor = true;
            this.btnMergeBrowseCsv.Click += new System.EventHandler(this.btnMergeBrowseCsv_Click);
            // 
            // tbMergeCsvPath
            // 
            this.tbMergeCsvPath.Location = new System.Drawing.Point(8, 66);
            this.tbMergeCsvPath.Name = "tbMergeCsvPath";
            this.tbMergeCsvPath.Size = new System.Drawing.Size(391, 20);
            this.tbMergeCsvPath.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 101);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "до даты";
            // 
            // btnMergeQuotes
            // 
            this.btnMergeQuotes.Location = new System.Drawing.Point(6, 232);
            this.btnMergeQuotes.Name = "btnMergeQuotes";
            this.btnMergeQuotes.Size = new System.Drawing.Size(75, 23);
            this.btnMergeQuotes.TabIndex = 8;
            this.btnMergeQuotes.Text = "Склеить";
            this.btnMergeQuotes.UseVisualStyleBackColor = true;
            this.btnMergeQuotes.Click += new System.EventHandler(this.btnMergeQuotes_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 156);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(66, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "таймфрейм";
            // 
            // tbMergeTimeframe
            // 
            this.tbMergeTimeframe.Location = new System.Drawing.Point(8, 172);
            this.tbMergeTimeframe.Name = "tbMergeTimeframe";
            this.tbMergeTimeframe.Size = new System.Drawing.Size(59, 20);
            this.tbMergeTimeframe.TabIndex = 9;
            this.tbMergeTimeframe.Text = "60";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(102, 156);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(43, 13);
            this.label9.TabIndex = 12;
            this.label9.Text = "знаков";
            // 
            // tbMergeDigits
            // 
            this.tbMergeDigits.Location = new System.Drawing.Point(105, 172);
            this.tbMergeDigits.Name = "tbMergeDigits";
            this.tbMergeDigits.Size = new System.Drawing.Size(59, 20);
            this.tbMergeDigits.TabIndex = 11;
            this.tbMergeDigits.Text = "4";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 447);
            this.Controls.Add(this.tabControl);
            this.Name = "MainForm";
            this.Text = "Котировки";
            this.Load += new System.EventHandler(this.MainFormLoad);
            this.tabControl.ResumeLayout(false);
            this.pageTotal.ResumeLayout(false);
            this.splitContainerInfo.Panel1.ResumeLayout(false);
            this.splitContainerInfo.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerInfo)).EndInit();
            this.splitContainerInfo.ResumeLayout(false);
            this.contextMenuInfo.ResumeLayout(false);
            this.panelInfoTop.ResumeLayout(false);
            this.panelInfoTop.PerformLayout();
            this.pageGaps.ResumeLayout(false);
            this.pageGaps.PerformLayout();
            this.tabDaysOff.ResumeLayout(false);
            this.panelDaysOffTop.ResumeLayout(false);
            this.panelDaysOffTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage pageGaps;
        private System.Windows.Forms.TabPage pageTotal;
        private System.Windows.Forms.Panel panelInfoTop;
        private System.Windows.Forms.Button btnBrowseQuoteFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbQuoteFolder;
        private System.Windows.Forms.SplitContainer splitContainerInfo;
        private System.Windows.Forms.RichTextBox boxInfo;
        private FastGrid.FastGrid gridInfo;
        private System.Windows.Forms.ContextMenuStrip contextMenuInfo;
        private System.Windows.Forms.ToolStripMenuItem menuitemTrimHistory;
        private System.Windows.Forms.ToolStripMenuItem menuitemMakeIndexes;
        private System.Windows.Forms.TabPage tabDaysOff;
        private System.Windows.Forms.RichTextBox tbFilterDaysOffResult;
        private System.Windows.Forms.Panel panelDaysOffTop;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbDaysOffFilterStartTime;
        private System.Windows.Forms.Button btnFilterDaysOff;
        private System.Windows.Forms.Button btnBrowseDaysOffFilterQuoteFolder;
        private System.Windows.Forms.TextBox tbDaysOffQuoteFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbDaysOffDuration;
        private System.Windows.Forms.TextBox tbStartHourOff;
        private System.Windows.Forms.TextBox tbStartDayOff;
        private System.Windows.Forms.Button btnBuildSQLFilter;
        private System.Windows.Forms.Button btnMergeQuotes;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnMergeBrowseCsv;
        private System.Windows.Forms.TextBox tbMergeCsvPath;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnMergeBrowseQuote;
        private System.Windows.Forms.TextBox tbMergeQuotePath;
        private System.Windows.Forms.DateTimePicker dpMergeEndDate;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbMergeDigits;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbMergeTimeframe;
    }
}

