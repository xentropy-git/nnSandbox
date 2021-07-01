using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * TODO:
 *  Calculate dbiases on backward phase for DenseLayers.  Must implement a function like np.sum()
 *  Implement AdjustValues() for each class to adjust weights and biases based on partial derivative calcs
 *      Might be helpful to create a matrix.add function
 *      matrix.add (matrix [weights], -learning_rate * matrix2 [dweights])
 *  Part of the stochastic gradient descent is already implemented, as samples are generated and sent on the forward pass
 *  The next part of the SGD algorithm will adjust values based on a learning rate, and then select a different
 *  random sample set on each iteration.
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
        public double[,] output;
        public double[,] dinputs;

        public LossFunction()
        {
        }

        public virtual void Forward(double[,] y_predicted, double[,] y_true)
        {
            throw new NotImplementedException();
        }

        public virtual void Backward(double[,] dvalues, double[,] y_true)
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
        public override void Backward(double[,] dvalues, double[,] y_true)
        {
            int samples = dvalues.GetUpperBound(0)+1;
            int labels = dvalues.GetUpperBound(1)+1;
            Console.WriteLine("Samples: {0}", samples);
            Console.WriteLine("Labels: {0}", labels);

            int tsamples = y_true.GetUpperBound(0)+1;
            int tlabels = y_true.GetUpperBound(1)+1;
            Console.WriteLine("True Samples: {0}", tsamples);
            Console.WriteLine("True Labels: {0}", tlabels);

            // calculate the gradient
            this.dinputs = new double[tsamples, tlabels];

        }
        public override void Forward (double[,] y_predicted, double[,] y_true)
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
            this.y_predicted = matrix.copy(y_predicted);
            this.y_true = matrix.copy(y_true);
            this.output = matrix.copy(y_predicted);
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
        public double[,] inputs;
        public double[,] dinputs;
        public ActivationFunction(int n_neurons)
        {
            this.output = new double[1, n_neurons];
        }

        public virtual void Forward(double[,] inputs)
        {
            throw new NotImplementedException();
        }
        public virtual void Backward(double[,] dinputs)
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
            this.inputs = matrix.copy(inputs);
            this.output = new double[inputs.GetUpperBound(0)+1, inputs.GetUpperBound(1)+1];
            for (int i = 0; i < inputs.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < inputs.GetUpperBound(1) + 1; j++)
                {
                    this.output[i, j] = Math.Max(0,inputs[i, j]);

                }

            }
        }
        public override void Backward (double[,] dvalues) {
            this.dinputs = matrix.copy(dvalues);

            // zero gradient where input values were negative
             for (int i = 0; i < this.inputs.GetUpperBound(0) + 1; i++)
             {
                for (int j = 0; j < this.inputs.GetUpperBound(1) + 1; j++)
                {
                    if (this.inputs[i, j] <= 0) this.dinputs[i,j] = 0;

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
    class ActivationSoftmaxLossCategoricalCrossentropy
    {
        // combines the softmax activation funcion and cross-entropy loss function
        // when these two are used together, the partial derivitave calculation
        // is vastly simpler and faster to compute.
        SoftMax activation;
        LossFunction loss;
        public double[,] output;
        public double[,] dinputs;
        public ActivationSoftmaxLossCategoricalCrossentropy (int n_neurons)
        {
            this.activation = new SoftMax(n_neurons);
            this.loss = new CategoricalCrossEntropy();
        }
        public double Forward(double [,] inputs, double [,] y_pred)
        {
            this.activation.Forward (inputs);
            this.output = this.activation.output;
            this.loss.Forward (this.output, y_pred);

            return this.loss.MeanLoss ();

        }
        public void Backward(double[,] dvalues, double[,] y_true)
        {
            int samples = dvalues.GetUpperBound(0)+1;
            int labels = dvalues.GetUpperBound(1)+1;
            Console.WriteLine("Samples: {0}", samples);
            Console.WriteLine("Labels: {0}", labels);
            int tsamples = y_true.GetUpperBound(0)+1;
            int tlabels = y_true.GetUpperBound(1)+1;
            Console.WriteLine("True Samples: {0}", tsamples);
            Console.WriteLine("True Labels: {0}", tlabels);

            // calculate the gradient
            this.dinputs = matrix.copy(dvalues);

             for (int i = 0; i < samples; i++)
             {
                // subtract 1 from entry in dinputs that represents the true value 
                // in the one-hot-encoded matrix y_true.
                for (int j = 0; j < labels; j++)
                {
                    if (y_true[i,j]==1) this.dinputs[i,j] -= 1; 
                    // normalize the gradient
                    this.dinputs[i,j] = this.dinputs[i,j] /samples;
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
        public double[,] dinputs;
        public double[,] inputs;
        public double[,] dbiases;
        public double[,] dweights;

        public DenseLayer (int n_inputs, int n_neurons)
        {
            this.weights = random_weights(n_inputs, n_neurons);
            this.biases = new double[n_neurons];
            this.output = new double[1,n_neurons];





        }

        public void Forward(double [,] inputs)
        {
            this.inputs = matrix.copy (inputs);
            this.output =matrix.add(matrix.dot(inputs, this.weights), this.biases);

        }
        public double [,] GetOutput ()
        {
            return output;
        }

        public double GetOutputAt (int i, int j)
        {
            return output[i,j];
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
                    r[i, j] = (double)(rand.NextDouble()*2-1)/10;
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
        public void Backward (double[,] dvalues) {
            this.dweights = matrix.dot (this.inputs, dvalues);
            //this.dbiases = matrix.sum (dvalues, axis=0, keepdims=True); // mimic np.sum output...
            this.dinputs = matrix.dot(dvalues, matrix.transpose(this.weights));

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

            
            // build the model
            DenseLayer layer1 = new DenseLayer(784, 24);
            ReLU activation1 = new ReLU(24);
            DenseLayer layer2 = new DenseLayer(24, 24);
            ReLU activation2 = new ReLU(24);
            DenseLayer layer3 = new DenseLayer(24, 10);
            ActivationSoftmaxLossCategoricalCrossentropy loss_activation = new ActivationSoftmaxLossCategoricalCrossentropy(10);
            //SoftMax activation3 = new SoftMax(10);
            //LossFunction loss = new CategoricalCrossEntropy();


            // forward phase of model
            layer1.Forward(inputs);
            activation1.Forward(layer1.output);
            layer2.Forward(activation1.output);
            activation2.Forward(layer2.output);
            layer3.Forward(activation2.output);
            double loss = loss_activation.Forward(layer3.output, class_targets);


            loss_activation.Backward (loss_activation.output, class_targets);
            layer3.Backward (loss_activation.dinputs);
            activation2.Backward (layer3.dinputs);
            layer2.Backward (activation2.dinputs);
            activation1.Backward (layer2.dinputs);
            layer1.Backward (activation1.dinputs);
            //activation3.Forward (layer3.output);
            //loss.Forward (activation3.output, class_targets);                     
            Console.WriteLine(matrix.MatrixToString(loss_activation.output));
            Console.WriteLine ("loss: {0}", loss.ToString("0.########"));
            //
            
            // Accuracy Calculation
            // for each row of matrix,
            // do equivalency check of arg max from prediction and arg max of true
            // the mean of the equivalency checks is the accuracy.
            /*
            double sum_correct = 0;
            for (int i = 0; i < layer3.GetRows(); i++)
            {
                (int argMaxA, double valueA) = matrix.argmax(layer3.GetOutput(), i);
                (int argMaxB, double valueB) = matrix.argmax(class_targets, i);
                Console.WriteLine("Predicted {0}, True {1}", argMaxA, argMaxB);
                if(argMaxA == argMaxB) sum_correct++;
            }
            double acc = sum_correct / layer3.GetRows();
            Console.WriteLine("acc: {0}", acc.ToString("0.########"));
            */

            
            
            Console.ReadKey();
        }
    }
}
