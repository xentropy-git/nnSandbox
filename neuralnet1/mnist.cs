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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace neuralnet1
{
    class mnist
    {
        byte[] fileBytes;
        byte[][,] imageData;
        byte[] labelData;
        string filepath = "../../data/train-images.idx3-ubyte";
        string labelpath = "../../data/train-labels.idx1-ubyte";
        int pos = 0;
        int length;
        uint n_images;
        uint img_y;
        uint img_x;
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
        public byte [,] GetByteImage(uint rows, uint cols)
        {
            byte[,] byteImage = new byte[cols, rows];
            for (int y = 0; y < rows; y++) {
                for (int x = 0; x < cols; x++)
                {
                    byteImage[x, y] = GetByte();
                    
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
                    byte b = imageData[n][x, y];
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
            catch (DirectoryNotFoundException e1)
            {
                Console.WriteLine("ERROR - Directory not found!");
                return;
            }
            catch (FileNotFoundException e1)
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
        public void LoadTrainDataset ()
        {
            this.LoadTrainLabels();
            Console.WriteLine("Opening file '{0}'", this.filepath);
            try
            {
                this.fileBytes = File.ReadAllBytes(this.filepath);
            } catch (DirectoryNotFoundException e1)
            {
                Console.WriteLine("ERROR - Directory not found!");
                return;
            }
            catch (FileNotFoundException e1)
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
            imageData = new byte[n_images][,];
            for (int imgn = 0; imgn < n_images; imgn++)
            {
                imageData[imgn] = GetByteImage(img_y, img_x);
            }
            Console.WriteLine("..Done!");

            Random rnd = new Random();
            int r_img = rnd.Next(0,(int)n_images); // creates a number between 1 and 12
            Console.WriteLine("Displaying random image from file {0}/{1}", (uint) r_img, n_images);

            Console.WriteLine("Label for this image is '{0}'", labelData[r_img]);
            DisplayImageInConsole((uint)r_img);


        }
    }
}
