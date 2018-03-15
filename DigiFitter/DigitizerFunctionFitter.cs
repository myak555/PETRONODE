using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Petronode.CommonData;

namespace Petronode.DigiFitter
{
    public partial class DigitizerFunctionFitter : UserControl
    {
        private double[] m_SourceVector = null;
        private double[] m_TargetVector = null;
        private int[] m_prmsIndex = null;

        private bool m_Update_Ongoing = false;
        public DigitizerFile Data = null;
        public DigitizerDataFitter DataFitter = null;
        public Nelder_Mead_Optimization NMO = null;
        public Nelder_Mead_Solution NMS = null;

        /// <summary>
        /// Constructor; pass the target function as a delegate
        /// </summary>
        public DigitizerFunctionFitter()
        {
            InitializeComponent();
            dataGridView2.MouseWheel += new MouseEventHandler(MouseWheelMove);
        }

        /// <summary>
        /// Updates all tables
        /// </summary>
        public void Refresh_Data()
        {
            if (Data == null) return;
            m_Update_Ongoing = true;
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(Data.Solution.GetTemplateNames());
            comboBox1.SelectedIndex = 0;
            RefreshFunctionList();
            RefreshParameterList();
            m_Update_Ongoing = false;
        }

        /// <summary>
        /// Processes a single click from the digitizer; dispatches the event into the child controls
        /// </summary>
        /// <param name="x">mouse location x</param>
        /// <param name="y">mouse location y</param>
        /// <param name="c">color unter mouse location</param>
        /// <param name="e">mouse parameters</param>
        public void ProcessDigitizerClick(int x, int y, int relative_x, int relative_y,
            string c, MouseEventArgs e, bool[] KeyModifiers)
        {
            if (Data == null) return;
            FitterFunction ff = Data.Solution.SelectedFunction;
            if (ff == null) return;
            Point p = new Point( x, y);
            PointF pf = Data.Calibration.LocationToValue(p);
            ff.SetParametersFromClick(pf.X, pf.Y);
            RefreshParameterList();
            if (DataFitter != null) DataFitter.Plot_Picks();
        }

        /// <summary>
        /// Processes a single click from the mistie control; dispatches the event into the child controls
        /// </summary>
        /// <param name="pf">mouse location x, y</param>
        /// <param name="e">mouse parameters</param>
        public void ProcessMistieClick(PointF pf, MouseEventArgs e, bool[] KeyModifiers)
        {
            if (Data == null) return;
            FitterFunction ff = Data.Solution.SelectedFunction;
            ff.SetParametersFromClick(pf.X, pf.Y);
            RefreshParameterList();
            if (DataFitter != null) DataFitter.Plot_Picks();
        }

        /// <summary>
        /// Called on mouse wheel event in the parent form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MouseWheelMove(object sender, MouseEventArgs e)
        {
            if (Data == null) return;
            if (Data.Solution.SelectedFunction == null) return;
            if (e.Button != MouseButtons.None) return;
            if (dataGridView2.SelectedCells.Count <= 0) return;
            double shift = (100.0 + Convert.ToDouble( e.Delta / ((GetAsyncKeyState(Keys.ShiftKey) < 0) ? 120 : 12)));
            shift *= 0.01;
            try
            {
                foreach (DataGridViewCell dgvr in dataGridView2.SelectedCells)
                {
                    FitterParameter fp = Data.Solution.SelectedFunction.Parameters[dgvr.RowIndex]; 
                    switch (dgvr.ColumnIndex)
                    {
                        case 1:
                            fp.Value *= shift;
                            break;
                        case 2:
                            fp.MinLimit *= shift;
                            break;
                        case 3:
                            fp.MaxLimit *= shift;
                            break;
                        default:
                            double v0 = fp.Value * shift;
                            double v1 = fp.MinLimit * shift;
                            double v2 = fp.MaxLimit * shift;
                            fp.Value = v0;
                            fp.MinLimit = v1;
                            fp.MaxLimit = v2;
                            break;
                    }
                    UpdateParameterRow(dgvr.RowIndex);
                }
            }
            catch{}
            if (DataFitter != null) DataFitter.Plot_Picks();
            ((HandledMouseEventArgs)e).Handled = true;
        }

        /// <summary>
        /// Prepares minimization by user-selected parameters
        /// </summary>
        /// <returns>true if preparation successful</returns>
        public bool PrepareMinimizationPartial()
        {
            if (Data == null) return false;
            if (m_Update_Ongoing) return false;
            if (Data.Solution.SelectedFunction == null) return false;
            if (dataGridView2.SelectedRows.Count <= 0) return false;

            PrepareMinimizationData();
            List<int> Ps = new List<int>();
            foreach (DataGridViewRow dgvr in dataGridView2.SelectedRows)
                Ps.Add(dgvr.Index);
            m_prmsIndex = Ps.ToArray();

            NMO = new Nelder_Mead_Optimization(3,
                Data.Solution.SelectedFunction.GetParameters(m_prmsIndex),
                Data.Solution.SelectedFunction.GetLowerLimits(m_prmsIndex),
                Data.Solution.SelectedFunction.GetUpperLimits(m_prmsIndex), 1000);
            NMO.onObjectiveFunction += new Nelder_Mead_Optimization.ObjectiveFunctionDelegate(TargetFunction1);
            NMO.onReportProgress += new Nelder_Mead_Optimization.ProgressReportDelegate(ReportProgress);
            return true;
        }

        /// <summary>
        /// Prepares minimization by all function parameters
        /// </summary>
        /// <returns>true if preparation successful</returns>
        public bool PrepareMinimizationFull()
        {
            if (Data == null) return false;
            if (m_Update_Ongoing) return false;
            if (Data.Solution.SelectedFunction == null) return false;

            PrepareMinimizationData();
            NMO = new Nelder_Mead_Optimization(3,
                Data.Solution.SelectedFunction.GetParameters(),
                Data.Solution.SelectedFunction.GetLowerLimits(),
                Data.Solution.SelectedFunction.GetUpperLimits(), 1000);
            NMO.onObjectiveFunction += new Nelder_Mead_Optimization.ObjectiveFunctionDelegate(TargetFunction2);
            NMO.onReportProgress += new Nelder_Mead_Optimization.ProgressReportDelegate(ReportProgress);
            return true;
        }

        /// <summary>
        /// Runs minimization
        /// </summary>
        public void RunMinimization()
        {
            if (NMO == null) return;
            m_Update_Ongoing = true;
            NMO.InitSolutions();
            NMS = NMO.LocateTarget();
            m_Update_Ongoing = false;
        }

        /// <summary>
        /// Presents minimization results
        /// </summary>
        public void PresentMinimizationResults()
        {
            if (Data == null) return;
            if (Data.Solution.SelectedFunction == null) return;
            if (NMS == null) return;
            if (m_prmsIndex != null)
            {
                Data.Solution.SelectedFunction.SetParameters(NMS.Vector, m_prmsIndex);
                m_prmsIndex = null;
            }
            else Data.Solution.SelectedFunction.SetParameters(NMS.Vector);
            foreach (DataGridViewRow dgvr in dataGridView2.Rows)
                UpdateParameterRow(dgvr.Index);
            if (DataFitter != null) DataFitter.Plot_Picks();
            return;
        }

        /// <summary>
        /// To remember the function choice
        /// </summary>
        public int FunctionSelected
        {
            get { return comboBox1.SelectedIndex; }
            set
            {
                if (value < 0) return;
                if (value >= comboBox1.Items.Count) return;
                comboBox1.SelectedIndex = value;
            }
        }

        public delegate void ProgressReportDelegate(int percent, object info);
        public ProgressReportDelegate onReportProgress = null;
        public delegate void ButtonActionDelegate(int action);
        public ButtonActionDelegate onMainFormAction = null;

        #region Private Methods
        private void button1_Click(object sender, EventArgs e)
        {
            if (Data == null) return;
            FitterFunction ff = Data.Solution.Add((string)comboBox1.SelectedItem);
            if (ff == null) return;
            string[] tmp = new string[1];
            tmp[0] = ff.ToString();
            dataGridView1.Rows.Add(tmp);
            dataGridView1.ClearSelection();
            dataGridView1.Rows[dataGridView1.Rows.Count - 1].Selected = true;
            if (DataFitter != null) DataFitter.Plot_Picks();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count <= 0) return;
            Data.Solution.SelectedFunctionIndex = dataGridView1.SelectedRows[0].Index; 
            RefreshParameterList();
            if (DataFitter != null) DataFitter.Plot_Picks();
        }

        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (Data == null) return;
            if( m_Update_Ongoing) return;
            Data.Solution.Functions.RemoveAt(e.RowIndex);
            Data.Solution.SelectedFunctionIndex = e.RowIndex - 1;
            if (Data.Solution.SelectedFunctionIndex >= 0)
                dataGridView1.Rows[Data.Solution.SelectedFunctionIndex].Selected = true;
            RefreshParameterList();
            if (DataFitter != null) DataFitter.Plot_Picks();
        }

        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            FitterFunction ff = Data.Solution.SelectedFunction;
            if (ff == null) return;
            double value = 0.0;
            try
            {
                value = Convert.ToDouble(dataGridView2[e.ColumnIndex, e.RowIndex].Value);
            }
            catch            
            {
                return;
            }
            FitterParameter fp = ff.Parameters[e.RowIndex];
            switch (e.ColumnIndex)
            {
                case 1:
                    fp.Value = value;
                    break;
                case 2:
                    fp.MinLimit = value;
                    break;
                case 3:
                    fp.MaxLimit = value;
                    break;
                default:
                    break;
            }
            UpdateParameterRow(e.RowIndex);
            if (DataFitter != null) DataFitter.Plot_Picks();
        }

        private void RefreshFunctionList()
        {
            if (Data == null) return;
            dataGridView1.SuspendLayout();
            dataGridView1.Rows.Clear();
            string[] tmp = new string[1];
            foreach (FitterFunction ff in Data.Solution.Functions)
            {
                tmp[0] = ff.ToString();
                dataGridView1.Rows.Add(tmp);
            }
            if (Data.Solution.SelectedFunctionIndex >= 0)
                dataGridView1.Rows[Data.Solution.SelectedFunctionIndex].Selected = true;
            dataGridView1.ResumeLayout();
        }

        private void RefreshParameterList()
        {
            dataGridView2.SuspendLayout();
            dataGridView2.Rows.Clear();
            FitterFunction ff = Data.Solution.SelectedFunction;
            if (ff == null)
            {
                dataGridView2.ResumeLayout();
                return;
            }
            foreach (FitterParameter fp in ff.Parameters)
            {
                string[] tmp = fp.ToStrings();
                dataGridView2.Rows.Add(tmp);
            }
            dataGridView2.ResumeLayout();
        }

        private void UpdateParameterRow(int r)
        {
            string[] tmp = Data.Solution.SelectedFunction.Parameters[r].ToStrings();
            for (int i = 1; i < tmp.Length; i++)
                dataGridView2[i, r].Value = tmp[i];
            if (dataGridView1.SelectedRows.Count <= 0) return;
            dataGridView1[0, Data.Solution.SelectedFunctionIndex].Value =
                Data.Solution.SelectedFunction.ToString();
        }

        private double TargetFunction()
        {
            double Target = 0.0;
            for (int i = 0; i < m_SourceVector.Length; i++)
            {
                double c = Data.Solution.SelectedFunction.Compute(m_SourceVector[i]);
                c -= m_TargetVector[i];
                Target += c * c;
            }
            Target /= m_SourceVector.Length;
            return Target;
        }

        private double TargetFunction1(double[] prms)
        {
            Data.Solution.SelectedFunction.SetParameters(prms, m_prmsIndex);
            return TargetFunction();
        }

        private double TargetFunction2(double[] prms)
        {
            Data.Solution.SelectedFunction.SetParameters(prms);
            return TargetFunction();
        }

        private void PrepareMinimizationData()
        {
            Data.RecomputeAll();
            List<double> src = new List<double>();
            List<double> trg = new List<double>();
            foreach (DigitizerPoint dp in Data.Points)
            {
                src.Add(dp.valueX);
                trg.Add(dp.deltaYp);
            }
            m_SourceVector = src.ToArray();
            m_TargetVector = trg.ToArray();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Data == null) return;
            if (m_Update_Ongoing) return;
            if (Data.Solution.SelectedFunction == null) return;
            if (onMainFormAction == null) return;
            onMainFormAction(2); 

            if (!PrepareMinimizationPartial()) return;
            RunMinimization();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Data == null) return;
            if (m_Update_Ongoing) return;
            if (Data.Solution.SelectedFunction == null) return;
            if (onMainFormAction == null) return;
            onMainFormAction(3); 

            if (!PrepareMinimizationFull()) return;
            RunMinimization();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (Data == null) return;
            if (Data.Solution.SelectedFunction == null) return;
            foreach (DataGridViewRow dgvr in dataGridView2.Rows)
            {
                FitterParameter fp = Data.Solution.SelectedFunction.Parameters[dgvr.Index];
                double p = (fp.Value >= 0) ? fp.Value : -fp.Value;
                p *= 0.1;
                fp.MinLimit = fp.Value - p;
                fp.MaxLimit = fp.Value + p;
                UpdateParameterRow(dgvr.Index);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (Data == null) return;
            if (onMainFormAction == null) return;
            onMainFormAction(5); 
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Data == null) return;
            if (Data.Solution.SelectedFunction == null) return;
            FitterParameter fp = Data.Solution.SelectedFunction.Parameters[e.RowIndex];
            switch (e.ColumnIndex)
            {
                case 1:
                    fp.MinLimit = fp.Value;
                    fp.MaxLimit = fp.Value;
                    break;
                case 2:
                    fp.MinLimit = fp.Value;
                    break;
                case 3:
                    fp.MaxLimit = fp.Value;
                    break;
                default:
                    double p = (fp.Value >= 0) ? fp.Value : -fp.Value;
                    p *= 0.1;
                    fp.MinLimit = fp.Value - p;
                    fp.MaxLimit = fp.Value + p;
                    break;
            }
            UpdateParameterRow(e.RowIndex);
        }

        private void ReportProgress(int percent, object info)
        {
            if (onReportProgress == null) return;
            onReportProgress(percent, info);
        }

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(Keys vKey);
        #endregion
    }
}
