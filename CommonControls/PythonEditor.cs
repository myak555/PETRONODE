using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Petronode.CommonControls
{
    public partial class PythonEditor : UserControl
    {
        public PythonEditor()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Saves file to disk - presumably file update happens on the fly
        /// </summary>
        public void SaveTextFile(string filename)
        {
            //dataGridView1.SuspendLayout();
            //try
            //{
            //    Data.Save(filename);
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}
            //finally
            //{
            //    dataGridView1.ResumeLayout();
            //}
        }

        /// <summary>
        /// Loads file from disk, updates graphics and table
        /// </summary>
        public void LoadTextFile(string filename)
        {
            //dataGridView1.SuspendLayout();
            //dataGridView1.Rows.Clear();
            //try
            //{
            //    Data.Load(filename);
            //    Refresh_Headers();
            //    foreach (DigiTaserPoint p in Data.Points)
            //        dataGridView1.Rows.Add(p.ToStrings());
            //    PlotPicks();
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}
            //finally
            //{
            //    dataGridView1.ResumeLayout();
            //}
        }
    }
}
