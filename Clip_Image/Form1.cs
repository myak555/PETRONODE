using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Petronode.Clip_Image
{
    public partial class Form1 : Form
    {
        string Source = "";
        string Destination = "";
        int GetTop = 0;
        int SkipTop = 0;
        bool ClearBlank = false;
        bool Ready = false;
        bool Error = false;

        /// <summary>
        /// Constructor, creates the form
        /// </summary>
        /// <param name="args"></param>
        public Form1( string[] args)
        {
            InitializeComponent();
            try
            {
                if (args.Length <= 0) throw new Exception("No input file given.");
                if (args.Length >= 1)
                {
                    Source = args[0];
                    if (!File.Exists(Source)) throw new Exception("File does not exist: " + Source);
                }
                if (args.Length >= 2 && !args[1].StartsWith("/")) Destination = args[1];
                if (args.Length >= 2 && args[1].StartsWith("/"))
                {
                    Destination = Source;
                    EvaluateArgument(args[1]);
                }
                if (args.Length >= 3) EvaluateArgument(args[2]);
                if (args.Length >= 4) EvaluateArgument(args[3]);
                if (args.Length >= 5) EvaluateArgument(args[4]);
                Ready = true;
            }
            catch (Exception ex)
            {
                label3.Text = "Error: " + ex.Message;
            }
            label1.Text = "Now processing: " + Source;
            label2.Text = "Destination: " + Destination;
            if (Ready) backgroundWorker1.RunWorkerAsync();
        }

        /// <summary>
        /// Evaluates an argument
        /// </summary>
        /// <param name="arg"></param>
        public void EvaluateArgument(string arg)
        {
            if (arg.StartsWith("/g=")) GetTop = Convert.ToInt32(arg.Substring(3));
            if (arg.StartsWith("/s=")) SkipTop = Convert.ToInt32(arg.Substring(3));
            if (arg.StartsWith("/c")) ClearBlank = true;
        }

        /// <summary>
        /// Performs processing
        /// </summary>
        public void Process()
        {
            Bitmap bmp1 = null;
            Bitmap bmp2 = null;
            FileStream fs = null;
            try
            {
                fs = File.Open( Source, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                bmp1 = new Bitmap( fs);
                fs.Close();
                fs = null;
                if (GetTop == 0) GetTop = bmp1.Height - SkipTop;
                string Message = "Skipping " + SkipTop.ToString() + " pixels from top. ";
                Message += "Getting " + GetTop.ToString() + " pixels. ";
                backgroundWorker1.ReportProgress( 0, Message);
                int top = SkipTop;
                int bottom = SkipTop + GetTop;
                if (ClearBlank)
                {
                    for (int j = 2; j <= GetTop; j++)
                    {
                        bool emptyLine = true;
                        for (int k = 0; k < bmp1.Width; k++)
                        {
                            Color c = bmp1.GetPixel(k, bottom - j);
                            if (c.R > 200 && c.G > 200 && c.B > 200) continue;
                            emptyLine = false;
                            break;
                        }
                        if (emptyLine) continue;
                        j--;
                        Message += "Skipping " + j.ToString() + " pixels from bottom.";
                        backgroundWorker1.ReportProgress(0, Message);
                        bottom -= j;
                        break;
                    }
                }
                if (bottom <= top) throw new Exception("Nothing to plot");
                bmp2 = new Bitmap( bmp1.Width, bottom-top);
                for (int i = 0; i < bmp2.Width; i++)
                {
                    if( (i%100) == 0)
                        backgroundWorker1.ReportProgress(100 * i / bmp2.Width, Message);
                    for (int j = 0; j < bmp2.Height; j++)
                    {
                        int k = top + j;
                        if (k >= bmp1.Height) continue;
                        Color c = bmp1.GetPixel(i, top + j);
                        bmp2.SetPixel( i, j, c);
                    }
                }
                Save(bmp2);
            }
            catch (Exception ex)
            {
                string Message = "Error: " + ex.Message.Replace( "\n", "").Replace( "\r", "");
                backgroundWorker1.ReportProgress(0, Message);
                Error = true;
            }
            finally
            {
                if (fs != null) fs.Close();
                if (bmp2 != null) bmp2.Dispose();
                if (bmp1 != null) bmp1.Dispose();
            }
        }

        /// <summary>
        /// Performs file save
        /// </summary>
        /// <param name="bmp"></param>
        public void Save(Bitmap bmp)
        {
            FileInfo fi = new FileInfo(Destination);
            switch (fi.Extension.ToLower())
            {
                case ".png":
                    bmp.Save(Destination, System.Drawing.Imaging.ImageFormat.Png);
                    break;
                case ".tif":
                case ".tiff":
                    bmp.Save(Destination, System.Drawing.Imaging.ImageFormat.Tiff);
                    break;
                case ".jpg":
                case ".jpeg":
                    bmp.Save(Destination, System.Drawing.Imaging.ImageFormat.Jpeg);
                    break;
                case ".bmp":
                    bmp.Save(Destination, System.Drawing.Imaging.ImageFormat.Bmp);
                    break;
                case ".emf":
                    bmp.Save(Destination, System.Drawing.Imaging.ImageFormat.Emf);
                    break;
                case ".gif":
                    bmp.Save(Destination, System.Drawing.Imaging.ImageFormat.Gif);
                    break;
                case ".wmf":
                    bmp.Save(Destination, System.Drawing.Imaging.ImageFormat.Wmf);
                    break;
                default: break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Process();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            timer1.Enabled = !Error;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            label3.Text = (string)e.UserState;
            progressBar1.Value = e.ProgressPercentage;
        }
    }
}