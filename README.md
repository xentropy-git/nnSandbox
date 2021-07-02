If you previously cloned this based on my announcement of a working version, please reclone it and try it again.  I had a push failure.  The code is now up to date but I lost my readme file.  Stay tuned for better documentation.

# nnSandbox
From scratch implementation of a neural network in c#


This project is for educational purposes only.  It is not intended for practical use as it will not be optimized to take advantage of parallel processing or use any libraries for performance.

This is a digit-classification network for hand-written digits based on the MNIST handwritten dataset provided here: http://yann.lecun.com/exdb/mnist/
It is built from the ground up without the aid of any advanced machine learning or data science libraries, and is intended to be an exercise in
understanding the fundamentals of machine learning rather than an actual full-scale implementation of a neural network framework.

The program can currently load the MNIST binary dataset and display the images/labels in the console.  It also implements a basic dense neuron layer but doesn't yet
implement any of the forward feeding or back propagation learning.  More to come!
Christopher Laponsie 4/21/2021
