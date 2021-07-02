using System;
using System.IO;

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
            
            int tsamples = y_true.GetUpperBound(0)+1;
            int tlabels = y_true.GetUpperBound(1)+1;
            

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
        public void WriteFile (BinaryWriter binWriter)
        {
            for (int i = 0; i < this.weights.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < this.weights.GetUpperBound(1) + 1; j++)
                {
                    binWriter.Write(this.weights[i, j]);
                }

            }

            for (int i = 0; i < this.biases.GetUpperBound(0) + 1; i++)
            {
                binWriter.Write(this.biases[i]);
            }

        }
        public void ReadFile(BinaryReader binReader)
        {
            for (int i = 0; i < this.weights.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < this.weights.GetUpperBound(1) + 1; j++)
                {
                    this.weights[i, j] = binReader.ReadDouble();
                }

            }

            for (int i = 0; i < this.biases.GetUpperBound(0) + 1; i++)
            {
                this.biases[i] = binReader.ReadDouble();
            }

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
            this.dweights = matrix.dot (matrix.transpose(this.inputs), dvalues);
            this.dbiases = matrix.sum (dvalues, 0); 
            this.dinputs = matrix.dot(dvalues, matrix.transpose(this.weights));         
            
        }

        public void UpdateParams (double learning_rate = 0.01) {
           this.weights = matrix.add(this.weights, matrix.scale (-learning_rate, this.dweights));
        }
    }
    class SGD
    {
        Random rnd;
        double learning_rate;
        public SGD (double lr)
        {
            this.learning_rate = lr;
            rnd = new Random();
        }
        public int[] RandomSamples (uint n_samples, int max_n) {
            int[] rv = new int[n_samples];
            for (int i = 0; i < n_samples; i++) {
                rv[i] = rnd.Next(0, max_n);
            }
            return rv;
        }
    }
    
    class Program
    {
    
        static void Main(string[] args)
        {
            // Variables
            double learning_rate = 0.01;
            double learning_rate_decay = 0; //1e-7;
            const string model_file = "model.bin";
            const string pretrained_model_file = "data/pretrained.bin";
            string logPath = "log.csv";
            uint batch_size = 32;
            int epoch = 0;

            // Model Constants
            int layers = 3;  // WARNING, this version only supports 3 layers.  Changing this number will break the program
            int layer1_inputs = 784; // don't change this, this corresponds to the pixel sof the 28x28 image
            int layer1_neurons = 200;
            int layer2_neurons = 100;
            int layer3_neurons = 10;   // don't change this; it corresponds to the number of digits to classify

           
            matrix.unit_tests();
            mnist m = new mnist();
            SGD sgd = new SGD(0.01);

            int[] samples = sgd.RandomSamples(5, (int)m.n_images);
            m.Load();
            double[,] inputs = m.GetBatchSamples(samples);
            double[,] class_targets = m.GetBatchClassifications(samples);

            
            // build the model
            DenseLayer layer1 = new DenseLayer(layer1_inputs, layer1_neurons);
            ReLU activation1 = new ReLU(layer1_neurons);
            DenseLayer layer2 = new DenseLayer(layer1_neurons, layer2_neurons);
            ReLU activation2 = new ReLU(layer2_neurons);
            DenseLayer layer3 = new DenseLayer(layer2_neurons, layer3_neurons);
            ActivationSoftmaxLossCategoricalCrossentropy loss_activation = new ActivationSoftmaxLossCategoricalCrossentropy(layer3_neurons);


            void LoadModel(string path = model_file)
            {
                if (File.Exists(path))
                {
                    Console.WriteLine("Loading Model {0}.", path);
                    using (BinaryReader binReader = new BinaryReader(File.Open(path, FileMode.Open)))
                    {
                        layers = binReader.ReadInt32();
                        epoch = binReader.ReadInt32();
                        layer1_inputs = binReader.ReadInt32();
                        layer1_neurons = binReader.ReadInt32();
                        layer2_neurons = binReader.ReadInt32();
                        layer3_neurons = binReader.ReadInt32();
                        Console.WriteLine("Model shape ({0} layers):", layers);
                        Console.WriteLine("Inputs: {0}", layer1_inputs);
                        Console.WriteLine("Layer 1 Neurons: {0}", layer1_neurons);
                        Console.WriteLine("Layer 2 Neurons: {0}", layer2_neurons);
                        Console.WriteLine("Layer 3 Neurons: {0}", layer3_neurons);
                        layer1 = new DenseLayer(layer1_inputs, layer1_neurons);
                        activation1 = new ReLU(layer1_neurons);
                        layer2 = new DenseLayer(layer1_neurons, layer2_neurons);
                        activation2 = new ReLU(layer2_neurons);
                        layer3 = new DenseLayer(layer2_neurons, layer3_neurons);
                        loss_activation = new ActivationSoftmaxLossCategoricalCrossentropy(layer3_neurons);
                        Console.WriteLine("Loading weights and biases.");
                        layer1.ReadFile(binReader);
                        layer2.ReadFile(binReader);
                        layer3.ReadFile(binReader);
                    }
                    Console.WriteLine("Done.");
                } else
                {
                    Console.WriteLine("Error! {0} does not exist", path);
                }
            }
            if (File.Exists(model_file))
            {
                Console.WriteLine("Model file '{0}' FOUND.  Would you like to load it? (y/n)", model_file);
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    Console.WriteLine("");
                    LoadModel();


                }
                Console.WriteLine("");
            }
 
            void ForwardAll(double[,] inp) {
                layer1.Forward(inp);
                activation1.Forward(layer1.output);
                layer2.Forward(activation1.output);
                activation2.Forward(layer2.output);
                layer3.Forward(activation2.output);
            }

            void GetRandomSamples() {
                samples = sgd.RandomSamples(batch_size, (int)m.n_images);
                inputs = m.GetBatchSamples(samples);
                class_targets = m.GetBatchClassifications(samples);
            }
            void AccuracyTest()
            {
                Console.WriteLine("Warning!  This may take several minutes as the accuracy test runs forward predictions on all 10k test samples.");
                Console.WriteLine("Would you like to continue? (y/n)");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Running...");
                    double[,] test_data = m.GetTestSamples();
                    double[,] test_labels = m.GetTestLabels();
                    ForwardAll(test_data);
                    double loss = loss_activation.Forward(layer3.output, test_labels);
                    double sum_correct = 0;
                    for (int j = 0; j < layer3.GetRows(); j++)
                    {
                        (int argMaxA, double valueA) = matrix.argmax(loss_activation.output, j);
                        (int argMaxB, double valueB) = matrix.argmax(test_labels, j);

                        if (argMaxA == argMaxB) sum_correct++;
                    }
                    double acc = sum_correct / layer3.GetRows();
                    Console.WriteLine("...Done!");
                    Console.WriteLine("The accuracy of this model is {0}%", (acc*100).ToString("0.####"));
                    Console.WriteLine("Press any key to continue.");
                    Console.ReadKey();
                    Console.WriteLine("");
                }
            }
            double GetAccuracy() {
                double sum_correct = 0;
                for (int j = 0; j < layer3.GetRows(); j++)
                {
                    (int argMaxA, double valueA) = matrix.argmax(loss_activation.output, j);
                    (int argMaxB, double valueB) = matrix.argmax(class_targets, j);
                    
                    if(argMaxA == argMaxB) sum_correct++;
                }
                double acc = sum_correct / layer3.GetRows();
                return acc;
            }

            void PredictionLoop()
            {
                int r_img = 0;
                

                bool keepRunning2 = true;

                while (keepRunning2 == true)
                {
                    int[] sample = { r_img };

                    ForwardAll(m.GetBatchSamples(sample));
                    double loss = loss_activation.Forward(layer3.output, class_targets);
                    m.DisplayImageInConsole((uint)r_img);
                    (int argMaxA, double valueA) = matrix.argmax(loss_activation.output, 0);
                    Console.WriteLine("Displaying image {0}/{1}", (uint)r_img, m.n_images);
                    Console.WriteLine("Predicted Label: {0} (Confidence = {1}%)", argMaxA, (valueA*100).ToString("0.##"));
                    Console.WriteLine("Actual Label: '{0}'", m.labelData[r_img]);
                    
                    Console.WriteLine("Navigation: (G)oto Image, (+/-) Go forward or backward, (ESC) Escape");

                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.G:
                            Console.WriteLine("");
                            Console.WriteLine("Choose an image number between 0 and 60000");

                            int newImage = Convert.ToInt32(Console.ReadLine());
                            if (newImage >= 0 && newImage < 60000)
                            {
                                r_img = newImage;
                            } else
                            {
                                Console.WriteLine("Number out of range.");
                            }
                            
                            
                            break;
                        case ConsoleKey.Escape: 
                            Console.WriteLine("");
                            keepRunning2 = false;
                            break;
                        case ConsoleKey.Add:
                        case ConsoleKey.OemPlus:
                        case ConsoleKey.RightArrow:
                            Console.WriteLine("");
                            r_img++;
                            if (r_img >= 60000) r_img -= 60000;
                            break;
                        case ConsoleKey.Subtract:
                        case ConsoleKey.OemMinus:
                        case ConsoleKey.LeftArrow:
                            Console.WriteLine("");
                            r_img--;
                            if (r_img < 0) r_img += 60000;
                            break;
                        default:
                            Console.WriteLine("");
                            Console.WriteLine("Invalid Option.");
                            break;

                    }
                }
            }
            void OptimizerLoop()
            {
                
                
                if (!File.Exists(logPath))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(logPath))
                    {
                        sw.WriteLine("epoch, loss, acc, lr");
                    }
                }
                double lossSum = 0;
                double accSum = 0;
                Console.WriteLine("Optimizer STARTED.");
                int s = 0;
                do
                {
                    while (!Console.KeyAvailable)
                    {
                        epoch++;
                        learning_rate = learning_rate * (1 / (1 + learning_rate_decay * epoch));
                        GetRandomSamples();
                        ForwardAll(inputs);
                        double loss = loss_activation.Forward(layer3.output, class_targets);
                        double acc = GetAccuracy();
                        lossSum += loss;
                        accSum += acc;
                        s++;

                        if (epoch % 100 == 0 || epoch == 0)
                        {

                            if (epoch > 0)
                            {
                                loss = lossSum / s;
                                acc = accSum / s;
                                lossSum = 0;
                                accSum = 0;
                                s = 0;
                            }
                            using (StreamWriter sw = File.AppendText(logPath))
                            {
                                sw.WriteLine("{0}, {1}, {2}, {3}", epoch, loss, acc, learning_rate);
                            }
                            
                            Console.WriteLine("Epoch: {0}, loss: {1}, acc: {2}, lr: {3}", epoch, loss.ToString("0.####"), acc.ToString("0.####"), learning_rate.ToString("0.######"));

                        }


                        loss_activation.Backward(loss_activation.output, class_targets);
                        layer3.Backward(loss_activation.dinputs);
                        activation2.Backward(layer3.dinputs);
                        layer2.Backward(activation2.dinputs);
                        activation1.Backward(layer2.dinputs);
                        layer1.Backward(activation1.dinputs);

                        layer3.UpdateParams(learning_rate);
                        layer2.UpdateParams(learning_rate);
                        layer1.UpdateParams(learning_rate);
                    }
                } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
                Console.WriteLine("Optimizer STOPPED.");



                Console.WriteLine("Would you like to save this model? (y/n)");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Saving Model.");
                    using (BinaryWriter binWriter = new BinaryWriter(File.Open(model_file, FileMode.Create)))
                    {
                        binWriter.Write(layers);
                        binWriter.Write(epoch);
                        binWriter.Write(layer1_inputs);
                        binWriter.Write(layer1_neurons);
                        binWriter.Write(layer2_neurons);
                        binWriter.Write(layer3_neurons);
                        layer1.WriteFile(binWriter);
                        layer2.WriteFile(binWriter);
                        layer3.WriteFile(binWriter);
                    }
                }
                Console.WriteLine("");
            }


            void NewModel()
            {
                Console.WriteLine("Enter number of layer 1 neurons.  (200 is default)");
                string entry = Console.ReadLine();
                int n_layer1_neurons = 200;
                try
                {
                    n_layer1_neurons = Convert.ToInt32(entry);
                }
                catch (System.FormatException e)
                {
                    n_layer1_neurons = 200;
                }
                if (n_layer1_neurons == 0) n_layer1_neurons = 200;
                Console.WriteLine("Layer 1 Neurons = {0}", n_layer1_neurons);
                Console.WriteLine("Enter number of layer 2 neurons.  (100 is default)");
                entry = Console.ReadLine();
                int n_layer2_neurons = 100;
                try
                {
                    n_layer2_neurons = Convert.ToInt32(entry);
                }
                catch (System.FormatException e)
                {
                    n_layer2_neurons = 100;
                }
                if (n_layer2_neurons == 0) n_layer2_neurons = 100;
                Console.WriteLine("Layer 2 Neurons = {0}", n_layer2_neurons);

                if (n_layer1_neurons > 0 && n_layer2_neurons > 0)
                {
                    layer1_neurons = n_layer1_neurons;
                    layer2_neurons = n_layer2_neurons;
                    Console.WriteLine("Creating new model with random weights and biases.");

                    layer1 = new DenseLayer(layer1_inputs, layer1_neurons);
                    activation1 = new ReLU(layer1_neurons);
                    layer2 = new DenseLayer(layer1_neurons, layer2_neurons);
                    activation2 = new ReLU(layer2_neurons);
                    layer3 = new DenseLayer(layer2_neurons, layer3_neurons);
                    loss_activation = new ActivationSoftmaxLossCategoricalCrossentropy(layer3_neurons);
                    Console.WriteLine("Done.");
                }
                else
                {
                    Console.WriteLine("Invalid network size.");
                }
            }

            // Main loop
            bool keepRunning = true;

            while (keepRunning == true)
            {
                Console.WriteLine("Please choose an option from the method:");
                Console.WriteLine("O) Run the SGD optimizer");
                Console.WriteLine("P) Run predictions");
                Console.WriteLine("A) Accuracy Test");
                Console.WriteLine("L) Load model {0}", pretrained_model_file);
                Console.WriteLine("N) New model");
                Console.WriteLine("ESC) Escape");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Escape:
                        Console.WriteLine("");
                        keepRunning = false;
                        break;
                    case ConsoleKey.O:
                        Console.WriteLine("");
                        OptimizerLoop();
                        break;
                    case ConsoleKey.L:
                        Console.WriteLine("");
                        LoadModel(pretrained_model_file);
                        break;
                    case ConsoleKey.A:
                        Console.WriteLine("");
                        AccuracyTest();
                        break;
                    case ConsoleKey.P:
                        Console.WriteLine("");
                        PredictionLoop();
                        break;
                    case ConsoleKey.N:
                        Console.WriteLine("");
                        NewModel();
                        break;
                    default:
                        Console.WriteLine("");
                        Console.WriteLine("Invalid Option.");
                        break;

                }
            }

            
            Console.WriteLine("Press any key to exit.");
            
            Console.ReadKey();
        }
    }
}
