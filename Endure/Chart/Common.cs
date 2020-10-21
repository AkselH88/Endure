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
            this.Offset = defoult.Offset;
            this.FontSize = defoult.FontSize;
            this.VerticalElements = defoult.VerticalElements;
            this.HorizontalElements = defoult.HorizontalElements;

            this.CurrentMax = defoult.CurrentMax;
            this.CurrentMin = defoult.CurrentMin;
        }

        readonly Defoult defoult = new Defoult();

        public int Offset { get; set; }
        public int FontSize { get; set; }
        public int VerticalElements { get; set; }
        public int HorizontalElements { get; set; }
        public int CurrentMax { get; set; }
        public int CurrentMin { get; set; }
        public double Height { get; private set; }
        public double Width { get; private set; }

        public void RevertToDefoult()
        {
            this.Offset = defoult.Offset;
            this.FontSize = defoult.FontSize;
            this.VerticalElements = defoult.VerticalElements;
            this.HorizontalElements = defoult.HorizontalElements;

            this.CurrentMax = defoult.CurrentMax;
            this.CurrentMin = defoult.CurrentMin;
        }

        public void OnSizeChange(Canvas canvas)
        {
            this.Height = canvas.ActualHeight;
            this.Width = canvas.ActualWidth;
        }
    }

    public class Defoult
    {
        public readonly int Offset = 20;
        public readonly int VerticalElements = 10;
        public readonly int HorizontalElements = 10;
        public readonly int FontSize = 14;

        public readonly int CurrentMax = 10;
        public readonly int CurrentMin = 10;
    }
}
