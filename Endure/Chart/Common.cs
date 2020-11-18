using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Endure
{
    public class Common
    {
        public Common()
        {
            this.Offset = Defoult.Offset;
            this.VerticalOffset = Defoult.Offset;
            this.FontSize = Defoult.FontSize;
            this.Padding = Defoult.Padding;
            this.VerticalElements = Defoult.VerticalElements;
            this.HorizontalElements = Defoult.HorizontalElements;

            this.CurrentMax = Defoult.CurrentMax;
            this.CurrentMin = Defoult.CurrentMin;
        }

        public int Offset { get; set; }
        public double VerticalOffset { get; set; }
        public double VerticalStartPos { get { return VerticalOffset + Padding; } }
        public int FontSize { get; set; }
        public int Padding { get; set; }
        public int VerticalElements { get; set; }
        public int HorizontalElements { get; set; }
        public int CurrentMax { get; set; }
        public int CurrentMin { get; set; }
        public double Height { get; private set; }
        public double Width { get; private set; }

        public void RevertToDefoult()
        {
            this.Offset = Defoult.Offset;
            this.VerticalOffset = Defoult.Offset;
            this.FontSize = Defoult.FontSize;
            this.Padding = Defoult.Padding;
            this.VerticalElements = Defoult.VerticalElements;
            this.HorizontalElements = Defoult.HorizontalElements;

            this.CurrentMax = Defoult.CurrentMax;
            this.CurrentMin = Defoult.CurrentMin;
        }

        public void OnSizeChange(Canvas canvas)
        {
            this.Height = canvas.ActualHeight;
            this.Width = canvas.ActualWidth;
        }
    }

    public static class Defoult
    {
        public static readonly int Offset = 20;
        public static readonly int VerticalElements = 10;
        public static readonly int HorizontalElements = 10;
        public static readonly int FontSize = 14;
        public static readonly int Padding = 4;

        public static readonly int CurrentMax = 10;
        public static readonly int CurrentMin = 0;
    }
}
