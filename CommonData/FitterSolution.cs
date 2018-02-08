using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Petronode.CommonData
{
    public class FitterSolution
    {
        private int m_SelectedFunctionIndex = -1;

        public List<FitterFunction> Functions = new List<FitterFunction>();
        public List<FitterFunction> FunctionTemplates = new List<FitterFunction>();
        public double Target = double.NaN;
        public double Target_Prefit = double.NaN;

        /// <summary>
        /// Creates an empty solution 
        /// </summary>
        public FitterSolution()
        {
            FunctionTemplates.Add(new LinearFunction());
            FunctionTemplates.Add(new FixPointGradientFunction());
            FunctionTemplates.Add(new SigmoidFunction());
            FunctionTemplates.Add(new BathtubFunction());
            FunctionTemplates.Add(new HubbertFunction());
            FunctionTemplates.Add(new GaussFunction());
            FunctionTemplates.Add(new KapitsaFunction());
            FunctionTemplates.Add(new WeibullFunction());
        }

        /// <summary>
        /// Creates a list of available functions
        /// </summary>
        public string[] GetTemplateNames()
        {
            List<string> tmp = new List<string>();
            foreach (FitterFunction f in FunctionTemplates)
                tmp.Add(f.Name);
            return tmp.ToArray();
        }

        /// <summary>
        /// Adds a function based on the name given
        /// </summary>
        public FitterFunction Add( string name)
        {
            foreach (FitterFunction f in FunctionTemplates)
            {
                if (f.Name != name) continue;
                FitterFunction tmp = f.Clone(); 
                Functions.Add(tmp);
                return tmp;
            }
            return null;
        }

        /// <summary>
        /// Computes an entire soluton
        /// </summary>
        public double Compute(float x)
        {
            double y = 0.0;
            try
            {
                double x1 = Convert.ToDouble(x);
                foreach (FitterFunction f in Functions) y += f.Compute(x1);
            }
            catch
            {
                y = double.NaN;
            }
            return y;
        }

        /// <summary>
        /// Computes an entire soluton
        /// </summary>
        public double Compute(double x)
        {
            double y = 0.0;
            try
            {
                foreach (FitterFunction f in Functions) y += f.Compute(x);
            }
            catch
            {
                y = double.NaN;
            }
            return y;
        }

        /// <summary>
        /// Computes an entire soluton
        /// </summary>
        public double Compute(DigitizerPoint dp)
        {
            dp.FitValue.X = dp.Value.X;
            dp.PrefitValue.X = dp.Value.X;
            double y1 = this.Compute(dp.Value.X);
            if (double.IsNaN(y1))
            {
                dp.FitValue.Y = float.NaN;
                dp.PrefitValue.Y = float.NaN;
                return y1;
            }
            double y2 = y1;
            if (m_SelectedFunctionIndex >= 0)
                y2 -= Functions[m_SelectedFunctionIndex].Compute(dp.Value.X);
            try
            {
                dp.FitValue.Y = (float)y1;
                dp.PrefitValue.Y = (float)y2;
            }
            catch
            {
                dp.FitValue.Y = float.NaN;
                dp.PrefitValue.Y = float.NaN;
            }
            return y1;
        }

        /// <summary>
        /// Computes an entire soluton
        /// </summary>
        public double Compute(List<DigitizerPoint> points)
        {
            Target = 0.0;
            Target_Prefit = 0.0;
            int Counter = 0;
            foreach (DigitizerPoint dp in points)
            {
                this.Compute(dp);
                if (!dp.isFitted) continue;
                Target += dp.deltaY2;
                Target_Prefit += dp.deltaYp2;
                Counter++;
            }
            if (Counter <= 0)
            {
                Target = double.NaN;
                Target_Prefit = double.NaN;
                return Target;
            }
            double N = 1.0 / Convert.ToDouble(Counter);
            Target *= N;
            Target_Prefit *= N;
            return Target;
        }

        /// <summary>
        /// Attempts to parse a file line as a Solution function
        /// </summary>
        /// <param name="s">line to parse</param>
        /// <returns>true if passed the comment block</returns>
        public bool LoadSolutionLine(string s)
        {
            foreach (FitterFunction f in FunctionTemplates)
            {
                string sCheck = "# " + f.Name + "(";
                if( !s.StartsWith( sCheck )) continue;
                s = s.Substring(sCheck.Length).Replace(" ", "").Replace(")", "");
                FitterFunction newFunction = f.Clone();
                foreach (FitterParameter p in newFunction.Parameters)
                {
                    p.FromString(s);
                }
                Functions.Add(newFunction);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Saves functions to file
        /// </summary>
        /// <param name="sw">writing destination</param>
        public void Save(StreamWriter sw)
        {
            if (Functions.Count <= 0) return;
            foreach (FitterFunction f in Functions)
                sw.WriteLine("# " + f.ToString());
            sw.WriteLine("#");
        }

        /// <summary>
        /// ToString for debugging
        /// </summary>
        /// <returns> string representation of solution</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i=0; i<Functions.Count; i++)
            {
                sb.Append(Functions[i].ToString());
                if( i < Functions.Count-1) sb.Append("\n");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Sets and retrieves the selected function index, -1 if not set
        /// </summary>
        public int SelectedFunctionIndex
        {
            get { return m_SelectedFunctionIndex; }
            set
            {
                m_SelectedFunctionIndex = value;
                if (m_SelectedFunctionIndex < 0 && Functions.Count > 0)
                    m_SelectedFunctionIndex = 0;
            }
        }

        /// <summary>
        /// Retrieves the selected function or null if not set
        /// </summary>
        public FitterFunction SelectedFunction
        {
            get
            {
                if( m_SelectedFunctionIndex < 0) return null;
                if (m_SelectedFunctionIndex >= Functions.Count) return null;
                return Functions[m_SelectedFunctionIndex];
            }
        }
    }
}
