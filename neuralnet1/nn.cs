using System;
using System.IO;
using System.Collections.Generic;
namespace neuralnet
{
    public static class Constants
    {
        // I plan to develop this program in the future to support other types of layers and activation
        // functions.  I expect that the format for the model file will change over time.
        // Planning for the future, these constants will be stored in the header of every model file.
        public const int MODEL_FILE_MAGIC_NUMBER = 4242;
        public const double MODEL_FILE_VERSION = 1.0;

        // the format of the model file is the {header}{body}
        // the header is {magic_number}{version_number}{epochs}{components}
        // the body is {module_type}{module_data} repeated for n components
        // the module_data for a layer is {inputs}{neurons} then {weight w} for inputs*neurons
        // so inputs/neurons gives the size of the matrix to load
        // {module_data} for functions in empty

        // version 1.0 assumes the last layer's activation is softmax
        // and loss function is categorical cross entropy
        // this information is not stored in the model file
        // it is implicit through model_file_version = 1.0

        public enum ModelComponentType
        {
            DenseLayer = 0,
            ReLUFunction = 10,
            SigmoidFunction = 11,
            SoftMaxFunction = 20
        }
    }

    class LossFunction
    {
        public double[,] y_predicted;
        public double[,] y_true;
        public double[] loss;
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
            int count = 0;
            double sum = 0;
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
        public CategoricalCrossEntropy() : base()
        {
        }
        public override void Backward(double[,] dvalues, double[,] y_true)
        {
            int samples = dvalues.GetUpperBound(0) + 1;
            int labels = dvalues.GetUpperBound(1) + 1;
            Console.WriteLine("Samples: {0}", samples);
            Console.WriteLine("Labels: {0}", labels);

            int tsamples = y_true.GetUpperBound(0) + 1;
            int tlabels = y_true.GetUpperBound(1) + 1;
            Console.WriteLine("True Samples: {0}", tsamples);
            Console.WriteLine("True Labels: {0}", tlabels);

            // calculate the gradient
            this.dinputs = new double[tsamples, tlabels];

        }
        public override void Forward(double[,] y_predicted, double[,] y_true)
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
                    double clipped_predicted = Math.Min(Math.Max(y_predicted[i, j], 1e-7), 1 - 1e-7);
                    //Console.WriteLine(clipped_predicted.ToString("0.########"));

                    sum_of_logs += Math.Log(clipped_predicted) * y_true[i, j];
                }
                this.loss[i] = -(sum_of_logs);

            }
        }

    }

    // ModelPart is a simple business object that layers and activation functions inherit from
    // this allows a neural network model to be made up of an arbitrary number of model parts,
    // Each model part feeds forward into the next during the forward phase
    class ModelComponent
    {
        public double[,] output;
        public double[,] dinputs;
        public ModelComponent()
        {
        }
        public virtual void Forward(double[,] inputs)
        {
            throw new NotImplementedException();
        }
        public virtual void Backward(double[,] dinputs)
        {
            throw new NotImplementedException();
        }

        public virtual void WriteFile(BinaryWriter binWriter, double version)
        {
            throw new NotImplementedException();
        }
        public virtual void ReadFile(BinaryReader binReader, double version)
        {
            throw new NotImplementedException();
        }
    }
    class ActivationFunction : ModelComponent
    {

        public double[,] inputs;

        public ActivationFunction(int n_neurons)
        {
            this.output = new double[1, n_neurons];
        }
    }

    class Layer : ModelComponent
    {
        public double[,] weights;
        public double[,] biases;

        public double[,] inputs;
        public double[,] dbiases;
        public double[,] dweights;

        // momentum history
        public bool momentum_started;
        public double[,] weight_momentums;
        public double[,] bias_momentums;
        public Layer(int n_inputs, int n_neurons)
        {
        }
    }


    class ReLU : ActivationFunction
    {
        public ReLU(int n_neurons) : base(n_neurons)
        {
        }
        public override void Forward(double[,] inputs)
        {
            this.inputs = matrix.copy(inputs);
            this.output = new double[inputs.GetUpperBound(0) + 1, inputs.GetUpperBound(1) + 1];
            for (int i = 0; i < inputs.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < inputs.GetUpperBound(1) + 1; j++)
                {
                    this.output[i, j] = Math.Max(0, inputs[i, j]);

                }

            }
        }
        public override void Backward(double[,] dvalues)
        {
            this.dinputs = matrix.copy(dvalues);

            // zero gradient where input values were negative
            for (int i = 0; i < this.inputs.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < this.inputs.GetUpperBound(1) + 1; j++)
                {
                    if (this.inputs[i, j] <= 0) this.dinputs[i, j] = 0;

                }
            }
        }
        public override string ToString()
        {
            return "ReLU Activation";
        }
        public override void WriteFile(BinaryWriter binWriter, double version)
        {
            if (version >= 1) binWriter.Write((int)Constants.ModelComponentType.ReLUFunction);

        }
        public override void ReadFile(BinaryReader binReader, double version)
        {

        }
    }
    class Sigmoid : ActivationFunction
    {
        public Sigmoid(int n_neurons) : base(n_neurons)
        {
        }
        public override void Forward(double[,] inputs)
        {
            this.inputs = matrix.copy(inputs);
            this.output = new double[inputs.GetUpperBound(0) + 1, inputs.GetUpperBound(1) + 1];
            for (int i = 0; i < inputs.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < inputs.GetUpperBound(1) + 1; j++)
                {
                    this.output[i, j] = 1 / (1 + Math.Exp(-inputs[i, j]));

                }

            }
        }
        public override void Backward(double[,] dvalues)
        {
            this.dinputs = matrix.copy(dvalues);

            // zero gradient where input values were negative
            for (int i = 0; i < this.inputs.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < this.inputs.GetUpperBound(1) + 1; j++)
                {
                    // Derivative - calculates from output of the sigmoid function
                    this.dinputs[i, j] = dvalues[i, j] * (1 - this.output[i, j]) * this.output[i, j];

                }
            }
        }
        public override string ToString()
        {
            return "Sigmoid Activation";
        }
        public override void WriteFile(BinaryWriter binWriter, double version)
        {
            if (version >= 1) binWriter.Write((int)Constants.ModelComponentType.SigmoidFunction);
        }
        public override void ReadFile(BinaryReader binReader, double version)
        {

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
                double maxVal = 0;
                for (int j = 0; j < inputs.GetUpperBound(1) + 1; j++)
                {
                    if (inputs[i, j] > maxVal) maxVal = inputs[i, j];
                }

                // subtract tha max value so the upper limit is 0
                // output the exponent
                double rowSum = 0;
                for (int j = 0; j < inputs.GetUpperBound(1) + 1; j++)
                {
                    double newVal = Math.Exp(inputs[i, j] - maxVal);
                    this.output[i, j] = newVal;
                    rowSum += newVal;
                }

                // divide the outputs by the sum to get a percentage
                // final result is a probability distribution

                for (int j = 0; j < inputs.GetUpperBound(1) + 1; j++)
                {
                    this.output[i, j] = this.output[i, j] / rowSum;
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
        public ActivationSoftmaxLossCategoricalCrossentropy(int n_neurons)
        {
            this.activation = new SoftMax(n_neurons);
            this.loss = new CategoricalCrossEntropy();
        }
        public double Forward(double[,] inputs, double[,] y_pred)
        {
            this.activation.Forward(inputs);
            this.output = this.activation.output;
            this.loss.Forward(this.output, y_pred);

            return this.loss.MeanLoss();

        }

        // forward only without calculating loss
        // returns the output
        public double[,] Forward(double[,] inputs)
        {
            this.activation.Forward(inputs);
            this.output = this.activation.output;
            return this.output;
        }
        public double CalculateLoss(double[,] y_pred)
        {
            this.loss.Forward(this.output, y_pred);
            return this.loss.MeanLoss();
        }
        public double CalculateAccuracy(double[,] y_pred)
        {
            double sum_correct = 0;
            int samples = this.output.GetUpperBound(0) + 1;
            for (int j = 0; j < samples; j++)
            {
                (int argMaxA, double valueA) = matrix.argmax(output, j);
                (int argMaxB, double valueB) = matrix.argmax(y_pred, j);

                if (argMaxA == argMaxB) sum_correct++;
            }
            double acc = sum_correct / samples;
            return acc;
        }
        public void Backward(double[,] dvalues, double[,] y_true)
        {
            int samples = dvalues.GetUpperBound(0) + 1;
            int labels = dvalues.GetUpperBound(1) + 1;

            int tsamples = y_true.GetUpperBound(0) + 1;
            int tlabels = y_true.GetUpperBound(1) + 1;


            // calculate the gradient
            this.dinputs = matrix.copy(dvalues);

            for (int i = 0; i < samples; i++)
            {
                // subtract 1 from entry in dinputs that represents the true value 
                // in the one-hot-encoded matrix y_true.
                for (int j = 0; j < labels; j++)
                {
                    if (y_true[i, j] == 1) this.dinputs[i, j] -= 1;
                    // normalize the gradient
                    this.dinputs[i, j] = this.dinputs[i, j] / samples;
                }


            }


        }
    }
    class DenseLayer : Layer
    {
        static Random rand = new Random();


        public DenseLayer(int n_inputs, int n_neurons) : base(n_inputs, n_neurons)
        {
            this.weights = random_weights(n_inputs, n_neurons);
            this.biases = new double[1, n_neurons];
            this.output = new double[1, n_neurons];
        }
        public override void WriteFile(BinaryWriter binWriter, double version)
        {
            if (version >= 1)
            {
                binWriter.Write((int)Constants.ModelComponentType.DenseLayer);
                if (version >= 1.0)
                {
                    binWriter.Write(this.weights.GetUpperBound(0) + 1);
                    binWriter.Write(this.weights.GetUpperBound(1) + 1);

                }
            }

            for (int i = 0; i < this.weights.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < this.weights.GetUpperBound(1) + 1; j++)
                {
                    binWriter.Write(this.weights[i, j]);
                }

            }

            for (int i = 0; i < this.biases.GetUpperBound(0) + 1; i++)
            {
                binWriter.Write(this.biases[0, i]);
            }

        }
        public override void ReadFile(BinaryReader binReader, double version)
        {
            if (version >= 1.0)
            {
                int n_inputs = binReader.ReadInt32();
                int n_neurons = binReader.ReadInt32();
                this.weights = new double[n_inputs, n_neurons];
                this.biases = new double[1, n_neurons];
                this.output = new double[1, n_neurons];
            }
            for (int i = 0; i < this.weights.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < this.weights.GetUpperBound(1) + 1; j++)
                {
                    this.weights[i, j] = binReader.ReadDouble();
                }

            }

            for (int i = 0; i < this.biases.GetUpperBound(0) + 1; i++)
            {
                this.biases[0, i] = binReader.ReadDouble();
            }

        }
        public override void Forward(double[,] inputs)
        {
            this.inputs = matrix.copy(inputs);
            this.output = matrix.add(matrix.dot(inputs, this.weights), this.biases);

        }
        public double[,] GetOutput()
        {
            return output;
        }

        public double GetOutputAt(int i, int j)
        {
            return output[i, j];
        }
        public int GetRows()
        {
            return this.GetOutput().GetUpperBound(0) + 1;
        }

        public int GetCols()
        {
            return this.GetOutput().GetUpperBound(1) + 1;
        }
        static double[,] random_weights(int a, int b)
        {
            double[,] r = new double[a, b];
            for (int i = 0; i < a; i++)
            {
                for (int j = 0; j < b; j++)
                {
                    r[i, j] = (double)(rand.NextDouble() * 2 - 1) / 10;
                }

            }
            return r;

        }
        public override string ToString()
        {
            return "DenseLayer (inputs = " + (this.weights.GetUpperBound(0) + 1) + ", neurons = " + (this.weights.GetUpperBound(1) + 1) + ")";
        }

        public override void Backward(double[,] dvalues)
        {
            this.dweights = matrix.dot(matrix.transpose(this.inputs), dvalues);
            this.dbiases = matrix.sum(dvalues, 0);
            this.dinputs = matrix.dot(dvalues, matrix.transpose(this.weights));

        }
    }

    // Model class contains all of the information about the neural network
    // number of layers, size of layers, activation functions, etc.
    // it is designed to be dynamic and be able to describe many
    // sizes of networks.  It also handles saving/loading of
    // weights and biases

    class Model
    {
        public List<ModelComponent> components
        { get; }

        public ActivationSoftmaxLossCategoricalCrossentropy loss_activation
        { get; set; }

        double[,] output;
        public int epoch; // remember how many epochs this model has been trained for.
        double version = Constants.MODEL_FILE_VERSION;

        public Model()
        {
            this.components = new List<ModelComponent>();
        }
        public void AddComponent(ModelComponent c)
        {
            this.components.Add(c);
        }
        public double[,] Forward(double[,] inp)
        {
            this.components[0].Forward(inp);
            for (int i = 1; i < this.components.Count; i++)
            {
                this.components[i].Forward(this.components[i - 1].output);
            }
            this.output = this.loss_activation.Forward(this.components[this.components.Count - 1].output);
            return this.output;
        }
        public double CalculateLoss(double[,] y_pred)
        {
            return this.loss_activation.CalculateLoss(y_pred);
        }
        public double CalculateAccuracy(double[,] y_pred)
        {

            return this.loss_activation.CalculateAccuracy(y_pred);

        }
        public void Backward(double[,] class_targets, double learning_rate)
        {
            this.loss_activation.Backward(this.loss_activation.output, class_targets);
            this.components[this.components.Count - 1].Backward(this.loss_activation.dinputs);

            if (this.components.Count > 1)
            {
                for (int i = this.components.Count - 2; i >= 0; i--)
                {
                    this.components[i].Backward(this.components[i + 1].dinputs);
                }
            }


        }

        public void Save(string model_file)
        {
            using (BinaryWriter binWriter = new BinaryWriter(File.Open(model_file, FileMode.Create)))
            {
                binWriter.Write(Constants.MODEL_FILE_MAGIC_NUMBER);
                binWriter.Write(Constants.MODEL_FILE_VERSION);
                binWriter.Write(this.epoch);
                binWriter.Write(this.components.Count);
                foreach (ModelComponent component in this.components)
                {
                    component.WriteFile(binWriter, Constants.MODEL_FILE_VERSION);

                }

            }
        }
        public void Load(string path)
        {
            bool try_v0 = false;
            if (File.Exists(path))
            {
                using (BinaryReader binReader = new BinaryReader(File.Open(path, FileMode.Open)))
                {
                    int magic_number = binReader.ReadInt32();
                    if (magic_number == Constants.MODEL_FILE_MAGIC_NUMBER)
                    {
                        this.version = binReader.ReadDouble();
                        Console.WriteLine("Loading Model Format verion {0} from {1}.", this.version, path);
                        this.epoch = binReader.ReadInt32();
                        int maxC = binReader.ReadInt32();
                        this.components.Clear();
                        for (int c = 0; c < maxC; c++)
                        {
                            ModelComponent component = null;
                            switch (binReader.ReadInt32())
                            {
                                case (int)Constants.ModelComponentType.DenseLayer:
                                    component = new DenseLayer(0, 0);
                                    component.ReadFile(binReader, this.version);
                                    break;
                                case (int)Constants.ModelComponentType.ReLUFunction:
                                    component = new ReLU(0);
                                    component.ReadFile(binReader, this.version);
                                    break;
                                case (int)Constants.ModelComponentType.SigmoidFunction:
                                    component = new Sigmoid(0);
                                    component.ReadFile(binReader, this.version);
                                    break;
                                case (int)Constants.ModelComponentType.SoftMaxFunction:
                                    throw new NotImplementedException();

                            }
                            if (component != null) this.AddComponent(component);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Incorrect magic number in file header.");
                        try_v0 = true;
                    };
                }
            }
            else
            {
                Console.WriteLine("Error! {0} does not exist", path);
            }
            if (try_v0)
            {
                Console.WriteLine("Trying to load model data as version 0 format...");
                this.LoadModelV0(path);
            }
        }
        // Loads models from the original model file version
        // this version only supported 3 layers with ReLU activation between them
        public void LoadModelV0(string path)
        {
            if (File.Exists(path))
            {
                Console.WriteLine("Loading V0 format Model {0}.", path);
                using (BinaryReader binReader = new BinaryReader(File.Open(path, FileMode.Open)))
                {
                    int layers = binReader.ReadInt32();
                    this.epoch = binReader.ReadInt32();
                    this.version = 0;
                    int layer1_inputs = binReader.ReadInt32();
                    int layer1_neurons = binReader.ReadInt32();
                    int layer2_neurons = binReader.ReadInt32();
                    int layer3_neurons = binReader.ReadInt32();

                    Layer layer1 = new DenseLayer(layer1_inputs, layer1_neurons);
                    ActivationFunction activation1 = new ReLU(layer1_neurons);
                    Layer layer2 = new DenseLayer(layer1_neurons, layer2_neurons);
                    ActivationFunction activation2 = new ReLU(layer2_neurons);
                    Layer layer3 = new DenseLayer(layer2_neurons, layer3_neurons);
                    layer1.ReadFile(binReader, 0);
                    layer2.ReadFile(binReader, 0);
                    layer3.ReadFile(binReader, 0);

                    // Build the model
                    this.components.Clear();
                    this.AddComponent(layer1);
                    this.AddComponent(activation1);
                    this.AddComponent(layer2);
                    this.AddComponent(activation2);
                    this.AddComponent(layer3);
                    this.loss_activation = new ActivationSoftmaxLossCategoricalCrossentropy(layer3_neurons);
                }
                Console.WriteLine("Done.");
            }
            else
            {
                Console.WriteLine("Error! {0} does not exist", path);
            }
        }
        public void BuildDefault()
        {
            this.components.Clear();
            int layer1_inputs = 784; // don't change this, this corresponds to the pixel sof the 28x28 image
            int layer1_neurons = 200;
            int layer2_neurons = 100;
            int layer3_neurons = 10;   // don't change this; it corresponds to the number of digits to classify
            this.AddComponent(new DenseLayer(layer1_inputs, layer1_neurons));
            this.AddComponent(new ReLU(layer1_neurons));
            this.AddComponent(new DenseLayer(layer1_neurons, layer2_neurons));
            this.AddComponent(new ReLU(layer2_neurons));
            this.AddComponent(new DenseLayer(layer2_neurons, layer3_neurons));
            this.loss_activation = new ActivationSoftmaxLossCategoricalCrossentropy(layer3_neurons);
        }
        public override string ToString()
        {
            string s = "Model {\n";
            s += "   version = " + this.version + "\n";
            s += "   epoch = " + this.epoch + "\n";
            s += "   components (" + this.components.Count + ") {\n";
            foreach (ModelComponent c in this.components)
            {
                s += "         " + c + "\n";
            }
            s += "   }\n";
            if (version <= 1.0)
            {
                s += "   last_layer_activation = SOFTMAX/CATEGORICAL CROSS ENTROPY\n";
            }
            s += "}\n";

            return s;
        }
    }
    class OptimizerSGD
    {
        Random rnd;
        public double starting_learning_rate = 1.0;
        public double current_learning_rate = 0.01;
        public double learning_rate_decay = 0.001;
        public int batch_size = 32;
        public double momentum = 0.5;

        int[] samples;
        double[,] inputs;
        double[,] class_targets;

        List<double[]> history;
        public OptimizerSGD()
        {
            rnd = new Random();
            history = new List<double[]>();
        }


        public int[] GenerateRandomValues(uint n_samples, int max_n)
        {
            int[] rv = new int[n_samples];
            for (int i = 0; i < n_samples; i++)
            {
                rv[i] = rnd.Next(0, max_n);
            }
            return rv;
        }
        public void RandomizeBatch(mnist m)
        {
            this.samples = this.GenerateRandomValues((uint)this.batch_size, (int)m.n_images);
            this.inputs = m.GetBatchSamples(samples);
            this.class_targets = m.GetBatchClassifications(samples);
        }
        public void Train(Model model, string logPath, int printEvery = 100)
        {
            model.epoch++;
            this.current_learning_rate = this.starting_learning_rate * (1 / (1 + this.learning_rate_decay * model.epoch));
            model.Forward(this.inputs);
            double loss = model.CalculateLoss(this.class_targets);
            double acc = model.CalculateAccuracy(this.class_targets);
            double[] row = { model.epoch, loss, acc, this.current_learning_rate };
            this.history.Add(row);


            if (model.epoch % printEvery == 0)
            {
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    foreach (double[] r in this.history)
                    {
                        sw.WriteLine("{0}, {1}, {2}, {3}", r[0], r[1], r[2], r[3]);
                    }
                }
                double lossSum = 0;
                double accSum = 0;
                foreach (double[] r in this.history)
                {
                    lossSum += r[1];
                    accSum += r[2];
                }
                double mean_loss = lossSum / (this.history.Count);
                double mean_acc = accSum / (this.history.Count);
                this.history.Clear();



                Console.WriteLine("Epoch: {0}, loss: {1}, acc: {2}, lr: {3}", model.epoch, mean_loss.ToString("0.####"),
                    mean_acc.ToString("0.####"), this.current_learning_rate.ToString("0.######"));
            }
            model.Backward(this.class_targets, this.current_learning_rate);

            foreach (ModelComponent c in model.components)
            {
                if (c is Layer) this.UpdateParams(c as Layer);
            }
        }
        public void UpdateParams(Layer l)
        {
            double[,] weight_updates = new double[0, 0];
            double[,] bias_updates = new double[0, 0];
            if (this.momentum > 0)
            {
                // momentum is a retain factor that is multiplied by previous updates and updates with current gradient
                // aids in overcoming local minima

                if (!l.momentum_started)
                {
                    // no history of momentums yet, start one
                    l.weight_momentums = matrix.zeros_like(l.weights);
                    l.bias_momentums = matrix.zeros_like(l.biases);
                    l.momentum_started = true;
                }
                weight_updates = matrix.add(matrix.scale(this.momentum, l.weight_momentums),
                                                matrix.scale(-this.current_learning_rate, l.dweights));
                l.weight_momentums = weight_updates;
                bias_updates = matrix.add(matrix.scale(this.momentum, l.bias_momentums),
                                                matrix.scale(-this.current_learning_rate, l.dbiases));
                l.bias_momentums = bias_updates;


            }
            else
            {
                weight_updates = matrix.scale(-this.current_learning_rate, l.dweights);
                bias_updates = matrix.scale(-this.current_learning_rate, l.dbiases);
            }
            l.weights = matrix.add(l.weights, weight_updates);
            l.biases = matrix.add(l.biases, bias_updates);


        }

    }
}