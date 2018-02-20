using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Petronode.OilfieldFileAccess.Converters;
using Petronode.OilfieldFileAccess.LAS;

namespace Petronode.AUS_Geomag
{
    public partial class Form1 : Form
    {
        private AGRFGeomagRequest agrf = new AGRFGeomagRequest();
        private const string standardResponse = 
            "Check inputs and click <Send Request> to get parameters from geoAGRF website";

        public Form1()
        {
            InitializeComponent();
        }
        public Form1( string filename)
        {
            InitializeComponent();
            textBox1.Text = filename;
        }

        /// <summary>
        /// Locate a LAS file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            textBox1.Text = openFileDialog1.FileName;
        }

        /// <summary>
        /// Copy response to clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(textBox6.Text);
            }
            catch (Exception ex)
            {
                label7.Text = "Error: " + ex.Message;
            }
        }

        /// <summary>
        /// Copy response to clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(textBox7.Text);
            }
            catch (Exception ex)
            {
                label7.Text = "Error: " + ex.Message;
            }
        }

        /// <summary>
        /// Send the request to website and gets the reply
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                textBox2.Text = agrf.lat.ToString();
                textBox3.Text = agrf.lon.ToString();
                textBox4.Text = agrf.date.ToString("dd-MMM-yyyy");
                textBox5.Text = agrf.depth.ToString("0.0");
                textBox6.Text = "";
                textBox7.Text = "";
                label7.Text = "Waiting for reply...";
                this.Refresh();
                agrf.RequestData();
                label7.Text = agrf.AGRFsite + " has replied:";
                textBox6.Text = agrf.Declination.ToString();
                textBox7.Text = agrf.FieldAmplitude.ToString();
            }
            catch (Exception ex)
            {
                label7.Text = "Error: " + ex.Message;
            }
        }

        /// <summary>
        /// Creates a remark string
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder( "Referenced to True North: D = " );
                sb.Append(textBox6.Text);
                sb.Append(" deg, F = ");
                sb.Append(textBox7.Text);
                sb.Append(" nT.");
                Clipboard.SetText(sb.ToString());
            }
            catch (Exception ex)
            {
                label7.Text = "Error: " + ex.Message;
            }
        }

        /// <summary>
        /// LAS file parsing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            label5.Visible = false;
            if (!File.Exists(textBox1.Text))
            {
                label7.Text = "File does not exist";
                return;
            }
            try
            {
                LAS_File lf = new LAS_File(textBox1.Text, false);
                double start = 0.0;
                double stop = 0.0;
                foreach (LAS_Constant lc in lf.Constants)
                {
                    if (lc.Name.ToUpper().StartsWith("LATI"))
                        textBox2.Text = ReplaceFunnyChars( lc.Value);
                    if (lc.Name.ToUpper().StartsWith("LONG"))
                        textBox3.Text = ReplaceFunnyChars(lc.Value);
                    if (lc.Name.ToUpper().StartsWith("DATE")) textBox4.Text = lc.Value;
                    if (lc.Name.ToUpper().StartsWith("STRT")) start = Convert.ToDouble(lc.Value);
                    if (lc.Name.ToUpper().StartsWith("STOP")) stop = Convert.ToDouble(lc.Value);
                }
                double d = (start + stop) * 0.5;
                textBox5.Text = d.ToString( "0.0");
            }
            catch (Exception ex)
            {
                label7.Text = "Error: " + ex.Message;
            }
        }

        /// <summary>
        /// Removes degrees and other funny chars from the LAT-LONG strings
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string ReplaceFunnyChars(string s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c < ' ' || c > 'z') c = ' ';
                sb.Append(c);
            }
            return sb.ToString().Replace("DMS", "");
        }

        /// <summary>
        /// User manually changes inputs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessEntries(object sender, EventArgs e)
        {
            try
            {
                agrf.lat.AngleS = textBox2.Text;
                agrf.lon.AngleS = textBox3.Text;
                agrf.date = Convert.ToDateTime(textBox4.Text);
                agrf.depth = Convert.ToDouble(textBox5.Text);
                label5.Visible = !(agrf.IsEasternAustralia);
                label7.Text = standardResponse;
            }
            catch (Exception ex)
            {
                label7.Text = "Error: " + ex.Message;
            }
        }
    }
}