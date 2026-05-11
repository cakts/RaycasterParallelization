using System;

namespace Raytracer
{
    public static class Utility
    {
        public const double Infinity = double.PositiveInfinity;
        public const double Pi = 3.1415926535897932385;

        private static Random random = new Random();

        public static double RandomDouble()
        {
            return random.NextDouble();
        }

        public static double RandomDouble(double min, double max)
        {
            return min + (max - min) * RandomDouble();
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees * Pi / 180.0;
        }
    }
}