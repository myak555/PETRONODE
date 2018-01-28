using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Petronode.CommonControls;

namespace Petronode.ColorPicker
{
    public partial class Form1 : Form
    {
        Petronode.ColorPicker.Properties.Settings frmSettings =
            new Petronode.ColorPicker.Properties.Settings();

        public int ReturnValue = -1;
        public bool ReturnOnClick = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public Form1(string[] args)
        {
            InitializeComponent();
            colorSelectorControl1.V = Convert.ToInt32(numericUpDown1.Value);
            colorSelectorControl1.OnVolumeChange += new ColorSelectorControl.VolumeChangeDelegate(ChangeVolume);
            colorSelectorControl1.OnColorChange += new ColorSelectorControl.ColorChangeDelegate(ChangeColor);
            defaultColorControl1.OnColorChange += new DefaultColorControl.ColorChangeDelegate(ChangeColor);
            customColorControl1.OnColorChange += new CustomColorControl.ColorChangeDelegate(ChangeColor);
            SetConfigFromArguments(frmSettings.Petronode_ColorPicker_Configuration);
            if (args.Length == 1)
            {
                if (args[0].StartsWith("/r"))
                {
                    ReturnOnClick = true;
                    return;
                }
                SetConfigFromArguments(args[0]);
            }
            if (args.Length >= 2)
            {
                ReturnOnClick = args[0].StartsWith("/r");
                SetConfigFromArguments(args[1]);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string color1 = ColorParser.GetColorString(panel1.BackColor);
            string color2 = customColorControl1.GetConfigString();
            frmSettings.Petronode_ColorPicker_Configuration = color1 + color2;
            frmSettings.Save();
        }

        private void SetConfigFromArguments(string config)
        {
            if (config.Length < 6)
            {
                numericUpDown2.Value = 255;
                numericUpDown3.Value = 255;
                numericUpDown4.Value = 255;
            }
            else
            {
                string s = config.Substring(0, 6);
                Color c = ColorParser.GetColor(s);
                numericUpDown2.Value = (decimal)c.R;
                numericUpDown3.Value = (decimal)c.G;
                numericUpDown4.Value = (decimal)c.B;
                customColorControl1.SetConfigString(config.Substring(6));
            }
        }

        private void volumeUpDown_ValueChanged(object sender, EventArgs e)
        {
            colorSelectorControl1.V = Convert.ToInt32(numericUpDown1.Value);
        }

        private void colorUpDown_ValueChanged(object sender, EventArgs e)
        {
            decimal max = numericUpDown2.Value;
            if (max < numericUpDown3.Value) max = numericUpDown3.Value;
            if (max < numericUpDown4.Value) max = numericUpDown4.Value;
            numericUpDown1.Value = max;
            int r = Convert.ToInt32(numericUpDown2.Value);
            int g = Convert.ToInt32(numericUpDown3.Value);
            int b = Convert.ToInt32(numericUpDown4.Value);
            panel1.BackColor = Color.FromArgb(255, r, g, b);
        }

        private void ChangeVolume(int v)
        {
            numericUpDown1.Value = v;
        }

        private void ChangeColor(Color c)
        {
            numericUpDown2.Value = (decimal)c.R;
            numericUpDown3.Value = (decimal)c.G;
            numericUpDown4.Value = (decimal)c.B;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText( ColorParser.GetColorString(panel1.BackColor));
            ReturnValue = ((int)numericUpDown2.Value << 16) + ((int)numericUpDown3.Value << 8) + ((int)numericUpDown4.Value);
            if (ReturnOnClick) Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            customColorControl1.AddCustomColor(panel1.BackColor);
        }
    }
}