using System;
using System.Collections.Generic;
using System.Text;

namespace Petronode.CommonData
{
    public abstract class FitterFunction
    {
        public string Name = "None";
        public string Description = "None";
        public List<FitterParameter> Parameters = new List<FitterParameter>();

        /// <summary>
        /// Creates a default function; only allowed for derived classes 
        /// </summary>
        protected FitterFunction()
        {
        }

        /// <summary>
        /// ToString for debugging
        /// </summary>
        /// <returns> string representation of function</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Name);
            sb.Append("(");
            for (int i = 0; i < Parameters.Count; i++)
            {
                sb.Append(" ");
                sb.Append(Parameters[i].ToString());
                sb.Append( (i == Parameters.Count - 1)? ")" : ",");
            }
            return sb.ToString();
        }

        /// <summary>
        /// A default way of setting parameters from a click
        /// </summary>
        /// <param name="x">converted digitizer x</param>
        /// <param name="y">converted digitizer y</param>
        /// <returns>true if set</returns>
        public virtual bool SetParametersFromClick( float x, float y)
        {
            bool isset = false;
            foreach (FitterParameter dfp in Parameters)
            {
                if (dfp.AcceptDigitizerX)
                {
                    isset = true;
                    dfp.Value = x;
                }
                if (dfp.AcceptDigitizerY)
                {
                    isset = true;
                    dfp.Value = y;
                }
            }
            return isset;
        }

        /// <summary>
        /// Sets parameters from array
        /// </summary>
        public void SetParameters(double[] prms)
        {
            for (int i = 0; i < Parameters.Count && i < prms.Length; i++)
                Parameters[i].Value = prms[i];
        }

        /// <summary>
        /// Sets parameters from array
        /// </summary>
        public void SetParameters(double[] prms,  int[] index)
        {
            for (int i = 0; i < prms.Length; i++)
                Parameters[index[i]].Value = prms[i];
        }

        /// <summary>
        /// Gets parameters into an array
        /// </summary>
        public double[] GetParameters()
        {
            double[] prms = new double[Parameters.Count];
            for (int i = 0; i < prms.Length; i++)
                prms[i] = Parameters[i].Value;
            return prms;
        }

        /// <summary>
        /// Gets parameters into an array
        /// </summary>
        public double[] GetParameters( int[] index)
        {
            double[] prms = new double[index.Length];
            for (int i = 0; i < prms.Length; i++)
                prms[i] = Parameters[index[i]].Value;
            return prms;
        }

        /// <summary>
        /// Gets lower limits into an array
        /// </summary>
        public double[] GetLowerLimits()
        {
            double[] prms = new double[Parameters.Count];
            for (int i = 0; i < prms.Length; i++)
                prms[i] = Parameters[i].MinLimit;
            return prms;
        }

        /// <summary>
        /// Gets lower limits into an array
        /// </summary>
        public double[] GetLowerLimits(int[] index)
        {
            double[] prms = new double[index.Length];
            for (int i = 0; i < prms.Length; i++)
                prms[i] = Parameters[index[i]].MinLimit;
            return prms;
        }

        /// <summary>
        /// Gets upper limits into an array
        /// </summary>
        public double[] GetUpperLimits()
        {
            double[] prms = new double[Parameters.Count];
            for (int i = 0; i < prms.Length; i++)
                prms[i] = Parameters[i].MaxLimit;
            return prms;
        }

        /// <summary>
        /// Gets upper limits into an array
        /// </summary>
        public double[] GetUpperLimits(int[] index)
        {
            double[] prms = new double[index.Length];
            for (int i = 0; i < prms.Length; i++)
                prms[i] = Parameters[index[i]].MaxLimit;
            return prms;
        }

        public abstract FitterFunction Clone();
        public abstract double Compute( double x);

        protected void CloneFunction(FitterFunction template)
        {
            this.Name = template.Name;
            this.Description = template.Description;
            this.Parameters.Clear();
            foreach (FitterParameter p in template.Parameters)
                this.Parameters.Add(new FitterParameter(p)); 
        }
    }
}
