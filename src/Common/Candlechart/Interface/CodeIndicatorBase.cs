using System.Collections.Generic;
using Candlechart.Series;
using Entity;

namespace Candlechart.Interface
{
    public abstract class CodeIndicatorBase
    {
        public virtual AsteriskTooltip CheckCandleMark(List<CandleData> candles,
            List<List<double>> series, int index)
        {
            return null;
        }

        public virtual double CalcIndcatorValue(List<CandleData> candles,
            List<List<double>> series, int index)
        {
            return 0;
        }
    }
}
