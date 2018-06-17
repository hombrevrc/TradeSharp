namespace TradeSharp.Client.Forms
{
    partial class AccountTradeResultsForm
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AccountTradeResultsForm));
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.panelProgress = new System.Windows.Forms.Panel();
            this.lbProgress = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageSetup = new System.Windows.Forms.TabPage();
            this.btnMakeReportHTML = new System.Windows.Forms.Button();
            this.panelOptions = new System.Windows.Forms.Panel();
            this.cbShowBalanceCurve = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbTimeframe = new System.Windows.Forms.TextBox();
            this.cbStartFrom = new System.Windows.Forms.CheckBox();
            this.cbFilterByMagic = new System.Windows.Forms.CheckBox();
            this.tbMagic = new System.Windows.Forms.TextBox();
            this.cbDefaultUploadQuotes = new System.Windows.Forms.CheckBox();
            this.dpStart = new System.Windows.Forms.DateTimePicker();
            this.btnShowOptions = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.tabPageStat = new System.Windows.Forms.TabPage();
            this.dgStat = new FastGrid.FastGrid();
            this.tabPageEquityCurve = new System.Windows.Forms.TabPage();
            this.chartProfit = new FastMultiChart.FastMultiChart();
            this.tabPageProfit1000 = new System.Windows.Forms.TabPage();
            this.chartProfit1000 = new FastMultiChart.FastMultiChart();
            this.tabPageDrowDawn = new System.Windows.Forms.TabPage();
            this.chartDrawDown = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabPageEquityDrawDown = new System.Windows.Forms.TabPage();
            this.chartEquityDrawDown = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panelProgress.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageSetup.SuspendLayout();
            this.panelOptions.SuspendLayout();
            this.tabPageStat.SuspendLayout();
            this.tabPageEquityCurve.SuspendLayout();
            this.tabPageProfit1000.SuspendLayout();
            this.tabPageDrowDawn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartDrawDown)).BeginInit();
            this.tabPageEquityDrawDown.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartEquityDrawDown)).BeginInit();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar.Location = new System.Drawing.Point(0, 0);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(581, 23);
            this.progressBar.TabIndex = 37;
            // 
            // panelProgress
            // 
            this.panelProgress.Controls.Add(this.lbProgress);
            this.panelProgress.Controls.Add(this.progressBar);
            this.panelProgress.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelProgress.Location = new System.Drawing.Point(0, 401);
            this.panelProgress.Name = "panelProgress";
            this.panelProgress.Size = new System.Drawing.Size(581, 23);
            this.panelProgress.TabIndex = 38;
            // 
            // lbProgress
            // 
            this.lbProgress.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lbProgress.AutoSize = true;
            this.lbProgress.Location = new System.Drawing.Point(3, -9);
            this.lbProgress.Name = "lbProgress";
            this.lbProgress.Size = new System.Drawing.Size(0, 13);
            this.lbProgress.TabIndex = 38;
            this.lbProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tabControl);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(581, 401);
            this.panel2.TabIndex = 39;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageSetup);
            this.tabControl.Controls.Add(this.tabPageStat);
            this.tabControl.Controls.Add(this.tabPageEquityCurve);
            this.tabControl.Controls.Add(this.tabPageProfit1000);
            this.tabControl.Controls.Add(this.tabPageDrowDawn);
            this.tabControl.Controls.Add(this.tabPageEquityDrawDown);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(581, 401);
            this.tabControl.TabIndex = 0;
            // 
            // tabPageSetup
            // 
            this.tabPageSetup.Controls.Add(this.btnMakeReportHTML);
            this.tabPageSetup.Controls.Add(this.panelOptions);
            this.tabPageSetup.Controls.Add(this.btnShowOptions);
            this.tabPageSetup.Controls.Add(this.btnUpdate);
            this.tabPageSetup.Location = new System.Drawing.Point(4, 22);
            this.tabPageSetup.Name = "tabPageSetup";
            this.tabPageSetup.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSetup.Size = new System.Drawing.Size(573, 375);
            this.tabPageSetup.TabIndex = 0;
            this.tabPageSetup.Text = "`````";
            this.tabPageSetup.UseVisualStyleBackColor = true;
            this.tabPageSetup.Move += new System.EventHandler(this.TabPageSetupMove);
            // 
            // btnMakeReportHTML
            // 
            this.btnMakeReportHTML.Enabled = false;
            this.btnMakeReportHTML.Location = new System.Drawing.Point(150, 41);
            this.btnMakeReportHTML.Name = "btnMakeReportHTML";
            this.btnMakeReportHTML.Size = new System.Drawing.Size(107, 23);
            this.btnMakeReportHTML.TabIndex = 12;
            this.btnMakeReportHTML.Tag = "TitleHTMLReport";
            this.btnMakeReportHTML.Text = "отчет в HTML";
            this.btnMakeReportHTML.UseVisualStyleBackColor = true;
            this.btnMakeReportHTML.Click += new System.EventHandler(this.BtnMakeReportHtmlClick);
            // 
            // panelOptions
            // 
            this.panelOptions.Controls.Add(this.cbShowBalanceCurve);
            this.panelOptions.Controls.Add(this.label1);
            this.panelOptions.Controls.Add(this.tbTimeframe);
            this.panelOptions.Controls.Add(this.cbStartFrom);
            this.panelOptions.Controls.Add(this.cbFilterByMagic);
            this.panelOptions.Controls.Add(this.tbMagic);
            this.panelOptions.Controls.Add(this.cbDefaultUploadQuotes);
            this.panelOptions.Controls.Add(this.dpStart);
            this.panelOptions.Location = new System.Drawing.Point(8, 70);
            this.panelOptions.Name = "panelOptions";
            this.panelOptions.Size = new System.Drawing.Size(249, 132);
            this.panelOptions.TabIndex = 11;
            this.panelOptions.Visible = false;
            // 
            // cbShowBalanceCurve
            // 
            this.cbShowBalanceCurve.AutoSize = true;
            this.cbShowBalanceCurve.Location = new System.Drawing.Point(3, 104);
            this.cbShowBalanceCurve.Name = "cbShowBalanceCurve";
            this.cbShowBalanceCurve.Size = new System.Drawing.Size(158, 17);
            this.cbShowBalanceCurve.TabIndex = 23;
            this.cbShowBalanceCurve.Tag = "TitleShowBalanceCurve";
            this.cbShowBalanceCurve.Text = "показать кривую баланса";
            this.cbShowBalanceCurve.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbShowBalanceCurve.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(107, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 22;
            this.label1.Tag = "TitleTimeframeSmall";
            this.label1.Text = "таймфрейм";
            // 
            // tbTimeframe
            // 
            this.tbTimeframe.Location = new System.Drawing.Point(3, 55);
            this.tbTimeframe.Name = "tbTimeframe";
            this.tbTimeframe.Size = new System.Drawing.Size(98, 20);
            this.tbTimeframe.TabIndex = 21;
            this.tbTimeframe.Text = "1440";
            // 
            // cbStartFrom
            // 
            this.cbStartFrom.AutoSize = true;
            this.cbStartFrom.Location = new System.Drawing.Point(107, 6);
            this.cbStartFrom.Name = "cbStartFrom";
            this.cbStartFrom.Size = new System.Drawing.Size(69, 17);
            this.cbStartFrom.TabIndex = 20;
            this.cbStartFrom.Tag = "TitleBeginFrom";
            this.cbStartFrom.Text = "начать с";
            this.cbStartFrom.UseVisualStyleBackColor = true;
            this.cbStartFrom.CheckedChanged += new System.EventHandler(this.CbStartFromCheckedChanged);
            // 
            // cbFilterByMagic
            // 
            this.cbFilterByMagic.AutoSize = true;
            this.cbFilterByMagic.Location = new System.Drawing.Point(107, 32);
            this.cbFilterByMagic.Name = "cbFilterByMagic";
            this.cbFilterByMagic.Size = new System.Drawing.Size(110, 17);
            this.cbFilterByMagic.TabIndex = 19;
            this.cbFilterByMagic.Tag = "TitleFilterByMagic";
            this.cbFilterByMagic.Text = "фильтр по Magic";
            this.cbFilterByMagic.UseVisualStyleBackColor = true;
            this.cbFilterByMagic.CheckedChanged += new System.EventHandler(this.CbFilterByMagicCheckedChanged);
            // 
            // tbMagic
            // 
            this.tbMagic.Enabled = false;
            this.tbMagic.Location = new System.Drawing.Point(3, 29);
            this.tbMagic.Name = "tbMagic";
            this.tbMagic.Size = new System.Drawing.Size(98, 20);
            this.tbMagic.TabIndex = 18;
            // 
            // cbDefaultUploadQuotes
            // 
            this.cbDefaultUploadQuotes.AutoSize = true;
            this.cbDefaultUploadQuotes.Checked = true;
            this.cbDefaultUploadQuotes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDefaultUploadQuotes.Location = new System.Drawing.Point(3, 81);
            this.cbDefaultUploadQuotes.Name = "cbDefaultUploadQuotes";
            this.cbDefaultUploadQuotes.Size = new System.Drawing.Size(145, 17);
            this.cbDefaultUploadQuotes.TabIndex = 17;
            this.cbDefaultUploadQuotes.Tag = "TitleQuotesWithoutRequest";
            this.cbDefaultUploadQuotes.Text = "котировки без запроса";
            this.cbDefaultUploadQuotes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbDefaultUploadQuotes.UseVisualStyleBackColor = true;
            // 
            // dpStart
            // 
            this.dpStart.CustomFormat = "dd.MM.yyyy";
            this.dpStart.Enabled = false;
            this.dpStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dpStart.Location = new System.Drawing.Point(3, 3);
            this.dpStart.Name = "dpStart";
            this.dpStart.Size = new System.Drawing.Size(98, 20);
            this.dpStart.TabIndex = 16;
            // 
            // btnShowOptions
            // 
            this.btnShowOptions.Location = new System.Drawing.Point(8, 41);
            this.btnShowOptions.Name = "btnShowOptions";
            this.btnShowOptions.Size = new System.Drawing.Size(121, 23);
            this.btnShowOptions.TabIndex = 10;
            this.btnShowOptions.Tag = "TitleShowOptions";
            this.btnShowOptions.Text = "показать опции";
            this.btnShowOptions.UseVisualStyleBackColor = true;
            this.btnShowOptions.Click += new System.EventHandler(this.BtnShowOptionsClick);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnUpdate.Location = new System.Drawing.Point(8, 6);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(121, 29);
            this.btnUpdate.TabIndex = 3;
            this.btnUpdate.Tag = "TitleCalculate";
            this.btnUpdate.Text = "Расчет";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.BtnUpdateClick);
            // 
            // tabPageStat
            // 
            this.tabPageStat.Controls.Add(this.dgStat);
            this.tabPageStat.Location = new System.Drawing.Point(4, 22);
            this.tabPageStat.Name = "tabPageStat";
            this.tabPageStat.Size = new System.Drawing.Size(573, 375);
            this.tabPageStat.TabIndex = 2;
            this.tabPageStat.Tag = "TitleStatistics";
            this.tabPageStat.Text = "Статистика";
            // 
            // dgStat
            // 
            this.dgStat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dgStat.CaptionHeight = 20;
            this.dgStat.CellEditMode = FastGrid.FastGrid.CellEditModeTrigger.LeftClick;
            this.dgStat.CellHeight = 18;
            this.dgStat.CellPadding = 5;
            this.dgStat.ColorAltCellBackground = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(247)))), ((int)(((byte)(247)))));
            this.dgStat.ColorAnchorCellBackground = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.dgStat.ColorCellBackground = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(247)))), ((int)(((byte)(247)))));
            this.dgStat.ColorCellFont = System.Drawing.Color.Black;
            this.dgStat.ColorCellOutlineLower = System.Drawing.Color.White;
            this.dgStat.ColorCellOutlineUpper = System.Drawing.Color.DarkGray;
            this.dgStat.ColorSelectedCellBackground = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(247)))), ((int)(((byte)(227)))));
            this.dgStat.ColorSelectedCellFont = System.Drawing.Color.Black;
            this.dgStat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgStat.FitWidth = false;
            this.dgStat.FontAnchoredRow = null;
            this.dgStat.FontCell = null;
            this.dgStat.FontHeader = null;
            this.dgStat.FontSelectedCell = null;
            this.dgStat.Location = new System.Drawing.Point(0, 0);
            this.dgStat.MinimumTableWidth = null;
            this.dgStat.MultiSelectEnabled = false;
            this.dgStat.Name = "dgStat";
            this.dgStat.Padding = new System.Windows.Forms.Padding(3);
            this.dgStat.SelectEnabled = true;
            this.dgStat.Size = new System.Drawing.Size(573, 375);
            this.dgStat.StickFirst = false;
            this.dgStat.StickLast = false;
            this.dgStat.TabIndex = 2;
            // 
            // tabPageEquityCurve
            // 
            this.tabPageEquityCurve.Controls.Add(this.chartProfit);
            this.tabPageEquityCurve.Location = new System.Drawing.Point(4, 22);
            this.tabPageEquityCurve.Name = "tabPageEquityCurve";
            this.tabPageEquityCurve.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageEquityCurve.Size = new System.Drawing.Size(573, 375);
            this.tabPageEquityCurve.TabIndex = 1;
            this.tabPageEquityCurve.Tag = "TitleProfit";
            this.tabPageEquityCurve.Text = "Доходность";
            this.tabPageEquityCurve.UseVisualStyleBackColor = true;
            // 
            // chartProfit
            // 
            this.chartProfit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartProfit.InnerMarginXLeft = 10;
            this.chartProfit.Location = new System.Drawing.Point(3, 3);
            this.chartProfit.MarginXLeft = 10;
            this.chartProfit.MarginXRight = 10;
            this.chartProfit.MarginYBottom = 10;
            this.chartProfit.MarginYTop = 10;
            this.chartProfit.Name = "chartProfit";
            this.chartProfit.RenderPolygons = true;
            this.chartProfit.ScaleDivisionXMaxPixel = -1;
            this.chartProfit.ScaleDivisionXMinPixel = 70;
            this.chartProfit.ScaleDivisionYMaxPixel = 60;
            this.chartProfit.ScaleDivisionYMinPixel = 20;
            this.chartProfit.ScrollBarHeight = 80;
            this.chartProfit.ShowHints = true;
            this.chartProfit.ShowScaleDivisionXLabel = true;
            this.chartProfit.ShowScaleDivisionYLabel = true;
            this.chartProfit.Size = new System.Drawing.Size(567, 369);
            this.chartProfit.TabIndex = 1;
            // 
            // tabPageProfit1000
            // 
            this.tabPageProfit1000.Controls.Add(this.chartProfit1000);
            this.tabPageProfit1000.Location = new System.Drawing.Point(4, 22);
            this.tabPageProfit1000.Name = "tabPageProfit1000";
            this.tabPageProfit1000.Size = new System.Drawing.Size(573, 375);
            this.tabPageProfit1000.TabIndex = 3;
            this.tabPageProfit1000.Tag = "TitleProfitBy1000";
            this.tabPageProfit1000.Text = "Доходность на 1000";
            this.tabPageProfit1000.UseVisualStyleBackColor = true;
            // 
            // chartProfit1000
            // 
            this.chartProfit1000.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartProfit1000.InnerMarginXLeft = 10;
            this.chartProfit1000.Location = new System.Drawing.Point(0, 0);
            this.chartProfit1000.MarginXLeft = 10;
            this.chartProfit1000.MarginXRight = 10;
            this.chartProfit1000.MarginYBottom = 10;
            this.chartProfit1000.MarginYTop = 10;
            this.chartProfit1000.Name = "chartProfit1000";
            this.chartProfit1000.RenderPolygons = true;
            this.chartProfit1000.ScaleDivisionXMaxPixel = -1;
            this.chartProfit1000.ScaleDivisionXMinPixel = 70;
            this.chartProfit1000.ScaleDivisionYMaxPixel = 60;
            this.chartProfit1000.ScaleDivisionYMinPixel = 20;
            this.chartProfit1000.ScrollBarHeight = 80;
            this.chartProfit1000.ShowHints = true;
            this.chartProfit1000.ShowScaleDivisionXLabel = true;
            this.chartProfit1000.ShowScaleDivisionYLabel = true;
            this.chartProfit1000.Size = new System.Drawing.Size(573, 375);
            this.chartProfit1000.TabIndex = 0;
            // 
            // tabPageDrowDawn
            // 
            this.tabPageDrowDawn.Controls.Add(this.chartDrawDown);
            this.tabPageDrowDawn.Location = new System.Drawing.Point(4, 22);
            this.tabPageDrowDawn.Name = "tabPageDrowDawn";
            this.tabPageDrowDawn.Size = new System.Drawing.Size(573, 375);
            this.tabPageDrowDawn.TabIndex = 4;
            this.tabPageDrowDawn.Tag = "TitleDrowDawn";
            this.tabPageDrowDawn.Text = "Прибыль / Просадка";
            this.tabPageDrowDawn.UseVisualStyleBackColor = true;
            // 
            // chartDrawDown
            // 
            chartArea1.CursorX.IsUserSelectionEnabled = true;
            chartArea1.Name = "ChartArea1";
            this.chartDrawDown.ChartAreas.Add(chartArea1);
            this.chartDrawDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartDrawDown.Location = new System.Drawing.Point(0, 0);
            this.chartDrawDown.Name = "chartDrawDown";
            series1.BorderWidth = 10;
            series1.ChartArea = "ChartArea1";
            series1.Color = System.Drawing.Color.Firebrick;
            series1.Name = "SeriesDrowDawn";
            series1.ToolTip = "#VALY{F2} %";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Date;
            series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series2.ChartArea = "ChartArea1";
            series2.Color = System.Drawing.Color.ForestGreen;
            series2.Name = "SeriesRunUp";
            series2.ToolTip = "#VALY{F2} %";
            series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Date;
            series2.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            this.chartDrawDown.Series.Add(series1);
            this.chartDrawDown.Series.Add(series2);
            this.chartDrawDown.Size = new System.Drawing.Size(573, 375);
            this.chartDrawDown.TabIndex = 2;
            this.chartDrawDown.Text = "chart1";
            title1.Name = "Title1";
            title1.Text = "DrawDown / RunUp (%)";
            this.chartDrawDown.Titles.Add(title1);
            this.chartDrawDown.DoubleClick += new System.EventHandler(this.ChartDrowDawnScaleReset);
            // 
            // tabPageEquityDrawDown
            // 
            this.tabPageEquityDrawDown.Controls.Add(this.chartEquityDrawDown);
            this.tabPageEquityDrawDown.Location = new System.Drawing.Point(4, 22);
            this.tabPageEquityDrawDown.Name = "tabPageEquityDrawDown";
            this.tabPageEquityDrawDown.Size = new System.Drawing.Size(573, 375);
            this.tabPageEquityDrawDown.TabIndex = 5;
            this.tabPageEquityDrawDown.Text = "Средства / Просадка";
            this.tabPageEquityDrawDown.UseVisualStyleBackColor = true;
            // 
            // chartEquityDrawDown
            // 
            chartArea2.CursorX.IsUserSelectionEnabled = true;
            chartArea2.Name = "ChartArea1";

            chartArea3.CursorX.IsUserSelectionEnabled = true;
            chartArea3.Name = "ChartArea3";
            this.chartEquityDrawDown.ChartAreas.Add(chartArea2);
            this.chartEquityDrawDown.ChartAreas.Add(chartArea3);

            this.chartEquityDrawDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartEquityDrawDown.Location = new System.Drawing.Point(0, 0);
            this.chartEquityDrawDown.Name = "chartEquityDrawDown";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Area;
            series3.Color = System.Drawing.Color.ForestGreen;
            series3.Name = "Equity";
            series3.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Date;
            series4.ChartArea = "ChartArea3";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Area;
            series4.Color = System.Drawing.Color.Firebrick;
            series4.Name = "DrawDown";
            series4.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Date;
            series3.ToolTip = "#VALY{F2}";
            series4.ToolTip = "#VALY{F2}";
            this.chartEquityDrawDown.Series.Add(series3);
            this.chartEquityDrawDown.Series.Add(series4);
            this.chartEquityDrawDown.Size = new System.Drawing.Size(573, 375);
            this.chartEquityDrawDown.TabIndex = 0;
            this.chartEquityDrawDown.DoubleClick += new System.EventHandler(this.ChartDrowDawnScaleReset);
            // 
            // AccountTradeResultsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(581, 424);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panelProgress);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AccountTradeResultsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "TitleTradeOperationsHistory";
            this.Text = "История торговых операций";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AccountTradeResultsFormFormClosing);
            this.Load += new System.EventHandler(this.AccountTradeResultsFormLoad);
            this.ResizeEnd += new System.EventHandler(this.AccountTradeResultsFormResizeEnd);
            this.panelProgress.ResumeLayout(false);
            this.panelProgress.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabPageSetup.ResumeLayout(false);
            this.panelOptions.ResumeLayout(false);
            this.panelOptions.PerformLayout();
            this.tabPageStat.ResumeLayout(false);
            this.tabPageEquityCurve.ResumeLayout(false);
            this.tabPageProfit1000.ResumeLayout(false);
            this.tabPageDrowDawn.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartDrawDown)).EndInit();
            this.tabPageEquityDrawDown.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartEquityDrawDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Panel panelProgress;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lbProgress;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageSetup;
        private System.Windows.Forms.Button btnMakeReportHTML;
        private System.Windows.Forms.Panel panelOptions;
        private System.Windows.Forms.CheckBox cbShowBalanceCurve;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbTimeframe;
        private System.Windows.Forms.CheckBox cbStartFrom;
        private System.Windows.Forms.CheckBox cbFilterByMagic;
        private System.Windows.Forms.TextBox tbMagic;
        private System.Windows.Forms.CheckBox cbDefaultUploadQuotes;
        private System.Windows.Forms.DateTimePicker dpStart;
        private System.Windows.Forms.Button btnShowOptions;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.TabPage tabPageStat;
        private FastGrid.FastGrid dgStat;
        private System.Windows.Forms.TabPage tabPageEquityCurve;
        private FastMultiChart.FastMultiChart chartProfit;
        private System.Windows.Forms.TabPage tabPageProfit1000;
        private FastMultiChart.FastMultiChart chartProfit1000;
        private System.Windows.Forms.TabPage tabPageDrowDawn;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartDrawDown;
        private System.Windows.Forms.TabPage tabPageEquityDrawDown;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartEquityDrawDown;
    }
}