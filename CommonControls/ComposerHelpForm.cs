using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Petronode.CommonControls
{
    public partial class ComposerHelpForm : Form
    {
        public ComposerHelpForm()
        {
            InitializeComponent();
            string filename = Application.StartupPath + "\\Slide Composer.txt";
            if (!File.Exists(filename)) return;
            FileStream fs = null;
            StreamReader sr = null;
            try
            {
                fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                sr = new StreamReader(fs);
                string r = sr.ReadToEnd();
                richTextBox1.Text = r;
                richTextBox1.HideSelection = false;
                richTextBox1.Select(0, 0);
                richTextBox1.ScrollToCaret();
            }
            catch (Exception) { }
            finally
            {
                if( sr != null) sr.Close();
                if( fs != null) fs.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length <= 0) return;
            int i = richTextBox1.SelectionStart;
            string s = richTextBox1.Text.Substring( 0, i);
            int j = s.LastIndexOf( textBox1.Text);
            if (j < 0) return;
            richTextBox1.Select(j, textBox1.Text.Length);
            //richTextBox1.ScrollToCaret();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length <= 0) return;
            int i = richTextBox1.SelectionStart + richTextBox1.SelectionLength;
            int j = richTextBox1.Text.IndexOf(textBox1.Text, i);
            if (j < 0) return;
            richTextBox1.Select(j, textBox1.Text.Length);
            //richTextBox1.ScrollToCaret();
        }
    }
}