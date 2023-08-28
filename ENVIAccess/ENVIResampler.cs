using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petronode.ENVIAccess
{
    public class InterpolationQuad
    {
        public int Index0 = -1;
        public int Index1 = -1;
        public double Coeff0 = 1.0;
        public double Coeff1 = 1.0;

        public InterpolationQuad(double x, double[] basis)
        {
            if (x < basis[0])
            {
                Index0 = 0;
                return;
            }
            if (x > basis[basis.Length-1])
            {
                Index0 = basis.Length - 1;
                return;
            }

            int closestIndex = 0;
            double closestDiff = Math.Abs(basis[0] - x);
            for( int i=1; i<basis.Length; i++)
            {
                double v = Math.Abs(basis[i] - x);
                if (v >= closestDiff) continue;
                closestDiff = v;
                closestIndex = i; 
            }

            double v0 = basis[closestIndex];
            if( Math.Abs(v0 - x) < 1e-30)
            {
                Index0 = closestIndex;
                return;
            }
            if( x < v0)
            {
                Index0 = closestIndex - 1;
                Index1 = closestIndex;
                if (Index0 < 0) return;
            }
            else
            {
                Index0 = closestIndex;
                Index1 = closestIndex + 1;
                if (Index1 >= basis.Length)
                {
                    Index1 = -1;
                    return;
                }
            }
            v0 = basis[Index0];
            double v1 = basis[Index1];
            double diff = Math.Abs(v0 - v1);
            if ( diff < 1e-30)
            {
                // simple average for an overlap point
                Coeff0 = 0.5;
                Coeff1 = 0.5;
                return;
            }
            Coeff0 = (v1 - x) / diff;
            Coeff1 = (x - v0) / diff;
        }

        public double Interpolate( double[] vec)
        {
            if (Index0 < 0 && Index1 < 0) return Double.NaN;
            if (Index0 >= 0 && Index1 < 0) return vec[Index0];
            if (Index0 < 0 && Index1 >= 0) return vec[Index1];
            return vec[Index0] * Coeff0 + vec[Index1] * Coeff1;
        }

        public double Interpolate( float[] vec, int offset=0)
        {
            if (Index0 < 0 && Index1 < 0) return Double.NaN;
            if (Index0 >= 0 && Index1 < 0) return Convert.ToDouble( vec[Index0 + offset]);
            if (Index0 < 0 && Index1 >= 0) return Convert.ToDouble( vec[Index1 + offset]);
            return Convert.ToDouble( vec[Index0 + offset]) * Coeff0 + Convert.ToDouble(vec[Index1 + offset]) * Coeff1;
        }
    }

    public class ENVIResampler
    {
        public bool isValid = false;
        public double[] Wavelengths = new double[0];
        public ENVIDataCube ParentCube = null;
        private InterpolationQuad[] m_Interpolations = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="wlFrom"></param>
        /// <param name="wlTo"></param>
        /// <param name="wlStep"></param>
        public ENVIResampler( double wlFrom, double wlTo, double wlStep, double[] OriginalWavelengths)
        {
            List<double> tmp = new List<double>();
            int wlIndex = 0;
            while(true)
            {
                double wl = wlFrom + wlIndex * wlStep;
                if (wl > wlTo) break;
                tmp.Add(wl);
                wlIndex++;
            }
            Wavelengths = tmp.ToArray();
            SetCoefficients(OriginalWavelengths);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="wavelengths"></param>
        public ENVIResampler(double[] wavelengths, double[] OriginalWavelengths)
        {
            Wavelengths = wavelengths;
            SetCoefficients(OriginalWavelengths);
        }

        public double[] GetResampled(double[] vec)
        {
            if (m_Interpolations == null) return new double[0];
            double[] result = new double[m_Interpolations.Length];
            for( int i=0; i<result.Length; ++i)
            {
                result[i] = m_Interpolations[i].Interpolate(vec);
            }
            return result;
        }

        public double[] GetResampled(float[] vec, int offset = 0)
        {
            if (!isValid || m_Interpolations == null) return new double[0];
            double[] result = new double[m_Interpolations.Length];
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = m_Interpolations[i].Interpolate(vec, offset);
            }
            return result;
        }

        public double[] GetResampled(int offset)
        {
            if( ParentCube == null) return new double[0];
            return GetResampled(ParentCube.Data, offset);
        }

        protected void SetCoefficients(double[] OriginalWavelengths)
        {
            List<InterpolationQuad> tmp = new List<InterpolationQuad>();
            foreach( double wl in Wavelengths)
            {
                tmp.Add(new InterpolationQuad(wl, OriginalWavelengths));
            }
            if (tmp.Count <= 0) return;
            m_Interpolations = tmp.ToArray();
            isValid = true;
        }
    }
}
