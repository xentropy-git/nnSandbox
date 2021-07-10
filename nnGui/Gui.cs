using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using neuralnet;

namespace nnGui
{
    public partial class Gui : Form
    {
        public Gui()
        {
            InitializeComponent();
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            Globals.img_n = this.hScrollBar1.Value;
            
            Globals.UpdateImage();
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Globals.Init();
        }

        private void ConsoleRichText_TextChanged(object sender, EventArgs e)
        {

        }

        private void momentumInput_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void doubleInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void learningRateInput_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Globals.sgd.starting_learning_rate = double.Parse(learningRateInput.Text);
            }
            catch (System.FormatException)
            {
                Globals.sgd.starting_learning_rate = 0;
                learningRateInput.Text = "";
            }
        }

        private void learningRateDecayInput_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Globals.sgd.learning_rate_decay = double.Parse(learningRateDecayInput.Text);
            }
            catch (System.FormatException)
            {
                Globals.sgd.learning_rate_decay = 0;
                learningRateDecayInput.Text = "";
            }
        }

        private void momentumInput_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Globals.sgd.momentum = double.Parse(momentumInput.Text);
            }
            catch (System.FormatException)
            {
                Globals.sgd.momentum = 0;
                momentumInput.Text = "";
            }
        }

        private void optimizerWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            double[][] data = (double[][])e.UserState;
            double epoch = 0;
            int records = data.GetUpperBound(0)+1;
            double loss_sum = 0;
            double acc_sum = 0;
            double lr = 0;
            for (int i = 0; i < records; i++)
            {
                this.lossChart.Series["Loss"].Points.AddXY(data[i][0], data[i][1]);
                this.accChart.Series["Acc"].Points.AddXY(data[i][0], data[i][2]);
                this.lrChart.Series["LR"].Points.AddXY(data[i][0], data[i][3]);
                epoch = data[i][0];
                loss_sum += data[i][1];
                acc_sum += data[i][2];
                lr = data[i][3];
            }

            using (StreamWriter sw = File.AppendText(Globals.logPath))
            {
                for (int i = 0; i < records; i++)
                {
                    sw.WriteLine("{0}, {1}, {2}, {3}", data[i][0], data[i][1], data[i][2], data[i][3]);
                }
            }

            double loss = loss_sum / records;
            double acc = acc_sum / records;
            Console.WriteLine("Epoch: {0}, loss: {1}, acc: {2}, lr: {3}", epoch, loss.ToString("0.####"),
                   acc.ToString("0.####"), lr.ToString("0.######"));

            Globals.RedrawLayerImages();
        }

        private void optimizerWorker_DoWork(object sender,
          DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;
            e.Result = Globals.OptimizerWork(worker,  e);
        }

        private void accuracyWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            double acc = (double)e.UserState;
            Console.WriteLine("...Done!");
            Console.WriteLine("The accuracy of this model is {0}%", (acc * 100).ToString("0.####"));
            EnableInterface(true);
        }

        private void accuracyWorker_DoWork(object sender,
          DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;
            e.Result = Globals.AccuracyWork(worker, e);
        }

        private void batchSizeInput_ValueChanged(object sender, EventArgs e)
        {
            Globals.sgd.batch_size = (int)batchSizeInput.Value;
        }
        private void DisableInterface(bool accuracyTest = false)
        {
            stopSGDButton.Enabled = true;
            startSGDButton.Enabled = false;
            learningRateInput.Enabled = false;
            learningRateDecayInput.Enabled = false;
            momentumInput.Enabled = false;
            batchSizeInput.Enabled = false;
            hScrollBar1.Enabled = false;

            addDenseLayerButton.Enabled = false;
            addSigmoidFunctionButton.Enabled = false;
            addOutputLayerButton.Enabled = false;
            addReLUFunctionButton.Enabled = false;
            makeDefaultModelButton.Enabled = false;
            newModelButton.Enabled = false;
            saveModelButton.Enabled = false;
            loadModelButton.Enabled = false;
            accuracyTestButton.Enabled = false;
            if (accuracyTest) stopSGDButton.Enabled = false;
        }
        private void EnableInterface(bool accuracyTest = false)
        {
            stopSGDButton.Enabled = false;
            startSGDButton.Enabled = true;
            learningRateInput.Enabled = true;
            learningRateDecayInput.Enabled = true;
            momentumInput.Enabled = true;
            batchSizeInput.Enabled = true;
            hScrollBar1.Enabled = true;

            addDenseLayerButton.Enabled = true;
            addSigmoidFunctionButton.Enabled = true;
            addOutputLayerButton.Enabled = true;
            addReLUFunctionButton.Enabled = true;
            makeDefaultModelButton.Enabled = true;
            newModelButton.Enabled = true;
            saveModelButton.Enabled = true;
            loadModelButton.Enabled = true;
            accuracyTestButton.Enabled = true;
            if (accuracyTest) stopSGDButton.Enabled = true;
        }
        private void startSGDButton_Click(object sender, EventArgs e)
        {
            if (!optimizerWorker.IsBusy)
            {
                
                Console.WriteLine("SGD Optimizer Starting...");
                Console.WriteLine("Model Definition:");
                Console.WriteLine(Globals.model);

                DisableInterface();
               

                optimizerWorker.RunWorkerAsync();
            }
        }

        private void stopSGDButton_Click(object sender, EventArgs e)
        {
            if (!optimizerWorker.CancellationPending)
            {
                EnableInterface();
                Console.WriteLine("SGD Optimizer Stopping.");
                optimizerWorker.CancelAsync();
            }
        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void newModelButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("New model");
            Globals.model.components.Clear();
            Globals.model.epoch = 0;
            modelDataGrid.Rows.Clear();
            modelDataGrid.Rebuild(Globals.model);
        }

        private void addDenseLayerButton_Click(object sender, EventArgs e)
        {
            int inputs = 728;
            int neurons = 100;

            if (Globals.model.components.Count == 0)
            {
                // this is going to be the first layer, so we inputs should be 728
                inputs = 728;
            } else
            {
                // get the neuron size of the most recent layer.
                foreach (ModelComponent c in Globals.model.components)
                {
                    if (c is Layer)
                    {
                        Layer l = c as Layer;
                        inputs = l.weights.GetUpperBound(1) + 1;
                    }
                }
            }
            bool keepAsking = true;
            while (keepAsking)
            {
                string entry = Interaction.InputBox("How many neurons on this layer?", "New Dense Layer", "");
                try
                {
                    neurons = int.Parse(entry);
                    if (neurons >0 && neurons <= 9999) keepAsking = false;

                }
                catch (System.FormatException)
                {
                    keepAsking = true;
                }
                if (keepAsking) MessageBox.Show("Please enter an integer between 0 and 9999", "Invalid entry");
            }
            Globals.model.AddComponent(new DenseLayer(inputs, neurons));
            modelDataGrid.Rows.Clear();
            modelDataGrid.Rebuild(Globals.model);
        }
        private void RebuildModelElements()
        {
            modelDataGrid.Rows.Clear();
            modelDataGrid.Rebuild(Globals.model);
            modelVisualizer.Rebuild(Globals.model);

        }
        private void addOutputLayerButton_Click(object sender, EventArgs e)
        {
            int inputs = 0;
            int neurons = 10;

            if (Globals.model.components.Count == 0)
            {
                // this is going to be the first layer, so we inputs should be 728
                inputs = 728;
            }
            else
            {
                // get the neuron size of the most recent layer.
                foreach (ModelComponent c in Globals.model.components)
                {
                    if (c is Layer)
                    {
                        Layer l = c as Layer;
                        inputs = l.weights.GetUpperBound(1) + 1;
                    }
                }
            }
            
            Globals.model.AddComponent(new DenseLayer(inputs, neurons));
            RebuildModelElements();
        }

        private void addReLUFunctionButton_Click(object sender, EventArgs e)
        {
            Globals.model.AddComponent(new ReLU(0));
            RebuildModelElements();
        }

        private void addSigmoidFunctionButton_Click(object sender, EventArgs e)
        {
            Globals.model.AddComponent(new Sigmoid(0));
            RebuildModelElements();
        }

        private void makeDefaultModelButton_Click(object sender, EventArgs e)
        {
            Globals.model.components.Clear();
            Globals.model.BuildDefault();
            RebuildModelElements();
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void loadModelButton_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory(); ;
                openFileDialog.Filter = "BIN files (*.bin)|*.bin";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    Globals.model.Load(filePath);
                    MessageBox.Show("Model loaded.", "Success");

                }

            }
            RebuildModelElements();
        }

        private void saveModelButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "BIN files (*.bin)|*.bin";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Globals.model.Save(saveFileDialog1.FileName);
                MessageBox.Show("Model saved.", "Success");
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void accuracyTestButton_Click(object sender, EventArgs e)
        {
            
            if (!accuracyWorker.IsBusy)
            {

                Console.WriteLine("Running Accuracy Test...");
                DisableInterface(true);
                accuracyWorker.RunWorkerAsync();
            }

        }
    }
}
