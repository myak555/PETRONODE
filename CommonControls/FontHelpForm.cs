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
    public partial class FontHelpForm : Form
    {
        public FontHelpForm()
        {
            InitializeComponent();
            FontFamily[] ff = FontFamily.Families;
            StringBuilder sb = new StringBuilder();
            sb.Append("This machine has the following fonts:\n");
            sb.Append("\n");
            try
            {
                for (int i = 0; i < ff.Length; i++)
                {
                    sb.Append(ff[i].Name);
                    sb.Append("\n");
                }
                richTextBox1.Text = sb.ToString();
                richTextBox1.HideSelection = false;
                richTextBox1.Select(0, 0);
                richTextBox1.ScrollToCaret();
            }
            catch (Exception) { }
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