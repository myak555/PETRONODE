using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Petronode.Mathematics.Complex
{
    /// <summary>
    /// <p>A double-precision complex number representation.</p>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ComplexD : IComparable, ICloneable 
    {
        /// <summary>
        /// The real component of the complex number
        /// </summary>
        public double Re;

        /// <summary>
        /// The imaginary component of the complex number
        /// </summary>
        public double Im;

		//-----------------------------------------------------------------------------------
		//-----------------------------------------------------------------------------------

        /// <summary>
        /// Create a complex number from a real and an imaginary component
        /// </summary>
        /// <param name="real">real part</param>
        /// <param name="imaginary">imaginary part</param>
        public ComplexD( double real, double imaginary ) 
        {
	        this.Re	= real;
	        this.Im	= imaginary;
        }

		/// <summary>
		/// Create a complex number based on an existing complex number
		/// </summary>
		/// <param name="c"></param>
		public ComplexD( ComplexD c )
        {
			this.Re	= c.Re;
			this.Im	= c.Im;
		}

		/// <summary>
		/// Create a complex number from a real and an imaginary component
		/// </summary>
		/// <param name="real"></param>
		/// <param name="imaginary"></param>
		/// <returns></returns>
		static public ComplexD FromRealImaginary( double real, double imaginary )
        {
			ComplexD c;
			c.Re = real;
			c.Im = imaginary;
			return c;
		}

		/// <summary>
		/// Create a complex number from a modulus (length) and an argument (radian)
		/// </summary>
		/// <param name="modulus"></param>
		/// <param name="argument"></param>
		/// <returns></returns>
		static public ComplexD FromModulusArgument( double modulus, double argument )
        {
			ComplexD c;
			c.Re = modulus * System.Math.Cos( argument );
			c.Im = modulus * System.Math.Sin( argument );
			return c;
		}

		/// <summary>
		/// Create a complex number from a real and assume imaginary is zero
		/// </summary>
		/// <returns></returns>
		static public ComplexD FromReal( double real) 
		{
			ComplexD c;
			c.Re = real;
			c.Im = 0.0;
			return c;
		}

		//-----------------------------------------------------------------------------------
		//-----------------------------------------------------------------------------------

        /// <summary>
        /// Implements clonable interface
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone() 
		{
			return new ComplexD( this );
		}

		/// <summary>
		/// Clone the complex number
		/// </summary>
		/// <returns></returns>
		public ComplexD Clone()
        {
			return	new ComplexD( this );
		}
		
		//-----------------------------------------------------------------------------------
		//-----------------------------------------------------------------------------------

		/// <summary>
		/// The modulus (length) of the complex number
		/// </summary>
		/// <returns></returns>
		public double GetModulus()
        {
            return Math.Sqrt(GetModulusSquared());
		}

		/// <summary>
		/// The squared modulus (length^2) of the complex number
		/// </summary>
		/// <returns></returns>
		public double GetModulusSquared()
        {
			double x = this.Re;
			double y = this.Im;
			return x*x + y*y;
		}

		/// <summary>
		/// The argument (radians) of the complex number
		/// </summary>
		/// <returns></returns>
		public double GetArgument()
        {
			return Math.Atan2( this.Im, this.Re );
		}

		//-----------------------------------------------------------------------------------

		/// <summary>
		/// Get the conjugate of the complex number
		/// </summary>
		/// <returns></returns>
		public ComplexD GetConjugate()
        {
			return FromRealImaginary( this.Re, -this.Im );
		}

		//-----------------------------------------------------------------------------------

		/// <summary>
		/// Scale the complex number to 1.
		/// </summary>
		public void Normalize()
        {
			double modulus = this.GetModulus();
			if( modulus == 0.0 ) {
				throw new DivideByZeroException( "Can not normalize a complex number that is zero." );
			}
			this.Re	= this.Re / modulus;
			this.Im	= this.Im / modulus;
		}

		//-----------------------------------------------------------------------------------
		//-----------------------------------------------------------------------------------

		/// <summary>
		/// Convert to a from double precision complex number to a single precison complex number
		/// </summary>
		/// <param name="cF"></param>
		/// <returns></returns>
		public static explicit operator ComplexD ( ComplexF cF )
        {
			ComplexD c;
			c.Re	= (double) cF.Re;
			c.Im	= (double) cF.Im;
			return c;
		}
		
		/// <summary>
		/// Convert from a single precision real number to a complex number
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static explicit operator ComplexD ( double d )
        {
			ComplexD c;
			c.Re	= d;
			c.Im	= 0.0;
			return c;
		}

		/// <summary>
		/// Convert from a single precision complex to a real number
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public static explicit operator double ( ComplexD c )
        {
			return c.Re;
		}
		
		//-----------------------------------------------------------------------------------
		//-----------------------------------------------------------------------------------

		/// <summary>
		/// Are these two complex numbers equivalent?
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool	operator==( ComplexD a, ComplexD b )
        {
			return	( a.Re == b.Re ) && ( a.Im == b.Im );
		}

		/// <summary>
		/// Are these two complex numbers different?
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool	operator!=( ComplexD a, ComplexD b ) 
        {
			return	( a.Re != b.Re ) || ( a.Im != b.Im );
		}

		/// <summary>
		/// Get the hash code of the complex number
		/// </summary>
		/// <returns></returns>
		public override int	GetHashCode() 
        {
			return	( this.Re.GetHashCode() ^ this.Im.GetHashCode() );
		}

		/// <summary>
		/// Is this complex number equivalent to another object?
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public override bool Equals( object o )
        {
			if( o is ComplexD || o is double )
            {
				ComplexD c = (ComplexD) o;
				return  ( this == c );
			}
            return false;
		}

		//-----------------------------------------------------------------------------------
		//-----------------------------------------------------------------------------------

		/// <summary>
		/// Compare to other complex numbers or real numbers
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public int	CompareTo( object o ) 
        {
			if( o == null ) 
            {
				return 1;  // null sorts before current
			}
			if( o is ComplexD ) 
            {
				return	this.GetModulus().CompareTo( ((ComplexD)o).GetModulus() );
			}
			if( o is double ) 
            {
				return	this.GetModulus().CompareTo( (double)o );
			}
			if( o is ComplexF ) 
            {
				return	this.GetModulus().CompareTo( ((ComplexF)o).GetModulus() );
			}
			if( o is float ) 
            {
				return	this.GetModulus().CompareTo( (float)o );
			}
			throw new ArgumentException();
		}

		//-----------------------------------------------------------------------------------
		//-----------------------------------------------------------------------------------

		/// <summary>
		/// This operator doesn't do much. :-)
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static ComplexD operator+( ComplexD a ) 
        {
			return a;
		}

		/// <summary>
		/// Negate the complex number
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static ComplexD operator-( ComplexD a ) 
        {
			a.Re = -a.Re;
			a.Im = -a.Im;
			return a;
		}

		/// <summary>
		/// Add a complex number to a real
		/// </summary>
		/// <param name="a"></param>
		/// <param name="f"></param>
		/// <returns></returns>
		public static ComplexD operator+( ComplexD a, double f ) 
        {
			a.Re += f;
			return a;
		}

		/// <summary>
		/// Add a real to a complex number
		/// </summary>
		/// <param name="f"></param>
		/// <param name="a"></param>
		/// <returns></returns>
		public static ComplexD operator+( double f, ComplexD a ) 
        {
			a.Re += f;
			return a;
		}

		/// <summary>
		/// Add two complex numbers
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static ComplexD operator+( ComplexD a, ComplexD b ) 
        {
			a.Re += b.Re;
			a.Im += b.Im;
			return a;
		}

		/// <summary>
		/// Subtract a real from a complex number
		/// </summary>
		/// <param name="a"></param>
		/// <param name="f"></param>
		/// <returns></returns>
		public static ComplexD operator-( ComplexD a, double f ) 
        {
			a.Re -= f;
			return a;
		}

		/// <summary>
		/// Subtract a complex number from a real
		/// </summary>
		/// <param name="f"></param>
		/// <param name="a"></param>
		/// <returns></returns>
		public static ComplexD operator-( double f, ComplexD a )
        {
			a.Re = f - a.Re;
			return a;
		}

		/// <summary>
		/// Subtract two complex numbers
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static ComplexD operator-( ComplexD a, ComplexD b )
        {
			a.Re -= b.Re;
			a.Im -= b.Im;
			return a;
		}

		/// <summary>
		/// Multiply a complex number by a real
		/// </summary>
		/// <param name="a"></param>
		/// <param name="f"></param>
		/// <returns></returns>
		public static ComplexD operator*( ComplexD a, double f )
        {
			a.Re *= f;
			a.Im *= f;
			return a;
		}
		
		/// <summary>
		/// Multiply a real by a complex number
		/// </summary>
		/// <param name="f"></param>
		/// <param name="a"></param>
		/// <returns></returns>
		public static ComplexD operator*( double f, ComplexD a )
        {
			a.Re *= f;
			a.Im *= f;
			return a;
		}
		
		/// <summary>
		/// Multiply two complex numbers together
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static ComplexD operator*( ComplexD a, ComplexD b )
        {
			// (x + yi)(u + vi) = (xu – yv) + (xv + yu)i. 
			double	x = a.Re, y = a.Im;
			double	u = b.Re, v = b.Im;
			a.Re = x*u - y*v;
			a.Im = x*v + y*u;
			return a;
		}

		/// <summary>
		/// Divide a complex number by a real number
		/// </summary>
		/// <param name="a"></param>
		/// <param name="f"></param>
		/// <returns></returns>
		public static ComplexD operator/( ComplexD a, double f )
        {
			if( f == 0 ) throw new DivideByZeroException();
			a.Re /= f;
			a.Im /= f;
			return a;
		}

		/// <summary>
		/// Divide a double number by a complex number
		/// </summary>
		/// <param name="a"></param>
		/// <param name="f"></param>
		/// <returns></returns>
		public static ComplexD operator/( double f, ComplexD a ) 
		{
			double	u = a.Re,	v = a.Im;
			double	denom = u*u + v*v;
			if( denom == 0 ) throw new DivideByZeroException();
            denom = f / denom;
			a.Re	= u * denom;
			a.Im	= - v * denom;
			return a;
		}

		/// <summary>
		/// Divide a complex number by a complex number
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static ComplexD operator/( ComplexD a, ComplexD b ) 
		{
			double	x = a.Re,	y = a.Im;
			double	u = b.Re,	v = b.Im;
			double	denom = u*u + v*v;
			if( denom == 0 ) throw new DivideByZeroException();
			a.Re	= ( x*u + y*v ) / denom;
			a.Im	= ( y*u - x*v ) / denom;
			return a;
		}

		/// <summary>
		/// Parse a complex representation in this fashion: "( %f, %f )"
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		static public ComplexD Parse( string s )
        {
            char[] sep = {'(', ',', 'i', ' '};
            string[] ss = s.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            ComplexD c = FromReal(0.0);
            if (ss.Length >= 1) c.Re = Convert.ToDouble(ss[0]);
            if (ss.Length >= 2) c.Im = Convert.ToDouble(ss[1]);
            return c;
		}
		
		/// <summary>
		/// Get the string representation
		/// </summary>
		/// <returns></returns>
		public override string ToString()
        {
			return	String.Format( "( {0}, {1}i )", this.Re, this.Im );
		}

        /// <summary>
        /// Get the string representation
        /// </summary>
        /// <returns></returns>
        public string ToString( string format)
        {
            string s = "( " + Re.ToString(format) + ", " + Im.ToString(format) + "i )"; 
            return s;
        }

		//-----------------------------------------------------------------------------------
		//-----------------------------------------------------------------------------------

		/// <summary>
		/// Determine whether two complex numbers are almost (i.e. within the tolerance) equivalent.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="tolerance"></param>
		/// <returns></returns>
		static public bool IsEqual( ComplexD a, ComplexD b, double tolerance )
        {
			return
				( Math.Abs( a.Re - b.Re ) < tolerance ) &&
				( Math.Abs( a.Im - b.Im ) < tolerance );
		}
		
		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------

		/// <summary>
		/// Represents zero
		/// </summary>
		static public ComplexD	Zero
        {
			get	{	return	new ComplexD( 0.0, 0.0 );}
		}

		/// <summary>
		/// Represents the result of sqrt( -1 )
		/// </summary>
		static public ComplexD	I
        {
			get {	return	new ComplexD( 0.0, 1.0 );	}
		}

		/// <summary>
		/// Represents the largest possible value of ComplexD.
		/// </summary>
		static public ComplexD	MaxValue 
        {
			get {	return	new ComplexD( double.MaxValue, double.MaxValue );	}
		}

		/// <summary>
		/// Represents the smallest possible value of ComplexD.
		/// </summary>
		static public ComplexD	MinValue
        {
			get {	return	new ComplexD( double.MinValue, double.MinValue );	}
		}

        /// <summary>
        /// Represents the absent value of ComplexD.
        /// </summary>
        static public ComplexD NaN
        {
            get { return new ComplexD(Double.NaN, Double.NaN); }
        }
		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
	}

}
