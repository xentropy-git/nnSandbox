using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using neuralnet;
using System.Text;


namespace nnGui
{
    public static class Globals
    {
        public const string app_version = "1.0.0";
        public const string pretrained_model_file = "models/pretrained.bin";
        public const string logPath = "log.csv";
        public static mnist m;
        public static Gui form;
        public static int img_n = 0;
        public static Model model;
        public static OptimizerSGD sgd;
        public static void Init()
        {
            Console.WriteLine("nnSandbox " + app_version);
            Console.WriteLine("By Chris Laponsie");
            Console.WriteLine("For educational purposes only");
            Console.WriteLine("-----------------------------------------");
            m = new mnist();
            m.Load();
            Console.WriteLine("-----------------------------------------");
            model = new Model();
            model.BuildDefault();
            //model.Load(pretrained_model_file);
            sgd = new OptimizerSGD();
            form.momentumInput.Text = sgd.momentum.ToString();
            form.learningRateDecayInput.Text = sgd.learning_rate_decay.ToString();
            form.learningRateInput.Text = sgd.starting_learning_rate.ToString();
            form.batchSizeInput.Value = sgd.batch_size;

            UpdateImage();
            form.modelVisualizer.Rebuild(model);
            form.modelDataGrid.Rebuild(model);

            if (!File.Exists(logPath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(logPath))
                {
                    sw.WriteLine("epoch, loss, acc, lr");
                }
            }
        }
        public static void RedrawLayerImages ()
        {
            form.modelVisualizer.Redraw(Globals.model);
        }
        public static double AccuracyWork (System.ComponentModel.BackgroundWorker worker, System.ComponentModel.DoWorkEventArgs e)
        {
            double[,] test_data = Globals.m.GetTestSamples();
            double[,] test_labels = Globals.m.GetTestLabels();
            Globals.model.Forward(test_data);
            Globals.model.CalculateLoss(test_labels);
            double acc = Globals.model.CalculateAccuracy(test_labels);
            worker.ReportProgress(1, acc);
            return 1;
        }
        public static double OptimizerWork (System.ComponentModel.BackgroundWorker worker, System.ComponentModel.DoWorkEventArgs e)
        {
            if (worker.CancellationPending)
            {
                e.Cancel = true;
            } else
            { 
                sgd.RandomizeBatch(m);
                sgd.Train(model);
                if (model.epoch % 100 == 0)
                {
                    worker.ReportProgress(model.epoch, sgd.DumpHistory());
                }
                return OptimizerWork(worker, e);
            }
            return 1;
            
        }
        public static void UpdateImage()
        {
            Globals.form.pictureBox1.Image = Utility.BitmapFromMNIST(Globals.m, Globals.img_n, 1);
            Globals.form.image_index_label.Text = Globals.img_n.ToString();
            Globals.form.class_true_label.Text = Globals.m.labelData[Globals.img_n].ToString();
            int[] sample = { Globals.img_n };
            (int argMaxA, double valueA) = matrix.argmax(Globals.model.Forward(Globals.m.GetBatchSamples(sample)), 0);

            if (argMaxA == Globals.m.labelData[Globals.img_n])
            {
                Globals.form.class_predicted_label.BackColor = Color.Green;
            } else
            {
                Globals.form.class_predicted_label.BackColor = Color.Red;
            }
            Globals.form.class_predicted_label.Text = argMaxA.ToString();
            Globals.form.confidence_label.Text = (valueA * 100).ToString("0.###") + "%";
        }
    }
    public class Utility
    {
        public static Bitmap MatrixToBitMap(double[,] m0, bool transpose = false)
        {
            int rows = m0.GetUpperBound(0) + 1;
            int cols = m0.GetUpperBound(1) + 1;

            // create a normalized matrix of values between 0 and 1
            double[,] normalized = new double[rows, cols];
            double min_value = 0;
            double max_value = 0;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (m0[y, x] < min_value) min_value = m0[y, x];
                    if (m0[y, x] > max_value) max_value = m0[y, x];
                }
            }

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    normalized[y, x] = (m0[y, x] - min_value) / (max_value - min_value);
                }
            }

            // creates an image of transposed dimensions of the matrix (looks better in the form)
            int width = cols;
            int height = rows;

            if (transpose)
            {
                width = rows;
                height = cols;
            }
            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int r0 = 255;
                    int g0 = 0;
                    int b0 = 0;

                    int r1 = 255;
                    int g1 = 255;
                    int b1 = 255;

                    int r2 = 0;
                    int g2 = 0;
                    int b2 = 255;

                    int r = 0;
                    int g = 0;
                    int b = 0;
                    double value = normalized[y, x];

                    // interpolate color gradient
                    // rgb0 = color of values close to 0
                    // rgb1 = color of values close to 0.5
                    // rgb2 = color ov values close 1
                    if (value >= 0.5)
                    {
                        r = (int)((r2 - r1) * (value - 0.5) / 0.5 + r1);
                        g = (int)((g2 - g1) * (value - 0.5) / 0.5 + g1);
                        b = (int)((b2 - b1) * (value - 0.5) / 0.5 + b1);
                    } else
                    {
                        r = (int)((r1 - r0) * (value) / 0.5 + r0);
                        g = (int)((g1 - g0) * (value) / 0.5 + g0);
                        b = (int)((b1 - b0) * (value) / 0.5 + b0);
                    }
                    r = Math.Max(0, Math.Min(255, r));
                    g = Math.Max(0, Math.Min(255, g));
                    b = Math.Max(0, Math.Min(255, b));
                    Color c = Color.FromArgb(r, g, b);
                    if (transpose) bmp.SetPixel(y, x, c);
                    else bmp.SetPixel(x, y, c);
                }
            }
            return bmp;
        }
        public static Bitmap BitmapFromMNIST(mnist m, int index = 0, int scale = 1)
        {
            Bitmap bmp = new Bitmap(28*scale, 28*scale, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    //Average out the RGB components to find the Gray Color
                    int value = m.imageData[index][(int)(y/scale) * 28 + (int)(x /scale)];
                    Color c = Color.FromArgb(value, value, value);
                    bmp.SetPixel(x, y, c);
                }
            }
            return bmp;
        }
    }
    public class ControlWriter : TextWriter
    {
        // class that redirects output from the Console to a GUI element
        private RichTextBox textbox;
        public ControlWriter(RichTextBox textbox)
        {
            this.textbox = textbox;
        }
        public override void Write(char value)
        {
            if (value != 13) textbox.Text += value;
            textbox.SelectionStart = textbox.Text.Length;

            textbox.ScrollToCaret();
        }
        public override void Write(string value)
        {
            value = value.Replace("\\r", ""); // remove carriage returns so we don't have double line endings in richtext
            textbox.Text += value;

            textbox.SelectionStart = textbox.Text.Length;

            textbox.ScrollToCaret();
        }
        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        
        
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Globals.form = new Gui();
            Console.SetOut(new ControlWriter(Globals.form.ConsoleRichText));
            Application.Run(Globals.form);
        }


    }
}
