using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Petronode.SeismicUIComponents;

namespace Petronode.Timepick
{
    public partial class Form1 : Form
    {
        private Petronode.Timepick.Properties.Settings frmSettings1
            = new Petronode.Timepick.Properties.Settings();

        public Form1( string[] args)
        {
            InitializeComponent();
            shot_Directory_Comparison1.Raw_Shot_Directory = (args.Length >= 1)?
                args[0] : frmSettings1.Seismic_Raw_Folder;
            shot_Directory_Comparison1.Selected_Shot_Directory = (args.Length >= 2)?
                args[1] : frmSettings1.Seismic_Selected_Shot_Folder;
            shot_Directory_Comparison1.Search_Pattern = (args.Length >= 3)?
                args[2] : frmSettings1.Seismic_File_Mask;
            shot_Directory_Comparison1.onSelectRow +=
                new Shot_Directory_Comparison.SelectRowDeleqate(this.FileSelectionChange);
            shot_Directory_Comparison1.LoadSelectedFile();
            timer1.Enabled = true;
            surface_Shot_View1.Time_Min = frmSettings1.Surface_Time_Min;
            surface_Shot_View1.Time_Max = frmSettings1.Surface_Time_Max;
            surface_Shot_View1.Vertical_Scale = frmSettings1.Surface_Vertical_Scale;
            surface_Shot_View1.Window_Size = frmSettings1.Surface_Window;
            surface_Shot_View1.SNR = frmSettings1.Surface_SNR;
            surface_Shot_View1.TuningType = frmSettings1.Surface_Tuning;
            downhole_Shot_View1.Time_Min = frmSettings1.Downhole_Time_Min;
            downhole_Shot_View1.Time_Max = frmSettings1.Downhole_Time_Max;
            downhole_Shot_View1.Vertical_Scale = frmSettings1.Downhole_Vertical_Scale;
            downhole_Shot_View1.Window_Start = frmSettings1.Downhole_Window_Start;
            downhole_Shot_View1.Window_Size = frmSettings1.Downhole_Window;
            downhole_Shot_View1.SNR = frmSettings1.Downhole_SNR;
            downhole_Shot_View1.TuningType = frmSettings1.Downhole_Tuning;
            downhole_Shot_View1.Show_Component = frmSettings1.Display_Component;
            downhole_Shot_View1.Pick_Component = frmSettings1.Timepick_Component;
        }

        /// <summary>
        /// Serves form closing event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmSettings1.Seismic_Raw_Folder = shot_Directory_Comparison1.Raw_Shot_Directory;
            frmSettings1.Seismic_Selected_Shot_Folder = shot_Directory_Comparison1.Selected_Shot_Directory;
            frmSettings1.Seismic_File_Mask = shot_Directory_Comparison1.Search_Pattern;
            frmSettings1.Surface_Time_Min = surface_Shot_View1.Time_Min;
            frmSettings1.Surface_Time_Max = surface_Shot_View1.Time_Max;
            frmSettings1.Surface_Vertical_Scale = surface_Shot_View1.Vertical_Scale;
            frmSettings1.Surface_Window = surface_Shot_View1.Window_Size;
            frmSettings1.Surface_SNR = surface_Shot_View1.SNR;
            frmSettings1.Surface_Tuning = surface_Shot_View1.TuningType;
            frmSettings1.Downhole_Time_Min = downhole_Shot_View1.Time_Min;
            frmSettings1.Downhole_Time_Max = downhole_Shot_View1.Time_Max;
            frmSettings1.Downhole_Vertical_Scale = downhole_Shot_View1.Vertical_Scale;
            frmSettings1.Downhole_Window_Start = downhole_Shot_View1.Window_Start;
            frmSettings1.Downhole_Window = downhole_Shot_View1.Window_Size;
            frmSettings1.Downhole_SNR = downhole_Shot_View1.SNR;
            frmSettings1.Downhole_Tuning = downhole_Shot_View1.TuningType;
            frmSettings1.Display_Component = downhole_Shot_View1.Show_Component;
            frmSettings1.Timepick_Component = downhole_Shot_View1.Pick_Component;
            frmSettings1.Save();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            shot_Directory_Comparison1.ScanDirectories();
        }

        private void FileSelectionChange(VSP_Shot shot, bool selected)
        {
            surface_Shot_View1.SetShotData(shot);
            downhole_Shot_View1.SetShotData(shot);
        }
    }
}