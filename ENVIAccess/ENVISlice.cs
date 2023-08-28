using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petronode.ENVIAccess
{
    public class ENVISlice
    {
        public bool isValid = false;
        public int Lines = 0;
        public int Samples = 0;
        public double Wavelength = Double.NaN;
        public double Minimum = Double.NaN;
        public double Maximum = Double.NaN;
        public int NumberMissing = 0;
        public double[] Data = new double[0];
        public ENVIDataCube ParentCube = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public ENVISlice()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ENVISlice(int lines, int samples, double wavelength, double[] data)
        {
            Lines = lines;
            Samples = samples;
            Wavelength = wavelength;
            Data = data;
            SetValidity();
            if (isValid) ComputeStats();
        }

        /// <summary>
        /// Returns the sample index in the Data array
        /// </summary>
        /// <param name="line"></param>
        /// <param name="sample"></param>
        /// <returns></returns>
        public int GetSampleIndex(int line, int sample)
        {
            return line * Samples + sample;
        }

        /// <summary>
        /// Returns the sample index in the Data array
        /// </summary>
        /// <param name="line"></param>
        /// <param name="sample"></param>
        /// <returns></returns>
        public double GetSample(int line, int sample)
        {
            return Data[GetSampleIndex(line,sample)];
        }

        protected void SetValidity()
        {
            if (Wavelength <= 0.0) return;
            if (Data.Length <= 0) return;
            if (Data.Length != Lines * Samples) return;
            isValid = true;
        }
        protected void ComputeStats()
        {
            for (int index = 0; index < Data.Length; ++index)
            {
                double v = Data[index];
                if(Double.IsNaN(v))
                {
                    NumberMissing++;
                    continue;
                }
                if (Double.IsNaN(Minimum) || v < Minimum) Minimum = v;
                if (Double.IsNaN(Maximum) || v > Maximum) Maximum = v;
            }
        }
    }
}
