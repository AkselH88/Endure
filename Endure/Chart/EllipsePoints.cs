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

        public int CurrentMax { get; private set; }
        public int CurrentMin { get; private set; }
        public bool Update { get; private set; }
        public bool changeMax { get; private set; }
        public bool changeMin { get; private set; }

        readonly int Diameter = 5;
        readonly double Radius = 2.5;

        KeyDate Left;
        KeyDate Right;

        public void Initialize(Common common)
        {
            this.common = common;
            CurrentMax = common.CurrentMax;
            CurrentMin = common.CurrentMin;
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

        public void Add(string[] date, string[] text, List<string> horizontalText, List<double> horizontalPositions, Canvas canvas)
        {
            string FullDate = $"{int.Parse(date[0])}.{int.Parse(date[1])}.{date[2]}";
            string ToolTip = $"{date[0]}, {text[0]}.{text[1]}";

            if (!Initialized)
            {
                Left = new KeyDate(horizontalText.First());
                Right = new KeyDate(horizontalText.Last());
            }

            if (!horizontalText.Contains(FullDate))
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
                    SetEllipsePosition(ellipses[FullDate], FullDate, horizontalPositions[horizontalText.IndexOf(FullDate)], canvas);
                }
                else
                {
                    ellipses.Add(FullDate, NewEllipse(ToolTip));
                    SetEllipsePosition(ellipses[FullDate], FullDate, horizontalPositions[horizontalText.IndexOf(FullDate)], canvas);

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
            NeedSizeUpdate();

            if(!Initialized)
            {
                Initialized = true;
            }
        }

        private void SetEllipsePosition(Ellipse ellipse, string key, double position, Canvas canvas)
        {
            string[] text = ellipse.ToolTip.ToString().Split(" ")[1].Split(".");
            double Top = ((common.Height - 2 * common.Offset) / (common.CurrentMax - common.CurrentMin)) * (common.CurrentMax - double.Parse($"{text[0]},{text[1]}")) + common.FontSize + Radius;
            double Left = position + common.FontSize;

            lines.Add(key, Right, Left, Top + Radius, canvas);

            Canvas.SetTop(ellipse, Top);
            Canvas.SetLeft(ellipse, Left - Radius);
        }

        private void SetEllipsePosition(Ellipse ellipse, string key, double position)
        {
            string[] text = ellipse.ToolTip.ToString().Split(" ")[1].Split(".");
            double Top = ((common.Height - 2 * common.Offset) / (common.CurrentMax - common.CurrentMin)) * (common.CurrentMax - double.Parse($"{text[0]},{text[1]}")) + common.FontSize + Radius;
            double Left = position + common.FontSize;

            lines.OnSizeChange(key, Left, Top + Radius);

            Canvas.SetTop(ellipse, Top);
            Canvas.SetLeft(ellipse, Left - Radius);
        }


        public void ReDraw(List<string> horizontalText, Canvas canvas)
        {
            if (Initialized)
            {
                if (lines.DrawTail(Left, out Line tail))
                {
                    canvas.Children.Add(tail);
                }

                foreach (var position in horizontalText)
                {
                    if (ellipses.ContainsKey(position))
                    {
                        canvas.Children.Add(lines.GetLine(position));
                        canvas.Children.Add(ellipses[position]);
                    }
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

        public void OnMove(string[] removeAndAdd, bool moveForward, List<string> horizontalText, List<double> horizontalPositions, Canvas canvas)
        {
            if (Initialized)
            {
                EllipseOutOfRange(removeAndAdd[0], moveForward, canvas);
                ReaplyEllipseToCanvas(removeAndAdd[1], moveForward, canvas);

                foreach (var position in horizontalText)
                {
                    if (ellipses.ContainsKey(position))
                    {
                        SetEllipsePosition(ellipses[position], position, horizontalPositions[horizontalText.IndexOf(position)]);
                    }
                }

                UpdateLeftAndRight(horizontalText.First(), horizontalText.Last());

                UpdateFrontAndBack();
                NeedSizeUpdate();
            }
        }

        public void OnSizeChange(List<string> horizontalText, List<double> horizontalPositions)
        {
            if (Initialized)
            {
                foreach (var position in horizontalText)
                {
                    if (ellipses.ContainsKey(position))
                    {
                        SetEllipsePosition(ellipses[position], position, horizontalPositions[horizontalText.IndexOf(position)]);
                    }
                }

                UpdateFrontAndBack();
            }
        }

        private void UpdateLeftAndRight(string left, string right)
        {
            Left.SetFullDate(left);
            Right.SetFullDate(right);
        }

        private double TopLine(string key)
        {
            string[] text = ellipses[key].ToolTip.ToString().Split(" ")[1].Split(".");
            return ((common.Height - 2 * common.Offset) / (common.CurrentMax - common.CurrentMin)) * (common.CurrentMax - double.Parse($"{text[0]},{text[1]}")) + common.FontSize + 2 * Radius;
        }

        private void UpdateFrontAndBack()
        {
            if (lines.HeadNeedUpdate(out string key, Right))
            {
                lines.UpdateHead(TopLine(key), ((common.Width - (common.VerticalStartPos + common.Offset)) / common.HorizontalElements), common.VerticalStartPos, Left, Right);
            }

            if (lines.TailNeedUpdate(out key, Left))
            {
                lines.UpdateTail(TopLine(key), ((common.Width - (common.VerticalStartPos + common.Offset)) / common.HorizontalElements), common.VerticalStartPos);
            }
        }

        private void RevertSizeUpdateBoolians()
        {
            Update = false;
            changeMax = false;
            changeMin = false;
        }
        private void NeedSizeUpdate()
        {
            KeyDate i = lines.TailKey;
            RevertSizeUpdateBoolians();
            int max = 0;
            int min = CurrentMax;
            bool change = false;

            if (lines.TailKey == lines.HeadKey && lines.HeadKey <= Left && lines.HeadKey == lines.GetNext(lines.HeadKey) ||
                lines.TailKey == lines.HeadKey && lines.HeadKey == lines.GetNext(lines.HeadKey) ||
                lines.TailKey == lines.HeadKey && lines.TailKey >= Right)
            {
                max = min = int.Parse(ellipses[i.ToString()].ToolTip.ToString().Split(" ")[1].Split(".")[0]);
                change = true;
            }
            else if(lines.TailKey != lines.HeadKey || lines.HeadKey != lines.GetNext(lines.HeadKey))
            {
                KeyDate temp = i;
                while (i <= lines.GetNext(lines.HeadKey))
                {
                    int ellipseValue = int.Parse(ellipses[i.ToString()].ToolTip.ToString().Split(" ")[1].Split(".")[0]);
                    if (max < ellipseValue)
                    {
                        max = ellipseValue;
                    }
                    if (min > ellipseValue)
                    {
                        min = ellipseValue;
                    }

                    i = lines.GetNext(i);
                    if (temp == i)
                        break;
                    temp = i;
                }

                change = true;
            }

            if (change)
            {
                int tempMax = RetriveNewNum(max, 1);
                if (tempMax != CurrentMax)
                {
                    if (tempMax < 10)
                        CurrentMax = 10;
                    else
                        CurrentMax = tempMax;

                    changeMax = true;
                }

                int tempMin = RetriveNewNum(min, -1);
                if (tempMin != CurrentMin)
                {
                    if (tempMin < 0)
                        CurrentMin = 0;
                    else
                        CurrentMin = tempMin;

                    changeMin = true;
                }
            }

            Update = changeMax || changeMin;
        }

        private int RetriveNewNum(int current, int direction)
        {
            int newNum = 0;

            string stringNum = $"{current}";
            int multiplier = (int)Math.Pow(10, stringNum.Length - 1);

            if (stringNum.Length == 1)
            {
                if (direction < 0)
                    return newNum;
                else if (direction > 0)
                    return multiplier;
            }
            else
            {
                for (int i = 0; i <= stringNum.Length - 2; i++)
                {
                    if (i != stringNum.Length - 2)
                        newNum += int.Parse($"{stringNum.ElementAt(i)}") * multiplier;
                    else
                        newNum += (int.Parse($"{stringNum.ElementAt(i)}") + direction) * multiplier;

                    multiplier /= 10;
                }
            }

            if (direction < 0)
                if(newNum + 10 < current)
                    newNum += 10;
            else if (direction > 0)
                if(newNum - 10 > current)
                    newNum -= 10;

            return newNum;
        }
    }
}
