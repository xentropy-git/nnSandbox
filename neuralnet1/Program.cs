using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * TODO NEXT:
 *  Implement loss function (categorical cross entropy)
 *  Implement optimizer
 */

namespace neuralnet1
{
    enum Activation
    {
        ReLU,
        Sigmoid,
        SoftMax
    }
    class ActivationFunction
    {
        public double[,] output;
        public ActivationFunction(int n_neurons)
        {
            this.output = new double[1, n_neurons];
        }

        public virtual void Forward(double[,] inputs)
        {
            throw new NotImplementedException();
        }
    }
    class ReLU : ActivationFunction
    {
        public ReLU(int n_neurons): base (n_neurons)
        {
        }
        public override void Forward (double[,] inputs)
        {
            this.output = new double[inputs.GetUpperBound(0)+1, inputs.GetUpperBound(1)+1];
            for (int i = 0; i < inputs.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < inputs.GetUpperBound(1) + 1; j++)
                {
                    this.output[i, j] = Math.Max(0,inputs[i, j]);

                }

            }
        }
    }

    class SoftMax : ActivationFunction
    {
        public SoftMax(int n_neurons) : base(n_neurons)
        {
        }
        public override void Forward(double[,] inputs)
        {
            this.output = new double[inputs.GetUpperBound(0) + 1, inputs.GetUpperBound(1) + 1];
            for (int i = 0; i < inputs.GetUpperBound(0) + 1; i++)
            {
                // calculate the max value for each row
                double maxVal= 0;
                for (int j = 0; j < inputs.GetUpperBound(1) + 1; j++)
                {
                    if (inputs[i, j] > maxVal) maxVal = inputs[i, j];
                }
                
                // subtract tha max value so the upper limit is 0
                // output the exponent
                double rowSum = 0;
                for (int j = 0; j < inputs.GetUpperBound(1) + 1; j++)
                {
                    double newVal = Math.Exp(inputs[i,j]- maxVal);
                    this.output[i, j] = newVal;
                    rowSum += newVal;
                }

                // divide the outputs by the sum to get a percentage
                // final result is a probability distribution
                for (int j = 0; j < inputs.GetUpperBound(1) + 1; j++)
                {
                    this.output[i, j] = this.output[i, j]/rowSum ;
                }

            }
        }
    }
    class DenseLayer
    {
        static Random rand = new Random();
        public double[,] weights;
        public double[] biases;
        public double[,] output;
        public ActivationFunction activation_function;
        public DenseLayer (int n_inputs, int n_neurons, Activation at )
        {
            this.weights = random_weights(n_inputs, n_neurons);
            this.biases = new double[n_neurons];
            this.output = new double[1,n_neurons];
            if (at == Activation.ReLU) this.activation_function = new ReLU(n_neurons);
            if (at == Activation.SoftMax) this.activation_function = new SoftMax(n_neurons);




        }

        public void Forward(double [,] inputs)
        {
            this.output =matrix.add(matrix.dot(inputs, this.weights), this.biases);
            activation_function.Forward(this.output);
        }
        public double [,] GetOutput ()
        {
            return activation_function.output;
        }
        static double[,] random_weights (int a, int b)
        {
            double[,] r = new double[a, b];
            for (int i = 0; i < a; i++)
            {
                for (int j = 0; j < b; j++)
                {
                    r[i, j] = (double)rand.NextDouble()*2-1;

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
            matrix.unit_tests();
            mnist m = new mnist();
            m.LoadTrainDataset();

            
            double[,] inputs = {{ 1.0f, 2.0f, 3.0f, 2.5f },
                               {2.0f, 5.0f, -1.0f, 2.0f },
                               {-1.5f, 2.7f, 3.3f, -0.8f } };
   
            DenseLayer layer1 = new DenseLayer(4, 5, Activation.ReLU);
            DenseLayer layer2 = new DenseLayer(5, 2, Activation.SoftMax);
            layer1.Forward(inputs);
            layer2.Forward(layer1.GetOutput());
            Console.WriteLine(matrix.MatrixToString(layer2.output));
            Console.WriteLine(matrix.MatrixToString(layer2.GetOutput()));

            Console.ReadKey();
        }
    }
}
