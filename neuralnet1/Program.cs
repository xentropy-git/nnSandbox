using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * TODO:
 *  Implement ArgMax()
 * Implement accuracy() returns the % of time the neural network correctly classified the inputs
 *    number_of_accurate_predections / number_of_samples
 * 
 */
namespace neuralnet1
{
    enum Activation
    {
        ReLU,
        Sigmoid,
        SoftMax
    }
    enum Loss
    {
        CategoricalCrossEntropy
    }
    class LossFunction
    {
        public double[,] y_predicted;
        public double[,] y_true;
        public double [] loss;
        public LossFunction()
        {
        }

        public virtual void Calculate(double[,] y_predicted, double[,] y_true)
        {
            throw new NotImplementedException();
        }

        public double MeanLoss()
        {
            int count =0;
            double sum =0;
            foreach (double d in this.loss)
            {
                sum += d;
                count++;
            }
            return sum / count;
        }
    }
    class CategoricalCrossEntropy : LossFunction
    {
        public CategoricalCrossEntropy(): base ()
        {
        }
        public override void Calculate (double[,] y_predicted, double[,] y_true)
        {
            /*
             * Categorical Cross Entropy
             * predicted - a 2d array of outputs from the softmax layer
             *      rows represent batches and columns represent neuron outputs
             * true - a 2d array of one hot encoded categories
             *      rows represent batches and columns represent desired neuron outputs
             *      ie: for 10 digits, the nth index is 1 and the rest are 0 where n is the digit
             *      1 = 0, 1, 0, 0, 0, 0, 0, 0, 0 ,0
             *      5 = 0, 0, 0, 0, 0, 1, 0, 0, 0 ,0
             *      
             *      loss is calculated as the negative sum of logs of the output multiplied by the desired outputs
             *      
             */
            this.y_predicted = y_predicted;
            this.y_true = y_true;
            this.loss = new double[y_predicted.GetUpperBound(0) + 1];
          

            for (int i = 0; i < y_predicted.GetUpperBound(0) + 1; i++)
            {
                double sum_of_logs = 0;
                for (int j = 0; j < y_predicted.GetUpperBound(1) + 1; j++)
                {
                    double clipped_predicted = Math.Min(Math.Max(y_predicted[i, j], 1e-7),1-1e-7);
                    //Console.WriteLine(clipped_predicted.ToString("0.########"));

                    sum_of_logs += Math.Log(clipped_predicted) * y_true[i, j];
                }
                this.loss[i] = -(sum_of_logs);

            }
        }

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
            if (at == Activation.Sigmoid) throw new NotImplementedException();




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

        public double GetOutputAt (int i, int j)
        {
            return activation_function.output[i,j];
        }
        public int GetRows()
        {
            return this.GetOutput().GetUpperBound(0) + 1;
        }

        public int GetCols()
        {
            return this.GetOutput().GetUpperBound(1) + 1;
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
            int[] samples = { 0, 1, 5, 100, 999 };
            m.Load();
            double[,] inputs = m.GetBatchSamples(samples);
            double[,] class_targets = m.GetBatchClassifications(samples);

            DenseLayer layer1 = new DenseLayer(784, 24, Activation.ReLU);
            DenseLayer layer2 = new DenseLayer(24, 24, Activation.ReLU);
            DenseLayer layer3 = new DenseLayer(24, 10, Activation.SoftMax);
            layer1.Forward(inputs);
            layer2.Forward(layer1.GetOutput());
            layer3.Forward(layer2.GetOutput());
            LossFunction loss = new CategoricalCrossEntropy();
                        

            //Console.WriteLine(matrix.MatrixToString(layer3.GetOutput()));
            loss.Calculate(layer3.GetOutput(), class_targets);

            // Accuracy Calculation
            // for each row of matrix,
            // do equivalency check of arg max from prediction and arg max of true
            // the mean of the equivalency checks is the accuracy.
            double sum_correct = 0;
            for (int i = 0; i < layer3.GetRows(); i++)
            {
                (int argMaxA, double valueA) = matrix.argmax(layer3.GetOutput(), i);
                (int argMaxB, double valueB) = matrix.argmax(class_targets, i);
                Console.WriteLine("Predicted {0}, True {1}", argMaxA, argMaxB);
                if(argMaxA == argMaxB) sum_correct++;
            }
            double acc = sum_correct / layer3.GetRows();

            Console.WriteLine ("loss: {0}", loss.MeanLoss().ToString("0.########"));
            Console.WriteLine("acc: {0}", acc.ToString("0.########"));
            Console.ReadKey();
        }
    }
}
