namespace Spatial_full
{
    public class SpVector3
    {
        public double X, Y, Z;
        public SpVector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public static SpVector3 Normalize(SpVector3 A)
        {
            double invLength = System.Math.Sqrt(A.X * A.X + A.Y * A.Y + A.Z * A.Z);
            return new SpVector3(A.X * invLength, A.Y * invLength, A.Z * invLength);
        }
    }
}