namespace Raytracer
{
    public class Ray
    {
        public Point3 Origin { get; private set; }
        public Vec3 Direction { get; private set; }

        public Ray()
        {
            Origin = new Point3();
            Direction = new Vec3();
        }

        public Ray(Point3 origin, Vec3 direction)
        {
            Origin = origin;
            Direction = direction;
        }

        public Point3 At(double t)
        {
            Vec3 result = Origin + t * Direction;
            return new Point3(result.X(), result.Y(), result.Z());
        }
    }
}
