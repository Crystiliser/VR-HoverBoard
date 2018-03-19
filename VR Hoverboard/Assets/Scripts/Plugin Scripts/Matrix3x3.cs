namespace Spatial_full
{
    public class Matrix3x3
    {
        public double[,] matrix = new double[3, 3];
        public Matrix3x3(
            double e00, double e01, double e02,
            double e10, double e11, double e12,
            double e20, double e21, double e22)
        {
            matrix[0, 0] = e00;
            matrix[0, 1] = e01;
            matrix[0, 2] = e02;
            matrix[1, 0] = e10;
            matrix[1, 1] = e11;
            matrix[1, 2] = e12;
            matrix[2, 0] = e20;
            matrix[2, 1] = e21;
            matrix[2, 2] = e22;
        }
        public Matrix3x3(Matrix3x3 otherMatrix)
        {
            matrix[0, 0] = otherMatrix.matrix[0, 0];
            matrix[0, 1] = otherMatrix.matrix[0, 1];
            matrix[0, 2] = otherMatrix.matrix[0, 2];
            matrix[1, 0] = otherMatrix.matrix[1, 0];
            matrix[1, 1] = otherMatrix.matrix[1, 1];
            matrix[1, 2] = otherMatrix.matrix[1, 2];
            matrix[2, 0] = otherMatrix.matrix[2, 0];
            matrix[2, 1] = otherMatrix.matrix[2, 1];
            matrix[2, 2] = otherMatrix.matrix[2, 2];
        }
        public static SpVector3 Multiply(SpVector3 A, Matrix3x3 B)
        {
            return new SpVector3(
                B.matrix[0, 0] * A.X + B.matrix[1, 0] * A.Y + B.matrix[2, 0] * A.Z,
                B.matrix[0, 1] * A.X + B.matrix[1, 1] * A.Y + B.matrix[2, 1] * A.Z,
                B.matrix[0, 2] * A.X + B.matrix[1, 2] * A.Y + B.matrix[2, 2] * A.Z);
        }
        public static Matrix3x3 Multiply(Matrix3x3 A, Matrix3x3 B)
        {
            return new Matrix3x3(
                B.matrix[0, 0] * A.matrix[0, 0] + B.matrix[0, 1] * A.matrix[1, 0] + B.matrix[0, 2] * A.matrix[2, 0],
                B.matrix[0, 0] * A.matrix[0, 1] + B.matrix[0, 1] * A.matrix[1, 1] + B.matrix[0, 2] * A.matrix[2, 1],
                B.matrix[0, 0] * A.matrix[0, 2] + B.matrix[0, 1] * A.matrix[1, 2] + B.matrix[0, 2] * A.matrix[2, 2],
                B.matrix[1, 0] * A.matrix[0, 0] + B.matrix[1, 1] * A.matrix[1, 0] + B.matrix[1, 2] * A.matrix[2, 0],
                B.matrix[1, 0] * A.matrix[0, 1] + B.matrix[1, 1] * A.matrix[1, 1] + B.matrix[1, 2] * A.matrix[2, 1],
                B.matrix[1, 0] * A.matrix[0, 2] + B.matrix[1, 1] * A.matrix[1, 2] + B.matrix[1, 2] * A.matrix[2, 2],
                B.matrix[2, 0] * A.matrix[0, 0] + B.matrix[2, 1] * A.matrix[1, 0] + B.matrix[2, 2] * A.matrix[2, 0],
                B.matrix[2, 0] * A.matrix[0, 1] + B.matrix[2, 1] * A.matrix[1, 1] + B.matrix[2, 2] * A.matrix[2, 1],
                B.matrix[2, 0] * A.matrix[0, 2] + B.matrix[2, 1] * A.matrix[1, 2] + B.matrix[2, 2] * A.matrix[2, 2]);
        }
    }
}