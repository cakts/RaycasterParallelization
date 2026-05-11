namespace Raytracer
{
    public class HitRecord
    {
        public Point3 P { get; set; }
        public Vec3 Normal { get; set; }
        public double T { get; set; }
        public Material Mat { get; set; }
        public bool FrontFace { get; set; }

        public HitRecord()
        {
            P = new Point3();
            Normal = new Vec3();
            T = 0.0;
            Mat = null;
            FrontFace = false;
        }

        public void SetFaceNormal(Ray r, Vec3 outwardNormal)
        {
            // Sets the hit record normal vector.
            // NOTE: the parameter `outwardNormal` is assumed to have unit length.
            FrontFace = Vec3.Dot(r.Direction, outwardNormal) < 0;
            Normal = FrontFace ? outwardNormal : -outwardNormal;
        }
    }
}