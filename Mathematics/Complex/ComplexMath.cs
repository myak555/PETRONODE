using System;
using System.Diagnostics;

namespace Petronode.Mathematics.Complex
{
	/// <summary>
	/// <p>Various mathematical functions for complex numbers.</p>
	/// </summary>
	public class ComplexMath 
	{
		
		//---------------------------------------------------------------------------------------------------

		private ComplexMath() {
		}

		//---------------------------------------------------------------------------------------------------

		/// <summary>
		/// Swap two complex numbers
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		static public void Swap( ref ComplexD a, ref ComplexD b ) {
			ComplexD temp = a;
			a = b;
			b = temp;
		}

		/// <summary>
		/// Swap two complex numbers
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		static public void Swap( ref ComplexF a, ref ComplexF b ) {
			ComplexF temp = a;
			a = b;
			b = temp;
		}
		
		//---------------------------------------------------------------------------------------------------

		private const double _halfOfRoot2	= 0.70710678; //0.5 * Math.Sqrt( 2 );

		/// <summary>
		/// Calculate the square root of a complex number
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		static public ComplexF	Sqrt( ComplexF c ) {
			double	x	= c.Re;
			double	y	= c.Im;

			double	modulus	= Math.Sqrt( x*x + y*y );
			int		sign	= ( y < 0 ) ? -1 : 1;

			c.Re		= (float)( _halfOfRoot2 * Math.Sqrt( modulus + x ) );
			c.Im	= (float)( _halfOfRoot2 * sign * Math.Sqrt( modulus - x ) );

			return	c;
		}

		/// <summary>
		/// Calculate the square root of a complex number
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		static public ComplexD	Sqrt( ComplexD c ) {
			double	x	= c.Re;
			double	y	= c.Im;

			double	modulus	= Math.Sqrt( x*x + y*y );
			int		sign	= ( y < 0 ) ? -1 : 1;

			c.Re		= (double)( _halfOfRoot2 * Math.Sqrt( modulus + x ) );
			c.Im	= (double)( _halfOfRoot2 * sign * Math.Sqrt( modulus - x ) );

			return	c;
		}

		//---------------------------------------------------------------------------------------------------

		/// <summary>
		/// Calculate the power of a complex number
		/// </summary>
		/// <param name="c"></param>
		/// <param name="exponent"></param>
		/// <returns></returns>
		static public ComplexF	Pow( ComplexF c, double exponent ) {
			double	x	= c.Re;
			double	y	= c.Im;
			
			double	modulus		= Math.Pow( x*x + y*y, exponent * 0.5 );
			double	argument	= Math.Atan2( y, x ) * exponent;

			c.Re		= (float)( modulus * System.Math.Cos( argument ) );
			c.Im = (float)( modulus * System.Math.Sin( argument ) );

			return	c;
		}

		/// <summary>
		/// Calculate the power of a complex number
		/// </summary>
		/// <param name="c"></param>
		/// <param name="exponent"></param>
		/// <returns></returns>
		static public ComplexD	Pow( ComplexD c, double exponent ) {
			double	x	= c.Re;
			double	y	= c.Im;
			
			double	modulus		= Math.Pow( x*x + y*y, exponent * 0.5 );
			double	argument	= Math.Atan2( y, x ) * exponent;

			c.Re		= (double)( modulus * System.Math.Cos( argument ) );
			c.Im = (double)( modulus * System.Math.Sin( argument ) );

			return	c;
		}
		
		//---------------------------------------------------------------------------------------------------

	}
}
