using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Petronode.OilfieldFileAccess;
using Petronode.OilfieldFileAccess.Converters;
using Petronode.OilfieldFileAccess.SEGY;

namespace MY.Weatherford.EBCDIC_Set
{
    public partial class Form1 : Form
    {
        private MY.Weatherford.EBCDIC_Set.Properties.Settings frmSettings1
            = new MY.Weatherford.EBCDIC_Set.Properties.Settings();

        SEGY_File myFile = null;
        string headFile = null;
        public bool Completed = false;

        public Form1(string[] args)
        {
            InitializeComponent();
            try
            {
                if (args.Length <= 0) textBox1.Text = frmSettings1.LastSEGY;
                if (args.Length > 0) textBox1.Text = args[0];
                if (args.Length > 1) SetHeader(args[1]);
                if (args.Length > 2)
                {
                    string command = args[2].ToUpper();
                    if (command.StartsWith("/E"))
                    {
                        Completed = true;
                        return;
                    }
                    if (command.StartsWith("/S"))
                    {
                        SaveFile();
                        Completed = true;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmSettings1.LastSEGY = textBox1.Text;
            frmSettings1.Save();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            myFile = null;
            dataGridView1.Rows.Clear();
            if (!File.Exists(textBox1.Text)) return;
            myFile = new SEGY_File(textBox1.Text, false);
            string[] tmp = new string[2]; 
            for (int i = 1; i <= 40; i++)
            {
                tmp[0] = "C" + i.ToString().PadLeft(2,'0');
                tmp[1] = myFile.Parameters[i-1].Value;
                dataGridView1.Rows.Add(tmp);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            textBox1.Text = openFileDialog1.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() != DialogResult.OK) return;
            try
            {
                SetHeader( openFileDialog2.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;
            try
            {
                SaveHeader( saveFileDialog1.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (File.Exists(textBox1.Text))
            {
                FileInfo fi = new FileInfo(textBox1.Text);
                folderBrowserDialog1.SelectedPath = fi.DirectoryName;
            }
            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK) return;
            Folder_Process_Form fpf = new Folder_Process_Form(
                folderBrowserDialog1.SelectedPath, PrepareEBCDIC());
            fpf.ShowDialog();
        }

        private void SetHeader(string name)
        {
            headFile = name;
            if (!File.Exists(headFile)) return;
            if (myFile == null) return;
            FileStream fs = File.Open(headFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fs);
            for (int i = 0; i < 40; i++)
            {
                string s = sr.ReadLine();
                if (s == null)
                {
                    s = "                                                                                ";
                }
                dataGridView1.Rows[i].Cells[1].Value = s;
                myFile.Parameters[i].Value = s;
            }
            fs.Close();
        }

        private void SaveFile()
        {
            if (myFile == null) return;
            byte[] EBCDICHeader = PrepareEBCDIC();
            FileStream fs = File.Open(myFile.FileName, FileMode.Open, FileAccess.Write, FileShare.Read);
            fs.Write(EBCDICHeader, 0, EBCDICHeader.Length);
            fs.Close();
        }

        private void SaveHeader( string file)
        {
            if (myFile == null) return;
            FileStream fs = File.Open(file, FileMode.Create, FileAccess.Write, FileShare.Read);
            StreamWriter sw = new StreamWriter(fs);
            for (int i = 0; i < 40; i++)
            {
                string s = dataGridView1.Rows[i].Cells[1].Value.ToString();
                if (s == null)
                {
                    s = "                                                                                ";
                }
                if (s.Length > 80) s = s.Substring(0, 80);
                if (s.Length < 80) s = s.PadRight(80);
                myFile.Parameters[i].Value = s;
                sw.WriteLine(s);
            }
            fs.Close();
        }

        private byte[] PrepareEBCDIC()
        {
            byte[] EBCDICHeader = new byte[3200];
            NumberUnion fu = new NumberUnion();
            for (int i = 0; i < 40; i++)
            {
                string s = dataGridView1.Rows[i].Cells[1].Value.ToString();
                if (s == null)
                {
                    s = "                                                                                ";
                }
                if (s.Length > 80) s = s.Substring(0, 80);
                if (s.Length < 80) s = s.PadRight(80);
                myFile.Parameters[i].Value = s;
                BufferConverter.SetEBCDICBytesString(EBCDICHeader, fu, s, 80, 80 * i);
            }
            return EBCDICHeader;
        }
    }
}