using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Candlechart;
using Candlechart.ChartMath;
using Entity;
using TradeSharp.Client.Forms;
using TradeSharp.Client.Util;
using TradeSharp.Contract.Entity;
using TradeSharp.Robot.Robot;
using TradeSharp.Util;

namespace TradeSharp.Client.BL.Script
{
    [DisplayName("Объемы для робота FX")]
    public class ScriptSetupFxRobotVolumes : TerminalScript
    {
        public class FxRobotVolumeSets
        {
            public FiboLevelRobot TargetRobot { get; set; }

            public RobotSourceType RobotSource { get; set; }

            public bool FarmIsStopped { get; set; }

            public Account Account { get; set; }

            public int VolumeStart { get; set; }

            public int VolumeStep { get; set; }

            public string Message { get; set; }
        }

        public enum RobotSourceType { Тестер = 0, РоботыLive }

        [DisplayName("Роботы")]
        [Category("Основные")]
        [Description("Порядок выбора роботов для установки точек")]
        [PropertyXMLTag("RobotSourceType")]
        public RobotSourceType RobotSource { get; set; }
        
        public override bool CanBeTriggered => true;

        public ScriptSetupFxRobotVolumes()
        {
            ScriptTarget = TerminalScriptTarget.Тикер;
            ScriptName = "Объемы для робота FX";
        }
        
        public override string ActivateScript(string ticker)
        {
            // отыскать киборга
            List<BaseRobot> robots;
            if (RobotSource == RobotSourceType.РоботыLive)
            {
                //if (MainForm.Instance.RobotFarm.State != RobotFarm.RobotFarmState.Stopped)
                robots = MainForm.Instance.RobotFarm.GetRobotCopies();
                if (robots == null || robots.Count == 0) robots = RobotFarm.LoadRobots();
            }
            else // if (RobotSource == RobotSourceType.Тестер)
            {
                if (RoboTesterForm.Instance == null)
                {
                    MessageBox.Show("Окно тестера роботов не запущено");
                    return "Окно тестера роботов не запущено";
                }
                robots = RoboTesterForm.Instance.GetUsedRobots();
            }
            if (robots == null) robots = new List<BaseRobot>();

            var fiboLevelRobots = robots.Where(r => r is FiboLevelRobot).Cast<FiboLevelRobot>();
            var targetRobots =
                fiboLevelRobots.Where(r => r.Graphics.Any(g => g.a == ticker)).ToList();
            if (targetRobots.Count == 0)
            {
                var msg = $"Роботов для {ticker} не найдено";
                MessageBox.Show(msg);
                return msg;
            }

            // если таких киборгов несколько - предложить выбор
            var robot = targetRobots[0];

            if (targetRobots.Count > 1)
            {
                var number = 1;
                var robotNames = targetRobots.Select(r => (object)
                    $"Робот {number++} (A:{DalSpot.Instance.FormatPrice(ticker, r.PriceA)}, B:{DalSpot.Instance.FormatPrice(ticker, r.PriceB)})").ToList();

                object selObj;
                string selText;
                if (!Dialogs.ShowComboDialog("Укажите робота", robotNames, out selObj, out selText, true))
                    return "Робот не выбран";

                var selIndex = robotNames.IndexOf(selText);
                if (selIndex < 0) return "Робот не выбран";
                robot = targetRobots[selIndex];
            }

            var sets = new FxRobotVolumeSets
            {
                TargetRobot = robot,
                FarmIsStopped = MainForm.Instance.RobotFarm.State == RobotFarm.RobotFarmState.Stopped,
                RobotSource = RobotSource,
                Account = AccountStatus.Instance.AccountData
            };

            var dlg = new FxRobotVolumeSetupDlg(sets);
            if (dlg.ShowDialog() != DialogResult.OK)
                return "Отменено";

            if (RobotSource == RobotSourceType.РоботыLive)
            {
                MainForm.Instance.RobotFarm.SetRobotSettings(robots);
                RobotFarm.SaveRobots(robots);
                MainForm.Instance.ShowRobotPortfolioDialog(robot.GetUniqueName());
            }
            else
            {
                RoboTesterForm.Instance.SaveRobots(robots);
                RoboTesterForm.Instance.ReadLastRobotSettings();
                MainForm.Instance.EnsureRoboTesterForm(robot.GetUniqueName());
            }
            return "";
        }

        public override string ActivateScript(bool byTrigger)
        {
            throw new Exception($"Неверный тип вызова скрипта \"{ScriptName}\"");
        }

        public override string ActivateScript(CandleChartControl chart, PointD worldCoords)
        {
            return ActivateScript(chart.Symbol);
        }
    }
}
