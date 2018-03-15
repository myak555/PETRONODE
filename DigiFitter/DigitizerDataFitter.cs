using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Petronode.CommonData;
using Petronode.CommonControls;

namespace Petronode.DigiFitter
{
    public partial class DigitizerDataFitter : UserControl
    {
        private Pen m_Pen1 = new Pen(Color.White, 3f);
        private Pen m_Pen2 = new Pen(Color.Black, 1f);
        private Pen m_Pen3 = new Pen(Color.Red, 2f);
        private Pen m_Pen4 = new Pen(Color.Blue, 2f);

        public string FitFormat = "0.000e+000";
        public PictureDigitizerControl PictureDigitizer = null;
        public DigitizerFile Data = null;
        public bool SupressPlot = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public DigitizerDataFitter()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        /// <summary>
        /// Makes initial table
        /// </summary>
        public void Refresh_Data()
        {
            if (Data == null) return;
            SupressPlot = true;
            dataGridView1.SuspendLayout();
            dataGridView1.Rows.Clear();
            Refresh_Headers();
            foreach (DigitizerPoint p in Data.Points)
            {
                dataGridView1.Rows.Add(p.ToHorizontalFitStrings());
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Tag = p;
            }
            dataGridView1.ResumeLayout();
            SupressPlot = false;
            Plot_Picks();
        }

        /// <summary>
        /// Redraws data picks - to be called externally upon image change
        /// </summary>
        public void Plot_Picks()
        {
            if (PictureDigitizer == null) return;
            if (Data == null) return;
            if (SupressPlot) return;
            SupressPlot = true;
            textBox1.Text = Data.RecomputeAll().ToString(FitFormat);
            foreach (DataGridViewRow dgvr in dataGridView1.Rows)
            {
                DigitizerPoint dp = (DigitizerPoint)dgvr.Tag;
                string[] tmp = dp.ToHorizontalFitStrings();
                dgvr.Cells[2].Value = tmp[2];
                dgvr.Cells[3].Value = tmp[3];
            }
            
            Graphics g = Graphics.FromImage(PictureDigitizer.OverlayImage);
            g.Clear(Color.Transparent);
            PlotFits(g);
            PlotValues(g);
            PlotSelection(g);
            g.Dispose();
            PictureDigitizer.Refresh();
            if (OnPlotNext != null) OnPlotNext(); 
            SupressPlot = false;
        }

        /// <summary>
        /// Updates the headers
        /// </summary>
        public void Refresh_Headers()
        {
            dataGridView1.Columns[0].HeaderText = Data.Calibration.XAxisName;
            dataGridView1.Columns[1].HeaderText = Data.Calibration.YAxisName;
        }

        /// <summary>
        /// Delegate to plot the next graph
        /// </summary>
        public delegate void PlotNextDelegate();
        public PlotNextDelegate OnPlotNext = null;

        #region Private Methods
        private void PlotDots(Graphics g, List<Point> points)
        {
            if (comboBox1.Text != "Dot") return;
            foreach (Point p in points)
            {
                int xLoc = p.X;
                int yLoc = p.Y;
                Rectangle r1 = new Rectangle(xLoc - 4, yLoc - 4, 9, 9);
                g.DrawEllipse(m_Pen1, r1);
                g.DrawEllipse(m_Pen2, r1);
            }
        }

        private void PlotCrosses(Graphics g, List<Point> points, bool useSwitch)
        {
            if (useSwitch && comboBox1.Text != "Cross") return;
            foreach (Point p in points)
            {
                int xLoc = p.X;
                int yLoc = p.Y;
                g.DrawLine(m_Pen1, p.X - 5, p.Y, p.X + 5, p.Y);
                g.DrawLine(m_Pen1, p.X, p.Y - 5, p.X, p.Y + 5);
                g.DrawLine(m_Pen2, p.X - 4, p.Y, p.X + 4, p.Y);
                g.DrawLine(m_Pen2, p.X, p.Y - 4, p.X, p.Y + 4);
            }
        }

        private void PlotBoxes(Graphics g, List<Point> points)
        {
            int maxX = PictureDigitizer.OverlayImage.Width;
            int maxY = PictureDigitizer.OverlayImage.Height;
            foreach (Point p in points)
            {
                int xLoc = p.X;
                int yLoc = p.Y;
                g.DrawRectangle(m_Pen1, xLoc-6, yLoc-6, 12, 12);
                g.DrawRectangle(m_Pen2, xLoc-6, yLoc-6, 12, 12);
                g.DrawLine(m_Pen2, 0, yLoc, xLoc-8, yLoc);
                g.DrawLine(m_Pen2, xLoc+8, yLoc, maxX, yLoc);
                g.DrawLine(m_Pen2, xLoc, 0, xLoc, yLoc-8);
                g.DrawLine(m_Pen2, xLoc, yLoc+8, xLoc, maxY);
            }
        }

        private void PlotLine(Graphics g, List<Point> points)
        {
            if (comboBox1.Text != "Line") return;
            if (points.Count < 1)
            {
                PlotCrosses(g, points, false);
                return;
            }
            Point[] A = points.ToArray();
            g.DrawLines(m_Pen1, A);
            g.DrawLines(m_Pen2, A);
        }

        private void PlotFits(Graphics g)
        {
            List<Point> myPoints = Data.GetPrefitPoints();
            if (myPoints.Count > 1) g.DrawLines(m_Pen4, myPoints.ToArray());
            myPoints = Data.GetFitPoints();
            if (myPoints.Count > 1) g.DrawLines(m_Pen3, myPoints.ToArray());
        }

        private void PlotValues(Graphics g)
        {
            List<Point> myPoints = Data.GetDigitizerPoints();
            PlotCrosses(g, myPoints, true);
            PlotLine(g, myPoints);
            PlotDots(g, myPoints);
        }

        private void PlotSelection(Graphics g)
        {
            List<Point> myPoints = new List<Point>();
            foreach (DataGridViewRow dgvr in dataGridView1.SelectedRows)
            {
                DigitizerPoint p = (DigitizerPoint)dgvr.Tag;
                if (!p.isPlottable) continue;
                myPoints.Add(p.Location);
            }
            PlotBoxes(g, myPoints);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SupressPlot) return;
            Plot_Picks();
        }
        #endregion
    }
}
