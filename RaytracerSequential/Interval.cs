using System;

namespace Raytracer
{
    public class Interval
    {
        public double Min { get; set; }
        public double Max { get; set; }

        public Interval()
        {
            Min = Utility.Infinity;
            Max = -Utility.Infinity;
        }

        public Interval(double min, double max)
        {
            Min = min;
            Max = max;
        }

        public bool Contains(double x)
        {
            return Min <= x && x <= Max;
        }

        public bool Surrounds(double x)
        {
            return Min < x && x < Max;
        }

        public double Clamp(double x)
        {
            if (x < Min) return Min;
            if (x > Max) return Max;
            return x;
        }

        public static readonly Interval Empty = new Interval(Utility.Infinity, -Utility.Infinity);
        public static readonly Interval Universe = new Interval(-Utility.Infinity, Utility.Infinity);
    }
}