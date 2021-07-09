using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing;
using neuralnet;
using System.Collections.Generic;

namespace nnGui
{
    /// <summary>
    /// Inherits from PictureBox; adds Interpolation Mode Setting
    /// </summary>
    public class PictureBoxWithInterpolationMode : PictureBox
    {
        public InterpolationMode InterpolationMode { get; set; }

        protected override void OnPaint(PaintEventArgs paintEventArgs)
        {
            paintEventArgs.Graphics.InterpolationMode = InterpolationMode;
            base.OnPaint(paintEventArgs);
        }
    }

    public partial class ModelTable : DataGridView
    {
        public void Rebuild(Model model)
        {
            foreach (ModelComponent c in model.components)
            {
                if (c is DenseLayer)
                {
                    DenseLayer L = c as DenseLayer;
                    this.Rows.Add("DenseLayer", (L.weights.GetUpperBound(0)+1).ToString(), (L.weights.GetUpperBound(1) + 1).ToString());
                } else if (c is Sigmoid)
                {
                    this.Rows.Add("Sigmoid Function", "", "");
                }
                else if (c is ReLU)
                {
                    this.Rows.Add("ReLU Function", "", "");
                } else
                {
                    this.Rows.Add("Unknown Component", "", "");
                }
            }
        }
    }

    /// <summary>
    /// Inherits from TableLayoutPanel
    /// Creates rows/columns to match the number of layers in the model
    /// Draws an image representing each layer's weights and biases
    /// </summary>
    public partial class ModelVisualizer : TableLayoutPanel
    {
        public List<PictureBoxWithInterpolationMode> pictures;
        public void Redraw(Model model)
        {
            int n = 0;
            foreach (ModelComponent c in model.components)
            {
                if (c is Layer)
                {
                    Layer l = c as Layer;
                    

                    Bitmap image = Utility.MatrixToBitMap(l.weights, true);
                    Bitmap image2 = Utility.MatrixToBitMap(l.biases, false);
                    PictureBoxWithInterpolationMode p = this.pictures[n++];
                    PictureBoxWithInterpolationMode p2 = this.pictures[n++];
                    p.Image = image;
                    p2.Image = image2;


                }
            }
        }
        public void Rebuild(Model model)
        {
            if (this.pictures is null) pictures = new List<PictureBoxWithInterpolationMode>();
            else this.pictures.Clear();
            this.AutoSize = true;
            this.Controls.Clear();
            this.RowStyles.Clear();
            this.ColumnCount = 1;
            this.AutoScroll = true;
            this.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            //for (int i = 0; i < 2; i++)
            //{
            this.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            //}
            this.Dock = DockStyle.Fill;

            int n = 0;
            foreach (ModelComponent c in model.components)
            {
                if (c is Layer)
                {
                    Layer l = c as Layer;
                    n++;
                    Bitmap image = Utility.MatrixToBitMap(l.weights, true);
                    PictureBoxWithInterpolationMode picture = new PictureBoxWithInterpolationMode();
                    this.pictures.Add(picture);
                    picture.InterpolationMode = InterpolationMode.NearestNeighbor;
                    picture.Image = image;
                    //picture.AutoSize = true;
                    picture.SizeMode = PictureBoxSizeMode.StretchImage;
                    picture.BorderStyle = BorderStyle.FixedSingle;
                    picture.Dock = DockStyle.Fill;
                    
                    

                    Bitmap image2 = Utility.MatrixToBitMap(l.biases, false);
                    PictureBoxWithInterpolationMode picture2 = new PictureBoxWithInterpolationMode();
                    this.pictures.Add(picture2);
                    picture2.InterpolationMode = InterpolationMode.NearestNeighbor;
                    picture2.Image = image2;
                    picture2.Dock = DockStyle.Fill;
                    //picture2.AutoSize = true;
                    picture2.SizeMode = PictureBoxSizeMode.StretchImage;
                    
                    //picture2.BorderStyle = BorderStyle.Fixed3D;

                    this.RowCount += 4;
                    
                    this.Controls.Add(new Label() { Text = "Dense Layer ("+n.ToString()+") Weights", Size=new Size(180,25) }, 0, this.RowCount - 4);
                    this.Controls.Add(picture, 0, this.RowCount - 3);

                    this.Controls.Add(new Label() { Text = "Dense Layer (" + n.ToString() + ") Biases", Size = new Size(180, 25) }, 0, this.RowCount - 2);
                    this.Controls.Add(picture2, 0, this.RowCount - 1);

                }
                this.RowCount += 1;
            }
        }
    }

    

    partial class Gui
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

       
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.label1 = new System.Windows.Forms.Label();
            this.image_index_label = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.class_true_label = new System.Windows.Forms.Label();
            this.class_predicted_label = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.confidence_label = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.stopSGDButton = new System.Windows.Forms.Button();
            this.learningRateInput = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.batchSizeInput = new System.Windows.Forms.NumericUpDown();
            this.momentumInput = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.learningRateDecayInput = new System.Windows.Forms.TextBox();
            this.startSGDButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.lrChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.lossChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.accChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.loadModelButton = new System.Windows.Forms.Button();
            this.saveModelButton = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.makeDefaultModelButton = new System.Windows.Forms.Button();
            this.addOutputLayerButton = new System.Windows.Forms.Button();
            this.addSigmoidFunctionButton = new System.Windows.Forms.Button();
            this.addReLUFunctionButton = new System.Windows.Forms.Button();
            this.addDenseLayerButton = new System.Windows.Forms.Button();
            this.newModelButton = new System.Windows.Forms.Button();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.ConsoleRichText = new System.Windows.Forms.RichTextBox();
            this.optimizerWorker = new System.ComponentModel.BackgroundWorker();
            this.accuracyWorker = new System.ComponentModel.BackgroundWorker();
            this.accuracyTestButton = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.pictureBox1 = new nnGui.PictureBoxWithInterpolationMode();
            this.modelVisualizer = new nnGui.ModelVisualizer();
            this.modelDataGrid = new nnGui.ModelTable();
            this.CType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Inputs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Outputs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.batchSizeInput)).BeginInit();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lrChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lossChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.accChart)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.modelDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hScrollBar1.Location = new System.Drawing.Point(0, 502);
            this.hScrollBar1.Maximum = 59999;
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(621, 19);
            this.hScrollBar1.TabIndex = 1;
            this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar1_Scroll);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(301, 40);
            this.label1.TabIndex = 2;
            this.label1.Text = "Image Index";
            // 
            // image_index_label
            // 
            this.image_index_label.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.image_index_label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.image_index_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.image_index_label.Location = new System.Drawing.Point(310, 0);
            this.image_index_label.Name = "image_index_label";
            this.image_index_label.Size = new System.Drawing.Size(302, 40);
            this.image_index_label.TabIndex = 3;
            this.image_index_label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(301, 40);
            this.label2.TabIndex = 4;
            this.label2.Text = "Class True";
            // 
            // class_true_label
            // 
            this.class_true_label.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.class_true_label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.class_true_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.class_true_label.Location = new System.Drawing.Point(310, 40);
            this.class_true_label.Name = "class_true_label";
            this.class_true_label.Size = new System.Drawing.Size(302, 40);
            this.class_true_label.TabIndex = 5;
            this.class_true_label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // class_predicted_label
            // 
            this.class_predicted_label.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.class_predicted_label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.class_predicted_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.class_predicted_label.Location = new System.Drawing.Point(310, 80);
            this.class_predicted_label.Name = "class_predicted_label";
            this.class_predicted_label.Size = new System.Drawing.Size(302, 40);
            this.class_predicted_label.TabIndex = 7;
            this.class_predicted_label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(3, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(301, 40);
            this.label4.TabIndex = 6;
            this.label4.Text = "Class Predicted";
            // 
            // confidence_label
            // 
            this.confidence_label.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.confidence_label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.confidence_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.confidence_label.Location = new System.Drawing.Point(310, 120);
            this.confidence_label.Name = "confidence_label";
            this.confidence_label.Size = new System.Drawing.Size(302, 42);
            this.confidence_label.TabIndex = 9;
            this.confidence_label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(3, 120);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(301, 42);
            this.label5.TabIndex = 8;
            this.label5.Text = "Confidence";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.hScrollBar1, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(621, 690);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.class_true_label, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.class_predicted_label, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.confidence_label, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label5, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.image_index_label, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 525);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(615, 162);
            this.tableLayoutPanel2.TabIndex = 11;
            this.tableLayoutPanel2.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel2_Paint);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(641, 694);
            this.tabControl1.TabIndex = 11;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tableLayoutPanel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(633, 668);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Dataset Browser";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tableLayoutPanel3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(633, 668);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "SGD Optimizer";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel5, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.modelVisualizer, 0, 3);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.Padding = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 230F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(633, 672);
            this.tableLayoutPanel3.TabIndex = 0;
            this.tableLayoutPanel3.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel3_Paint);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.stopSGDButton, 1, 4);
            this.tableLayoutPanel4.Controls.Add(this.learningRateInput, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.label6, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.label8, 0, 3);
            this.tableLayoutPanel4.Controls.Add(this.batchSizeInput, 1, 3);
            this.tableLayoutPanel4.Controls.Add(this.momentumInput, 1, 2);
            this.tableLayoutPanel4.Controls.Add(this.label7, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.learningRateDecayInput, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.startSGDButton, 0, 4);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(5, 5);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 5;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(627, 224);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // stopSGDButton
            // 
            this.stopSGDButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stopSGDButton.Enabled = false;
            this.stopSGDButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stopSGDButton.Location = new System.Drawing.Point(318, 165);
            this.stopSGDButton.Margin = new System.Windows.Forms.Padding(5);
            this.stopSGDButton.Name = "stopSGDButton";
            this.stopSGDButton.Size = new System.Drawing.Size(304, 54);
            this.stopSGDButton.TabIndex = 6;
            this.stopSGDButton.Text = "Stop";
            this.stopSGDButton.UseVisualStyleBackColor = true;
            this.stopSGDButton.Click += new System.EventHandler(this.stopSGDButton_Click);
            // 
            // learningRateInput
            // 
            this.learningRateInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.learningRateInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.learningRateInput.Location = new System.Drawing.Point(316, 3);
            this.learningRateInput.Name = "learningRateInput";
            this.learningRateInput.Size = new System.Drawing.Size(308, 31);
            this.learningRateInput.TabIndex = 1;
            this.learningRateInput.TextChanged += new System.EventHandler(this.learningRateInput_TextChanged);
            this.learningRateInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.doubleInput_KeyPress);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(307, 40);
            this.label3.TabIndex = 0;
            this.label3.Text = "Learning Rate";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(3, 40);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(307, 40);
            this.label6.TabIndex = 1;
            this.label6.Text = "Learning Rate Decay";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(3, 120);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(307, 40);
            this.label8.TabIndex = 3;
            this.label8.Text = "Batch Size";
            // 
            // batchSizeInput
            // 
            this.batchSizeInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.batchSizeInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.batchSizeInput.Location = new System.Drawing.Point(316, 123);
            this.batchSizeInput.Name = "batchSizeInput";
            this.batchSizeInput.Size = new System.Drawing.Size(308, 31);
            this.batchSizeInput.TabIndex = 4;
            this.batchSizeInput.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.batchSizeInput.ValueChanged += new System.EventHandler(this.batchSizeInput_ValueChanged);
            // 
            // momentumInput
            // 
            this.momentumInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.momentumInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.momentumInput.Location = new System.Drawing.Point(316, 83);
            this.momentumInput.Name = "momentumInput";
            this.momentumInput.Size = new System.Drawing.Size(308, 31);
            this.momentumInput.TabIndex = 3;
            this.momentumInput.TextChanged += new System.EventHandler(this.momentumInput_TextChanged);
            this.momentumInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.doubleInput_KeyPress);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(3, 80);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(307, 40);
            this.label7.TabIndex = 2;
            this.label7.Text = "Momentum";
            // 
            // learningRateDecayInput
            // 
            this.learningRateDecayInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.learningRateDecayInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.learningRateDecayInput.Location = new System.Drawing.Point(316, 43);
            this.learningRateDecayInput.Name = "learningRateDecayInput";
            this.learningRateDecayInput.Size = new System.Drawing.Size(308, 31);
            this.learningRateDecayInput.TabIndex = 2;
            this.learningRateDecayInput.TextChanged += new System.EventHandler(this.learningRateDecayInput_TextChanged);
            this.learningRateDecayInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.doubleInput_KeyPress);
            // 
            // startSGDButton
            // 
            this.startSGDButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.startSGDButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startSGDButton.Location = new System.Drawing.Point(5, 165);
            this.startSGDButton.Margin = new System.Windows.Forms.Padding(5);
            this.startSGDButton.Name = "startSGDButton";
            this.startSGDButton.Size = new System.Drawing.Size(303, 54);
            this.startSGDButton.TabIndex = 5;
            this.startSGDButton.Text = "Start";
            this.startSGDButton.UseVisualStyleBackColor = true;
            this.startSGDButton.Click += new System.EventHandler(this.startSGDButton_Click);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.lrChart, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.lossChart, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.accChart, 1, 1);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(5, 235);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(627, 281);
            this.tableLayoutPanel5.TabIndex = 2;
            // 
            // lrChart
            // 
            this.lrChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.Name = "ChartArea1";
            this.lrChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.lrChart.Legends.Add(legend1);
            this.lrChart.Location = new System.Drawing.Point(3, 143);
            this.lrChart.Name = "lrChart";
            this.lrChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "LR";
            this.lrChart.Series.Add(series1);
            this.lrChart.Size = new System.Drawing.Size(307, 135);
            this.lrChart.TabIndex = 3;
            this.lrChart.Text = "chart1";
            // 
            // lossChart
            // 
            this.lossChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea2.Name = "ChartArea1";
            this.lossChart.ChartAreas.Add(chartArea2);
            this.tableLayoutPanel5.SetColumnSpan(this.lossChart, 2);
            legend2.Name = "Legend1";
            this.lossChart.Legends.Add(legend2);
            this.lossChart.Location = new System.Drawing.Point(3, 3);
            this.lossChart.Name = "lossChart";
            this.lossChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SemiTransparent;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Legend = "Legend1";
            series2.Name = "Loss";
            this.lossChart.Series.Add(series2);
            this.lossChart.Size = new System.Drawing.Size(621, 134);
            this.lossChart.TabIndex = 1;
            this.lossChart.Text = "chart1";
            // 
            // accChart
            // 
            this.accChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea3.Name = "ChartArea1";
            this.accChart.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            this.accChart.Legends.Add(legend3);
            this.accChart.Location = new System.Drawing.Point(316, 143);
            this.accChart.Name = "accChart";
            this.accChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Excel;
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Legend = "Legend1";
            series3.Name = "Acc";
            this.accChart.Series.Add(series3);
            this.accChart.Size = new System.Drawing.Size(308, 135);
            this.accChart.TabIndex = 2;
            this.accChart.Text = "chart1";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.panel3);
            this.tabPage3.Controls.Add(this.panel2);
            this.tabPage3.Controls.Add(this.panel1);
            this.tabPage3.Controls.Add(this.modelDataGrid);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(633, 668);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Model Design";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.loadModelButton);
            this.panel2.Controls.Add(this.saveModelButton);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Location = new System.Drawing.Point(9, 212);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(621, 65);
            this.panel2.TabIndex = 2;
            // 
            // loadModelButton
            // 
            this.loadModelButton.Location = new System.Drawing.Point(460, 39);
            this.loadModelButton.Name = "loadModelButton";
            this.loadModelButton.Size = new System.Drawing.Size(155, 23);
            this.loadModelButton.TabIndex = 7;
            this.loadModelButton.Text = "Load Model";
            this.loadModelButton.UseVisualStyleBackColor = true;
            this.loadModelButton.Click += new System.EventHandler(this.loadModelButton_Click);
            // 
            // saveModelButton
            // 
            this.saveModelButton.Location = new System.Drawing.Point(3, 39);
            this.saveModelButton.Name = "saveModelButton";
            this.saveModelButton.Size = new System.Drawing.Size(155, 23);
            this.saveModelButton.TabIndex = 6;
            this.saveModelButton.Text = "Save Model";
            this.saveModelButton.UseVisualStyleBackColor = true;
            this.saveModelButton.Click += new System.EventHandler(this.saveModelButton_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(14, 9);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(570, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Make sure the first layer has 728 inputs and the last layer has 10 neurons for th" +
    "e model to work with the MNIST dataset.";
            this.label9.Click += new System.EventHandler(this.label9_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.makeDefaultModelButton);
            this.panel1.Controls.Add(this.addOutputLayerButton);
            this.panel1.Controls.Add(this.addSigmoidFunctionButton);
            this.panel1.Controls.Add(this.addReLUFunctionButton);
            this.panel1.Controls.Add(this.addDenseLayerButton);
            this.panel1.Controls.Add(this.newModelButton);
            this.panel1.Location = new System.Drawing.Point(463, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(161, 199);
            this.panel1.TabIndex = 1;
            // 
            // makeDefaultModelButton
            // 
            this.makeDefaultModelButton.Location = new System.Drawing.Point(3, 173);
            this.makeDefaultModelButton.Name = "makeDefaultModelButton";
            this.makeDefaultModelButton.Size = new System.Drawing.Size(155, 23);
            this.makeDefaultModelButton.TabIndex = 5;
            this.makeDefaultModelButton.Text = "Make Default Model";
            this.makeDefaultModelButton.UseVisualStyleBackColor = true;
            this.makeDefaultModelButton.Click += new System.EventHandler(this.makeDefaultModelButton_Click);
            // 
            // addOutputLayerButton
            // 
            this.addOutputLayerButton.Location = new System.Drawing.Point(6, 119);
            this.addOutputLayerButton.Name = "addOutputLayerButton";
            this.addOutputLayerButton.Size = new System.Drawing.Size(155, 23);
            this.addOutputLayerButton.TabIndex = 4;
            this.addOutputLayerButton.Text = "Add Output Layer";
            this.addOutputLayerButton.UseVisualStyleBackColor = true;
            this.addOutputLayerButton.Click += new System.EventHandler(this.addOutputLayerButton_Click);
            // 
            // addSigmoidFunctionButton
            // 
            this.addSigmoidFunctionButton.Location = new System.Drawing.Point(3, 90);
            this.addSigmoidFunctionButton.Name = "addSigmoidFunctionButton";
            this.addSigmoidFunctionButton.Size = new System.Drawing.Size(155, 23);
            this.addSigmoidFunctionButton.TabIndex = 3;
            this.addSigmoidFunctionButton.Text = "Add Sigmoid Function";
            this.addSigmoidFunctionButton.UseVisualStyleBackColor = true;
            this.addSigmoidFunctionButton.Click += new System.EventHandler(this.addSigmoidFunctionButton_Click);
            // 
            // addReLUFunctionButton
            // 
            this.addReLUFunctionButton.Location = new System.Drawing.Point(3, 61);
            this.addReLUFunctionButton.Name = "addReLUFunctionButton";
            this.addReLUFunctionButton.Size = new System.Drawing.Size(155, 23);
            this.addReLUFunctionButton.TabIndex = 2;
            this.addReLUFunctionButton.Text = "Add ReLU Function";
            this.addReLUFunctionButton.UseVisualStyleBackColor = true;
            this.addReLUFunctionButton.Click += new System.EventHandler(this.addReLUFunctionButton_Click);
            // 
            // addDenseLayerButton
            // 
            this.addDenseLayerButton.Location = new System.Drawing.Point(3, 32);
            this.addDenseLayerButton.Name = "addDenseLayerButton";
            this.addDenseLayerButton.Size = new System.Drawing.Size(155, 23);
            this.addDenseLayerButton.TabIndex = 1;
            this.addDenseLayerButton.Text = "Add Dense Layer";
            this.addDenseLayerButton.UseVisualStyleBackColor = true;
            this.addDenseLayerButton.Click += new System.EventHandler(this.addDenseLayerButton_Click);
            // 
            // newModelButton
            // 
            this.newModelButton.Location = new System.Drawing.Point(3, 3);
            this.newModelButton.Name = "newModelButton";
            this.newModelButton.Size = new System.Drawing.Size(155, 23);
            this.newModelButton.TabIndex = 0;
            this.newModelButton.Text = "New Model";
            this.newModelButton.UseVisualStyleBackColor = true;
            this.newModelButton.Click += new System.EventHandler(this.newModelButton_Click);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(641, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 694);
            this.splitter1.TabIndex = 13;
            this.splitter1.TabStop = false;
            // 
            // ConsoleRichText
            // 
            this.ConsoleRichText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(169)))), ((int)(((byte)(184)))));
            this.ConsoleRichText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ConsoleRichText.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConsoleRichText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(23)))), ((int)(((byte)(14)))));
            this.ConsoleRichText.Location = new System.Drawing.Point(641, 0);
            this.ConsoleRichText.Margin = new System.Windows.Forms.Padding(10);
            this.ConsoleRichText.Name = "ConsoleRichText";
            this.ConsoleRichText.ReadOnly = true;
            this.ConsoleRichText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.ConsoleRichText.Size = new System.Drawing.Size(477, 694);
            this.ConsoleRichText.TabIndex = 12;
            this.ConsoleRichText.TabStop = false;
            this.ConsoleRichText.Text = "";
            this.ConsoleRichText.TextChanged += new System.EventHandler(this.ConsoleRichText_TextChanged);
            // 
            // optimizerWorker
            // 
            this.optimizerWorker.WorkerReportsProgress = true;
            this.optimizerWorker.WorkerSupportsCancellation = true;
            this.optimizerWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.optimizerWorker_DoWork);
            this.optimizerWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.optimizerWorker_ProgressChanged);

            this.accuracyWorker.WorkerReportsProgress = true;
            this.accuracyWorker.WorkerSupportsCancellation = true;
            this.accuracyWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.accuracyWorker_DoWork);
            this.accuracyWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.accuracyWorker_ProgressChanged);
            // 
            // accuracyTestButton
            // 
            this.accuracyTestButton.Location = new System.Drawing.Point(223, 29);
            this.accuracyTestButton.Name = "accuracyTestButton";
            this.accuracyTestButton.Size = new System.Drawing.Size(155, 23);
            this.accuracyTestButton.TabIndex = 8;
            this.accuracyTestButton.Text = "Accuracy Test";
            this.accuracyTestButton.UseVisualStyleBackColor = true;
            this.accuracyTestButton.Click += new System.EventHandler(this.accuracyTestButton_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label11);
            this.panel3.Controls.Add(this.label10);
            this.panel3.Controls.Add(this.accuracyTestButton);
            this.panel3.Location = new System.Drawing.Point(8, 283);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(621, 61);
            this.panel3.TabIndex = 9;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(296, 13);
            this.label10.TabIndex = 8;
            this.label10.Text = "The accuracy test will run your model on all 10k test images.  ";
            this.label10.Click += new System.EventHandler(this.label10_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 13);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(489, 13);
            this.label11.TabIndex = 9;
            this.label11.Text = "Warning, this may take a few minutes depending on the size of your model and spec" +
    "s of your machine.";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(615, 496);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // modelVisualizer
            // 
            this.modelVisualizer.AutoSize = true;
            this.modelVisualizer.ColumnCount = 1;
            this.modelVisualizer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.modelVisualizer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.modelVisualizer.Location = new System.Drawing.Point(5, 530);
            this.modelVisualizer.Name = "modelVisualizer";
            this.modelVisualizer.Size = new System.Drawing.Size(0, 0);
            this.modelVisualizer.TabIndex = 3;
            // 
            // modelDataGrid
            // 
            this.modelDataGrid.AllowUserToAddRows = false;
            this.modelDataGrid.AllowUserToDeleteRows = false;
            this.modelDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.modelDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CType,
            this.Inputs,
            this.Outputs});
            this.modelDataGrid.Location = new System.Drawing.Point(8, 6);
            this.modelDataGrid.Name = "modelDataGrid";
            this.modelDataGrid.ReadOnly = true;
            this.modelDataGrid.Size = new System.Drawing.Size(449, 199);
            this.modelDataGrid.TabIndex = 0;
            // 
            // CType
            // 
            this.CType.HeaderText = "Component Type";
            this.CType.Name = "CType";
            this.CType.ReadOnly = true;
            // 
            // Inputs
            // 
            this.Inputs.HeaderText = "Inputs";
            this.Inputs.Name = "Inputs";
            this.Inputs.ReadOnly = true;
            // 
            // Outputs
            // 
            this.Outputs.HeaderText = "Outputs";
            this.Outputs.Name = "Outputs";
            this.Outputs.ReadOnly = true;
            // 
            // Gui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1118, 694);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.ConsoleRichText);
            this.Controls.Add(this.tabControl1);
            this.Name = "Gui";
            this.Text = "nnSandbox";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.batchSizeInput)).EndInit();
            this.tableLayoutPanel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lrChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lossChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.accChart)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.modelDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public PictureBoxWithInterpolationMode pictureBox1;
        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label image_index_label;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label class_true_label;
        public System.Windows.Forms.Label class_predicted_label;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label confidence_label;
        private System.Windows.Forms.Label label5;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Splitter splitter1;
        public RichTextBox ConsoleRichText;
        private TableLayoutPanel tableLayoutPanel3;
        private TableLayoutPanel tableLayoutPanel4;
        private Label label3;
        private Label label6;
        private Label label7;
        private Label label8;
        public NumericUpDown batchSizeInput;
        public TextBox momentumInput;
        public TextBox learningRateDecayInput;
        public TextBox learningRateInput;
        private Button stopSGDButton;
        private Button startSGDButton;
        private System.ComponentModel.BackgroundWorker optimizerWorker;
        private System.ComponentModel.BackgroundWorker accuracyWorker;
        private TableLayoutPanel tableLayoutPanel5;
        public ModelVisualizer modelVisualizer;
        private System.Windows.Forms.DataVisualization.Charting.Chart lrChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart accChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart lossChart;
        private TabPage tabPage3;
        public ModelTable modelDataGrid;
        private DataGridViewTextBoxColumn CType;
        private DataGridViewTextBoxColumn Inputs;
        private DataGridViewTextBoxColumn Outputs;
        private Panel panel1;
        private Button addSigmoidFunctionButton;
        private Button addReLUFunctionButton;
        private Button addDenseLayerButton;
        private Button newModelButton;
        private Button addOutputLayerButton;
        private Button makeDefaultModelButton;
        private Panel panel2;
        private Label label9;
        private Button loadModelButton;
        private Button saveModelButton;
        private Panel panel3;
        private Label label10;
        private Button accuracyTestButton;
        private Label label11;
    }
}

