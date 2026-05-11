using System;

namespace Raytracer
{
    public static class Utility
    {
        public const double Infinity = double.PositiveInfinity;
        public const double Pi = 3.1415926535897932385;

        // Thread-local Random instance to avoid race conditions in parallel execution
        [ThreadStatic]
        private static Random? random;

        private static Random GetRandom()
        {
            if (random == null)
            {
                int seed = Environment.CurrentManagedThreadId ^ (int)DateTime.Now.Ticks;
                random = new Random(seed);
            }
            return random;
        }

        public static double RandomDouble()
        {
            return GetRandom().NextDouble();
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

