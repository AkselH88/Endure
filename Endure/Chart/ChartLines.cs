using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Endure
{
    public class ChartLines
    {
        Common common;

        readonly List<Line> Lines = new List<Line>();
        private Pair<Line, Line> XYLines;

        private bool Initialized = false;
        public bool DrawLines { get; set; }

        public void ApplyHeightAndWidthLine(Common common, Canvas canvas)
        {
            if (!Initialized)
            {
                this.common = common;
                Line HeightLine = new Line
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,

                    X1 = common.VerticalStartPos,
                    Y1 = 0,
                    X2 = common.VerticalStartPos,
                    Y2 = common.Height - common.Offset
                };
                Line WidthLine = new Line
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,

                    X1 = common.VerticalStartPos,
                    Y1 = common.Height - common.Offset,
                    X2 = common.Width,
                    Y2 = common.Height - common.Offset,
                };

                XYLines = new Pair<Line, Line>(WidthLine, HeightLine);
                DrawLines = false;
                Initialized = true;
            }

            canvas.Children.Add(XYLines.Second);
            canvas.Children.Add(XYLines.First);
        }

        public void AddLines(Canvas canvas, List<double> VertivalPositions, List<double> HorizontalPositions)
        {
            foreach (double position in VertivalPositions)
            {
                Line line = new Line
                {
                    Name = "Vertical",
                    StrokeThickness = 1,
                    Stroke = Brushes.Gray,

                    X1 = common.VerticalStartPos,
                    X2 = common.Width,

                    Y1 = position + 12,
                    Y2 = position + 12
                };

                canvas.Children.Add(line);
                Lines.Add(line);
            }
            foreach (var position in HorizontalPositions)
            {
                Line line = new Line
                {
                    Name = "Horizontal",
                    StrokeThickness = 1,
                    Stroke = Brushes.Gray,

                    X1 = position + common.FontSize,
                    X2 = position + common.FontSize,

                    Y1 = 0,
                    Y2 = common.Height - common.Offset
                };

                canvas.Children.Add(line);
                Lines.Add(line);
            }
        }

        public void ReApplyLines(Canvas canvas)
        {
            foreach (Line line in Lines)
            {
                canvas.Children.Add(line);
            }
        }

        private void UpdateLines(List<double> VertivalPositions, List<double> HorizontalPosition)
        {
            int i = 0;
            int j = 0;
            foreach (Line line in Lines)
            {
                if (line.Name == "Vertical")
                {
                    line.X1 = common.VerticalStartPos;
                    line.X2 = common.Width;
                    line.Y1 = line.Y2 = VertivalPositions[i] + 12;

                    i++;
                }
                else
                {
                    line.X1 = line.X2 = HorizontalPosition[j] + common.FontSize;
                    //line.Y1 = common.VerticalStartPos;
                    line.Y2 = common.Height - common.Offset;

                    j++;
                }
            }
        }

        public void RemoveLines(Canvas canvas)
        {
            foreach (Line line in Lines)
            {
                canvas.Children.Remove(line);
            }
            Lines.Clear();
        }

        public void OnSizeChange(List<double> VertivalPosition, List<double> HorizontalPosition)
        {
            if (Initialized)
            {
                XYLines.First.X1 = common.VerticalStartPos;
                XYLines.First.Y1 = common.Height - common.Offset;
                XYLines.First.X2 = common.Width;
                XYLines.First.Y2 = common.Height - common.Offset;

                XYLines.Second.X1 = common.VerticalStartPos;
                XYLines.Second.X2 = common.VerticalStartPos;
                XYLines.Second.Y2 = common.Height - common.Offset;
            }
            if (DrawLines)
            {
                UpdateLines(VertivalPosition, HorizontalPosition);
            }
        }
    }
}
