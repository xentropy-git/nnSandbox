/*
 * This file contains classes used to load the mnist handwritten digit dataset.
 * http://yann.lecun.com/exdb/mnist/
 * train-images-idx3-ubyte contains 60000 images encoded as follows
 * 1) first 4 bytes is a 32 bit uint in high endian magic number equal to 2051
 * 2) the next 4 bytes is a 32 bit uint in high endian representing number of images in dataset (60000)
 * 3) the next 4 bytes are the number of rows in each image (28)
 * 4) the next 4 bytes are the number of columns in each image (28)
 * 5) the remaining bits are just a stream of bits, row by row, of the images of the dataset
 *      ie: 28*28 = 784, so each sequance of 784 bits represents one image.  we assemble these into 28x28 matrices of bytes
 *      output is a 60000x28x28 array from bytes which will later be converted to floats for the network to use as inputs
 *      
 *      
 *  Once loaded, this data is not immediately ready for the neural network.  It must be scaled/normalized to values between -1 and 1.
 *  
 *  
 *  -- Christopher Laponsie 4/21/2021 
  */

using System;
using System.IO;

namespace neuralnet
{
    class mnist
    {
        byte[] fileBytes;
        public byte[][] imageData;
        public double[][] normalData;
        public double[][] classifications; // one-hot-encoded classifications
        public byte[] labelData;
        string filepath = "data/train-images.idx3-ubyte";
        string labelpath = "data/train-labels.idx1-ubyte";


        public byte[][] t_imageData;
        public double[][] t_normalData;
        public double[][] t_classifications; // one-hot-encoded classifications
        public byte[] t_labelData;
        string t_filepath = "data/t10k-images.idx3-ubyte";
        string t_labelpath = "data/t10k-labels.idx1-ubyte";

        int pos = 0;
        int length;
        public uint n_images;
        public uint t_n_images;
        uint img_y;
        uint img_x;

        public double[,] GetTestSamples()
        {
            // returns a test dataset of all 10k images ready for forward pass
            double[,] batch_samples = new double[10000, img_x * img_y];
            for (int n = 0; n < 10000; n++)
            {
                for (int i = 0; i < (img_x * img_y); i++)
                {
                    batch_samples[n, i] = this.t_normalData[n][i];
                }
            }
            return batch_samples;
        }
        public double[,] GetTestLabels()
        {
            double[,] batch_classifications = new double[10000, 10];
            for (int n = 0; n < 10000; n++)
            {
                for (int i = 0; i < 10; i++)
                {
                    batch_classifications[n, i] = this.t_classifications[n][i];
                }
            }
            return batch_classifications;
        }
        public double[,] GetBatchSamples(int[] sample_indices)
        {
            // takes in an array of integers and constructs a 2d array of samples matching corresponding to indices
            double[,] batch_samples = new double[sample_indices.GetUpperBound(0)+1, img_x * img_y];
            for (int n = 0; n <= sample_indices.GetUpperBound(0); n++)
            {
                for (int i = 0; i < (img_x * img_y); i++)
                {
                    batch_samples[n, i] = this.normalData[sample_indices[n]][i];
                }
            }

            return batch_samples;
        }
        public double[,] GetBatchClassifications(int[] sample_indices)
        {
            double[,] batch_classifications = new double[sample_indices.GetUpperBound(0) + 1, 10];
            for (int n = 0; n <= sample_indices.GetUpperBound(0); n++)
            {
                for (int i = 0; i < 10; i++)
                {
                    batch_classifications[n, i] = this.classifications[sample_indices[n]][i];
                }
            }
            return batch_classifications;
        }
        public void OneHotEncodeLabels()
        {
            this.classifications = new double[n_images][];
            for (int n = 0; n < n_images; n++)
            {
                this.classifications[n] = new double[10];
                for (int i = 0; i < 10; i++)
                {
                    if (i == labelData[n]) this.classifications[n][i] = 1;
                    else this.classifications[n][i] = 0;
                }
            }

            this.t_classifications = new double[t_n_images][];
            for (int n = 0; n < t_n_images; n++)
            {
                this.t_classifications[n] = new double[10];
                for (int i = 0; i < 10; i++)
                {
                    if (i == t_labelData[n]) this.t_classifications[n][i] = 1;
                    else this.t_classifications[n][i] = 0;
                }
            }
        }
        public void NormalizeData()
        {
            this.normalData = new double[n_images][];
            for (int n = 0; n < n_images; n++) {
                this.normalData[n] = new double[img_x * img_y];
                for (int i = 0; i < (img_x * img_y); i++)
                {
                    normalData[n][i] = (double)imageData[n][i] / 255;
                }
            }

            this.t_normalData = new double[t_n_images][];
            for (int n = 0; n < t_n_images; n++)
            {
                this.t_normalData[n] = new double[img_x * img_y];
                for (int i = 0; i < (img_x * img_y); i++)
                {
                    t_normalData[n][i] = (double)t_imageData[n][i] / 255;
                }
            }


        }
        
        public byte GetByte()
        {
            if (pos < length) { 
                byte b = fileBytes[pos];
                pos++;
                
                return b;
            }
            else
            {
                Console.WriteLine("Unexpected end of file!");
                return 0x00;
            }
        }
        public byte [] GetByteImage(uint rows, uint cols)
        {
            byte[] byteImage = new byte[cols*rows];
            for (int y = 0; y < rows; y++) {
                for (int x = 0; x < cols; x++)
                {
                    byteImage[y*cols + x] = GetByte();
                    
                }
            }
            return byteImage;
        }
        public uint GetUINT()
        {
            // read the next 4 bytes as 32 bit big endian integer according to the idx3-ubyte file type
            byte[] buffer = new byte[4];
            buffer[0] = GetByte();
            buffer[1] = GetByte();
            buffer[2] = GetByte();
            buffer[3] = GetByte();
            Array.Reverse(buffer);
            uint i = BitConverter.ToUInt32(buffer, 0);
            return i;
        }
        public void DisplayImageInConsole (uint n)
        {
            // draws and ascii image in the console just to quickly verify that the imageset loaded correctly
            for (int y = 0; y < img_y; y++)
            {
                for (int x = 0; x < img_x; x++)
                {
                    byte b = imageData[n][y* img_x + x];
                    char o = ' ';
                    if (b > 25) o = '`';
                    if (b > 50) o = '.';
                    if (b > 75) o = ':';
                    if (b > 100) o = '-';
                    if (b > 125) o = '=';
                    if (b > 150) o = '^';
                    if (b > 175) o = '%';
                    if (b > 200) o = '&';
                    if (b > 225) o = '#';
                    Console.Write(o);
                }
                Console.Write("\n");
            }
        }
        public void LoadTrainLabels()
        {
            Console.WriteLine("Opening file '{0}'", this.labelpath);
            try
            {
                this.fileBytes = File.ReadAllBytes(this.labelpath);
            }
            catch (DirectoryNotFoundException )
            {
                Console.WriteLine("ERROR - Directory not found!");
                return;
            }
            catch (FileNotFoundException )
            {
                Console.WriteLine("ERROR - File not found!");
                return;
            }
            pos = 0;
            length = fileBytes.Length;
            Console.WriteLine("...OK.  {0} bytes.", length);
            Console.WriteLine("Parsing training labels");
            // file specifications described by http://yann.lecun.com/exdb/mnist/
            // all integers are MSB (high endian)
            // bytes 0 to 3 represent a 32 bit integer magic number equal to 2051
            if (GetUINT() != 2049)
            {
                Console.WriteLine("ERROR - Magic number is not correct.");
                return;
            }

            uint n_labels = GetUINT();
            Console.WriteLine("Loading {0} labels from file.", n_labels);
            labelData = new byte[n_labels];
            for (int n = 0; n < n_labels; n++)
            {
                labelData[n] = GetByte();
            }
            Console.WriteLine("..Done!");
        }

        public void LoadTestLabels()
        {
            Console.WriteLine("Opening file '{0}'", this.t_labelpath);
            try
            {
                this.fileBytes = File.ReadAllBytes(this.t_labelpath);
            }
            catch (DirectoryNotFoundException )
            {
                Console.WriteLine("ERROR - Directory not found!");
                return;
            }
            catch (FileNotFoundException )
            {
                Console.WriteLine("ERROR - File not found!");
                return;
            }
            pos = 0;
            length = fileBytes.Length;
            Console.WriteLine("...OK.  {0} bytes.", length);
            Console.WriteLine("Parsing test labels");
            // file specifications described by http://yann.lecun.com/exdb/mnist/
            // all integers are MSB (high endian)
            // bytes 0 to 3 represent a 32 bit integer magic number equal to 2051
            if (GetUINT() != 2049)
            {
                Console.WriteLine("ERROR - Magic number is not correct.");
                return;
            }

            uint n_labels = GetUINT();
            Console.WriteLine("Loading {0} labels from file.", n_labels);
            t_labelData = new byte[n_labels];
            for (int n = 0; n < n_labels; n++)
            {
                t_labelData[n] = GetByte();
            }
            Console.WriteLine("..Done!");

        }
        public void Load()
        {
            this.LoadTrainLabels();
            this.LoadTrainDataset();
            
            this.LoadTestLabels();
            this.LoadTestDataset();
            this.NormalizeData();
            this.OneHotEncodeLabels();

            /*
            Random rnd = new Random();
            int r_img = rnd.Next(0, (int)n_images); // creates a number between 1 and 12
            Console.WriteLine("Displaying random image from file {0}/{1}", (uint)r_img, n_images);

            Console.WriteLine("Label for this image is '{0}'", labelData[r_img]);
            Console.WriteLine("One-hot-encode is {0}", matrix.VectorToString(this.classifications[r_img]));
            DisplayImageInConsole((uint)r_img);
            */
        }
        public void LoadTrainDataset ()
        {
            
            Console.WriteLine("Opening file '{0}'", this.filepath);
            try
            {
                this.fileBytes = File.ReadAllBytes(this.filepath);
            } catch (DirectoryNotFoundException )
            {
                Console.WriteLine("ERROR - Directory not found!");
                return;
            }
            catch (FileNotFoundException )
            {
                Console.WriteLine("ERROR - File not found!");
                return;
            }
            pos = 0;
            length = fileBytes.Length;
            Console.WriteLine("...OK.  {0} bytes.", length);
            Console.WriteLine("Parsing training data");
            // file specifications described by http://yann.lecun.com/exdb/mnist/
            // all integers are MSB (high endian)
            // bytes 0 to 3 represent a 32 bit integer magic number equal to 2051
            if (GetUINT() != 2051)
            {
                Console.WriteLine("ERROR - Magic number is not correct.");
                return;
            }

            n_images = GetUINT();
            
            img_y = GetUINT();
            img_x = GetUINT();
            Console.WriteLine("Image size is {0}x{1}", img_x, img_y);
            if (img_x != 28 || img_y != 28)
            {
                Console.WriteLine("Incorrect image size.  Should be 28x28.");
                return;
            }
            Console.WriteLine("Loading {0} images from file.", n_images);
            imageData = new byte[n_images][];
            for (int imgn = 0; imgn < n_images; imgn++)
            {
                imageData[imgn] = GetByteImage(img_y, img_x);
            }
            Console.WriteLine("..Done!");
        }
        public void LoadTestDataset()
        {

            Console.WriteLine("Opening file '{0}'", this.t_filepath);
            try
            {
                this.fileBytes = File.ReadAllBytes(this.t_filepath);
            }
            catch (DirectoryNotFoundException )
            {
                Console.WriteLine("ERROR - Directory not found!");
                return;
            }
            catch (FileNotFoundException )
            {
                Console.WriteLine("ERROR - File not found!");
                return;
            }
            pos = 0;
            length = fileBytes.Length;
            Console.WriteLine("...OK.  {0} bytes.", length);
            Console.WriteLine("Parsing training data");
            // file specifications described by http://yann.lecun.com/exdb/mnist/
            // all integers are MSB (high endian)
            // bytes 0 to 3 represent a 32 bit integer magic number equal to 2051
            if (GetUINT() != 2051)
            {
                Console.WriteLine("ERROR - Magic number is not correct.");
                return;
            }

            t_n_images = GetUINT();

            img_y = GetUINT();
            img_x = GetUINT();
            Console.WriteLine("Image size is {0}x{1}", img_x, img_y);
            if (img_x != 28 || img_y != 28)
            {
                Console.WriteLine("Incorrect image size.  Should be 28x28.");
                return;
            }
            Console.WriteLine("Loading {0} images from file.", n_images);
            t_imageData = new byte[t_n_images][];
            for (int imgn = 0; imgn < t_n_images; imgn++)
            {
                t_imageData[imgn] = GetByteImage(img_y, img_x);
            }
            Console.WriteLine("..Done!");




        }
    }
}
