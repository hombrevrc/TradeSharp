using System;
using TradeSharpEntity.Model;

namespace TradeSharpEntity.Analysis
{
    public partial class TradeSignalAnalysis
    {
        struct ProsCons
        {
            public float pro, con;

            public ProsCons(float p, float c)
            {
                pro = p;
                con = c;
            }
        }

        class OffsetMistake
        {
            public int offset;

            public int count;

            public decimal mistakeSum;
        }

        class SignalStat
        {
            private int side;

            public float enter;

            public DateTime time, endTime;

            private float maxPros, maxCons;

            public readonly ProsCons[] proCons;

            private int timeframes, timeIndex = 0;

            public SignalStat(TradeSignal s, int tf, int timeframes)
            {
                side = s.Side;
                enter = (float)s.Enter;
                time = s.Time;
                this.timeframes = timeframes;
                endTime = s.Time.AddMinutes(tf * timeframes);
                proCons = new ProsCons[timeframes];
            }

            public void UpdateProsCons(Candle c, DateTime t)
            {
                var pro = side > 0 ? c.h - enter : enter - c.l;
                var con = side > 0 ? enter - c.l : c.h - enter;
                maxPros = Math.Max(pro, maxPros);
                maxCons = Math.Max(con, maxCons);
                var index = (int)(t - time).TotalMinutes / timeframes;
                var newVal = new ProsCons(maxPros, maxCons);
                if (index == timeIndex)
                    proCons[index] = newVal;
                else
                {
                    for (timeIndex = timeIndex + 1; timeIndex < proCons.Length && timeIndex <= index; timeIndex++)
                        proCons[timeIndex] = newVal;
                }
            }
        }
    }
}
