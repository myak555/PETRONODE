using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Petronode.CommonControls
{
    public partial class DefaultColorControl : UserControl
    {
        private int c_picX = 256;
        private int c_picY = 256;
        public List<Color> Colors = new List<Color>();

        /// <summary>
        /// Constructor, creates the control
        /// </summary>
        public DefaultColorControl()
        {
            InitializeComponent();
            PrepareArrays();
            Bitmap bmp = PrepareHSVBitmap();
            pictureBox1.Image = bmp;
        }

        public delegate void ColorChangeDelegate( Color c);
        public ColorChangeDelegate OnColorChange = null;

        #region Precomputed Presentation
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
                    if (index >= Colors.Count)
                    {
                        g.FillRectangle(Brushes.RoyalBlue, x, y, 30, 30);
                    }
                    else
                    {
                        SolidBrush b = new SolidBrush(Colors[index]);
                        g.FillRectangle(b, x, y, 30, 30);
                        b.Dispose();
                    }
                    g.DrawRectangle(p, x, y, 30, 30);
                    index++;
                }
            }
            p.Dispose();
            g.Dispose();
            return tmp;
        }

        private void PrepareArrays()
        {
            for (int i = 0, c = 255; i < 8; i++, c -= 25) Colors.Add(Color.FromArgb(255, c, c, c));
            Colors.Add(Color.Black);
            for (int i = 0, c = 255; i <= 8; i++, c -= 20) Colors.Add(Color.FromArgb(255, c, 0, 0));
            for (int i = 0, c = 255; i <= 8; i++, c -= 20) Colors.Add(Color.FromArgb(255, c, c>>1, 0));
            for (int i = 0, c = 255; i <= 8; i++, c -= 20) Colors.Add(Color.FromArgb(255, c, c, 0));
            for (int i = 0, c = 255; i <= 8; i++, c -= 20) Colors.Add(Color.FromArgb(255, 0, c, 0));
            for (int i = 0, c = 255; i <= 8; i++, c -= 20) Colors.Add(Color.FromArgb(255, 0, c, c));
            for (int i = 0, c = 255; i <= 8; i++, c -= 20) Colors.Add(Color.FromArgb(255, 0, 0, c));
            for (int i = 0, c = 255; i <= 8; i++, c -= 20) Colors.Add(Color.FromArgb(255, c, 0, c));
        }

        #endregion

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (OnColorChange == null) return;
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
            OnColorChange( Colors[index]);
        }
    }
}
