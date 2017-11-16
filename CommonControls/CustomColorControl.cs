using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Petronode.CommonControls
{
    public partial class CustomColorControl : UserControl
    {
        private int c_picX = 256;
        private int c_picY = 96;
        public List<Color> Colors = new List<Color>();

        /// <summary>
        /// Constructor, creates the control
        /// </summary>
        public CustomColorControl()
        {
            InitializeComponent();
            Bitmap bmp = PrepareHSVBitmap();
            pictureBox1.Image = bmp;
        }

        public delegate void ColorChangeDelegate( Color c);
        public ColorChangeDelegate OnColorChange = null;

        /// <summary>
        /// Sets the config string
        /// </summary>
        /// <param name="config">sting in format RRGGBB</param>
        public void SetConfigString(string config)
        {
            Colors.Clear();
            for (int i = 0; i < config.Length; i += 6)
            {
                string sub = config.Substring(i, 6);
                if (sub.Length < 6) break;
                Colors.Add( ColorConverter.GetColor(sub));
                if (Colors.Count >= 27) break;
                Image old_bmp = pictureBox1.Image;
                Bitmap bmp = PrepareHSVBitmap();
                pictureBox1.Image = bmp;
                if (old_bmp != null) old_bmp.Dispose();
            }
        }

        /// <summary>
        /// Gets the config string
        /// </summary>
        public string GetConfigString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Color c in Colors)
            {
                sb.Append( ColorConverter.GetColorString( c) );
            }
            return sb.ToString();
        }

        //
        // Adds a custom color to the form
        //
        public void AddCustomColor(Color c)
        {
            if (Colors.Count >= 27) Colors.RemoveAt(0);
            Colors.Add(c);
            Image old_bmp = pictureBox1.Image;
            Bitmap bmp = PrepareHSVBitmap();
            pictureBox1.Image = bmp;
            if (old_bmp != null) old_bmp.Dispose();
        }

        private Bitmap PrepareHSVBitmap()
        {
            //double dV = Convert.ToDouble(m_V) / 255.0;
            Bitmap tmp = new Bitmap(c_picX + 40, c_picY + 10);
            Graphics g = Graphics.FromImage(tmp);
            g.Clear(this.BackColor);
            Pen p = new Pen(this.ForeColor);
            int index = 0;
            for (int y = 5; y < c_picY; y+=32)
            {
                for (int x = 5; x < c_picX + 32; x+=32)
                {
                    Color c = (index >= Colors.Count)? this.BackColor: Colors[index];
                    SolidBrush b = new SolidBrush(c);
                    g.FillRectangle(b, x, y, 30, 30);
                    b.Dispose();
                    g.DrawRectangle(p, x, y, 30, 30);
                    index++;
                }
            }
            p.Dispose();
            g.Dispose();
            return tmp;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X - 5;
            int y = e.Y - 5;
            if (x > c_picX + 32) return;
            if (y > c_picY) return;
            int jx = x % 32;
            int jy = y % 32;
            if (jx == 0 || jx > 30) return;
            if (jy == 0 || jy > 30) return;
            int index = y / 32;
            index *= 9;
            index += x / 32;
            if (index < 0 || index >= Colors.Count) return;
            if (e.Button == MouseButtons.Left)
            {
                if (OnColorChange == null) return;
                OnColorChange(Colors[index]);
            }
            if (e.Button == MouseButtons.Right)
            {
                Colors.RemoveAt(index);
                Image old_bmp = pictureBox1.Image;
                Bitmap bmp = PrepareHSVBitmap();
                pictureBox1.Image = bmp;
                if (old_bmp != null) old_bmp.Dispose();
            }
        }
    }
}
