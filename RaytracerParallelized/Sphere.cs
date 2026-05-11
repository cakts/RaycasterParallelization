using System;

namespace Raytracer
{
    public class Sphere : IHittable
    {
        public Point3 Center { get; set; }
        public double Radius { get; set; }
        public Material Mat { get; set; }

        public Sphere()
        {
            Center = new Point3();
            Radius = 0.0;
            Mat = null;
        }

        public Sphere(Point3 center, double radius, Material material)
        {
            Center = center;
            Radius = radius;
            Mat = material;
        }

        public bool Hit(Ray r, Interval rayT, HitRecord rec)
        {
            Vec3 oc = r.Origin - Center;
            double a = r.Direction.LengthSquared();
            double halfB = Vec3.Dot(oc, r.Direction);
            double c = oc.LengthSquared() - Radius * Radius;

            double discriminant = halfB * halfB - a * c;
            if (discriminant < 0)
                return false;

            double sqrtd = Math.Sqrt(discriminant);

            // Find the nearest root that lies in the acceptable range.
            double root = (-halfB - sqrtd) / a;
            if (!rayT.Surrounds(root))
            {
                root = (-halfB + sqrtd) / a;
                if (!rayT.Surrounds(root))
                {
                    return false;
                }
            }

            rec.T = root;
            rec.P = r.At(rec.T);
            Vec3 outwardNormal = (rec.P - Center) / Radius;
            rec.SetFaceNormal(r, outwardNormal);
            rec.Mat = Mat;

            return true;
        }
    }
}