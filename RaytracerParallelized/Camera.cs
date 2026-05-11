using System;
using System.IO;

namespace Raytracer
{
    public class Camera
    {
        public double AspectRatio { get; set; } = 1.0;
        public int ImageWidth { get; set; } = 100;
        public int MaxDepth { get; set; } = 10;
        public double VFov { get; set; } = 90;
        public Point3 LookFrom { get; set; } = new Point3(0, 0, 0);
        public Point3 LookAt { get; set; } = new Point3(0, 0, -1);
        public Vec3 VUp { get; set; } = new Vec3(0, 1, 0);
        public double DefocusAngle { get; set; } = 0;
        public double FocusDist { get; set; } = 10;
        public int SamplesPerPixel { get; set; } = 10;

        private int imageHeight;
        private Point3 center;
        private Point3 pixel00Loc;
        private Vec3 pixelDeltaU;
        private Vec3 pixelDeltaV;
        private Vec3 u, v, w;
        private Vec3 defocusDiskU;
        private Vec3 defocusDiskV;

        public void Render(IHittable world, string outputFile, ParallelOptions parallelOptions)
        {
            Initialize();

            // Image buffer to store pixels in any order
            Color[,] imageBuffer = new Color[ImageWidth, imageHeight];

            Parallel.For(0, imageHeight, parallelOptions, j =>
            {
                // Each thread process one row at a time
                for (int i = 0; i < ImageWidth; i++)
                {
                    Color pixelColor = new Color(0, 0, 0);

                    for (int sample = 0; sample < SamplesPerPixel; sample++)
                    {
                        Vec3 offset = new Vec3(0, 0, 0);
                        Vec3 pixelCenter = pixel00Loc + ((i + offset.X()) * pixelDeltaU) + ((j + offset.Y()) * pixelDeltaV);

                        Point3 rayOrigin = (DefocusAngle <= 0) ? center : DefocusDiskSample();
                        Vec3 rayDirection = pixelCenter - rayOrigin;
                        Ray r = new Ray(rayOrigin, rayDirection);

                        Vec3 sampleColor = RayColor(r, MaxDepth, world);
                        pixelColor = new Color(
                            pixelColor.X() + sampleColor.X(),
                            pixelColor.Y() + sampleColor.Y(),
                            pixelColor.Z() + sampleColor.Z()
                        );
                    }

                    Vec3 finalColor = pixelColor / SamplesPerPixel;
                    imageBuffer[i, j] = new Color(finalColor.X(), finalColor.Y(), finalColor.Z());
                }
            });

            // Write buffered pixels to PPM file sequentially
            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                writer.WriteLine($"P3\n{ImageWidth} {imageHeight}\n255");

                for (int j = 0; j < imageHeight; j++)
                {
                    for (int i = 0; i < ImageWidth; i++)
                    {
                        ColorUtility.WriteColor(writer, imageBuffer[i, j]);
                    }
                }
            }
        }

        private void Initialize()
        {
            imageHeight = (int)(ImageWidth / AspectRatio);
            imageHeight = (imageHeight < 1) ? 1 : imageHeight;

            center = LookFrom;

            // Determine viewport dimensions
            double theta = Utility.DegreesToRadians(VFov);
            double h = Math.Tan(theta / 2);
            double viewportHeight = 2 * h * FocusDist;
            double viewportWidth = viewportHeight * ((double)ImageWidth / imageHeight);

            // Calculate the u,v,w unit basis vectors for the camera coordinate frame
            w = Vec3.UnitVector(LookFrom - LookAt);
            u = Vec3.UnitVector(Vec3.Cross(VUp, w));
            v = Vec3.Cross(w, u);

            // Calculate the vectors across the horizontal and down the vertical viewport edges
            Vec3 viewportU = viewportWidth * u;
            Vec3 viewportV = viewportHeight * -v;

            // Calculate the horizontal and vertical delta vectors from pixel to pixel
            pixelDeltaU = viewportU / ImageWidth;
            pixelDeltaV = viewportV / imageHeight;

            // Calculate the location of the upper left pixel
            Vec3 viewportUpperLeft = center - (FocusDist * w) - viewportU / 2 - viewportV / 2;
            pixel00Loc = new Point3(
                viewportUpperLeft.X() + 0.5 * (pixelDeltaU.X() + pixelDeltaV.X()),
                viewportUpperLeft.Y() + 0.5 * (pixelDeltaU.Y() + pixelDeltaV.Y()),
                viewportUpperLeft.Z() + 0.5 * (pixelDeltaU.Z() + pixelDeltaV.Z())
            );

            // Calculate the camera defocus disk basis vectors
            double defocusRadius = FocusDist * Math.Tan(Utility.DegreesToRadians(DefocusAngle / 2));
            defocusDiskU = u * defocusRadius;
            defocusDiskV = v * defocusRadius;
        }

        private Point3 DefocusDiskSample()
        {
            Vec3 p = Vec3.RandomInUnitDisk();
            Vec3 sample = center + (p[0] * defocusDiskU) + (p[1] * defocusDiskV);
            return new Point3(sample.X(), sample.Y(), sample.Z());
        }

        private Color RayColor(Ray r, int depth, IHittable world)
        {
            HitRecord rec = new HitRecord();

            if (depth == 0)
            {
                return new Color(0, 0, 0);
            }

            if (world.Hit(r, new Interval(0.000001, Utility.Infinity), rec))
            {
                Ray scattered;
                Color attenuation;
                if (rec.Mat.Scatter(r, rec, out attenuation, out scattered))
                {
                    Vec3 recursiveColor = RayColor(scattered, depth - 1, world);
                    return new Color(
                        attenuation.X() * recursiveColor.X(),
                        attenuation.Y() * recursiveColor.Y(),
                        attenuation.Z() * recursiveColor.Z()
                    );
                }
                else
                {
                    return new Color(0, 0, 0);
                }
            }

            Vec3 unitDirection = Vec3.UnitVector(r.Direction);
            double a = 0.5 * (unitDirection.Y() + 1.0);
            Vec3 blended = (1.0 - a) * new Color(1.0, 1.0, 1.0) + a * new Color(0.5, 0.7, 1.0);
            return new Color(blended.X(), blended.Y(), blended.Z());
        }
    }
}
