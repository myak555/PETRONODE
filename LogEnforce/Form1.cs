using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Petronode.OilfieldFileAccess.LAS;

namespace Petronode.LogEnforce
{
    public partial class Form1 : Form
    {
        private Petronode.LogEnforce.Properties.Settings frmSettings1
            = new Petronode.LogEnforce.Properties.Settings();
        private LAS_File las = null;
        private RuleList ruleList = null;

        public Form1()
        {
            InitializeComponent();
            textBox1.Text = frmSettings1.Last_LAS_File;
            textBox2.Text = frmSettings1.Rules_File;
        }

        public Form1(string LAS)
        {
            InitializeComponent();
            textBox1.Text = LAS;
            textBox2.Text = frmSettings1.Rules_File;
        }

        public Form1(string LAS, string Rules)
        {
            InitializeComponent();
            textBox1.Text = LAS;
            textBox2.Text = Rules;
        }

        public Form1(string LAS, string Rules, string Key)
        {
            InitializeComponent();
            textBox1.Text = LAS;
            textBox2.Text = Rules;
            if (Key == "/p") button3_Click(this, new EventArgs());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmSettings1.Last_LAS_File = textBox1.Text;
            frmSettings1.Rules_File = textBox2.Text;
            frmSettings1.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            textBox1.Text = openFileDialog1.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() != DialogResult.OK) return;
            textBox2.Text = openFileDialog2.FileName;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            checkBox1.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            int pass = 0;
            int warning = 0;
            int error = 0;
            try
            {
                if( !File.Exists( textBox1.Text)) throw new Exception( "File does not exist: " + textBox1.Text);
                if( !File.Exists( textBox2.Text)) throw new Exception( "File does not exist: " + textBox2.Text);
                las = new LAS_File(textBox1.Text, true);
                ruleList = new RuleList( textBox2.Text);
                ruleList.Enforce(las);
                checkBox1_CheckedChanged(this, new EventArgs());
                pass = ruleList.GetPassCount();
                warning = ruleList.GetWarningCount();
                error = ruleList.GetErrorCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK);
            }
            checkBox1.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            if (error > 0)
            {
                MessageBox.Show("Check completed with " + error.ToString() + " error(s) and " + warning.ToString()
                    + " warning(s). Do not deliver to Client before fixing it!", "Errors!", MessageBoxButtons.OK);
                return;
            }
            if (warning > 0)
            {
                MessageBox.Show("Check completed with no error(s) but " + warning.ToString()
                    + " warning(s). Fix as many as you can before delivering to Client!", "Warnings!", MessageBoxButtons.OK);
                return;
            }
            if (pass > 0)
            {
                MessageBox.Show("Check completed with " + pass.ToString()
                    + " rule(s) in Pass state. This log can be safely sent to client.", "Good job!", MessageBoxButtons.OK);
                return;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (las == null || ruleList == null) return;
            bool showErrorsOnly = (sender == button4);
            string reportName = las.FileName.ToUpper();
            reportName = reportName.Replace(".LAS", "_Enforce_Report.txt");
            if( File.Exists( reportName))
            {
                if( MessageBox.Show( "File " + reportName + " exists. Overwrite?",
                    "Confirm Overwrite", MessageBoxButtons.YesNo) == DialogResult.No) return;
            }
            StreamWriter sw = null;
            try
            {
                sw = File.CreateText( reportName);
                sw.WriteLine( "======================");
                sw.WriteLine( "= LogEnforce! Report =");
                sw.WriteLine( "======================");
                sw.WriteLine();
                sw.WriteLine("File : " + las.FileName);
                sw.WriteLine("Date : " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm"));
                sw.WriteLine("Level: " + (showErrorsOnly? "Errors and Warnings only": "Full Report"));
                sw.WriteLine();
                sw.WriteLine("======================");
                sw.WriteLine();
                foreach (Rule r in ruleList.Rules)
                {
                    if (r.Status == "Error" || r.Status == "Warning")
                    {
                        sw.WriteLine( r.ToString());
                        continue;
                    }
                    if (showErrorsOnly) continue;
                    sw.WriteLine( r.ToString());
                }
                sw.Close();
                sw = null;
                MessageBox.Show("Report saved to: " + reportName, "Report Saved", MessageBoxButtons.OK);
            }
            catch( Exception ex)
            {
                MessageBox.Show( "Error: " + ex.Message, "Error", MessageBoxButtons.OK);
            }
            finally
            {
                if( sw != null) sw.Close();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

            dataGridView1.Rows.Clear();
            if (ruleList == null) return;
            bool showErrorsOnly = checkBox1.Checked;
            foreach (Rule r in ruleList.Rules)
            {
                if (r.Status == "Error")
                {
                    dataGridView1.Rows.Add(r.ToStrings());
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2].Style.BackColor = Color.LightPink;
                    continue;
                }
                if (r.Status == "Warning")
                {
                    dataGridView1.Rows.Add(r.ToStrings());
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2].Style.BackColor = Color.LemonChiffon;
                    continue;
                }
                if (showErrorsOnly) continue;
                dataGridView1.Rows.Add(r.ToStrings());
                if( r.Status == "Pass")
                {
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2].Style.BackColor = Color.LightGreen;
                }
            }
        }
    }
}