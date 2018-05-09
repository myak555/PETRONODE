using System;

namespace Petronode.Mathematics
{
	public class MinMax
	{
		private double m_Min = 0.0;
		private double m_Max = 0.0;
		private double m_PtP = 0.0;
		private int m_PtPbits = 0;
	
		public MinMax()
		{
		}

		public MinMax( float Mi, float Ma)
		{
			m_Min = (double)Mi;
			m_Max = (double)Ma;
			RecomputePTP();
		}
		public MinMax( double Mi, double Ma)
		{
			m_Min = Mi;
			m_Max = Ma;
			RecomputePTP();
		}

		public float MinF
		{
			get{ return (float)m_Min;}
			set
			{
				m_Min = (double)value;
				RecomputePTP();
			}
		}
		public float MaxF
		{
			get{ return (float)m_Max;}
			set
			{
				m_Max = (double)value;
				RecomputePTP();
			}
		}

		public double Min
		{
			get{ return m_Min;}
			set
			{
				m_Min = value;
				RecomputePTP();
			}
		}
		public double Max
		{
			get{ return m_Max;}
			set
			{
				m_Max = value;
				RecomputePTP();
			}
		}

		public double PtP
		{
			get{ return m_PtP;}
		}
		public float PtPF
		{
			get{ return (float)m_PtP;}
		}
		public int PtPbits
		{
			get{ return m_PtPbits;}
		}

		private void RecomputePTP()
		{
			m_PtP = Max-Min;
			if( m_PtP < 1.0)
			{
				m_PtPbits = 0;
				return;
			}
			m_PtPbits = Convert.ToInt32( System.Math.Log( m_PtP, 2.0));
		}
	}
	
	/// <summary>
	/// Class SeismicMath includes some useful computations.
	/// </summary>
	public class SeismicMath
	{
		/// <summary>
		/// Computes the parameters on a set of traces
		/// </summary>
		/// <param name="Samples">An array of samples</param>
		/// <returns>A pointer to the array of MinMax structures</returns>
		public static MinMax[] ComputeMinMax( float[][] Samples)
		{
			MinMax[] tmpMinMax = new MinMax[ Samples.Length];
			for( int i=0; i<Samples.Length; i++)
			{
				float[] tmp = Samples[i];
				float tmpMin = tmp[0];
				float tmpMax = tmp[0];
				for( int j=1; j<tmp.Length; j++)
				{
					if( tmp[j]<tmpMin) tmpMin = tmp[j];
					if( tmp[j]>tmpMax) tmpMax = tmp[j];
				}//for
				tmpMinMax[i].MinF = tmpMin;
				tmpMinMax[i].MaxF = tmpMax;
			}//for
			return tmpMinMax;
		}

		/// <summary>
		/// Computes the parameters on a single trace
		/// </summary>
		/// <param name="Samples">An array of samples</param>
		/// <returns>A MinMax structure</returns>
		public static MinMax ComputeMinMax( float[] Samples)
		{
			float[] tmp = Samples;
			float tmpMin = tmp[0];
			float tmpMax = tmp[0];
			for( int j=1; j<tmp.Length; j++)
			{
				if( tmp[j]<tmpMin) tmpMin = tmp[j];
				if( tmp[j]>tmpMax) tmpMax = tmp[j];
			}//for
			return new MinMax( tmpMin, tmpMax);
		}

		/// <summary>
		/// Computes a number of a=2^n, such as a is the smallest of a>=nSample
		/// </summary>
		/// <param name="nSample">Number of Samples in the trace</param>
		/// <returns></returns>
		public static int NearPowerOf2(int nSample)
		{
			int nPowerOf2 = 2;
			while(nSample > nPowerOf2)
			{
				nPowerOf2 += nPowerOf2;
			}
			return nPowerOf2;
		}

		/// <summary>
		/// Computes a number of a = 2^n * 3^k * 5^m, such as a is the smallest of a>=nSample
		/// This sample number may be used in conjunction with the FFT routines that wiork best on the
		/// trace lengths exual to multiples of small primes
		/// </summary>
		/// <param name="nSample">Number of Samples in the trace</param>
		/// <returns></returns>
		public static int NearPowerOfSmallPrimes(int nSample)
		{
			int i = 1;
			while( nSample%5 == 0)
			{
				i *= 5;
				nSample /= 5;
			}
			while( nSample%3 == 0)
			{
				i *= 3;
				nSample /= 3;
			}
			while( nSample%2 == 0)
			{
				i *= 2;
				nSample /= 2;
			}
			if( nSample<=1) return i;

			// the remainder of NSample cannot be represented as 2^n * 3^k * 5^m
			// adding 1 guarantees that it at least dividable by 2
			return i * NearPowerOfSmallPrimes( nSample+1); 
		}

		/// <summary>
		/// Limits the value from top and bottom
		/// </summary>
		/// <param name="Val">Value to limit</param>
		/// <param name="MinVal">Minimum value</param>
		/// <param name="MaxVal">Maximum value</param>
		/// <returns></returns>
		public static int LimitValue( int MinVal, int Val, int MaxVal)
		{
			if( Val < MinVal) return MinVal;
			if( Val > MaxVal) return MaxVal;
			return Val;
		}

		/// <summary>
		/// Limits the value from top and bottom
		/// </summary>
		/// <param name="Val">Value to limit</param>
		/// <param name="MinVal">Minimum value</param>
		/// <param name="MaxVal">Maximum value</param>
		/// <returns></returns>
		public static float LimitValue( float MinVal, float Val, float MaxVal)
		{
			if( Val < MinVal) return MinVal;
			if( Val > MaxVal) return MaxVal;
			return Val;
		}

		/// <summary>
		/// Limits the value from top and bottom
		/// </summary>
		/// <param name="Val">Value to limit</param>
		/// <param name="MinVal">Minimum value</param>
		/// <param name="MaxVal">Maximum value</param>
		/// <returns></returns>
		public static double LimitValue( double MinVal, double Val, double MaxVal)
		{
			if( Val < MinVal) return MinVal;
			if( Val > MaxVal) return MaxVal;
			return Val;
		}

		/// <summary>
		/// Finds if the value is within boundaries from top to bottom
		/// </summary>
		/// <param name="Val">Value to check</param>
		/// <param name="MinVal">Minimum value</param>
		/// <param name="MaxVal">Maximum value</param>
		/// <returns></returns>
		public static bool IsWithin( int MinVal, int Val, int MaxVal)
		{
			if( Val < MinVal) return false;
			if( Val > MaxVal) return false;
			return true;
		}

		/// <summary>
		/// Finds if the value is within boundaries from top to bottom
		/// </summary>
		/// <param name="Val">Value to check</param>
		/// <param name="MinVal">Minimum value</param>
		/// <param name="MaxVal">Maximum value</param>
		/// <returns></returns>
		public static bool IsWithin( float MinVal, float Val, float MaxVal)
		{
			if( Val < MinVal) return false;
			if( Val > MaxVal) return false;
			return true;
		}

		/// <summary>
		/// Finds if the value is within boundaries from top to bottom
		/// </summary>
		/// <param name="Val">Value to check</param>
		/// <param name="MinVal">Minimum value</param>
		/// <param name="MaxVal">Maximum value</param>
		/// <returns></returns>
		public static bool IsWithin( double MinVal, double Val, double MaxVal)
		{
			if( Val < MinVal) return false;
			if( Val > MaxVal) return false;
			return true;
		}

		/// <summary>
		/// Determines true angle from X, Y Cartesian coordinates (X is horizontal, Y is vertical, counter-clockwise from X to Y)
		/// The true anlge is a Cartesian argument, from X counter-clockwise
		/// </summary>
		/// <param name="X">X-coordinate</param>
		/// <param name="Y">Y-coordinate</param>
		/// <returns>Angle in radians, within 0.0 to 2*Pi</returns>
		public static float TrueAngle( float X, float Y)
		{
			double modX = Math.Abs( X);
			double modY = Math.Abs( Y);
			double myEpsilon = System.Single.Epsilon * 5.0;
			if( modX < myEpsilon && modY < myEpsilon) return 0.0f;
			if( modX >= modY)
			{
				double angle = Math.Atan( modY / modX);
				if( X >= 0.0f && Y >= 0.0f) return (float)angle;
				if( X < 0.0f && Y >= 0.0f) return (float)(Math.PI-angle);
				if( X < 0.0f && Y < 0.0f) return (float)(Math.PI+angle);
				return (float)(2.0*Math.PI-angle);
			}
			else
			{
				double angle = Math.Atan( modX / modY);
				if( X >= 0.0f && Y >= 0.0f) return (float)(Math.PI*0.5 - angle);
				if( X < 0.0f && Y >= 0.0f) return (float)(Math.PI*0.5 + angle);
				if( X < 0.0f && Y < 0.0f) return (float)(Math.PI*1.5 - angle);
				return (float)(Math.PI*1.5+angle);
			}
		}

		/// <summary>
		/// Determines true angle from X, Y Cartesian coordinates (X is horizontal, Y is vertical, counter-clockwise from X to Y)
		/// The true anlge is a Cartesian argument, from X counter-clockwise
		/// </summary>
		/// <param name="X">X-coordinate</param>
		/// <param name="Y">Y-coordinate</param>
		/// <returns>Angle in radians, within 0.0 to 2*Pi</returns>
		public static double TrueAngle( double X, double Y)
		{
			double modX = Math.Abs( X);
			double modY = Math.Abs( Y);
			double myEpsilon = System.Double.Epsilon * 5.0;
			if( modX < myEpsilon && modY < myEpsilon) return 0.0;
			if( modX >= modY)
			{
				double angle = Math.Atan( modY / modX);
				if( X >= 0.0 && Y >= 0.0) return angle;
				if( X < 0.0 && Y >= 0.0) return Math.PI-angle;
				if( X < 0.0 && Y < 0.0) return Math.PI+angle;
				return 2.0*Math.PI-angle;
			}
			else
			{
				double angle = Math.Atan( modX / modY);
				if( X >= 0.0 && Y >= 0.0) return Math.PI*0.5 - angle;
				if( X < 0.0 && Y >= 0.0) return Math.PI*0.5 + angle;
				if( X < 0.0 && Y < 0.0) return Math.PI*1.5 - angle;
				return Math.PI*1.5+angle;
			}
		}

		/// <summary>
		/// Converts Radians to degrees
		/// </summary>
		public static float Rad2Degr( float Rad)
		{
			return (float)(Rad * 180.0 / Math.PI);
		}

		/// <summary>
		/// Converts Radians to degrees
		/// </summary>
		public static float Degr2Rad( float Degr)
		{
			return (float)(Degr * Math.PI / 180.0);
		}

		/// <summary>
		/// Converts Radians to degrees
		/// </summary>
		public static double Rad2Degr( double Rad)
		{
			return Rad * 180.0 / Math.PI;
		}

		/// <summary>
		/// Converts Radians to degrees
		/// </summary>
		public static double Degr2Rad( double Degr)
		{
			return Degr * Math.PI / 180.0;
		}

		/// <summary>
		/// Determines true azimuth from X, Y Topographic coordinates (X-North, Y-East)
		/// </summary>
		/// <param name="North">X-coordinate</param>
		/// <param name="East">Y-coordinate</param>
		/// <returns>Angle in radians, within 0.0 to 2*Pi, representing true azimuth from North</returns>
		public static float TopographicAzimuth( float North, float East)
		{
			return SeismicMath.TrueAngle( North, East); 
		}

		/// <summary>
		/// Determines true azimuth from X, Y Topographic coordinates (X-North, Y-East)
		/// </summary>
		/// <param name="North">X-coordinate</param>
		/// <param name="East">Y-coordinate</param>
		/// <returns>Angle in degrees, within 0.0 to 360, representing true azimuth from North</returns>
		public static float TopographicAzimuthDegr( float North, float East)
		{
			return SeismicMath.Rad2Degr( SeismicMath.TrueAngle( North, East)); 
		}

		/// <summary>
		/// Determines true azimuth from X, Y Topographic coordinates (X-North, Y-East)
		/// </summary>
		/// <param name="North">X-coordinate</param>
		/// <param name="East">Y-coordinate</param>
		/// <returns>Angle in radians, within 0.0 to 2*Pi, representing true azimuth from North</returns>
		public static double TopographicAzimuth( double North, double East)
		{
			return SeismicMath.TrueAngle( North, East); 
		}

		/// <summary>
		/// Determines true azimuth from X, Y Topographic coordinates (X-North, Y-East)
		/// </summary>
		/// <param name="North">X-coordinate</param>
		/// <param name="East">Y-coordinate</param>
		/// <returns>Angle in degrees, within 0.0 to 360, representing true azimuth from North</returns>
		public static double TopographicAzimuthDegr( double North, double East)
		{
			return SeismicMath.Rad2Degr( SeismicMath.TrueAngle( North, East)); 
		}

		/// <summary>
		/// Determines range from X, Y Topographic coordinates (X-North, Y-East)
		/// </summary>
		/// <param name="North">X-coordinate</param>
		/// <param name="East">Y-coordinate</param>
		/// <returns>range</returns>
		public static float TopographicRange( float North, float East)
		{
			return (float)Math.Sqrt( North*North + East*East); 
		}

		/// <summary>
		/// Determines range from X, Y Topographic coordinates (X-North, Y-East)
		/// </summary>
		/// <param name="North">X-coordinate</param>
		/// <param name="East">Y-coordinate</param>
		/// <returns>range</returns>
		public static double TopographicRange( double North, double East)
		{
			return Math.Sqrt( North*North + East*East); 
		}
	}
}
