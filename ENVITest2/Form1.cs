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

namespace Petronode.ENVITest2
{
    public partial class Form1 : Form
    {
        private readonly Petronode.ENVITest2.Properties.Settings frmSettings
            = new Petronode.ENVITest2.Properties.Settings();

        private Petronode.ENVIAccess.ENVIDataCube m_DataCube = null;
        private string m_Filename = "";
        public Form1(string[] args)
        {
            InitializeComponent();
            m_Filename = (args.Length > 0) ? args[0] : frmSettings.ENVI_File_Name;
            splitContainer1.SplitterDistance = panel1.Width * 2 / 3;
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
        /// Save file as 
        /// </summary>
        private void Button2_Click(object sender, EventArgs e)
        {
            if (m_DataCube == null || !m_DataCube.isValid) return;
            if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;
            m_DataCube.Save(saveFileDialog1.FileName);
        }

        /// <summary>
        /// Saves everything on form closure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmSettings.ENVI_File_Name = textBox1.Text;
            frmSettings.Save();
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                m_DataCube = new ENVIDataCube(textBox1.Text);
                if (m_DataCube == null || !m_DataCube.isValid) return;
                string[] tmp = new string[2];
                dataGridView2.Rows.Clear();
                tmp[0] = "Lines";
                tmp[1] = m_DataCube.Lines.ToString();
                dataGridView2.Rows.Add(tmp);
                tmp[0] = "Samples";
                tmp[1] = m_DataCube.Samples.ToString();
                dataGridView2.Rows.Add(tmp);
                tmp[0] = "Bands";
                tmp[1] = m_DataCube.Bands.ToString();
                dataGridView2.Rows.Add(tmp);
                tmp[0] = "Data Type";
                tmp[1] = m_DataCube.DataType.ToString();
                dataGridView2.Rows.Add(tmp);
                tmp[0] = "Interleave";
                tmp[1] = m_DataCube.Interleave;
                dataGridView2.Rows.Add(tmp);
                tmp[0] = "Header Offset";
                tmp[1] = m_DataCube.HeaderOffset.ToString();
                dataGridView2.Rows.Add(tmp);
                tmp[0] = "Byte Order";
                tmp[1] = m_DataCube.ByteOrder.ToString();
                dataGridView2.Rows.Add(tmp);
                tmp[0] = "File Type";
                tmp[1] = m_DataCube.FileType;
                dataGridView2.Rows.Add(tmp);
                tmp[0] = "Data Ignore Value";
                tmp[1] = m_DataCube.DataIgnoreValue.ToString("F2");
                dataGridView2.Rows.Add(tmp);
                textBox2.Lines = m_DataCube.Description;
                dataGridView1.Rows.Clear();
                for( int index=1; index<=m_DataCube.Wavelengths.Length; index++)
                {
                    tmp[0] = index.ToString();
                    tmp[1] = m_DataCube.Wavelengths[index-1].ToString("F4");
                    dataGridView1.Rows.Add( tmp);
                }
                Bitmap bmp = new Bitmap(m_DataCube.Lines, m_DataCube.Samples);
                ENVISlice slice = m_DataCube.GetWavelengthSlice(900.0);
                for (int line = 0; line < slice.Lines; ++line)
                {
                    for (int sample = 0, j= slice.Samples - 1; sample < slice.Samples; ++sample,--j)
                    {
                        double v = slice.GetSample(line, sample);
                        bmp.SetPixel(line, j, Double.IsNaN(v) ? Color.Black : Color.LimeGreen);
                    }
                }
                pictureBox1.Image = bmp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "File error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = m_Filename;
        }
    }
}
