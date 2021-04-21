using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neuralnet1
{
    public class matrix
    {
        public static float dot(float[] vector1, float[] vector2)
        {
            // calculates dot product on two vectors of the same shape
            float sum = 0;
            for (int i = 0; i < vector1.Length; i++)
            {
                sum += vector1[i] * vector2[i];
            }
            return sum;
        }

        public static float[] dot(float[,] matrix1, float[] vector2)
        {
            // calculates dot product on matrix and vector
            float[] rv = new float[matrix1.GetUpperBound(0) + 1];
            for (int i = 0; i < matrix1.GetUpperBound(0) + 1; i++)
            {
                float sum = 0;
                for (int j = 0; j < matrix1.GetUpperBound(1) + 1; j++)
                {
                    sum += matrix1[i, j] * vector2[j];

                }
                rv[i] = sum;
            }
            return rv;
        }
        public static float[,] transpose(float[,] m1)
        {
            // switches rows and columns
            int rows = m1.GetUpperBound(1) + 1;
            int cols = m1.GetUpperBound(0) + 1;
            float[,] rv = new float[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    rv[i, j] = m1[j, i];
                }
            }
            return rv;
        }
        public static float[,] dot(float[,] m1, float[,] m2)
        {
            // calculates dot product on two matricies
            // matrix (row, column)
            int rows = m1.GetUpperBound(0) + 1;
            int cols = m2.GetUpperBound(1) + 1;
            float[,] rv = new float[rows, cols];

            for (int i = 0; i < rows; i++)
            {

                for (int j = 0; j < cols; j++)
                {
                    float col_sum = 0f;
                    for (int k = 0; k < m2.GetUpperBound(0) + 1; k++)
                    {
                        col_sum += m2[k, j] * m1[i, k];
                    }
                    rv[i, j] = col_sum;
                }

            }

            return rv;
        }

        public static float[,] add(float[,] m1, float[] v2)
        {
            int rows = m1.GetUpperBound(0) + 1;
            int cols = m1.GetUpperBound(1) + 1;
            float[,] rv = new float[rows, cols];

            for (int i = 0; i < rows; i++)
            {

                for (int j = 0; j < cols; j++)
                {

                    rv[i, j] = m1[i, j] + v2[j];
                }

            }
            return rv;
        }

        public static float[] add(float[] matrix1, float[] matrix2)
        {
            // adds a vector quantity to a matrix
            float[] rv = new float[matrix1.GetUpperBound(0) + 1];
            for (int i = 0; i < matrix1.GetUpperBound(0) + 1; i++)
            {
                rv[i] = matrix1[i] + matrix2[i];
            }
            return rv;
        }
    }
}
