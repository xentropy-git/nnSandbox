using System;
using System.IO;
using System.Collections.Generic;


namespace neuralnet
{
    class Program
    {
    
        static void Main(string[] args)
        {
            // Variables
            const string model_file = "model.bin";
            const string pretrained_model_file = "models/pretrained.bin";
            const string logPath = "log.csv";
           
            matrix.unit_tests();
            mnist m = new mnist();
            OptimizerSGD sgd = new OptimizerSGD();
            m.Load();
            
            // New model class
            Model model = new Model();

            // build default model
            model.BuildDefault();

            if (File.Exists(model_file))
            {
                Console.WriteLine("Model file '{0}' FOUND.  Would you like to load it? (y/n)", model_file);
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    Console.WriteLine("");
                    model.Load(model_file);
                    Console.WriteLine(model);
                }
                Console.WriteLine("");
            }
            MainMenu();
 
  
            void MenuOptionAccuracyTest()
            {
                Console.WriteLine("Warning!  This may take several minutes as the accuracy test runs forward predictions on all 10k test samples.");
                if (InputYesNo("Would you like to continue?"))
                {
                    Console.WriteLine("");
                    Console.WriteLine("Running...");
                    double[,] test_data = m.GetTestSamples();
                    double[,] test_labels = m.GetTestLabels();
                    model.Forward (test_data);
                    double loss = model.CalculateLoss(test_labels);
                    double acc = model.CalculateAccuracy(test_labels);
                    Console.WriteLine("...Done!");
                    Console.WriteLine("The accuracy of this model is {0}%", (acc*100).ToString("0.####"));
                    Console.WriteLine("Press any key to continue.");
                    Console.ReadKey();
                    Console.WriteLine("");
                }
            }
 
            void MenuOptionsRunPredictions()
            {
                int r_img = 0;
                bool keepRunning2 = true;
                while (keepRunning2 == true)
                {
                    int[] sample = { r_img };
                    m.DisplayImageInConsole((uint)r_img);
                  
                    (int argMaxA, double valueA) = matrix.argmax( model.Forward(m.GetBatchSamples(sample)), 0 );
                    Console.WriteLine("Displaying image {0}/{1}", (uint)r_img, m.n_images);
                    Console.WriteLine("Predicted Label: {0} (Confidence = {1}%)", argMaxA, (valueA*100).ToString("0.##"));
                    Console.WriteLine("Actual Label: '{0}'", m.labelData[r_img]);                  
                    Console.WriteLine("Navigation: (G)oto Image, (+/-) Go forward or backward, (ESC) Escape");

                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.G:
                            Console.WriteLine("");
                            r_img = InputInt32("Choose an image number.", 0, 59999);
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
            void MenuOptionConfigureOptimizer()
            {
                bool keepRunning = true;
                while (keepRunning == true)
                {
                    ConsoleKey[] options = {ConsoleKey.L,
                                            ConsoleKey.D,
                                            ConsoleKey.B,
                                            ConsoleKey.M,
                                            ConsoleKey.Escape};
                    string[] optionsText = {"Starting Learning Rate = " + sgd.starting_learning_rate,
                                            "Learning Rate Decay = " + sgd.learning_rate_decay, 
                                            "Batch Size = " + sgd.batch_size,
                                            "Momentum = " + sgd.momentum,
                                            "Escape"};
                    ConsoleKey k = InputMenuOption ("Select an option to change:",
                        options,
                        optionsText);
                    switch (k)
                    {
                        case ConsoleKey.Escape:
                            keepRunning = false;
                            break;
                        case ConsoleKey.L:
                            sgd.starting_learning_rate = InputDouble("Enter new starting learning rate:", 0.0, 2.0);
                            break;
                        case ConsoleKey.D:
                            sgd.learning_rate_decay = InputDouble("Enter new learning rate decay:", 0.0, 1.0);
                            break;
                        case ConsoleKey.B:
                            sgd.batch_size = InputInt32("Enter new batch size", 1, 128);
                            break;
                        case ConsoleKey.M:
                            sgd.momentum = InputDouble("Enter new momentum", 0, 1);
                            break;
                    }
                }
            }
            void MenuOptionRunOptimizer()
            {             
                if (!File.Exists(logPath))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(logPath))
                    {
                        sw.WriteLine("epoch, loss, acc, lr");
                    }
                }
                Console.WriteLine("Optimizer STARTED.");
                Console.WriteLine("Press ESC at any time to stop training.");
                do
                {
                    while (!Console.KeyAvailable)
                    {
                        sgd.RandomizeBatch(m);
                        sgd.Train(model);
                        if (model.epoch % 100 == 0)
                        {
                            sgd.LogHistory(logPath);
                            (int epoch, double loss, double acc, double lr) = sgd.GetRollingAverage();
                            Console.WriteLine("Epoch: {0}, loss: {1}, acc: {2}, lr: {3}", epoch, loss.ToString("0.####"),
                                acc.ToString("0.####"), lr.ToString("0.######"));

                        }
                    }
                } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
                Console.WriteLine("Optimizer STOPPED.");

                if (InputYesNo("Would you like to save this model?"))
                {
                    Console.WriteLine("Saving Model.");
                    model.Save(model_file);
                }
                Console.WriteLine("");
            }
            
            bool InputYesNo (string prompt)
            {
                Console.WriteLine(prompt + " (y/n)");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    Console.WriteLine("");
                    return true;
                } else {
                    Console.WriteLine("");
                    return false;
                }
            }

            double InputDouble (string prompt, double min_value, double max_value)
            {
                double value = 0;
                bool success = false;
                while (success == false)
                {
                    Console.WriteLine(prompt + " ("+ min_value + " to " + max_value+")");
                    string entry = Console.ReadLine();
                    try
                    {
                        value = Convert.ToDouble(entry);
                        if (value >= min_value && value <= max_value)
                        {
                            success = true;
                        }
                        else
                        {
                            Console.WriteLine("Value out of range.");
                            success = false;
                        }
                        
                    }
                    catch (System.FormatException)
                    {
                        success = false;
                        Console.WriteLine("Value not recognized as double (" + entry +").  Try again.");
                    }
                }
                return value;
            }
            int InputInt32 (string prompt, int min_value, int max_value)
            {
                int value = 0;
                bool success = false;
                while (success == false)
                {
                    Console.WriteLine(prompt + " ("+ min_value + " to " + max_value+")");
                    string entry = Console.ReadLine();
                    try
                    {
                        value = Convert.ToInt32(entry);
                        if (value >= min_value && value <= max_value)
                        {
                            success = true;
                        }
                        else
                        {
                            Console.WriteLine("Value out of range.");
                            success = false;
                        }
                        
                    }
                    catch (System.FormatException)
                    {
                        success = false;
                        Console.WriteLine("Value not recognized as integer (" + entry +").  Try again.");
                    }
                }
                return value;
            }

            ConsoleKey InputMenuOption (string prompt, ConsoleKey [] options, string [] optionText)
            {
                ConsoleKey key = ConsoleKey.NoName;
                while (key == ConsoleKey.NoName)
                {
                    Console.WriteLine(prompt);
                    for (int n = 0; n < options.GetUpperBound(0)+1; n++)
                    {
                        Console.WriteLine(options[n].ToString()+") "+optionText[n]);
                    }
                    key = Console.ReadKey().Key;
                    Console.WriteLine("");
                    for (int n = 0; n < options.GetUpperBound(0)+1; n++)
                    {
                        if (key == options[n]) return key;
                    }
                    Console.WriteLine("Invalid option.  Please try again.");
                    key = ConsoleKey.NoName;
                }
                return key;
                
            }
            void MenuOptionNewModel()
            {
                Console.WriteLine ("Building a new model.");
                model = new Model();
                int layers = InputInt32("How many layers would you like?", 2, 10);
                int last_layer_neurons = -1;
                for (int i = 0; i < layers; i++)
                {
                    Console.WriteLine("Creating layer "+i);
                    int inputs = -1;
                    if (i == 0) {
                        Console.WriteLine("Warning: It is strongly suggested that you use the default input size of 784 for the MNIST dataset, since this determined by the image size.  Only enter a different value if you have altered the code to use another dataset.");
                        if (InputYesNo("Would you like to accept the default input size of 784?"))
                        {
                            inputs = 784;
                        }
                    }
                    if (inputs == -1 && last_layer_neurons != -1) inputs = last_layer_neurons;
                    if (inputs == -1)
                    {
                        inputs = InputInt32("Please enter the number of INPUTS for layer " + i, 1, 999999);
                    }
                    int neurons = -1;
                    if (i == layers-1)
                    {
                        Console.WriteLine("Warning: It is strongly suggested that you use the default neuron size of 10, since there are 10 digits to classify.  Only enter a different value if you have altered the code to use another dataset.");
                        if (InputYesNo("Would you like to accept the default neuron size of 10?"))
                        {
                            neurons = 10;
                        }
                    }
                    if (neurons == -1)
                    {
                        neurons = InputInt32("Please enter the number of NEURONS for layer " + i, 1, 999999);
                    }
                    last_layer_neurons = neurons;
                    Layer l = new DenseLayer(inputs, neurons);
                    model.AddComponent (l);
                    if (i < layers -1)
                    {
                        ConsoleKey[] options = {ConsoleKey.R, ConsoleKey.S};
                        string[] optionsText = {"ReLU", "Sigmoid"};
                        ConsoleKey k = InputMenuOption ("Please select an activation function for layer "+i,
                            options,
                            optionsText);
                        switch (k)
                        {
                            case ConsoleKey.R:
                                Console.WriteLine("Adding ReLU activation function.");
                                model.AddComponent(new ReLU(last_layer_neurons));
                                break;
                            case ConsoleKey.S:
                                Console.WriteLine("Adding Sigmoid activation function.");
                                model.AddComponent(new Sigmoid(last_layer_neurons));
                                break;
                        }
                    }
                }
                Console.WriteLine("Adding Softmax activation and Categorical Crossentropy loss [Nothing else is implemented yet].");
                model.loss_activation = new ActivationSoftmaxLossCategoricalCrossentropy(last_layer_neurons);
                Console.WriteLine("Done creating model.");
                Console.WriteLine(model);
                
            }

            // Main loop
            void MainMenu()
            {
                bool keepRunning = true;
                while (keepRunning == true)
                {
                    ConsoleKey[] options = {ConsoleKey.C, 
                                            ConsoleKey.O,
                                            ConsoleKey.P,
                                            ConsoleKey.A,
                                            ConsoleKey.L,
                                            ConsoleKey.N,
                                            ConsoleKey.Escape};
                    string[] optionsText = {"Configure optimizer",
                                            "Run the optimizer", 
                                            "Run predictions",
                                            "Accuracy test",
                                            "Load model "+pretrained_model_file,
                                            "New model",
                                            "Escape"};
                    ConsoleKey k = InputMenuOption ("Please choose an option from the menu:",
                        options,
                        optionsText);
                    switch (k)
                    {
                        case ConsoleKey.Escape:
                            keepRunning = false;
                            break;
                        case ConsoleKey.C:
                            MenuOptionConfigureOptimizer();
                            break;
                        case ConsoleKey.O:
                            MenuOptionRunOptimizer();
                            break;
                        case ConsoleKey.L:
                            model.Load(pretrained_model_file);
                            Console.WriteLine(model);
                            break;
                        case ConsoleKey.A:
                            MenuOptionAccuracyTest();
                            break;
                        case ConsoleKey.P:
                            MenuOptionsRunPredictions();
                            break;
                        case ConsoleKey.N:
                            MenuOptionNewModel();
                            break;
                    }
                }
            }
            
            Console.WriteLine("");
            Console.WriteLine("Press any key to exit.");
            
            Console.ReadKey();
        }
    }
}
