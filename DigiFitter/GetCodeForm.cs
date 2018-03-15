using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Petronode.CommonData;

namespace Petronode.DigiFitter
{
    public partial class GetCodeForm : Form
    {
        private DigitizerFile m_Data = null;

        public GetCodeForm()
        {
            InitializeComponent();
        }

        public string Variable
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }

        public string Function
        {
            get { return textBox2.Text; }
            set { textBox2.Text = value; }
        }

        public DigitizerFile Data
        {
            get { return m_Data; }
            set
            {
                m_Data = value;
                textBox1_TextChanged(this, null);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (m_Data == null) return;
            StringBuilder sb = new StringBuilder();
            foreach (FitterFunction ff in m_Data.Solution.Functions)
            {
                sb.Append(textBox1.Text);
                sb.Append(ff.ToString());
                sb.Append(textBox2.Text);
                sb.Append("\r\n");
            }
            richTextBox1.Text = sb.ToString();
        }
    }
}