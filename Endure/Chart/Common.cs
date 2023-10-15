using System.Windows.Controls;

namespace Endure
{
    public class Common
    {
        public Common()
        {
            RevertToDefault();
        }

        public int Offset { get; set; }
        public double VerticalOffset { get; set; }
        public double VerticalStartPos { get { return VerticalOffset + Padding; } }
        public int FontSize { get; set; }
        public int Padding { get; set; }
        public int VerticalElements { get; set; }
        public int HorizontalElements { get; set; }
        public int Jump { get; set; }
        public int CurrentMax { get; set; }
        public int CurrentMin { get; set; }
        public double Height { get; private set; }
        public double Width { get; private set; }

        public void RevertToDefault()
        {
            this.Offset = Default.Offset;
            this.VerticalOffset = Default.Offset;
            this.FontSize = Default.FontSize;
            this.Padding = Default.Padding;
            this.VerticalElements = Default.VerticalElements;
            this.HorizontalElements = Default.HorizontalElements;
            this.Jump = Default.Jump;

            this.CurrentMax = Default.CurrentMax;
            this.CurrentMin = Default.CurrentMin;
        }

        public void OnSizeChange(Canvas canvas)
        {
            this.Height = canvas.ActualHeight;
            this.Width = canvas.ActualWidth;
        }
    }

    public static class Default
    {
        public static readonly int Offset = 20;
        public static readonly int VerticalElements = 10;
        public static readonly int HorizontalElements = 10;
        public static readonly int Jump = 1;
        public static readonly int FontSize = 14;
        public static readonly int Padding = 4;

        public static readonly int CurrentMax = 10;
        public static readonly int CurrentMin = 0;
    }
}
