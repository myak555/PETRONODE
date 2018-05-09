using System;
using System.Diagnostics;

namespace Petronode.Mathematics.Complex
{
	/// <summary>
	/// <p>A set of statistical utilities for complex number arrays</p>
	/// </summary>
	public class ComplexStats
	{
		//---------------------------------------------------------------------------------------------

		private ComplexStats() {
		}

		//---------------------------------------------------------------------------------------------
		//--------------------------------------------------------------------------------------------

		/// <summary>
		/// Calculate the sum
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		static public  ComplexF		Sum( ComplexF[] data ) {
			Debug.Assert( data != null );
			return	SumRecursion( data, 0, data.Length );
		}
		static private ComplexF		SumRecursion( ComplexF[] data, int start, int end ) {
			Debug.Assert( 0 <= start, "start = " + start );
			Debug.Assert( start < end, "start = " + start + " and end = " + end );
			Debug.Assert( end <= data.Length, "end = " + end + " and data.Length = " + data.Length );
			if( ( end - start ) <= 1000 ) {
				ComplexF sum = ComplexF.Zero;
				for( int i = start; i < end; i ++ ) {
					sum += data[ i ];
				
				}
				return	sum;
			}
			else {
				int middle = ( start + end ) >> 1;
				return	SumRecursion( data, start, middle ) + SumRecursion( data, middle, end );
			}
		}

		/// <summary>
		/// Calculate the sum
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		static public  ComplexD		Sum( ComplexD[] data ) {
			Debug.Assert( data != null );
			return	SumRecursion( data, 0, data.Length );
		}
		static private ComplexD		SumRecursion( ComplexD[] data, int start, int end ) {
			Debug.Assert( 0 <= start, "start = " + start );
			Debug.Assert( start < end, "start = " + start + " and end = " + end );
			Debug.Assert( end <= data.Length, "end = " + end + " and data.Length = " + data.Length );
			if( ( end - start ) <= 1000 ) {
				ComplexD sum = ComplexD.Zero;
				for( int i = start; i < end; i ++ ) {
					sum += data[ i ];
				
				}
				return	sum;
			}
			else {
				int middle = ( start + end ) >> 1;
				return	SumRecursion( data, start, middle ) + SumRecursion( data, middle, end );
			}
		}

		//--------------------------------------------------------------------------------------------
		//--------------------------------------------------------------------------------------------

		/// <summary>
		/// Calculate the sum of squares
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		static public ComplexF		SumOfSquares( ComplexF[] data ) {
			Debug.Assert( data != null );
			return	SumOfSquaresRecursion( data, 0, data.Length );
		}
		static private ComplexF		SumOfSquaresRecursion( ComplexF[] data, int start, int end ) {
			Debug.Assert( 0 <= start, "start = " + start );
			Debug.Assert( start < end, "start = " + start + " and end = " + end );
			Debug.Assert( end <= data.Length, "end = " + end + " and data.Length = " + data.Length );
			if( ( end - start ) <= 1000 ) {
				ComplexF sumOfSquares = ComplexF.Zero;
				for( int i = start; i < end; i ++ ) {
					sumOfSquares += data[ i ] * data[ i ];
				
				}
				return	sumOfSquares;
			}
			else {
				int middle = ( start + end ) >> 1;
				return	SumOfSquaresRecursion( data, start, middle ) + SumOfSquaresRecursion( data, middle, end );
			}
		}

		/// <summary>
		/// Calculate the sum of squares
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		static public ComplexD		SumOfSquares( ComplexD[] data ) {
			Debug.Assert( data != null );
			return	SumOfSquaresRecursion( data, 0, data.Length );
		}
		static private ComplexD		SumOfSquaresRecursion( ComplexD[] data, int start, int end ) {
			Debug.Assert( 0 <= start, "start = " + start );
			Debug.Assert( start < end, "start = " + start + " and end = " + end );
			Debug.Assert( end <= data.Length, "end = " + end + " and data.Length = " + data.Length );
			if( ( end - start ) <= 1000 ) {
				ComplexD sumOfSquares = ComplexD.Zero;
				for( int i = start; i < end; i ++ ) {
					sumOfSquares += data[ i ] * data[ i ];
				
				}
				return	sumOfSquares;
			}
			else {
				int middle = ( start + end ) >> 1;
				return	SumOfSquaresRecursion( data, start, middle ) + SumOfSquaresRecursion( data, middle, end );
			}
		}

		//--------------------------------------------------------------------------------------------
		//--------------------------------------------------------------------------------------------

		/// <summary>
		/// Calculate the mean (average)
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		static public ComplexF		Mean( ComplexF[] data ) {
			return	ComplexStats.Sum( data ) / data.Length;
		}

		/// <summary>
		/// Calculate the mean (average)
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		static public ComplexD		Mean( ComplexD[] data ) {
			return	ComplexStats.Sum( data ) / data.Length;
		}

		/// <summary>
		/// Calculate the variance
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		static public ComplexF	Variance( ComplexF[] data ) {
			Debug.Assert( data != null );
			if( data.Length == 0 ) {
				throw new DivideByZeroException( "length of data is zero" );
			}
			return	ComplexStats.SumOfSquares( data ) / data.Length - ComplexStats.Sum( data );
		}
		/// <summary>
		/// Calculate the variance 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		static public ComplexD	Variance( ComplexD[] data ) {
			Debug.Assert( data != null );
			if( data.Length == 0 ) {
				throw new DivideByZeroException( "length of data is zero" );
			}
			return	ComplexStats.SumOfSquares( data ) / data.Length - ComplexStats.Sum( data );
		}

		/// <summary>
		/// Calculate the standard deviation
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		static public ComplexF	StdDev( ComplexF[] data ) {
			Debug.Assert( data != null );
			if( data.Length == 0 ) {
				throw new DivideByZeroException( "length of data is zero" );
			}
			return	ComplexMath.Sqrt( ComplexStats.Variance( data ) );
		}
		/// <summary>
		/// Calculate the standard deviation 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		static public ComplexD	StdDev( ComplexD[] data ) {
			Debug.Assert( data != null );
			if( data.Length == 0 ) {
				throw new DivideByZeroException( "length of data is zero" );
			}
			return	ComplexMath.Sqrt( ComplexStats.Variance( data ) );
		}

		//--------------------------------------------------------------------------------------------
		//--------------------------------------------------------------------------------------------

		/// <summary>
		/// Calculate the root mean squared (RMS) error between two sets of data.
		/// </summary>
		/// <param name="alpha"></param>
		/// <param name="beta"></param>
		/// <returns></returns>
		static public float	RMSError( ComplexF[] alpha, ComplexF[] beta ) {
			Debug.Assert( alpha != null );
			Debug.Assert( beta != null );
			Debug.Assert( beta.Length == alpha.Length );

			return (float) Math.Sqrt( SumOfSquaredErrorRecursion( alpha, beta, 0, alpha.Length ) );
		}
		static private float	SumOfSquaredErrorRecursion( ComplexF[] alpha, ComplexF[] beta, int start, int end ) {
			Debug.Assert( 0 <= start, "start = " + start );
			Debug.Assert( start < end, "start = " + start + " and end = " + end );
			Debug.Assert( end <= alpha.Length, "end = " + end + " and alpha.Length = " + alpha.Length );
			Debug.Assert( beta.Length == alpha.Length );
			if( ( end - start ) <= 1000 ) {
				float sumOfSquaredError = 0;
				for( int i = start; i < end; i ++ ) {
					ComplexF delta = beta[ i ] - alpha[ i ];
					sumOfSquaredError += ( delta.Re * delta.Re ) + ( delta.Im * delta.Im );
				
				}
				return	sumOfSquaredError;
			}
			else {
				int middle = ( start + end ) >> 1;
				return	SumOfSquaredErrorRecursion( alpha, beta, start, middle ) + SumOfSquaredErrorRecursion( alpha, beta, middle, end );
			}
		}

		/// <summary>
		/// Calculate the root mean squared (RMS) error between two sets of data.
		/// </summary>
		/// <param name="alpha"></param>
		/// <param name="beta"></param>
		/// <returns></returns>
		static public double	RMSError( ComplexD[] alpha, ComplexD[] beta ) {
			Debug.Assert( alpha != null );
			Debug.Assert( beta != null );
			Debug.Assert( beta.Length == alpha.Length );

			return Math.Sqrt( SumOfSquaredErrorRecursion( alpha, beta, 0, alpha.Length ) );
		}
		static private double	SumOfSquaredErrorRecursion( ComplexD[] alpha, ComplexD[] beta, int start, int end ) {
			Debug.Assert( 0 <= start, "start = " + start );
			Debug.Assert( start < end, "start = " + start + " and end = " + end );
			Debug.Assert( end <= alpha.Length, "end = " + end + " and alpha.Length = " + alpha.Length );
			Debug.Assert( beta.Length == alpha.Length );
			if( ( end - start ) <= 1000 ) {
				double sumOfSquaredError = 0;
				for( int i = start; i < end; i ++ ) {
					ComplexD delta = beta[ i ] - alpha[ i ];
					sumOfSquaredError += ( delta.Re * delta.Re ) + ( delta.Im * delta.Im );
				
				}
				return	sumOfSquaredError;
			}
			else {
				int middle = ( start + end ) >> 1;
				return	SumOfSquaredErrorRecursion( alpha, beta, start, middle ) + SumOfSquaredErrorRecursion( alpha, beta, middle, end );
			}
		}


	}
}
