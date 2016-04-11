using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using FastGrid;
using QuoteManager.BL;
using System.Linq;
using System.Threading;
using Entity;
using TradeSharp.Util;

namespace QuoteManager
{
    public partial class MainForm : Form
    {
        private readonly string iniFilePath = ExecutablePath.ExecPath + "\\quotemgr.txt";
        private const string IniSectPath = "path";
        private const string IniKeySrcFolder = "quoteFolder";

        public MainForm()
        {
            InitializeComponent();
            SetupGrid();
        }

        private void SetupGrid()
        {
            gridInfo.MultiSelectEnabled = true;
            gridInfo.ColorAltCellBackground = Color.FromArgb(220, 220, 212);
            gridInfo.Columns.Add(new FastColumn("TickerName", "Тикер")
            { ColumnWidth = 68, SortOrder = FastColumnSort.Ascending });
            gridInfo.Columns.Add(new FastColumn("Size", "KB") { ColumnWidth = 68 });
            gridInfo.Columns.Add(new FastColumn("StartDate", "Начало")
            { ColumnWidth = 71, FormatString = "dd.MM.yyyy" });
            gridInfo.Columns.Add(new FastColumn("EndDate", "Конец")
            { ColumnWidth = 71, FormatString = "dd.MM.yyyy" });
            gridInfo.MinimumTableWidth = gridInfo.Columns.Sum(c => c.ColumnWidth);
        }

        private void BtnBrowseQuoteFolderClick(object sender, EventArgs e)
        {
            var dlg = new FolderBrowserDialog
            {
                Description = "Каталог котировок",
                ShowNewFolderButton = true
            };
            if (Directory.Exists(tbQuoteFolder.Text))
                dlg.SelectedPath = tbQuoteFolder.Text;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                tbQuoteFolder.Text = dlg.SelectedPath;
                OnFolderSelected(tbQuoteFolder.Text);
            }
        }

        private void OnFolderSelected(string path)
        {
            new IniFile(iniFilePath).WriteValue(IniSectPath, IniKeySrcFolder, path);
            // прочитать статистику из каталога
            ShowQuoteFolderStat(path);
        }

        private void ShowQuoteFolderStat(string path)
        {
            if (!Directory.Exists(path)) return;
            var infos = new List<QuoteFileInfo>();
            foreach (var fileName in Directory.GetFiles(path, "*.quote"))
            {
                var inf = QuoteFileInfo.ReadFile(fileName);
                if (inf != null) infos.Add(inf);
            }
            var totalFiles = infos.Count;
            var totalSizeKb = infos.Sum(i => i.Size) / 1024;

            var sb = new StringBuilder();
            sb.AppendLine(string.Format("{0} файлов, {1} KB котировок", totalFiles, totalSizeKb));
            boxInfo.Text += sb.ToString();

            gridInfo.DataBind(infos);
        }

        private void MainFormLoad(object sender, EventArgs e)
        {
            var quoteFolder = new IniFile(iniFilePath).ReadValue(IniSectPath, IniKeySrcFolder, string.Empty);
            if (!string.IsNullOrEmpty(quoteFolder))
            {
                tbQuoteFolder.Text = quoteFolder;
                tbDaysOffQuoteFolder.Text = quoteFolder;
                ShowQuoteFolderStat(quoteFolder);
            }

            tbStartDayOff.Text = ((DayOfWeek)StandardDaysOff.startDayOff).ToString();
            tbDaysOffDuration.Text = StandardDaysOff.durationHours.ToString();
            tbStartHourOff.Text = StandardDaysOff.startHourOff.ToString();
        }

        private void MenuitemTrimHistoryClick(object sender, EventArgs e)
        {
            // для всех либо для выбранных строк
            var selectedRows = gridInfo.rows.Where(r => r.Selected).ToList();
            if (selectedRows.Count == 0)
                selectedRows.AddRange(gridInfo.rows.ToList());
            if (selectedRows.Count == 0) return;

            // получить список записей QuoteFileInfo
            var fileInfos = selectedRows.Select(r => (QuoteFileInfo)r.ValueObject).ToList();

            // открыть диалог отсечения истории
            var dlg = new TruncateHistoryForm(fileInfos);
            dlg.ShowDialog();
        }

        private void MenuitemMakeIndexesClick(object sender, EventArgs e)
        {
            var quoteInfo = gridInfo.rows.Select(r => (QuoteFileInfo)r.ValueObject).ToList();
            new IndexMakerForm(tbQuoteFolder.Text, quoteInfo).ShowDialog();
        }

        private void btnBrowseDaysOffFilterQuoteFolder_Click(object sender, EventArgs e)
        {
            var dlg = new FolderBrowserDialog
            {
                Description = "Каталог котировок T#",
                ShowNewFolderButton = false
            };
            if (dlg.ShowDialog() == DialogResult.OK)
                tbDaysOffQuoteFolder.Text = dlg.SelectedPath;
        }

        private void btnFilterDaysOff_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbDaysOffQuoteFolder.Text)) return;
            if (!Directory.Exists(tbDaysOffQuoteFolder.Text)) return;

            InitDaysOff();
            var startTime = DateTime.ParseExact(tbDaysOffFilterStartTime.Text, "yyyy-MM-dd HH:mm",
                CultureInfo.InvariantCulture, DateTimeStyles.None);

            foreach (var file in Directory.GetFiles(tbDaysOffQuoteFolder.Text, "*.quote"))
            {
                ProcessDaysOffFilteredFile(file, startTime);
            }
        }

        private void InitDaysOff()
        {
            StandardDaysOff.startDayOff = (int) Enum.Parse(typeof (DayOfWeek), tbStartDayOff.Text);
            StandardDaysOff.durationHours = tbDaysOffDuration.Text.ToInt();
            StandardDaysOff.startHourOff = tbStartHourOff.Text.ToInt();
        }

        private void ProcessDaysOffFilteredFile(string path, DateTime startTime)
        {
            var symbol = Path.GetFileNameWithoutExtension(path);
            var precision = DalSpot.Instance.GetPrecision10(symbol);

            int filtered = 0, total = 0;
            var filteredCandles = new List<CandleData>();
            using (var sr = new StreamReader(path))
            {
                DateTime? time = null;
                CandleData prev = null;
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line)) continue;

                    var candle = CandleData.ParseLine(line, ref time, precision, ref prev);
                    if (candle == null) continue;

                    prev = candle;
                    total++;
                    if (candle.timeOpen >= startTime && StandardDaysOff.IsDayOff(candle.timeOpen))
                    {
                        filtered++;
                        continue;
                    }
                    filteredCandles.Add(candle);
                }
            }

            CandleData.SaveInFile(path + ".temp", symbol, filteredCandles);
            LogDaysOffMessage($"{symbol}: отфильтровано {filtered} из {total} свечей M1");
            Thread.Sleep(10);
        }

        private void LogDaysOffMessage(string msg)
        {
            Invoke(new Action<string>(s => tbFilterDaysOffResult.AppendText(s + Environment.NewLine)), msg);
        }

        private void btnBuildSQLFilter_Click(object sender, EventArgs e)
        {
            InitDaysOff();
            var startTime = DateTime.ParseExact(tbDaysOffFilterStartTime.Text, "yyyy-MM-dd HH:mm",
                CultureInfo.InvariantCulture, DateTimeStyles.None);

            while (true)
            {
                DateTime dayStart, dayEnd;
                GetNextDaysOff(startTime, out dayStart, out dayEnd);
                if (dayStart >= DateTime.Now) break;
                startTime = dayEnd.AddHours(1);

                tbFilterDaysOffResult.AppendText($"delete from QUOTE where [date] between '{dayStart:yyyy-MM-dd HH:mm:ss}' and '{dayEnd:yyyy-MM-dd HH:mm:ss}';" + 
                    Environment.NewLine);
            }
            
        }

        private void GetNextDaysOff(DateTime time, out DateTime startOff, out DateTime endOff)
        {
            DateTime startWeekDay;
            try
            {
                startWeekDay = time.DayOfWeek == DayOfWeek.Sunday
                    ? time.AddDays(-6).Date
                    : time.AddDays(-(int)time.DayOfWeek).Date;
            }
            catch (ArgumentOutOfRangeException)
            {
                startWeekDay = time;
            }
            startOff = startWeekDay.AddDays(StandardDaysOff.startDayOff).AddHours(StandardDaysOff.startHourOff);
            endOff = startWeekDay.AddDays(StandardDaysOff.startDayOff).AddHours(StandardDaysOff.startHourOff + StandardDaysOff.durationHours);
        }

        private void btnMergeBrowseQuote_Click(object sender, EventArgs e)
        {
            BrowseMergeFile("quote", "Файл котировок T#", tbMergeQuotePath);
        }

        private void btnMergeBrowseCsv_Click(object sender, EventArgs e)
        {
            BrowseMergeFile("csv", "Файл котировок MT4 (csv)", tbMergeCsvPath);
        }

        private void BrowseMergeFile(string ext, string title, TextBox target)
        {
            var dlg = new OpenFileDialog
            {
                Title = title,
                DefaultExt = ext,
                Filter = $"Файлы *.{ext}|*.{ext}|Все файлы (*.*)|*.*",
                FilterIndex = 0,
                CheckFileExists = true
            };

            if (dlg.ShowDialog() == DialogResult.OK)
                target.Text = dlg.FileName;
        }

        private void btnMergeQuotes_Click(object sender, EventArgs e)
        {
            var endDate = dpMergeEndDate.Value;
            var pathQuote = tbMergeQuotePath.Text;
            if (!File.Exists(pathQuote)) return;
            var pathCsv = tbMergeCsvPath.Text;
            if (!File.Exists(pathCsv)) return;

            var timeframe = tbMergeTimeframe.Text.ToInt();
            var digits = tbMergeDigits.Text.ToInt();

            new SharpMt4QuoteMerger(pathQuote, pathCsv, endDate, timeframe, digits).Merge();
        }
    }
}