using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Windows.Media.Imaging;

using r = Pandemic.Properties.Resources;

namespace Pandemic.Game.DataTypes
{
    static class virusPics
    {
        public static BitmapSource[,] virusCounters;

        private static int size = 45;

        static virusPics()
        {
            //blue, yellow, black, red
            virusCounters = new BitmapSource[,]{{converter(r.blue1.GetHbitmap()), converter(r.blue2.GetHbitmap()), converter(r.blue3.GetHbitmap())},
            {converter(r.yellow1.GetHbitmap()), converter(r.yellow2.GetHbitmap()), converter(r.yellow3.GetHbitmap())},
            {converter(r.black1.GetHbitmap()), converter(r.black2.GetHbitmap()), converter(r.black3.GetHbitmap())},
            {converter(r.red1.GetHbitmap()), converter(r.red2.GetHbitmap()), converter(r.red3.GetHbitmap())}};
        }

        private static BitmapSource converter(IntPtr bm)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bm, IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(size, size));
        }

        public static BitmapSource getImage(int color, int num)
        {
            if(num == 0)
                return null;

            return virusCounters[color, num - 1];
        }
    }
}
