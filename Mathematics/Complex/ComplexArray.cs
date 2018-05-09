using System;
using System.Diagnostics;

namespace Petronode.Mathematics.Complex
{
	/// <summary>
	/// <p>A set of array utilities for complex number arrays</p>
	/// </summary>
	public class ComplexArray 
	{

		//---------------------------------------------------------------------------------------------

		private ComplexArray() {
		}

		//---------------------------------------------------------------------------------------------

		/// <summary>
		/// Clamp length (modulus) of the elements in the complex array
		/// </summary>
		/// <param name="array"></param>
		/// <param name="fMinimum"></param>
		/// <param name="fMaximum"></param>
		static public void ClampLength( ComplexD[] array, double fMinimum, double fMaximum ) {
			for( int i = 0; i < array.Length; i ++ ) {
				array[i] = ComplexD.FromModulusArgument( Math.Max( fMinimum, Math.Min( fMaximum, array[i].GetModulus() ) ), array[i].GetArgument() );
			}
		}

		/// <summary>
		/// Clamp elements in the complex array to range [minimum,maximum]
		/// </summary>
		/// <param name="array"></param>
		/// <param name="minimum"></param>
		/// <param name="maximum"></param>
		static public void Clamp( ComplexD[] array, ComplexD minimum, ComplexD maximum ) {
			for( int i = 0; i < array.Length; i ++ ) {
				array[i].Re	= Math.Min( Math.Max( array[ i ].Re, minimum.Re ), maximum.Re );
				array[i].Im	= Math.Min( Math.Max( array[ i ].Re, minimum.Im ), maximum.Im );
			}
		}

		/// <summary>
		/// Clamp elements in the complex array to real unit range (i.e. [0,1])
		/// </summary>
		/// <param name="array"></param>
		static public void ClampToRealUnit( ComplexD[] array ) {
			for( int i = 0; i < array.Length; i ++ ) {
				array[i].Re =  Math.Min( Math.Max( array[i].Re, 0 ), 1 );
				array[i].Im = 0;
			}
		}
		
		//---------------------------------------------------------------------------------------------

		static private bool			_workspaceFLocked	= false;
		static private ComplexF[]	_workspaceF			= new ComplexF[ 0 ];

		static private void		LockWorkspaceF( int length, ref ComplexF[] workspace ) {
			Debug.Assert( _workspaceFLocked == false );
			_workspaceFLocked = true;
			if( length >= _workspaceF.Length ) {
				_workspaceF	= new ComplexF[ length ];
			}
			workspace =	_workspaceF;
		}
		static private void		UnlockWorkspaceF( ref ComplexF[] workspace ) {
			Debug.Assert( _workspaceF == workspace );
			Debug.Assert( _workspaceFLocked == true );
			_workspaceFLocked = false;
			workspace = null;
		}

		//---------------------------------------------------------------------------------------------

		/// <summary>
		/// Shift (offset) the elements in the array
		/// </summary>
		/// <param name="array"></param>
		/// <param name="offset"></param>
		static public void Shift( ComplexD[] array, int offset ) {
			Debug.Assert( array != null );
			Debug.Assert( offset >= 0 );
			Debug.Assert( offset < array.Length );

			if( offset == 0 ) {
				return;
			}

			int			length	= array.Length;
			ComplexD[]	temp	= new ComplexD[ length ];

			for( int i = 0; i < length; i ++ ) {
				temp[ ( i + offset ) % length ] = array[ i ];
			}
			for( int i = 0; i < length; i ++ ) {
				array[ i ] = temp[ i ];
			}
		}

		/// <summary>
		/// Shift (offset) the elements in the array
		/// </summary>
		/// <param name="array"></param>
		/// <param name="offset"></param>
		static public void Shift( ComplexF[] array, int offset ) {
			Debug.Assert( array != null );
			Debug.Assert( offset >= 0 );
			Debug.Assert( offset < array.Length );

			if( offset == 0 ) {
				return;
			}

			int			length		= array.Length;
			ComplexF[]	workspace	= null;
			ComplexArray.LockWorkspaceF( length, ref workspace );

			for( int i = 0; i < length; i ++ ) {
				workspace[ ( i + offset ) % length ] = array[ i ];
			}
			for( int i = 0; i < length; i ++ ) {
				array[ i ] = workspace[ i ];
			}

			ComplexArray.UnlockWorkspaceF( ref workspace );
		}

		//---------------------------------------------------------------------------------------------

		/// <summary>
		/// Get the range of element lengths
		/// </summary>
		/// <param name="array"></param>
		/// <param name="minimum"></param>
		/// <param name="maximum"></param>
		static public void GetLengthRange( ComplexD[] array, ref double minimum, ref double maximum ) {
			minimum = +double.MaxValue;
			maximum = -double.MaxValue;
			for( int i = 0; i < array.Length; i ++ ) {
				double temp = array[i].GetModulus();
				minimum = Math.Min( temp, minimum );
				maximum = Math.Max( temp, maximum );
			}
		}
		/// <summary>
		/// Get the range of element lengths
		/// </summary>
		/// <param name="array"></param>
		/// <param name="minimum"></param>
		/// <param name="maximum"></param>
		static public void GetLengthRange( ComplexF[] array, ref float minimum, ref float maximum ) {
			minimum = +float.MaxValue;
			maximum = -float.MaxValue;
			for( int i = 0; i < array.Length; i ++ ) {
				float temp = array[i].GetModulus();
				minimum = Math.Min( temp, minimum );
				maximum = Math.Max( temp, maximum );
			}
		}

		// // <summary>
		// // Conver the complex array to a double array
		// // </summary>
		// // <param name="array"></param>
		// // <param name="style"></param>
		// // <returns></returns>
		/* static public double[]	ConvertToDoubleArray( ComplexD[] array, ConversionStyle style ) {
			double[] newArray = new double[ array.Length ];
			switch( style ) {
			case ConversionStyle.Length:
				for( int i = 0; i < array.Length; i ++ ) {
					newArray[i] = (double) array[i].GetModulus();
				}
				break;
			case ConversionStyle.Real:
				for( int i = 0; i < array.Length; i ++ ) {
					newArray[i] = (double) array[i].Re;
				}
				break;
			case ConversionStyle.Imaginary:
				for( int i = 0; i < array.Length; i ++ ) {
					newArray[i] = (double) array[i].Im;
				}
				break;
			default:
				Debug.Assert( false );
				break;
			}
			return	newArray;
		}	 */

		//---------------------------------------------------------------------------------------------

		/// <summary>
		/// Determine whether the elements in the two arrays are the same
		/// </summary>
		/// <param name="array1"></param>
		/// <param name="array2"></param>
		/// <param name="tolerance"></param>
		/// <returns></returns>
		static public bool		IsEqual( ComplexD[] array1, ComplexD[] array2, double tolerance ) {
			if ( array1.Length != array2.Length ) {
				return false;
			}
			for( int i = 0; i < array1.Length; i ++ ) {
				if( ComplexD.IsEqual( array1[i], array2[i], tolerance ) == false ) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		///  Determine whether the elements in the two arrays are the same
		/// </summary>
		/// <param name="array1"></param>
		/// <param name="array2"></param>
		/// <param name="tolerance"></param>
		/// <returns></returns>
		static public bool		IsEqual( ComplexF[] array1, ComplexF[] array2, float tolerance ) {
			if ( array1.Length != array2.Length ) {
				return false;
			}
			for( int i = 0; i < array1.Length; i ++ ) {
				if( ComplexF.IsEqual( array1[i], array2[i], tolerance ) == false ) {
					return false;
				}
			}
			return true;
		}

		//---------------------------------------------------------------------------------------------
		
		/// <summary>
		/// Add a specific value to each element in the array
		/// </summary>
		/// <param name="array"></param>
		/// <param name="offset"></param>
		static public void Offset( ComplexD[] array, double offset ) {
			int length = array.Length;
			for( int i = 0; i < length; i ++ ) {
				array[i].Re += offset;
			}
		}

		/// <summary>
		/// Add a specific value to each element in the array
		/// </summary>
		/// <param name="array"></param>
		/// <param name="offset"></param>
		static public void Offset( ComplexD[] array, ComplexD offset ) {
			int length = array.Length;
			for( int i = 0; i < length; i ++ ) {
				array[i] += offset;
			}
		}

		/// <summary>
		/// Add a specific value to each element in the array
		/// </summary>
		/// <param name="array"></param>
		/// <param name="offset"></param>
		static public void Offset( ComplexF[] array, float offset ) {
			int length = array.Length;
			for( int i = 0; i < length; i ++ ) {
				array[i].Re += offset;
			}
		}

		/// <summary>
		/// Add a specific value to each element in the array
		/// </summary>
		/// <param name="array"></param>
		/// <param name="offset"></param>
		static public void Offset( ComplexF[] array, ComplexF offset ) {
			int length = array.Length;
			for( int i = 0; i < length; i ++ ) {
				array[i] += offset;
			}
		}

		//---------------------------------------------------------------------------------------------
		
		/// <summary>
		/// Multiply each element in the array by a specific value
		/// </summary>
		/// <param name="array"></param>
		/// <param name="scale"></param>
		static public void Scale( ComplexD[] array, double scale ) {
			Debug.Assert( array != null );

			int length = array.Length;
			for( int i = 0; i < length; i ++ ) {
				array[i] *= scale;
			}
		}
		/// <summary>
		///  Multiply each element in the array by a specific value
		/// </summary>
		/// <param name="array"></param>
		/// <param name="scale"></param>
		/// <param name="start"></param>
		/// <param name="length"></param>
		static public void Scale( ComplexD[] array, double scale, int start, int length ) {
			Debug.Assert( array != null );
			Debug.Assert( start >= 0 );
			Debug.Assert( length >= 0 );
			Debug.Assert( ( start + length ) < array.Length );

			for( int i = 0; i < length; i ++ ) {
				array[i + start] *= scale;
			}
		}

		/// <summary>
		/// Multiply each element in the array by a specific value
		/// </summary>
		/// <param name="array"></param>
		/// <param name="scale"></param>
		static public void Scale( ComplexD[] array, ComplexD scale ) {
			Debug.Assert( array != null );

			int length = array.Length;
			for( int i = 0; i < length; i ++ ) {
				array[i] *= scale;
			}
		}
		/// <summary>
		/// Multiply each element in the array by a specific value 
		/// </summary>
		/// <param name="array"></param>
		/// <param name="scale"></param>
		/// <param name="start"></param>
		/// <param name="length"></param>
		static public void Scale( ComplexD[] array, ComplexD scale, int start, int length ) {
			Debug.Assert( array != null );
			Debug.Assert( start >= 0 );
			Debug.Assert( length >= 0 );
			Debug.Assert( ( start + length ) < array.Length );

			for( int i = 0; i < length; i ++ ) {
				array[i + start] *= scale;
			}
		}

		/// <summary>
		/// Multiply each element in the array by a specific value
		/// </summary>
		/// <param name="array"></param>
		/// <param name="scale"></param>
		static public void Scale( ComplexF[] array, float scale ) {
			Debug.Assert( array != null );

			int length = array.Length;
			for( int i = 0; i < length; i ++ ) {
				array[i] *= scale;
			}
		}
		/// <summary>
		/// Multiply each element in the array by a specific value 
		/// </summary>
		/// <param name="array"></param>
		/// <param name="scale"></param>
		/// <param name="start"></param>
		/// <param name="length"></param>
		static public void Scale( ComplexF[] array, float scale, int start, int length ) {
			Debug.Assert( array != null );
			Debug.Assert( start >= 0 );
			Debug.Assert( length >= 0 );
			Debug.Assert( ( start + length ) < array.Length );

			for( int i = 0; i < length; i ++ ) {
				array[i + start] *= scale;
			}
		}

		/// <summary>
		/// Multiply each element in the array by a specific value
		/// </summary>
		/// <param name="array"></param>
		/// <param name="scale"></param>
		static public void Scale( ComplexF[] array, ComplexF scale ) {
			Debug.Assert( array != null );

			int length = array.Length;
			for( int i = 0; i < length; i ++ ) {
				array[i] *= scale;
			}
		}
		/// <summary>
		/// Multiply each element in the array by a specific value 
		/// </summary>
		/// <param name="array"></param>
		/// <param name="scale"></param>
		/// <param name="start"></param>
		/// <param name="length"></param>
		static public void Scale( ComplexF[] array, ComplexF scale, int start, int length ) {
			Debug.Assert( array != null );
			Debug.Assert( start >= 0 );
			Debug.Assert( length >= 0 );
			Debug.Assert( ( start + length ) < array.Length );

			for( int i = 0; i < length; i ++ ) {
				array[i + start] *= scale;
			}
		}

		//---------------------------------------------------------------------------------------------

		/// <summary>
		/// Multiply each element in target array with corresponding element in rhs array
		/// </summary>
		/// <param name="target"></param>
		/// <param name="rhs"></param>
		static public void Multiply( ComplexD[] target, ComplexD[] rhs ) {
			ComplexArray.Multiply( target, rhs, target );
		}
		/// <summary>
		/// Multiply each element in lhs array with corresponding element in rhs array and
		/// put product in result array
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <param name="result"></param>
		static public void Multiply( ComplexD[] lhs, ComplexD[] rhs, ComplexD[] result ) {
			Debug.Assert( lhs != null );
			Debug.Assert( rhs != null );
			Debug.Assert( result != null );
			Debug.Assert( lhs.Length == rhs.Length );
			Debug.Assert( lhs.Length == result.Length );

			int length = lhs.Length;
			for( int i = 0; i < length; i ++ ) {
				result[i] = lhs[i] * rhs[i];
			}
		}

		/// <summary>
		/// Multiply each element in target array with corresponding element in rhs array
		/// </summary>
		/// <param name="target"></param>
		/// <param name="rhs"></param>
		static public void Multiply( ComplexF[] target, ComplexF[] rhs ) {
			ComplexArray.Multiply( target, rhs, target );
		}
		/// <summary>
		/// Multiply each element in lhs array with corresponding element in rhs array and
		/// put product in result array
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <param name="result"></param>
		static public void Multiply( ComplexF[] lhs, ComplexF[] rhs, ComplexF[] result ) {
			Debug.Assert( lhs != null );
			Debug.Assert( rhs != null );
			Debug.Assert( result != null );
			Debug.Assert( lhs.Length == rhs.Length );
			Debug.Assert( lhs.Length == result.Length );

			int length = lhs.Length;
			for( int i = 0; i < length; i ++ ) {
				result[i] = lhs[i] * rhs[i];
			}
		}

		//---------------------------------------------------------------------------------------------

		/// <summary>
		/// Divide each element in target array with corresponding element in rhs array
		/// </summary>
		/// <param name="target"></param>
		/// <param name="rhs"></param>
		static public void Divide( ComplexD[] target, ComplexD[] rhs ) {
			ComplexArray.Divide( target, rhs, target );
		}
		/// <summary>
		/// Divide each element in lhs array with corresponding element in rhs array and
		/// put product in result array
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <param name="result"></param>
		static public void Divide( ComplexD[] lhs, ComplexD[] rhs, ComplexD[] result ) {
			Debug.Assert( lhs != null );
			Debug.Assert( rhs != null );
			Debug.Assert( result != null );
			Debug.Assert( lhs.Length == rhs.Length );
			Debug.Assert( lhs.Length == result.Length );

			int length = lhs.Length;
			for( int i = 0; i < length; i ++ ) {
				result[i] = lhs[i] / rhs[i];
			}
		}

		/// <summary>
		/// Divide each element in target array with corresponding element in rhs array
		/// </summary>
		/// <param name="target"></param>
		/// <param name="rhs"></param>
		static public void Divide( ComplexF[] target, ComplexF[] rhs ) {
			ComplexArray.Divide( target, rhs, target );
		}
		/// <summary>
		/// Divide each element in lhs array with corresponding element in rhs array and
		/// put product in result array
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <param name="result"></param>
		static public void Divide( ComplexF[] lhs, ComplexF[] rhs, ComplexF[] result ) {
			Debug.Assert( lhs != null );
			Debug.Assert( rhs != null );
			Debug.Assert( result != null );
			Debug.Assert( lhs.Length == rhs.Length );
			Debug.Assert( lhs.Length == result.Length );

			ComplexF zero = ComplexF.Zero;
			int length = lhs.Length;
			for( int i = 0; i < length; i ++ ) {
				if( rhs[i] != zero ) {
					result[i] = lhs[i] / rhs[i];
				}
				else {
					result[i] = zero;
				}
			}
		}

		//---------------------------------------------------------------------------------------------

		/*static public void Flip( ComplexF[] array, Size3 size ) {
			Debug.Assert( array != null );

			ComplexF[]	workspace	= null;
			ComplexArray.LockWorkspaceF( size.GetTotalLength(), ref workspace );
			
			for( int z = 0; z < size.Depth; z ++ ) {
				for( int y = 0; y < size.Height; y ++ ) {
					int xyzOffset = 0 + y * size.Width + z * size.Width * size.Height;
					int abcOffset = size.Width - 1 + ( size.Height - y - 1 ) * size.Width + ( size.Depth - z - 1 ) * size.Width * size.Height;
					for( int x = 0; x < size.Width; x ++ ) {
						workspace[ xyzOffset ++ ] = array[ abcOffset -- ];
					}
				}
			}

			for( int i = 0; i < size.GetTotalLength(); i ++ ) {
				array[ i ] = workspace[ i ];
			}

			ComplexArray.UnlockWorkspaceF( ref workspace );
		}  */
		

		/// <summary>
		/// Copy an array
		/// </summary>
		/// <param name="dest"></param>
		/// <param name="source"></param>
		static public void Copy( ComplexD[] dest, ComplexD[] source ) {
			Debug.Assert( dest != null );
			Debug.Assert( source != null );
			Debug.Assert( dest.Length == source.Length );
			for( int i = 0; i < dest.Length; i ++ ) {
				dest[i] = source[i];
			}
		}

		/// <summary>
		/// Copy an array
		/// </summary>
		/// <param name="dest"></param>
		/// <param name="source"></param>
		static public void Copy( ComplexF[] dest, ComplexF[] source ) {
			Debug.Assert( dest != null );
			Debug.Assert( source != null );
			Debug.Assert( dest.Length == source.Length );
			for( int i = 0; i < dest.Length; i ++ ) {
				dest[i] = source[i];
			}
		}

		/// <summary>
		/// Reverse the elements in the array
		/// </summary>
		/// <param name="array"></param>
		static public void Reverse( ComplexD[] array ) {
			ComplexD temp;
			int length = array.Length;
			for( int i = 0; i < length/2; i ++ ) {
				temp = array[i];
				array[i] = array[length-1-i];
				array[length-1-i] = temp;
			}
		}

		/// <summary>
		/// Scale and offset the elements in the array so that the
		/// overall range is [0, 1]
		/// </summary>
		/// <param name="array"></param>
		static public void Normalize( ComplexD[] array ) {
			double min = 0, max = 0;
			GetLengthRange( array, ref min, ref max );
			Scale( array, ( 1 / ( max - min ) ) );
			Offset( array, ( - min / ( max - min ) ) );
		}

		/// <summary>
		/// Scale and offset the elements in the array so that the
		/// overall range is [0, 1]
		/// </summary>
		/// <param name="array"></param>
		static public void Normalize( ComplexF[] array ) {
			float min = 0, max = 0;
			GetLengthRange( array, ref min, ref max );
			Scale( array, ( 1 / ( max - min ) ) );
			Offset( array, ( - min / ( max - min ) ) );
		}

		/// <summary>
		/// Invert each element in the array
		/// </summary>
		/// <param name="array"></param>
		static public void Invert( ComplexD[] array ) {
			for( int i = 0; i < array.Length; i ++ ) {
				array[i] = ((ComplexD) 1 ) / array[i];
			}
		}

		/// <summary>
		/// Invert each element in the array
		/// </summary>
		/// <param name="array"></param>
		static public void Invert( ComplexF[] array ) {
			for( int i = 0; i < array.Length; i ++ ) {
				array[i] = ((ComplexF) 1 ) / array[i];
			}
		}

		/// <summary>
		/// Creates a complex array from an array of doubles
		/// </summary>
		/// <param name="RealArray">Real parts, the imaginary parts presumed zero</param>
		/// <returns></returns>
		static public ComplexD[] CreateFromReal( double[] RealArray)
		{
			Debug.Assert( RealArray != null );
			Debug.Assert( RealArray.Length >= 0 );
			ComplexD[] tmp = new ComplexD[ RealArray.Length];
			for( int i=0; i<tmp.Length; i++)
			{
				tmp[i].Re = (double)RealArray[i];
			}
			return tmp;
		}

		/// <summary>
		/// Creates a ComplexF array from an array of floats
		/// </summary>
		/// <param name="RealArray">Real parts, the imaginary parts presumed zero</param>
		/// <returns></returns>
		static public ComplexF[] CreateFromReal( float[] RealArray)
		{
			Debug.Assert( RealArray != null );
			Debug.Assert( RealArray.Length >= 0 );
			ComplexF[] tmp = new ComplexF[ RealArray.Length];
			for( int i=0; i<tmp.Length; i++)
			{
				tmp[i].Re = (float)RealArray[i];
			}
			return tmp;
		}

		/// <summary>
		/// Creates a double's array from complex array
		/// </summary>
		/// <param name="ComplexArray">ComplexD array, the imaginary parts are dropped</param>
		/// <returns></returns>
		static public double[] CreateFromComplex( ComplexD[] ComplexArray)
		{
			Debug.Assert( ComplexArray != null );
			Debug.Assert( ComplexArray.Length >= 0 );
			double[] tmp = new double[ ComplexArray.Length];
			for( int i=0; i<tmp.Length; i++)
			{
				tmp[i] = (double)ComplexArray[i].Re;
			}
			return tmp;
		}

		/// <summary>
		/// Creates a float's array from the ComplexF array
		/// </summary>
		/// <param name="ComplexArray">ComplexD array, the imaginary parts are dropped</param>
		/// <returns></returns>
		static public float[] CreateFromComplex( ComplexF[] ComplexArray)
		{
			Debug.Assert( ComplexArray != null );
			Debug.Assert( ComplexArray.Length >= 0 );
			float[] tmp = new float[ ComplexArray.Length];
			for( int i=0; i<tmp.Length; i++)
			{
				tmp[i] = (float)ComplexArray[i].Re;
			}
			return tmp;
		}

		/// <summary>
		/// Copies from complex array to the double array
		/// </summary>
		/// <param name="source">Compex array to copy from, the imaginary part is dropped</param>
		/// <param name="destination">double[] array to copy into</param>
		/// <returns></returns>
		static public void CopyFromComplex( ComplexD[] source, double[] destination)
		{
			Debug.Assert( source != null );
			Debug.Assert( source.Length >= 0 );
			Debug.Assert( destination != null );
			Debug.Assert( destination.Length >= source.Length );
			for( int i=0; i<destination.Length; i++)
			{
				destination[i] = (double)source[i].Re;
			}
		}

		/// <summary>
		/// Copies from complex array to the double array
		/// </summary>
		/// <param name="source">CompexF array to copy from, the imaginary part is dropped</param>
		/// <param name="destination">float[] array to copy into</param>
		/// <returns></returns>
		static public void CopyFromComplex( ComplexF[] source, float[] destination)
		{
			Debug.Assert( source != null );
			Debug.Assert( source.Length >= 0 );
			Debug.Assert( destination != null );
			Debug.Assert( destination.Length >= source.Length );
			for( int i=0; i<destination.Length; i++)
			{
				destination[i] = (float)source[i].Re;
			}
		}

		//----------------------------------------------------------------------------------------

	}
}
