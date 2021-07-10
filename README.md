# What is nnSandbox?
nnSandbox is a fully functional neural network framework and application for defining, training, and testing models on the MNIST handwritten digits dataset.  It is written from scratch in C# with Microsoft Visual Studio 16.6.2 with .NET Framework 4.9.04084.  It is intended for educational purposes only.  Information on the dataset can be found here: http://yann.lecun.com/exdb/mnist/.
# What ISN’T nnSandbox?
nnSandbox is not a fully featured framework.  It supports only handful activation functions (pretty much only what is necessary to achieve decent accuracy the handwritten dataset).  It is not optimized for speed or parallel processing.  
# What is included?
There are two solutions to the project: neuralnet1 and nnGui.  Neuralnet1 is a console application.  It is less user friendly, but it gets the job done.  It is much easier to understand the code in Neuralnet1, so I suggest starting there if you are trying to figure out how the code woks.  nnGui is a Windows Forms application that provides a graphical user interface to for defining, training, and testing models.
## Dataset
* Training data – 60,000 samples
  * data/train-labels.idx1-ubyte
  * data/train-images.idx3-ubyte
* Test data – 10,000 samples
  * data/t10k-labels.idx1-ubyte
  * data/t10k-images.idx3-ubyte

## Pretrained models
* 800h7000e.bin – 97.47% accuracy
  * 2 hidden layers (500x300) with ReLU activation.
  * Trained for 7000 epochs.
  * Biggest – 4040kb
* 300h4000e.bin – 91.65% accuracy
  * 2 hidden layers (200x100) with ReLU activation.
  * Trained for x epochs.
  * Medium – 1390kb
* 35h9000e.bin – 40.69% accuracy
  * 2 hidden layers (25x10) with ReLU activation.
  * Trained for 9000 epochs.
  * Smallest – 146kb

## nnGui
### Dataset Browser
![Dataset Browser Screenshot](https://github.com/Gambrivius/nnSandbox/blob/master/screenshots/dataset_browser.PNG?raw=true "Dataset Browser")

The dataset browser tab provides a useful tool for browsing the dataset.  Navigate through the dataset’s 60000 samples using the scroll bar under the image.  The image displays the 28x28 single channel image at that position in the dataset.  It also runs a forward pass of your model on that data and displays the prediction and confidence scores.
### SGD Optimizer
![Optimizer Screenshot](https://github.com/Gambrivius/nnSandbox/blob/master/screenshots/optimizer.PNG?raw=true "Optimizer")

The SGD Optimizer tab is the heart of the application.  This tab provides an interface to start and stop training the model using Stochastic Gradient Descent, SGD for short.  SGD is a fancy way of saying that the optimizer uses random mini-batches of sample data on each iteration or epoch of the training rather than the entire dataset.  The model’s parameters are then adjusted by their gradients multiplied by the learning rate.  The learning rate can be reduced over the course of training automatically by utilizing the learning rate decay.  Momentum is a feature that allows the optimizer to overcome local minima in the loss function.  It adjusts the parameters by a fraction of the rolling averages of the parameter’s gradients.  The momentum option specifies the fraction of the rolling average to “keep.”  A momentum of 0 disables this feature.
Clicking start will launch the optimizer’s background worker thread.  While the optimizer is running, all   other options on the application are disabled.  You can click stop at any time to stop the optimizer.
Every 100 iterations, the GUI will update with a chart of the loss, accuracy, and learning rate of the model.  Additionally, the parameters of the weights and biases for each layer are displayed in a colorful pictogram.  The values are normalized and then interpolated between red, white, and blue.  Red represents the lowest values and blue represents the highest values, with white being in the middle.
### Model Design
![Model Design Screenshot](https://github.com/Gambrivius/nnSandbox/blob/master/screenshots/model_design.PNG?raw=true "Model Design")

The model design tab provides an interface for defining, saving, and loading models.  It will force your first layer to have 728 inputs, but it won't force your last layer to have 10 neurons.  It is critically important that your last layer have 10 neurons because this will feed the final activation function, which is softmax.  Softmax generates a probability distribution.  The 10 neurons correspond with the 10 possible classes of digits (0-9).  You can use the buttons on the right to build your model, choosing the number of neurons for each hidden layer, as well as determining which activation function to use.  When you are done, you can go back to the optimizer and train your model.  Also in this tab, you may save or load models using the buttons below.  This will prompt a file dialog menu to select the model file you wish to save or load.
# NNFS
In developing this project, the book Neural Networks from Scratch in Python (https://nnfs.io/) by Harrison Kinsley & Daniel Kukiela was an indispensable tool.  The book breaks down tough subjects like performing gradient computations.  I can’t recommend this book enough.  Even though the book targets the Python language, the concepts are general enough to be applied to any language (even Scratch).
