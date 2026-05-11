using System.Collections.Generic;

namespace Raytracer
{
    public class HittableList : IHittable
    {
        public List<IHittable> Objects { get; private set; }

        public HittableList()
        {
            Objects = new List<IHittable>();
        }

        public HittableList(IHittable obj)
        {
            Objects = new List<IHittable>();
            Add(obj);
        }

        public void Clear()
        {
            Objects.Clear();
        }

        public void Add(IHittable obj)
        {
            Objects.Add(obj);
        }

        public bool Hit(Ray r, Interval rayT, HitRecord rec)
        {
            bool hitAnything = false;
            double closestSoFar = rayT.Max;

            foreach (IHittable obj in Objects)
            {
                if (obj.Hit(r, new Interval(rayT.Min, closestSoFar), rec))
                {
                    hitAnything = true;
                    closestSoFar = rec.T;
                }
            }

            return hitAnything;
        }
    }
}