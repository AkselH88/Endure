using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;


namespace Endure
{
    public class EllipsePoints
    {
        Common common;
        readonly Dictionary<string, Ellipse> ellipses = new Dictionary<string, Ellipse>();
        readonly LinesBetweenPoints lines = new LinesBetweenPoints();
        private bool Initialized = false;

        readonly int Diameter = 5;
        readonly double Radius = 2.5;

        KeyDate Left;
        KeyDate Right;

        public void Initialize(Common common)
        {
            this.common = common;
        }

        private Ellipse NewEllipse(string toToolTip)
        {
            return new Ellipse
            {
                Width = Diameter,
                Height = Diameter,
                Fill = Brushes.Coral,
                ToolTip = toToolTip
            };
        }

        public void Add(string[] date, string[] text, Dictionary<string, double> horizontalPositions, Canvas canvas)
        {
            string FullDate = $"{int.Parse(date[0])}.{int.Parse(date[1])}.{date[2]}";
            string ToolTip = $"{date[0]}, {text[0]}.{text[1]}";

            if (!horizontalPositions.ContainsKey(FullDate))
            {
                if (ellipses.ContainsKey(FullDate))
                {
                    ellipses[FullDate].ToolTip = ToolTip;
                }
                else
                {
                    lines.Add(FullDate, Left, Right, canvas);
                    //add line hær også!!!!!!!
                    ellipses.Add(FullDate, NewEllipse(ToolTip));

                    string next = lines.GetNext(FullDate);
                    if (canvas.Children.Contains(ellipses[next]))
                    {
                        canvas.Children.Remove(ellipses[next]);
                        canvas.Children.Add(ellipses[next]);
                    }
                }
            }
            else
            {
                if (ellipses.ContainsKey(FullDate))
                {
                    ellipses[FullDate].ToolTip = ToolTip;
                    SetEllipsePosition(ellipses[FullDate], FullDate, horizontalPositions[FullDate], canvas);
                }
                else
                {
                    ellipses.Add(FullDate, NewEllipse(ToolTip));
                    SetEllipsePosition(ellipses[FullDate], FullDate, horizontalPositions[FullDate], canvas);

                    string next = lines.GetNext(FullDate);
                    if(canvas.Children.Contains(ellipses[next]))
                    {
                        canvas.Children.Remove(ellipses[next]);
                        canvas.Children.Add(ellipses[next]);
                    }

                    canvas.Children.Add(ellipses[FullDate]);
                }
            }

            UpdateFrontAndBack();
        }

        private void SetEllipsePosition(Ellipse ellipse, string key, double position, Canvas canvas)
        {
            string[] text = ellipse.ToolTip.ToString().Split(" ")[1].Split(".");
            double Top = ((common.Height - 2 * common.Offset) / common.CurrentMax) * (common.CurrentMax - double.Parse($"{text[0]},{text[1]}")) + common.FontSize + Radius;
            double Left = position + common.FontSize;

            lines.Add(key, Right, Left, Top + Radius, canvas);

            Canvas.SetTop(ellipse, Top);
            Canvas.SetLeft(ellipse, Left - Radius);
        }

        private void SetEllipsePosition(Ellipse ellipse, string key, double position)
        {
            string[] text = ellipse.ToolTip.ToString().Split(" ")[1].Split(".");
            double Top = ((common.Height - 2 * common.Offset) / common.CurrentMax) * (common.CurrentMax - double.Parse($"{text[0]},{text[1]}")) + common.FontSize + Radius;
            double Left = position + common.FontSize;

            lines.OnSizeChange(key, Left, Top + Radius);

            Canvas.SetTop(ellipse, Top);
            Canvas.SetLeft(ellipse, Left - Radius);
        }


        public void ReDraw(Dictionary<string, double> horizontalPositions, Canvas canvas)
        {
            if(lines.DrawTail(Left, out Line tail))
            {
                canvas.Children.Add(tail);
            }

            foreach (var position in horizontalPositions)
            {
                if(ellipses.ContainsKey(position.Key))
                {
                    canvas.Children.Add(lines.GetLine(position.Key));
                    canvas.Children.Add(ellipses[position.Key]);
                }
            }
        }

        private void ReaplyEllipseToCanvas(string date, bool moveForward, Canvas canvas)
        {
            if (ellipses.ContainsKey(date))
            {
                lines.GetLine(date, moveForward, canvas);
                canvas.Children.Add(ellipses[date]);
            }
        }

        private void EllipseOutOfRange(string date, bool moveForward, Canvas canvas)
        {
            if (ellipses.ContainsKey(date))
            {
                // make lines void. send in canvas and aply head line / tail line ore create them from the start and only change them after .OutOfRange (i like the lather) 
                lines.OutOfRange(date, moveForward, canvas);
                canvas.Children.Remove(ellipses[date]);
            }
        }

        public void OnMove(string[] removeAndAdd, bool moveForward, Dictionary<string, double> horizontalPositions, Canvas canvas)
        {
            EllipseOutOfRange(removeAndAdd[0], moveForward, canvas);
            ReaplyEllipseToCanvas(removeAndAdd[1], moveForward, canvas);

            foreach (var position in horizontalPositions)
            {
                if(ellipses.ContainsKey(position.Key))
                {
                    SetEllipsePosition(ellipses[position.Key], position.Key, horizontalPositions[position.Key]);
                }
            }

            if(moveForward)
                UpdateLeftAndRight(horizontalPositions.Keys.First(), horizontalPositions.Keys.Last());
            else
                UpdateLeftAndRight(horizontalPositions.Keys.Last(), horizontalPositions.Keys.First());

            UpdateFrontAndBack();
        }

        public void OnSizeChange(Dictionary<string, double> horizontalPositions)
        {
            foreach (var position in horizontalPositions)
            {
                if (ellipses.ContainsKey(position.Key))
                {
                    SetEllipsePosition(ellipses[position.Key], position.Key, horizontalPositions[position.Key]);
                }
            }
            if(!Initialized)
            {
                Left = new KeyDate(horizontalPositions.Keys.First());
                Right = new KeyDate(horizontalPositions.Keys.Last());

                Initialized = true;
            }

            UpdateFrontAndBack();
        }

        private void UpdateLeftAndRight(string left, string right)
        {
            Left.SetFullDate(left);
            Right.SetFullDate(right);
        }

        private double TopLine(string key)
        {
            string[] text = ellipses[key].ToolTip.ToString().Split(" ")[1].Split(".");
            return ((common.Height - 2 * common.Offset) / common.CurrentMax) * (common.CurrentMax - double.Parse($"{text[0]},{text[1]}")) + common.FontSize + 2 * Radius;
        }

        private void UpdateFrontAndBack()
        {
            if (lines.HeadNeedUpdate(out string key, Right))
            {
                lines.UpdateHead(TopLine(key), ((common.Width - 2 * common.Offset) / common.HorizontalElements), common.Offset, Left, Right);
            }

            if (lines.TailNeedUpdate(out key, Left))
            {
                lines.UpdateTail(TopLine(key), ((common.Width - 2 * common.Offset) / common.HorizontalElements), common.Offset);
            }
        }
    }
}
