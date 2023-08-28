using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petronode.ENVIAccess
{
    public class ENVITrace
    {
        public bool isValid = false;
        public double[] Wavelengths = new double[0];
        public double[] Data = new double[0];
        public ENVIDataCube ParentCube = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public ENVITrace()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ENVITrace(double[] wavelengths, double[] data)
        {
            Wavelengths = wavelengths;
            Data = data;
            SetValidity();
        }

        protected void SetValidity()
        {
            if (Wavelengths.Length <= 0) return;
            if (Data.Length <= 0) return;
            if (Wavelengths.Length != Data.Length) return;
            for( int i=0; i<Data.Length; ++i)
            {
                if (Double.IsNaN(Data[i])) return;
            }
            isValid = true;
        }
    }
}
