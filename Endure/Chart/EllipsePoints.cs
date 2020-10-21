using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Endure
{
    public class LinesBetweenPoints
    {
        readonly LinkedMap<KeyDate, Line> lines = new LinkedMap<KeyDate, Line>();

        readonly int StrokeThickness = 1;

        bool Initialized = false;

        public void Add(string key, double x, double y, Canvas canvas)
        {
            KeyDate Key = new KeyDate(key);

            if(lines.ContainsKey(Key))
            {
                lines[Key].X1 = x;
                lines[Key].Y1 = y;

                if (Key == lines.Head)
                {
                    lines[Key].X2 = x;
                    lines[Key].Y2 = y;
                }

                if (!lines.IsTail(Key))
                {
                    lines[lines.GetPrevius(Key)].X2 = x;
                    lines[lines.GetPrevius(Key)].Y2 = y;
                }
            }
            else
            {
                Line line = new Line
                {
                    StrokeThickness = this.StrokeThickness,
                    Stroke = Brushes.Blue,

                    X1 = x,
                    Y1 = y,
                    X2 = x,
                    Y2 = y
                };

                if(lines.Initialized)
                {
                    if (Key < lines.Tail)
                    {
                        lines.InsertTail(Key, line);

                        lines[Key].X2 = lines[lines.GetNext(Key)].X1;
                        lines[Key].Y2 = lines[lines.GetNext(Key)].Y1;
                    }
                    else if (Key > lines.Head)
                    {
                        lines.InsertHead(Key, line);
                    }
                    else
                    {
                        KeyDate next = lines.Tail;
                        while (next != null)
                        {
                            if (Key > next)
                            {
                                if (Key < lines.GetNext(next))
                                {
                                    lines.InsertFront(next, Key, line);

                                    lines[Key].X2 = lines[lines.GetNext(Key)].X1;
                                    lines[Key].Y2 = lines[lines.GetNext(Key)].Y1;

                                    break;
                                }
                            }

                            next = lines.GetNext(next);
                        }
                    }

                    if(!lines.IsTail(Key))
                    {
                        lines[lines.GetPrevius(Key)].X2 = x;
                        lines[lines.GetPrevius(Key)].Y2 = y;
                    }
                }
                else
                {
                    lines.Initialize(Key, line);
                    Initialized = true;
                }

                if (!canvas.Children.Contains(lines[Key]))
                {
                    canvas.Children.Add(lines[Key]);
                }   
            }
        }

        public void OnSizeChange(string key, double x, double y)
        {
            if(Initialized)
            {
                KeyDate Key = new KeyDate(key);

                if(lines.ContainsKey(Key))
                {
                    lines[Key].X1 = x;
                    lines[Key].Y1 = y;
                    if(Key == lines.Head)
                    {
                        lines[Key].X2 = x;
                        lines[Key].Y2 = y;
                    }

                    if (!lines.IsTail(Key))
                    {
                        lines[lines.GetPrevius(Key)].X2 = x;
                        lines[lines.GetPrevius(Key)].Y2 = y;
                    }
                }
            }
        }

        public Line GetLine(string key)
        {
            KeyDate Key = new KeyDate(key);

            //if(lines.ContainsKey(Key))
                return lines[Key];

            //return null;
        }

        public Line OutOfRange(string key)
        {
            KeyDate Key = new KeyDate(key);

            //if(lines.ContainsKey(Key))
            return lines[Key];

            //return null;
        }
    }

    public class EllipsePoints
    {
        Common common;
        readonly Dictionary<string, Ellipse> ellipses = new Dictionary<string, Ellipse>();
        readonly LinesBetweenPoints lines = new LinesBetweenPoints();

        readonly int Diameter = 5;
        readonly double Radius = 2.5;

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
                    //add line hær også!!!!!!!
                    ellipses.Add(FullDate, NewEllipse(ToolTip));
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

                    canvas.Children.Add(ellipses[FullDate]);
                }
            }   
        }

        private void SetEllipsePosition(Ellipse ellipse, string key, double position, Canvas canvas)
        {
            string[] text = ellipse.ToolTip.ToString().Split(" ")[1].Split(".");
            double Top = ((common.Height - 2 * common.Offset) / common.CurrentMax) * (common.CurrentMax - double.Parse($"{text[0]},{text[1]}")) + common.FontSize + Radius;
            double Left = position + common.FontSize;

            lines.Add(key, Left, Top + Radius, canvas);

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
            foreach(var position in horizontalPositions)
            {
                if(ellipses.ContainsKey(position.Key))
                {
                    canvas.Children.Add(lines.GetLine(position.Key));
                    canvas.Children.Add(ellipses[position.Key]);
                }
            }
        }

        private void ReaplyEllipseToCanvas(string date, Canvas canvas)
        {
            if (ellipses.ContainsKey(date))
            {
                canvas.Children.Add(lines.GetLine(date));
                canvas.Children.Add(ellipses[date]);
            }
        }

        private void EllipseOutOfRange(string date, Canvas canvas)
        {
            if (ellipses.ContainsKey(date))
            {
                canvas.Children.Remove(lines.OutOfRange(date));
                canvas.Children.Remove(ellipses[date]);
            }
        }

        public void OnMove(string[] removeAndAdd, Dictionary<string, double> horizontalPositions, Canvas canvas)
        {
            EllipseOutOfRange(removeAndAdd[0], canvas);
            ReaplyEllipseToCanvas(removeAndAdd[1], canvas);

            foreach(var position in horizontalPositions)
            {
                if(ellipses.ContainsKey(position.Key))
                {
                    SetEllipsePosition(ellipses[position.Key], position.Key, horizontalPositions[position.Key]);
                }
            }
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
        }
    }
}
