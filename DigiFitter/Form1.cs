using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using Petronode.CommonData;
using Petronode.CommonControls;

namespace Petronode.DigiFitter
{
    public partial class Form1 : Form
    {
        Petronode.DigiFitter.Properties.Settings frmSettings = new Petronode.DigiFitter.Properties.Settings();
        private bool m_saveOngoing = false;
        private DigitizerFile m_Data = new DigitizerFile();
        private GetCodeForm m_GetCodeForm = new GetCodeForm();
        
        #region Constructors
        public Form1(string[] args)
        {
            InitializeComponent();

            // Set geometry
            int splitDist = (splitContainer1.Width - 4) / 3;
            if (splitDist < 280) splitDist = 280;
            splitDist = splitContainer1.Width - 4 - splitDist;
            splitContainer1.SplitterDistance = splitDist;
            splitContainer1.Panel2MinSize = 275;

            // Assign events and data
            imageLoadControl1.OnFileLoadReceived +=
                new ImageLoadControl.OnFileLoadDelegate(LoadImage);
            fileLoadControl1.OnFileLoadReceived +=
                new FileLoadControl.OnFileOperationDelegate(LoadTextFile);
            fileLoadControl1.OnFileSaveReceived +=
                new FileLoadControl.OnFileOperationDelegate(SaveTextFile);
            fileLoadControl1.Filter =
                "Comma Separated Files|*.csv|All files|*.*";
            digitizerDataFitter1.Data = this.m_Data;
            digitizerDataFitter1.PictureDigitizer = 
                pictureDigitizerControl1;
            digitizerDataFitter1.OnPlotNext += 
                new DigitizerDataFitter.PlotNextDelegate(mistieControl1.Plot_Picks);
            mistieControl1.Data = m_Data;
            digitizerFunctionFitter1.Data = this.m_Data;
            digitizerFunctionFitter1.DataFitter = digitizerDataFitter1;
            digitizerFunctionFitter1.onMainFormAction +=
                new DigitizerFunctionFitter.ButtonActionDelegate( InternalControlAction);
            digitizerFunctionFitter1.onReportProgress +=
                new DigitizerFunctionFitter.ProgressReportDelegate( MinimizationProgress);
            toolLauncherControl1.OnGetArgs += new ToolLauncherControl.GetArgsDelegate(GetArgs);
            toolLauncherControl1.HelpLocation = "DigiFitter";

            // Process parameters
            if (args.Length == 0)
            {
                imageLoadControl1.FileName = frmSettings.Previous_Image;
                fileLoadControl1.FileName = frmSettings.Previous_File;
            }
            if (args.Length >= 1) imageLoadControl1.FileName = args[0];
            if (args.Length >= 2) fileLoadControl1.FileName = args[1];
            digitizerFunctionFitter1.FunctionSelected = frmSettings.Function_Selection;

            // Mouse events processing starts after everything is loaded
            pictureDigitizerControl1.OnDigitizerEventReceived +=
                new PictureDigitizerControl.OnDizitizerEventDelegate(OnDigitizerClick);
            mistieControl1.OnDigitizerEventReceived += new MistieControl.OnDizitizerEventDelegate(OnMistieClick);
            imageLoadControl1.OnMouseScrollReceived += new MouseEventHandler(OtherControlWheelMove);
            this.MouseWheel += new MouseEventHandler(pictureDigitizerControl1.MouseWheelMove);
            m_GetCodeForm.Variable = frmSettings.Variable_String;
            m_GetCodeForm.Function = frmSettings.Function_String;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (File.Exists(fileLoadControl1.FileName)) SaveTextFile(this);
            frmSettings.Previous_Image = imageLoadControl1.FileName;
            frmSettings.Previous_File = fileLoadControl1.FileName;
            frmSettings.Variable_String = m_GetCodeForm.Variable;
            frmSettings.Function_String = m_GetCodeForm.Function;
            frmSettings.Function_Selection = digitizerFunctionFitter1.FunctionSelected;
            frmSettings.Save();
        }
        #endregion

        #region File Loading
        private void LoadImage(object sender)
        {
            pictureDigitizerControl1.RefreshImageFromFile(imageLoadControl1.FileName);
            digitizerDataFitter1.Plot_Picks();
        }

        private void LoadTextFile(object sender)
        {
            string filename = fileLoadControl1.FileName;
            if (!File.Exists(filename)) return;
            this.Text = filename + " - PETRONODE Function Fitter";
            try
            {
                m_Data.Load(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            digitizerFunctionFitter1.Refresh_Data();
            digitizerDataFitter1.Refresh_Data();
        }

        private string[] GetArgs()
        {
            string[] tmp = new string[2];
            tmp[0] = imageLoadControl1.FileName;
            tmp[1] = fileLoadControl1.FileName;
            return tmp;
        }
        #endregion

        #region File Saving
        private void SaveTextFile(object sender)
        {
            string filename = fileLoadControl1.FileName;
            this.Text = filename + " - PETRONODE Function Fitter";
            m_saveOngoing = true;
            enableControls(false);
            try
            {
                m_Data.Save(filename, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
            finally
            {
                enableControls(true);
                m_saveOngoing = false;
            }
        }
        #endregion

        private void InternalControlAction( int action)
        {
            if (action == 2)
            {
                if (backgroundWorker1.IsBusy) return;
                if (!digitizerFunctionFitter1.PrepareMinimizationPartial()) return;
                enableControls(false);
                backgroundWorker1.RunWorkerAsync();
                return;
            }
            if (action == 3)
            {
                if (backgroundWorker1.IsBusy) return;
                if (!digitizerFunctionFitter1.PrepareMinimizationFull()) return;
                enableControls(false);
                backgroundWorker1.RunWorkerAsync();
                return;
            }
            if (action == 5)
            {
                m_GetCodeForm.Data = m_Data;
                m_GetCodeForm.ShowDialog(this);
                return;
            }
        }

        private void OtherControlWheelMove(object sender, MouseEventArgs e)
        {
            pictureDigitizerControl1.MouseWheelMove(sender, e);
        }

        private void OnDigitizerClick(int x, int y, int relative_x, int relative_y,
            string c, MouseEventArgs e, bool[] KeyModifiers)
        {
            if (m_saveOngoing) return;
            m_saveOngoing = true;
            digitizerFunctionFitter1.ProcessDigitizerClick(x, y, relative_x, relative_y, c, e, KeyModifiers);
            m_saveOngoing = false;
        }

        private void OnMistieClick(PointF pf, MouseEventArgs e, bool[] KeyModifiers)
        {
            if (m_saveOngoing) return;
            m_saveOngoing = true;
            digitizerFunctionFitter1.ProcessMistieClick(pf, e, KeyModifiers);
            m_saveOngoing = false;
        }

        private void enableControls(bool status)
        {
            digitizerFunctionFitter1.Enabled = status;
            digitizerDataFitter1.Enabled = status;
            pictureDigitizerControl1.Enabled = status;
        }

        private void MinimizationProgress(int percent, object info)
        {
            if (!backgroundWorker1.IsBusy) return;
            try
            {
                backgroundWorker1.ReportProgress(percent);
            }
            catch { }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            digitizerFunctionFitter1.RunMinimization();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pictureDigitizerControl1.CompletionPercent = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            digitizerFunctionFitter1.PresentMinimizationResults();
            pictureDigitizerControl1.CompletionPercent = 0;
            enableControls(true);
        }
    }
}