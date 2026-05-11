using System;

namespace Raytracer
{
    public class Vec3
    {
        public double[] E { get; private set; }

        public Vec3() : this(0, 0, 0) { }

        public Vec3(double e0, double e1, double e2)
        {
            E = new double[] { e0, e1, e2 };
        }

        public double X() => E[0];
        public double Y() => E[1];
        public double Z() => E[2];

        public double this[int i]
        {
            get => E[i];
            set => E[i] = value;
        }

        public static Vec3 operator -(Vec3 v)
        {
            return new Vec3(-v.E[0], -v.E[1], -v.E[2]);
        }

        public static Vec3 operator +(Vec3 u, Vec3 v)
        {
            return new Vec3(u.E[0] + v.E[0], u.E[1] + v.E[1], u.E[2] + v.E[2]);
        }

        public static Vec3 operator -(Vec3 u, Vec3 v)
        {
            return new Vec3(u.E[0] - v.E[0], u.E[1] - v.E[1], u.E[2] - v.E[2]);
        }

        public static Vec3 operator *(Vec3 u, Vec3 v)
        {
            return new Vec3(u.E[0] * v.E[0], u.E[1] * v.E[1], u.E[2] * v.E[2]);
        }

        public static Vec3 operator *(double t, Vec3 v)
        {
            return new Vec3(t * v.E[0], t * v.E[1], t * v.E[2]);
        }

        public static Vec3 operator *(Vec3 v, double t)
        {
            return t * v;
        }

        public static Vec3 operator /(Vec3 v, double t)
        {
            return (1.0 / t) * v;
        }

        public double Length()
        {
            return Math.Sqrt(LengthSquared());
        }

        public double LengthSquared()
        {
            return E[0] * E[0] + E[1] * E[1] + E[2] * E[2];
        }

        public static double Dot(Vec3 u, Vec3 v)
        {
            return u.E[0] * v.E[0] + u.E[1] * v.E[1] + u.E[2] * v.E[2];
        }

        public static Vec3 Cross(Vec3 u, Vec3 v)
        {
            return new Vec3(
                u.E[1] * v.E[2] - u.E[2] * v.E[1],
                u.E[2] * v.E[0] - u.E[0] * v.E[2],
                u.E[0] * v.E[1] - u.E[1] * v.E[0]
            );
        }

        public static Vec3 UnitVector(Vec3 v)
        {
            return v / v.Length();
        }

        public static Vec3 Random()
        {
            return new Vec3(Utility.RandomDouble(), Utility.RandomDouble(), Utility.RandomDouble());
        }

        public static Vec3 Random(double min, double max)
        {
            return new Vec3(Utility.RandomDouble(min, max), Utility.RandomDouble(min, max), Utility.RandomDouble(min, max));
        }

        public bool NearZero()
        {
            double s = 1e-8;
            return (Math.Abs(E[0]) < s) && (Math.Abs(E[1]) < s) && (Math.Abs(E[2]) < s);
        }

        public static Vec3 Reflect(Vec3 v, Vec3 n)
        {
            return v - 2 * Dot(v, n) * n;
        }

        public static Vec3 Refract(Vec3 uv, Vec3 n, double etaiOverEtat)
        {
            double cosTheta = Math.Min(Dot(-uv, n), 1.0);
            Vec3 rOutPerp = etaiOverEtat * (uv + cosTheta * n);
            Vec3 rOutParallel = -Math.Sqrt(Math.Abs(1.0 - rOutPerp.LengthSquared())) * n;
            return rOutPerp + rOutParallel;
        }

        public static Vec3 RandomInUnitSphere()
        {
            while (true)
            {
                Vec3 p = Vec3.Random(-1, 1);
                if (p.LengthSquared() < 1)
                    return p;
            }
        }

        public static Vec3 RandomUnitVector()
        {
            return UnitVector(RandomInUnitSphere());
        }

        public static Vec3 RandomOnHemisphere(Vec3 normal)
        {
            Vec3 onUnitSphere = RandomUnitVector();
            if (Dot(onUnitSphere, normal) > 0.0)
                return onUnitSphere;
            else
                return -onUnitSphere;
        }

        public static Vec3 RandomInUnitDisk()
        {
            while (true)
            {
                Vec3 p = new Vec3(Utility.RandomDouble(-1, 1), Utility.RandomDouble(-1, 1), 0);
                if (p.LengthSquared() < 1)
                    return p;
            }
        }

        public override string ToString()
        {
            return $"{E[0]} {E[1]} {E[2]}";
        }
    }

    // Type aliases
    public class Point3 : Vec3
    {
        public Point3() : base() { }
        public Point3(double e0, double e1, double e2) : base(e0, e1, e2) { }
    }

    public class Color : Vec3
    {
        public Color() : base() { }
        public Color(double e0, double e1, double e2) : base(e0, e1, e2) { }
    }
}