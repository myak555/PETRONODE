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

namespace Petronode.DigiFitter
{
    public partial class MistieControl : UserControl
    {
        public Pen Pen1 = new Pen(Color.White, 3f);
        public Pen Pen2 = new Pen(Color.Black, 1f);
        public Pen Pen3 = new Pen(Color.Red, 2f);
        public Pen Pen4 = new Pen(Color.Blue, 2f);
        public Pen Pen5 = new Pen(Color.Green, 2f);
        public Font AxisFont = new Font( FontFamily.GenericMonospace, 8f);

        private List<Point> m_Misfits = new List<Point>();
        private List<Point> m_Function = new List<Point>();
        private List<Point> m_Residual = new List<Point>();
        private int m_dx = 0;
        private int m_dy = 0;
        private int m_zero_level = 10;

        private Bitmap m_ScreenImage = new Bitmap(2, 2);
        private DigitizerCalibration m_Calibration = new DigitizerCalibration();

        public DigitizerFile Data = null;
        public bool SupressPlot = false;
        public List<PointF> Misfits = new List<PointF>();
        public List<PointF> Function = new List<PointF>();
        public List<PointF> Residual = new List<PointF>();

        /// <summary>
        /// Constructor
        /// </summary>
        public MistieControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Redraws data picks - to be called externally upon image change
        /// </summary>
        public void Plot_Picks()
        {
            if (Data == null) return;
            if (SupressPlot) return;
            SupressPlot = true;
            m_dx = pictureBox1.Width;
            m_dy = pictureBox1.Height;
            Bitmap predisposed = m_ScreenImage;
            m_ScreenImage = new Bitmap(m_dx, m_dy);
            Graphics g = Graphics.FromImage(m_ScreenImage);
            g.Clear(Color.White);
            m_Calibration.LeftBottomLocation = new Point(60, m_dy - 20);
            m_Calibration.RightTopLocation = new Point(m_dx - 2, 2);
            GetData();
            PlotAxis( g);
            PlotCrosses(g, m_Misfits);
            PlotBars(g, m_Residual);
            if (m_Function.Count > 1) g.DrawLines(Pen3, m_Function.ToArray());
            if (OnPlotNext != null) OnPlotNext();
            g.Dispose();
            pictureBox1.Image = m_ScreenImage;
            if (predisposed != null) predisposed.Dispose();
            SupressPlot = false;
        }

        /// <summary>
        /// Delegate to the next graph
        /// </summary>
        public delegate void PlotNextDelegate();
        public PlotNextDelegate OnPlotNext = null;

        /// <summary>
        /// Delegate to service the mouse clicks in the image
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        public delegate void OnDizitizerEventDelegate(PointF pf, MouseEventArgs e, bool[] KeyModifiers);
        public OnDizitizerEventDelegate OnDigitizerEventReceived = null;

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Plot_Picks();
        }

        private void GetData()
        {
            Misfits.Clear();
            Function.Clear();
            Residual.Clear();
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;
            foreach( DigitizerPoint dp in Data.Points)
            {
                if( !dp.isFitted) continue;
                PointF m = new PointF(dp.Value.X, dp.Value.Y-dp.PrefitValue.Y);
                PointF f = new PointF(dp.Value.X, dp.FitValue.Y-dp.PrefitValue.Y);
                PointF r = new PointF(dp.Value.X, dp.Value.Y-dp.FitValue.Y);
                Misfits.Add( m);
                Function.Add( f);
                Residual.Add( r);
                if ( minX > dp.Value.X) minX = dp.Value.X;
                if ( maxX < dp.Value.X) maxX = dp.Value.X;
                if ( minY > m.Y) minY = m.Y;
                if ( maxY < m.Y) maxY = m.Y;
                if ( minY > f.Y) minY = f.Y;
                if ( maxY < f.Y) maxY = f.Y;
                if ( minY > r.Y) minY = r.Y;
                if ( maxY < r.Y) maxY = r.Y;
            }
            if (Data.Points.Count <= 0)
            {
                m_Calibration.LeftBottomValue.X = 0f;
                m_Calibration.LeftBottomValue.Y = 0f;
                m_Calibration.RightTopValue.X = 1f;
                m_Calibration.RightTopValue.Y = 1f;
                return;
            }
            if (maxY > 1.0e10f) maxY = 1.0e10f;
            if (minY < -1.0e10f) minY = -1.0e10f;
            if (Data.Points.Count <= 1)
            {
                m_Calibration.LeftBottomValue.X = minX - 1f;
                m_Calibration.LeftBottomValue.Y = minY - 1f;
                m_Calibration.RightTopValue.X = maxX + 1f;
                m_Calibration.RightTopValue.Y = maxY + 1f;
                return;
            }
            m_Calibration.LeftBottomValue.X = minX;
            m_Calibration.LeftBottomValue.Y = minY;
            m_Calibration.RightTopValue.X = maxX;
            m_Calibration.RightTopValue.Y = maxY;

            m_Misfits.Clear();
            m_Function.Clear();
            m_Residual.Clear();
            foreach (PointF p in Misfits) m_Misfits.Add(m_Calibration.ValueToLocation(p));
            foreach (PointF p in Function) m_Function.Add(m_Calibration.ValueToLocation(p));
            foreach (PointF p in Residual) m_Residual.Add(m_Calibration.ValueToLocation(p));
        }

        private void PlotAxis( Graphics g)
        {
            PointF p1 = new PointF(m_Calibration.LeftBottomValue.X, 0f);
            Point pl1 = m_Calibration.ValueToLocation(p1);
            m_zero_level = pl1.Y;
            g.DrawRectangle(Pen2,
                m_Calibration.LeftBottomLocation.X, m_Calibration.RightTopLocation.Y,
                m_Calibration.RightTopLocation.X - m_Calibration.LeftBottomLocation.X,
                m_Calibration.LeftBottomLocation.Y - m_Calibration.RightTopLocation.Y);
            g.DrawLine(Pen2,
                m_Calibration.LeftBottomLocation.X - 3, m_zero_level,
                m_Calibration.RightTopLocation.X, m_zero_level);
            g.DrawLine(Pen2,
                m_Calibration.LeftBottomLocation.X - 3, m_Calibration.RightTopLocation.Y,
                m_Calibration.LeftBottomLocation.X, m_Calibration.RightTopLocation.Y);
            g.DrawLine(Pen2,
                m_Calibration.LeftBottomLocation.X - 3, m_Calibration.LeftBottomLocation.Y,
                m_Calibration.LeftBottomLocation.X, m_Calibration.LeftBottomLocation.Y);
            g.DrawLine(Pen2,
                m_Calibration.LeftBottomLocation.X, m_Calibration.LeftBottomLocation.Y,
                m_Calibration.LeftBottomLocation.X, m_Calibration.LeftBottomLocation.Y + 3);
            g.DrawLine(Pen2,
                m_Calibration.RightTopLocation.X, m_Calibration.LeftBottomLocation.Y,
                m_Calibration.RightTopLocation.X, m_Calibration.LeftBottomLocation.Y + 3);
            g.DrawString(m_Calibration.RightTopValue.Y.ToString("0.000"),
                AxisFont, Brushes.Black,
                2f, Convert.ToSingle(m_Calibration.RightTopLocation.Y));
            g.DrawString(m_Calibration.LeftBottomValue.Y.ToString("0.000"),
                AxisFont, Brushes.Black,
                2f, Convert.ToSingle(m_Calibration.LeftBottomLocation.Y-8));
            g.DrawString(m_Calibration.LeftBottomValue.X.ToString("0.000"),
                AxisFont, Brushes.Black,
                Convert.ToSingle(m_Calibration.LeftBottomLocation.X-3),
                Convert.ToSingle(m_Calibration.LeftBottomLocation.Y+3));
            g.DrawString(m_Calibration.RightTopValue.X.ToString("0.000"),
                AxisFont, Brushes.Black,
                Convert.ToSingle(m_Calibration.RightTopLocation.X - 63),
                Convert.ToSingle(m_Calibration.LeftBottomLocation.Y + 3));
        }

        private void PlotDots(Graphics g, List<Point> points)
        {
            foreach (Point p in points)
            {
                int xLoc = p.X;
                int yLoc = p.Y;
                Rectangle r1 = new Rectangle(xLoc - 4, yLoc - 4, 9, 9);
                g.DrawEllipse(Pen1, r1);
                g.DrawEllipse(Pen2, r1);
            }
        }

        private void PlotBars(Graphics g, List<Point> points)
        {
            foreach (Point p in points)
            {
                int xLoc = p.X;
                int yLoc = p.Y;
                if( yLoc < m_Calibration.RightTopLocation.Y)
                    yLoc = m_Calibration.RightTopLocation.Y+1;
                if( yLoc > m_Calibration.LeftBottomLocation.Y)
                    yLoc = m_Calibration.LeftBottomLocation.Y-1;
                if( yLoc < m_zero_level)
                {
                    Rectangle r1 = new Rectangle( xLoc - 1, yLoc, 3, m_zero_level-yLoc);
                    g.FillRectangle(Brushes.LightGreen, r1);
                }
                if( yLoc > m_zero_level)
                {
                    Rectangle r1 = new Rectangle(xLoc - 1, m_zero_level, 3, yLoc - m_zero_level);
                    g.FillRectangle(Brushes.LightPink, r1);
                }
            }
        }

        private void PlotCrosses(Graphics g, List<Point> points)
        {
            foreach (Point p in points)
            {
                int xLoc = p.X;
                int yLoc = p.Y;
                g.DrawLine(Pen1, p.X - 5, p.Y, p.X + 5, p.Y);
                g.DrawLine(Pen1, p.X, p.Y - 5, p.X, p.Y + 5);
                g.DrawLine(Pen2, p.X - 4, p.Y, p.X + 4, p.Y);
                g.DrawLine(Pen2, p.X, p.Y - 4, p.X, p.Y + 4);
            }
        }

        private void PlotBoxes(Graphics g, List<Point> points)
        {
            foreach (Point p in points)
            {
                int xLoc = p.X;
                int yLoc = p.Y;
                g.DrawRectangle(Pen1, xLoc - 6, yLoc - 6, 12, 12);
                g.DrawRectangle(Pen2, xLoc - 6, yLoc - 6, 12, 12);
                g.DrawLine(Pen2, 0, yLoc, xLoc - 8, yLoc);
                g.DrawLine(Pen2, xLoc + 8, yLoc, m_dx, yLoc);
                g.DrawLine(Pen2, xLoc, 0, xLoc, yLoc - 8);
                g.DrawLine(Pen2, xLoc, yLoc + 8, xLoc, m_dy);
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (Data == null) return;
            if (SupressPlot) return;
            PointF pf = m_Calibration.LocationToValue(new Point(e.X, e.Y));
            bool[] modifier_Shift = new bool[3];
            modifier_Shift[0] = GetAsyncKeyState(Keys.ShiftKey) < 0;
            modifier_Shift[1] = GetAsyncKeyState(Keys.ControlKey) < 0;
            modifier_Shift[2] = GetAsyncKeyState(Keys.Alt) < 0;
            if (OnDigitizerEventReceived != null)
               OnDigitizerEventReceived(pf, e, modifier_Shift);
            //((HandledMouseEventArgs)e).Handled = true;
        }

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(Keys vKey);
    }
}
