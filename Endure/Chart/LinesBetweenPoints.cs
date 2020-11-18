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
        // Head and Tail Key is only relevent for whats on desplay.
        public KeyDate HeadKey { get; private set; }
        public KeyDate TailKey { get; private set; }

        bool Initialized = false;

        public void Add(string key, KeyDate left, KeyDate right, Canvas canvas)
        {
            KeyDate Key = new KeyDate(key);

            if (!lines.ContainsKey(Key))
            {
                Line line = new Line
                {
                    StrokeThickness = this.StrokeThickness,
                    Stroke = Brushes.Blue
                };

                if (lines.Initialized)
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
                        while(!FindPoint(Key, next, line))
                        {
                            next = lines.GetNext(next);
                        }
                    }

                    if (Key < left)
                    {
                        if (Key > TailKey)
                        {
                            canvas.Children.Remove(lines[TailKey]);
                            TailKey = Key;
                            canvas.Children.Add(lines[TailKey]);
                        }
                        else if (left < TailKey)
                        {
                            TailKey = Key;
                            canvas.Children.Add(lines[TailKey]);
                        }
                        if (Key > HeadKey)
                        {
                            HeadKey = Key;
                        }
                    }
                    else if(Key > right)
                    {
                        if(Key < TailKey)
                        {
                            TailKey = Key;
                        }
                        if(Key < HeadKey)
                        {
                            HeadKey = Key;
                        }
                    }

                }
                else
                {
                    lines.Initialize(Key, line);
                    TailKey = HeadKey = Key;
                    Initialized = true;

                    if(Key < left)
                        canvas.Children.Add(lines[Key]);
                }
            }

        }

        private bool FindPoint(KeyDate key, KeyDate next, Line line)
        {
            if (key > next)
            {
                if (key < lines.GetNext(next))
                {
                    lines.InsertFront(next, key, line);

                    lines[key].X2 = lines[lines.GetNext(key)].X1;
                    lines[key].Y2 = lines[lines.GetNext(key)].Y1;

                    return true;
                }
            }

            return false;
        }

        public string GetNext(string key)
        {
            KeyDate Key = new KeyDate(key);

            if (!lines.IsHead(Key))
                return lines.GetNext(Key).ToString();
            else
                return key;
        }

        public KeyDate GetNext(KeyDate key)
        {
            if (!lines.IsHead(key))
                return lines.GetNext(key);
            else
                return key;
        }

        public void Add(string key, KeyDate right, double x, double y, Canvas canvas)
        {
            KeyDate Key = new KeyDate(key);

            if (!OnSizeChange(Key, x, y))
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

                if (lines.Initialized)
                {
                    if (Key < lines.Tail)
                    {
                        lines.InsertTail(Key, line);
                        TailKey = Key;

                        lines[Key].X2 = lines[lines.GetNext(Key)].X1;
                        lines[Key].Y2 = lines[lines.GetNext(Key)].Y1;
                    }
                    else if (Key > lines.Head)
                    {
                        lines.InsertHead(Key, line);
                        HeadKey = Key;
                    }
                    else
                    {
                        KeyDate next = lines.Tail;
                        while (!FindPoint(Key, next, line))
                        {
                            next = lines.GetNext(next);
                        }

                        if (Key > HeadKey)
                        {
                            HeadKey = Key;
                        }
                    }

                    if(right < HeadKey)
                    {
                        if(canvas.Children.Contains(lines[HeadKey]))
                        {
                            canvas.Children.Remove(lines[HeadKey]);
                        }
                        HeadKey = Key;
                    }

                    if (!lines.IsTail(Key))
                    {
                        lines[lines.GetPrevius(Key)].X2 = x;
                        lines[lines.GetPrevius(Key)].Y2 = y;
                    }
                }
                else
                {
                    lines.Initialize(Key, line);
                    TailKey = HeadKey = Key;

                    Initialized = true;
                }

                if (!canvas.Children.Contains(lines[Key]))
                {
                    canvas.Children.Add(lines[Key]);
                }
            }
        }

        public bool OnSizeChange(KeyDate key, double x, double y)
        {
            if (lines.ContainsKey(key))
            {
                lines[key].X1 = x;
                lines[key].Y1 = y;
                if (key == lines.Head)
                {
                    lines[key].X2 = x;
                    lines[key].Y2 = y;
                }

                if (!lines.IsTail(key))
                {
                    lines[lines.GetPrevius(key)].X2 = x;
                    lines[lines.GetPrevius(key)].Y2 = y;
                }

                return true;
            }
            else
                return false;

        }

        public bool DrawTail(KeyDate left, out Line tail)
        {
            if(TailKey != null && TailKey < left)
            {
                tail = lines[TailKey];
                return true;
            }
            tail = null;
            return false;
        }

        public void OnSizeChange(string key, double x, double y)
        {
            if (Initialized)
            {
                KeyDate Key = new KeyDate(key);

                if (lines.ContainsKey(Key))
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
            }
        }

        public bool TailNeedUpdate(out string key, KeyDate left)
        {
            key = string.Empty;
            if (Initialized)
            {
                if(lines.Tail < left && lines.Head >= left)
                {
                    key = TailKey.ToString();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// x0 is the x offset from origo
        /// </summary>
        public void UpdateTail(double y, double pxShifted, double x0)
        {
            double P = (lines[TailKey].X2 - x0) / (DistanseX(TailKey, lines.GetNext(TailKey)) * pxShifted);
            lines[TailKey].X1 = lines[TailKey].X2 - DistanseX(TailKey, lines.GetNext(TailKey)) * pxShifted * P;

            if(lines[TailKey].Y2 > y)
            {
                double y0 = lines[TailKey].Y2 - y;
                lines[TailKey].Y1 = lines[TailKey].Y2 - y0 * P;
            }
            else if(lines[TailKey].Y2 < y)
            {
                double y0 =  y - lines[TailKey].Y2;
                lines[TailKey].Y1 = lines[TailKey].Y2 + y0 * P;
            }
            else
            {
                lines[TailKey].Y1 = lines[TailKey].Y2;
            }
        }

        public bool HeadNeedUpdate(out string key, KeyDate right)
        {
            key = string.Empty;
            if (Initialized)
            {
                if(lines.Head > right && lines.Tail <= right)
                {
                    if(lines.IsHead(HeadKey) || HeadKey > right)
                    {
                        key = HeadKey.ToString();
                    }
                    else
                    {
                        key = lines.GetNext(HeadKey).ToString();
                    }
                    
                    return true;
                }
            }
            return false;
        }
        public void UpdateHead(double y, double pxShifted, double x0, KeyDate left, KeyDate right)
        {
            if (!lines.IsHead(HeadKey))
            {
                if (HeadKey < left)
                {
                    lines[HeadKey].X2 = x0 + DistanseX(left, lines.GetNext(HeadKey)) * pxShifted;
                }
                else if(HeadKey > right)
                {
                    lines[lines.GetPrevius(HeadKey)].X2 = x0 + DistanseX(left, HeadKey) * pxShifted;
                    lines[lines.GetPrevius(HeadKey)].Y2 = y;
                }
                else
                {
                    lines[HeadKey].X2 = lines[HeadKey].X1 + DistanseX(HeadKey, lines.GetNext(HeadKey)) * pxShifted;
                }
                    
                lines[HeadKey].Y2 = y;
            }
            else
            {
                lines[lines.GetPrevius(HeadKey)].X2 = x0 + DistanseX(left, HeadKey) * pxShifted;
                lines[lines.GetPrevius(HeadKey)].Y2 = y;
            }
            
        }

        private int DistanseX(KeyDate date1, KeyDate date2)
        {
            DateTime dt1 = new DateTime(date1.Year, date1.Month, date1.Day);
            DateTime dt2 = new DateTime(date2.Year, date2.Month, date2.Day);

            return (dt2 - dt1).Days;
        }

        public Line GetLine(string key)
        {
            KeyDate Key = new KeyDate(key);

            return lines[Key];
        }

        public void GetLine(string key, bool moveForward, Canvas canvas)
        {
            KeyDate Key = new KeyDate(key);

            if (moveForward)
            {
                HeadKey = Key;
                if (!canvas.Children.Contains(lines[Key]))
                {
                    canvas.Children.Add(lines[Key]);
                }
            }
            else
            {
                if (Key != lines.Tail)
                {
                    TailKey = lines.GetPrevius(Key);
                    canvas.Children.Add(lines[TailKey]);
                }
                else
                {
                    TailKey = Key;
                }
            }
        }

        public void OutOfRange(string key, bool moveForward, Canvas canvas)
        {
            KeyDate Key = new KeyDate(key);

            if (moveForward)
            {
                TailKey = Key;
                if (!lines.IsTail(Key))
                {
                    canvas.Children.Remove(lines[lines.GetPrevius(Key)]);
                }
            }
            else
            {
                if (lines.IsTail(Key))
                {
                    HeadKey = Key;
                }
                else
                {
                    HeadKey = lines.GetPrevius(Key);
                }

                canvas.Children.Remove(lines[Key]);
            }
        }
    }
}
