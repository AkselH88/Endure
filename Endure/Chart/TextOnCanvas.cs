﻿using System;
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

        readonly List<TextBlock> verticalText = new List<TextBlock>();
        readonly List<TextBlock> horizontalText = new List<TextBlock>();
        
        public List<double> VertivalTextPosition { get; private set; }
        public Dictionary<string, double> HorizontalTextPositions { get; private set; }

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
                verticalText.Add(textBlock);

                VertivalTextPosition.Add(Canvas.GetTop(textBlock));
            }
        }

        void InitializeHorizontalText(Canvas canvas)
        {
            double width = canvas.ActualWidth - (common.Offset + common.VerticalStartPos);
            double height = canvas.ActualHeight - 2 * common.Offset;

            HorizontalPixelSeperator = width / common.HorizontalElements;
            HorizontalTextPositions = new Dictionary<string, double>();

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
                horizontalText.Add(textBlock);

                HorizontalTextPositions[$"{textBlock.Text}.{textBlock.ToolTip}"] = Canvas.GetLeft(textBlock);
            }
        }

        public void ReaplyTextOnCanvas(Common common, Canvas canvas)
        {
            if(Initialized)
            {
                foreach (TextBlock textBlock in verticalText)
                {
                    canvas.Children.Add(textBlock);
                }
                foreach (TextBlock textBlock in horizontalText)
                {
                    canvas.Children.Add(textBlock);
                    //ellipses.ReaplyEllipsesToCanvas($"{textBlock.Text}.{textBlock.ToolTip}", canvas);
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
            horizontalText.Add(textBlock);
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
            string outOfRange = $"{horizontalText[0].Text}.{horizontalText[0].ToolTip}";

            HorizontalTextPositions.Clear();

            for (int i = 1; i < horizontalText.Count; i++)
            {
                horizontalText[i - 1].ToolTip = horizontalText[i].ToolTip;
                horizontalText[i - 1].Text = horizontalText[i].Text;

                HorizontalTextPositions[$"{horizontalText[i - 1].Text}.{horizontalText[i - 1].ToolTip}"] = Canvas.GetLeft(horizontalText[i - 1]);
            }
            
            DayOffset++;
            horizontalText[^1].ToolTip = $"{DateTime.Today.AddDays(DayOffset).Month}.{DateTime.Today.AddDays(DayOffset).Year}";
            horizontalText[^1].Text = $"{DateTime.Today.AddDays(DayOffset).Day}";

            HorizontalTextPositions[$"{horizontalText[^1].Text}.{horizontalText[^1].ToolTip}"] = Canvas.GetLeft(horizontalText[^1]);

            string reaply = $"{horizontalText[^1].Text}.{horizontalText[^1].ToolTip}";
            string[] returnString = { outOfRange, reaply };

            return returnString;
        }

        public string[] OnMoveBackward(Canvas canvas)
        {
            string outOfRange = $"{horizontalText[^1].Text}.{horizontalText[^1].ToolTip}";

            HorizontalTextPositions.Clear();

            for (int i = horizontalText.Count - 1; i > 0; i--)
            {
                horizontalText[i].ToolTip = horizontalText[i - 1].ToolTip;
                horizontalText[i].Text = horizontalText[i - 1].Text;

                HorizontalTextPositions[$"{horizontalText[i].Text}.{horizontalText[i].ToolTip}"] = Canvas.GetLeft(horizontalText[i]);
            }

            DayOffset--;
            horizontalText[0].ToolTip = $"{DateTime.Today.AddDays(DayOffset - common.HorizontalElements).Month}.{DateTime.Today.AddDays(DayOffset - common.HorizontalElements).Year}";
            horizontalText[0].Text = $"{DateTime.Today.AddDays(DayOffset - common.HorizontalElements).Day}";

            HorizontalTextPositions[$"{horizontalText[0].Text}.{horizontalText[0].ToolTip}"] = Canvas.GetLeft(horizontalText[0]);

            string reaply = $"{horizontalText[0].Text}.{horizontalText[0].ToolTip}";
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

                foreach (TextBlock text in verticalText)
                {
                    double verticanPosition = height * (common.VerticalElements - verticalText.IndexOf(text)) / common.VerticalElements;

                    Canvas.SetTop(text, verticanPosition + common.FontSize / 2);

                    VertivalTextPosition[verticalText.IndexOf(text)] = verticanPosition + common.FontSize / 2;
                }
                foreach (TextBlock text in horizontalText)
                {
                    double horisontalPosition = width * horizontalText.IndexOf(text) / common.HorizontalElements + common.VerticalStartPos - common.FontSize;

                    Canvas.SetTop(text, height + common.Offset);
                    Canvas.SetLeft(text, horisontalPosition);

                    HorizontalTextPositions[$"{text.Text}.{text.ToolTip}"] = horisontalPosition;
                }
            }
        }

        public double MinHeight()
        {
            double height = 0;

            foreach (TextBlock block in verticalText)
            {
                height += block.ActualHeight;
            }
            return height;
        }

        public double MinWidth()
        {
            double width = 0;

            foreach (TextBlock block in horizontalText)
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
            foreach (TextBlock block in verticalText)
            {
                block.Text = $"{common.CurrentMin + scale * verticalText.IndexOf(block)}";
                block.Width = common.VerticalOffset;
            }
        }
    }
}
