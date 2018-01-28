using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Petronode.CommonControls
{
    public partial class ColorSelectorControl : UserControl
    {
        private int c_picX = 256;
        private int c_picY = 256;
        private double[,] m_R = null;
        private double[,] m_G = null;
        private double[,] m_B = null;
        private double[,] m_S = null;
        private int m_V = 255;

        /// <summary>
        /// Constructor, creates the control
        /// </summary>
        public ColorSelectorControl()
        {
            InitializeComponent();
            PrepareArrays();
            Bitmap bmp = PrepareHSVBitmap();
            pictureBox1.Image = bmp;
        }

        /// <summary>
        /// Sets and returns the control volume
        /// </summary>
        public int V
        {
            get { return m_V; }
            set
            {
                m_V = value;
                if (m_V < 0) m_V = 0;
                if (m_V > 255) m_V = 255;
                Bitmap bmp = PrepareHSVBitmap();
                Image old_bmp = pictureBox1.Image;
                pictureBox1.Image = bmp;
                if (old_bmp != null) old_bmp.Dispose();
            }
        }

        public delegate void VolumeChangeDelegate(int v);
        public VolumeChangeDelegate OnVolumeChange = null;

        public delegate void ColorChangeDelegate( Color c);
        public ColorChangeDelegate OnColorChange = null;

        #region Precomputed Presentation
        private Bitmap PrepareHSVBitmap()
        {
            double dV = Convert.ToDouble(m_V) / 255.0;
            Bitmap tmp = new Bitmap(c_picX + 40, c_picY + 10);
            Graphics g = Graphics.FromImage(tmp);
            g.Clear(this.BackColor);

            // makes color circle
            for (int x = 0; x < c_picX; x++)
            {
                for (int y = 0; y < c_picY; y++)
                {
                    tmp.SetPixel(x + 5, y + 5, MixColor(x, y, dV));
                }
            }
            Pen p = new Pen(this.ForeColor, 2f);
            g.DrawEllipse(p, 4, 4, c_picX+2, c_picY+2);

            // makes intensity slider
            int loc_x = c_picX + 30;
            for (int y = 0; y < c_picY; y++)
            {
                int j = 255 - y;
                Color shade = Color.FromArgb(255, j, j, j);
                for (int x = c_picX + 10; x < loc_x; x++)
                    tmp.SetPixel(x, y + 5, shade);
            }
            int loc_y = 260 - m_V;
            g.DrawLine(p, loc_x + 4, loc_y - 4, loc_x + 5, loc_y - 4);
            g.DrawLine(p, loc_x + 3, loc_y - 3, loc_x + 5, loc_y - 3);
            g.DrawLine(p, loc_x + 2, loc_y - 2, loc_x + 5, loc_y - 2);
            g.DrawLine(p, loc_x + 1, loc_y - 1, loc_x + 5, loc_y - 1);
            g.DrawLine(p, loc_x, loc_y, loc_x + 5, loc_y);
            g.DrawLine(p, loc_x + 1, loc_y + 1, loc_x + 5, loc_y + 1);
            g.DrawLine(p, loc_x + 2, loc_y + 2, loc_x + 5, loc_y + 2);
            g.DrawLine(p, loc_x + 3, loc_y + 3, loc_x + 5, loc_y + 3);
            g.DrawLine(p, loc_x + 4, loc_y + 4, loc_x + 5, loc_y + 4);
            p.Dispose();
            g.Dispose();
            return tmp;
        }

        private void PrepareArrays()
        {
            m_R = new double[c_picX, c_picY];
            m_G = new double[c_picX, c_picY];
            m_B = new double[c_picX, c_picY];
            m_S = new double[c_picX, c_picY];
            for (int x = 0; x < c_picX; x++)
                for (int y = 0; y < c_picY; y++) m_S[x, y] = -1.0;
            for (double angle = 0.0; angle < 360.0; angle += 0.4)
            {
                double[] hue = ConvertHue(angle);
                double a = Math.PI * angle / 180.0;
                double sinA = Math.Sin(a);
                double cosA = Math.Cos(a);
                for (double radius = 0.0; radius < 128.0; radius += 0.4)
                {
                    int x = Convert.ToInt32(radius * sinA) + (c_picX >> 1);
                    int y = (c_picY >> 1) - Convert.ToInt32(radius * cosA);
                    if (x < 0) continue;
                    if (y < 0) continue;
                    if (x >= c_picX) continue;
                    if (y >= c_picY) continue;
                    m_S[x, y] = radius / 128.0;
                    m_R[x, y] = hue[0];
                    m_G[x, y] = hue[1];
                    m_B[x, y] = hue[2];
                }
            }
        }

        /// <summary>
        /// Converts hue from degrees to an array of non-normalized RGB
        /// </summary>
        /// <param name="Hue"></param>
        /// <returns></returns>
        private double[] ConvertHue(double Hue)
        {
            double[] tmp = new double[3];
            tmp[0] = 0.0;
            tmp[1] = 0.0;
            tmp[2] = 0.0;
            if (Hue < 0.0) return tmp;
            if (Hue < 60.0)
            {
                tmp[0] = 1.0;
                tmp[1] = Hue / 60.0;
                return tmp;
            }
            Hue -= 60.0;
            if (Hue < 60.0)
            {
                tmp[0] = 1.0 - Hue / 60.0;
                tmp[1] = 1.0;
                return tmp;
            }
            Hue -= 60.0;
            if (Hue < 60.0)
            {
                tmp[1] = 1.0;
                tmp[2] = Hue / 60.0;
                return tmp;
            }
            Hue -= 60.0;
            if (Hue < 60.0)
            {
                tmp[1] = 1.0 - Hue / 60.0;
                tmp[2] = 1.0;
                return tmp;
            }
            Hue -= 60.0;
            if (Hue < 60.0)
            {
                tmp[0] = Hue / 60.0;
                tmp[2] = 1.0;
                return tmp;
            }
            Hue -= 60.0;
            if (Hue < 60.0)
            {
                tmp[0] = 1.0;
                tmp[2] = 1.0 - Hue / 60.0;
                return tmp;
            }
            return tmp;
        }

        private Color MixColor(int x, int y, double v)
        {
            if (m_S[x, y] < 0) return this.BackColor;
            double C = v * m_S[x, y];
            double M = v - C;
            int r = Convert.ToInt32((m_R[x, y] * C + M) * 255.0);
            int g = Convert.ToInt32((m_G[x, y] * C + M) * 255.0);
            int b = Convert.ToInt32((m_B[x, y] * C + M) * 255.0);
            return Color.FromArgb(255, r, g, b);
        }
        #endregion

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            int x = e.X - 5;
            int y = e.Y - 5;
            if (x > c_picX)
            {
                if (y < 0) y = 0;
                if (y > 255) y = 255;
                if (OnVolumeChange != null) OnVolumeChange( 255 - y);
                return;
            }
            if( x < 0 || x >= c_picX) return;
            if( y < 0 || y >= c_picY) return;
            if( m_S[x,y] < 0.0) return;
            if (OnColorChange != null) OnColorChange( MixColor( x, y, Convert.ToDouble(m_V)/255.0));
        }
    }
}
