using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neuralnet1
{

   class DenseLayer
    {
        static Random rand = new Random();
        public float[,] weights;
        public float[] biases;
        public float[,] outputs;
        public DenseLayer (int n_inputs, int n_neurons)
        {
            this.weights = random_weights(n_inputs, n_neurons);
            this.biases = new float[n_neurons];
            this.outputs = new float[1,n_neurons];


        }

        public void Forward(float [,] inputs)
        {
            this.outputs = matrix.add(matrix.dot(inputs, this.weights), this.biases);
        }
        static float[,] random_weights (int a, int b)
        {
            float[,] r = new float[a, b];
            for (int i = 0; i < a; i++)
            {
                for (int j = 0; j < b; j++)
                {
                    r[i, j] = (float)rand.NextDouble()*2-1;

                }
                
            }
            return r;

        }
        public override string ToString()
        {
            String r = "";
            r += "{\n";
            for (int i = 0; i < this.weights.GetUpperBound(0)+1; i++)
            {
                r += "  {";
                for (int j = 0; j < this.weights.GetUpperBound(1) + 1; j++)
                {
                    r +=  this.weights[i, j];
                    if (j == this.weights.GetUpperBound(1))
                    {
                        r += "}\n";
                    } else
                    {
                        r += ", ";
                    }

                }

            }
            r += "}\n";
            return r;
        }
    }
    class Program
    {
 
        
        static void Main(string[] args)
        {
            mnist m = new mnist();
            m.LoadTrainDataset();

            Console.WriteLine("This is C#");
            
            float[,] inputs = {{ 1.0f, 2.0f, 3.0f, 2.5f },
                               {2.0f, 5.0f, -1.0f, 2.0f },
                               {-1.5f, 2.7f, 3.3f, -0.8f } };
            /*
            float[,] weights = { { 0.2f, 0.8f, -0.5f, 1.0f },
                                { 0.5f, -0.91f, 0.26f, -0.5f },
                                { -0.26f, -0.27f, 0.17f, 0.87f }
                              };
            float[] biases = { 2.0f, 3.0f, 0.5f };
            
            float[,] outputs = matrix.add(matrix.dot(inputs, matrix.transpose (weights)), biases);
            
            Console.WriteLine("{0}, {1}, {2}", outputs[0,0], outputs[0, 1], outputs[0, 2]);
            Console.WriteLine("{0}, {1}, {2}", outputs[1, 0], outputs[1, 1], outputs[1, 2]);
            Console.WriteLine("{0}, {1}, {2}", outputs[2, 0], outputs[2, 1], outputs[2, 2]);
            Console.WriteLine(outputs.Length);


            Console.WriteLine("Press any key to exit.");
            */
            DenseLayer layer1 = new DenseLayer(3,7);
            layer1.Forward(inputs);
            Console.WriteLine(layer1);

            Console.ReadKey();
        }
    }
}
