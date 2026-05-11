namespace Raytracer
{
    public interface IHittable
    {
        bool Hit(Ray r, Interval rayT, HitRecord rec);
    }
}