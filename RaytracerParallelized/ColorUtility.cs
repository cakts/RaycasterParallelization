using System;
using System.IO;

namespace Raytracer
{
    public static class ColorUtility
    {
        private static double LinearToGamma(double linearComponent)
        {
            return Math.Sqrt(linearComponent);
        }

        public static void WriteColor(StreamWriter writer, Color pixelColor)
        {
            Interval intensity = new Interval(0.000, 0.999);

            int r = (int)(256 * intensity.Clamp(LinearToGamma(pixelColor.X())));
            int g = (int)(256 * intensity.Clamp(LinearToGamma(pixelColor.Y())));
            int b = (int)(256 * intensity.Clamp(LinearToGamma(pixelColor.Z())));

            writer.WriteLine($"{r} {g} {b}");
        }
    }
}