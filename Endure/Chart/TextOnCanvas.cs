using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace Endure
{
    public class TextOnCanvas
    {
        Common common;

        readonly List<TextBlock> verticalTextBlock = new List<TextBlock>();
        readonly List<TextBlock> horizontalTextBlock = new List<TextBlock>();
        
        public List<double> VertivalTextPosition { get; private set; }
        public List<string> HorizontalText { get; private set; }
        public List<double> HorizontalTextPositions { get; private set; }


        private bool Initialized = false;

        public double VerticalPixelSeperator;
        public double HorizontalPixelSeperator;

        int DayOffset;

        public void Initialize(Common common, Canvas canvas)
        {
            if(!Initialized)
            {
                this.common = common;
                DayOffset = -common.HorizontalElements;

                InitializeVerticalText(canvas);
                InitializeHorizontalText(canvas);
                Initialized = true;
            }
        }
        void InitializeVerticalText(Canvas canvas)
        {
            double height = common.Height - 2 * common.Offset;

            VerticalPixelSeperator = height / common.VerticalElements;
            VertivalTextPosition = new List<double>();

            for (int i = 0; i <= common.VerticalElements; i++)
            {
                TextBlock textBlock = new TextBlock
                {
                    Text = $"{i}",
                    FontSize = common.FontSize,
                    Width = common.VerticalOffset,
                    TextAlignment = TextAlignment.Right,
                    Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0))
                };

                Canvas.SetLeft(textBlock, 0);
                Canvas.SetTop(textBlock, VerticalPixelSeperator * (common.VerticalElements - i) + common.FontSize / 2);

                canvas.Children.Add(textBlock);
                verticalTextBlock.Add(textBlock);

                VertivalTextPosition.Add(Canvas.GetTop(textBlock));
            }
        }

        void InitializeHorizontalText(Canvas canvas)
        {
            double width = canvas.ActualWidth - (common.Offset + common.VerticalStartPos);
            double height = canvas.ActualHeight - 2 * common.Offset;

            HorizontalPixelSeperator = width / common.HorizontalElements;
            HorizontalText = new List<string>();
            HorizontalTextPositions = new List<double>();

            for (int i = 0; i <= common.HorizontalElements; i++)
            {
                DayOffset++;

                TextBlock textBlock = new TextBlock
                {
                    ToolTip = $"{DateTime.Today.AddDays(DayOffset).Month}.{DateTime.Today.AddDays(DayOffset).Year}",
                    Text = $"{DateTime.Today.AddDays(DayOffset).Day}",
                    FontSize = common.FontSize,
                    Width = 2 * common.FontSize,
                    TextAlignment = TextAlignment.Center,
                    Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0))
                };

                Canvas.SetLeft(textBlock, common.VerticalStartPos + HorizontalPixelSeperator * i - common.FontSize);
                Canvas.SetTop(textBlock, height + common.Offset);

                canvas.Children.Add(textBlock);
                horizontalTextBlock.Add(textBlock);

                HorizontalText.Add($"{textBlock.Text}.{textBlock.ToolTip}");
                HorizontalTextPositions.Add(Canvas.GetLeft(textBlock));
            }
        }

        public void ReaplyTextOnCanvas(Common common, Canvas canvas)
        {
            if(Initialized)
            {
                foreach (TextBlock textBlock in verticalTextBlock)
                {
                    canvas.Children.Add(textBlock);
                }
                foreach (TextBlock textBlock in horizontalTextBlock)
                {
                    canvas.Children.Add(textBlock);
                }
            }
            else
            {
                Initialize(common, canvas);
            }
        }

        void AddHorizontalText(double left, double top, string text, Canvas canvas)
        {
            TextBlock textBlock = new TextBlock
            {
                Text = text,
                FontSize = common.FontSize,
                Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0))
            };

            Canvas.SetTop(textBlock, top);
            Canvas.SetLeft(textBlock, left);

            canvas.Children.Add(textBlock);
            horizontalTextBlock.Add(textBlock);
        }

        TextBlock NewHorizontalText(double left, double top, string text)
        {
            TextBlock textBlock = new TextBlock
            {
                Text = text,
                FontSize = common.FontSize,
                Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0))
            };

            Canvas.SetTop(textBlock, top);
            Canvas.SetLeft(textBlock, left);

            return textBlock;
        }

        public string[] OnMoveForward(Canvas canvas)
        {
            string outOfRange = $"{horizontalTextBlock[0].Text}.{horizontalTextBlock[0].ToolTip}";

            HorizontalText.Clear();

            for (int i = 1; i < horizontalTextBlock.Count; i++)
            {
                horizontalTextBlock[i - 1].ToolTip = horizontalTextBlock[i].ToolTip;
                horizontalTextBlock[i - 1].Text = horizontalTextBlock[i].Text;

                HorizontalText.Add($"{horizontalTextBlock[i - 1].Text}.{horizontalTextBlock[i - 1].ToolTip}");
            }
            
            DayOffset++;
            horizontalTextBlock[^1].ToolTip = $"{DateTime.Today.AddDays(DayOffset).Month}.{DateTime.Today.AddDays(DayOffset).Year}";
            horizontalTextBlock[^1].Text = $"{DateTime.Today.AddDays(DayOffset).Day}";

            HorizontalText.Add($"{horizontalTextBlock[^1].Text}.{horizontalTextBlock[^1].ToolTip}");

            string reaply = $"{horizontalTextBlock[^1].Text}.{horizontalTextBlock[^1].ToolTip}";
            string[] returnString = { outOfRange, reaply };

            return returnString;
        }

        public string[] OnMoveBackward(Canvas canvas)
        {
            string outOfRange = $"{horizontalTextBlock[^1].Text}.{horizontalTextBlock[^1].ToolTip}";

            HorizontalText.Clear();

            for (int i = horizontalTextBlock.Count - 1; i > 0; i--)
            {
                horizontalTextBlock[i].ToolTip = horizontalTextBlock[i - 1].ToolTip;
                horizontalTextBlock[i].Text = horizontalTextBlock[i - 1].Text;

                //HorizontalText.Insert(0, $"{horizontalTextBlock[i].Text}.{horizontalTextBlock[i].ToolTip}");
                HorizontalText.Add($"{horizontalTextBlock[i].Text}.{horizontalTextBlock[i].ToolTip}");
            }

            DayOffset--;
            horizontalTextBlock[0].ToolTip = $"{DateTime.Today.AddDays(DayOffset - common.HorizontalElements).Month}.{DateTime.Today.AddDays(DayOffset - common.HorizontalElements).Year}";
            horizontalTextBlock[0].Text = $"{DateTime.Today.AddDays(DayOffset - common.HorizontalElements).Day}";

            //HorizontalText.Insert(0, $"{horizontalTextBlock[0].Text}.{horizontalTextBlock[0].ToolTip}");
            HorizontalText.Add($"{horizontalTextBlock[0].Text}.{horizontalTextBlock[0].ToolTip}");

            HorizontalText.Reverse();

            string reaply = $"{horizontalTextBlock[0].Text}.{horizontalTextBlock[0].ToolTip}";
            string[] returnString = { outOfRange, reaply };

            return returnString;
        }

        public void OnSizeChange()
        {
            if(Initialized)
            { 
                double height = common.Height - 2 * common.Offset;
                double width = common.Width - (common.Offset + common.VerticalStartPos);

                VerticalPixelSeperator = height / common.VerticalElements;
                HorizontalPixelSeperator = width / common.HorizontalElements;

                foreach (TextBlock text in verticalTextBlock)
                {
                    double verticanPosition = height * (common.VerticalElements - verticalTextBlock.IndexOf(text)) / common.VerticalElements;

                    Canvas.SetTop(text, verticanPosition + common.FontSize / 2);

                    VertivalTextPosition[verticalTextBlock.IndexOf(text)] = verticanPosition + common.FontSize / 2;
                }
                foreach (TextBlock text in horizontalTextBlock)
                {
                    double horisontalPosition = width * horizontalTextBlock.IndexOf(text) / common.HorizontalElements + common.VerticalStartPos - common.FontSize;

                    Canvas.SetTop(text, height + common.Offset);
                    Canvas.SetLeft(text, horisontalPosition);

                    HorizontalTextPositions[horizontalTextBlock.IndexOf(text)] = horisontalPosition;
                }
            }
        }

        public double MinHeight()
        {
            double height = 0;

            foreach (TextBlock block in verticalTextBlock)
            {
                height += block.ActualHeight;
            }
            return height;
        }

        public double MinWidth()
        {
            double width = 0;

            foreach (TextBlock block in horizontalTextBlock)
            {
                width += block.ActualWidth + common.FontSize;
            }
            return width;
        }

        public void UpdateText()
        {
            TextBlock tempBox = new TextBlock
            {
                FontSize = common.FontSize,
                Text = $"{common.CurrentMax}"
            };

            tempBox.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            tempBox.Arrange(new Rect(tempBox.DesiredSize));
            common.VerticalOffset = tempBox.ActualWidth;

            double maxScale = common.CurrentMax / common.VerticalElements;
            double minScale = common.CurrentMin / common.VerticalElements;
            double scale = (common.CurrentMax - common.CurrentMin) / common.VerticalElements;
            foreach (TextBlock block in verticalTextBlock)
            {
                block.Text = $"{common.CurrentMin + scale * verticalTextBlock.IndexOf(block)}";
                block.Width = common.VerticalOffset;
            }
        }
    }
}
