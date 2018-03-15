namespace Petronode.DigiFitter
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.mistieControl1 = new Petronode.DigiFitter.MistieControl();
            this.pictureDigitizerControl1 = new Petronode.CommonControls.PictureDigitizerControl();
            this.imageLoadControl1 = new Petronode.CommonControls.ImageLoadControl();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.fileLoadControl1 = new Petronode.CommonControls.FileLoadControl();
            this.toolLauncherControl1 = new Petronode.CommonControls.ToolLauncherControl();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.digitizerFunctionFitter1 = new Petronode.DigiFitter.DigitizerFunctionFitter();
            this.digitizerDataFitter1 = new Petronode.DigiFitter.DigitizerDataFitter();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.mistieControl1);
            this.splitContainer1.Panel1.Controls.Add(this.pictureDigitizerControl1);
            this.splitContainer1.Panel1.Controls.Add(this.imageLoadControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2.Controls.Add(this.fileLoadControl1);
            this.splitContainer1.Panel2.Controls.Add(this.toolLauncherControl1);
            this.splitContainer1.Size = new System.Drawing.Size(1032, 678);
            this.splitContainer1.SplitterDistance = 651;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // mistieControl1
            // 
            this.mistieControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mistieControl1.Location = new System.Drawing.Point(0, 536);
            this.mistieControl1.Name = "mistieControl1";
            this.mistieControl1.Size = new System.Drawing.Size(647, 136);
            this.mistieControl1.TabIndex = 2;
            // 
            // pictureDigitizerControl1
            // 
            this.pictureDigitizerControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureDigitizerControl1.CompletionPercent = 0;
            this.pictureDigitizerControl1.Location = new System.Drawing.Point(0, 39);
            this.pictureDigitizerControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureDigitizerControl1.Name = "pictureDigitizerControl1";
            this.pictureDigitizerControl1.Size = new System.Drawing.Size(647, 494);
            this.pictureDigitizerControl1.TabIndex = 1;
            // 
            // imageLoadControl1
            // 
            this.imageLoadControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.imageLoadControl1.FileName = "";
            this.imageLoadControl1.Location = new System.Drawing.Point(2, 2);
            this.imageLoadControl1.Name = "imageLoadControl1";
            this.imageLoadControl1.Size = new System.Drawing.Size(646, 37);
            this.imageLoadControl1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer2.Location = new System.Drawing.Point(2, 42);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.digitizerFunctionFitter1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.digitizerDataFitter1);
            this.splitContainer2.Size = new System.Drawing.Size(302, 630);
            this.splitContainer2.SplitterDistance = 315;
            this.splitContainer2.TabIndex = 4;
            // 
            // fileLoadControl1
            // 
            this.fileLoadControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileLoadControl1.FileName = "";
            this.fileLoadControl1.Filter = "Definition files|*.def|Comma Separated Files|*.csv|All files|*.*";
            this.fileLoadControl1.Location = new System.Drawing.Point(-3, 2);
            this.fileLoadControl1.Name = "fileLoadControl1";
            this.fileLoadControl1.Size = new System.Drawing.Size(373, 37);
            this.fileLoadControl1.TabIndex = 3;
            // 
            // toolLauncherControl1
            // 
            this.toolLauncherControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.toolLauncherControl1.HelpLocation = "";
            this.toolLauncherControl1.Location = new System.Drawing.Point(310, 42);
            this.toolLauncherControl1.Margin = new System.Windows.Forms.Padding(4);
            this.toolLauncherControl1.MaximumSize = new System.Drawing.Size(61, 12308);
            this.toolLauncherControl1.MinimumSize = new System.Drawing.Size(61, 57);
            this.toolLauncherControl1.Name = "toolLauncherControl1";
            this.toolLauncherControl1.Size = new System.Drawing.Size(61, 630);
            this.toolLauncherControl1.TabIndex = 0;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            // 
            // digitizerFunctionFitter1
            // 
            this.digitizerFunctionFitter1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.digitizerFunctionFitter1.Location = new System.Drawing.Point(0, 0);
            this.digitizerFunctionFitter1.Name = "digitizerFunctionFitter1";
            this.digitizerFunctionFitter1.Size = new System.Drawing.Size(298, 311);
            this.digitizerFunctionFitter1.TabIndex = 1;
            // 
            // digitizerDataFitter1
            // 
            this.digitizerDataFitter1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.digitizerDataFitter1.Location = new System.Drawing.Point(0, 0);
            this.digitizerDataFitter1.Name = "digitizerDataFitter1";
            this.digitizerDataFitter1.Size = new System.Drawing.Size(298, 307);
            this.digitizerDataFitter1.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1032, 678);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "PETRONODE Hubbert Fit (C) M.Yakimov, 2007";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private Petronode.CommonControls.ToolLauncherControl toolLauncherControl1;
        private Petronode.CommonControls.ImageLoadControl imageLoadControl1;
        private Petronode.CommonControls.PictureDigitizerControl pictureDigitizerControl1;
        private Petronode.CommonControls.FileLoadControl fileLoadControl1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private MistieControl mistieControl1;
        private DigitizerFunctionFitter digitizerFunctionFitter1;
        private DigitizerDataFitter digitizerDataFitter1;
    }
}

