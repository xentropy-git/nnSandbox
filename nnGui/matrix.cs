using System;


namespace neuralnet
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
        public static double [,] zeros_like (double [,] m0)
        {
            // make a matrix of 0 values of the same shape as m0
            int rows = m0.GetUpperBound(0) + 1;
            int cols = m0.GetUpperBound(1) + 1;
            double [,] rv = new double [rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    rv[i, j] =  0;
                }

            }
            return rv;
        }
        public static double[] scale(double scalar, double[] v1)
        {
            // Multiplies a vector by a scalar
            int length = v1.GetUpperBound(0) + 1;
            double[] rv = new double[length];

            for (int i = 0; i < length; i++)
            {
                rv[i] =  scalar * v1[i];
            }

            return rv;
        }
        
        public static double[,] scale(double scalar, double[,] m1)
        {
            // Multiplies a matrix by a scalar
            int rows = m1.GetUpperBound(0) + 1;
            int cols = m1.GetUpperBound(1) + 1;
            double[,] rv = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {

                for (int j = 0; j < cols; j++)
                {
                    rv[i, j] =  scalar * m1[i,j];
                }

            }

            return rv;
        }
        public static double[,] sum (double[,] m0, int axis = 0)
        {
            int rows = m0.GetUpperBound(0) + 1;
            int cols = m0.GetUpperBound(1) + 1;

            double[,] rv = new double[rows, 1];
            
            if (axis == 0) {     
                rv = new double[1, cols];
                for (int j = 0; j < cols; j++)
                {
                    double col_sum = 0f;
                    for (int i = 0; i < rows; i++)
                    {
                        col_sum += m0[i, j];
                    }
                    rv[0, j] = col_sum;
                }
                
            }
            else if (axis == 1) {
                rv = new double[rows, 1];
                for (int i = 0; i < rows; i++)
                {
                    double row_sum = 0f;
                    for (int j = 0; j < cols; j++)
                    {
                        row_sum += m0[i, j];
                    }
                    rv[i, 0] = row_sum;
                }
            }
            return rv;
        }
        public static double[,] copy(double[,] m1)
        {
            double[,] rv = m1.Clone() as double[,];
            return rv;
        }
        public static double[] copy(double[] v1)
        {
            // calculates dot product on two matricies
            // matrix (row, column)
            /*
            int size = v1.GetUpperBound(0) + 1;
            double[] rv = new double[size];
            v1.CopyTo(rv,0);
            */
            double[] rv = v1.Clone() as double[];
            return rv;
        }
        public static bool compare(double[,] m0, double[,] m1)
        {
            
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
                        return false;
                    }
                }

            }

            return true;
        }

        public static bool compare(double[] m0, double[] m1)
        {
            
            int size = m1.GetUpperBound(0) + 1;

            for (int i = 0; i < size; i++)
            {
                // tolerance is required due to precision/rounding errors
                double difference = Math.Abs(m0[i] * .00001);
                if (Math.Abs(m0[i] - m1[i]) > difference)
                {
                    return false;
                }
            }
            return true;
        }
        public static Tuple<int, double> argmax(double[,] a, int row)
        {
            // determines the argmax of a row of a matrix.
            double maxValue = 0;
            int maxIndex = 0;
            for (int i = 0; i < a.GetUpperBound(1)+1; i++)
            {
                if (a[row,i] > maxValue)
                {
                    maxValue = a[row,i];
                    maxIndex = i;
                }
            }
            return Tuple.Create(maxIndex, maxValue);
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

        public static double[,] add(double[,] m1, double[,] m2)
        {
            int rows = m1.GetUpperBound(0) + 1;
            int cols = m1.GetUpperBound(1) + 1;
            int rows2 = m2.GetUpperBound(0) + 1;
            int cols2 = m2.GetUpperBound(1) + 1;
            double[,] rv = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {

                for (int j = 0; j < cols; j++)
                {
                    if (rows == rows2 && cols == cols2)
                    {
                        rv[i, j] = m1[i, j] + m2[i, j];
                    } else if (rows2 == 1)
                    {
                        // m2 is of shape 1xn, so treat this like vector addition
                        // necessary for bias calculation
                        rv[i, j] = m1[i, j] + m2[0, j];
                    }
                    
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
                    r += m0[i, j].ToString("0.####");
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

        public static string VectorToString(double[] m0)
        {
            String r = "";
            r += "{";
            for (int i = 0; i < m0.GetUpperBound(0) + 1; i++)
            {
                r += m0[i];
                if (i != m0.GetUpperBound(0))
                {
                    r += ", ";
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
        }
        public static void copy_test_0 () {
            double[] test = {1, 2, 3, 4, 5};
            
            double[] test2 = copy(test);
            for (int i = 0; i < test2.GetUpperBound(0) + 1; i++)
            {
                test2[i] = test2[i] + 1;
            }

            if (compare(test, test2))
            {
                Console.WriteLine("Error, unexpected result from matrix.copy()");
                Console.WriteLine(VectorToString(test));
                Console.WriteLine(VectorToString(test2));
            }
        }

        public static void unit_sum_test_0()
        {

            double[,] test_input = 
                        {{10.0, -7.0, 2.0, 1.0},
                        {-6.0, 18.0, -14.0, -3.0},
                        {-10.0, 6.0, -1.0, -1.0}};

            double[,] test_expected = 
                        {{-6, 17, -13, -3}};

            double[,] output = sum(test_input, 0);

            if (!compare(output, test_expected))
            {
                Console.WriteLine("Error, unexpected result from matrix.sum(axis=0)");
                Console.WriteLine(MatrixToString(output));
            }
        }

        public static void unit_sum_test_1()
        {

            double[,] test_input = 
                        {{-1.0, 21.0, -4.0, 2.0},
                        {9.0, -2.0, -10.0, 7.0},
                        {-24.0, 0.0, -5.0, 2.0}};

            double[,] test_expected = 
                        {{18.0},{4.0}, {-27.0}};

            double[,] output = sum(test_input, 1);

            if (!compare(output, test_expected))
            {
                Console.WriteLine("Error, unexpected result from matrix.sum(axis=1)");
                Console.WriteLine(MatrixToString(output));
            }
        }

        public static void unit_tests ()
        {
            unit_dot_test_0();
            unit_dot_test_1();
            copy_test_0();
            unit_sum_test_0();
            unit_sum_test_1();

        }
    }
}
