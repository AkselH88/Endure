using System;
using System.Collections.Generic;
using System.Text;

namespace Endure
{
    public class KeyDate
    {
        public KeyDate() { }
        public KeyDate(int Day, int Month, int Year)
        {
            this.Day = Day;
            this.Month = Month;
            this.Year = Year;
        }
        public KeyDate(string DDMMYYYY)
        {
            string[] date = DDMMYYYY.Split(".");
            if (date.Length == 3)
            {
                this.Day = int.Parse(date[0]);
                this.Month = int.Parse(date[1]);
                this.Year = int.Parse(date[2]);
            }
        }

        public override int GetHashCode() { return $"{Day}.{Month}.{Year}".GetHashCode(); }

        public int Day { set; get; }
        public int Month { set; get; }
        public int Year { set; get; }

        public void SetFullDate(string DDMMYYYY)
        {
            string[] date = DDMMYYYY.Split(".");
            if (date.Length == 3)
            {
                this.Day = int.Parse(date[0]);
                this.Month = int.Parse(date[1]);
                this.Year = int.Parse(date[2]);
            }
        }
        public override bool Equals(object obj)
        {
            if (this as object == null && obj == null)
                return true;
            else if (obj == null)
                return false;
            else if (this as object == null)
                return false;

            KeyDate Obj = obj as KeyDate;

            return (Day == Obj.Day && Month == Obj.Month && Year == Obj.Year);
        }

        public static bool operator >(KeyDate a, KeyDate b) => /*(object)a != null && (object)b != null &&*/ a.MoreThan(b);
        public static bool operator <(KeyDate a, KeyDate b) => /*(object)a != null && (object)b != null &&*/ a.LessThan(b);
        public static bool operator >=(KeyDate a, KeyDate b) => /*(object)a != null && (object)b != null &&*/ (a.MoreThan(b) || a.Equals(b));
        public static bool operator <=(KeyDate a, KeyDate b) => /*(object)a != null && (object)b != null &&*/ (a.LessThan(b) || a.Equals(b));
        public static bool operator ==(KeyDate a, KeyDate b) => /*(object)a != null && (object)b != null &&*/ a.Equals(b);
        public static bool operator !=(KeyDate a, KeyDate b) => ((object)a != (object)b && !a.Equals((object)b));

        private bool MoreThan(KeyDate toCompere)
        {
            if (Year == toCompere.Year)
            {
                if (Month == toCompere.Month)
                {
                    if (Day > toCompere.Day)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (Month > toCompere.Month)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (Year > toCompere.Year)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool LessThan(KeyDate toCompere)
        {
            if (Year == toCompere.Year)
            {
                if (Month == toCompere.Month)
                {
                    if (Day < toCompere.Day)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (Month < toCompere.Month)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (Year < toCompere.Year)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string ToString() { return $"{Day}.{Month}.{Year}"; }
    }
}
