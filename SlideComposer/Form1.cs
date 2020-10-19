using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using Petronode.CommonData;
using Petronode.CommonControls;
using Petronode.SlideComposerLibrary;

namespace Petronode.SlideComposer
{
    public partial class Form1 : Form
    {
        Petronode.SlideComposer.Properties.Settings frmSettings
            = new Petronode.SlideComposer.Properties.Settings();

        /// <summary>
        /// Constructor, creates the form
        /// </summary>
        /// <param name="args"></param>
        public Form1(string[] args)
        {
            InitializeComponent();
            textBox1.Text = (args.Length > 0)? args[0]: frmSettings.SlideComposer_Definition;
            textBox3.Text = (args.Length > 1)? args[1]: frmSettings.Slide_Composer_Input;
            textBox4.Text = (args.Length > 2)? args[2]: frmSettings.Slide_Composer_Output;
            toolLauncherControl1.HelpLocation = "SlideComposer";
        }

        /// <summary>
        /// Sets definition file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if( File.Exists( textBox1.Text))
                openFileDialog1.FileName = textBox1.Text;
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            textBox1.Text = openFileDialog1.FileName;
        }

        /// <summary>
        /// Sets input folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textBox3.Text))
                folderBrowserDialog1.SelectedPath = textBox3.Text;
            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK) return;
            textBox3.Text = folderBrowserDialog1.SelectedPath;
        }

        /// <summary>
        /// Sets output folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textBox4.Text))
                folderBrowserDialog1.SelectedPath = textBox4.Text;
            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK) return;
            textBox4.Text = folderBrowserDialog1.SelectedPath;
        }

        /// <summary>
        /// Saves everything on form closure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if( backgroundWorker1.IsBusy) backgroundWorker1.CancelAsync();
            while (backgroundWorker1.IsBusy) Thread.Sleep(1000);
            SaveDefinitions();
            frmSettings.SlideComposer_Definition = textBox1.Text;
            frmSettings.Slide_Composer_Input = textBox3.Text;
            frmSettings.Slide_Composer_Output = textBox4.Text;
            frmSettings.Save();
        }

        /// <summary>
        /// Updates the editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!File.Exists(textBox1.Text)) return;
            FileStream fs = null;
            StreamReader sr = null;
            try
            {
                fs = File.Open(textBox1.Text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                sr = new StreamReader(fs);
                richTextBox2.Text = sr.ReadToEnd();
            }
            catch( Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if( sr != null) sr.Close();
                if( fs != null) fs.Close();
            }
        }

        /// <summary>
        /// Saves the definition file
        /// </summary>
        /// <returns></returns>
        private bool SaveDefinitions()
        {
            if (textBox1.Text.Length <= 1) return true;
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                fs = File.Open(textBox1.Text, FileMode.Create, FileAccess.Write, FileShare.Read);
                sw = new StreamWriter(fs);
                sw.Write(richTextBox2.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            finally
            {
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }
            return false;
        }

        private SlideDescriptionFile sdp = null;
        private List<string> inputList = new List<string>();
        private List<string> outputList = new List<string>();
        private string outputMessage = "";

        /// <summary>
        /// Performs processing of a single folder; also handles "Cancel" press
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (button4.Text == "Cancel")
            {
                backgroundWorker1.CancelAsync();
            }
            if (backgroundWorker1.IsBusy) return;
            if (SaveDefinitions()) return;
            try
            {
                sdp = new SlideDescriptionFile(textBox1.Text);
                inputList.Clear();
                outputList.Clear();
                inputList.Add(textBox3.Text);
                outputList.Add(textBox4.Text);
                outputMessage = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SetControls(false);
            backgroundWorker1.RunWorkerAsync();
        }

        /// <summary>
        /// Performs processing of multiple folders
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy) return;
            if (SaveDefinitions()) return;
            try
            {
                sdp = new SlideDescriptionFile(textBox1.Text);
                inputList.Clear();
                outputList.Clear();
                DirectoryInfo di = new DirectoryInfo(textBox3.Text);
                if (!di.Exists) throw new Exception(textBox3.Text + "not found");
                DirectoryInfo[] dis = di.GetDirectories();
                if (!Directory.Exists(textBox4.Text)) Directory.CreateDirectory(textBox4.Text);
                foreach (DirectoryInfo dd in dis)
                {
                    string src = textBox3.Text + "\\" + dd.Name;
                    string dst = textBox4.Text + "\\" + dd.Name;
                    inputList.Add(src);
                    outputList.Add(dst);
                }
                outputMessage = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SetControls(false);
            backgroundWorker1.RunWorkerAsync();
        }

        /// <summary>
        /// Sets the form controls
        /// </summary>
        /// <param name="status"></param>
        private void SetControls(bool status)
        {
            textBox1.Enabled = status;
            richTextBox2.Enabled = status;
            textBox3.Enabled = status;
            textBox4.Enabled = status;
            button1.Enabled = status;
            button2.Enabled = status;
            button3.Enabled = status;
            button4.Text = status? "Process Folder (F5)": "Cancel";
            button5.Enabled = status;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < inputList.Count; i++)
            {
                backgroundWorker1.ReportProgress( (i+1)*100/(inputList.Count+1));
                if( backgroundWorker1.CancellationPending)
                {
                    outputMessage = "Process aborted by user";
                    return;
                }
                try
                {
                    sdp.ComposeSlides(inputList[i], outputList[i]);
                }
                catch (Exception ex)
                {
                    outputMessage = ex.Message;
                    break;
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetControls(true);
            progressBar1.Value = 0;
            if (outputMessage == "")
            {
                label4.Visible = true;
                timer1.Enabled = true;
                //MessageBox.Show("Success: Processing completed for " + textBox4.Text, "Done", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                MessageBox.Show("Error: " + outputMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label4.Visible = false;
            timer1.Enabled = false;
        }

        private void richTextBox2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                e.Handled = true;
                ToolLauncher.LaunchLocalHelp("SlideComposer");
            }
            if (!backgroundWorker1.IsBusy && e.KeyCode == Keys.F5)
            {
                e.Handled = true;
                this.button4_Click(this, new EventArgs());
            }
            if (backgroundWorker1.IsBusy && e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                this.button4_Click(this, new EventArgs());
            }
        }
    }
}