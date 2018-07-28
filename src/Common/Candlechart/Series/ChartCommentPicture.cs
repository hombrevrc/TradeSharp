using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSharp.Util;

namespace Candlechart.Series
{
    public class ChartCommentPicture : ChartComment
    {
        [LocalizedDisplayName("TitlePivotPrice")]
        [LocalizedCategory("TitleMain")]
        [Browsable(false)]
        public Bitmap Picture { get; set; }

        public void Draw(Graphics grf, float left, float top, float width, float height)
        {
            if (Picture != null)
                grf.DrawImage(Picture, left, top, width, height);
        }
    }
}