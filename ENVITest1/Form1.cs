using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Petronode.ENVIAccess;

namespace Petronode.ENVITest1
{
    public partial class Form1 : Form
    {
        private readonly Petronode.ENVITest1.Properties.Settings frmSettings
            = new Petronode.ENVITest1.Properties.Settings();

        private ENVIDataCube m_DataCube_Test1 = null;

        public Form1(string[] args)
        {
            InitializeComponent();
            textBox1.Text = (args.Length > 0) ? args[0] : frmSettings.ENVI_File_Name;
        }

        /// <summary>
        /// Saves everything on form closure
        /// </summary>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmSettings.ENVI_File_Name = textBox1.Text;
            frmSettings.Save();
        }

        /// <summary>
        /// Sets the file name
        /// </summary>
        private void Button1_Click(object sender, EventArgs e)
        {
            if (File.Exists(textBox1.Text))
                openFileDialog1.FileName = textBox1.Text;
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            textBox1.Text = openFileDialog1.FileName;
        }

        /// <summary>
        /// Runs regression testing 
        /// </summary>
        private void Button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            dataGridView1.Rows.Add(Test1());
            //dataGridView1.Rows.Add(Test2());
            //dataGridView1.Rows.Add(Test3());
            dataGridView1.Rows.Add(Test4());
        }

        private string[] Test1()
        {
            string[] result = new string[3];
            result[0] = "01. Load ENVI file";
            result[1] = "";
            result[2] = "Fail";
            try
            {
                m_DataCube_Test1 = new ENVIDataCube(textBox1.Text);
                if (m_DataCube_Test1.isValid)
                {
                    result[2] = "Pass";
                }
                else
                {
                    result[1] = "Cube is invalid";
                }
            }
            catch (Exception ex)
            {
                result[1] = ex.Message;
            }
            return result;
        }

        private string[] Test2()
        {
            string[] result = new string[3];
            result[0] = "02. Save ENVI file";
            result[1] = "";
            result[2] = "Fail";
            try
            {
                if (!m_DataCube_Test1.isValid)
                {
                    result[1] = "Cube is invalid";
                    return result;
                }
                string newName = m_DataCube_Test1.Filename.Replace(".hdr", "_saved.hdr");
                if( newName == m_DataCube_Test1.Filename)
                {
                    result[1] = "Could not find .hdr in filename";
                    return result;
                }
                m_DataCube_Test1.Save( newName);
                ENVIDataCube m_DataCube_Test2 = new ENVIDataCube(newName);
                if (m_DataCube_Test2.isValid)
                {
                    result[2] = "Pass";
                }
                else
                {
                    result[1] = "Saved cube is invalid";
                }
                m_DataCube_Test2 = null;
            }
            catch (Exception ex)
            {
                result[1] = ex.Message;
            }
            return result;
        }
        
        private string[] Test3()
        {
            string[] result = new string[3];
            result[0] = "03. Crop data cube";
            result[1] = "";
            result[2] = "Fail";
            try
            {
                if (!m_DataCube_Test1.isValid)
                {
                    result[1] = "Cube is invalid";
                    return result;
                }
                string newName = m_DataCube_Test1.Filename.Replace(".hdr", "_crop.hdr");
                if (newName == m_DataCube_Test1.Filename)
                {
                    result[1] = "Could not find .hdr in filename";
                    return result;
                }
                ENVIDataCube m_DataCube_Test3 = m_DataCube_Test1.GetCropped( 10, m_DataCube_Test1.Lines-10,
                    20, m_DataCube_Test1.Samples - 20, 500.0, 2300.0);
                m_DataCube_Test3.Save(newName);
                if (m_DataCube_Test3.isValid)
                {
                    result[2] = "Pass";
                }
                else
                {
                    result[1] = "Saved cube is invalid";
                }
                m_DataCube_Test3 = null;
            }
            catch (Exception ex)
            {
                result[1] = ex.Message;
            }
            return result;
        }

        private string[] Test4()
        {
            string[] result = new string[3];
            result[0] = "04. Get resampled trace";
            result[1] = "";
            result[2] = "Fail";
            try
            {
                if (!m_DataCube_Test1.isValid)
                {
                    result[1] = "Cube is invalid";
                    return result;
                }
                m_DataCube_Test1.SetResampler( 1990.0, 2400.0, 1.0);
                ENVITrace trace1 = m_DataCube_Test1.GetPixelResampled(100, 100);
                ENVITrace trace2 = m_DataCube_Test1.GetPixelResampled(101, 101);
                double[] diff = new double[trace1.Data.Length];
                for( int i=0; i<diff.Length; ++i)
                {
                    diff[i] = trace1.Data[i] - trace2.Data[i];
                }
                result[1] = "Wl[" + trace1.Wavelengths[1].ToString() + "]=" + trace1.Data[1].ToString() + ", diff=" + diff[1].ToString();
                result[2] = "Pass";
            }
            catch (Exception ex)
            {
                result[1] = ex.Message;
            }
            return result;
        }
    }
}
