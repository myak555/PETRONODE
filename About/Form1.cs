using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Petronode.CommonControls;

namespace Petronode.About
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Form1(string[] args)
        {
            InitializeComponent();
            if (args.Length <= 0) return;
            int time = 0;
            try{ time = Convert.ToInt32( args[0]);}
            catch( Exception){}
            if( time <= 0) return;
            button1.Visible = false;
            timer1.Interval = time * 1000;
            timer1.Enabled = true;
        }

        /// <summary>
        /// Goes to the Petronode website in a browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            ToolLauncher.LaunchBrowser();
            Application.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}