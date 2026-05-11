using System;

namespace Raytracer
{
    public abstract class Material
    {
        public abstract bool Scatter(Ray rIn, HitRecord rec, out Color attenuation, out Ray scattered);
    }

    public class Lambertian : Material
    {
        private Color albedo;

        public Lambertian(Color a)
        {
            albedo = a;
        }

        public override bool Scatter(Ray rIn, HitRecord rec, out Color attenuation, out Ray scattered)
        {
            Vec3 scatterDirection = rec.Normal + Vec3.RandomUnitVector();

            if (scatterDirection.NearZero())
            {
                scatterDirection = rec.Normal;
            }

            scattered = new Ray(rec.P, scatterDirection);
            attenuation = albedo;
            return true;
        }
    }

    public class Metal : Material
    {
        private Color albedo;
        private double fuzz;

        public Metal(Color a, double f)
        {
            albedo = a;
            fuzz = f < 1 ? f : 1;
        }

        public override bool Scatter(Ray rIn, HitRecord rec, out Color attenuation, out Ray scattered)
        {
            Vec3 reflected = Vec3.Reflect(Vec3.UnitVector(rIn.Direction), rec.Normal);
            scattered = new Ray(rec.P, reflected + fuzz * Vec3.RandomUnitVector());
            attenuation = albedo;
            return Vec3.Dot(scattered.Direction, rec.Normal) > 0;
        }
    }

    public class Dielectric : Material
    {
        private double ir; // Index of Refraction

        public Dielectric(double indexOfRefraction)
        {
            ir = indexOfRefraction;
        }

        public override bool Scatter(Ray rIn, HitRecord rec, out Color attenuation, out Ray scattered)
        {
            Vec3 unitDirection = Vec3.UnitVector(rIn.Direction);
            double refractionRatio = rec.FrontFace ? (1.0 / ir) : ir;
            double cosTheta = Math.Min(Vec3.Dot(-unitDirection, rec.Normal), 1.0);
            double sinTheta = Math.Sqrt(1.0 - cosTheta * cosTheta);

            bool cannotRefract = refractionRatio * sinTheta > 1.0;
            Vec3 direction;

            if (cannotRefract || Reflectance(cosTheta, refractionRatio) > Utility.RandomDouble())
            {
                direction = Vec3.Reflect(unitDirection, rec.Normal);
            }
            else
            {
                direction = Vec3.Refract(unitDirection, rec.Normal, refractionRatio);
            }

            scattered = new Ray(rec.P, direction);
            attenuation = new Color(1, 1, 1);
            return true;
        }

        private static double Reflectance(double cosine, double refractionIndex)
        {
            // Use Schlick's approximation for reflectance.
            double r0 = (1 - refractionIndex) / (1 + refractionIndex);
            r0 = r0 * r0;
            return r0 + (1 - r0) * Math.Pow(1 - cosine, 5);
        }
    }
}