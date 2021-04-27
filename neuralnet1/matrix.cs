using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neuralnet1
{
    public class matrix
    {
        public static double dot(double[] vector1, double[] vector2)
        {
            // calculates dot product on two vectors of the same shape
            double sum = 0;
            for (int i = 0; i < vector1.Length; i++)
            {
                sum += vector1[i] * vector2[i];
            }
            return sum;
        }

        public static double[] dot(double[,] matrix1, double[] vector2)
        {
            // calculates dot product on matrix and vector
            double[] rv = new double[matrix1.GetUpperBound(0) + 1];
            for (int i = 0; i < matrix1.GetUpperBound(0) + 1; i++)
            {
                double sum = 0;
                for (int j = 0; j < matrix1.GetUpperBound(1) + 1; j++)
                {
                    sum += matrix1[i, j] * vector2[j];

                }
                rv[i] = sum;
            }
            return rv;
        }
        public static double[,] transpose(double[,] m1)
        {
            // switches rows and columns
            int rows = m1.GetUpperBound(1) + 1;
            int cols = m1.GetUpperBound(0) + 1;
            double[,] rv = new double[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    rv[i, j] = m1[j, i];
                }
            }
            return rv;
        }
        public static double[,] dot(double[,] m1, double[,] m2)
        {
            // calculates dot product on two matricies
            // matrix (row, column)
            int rows = m1.GetUpperBound(0) + 1;
            int cols = m2.GetUpperBound(1) + 1;
            double[,] rv = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {

                for (int j = 0; j < cols; j++)
                {
                    double col_sum = 0f;
                    for (int k = 0; k < m2.GetUpperBound(0) + 1; k++)
                    {
                        col_sum += m2[k, j] * m1[i, k];
                    }
                    rv[i, j] = col_sum;
                }

            }

            return rv;
        }
        public static bool compare(double[,] m0, double[,] m1)
        {
            // calculates dot product on two matricies
            // matrix (row, column)
            int rows = m1.GetUpperBound(0) + 1;
            int cols = m1.GetUpperBound(1) + 1;

            for (int i = 0; i < rows; i++)
            {

                for (int j = 0; j < cols; j++)
                {
                    // tolerance is required due to precision/rounding errors
                    double difference = Math.Abs(m0[i, j] * .00001);
                    if (Math.Abs(m0[i, j] - m1[i, j]) > difference)
                    {
                        Console.WriteLine(m0[i, j] + " != " + m1[i, j]);
                        return false;
                    }
                }

            }

            return true;
        }
        public static double[,] add(double[,] m1, double[] v2)
        {
            int rows = m1.GetUpperBound(0) + 1;
            int cols = m1.GetUpperBound(1) + 1;
            double[,] rv = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {

                for (int j = 0; j < cols; j++)
                {

                    rv[i, j] = m1[i, j] + v2[j];
                }

            }
            return rv;
        }
        public static string MatrixToString(double[,] m0)
        {
            String r = "";
            r += "{\n";
            for (int i = 0; i < m0.GetUpperBound(0) + 1; i++)
            {
                r += "  {";
                for (int j = 0; j < m0.GetUpperBound(1) + 1; j++)
                {
                    r += m0[i, j];
                    if (j == m0.GetUpperBound(1))
                    {
                        r += "}\n";
                    }
                    else
                    {
                        r += ", ";
                    }

                }

            }
            r += "}\n";
            return r;
        }
        public static double[] add(double[] matrix1, double[] matrix2)
        {
            // adds a vector quantity to a matrix
            double[] rv = new double[matrix1.GetUpperBound(0) + 1];
            for (int i = 0; i < matrix1.GetUpperBound(0) + 1; i++)
            {
                rv[i] = matrix1[i] + matrix2[i];
            }
            return rv;
        }
        public static void unit_dot_test_0()
        {
            double[,] m0 = {{ 1.0f, 7.0f, 3.0f, -1.0f },
                           {2.0f, 5.0f, -1.0f, 8.0f },
                           {0f, 1f, 6f, 3f } };
            double[,] m1 = {{ 7.0f, 3.0f, 1.0f },
                           {1.0f, 2.0f, -1.0f },
                           {2.0f, 5.0f, 3.0f },
                           {3.0f, 1.0f, 0.0f } };
            double[,] correct = {{17f, 31f, 3f },
                                {41f, 19f, -6f },
                                { 22f, 35f, 17f} };
            double[,] output = dot(m0, m1);

            if (!compare(output, correct))
            {
                Console.WriteLine("Error, unexpected result from matrix.dot()");
                Console.WriteLine(MatrixToString(output));
            }
            else
            {
                Console.WriteLine("Unit test passed");
            }
        }
        public static void unit_dot_test_1()
        {

            double[,] m0 = {{1.0, 2.0, 3.0, 2.5},
                        {2.0, 5.0, -1.0, 2.0},
                        {-1.5, 2.7, 3.3, -0.8}};

            double[,] m1 = {{1.0, -2.0, 3.0, 1.5, 1.3},
                        {0.9, 2.3, 1.8, -2.0, 9.0},
                        {9.0, 1.0, -2.0, 7.0, 1.0},
                        {-2.0, 1.3, 1.7, 2.4, 5.6}};
             
            double[,] correct = {{24.8,  8.85, 4.85, 24.5, 36.3},
                                {-6.5,  9.1,  20.4, -9.2, 57.8},
                                {32.23, 11.47, -7.6,  13.53, 21.17}};

            double[,] output = dot(m0, m1);

            if (!compare(output, correct))
            {
                Console.WriteLine("Error, unexpected result from matrix.dot()");
                Console.WriteLine(MatrixToString(output));
            }
            else
            {
                Console.WriteLine("Unit test passed");
            }
        }
        public static void unit_tests ()
        {
            unit_dot_test_0();
            unit_dot_test_1();
        }
    }
}
