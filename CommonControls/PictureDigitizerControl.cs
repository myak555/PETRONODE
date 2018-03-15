using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Petronode.CommonData;

namespace Petronode.CommonControls
{
    public partial class PictureDigitizerControl : UserControl
    {
        private int m_X = 0;
        private int m_Y = 0;
        private Bitmap m_Image = new Bitmap( 2,2);
        private Bitmap m_OverlayImage = new Bitmap(2, 2);
        private Bitmap m_ScreenImage = new Bitmap(2, 2);
        private int m_LastClickX = -1;
        private int m_LastClickY = -1;
        private bool m_SlowDrawFlg = false;

        public bool SetOverlay = true;

        /// <summary>
        /// Constructor
        /// </summary>
        public PictureDigitizerControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// sets and retrieves the current image
        /// </summary>
        public Bitmap Image
        {
            get { return m_Image; }
        }

        /// <summary>
        /// retrieves the current overlay image
        /// </summary>
        public Bitmap OverlayImage
        {
            get { return SetOverlay? m_OverlayImage: null; }
        }

        /// <summary>
        /// returns the displayed image copy 
        /// note that the original displayed image is volatile
        /// </summary>
        public Bitmap GetDisplayedImageCopy()
        {
            return new Bitmap(m_ScreenImage);
        }

        /// <summary>
        /// loads the image from file
        /// </summary>
        /// <param name="filename"></param>
        public void SetImage(string filename)
        {
            Bitmap bmp = GetImageFromFile(filename);
            SetNewImage(bmp);
        }

        /// <summary>
        /// loads the image from file
        /// </summary>
        /// <param name="img">Image</param>
        public void SetImage(Bitmap img)
        {
            if (img == null) img = new Bitmap(2, 2);
            SetNewImage(img);
        }

        /// <summary>
        /// performs the image refresh
        /// </summary>
        /// <param name="filename"></param>
        public void RefreshImageFromFile(string filename)
        {
            Bitmap bmp = GetImageFromFile(filename);
            SetExistingImage(bmp);
        }

        /// <summary>
        /// Retrieves the last click X
        /// </summary>
        public int LastClickX
        {
            get { return m_LastClickX; }
        }

        /// <summary>
        /// Retrieves the last click Y
        /// </summary>
        public int LastClickY
        {
            get { return m_LastClickY; }
        }

        /// <summary>
        /// Sets and retrieves completion percent
        /// </summary>
        public int CompletionPercent
        {
            get { return toolStripProgressBar1.Value; }
            set { toolStripProgressBar1.Value = value; }
        }

        /// <summary>
        /// Delegate to service the mouse clicks in the image
        /// </summary>
        /// <param name="x">x coordinate from left</param>
        /// <param name="y">y coordinate from top</param>
        /// <param name="c">Color under cursor</param>
        public delegate void OnDizitizerEventDelegate(int x, int y, int relative_x, int relative_y,
            string c, MouseEventArgs e, bool[] KeyModifiers);
        public OnDizitizerEventDelegate OnDigitizerEventReceived = null;

        /// <summary>
        /// gets image from file
        /// </summary>
        /// <param name="filename"></param>
        private Bitmap GetImageFromFile(string filename)
        {
            if (!File.Exists(filename)) return new Bitmap(2, 2);
            FileStream fs = null;
            try
            {
                fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return new Bitmap(fs);
            }
            catch (Exception)
            {
                return new Bitmap(2, 2);
            }
            finally
            {
                if (fs != null) fs.Close();
            }
        }

        /// <summary>
        /// Called on mouse wheel event in the parent form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MouseWheelMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.None) return;
            int scroll_move = e.Delta / ((GetAsyncKeyState(Keys.ShiftKey) < 0) ? 16 : 4);
            bool modifier_Control = GetAsyncKeyState(Keys.ControlKey) < 0;
            if (!modifier_Control)
            {
                int v = vScrollBar1.Value - scroll_move;
                if (v < 0) v = 0;
                if (v > vScrollBar1.Maximum) v = vScrollBar1.Maximum;
                vScrollBar1.Value = v;
            }
            else
            {
                int h = hScrollBar1.Value - scroll_move;
                if (h < 0) h = 0;
                if (h > hScrollBar1.Maximum) h = hScrollBar1.Maximum;
                hScrollBar1.Value = h;
            }
            ((HandledMouseEventArgs)e).Handled = true;
        }

        #region Private Methods
        private bool m_PreventDraw = false;

        private void SetNewImage( Bitmap bmp)
        {
            m_PreventDraw = true;
            m_X = 0;
            m_Y = 0;
            hScrollBar1.Value = 0;
            vScrollBar1.Value = 0;
            m_LastClickX = -1;
            m_LastClickY = -1;
            toolStripStatusLabel1.Text = "Origin: not set";
            m_Image.Dispose();
            m_Image = bmp;
            if (SetOverlay)
            {
                m_OverlayImage.Dispose();
                m_OverlayImage = new Bitmap(bmp.Width, bmp.Height);
                Graphics g = Graphics.FromImage(m_OverlayImage);
                g.Clear(Color.Transparent);
                g.Dispose();
            }
            m_PreventDraw = false;
            PaintControl();
        }

        private void SetExistingImage(Bitmap bmp)
        {
            if (bmp.Height != m_Image.Height || bmp.Width != m_Image.Width)
            {
                SetNewImage(bmp);
                return;
            }
            Bitmap prev = Image;
            m_Image = bmp;
            PaintControl();
            prev.Dispose();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            PaintControl();
        }

        private bool m_isPainting = false;

        private void PaintControl()
        {
            if (m_PreventDraw) return;
            if (m_isPainting) return;
            m_isPainting = true;
            int dx = pictureBox1.Width;
            int dy = pictureBox1.Height;
            Bitmap predisposed = m_ScreenImage;
            m_ScreenImage = new Bitmap(dx, dy);
            Graphics g = Graphics.FromImage(m_ScreenImage);
            g.Clear(Color.White);
            int hBar = 1;
            int vBar = 1;
            int hCng = 2;
            int vCng = 2;
            Rectangle srcRect = new Rectangle(m_X, m_Y, dx, dy);
            Rectangle dstRect = new Rectangle(0, 0, dx, dy);
            if (m_Image != null)
            {
                DrawMyImage(g, m_Image, dstRect, srcRect, m_ScreenImage);
                //DrawMyImage(m_Image, srcRect, m_ScreenImage);
                hBar = m_Image.Width;
                if (hBar < 1) hBar = 1;
                vBar = m_Image.Height;
                if (vBar < 1) vBar = 1;
                hCng = dx * hBar / m_Image.Width;
                if (hCng < 1) hCng = 1;
                vCng = dy * vBar / m_Image.Height;
                if (vCng < 1) vCng = 1;
            }
            if (SetOverlay && m_OverlayImage != null)
            {
                DrawMyImage(g, m_OverlayImage, dstRect, srcRect, m_ScreenImage);
                //DrawMyImage(m_OverlayImage, srcRect, m_ScreenImage);
            }
            g.Dispose();
            pictureBox1.Image = m_ScreenImage;
            if( predisposed != null) predisposed.Dispose();
            hScrollBar1.Maximum = hBar;
            vScrollBar1.Maximum = vBar;
            hScrollBar1.LargeChange = hCng;
            vScrollBar1.LargeChange = vCng;
            m_isPainting = false;
        }

        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            if (m_PreventDraw) return;
            m_X = hScrollBar1.Value;
            PaintControl();
        }

        private void vScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            if (m_PreventDraw) return;
            m_Y = vScrollBar1.Value;
            PaintControl();
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int x = m_X + e.X;
            int y = m_Y + e.Y;
            Color cc = Color.Black;
            if (x>=0 && y>=0 && x < m_Image.Width && y < m_Image.Height)
                cc = m_Image.GetPixel(x, y);
            string c = ColorParser.GetColorString(cc);
            bool[] modifier_Shift = new bool[3];
            modifier_Shift[0] = GetAsyncKeyState(Keys.ShiftKey) < 0;
            modifier_Shift[1] = GetAsyncKeyState(Keys.ControlKey) < 0;
            modifier_Shift[2] = GetAsyncKeyState(Keys.Alt) < 0;

            if (e.Button == MouseButtons.Left)
            {
                m_LastClickX = x;
                m_LastClickY = y;
                if (OnDigitizerEventReceived != null)
                    OnDigitizerEventReceived(x, y, 0, 0, c, e, modifier_Shift);
                StringBuilder sb = new StringBuilder();
                sb.Append("Origin: X=");
                sb.Append(m_LastClickX.ToString());
                sb.Append(", Y=");
                sb.Append(m_LastClickY.ToString());
                sb.Append(", C=");
                sb.Append(c);
                toolStripStatusLabel1.Text = sb.ToString();
                return;
            }
            if (e.Button == MouseButtons.Right)
            {
                int x1 = x;
                int y1 = y;
                if (m_LastClickX > 0) x1 = x - m_LastClickX;
                if (m_LastClickY > 0) y1 = y - m_LastClickY;
                if (OnDigitizerEventReceived == null) return;
                OnDigitizerEventReceived(x, y, x1, y1, c, e, modifier_Shift);
                return;
            }
            if (e.Button == MouseButtons.Middle)
            {
                if (OnDigitizerEventReceived == null) return;
                OnDigitizerEventReceived(x, y, 0, 0, c, e, modifier_Shift);
                return;
            }
            ((HandledMouseEventArgs)e).Handled = true;
        }

        private void DrawMyImage(Bitmap input, Rectangle src, Bitmap output)
        {
            for( int y=0; y<src.Height; y++)
            {
                if( y >= output.Height) continue;
                int yy = y + src.Top;
                if (yy >= input.Height) continue;
                for (int x = 0; x < src.Width; x++)
                {
                    if (x >= output.Width) continue;
                    int xx = x + src.Left;
                    if (xx >= input.Width) continue;
                    Color pix = input.GetPixel(xx, yy);
                    if (pix.A == 0) continue;
                    output.SetPixel(x, y, pix);
                }
            }
        }

        private void DrawMyImage(Graphics g, Bitmap input, Rectangle dstRect, Rectangle srcRect, Bitmap output)
        {
            if (srcRect.Right >= input.Width)
            {
                int dd = input.Width - srcRect.Right;
                int xx = srcRect.Width - dd;
                if (xx <= 0) return;
                srcRect.Width = xx;
                dstRect.Width = xx;
            }
            if (srcRect.Bottom >= input.Height)
            {
                int dd = input.Height - srcRect.Bottom;
                int yy = srcRect.Height - dd;
                if (yy <= 0) return;
                srcRect.Height = yy;
                dstRect.Height = yy;
            }
            if (m_SlowDrawFlg)
            {
                DrawMyImage(input, srcRect, output);
                return;
            }
            try
            {
                g.DrawImage(input, dstRect, srcRect, GraphicsUnit.Pixel);
            }
            catch (Exception)
            {
                m_SlowDrawFlg = true;
                //DrawMyImage(input, srcRect, output);
            }
        }

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(Keys vKey);
        #endregion
    }
}
