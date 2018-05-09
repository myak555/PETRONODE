using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Petronode.Mathematics
{
    public class Matrix
    {
        protected int m_NumColumns = 1;
        protected int m_NumRows = 1;
        protected double[][] m_Data = null;

        #region Constructors
        /// <summary>
        /// Constructor, creates Matrix 1x1
        /// </summary>
        public Matrix()
        {
            m_Data = AllocateMemory(1, 1);
        }

        /// <summary>
        /// Constructor, creates matrix from other matrix
        /// </summary>
        /// <param name="other">other matrix</param>
        public Matrix(Matrix other)
        {
            m_Data = AllocateMemory(other.ColumnsCount, other.RowsCount);
            Copy(other, this);
        }

        /// <summary>
        /// Constructor, creates Matrix nCols x nRows
        /// </summary>
        public Matrix(int nCols, int nRows)
        {
            m_Data = AllocateMemory( nCols, nRows);
        }

        /// <summary>
        /// Constructor, creates Matrix size x size
        /// </summary>
        public Matrix(int size)
        {
            m_Data = AllocateMemory(size, size);
        }

        /// <summary>
        /// Constructor, creates Matrix size x size
        /// </summary>
        public Matrix(int size, double diagonal)
        {
            m_Data = AllocateMemory(size, size);
            for (int i = 0; i < size; i++)
            {
                m_Data[i][i] = diagonal;
            }
        }

        /// <summary>
        /// Constructor, creates Matrix size x 1
        /// </summary>
        public Matrix(double[] data, bool horizontal)
        {
            if( horizontal)
            {
                m_Data = AllocateMemory(1, data.Length);
                for (int i = 0; i < data.Length; i++)
                {
                    m_Data[0][i] = data[i];
                }
            }
            else
            {
                m_Data = AllocateMemory(data.Length, 1);
                for (int i = 0; i < data.Length; i++)
                {
                    m_Data[i][0] = data[i];
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Allocates memory for matrix nCols x nRows
        /// </summary>
        /// <param name="nCols"></param>
        /// <param name="nRows"></param>
        /// <returns></returns>
        private double[][] AllocateMemory( int nCols, int nRows)
        {
            m_NumColumns = nCols;
            m_NumRows = nRows;
            double[][] tmp = new double[nCols][];
            for( int i=0; i<tmp.Length; i++)
            {
                tmp[i] = new double[nRows];
            }
            return tmp;
        }

        /// <summary>
        /// Copy matrices of the same size
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        private static void Copy(Matrix source, Matrix destination)
        {
            for (int i = 0; i < source.ColumnsCount; i++)
            {
                for (int j = 0; j < source.RowsCount; j++)
                {
                    destination.m_Data[i][j] = source.m_Data[i][j];
                }
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Retrieves the number of columns
        /// </summary>
        public int	ColumnsCount
        {
            get{ return m_NumColumns; }
        }
        
        /// <summary>matyrix
        /// Retrieves the number of rows
        /// </summary>
        public int	RowsCount
        {
            get{ return m_NumRows; }
        }

        /// <summary>
        /// Retrieves minimum
        /// </summary>
        public double Minimum
        {
            get
            {
                double minimum = Double.MaxValue;
	            for (int i = 0; i < m_NumColumns; i++)
		        {
		            for (int j = 0; j < m_NumRows; j++)
                    {
			            double v = m_Data[i][j];
                        if( v < minimum) minimum = v;
                    }
		        }
                return minimum;
            }
        }

        /// <summary>
        /// Retrieves maximum
        /// </summary>
        public double Maximum
        {
            get
            {
                double maximum = Double.MinValue;
	            for (int i = 0; i < m_NumColumns; i++)
		        {
		            for (int j = 0; j < m_NumRows; j++)
                    {
			            double v = m_Data[i][j];
                        if( v > maximum) maximum = v;
                    }
		        }
                return maximum;
            }
        }

        #endregion

        #region Operator Overloads
        /// <summary>
        /// This operator doesn't do much. :-)
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Matrix operator +(Matrix a)
        {
            Matrix result = new Matrix(a);
            return result;
        }

        /// <summary>
        /// Negate the matrix
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Matrix operator -(Matrix a)
        {
            Matrix result = new Matrix(a.ColumnsCount, a.RowsCount);
            for (int i = 0; i < result.m_NumColumns; i++)
            {
                for (int j = 0; j < result.m_NumRows; j++)
                {
                    result.SetElement(i, j, -a.GetElement(i, j));
                }
            }
            return result;
        }

        /// <summary>
        /// Add matrices
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (a.RowsCount != b.RowsCount || a.ColumnsCount != b.ColumnsCount)
                throw new Exception( "Matrices do not have the same size");
            Matrix result = new Matrix(a.ColumnsCount, a.RowsCount);
            for (int i = 0; i < result.m_NumColumns; i++)
            {
                for (int j = 0; j < result.m_NumRows; j++)
                {
                    result.SetElement(i, j, a.GetElement(i, j) + b.GetElement(i,j));
                }
            }
            return result;
        }

        /// <summary>
        /// Subtract two matrices
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix operator -(Matrix a, Matrix b)
        {
            if (a.RowsCount != b.RowsCount || a.ColumnsCount != b.ColumnsCount)
                throw new Exception("Matrices do not have the same size");
            Matrix result = new Matrix(a.ColumnsCount, a.RowsCount);
            for (int i = 0; i < result.m_NumColumns; i++)
            {
                for (int j = 0; j < result.m_NumRows; j++)
                {
                    result.SetElement(i, j, a.GetElement(i, j) - b.GetElement(i, j));
                }
            }
            return result;
        }

        /// <summary>
        /// Multiply two matrices
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.ColumnsCount != b.RowsCount)
                throw new Exception("Size mismatch: matrix a with " + a.ColumnsCount.ToString() +
                    " columns and matrix b with " + a.RowsCount.ToString() + " rows." );
            Matrix result = new Matrix(b.ColumnsCount, a.RowsCount);

	        // e.g.
	        // [A][B][C]   [G][H]     [A*G + B*I + C*K][A*H + B*J + C*L]
	        // [D][E][F] * [I][J] =   [D*G + E*I + F*K][D*H + E*J + F*L]
	        //             [K][L]
	        //
	        for (int i = 0; i < a.RowsCount; i++)
		    {
		        for (int j = 0; j < b.ColumnsCount; j++)
			    {
                    double value = 0.0;
                    for (int k = 0; k < b.RowsCount; k++)
                    {
                        value += a.GetElement(k, i) * b.GetElement(j, k);
                    }
                    result.SetElement(j, i, value);
                }
		    }
	        return result ;
        }

        /// <summary>
        /// Multiply matrix by double
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix operator *(Matrix a, double b)
        {
            Matrix result = new Matrix(a);
            for (int i = 0; i < result.m_NumColumns; i++)
            {
                for (int j = 0; j < result.m_NumRows; j++)
                {
                    result.m_Data[i][j] *= b;
                }
            }
            return result;
        }

        /// <summary>
        /// Multiply double by matrix
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix operator *(double a, Matrix b)
        {
            Matrix result = new Matrix(b);
            for (int i = 0; i < result.m_NumColumns; i++)
            {
                for (int j = 0; j < result.m_NumRows; j++)
                {
                    result.m_Data[i][j] *= a;
                }
            }
            return result;
        }

        /// <summary>
        /// compare two matrices
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Matrix a, Matrix b)
        {
            if (a.RowsCount != b.RowsCount || a.ColumnsCount != b.ColumnsCount)
                throw new Exception("Matrices do not have the same size");
            for (int i = 0; i < a.m_NumColumns; i++)
            {
                for (int j = 0; j < a.m_NumRows; j++)
                {
                    if (a.GetElement(i, j) != b.GetElement(i, j)) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// compare two matrices
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Matrix a, Matrix b)
        {
            bool result = a==b;
            return !result;
        }

        /// <summary>
        /// Defines an indexer
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double[] this[int index]
        {
            get { return m_Data[index]; }
        }

        /// <summary>
        /// Overwrite of Equals
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override bool Equals( object o)
        {
            Matrix m = (Matrix)o;
            return this == m;
        }

        /// <summary>
        /// Overwrite of GetHashCode
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override int GetHashCode()
        {
            double result = 0.0;
            for (int i = 0; i < m_NumColumns; i++)
            {
                for (int j = 0; j < m_NumRows; j++)
                {
                    result += m_Data[i][j];
                }
            }
            if (result > Convert.ToDouble(Int32.MaxValue)) return Int32.MaxValue;
            if (result < Convert.ToDouble(Int32.MinValue)) return Int32.MinValue;
            return Convert.ToInt32(result);
        }
        #endregion

        #region Matrix Operations
        /// <summary>
        /// Sets the element to value
        /// </summary>
        /// <param name="nCol">column position</param>
        /// <param name="nRow">row position</param>
        /// <param name="value">value</param>
        public void SetElement(int nCol, int nRow, double value)
        {
            m_Data[nCol][nRow] = value;
        }

        /// <summary>
        /// Gets the element
        /// </summary>
        /// <param name="nCol">column position</param>
        /// <param name="nRow">row position</param>
        public double GetElement(int nCol, int nRow)
        {
            return m_Data[nCol][nRow];
        }

        /// <summary>
        /// sets a column
        /// </summary>
        /// <param name="nCol"></param>
        /// <param name="Data"></param>
        public void SetColumn(int nCol, double[] Data)
        {
            for (int i = 0; i < m_NumRows; i++)
            {
                m_Data[nCol][i] = Data[i];
            }
        }

        /// <summary>
        /// Retrieves a column
        /// </summary>
        /// <param name="nCol"></param>
        /// <returns></returns>
        public double[] GetColumn(int nCol)
        {
            int Rows = RowsCount;
            double[] tmp = new double[m_NumRows];
            for (int i = 0; i < m_NumRows; i++)
            {
                tmp[i] = m_Data[nCol][i];
            }
            return tmp;
        }

        /// <summary>
        /// sets a row
        /// </summary>
        /// <param name="nRow"></param>
        /// <param name="Data"></param>
        public void SetRow(int nRow, double[] Data)
        {
            for (int i = 0; i < m_NumColumns; i++)
            {
                m_Data[i][nRow] = Data[i];
            }
        }

        /// <summary>
        /// Retrieves a row
        /// </summary>
        /// <param name="nRow"></param>
        /// <returns></returns>
        public double[] GetRow(int nRow)
        {
            double[] tmp = new double[m_NumColumns];
            for (int i = 0; i < m_NumColumns; i++)
            {
                tmp[i] = m_Data[i][nRow];
            }
            return tmp;
        }

        /// <summary>
        /// Returns a transposed matrix
        /// </summary>
        /// <returns></returns>
        public Matrix GetTransposed()
        {
            Matrix m = new Matrix(this);
            m.Transpose();
            return m;
        }

        /// <summary>
        /// Makes the matrix transposed
        /// </summary>
        public void Transpose()
        {
            int nC = m_NumColumns;
            int nR = m_NumRows;
            double[][] old_Data = m_Data;
            m_Data = AllocateMemory(nR, nC);
            for (int i = 0; i < m_NumColumns; i++)
            {
                for (int j = 0; j < m_NumRows; j++)
                {
                    m_Data[i][j] = old_Data[j][i];
                }
            }
        }

        /// <summary>
        /// Returns an inverted matrix
        /// </summary>
        /// <returns></returns>
        public Matrix GetInverted()
        {
            Matrix m = new Matrix(this);
            m.Invert();
            return m;
        }

        /// <summary>
        /// Makes the matrix inverted - only works on square matrices
        /// </summary>
        public void Invert()
        {
            // matrix inversion will only work on square matrices
            if (m_NumColumns != m_NumRows) throw new Exception("Matrix not square");

            for (int k = 0; k < m_NumColumns; k++)
            {
                double e = m_Data[k][k];
                m_Data[k][k] = 1.0;
                if (e == 0.0) throw new Exception("Matrix inversion error");
                e = 1.0 / e;
                for (int j = 0; j < m_NumColumns; j++) m_Data[k][j] *= e;
                for (int i = 0; i < m_NumColumns; i++)
                {
                    if (i == k) continue;
                    e = m_Data[i][k];
                    m_Data[i][k] = 0.0;
                    for (int j = 0; j < m_NumColumns; j++)
                        m_Data[i][j] = m_Data[i][j] - e * m_Data[k][j];
                }
            }
        }

        /// <summary>
        /// Returns a covariant
        /// </summary>
        /// <returns></returns>
        public Matrix GetCovariant()
        {
            Matrix m = new Matrix(this);
            m.Covariant();
            return m;
        }

        /// <summary>
        /// Makes the matrix a covariant of itself
        /// </summary>
        public void Covariant()
        {
	        Matrix	transposed = new Matrix(this);
            transposed.Transpose();
            Matrix result = this * transposed;
            m_Data = result.m_Data;
            m_NumColumns = result.m_NumColumns;
            m_NumRows = result.m_NumRows;
        }

        /// <summary>
        /// Extracts a sub-matrix
        /// </summary>
        /// <param name="col_start"></param>
        /// <param name="row_start"></param>
        /// <param name="col_size"></param>
        /// <param name="row_size"></param>
        /// <returns></returns>
        public Matrix ExtractSubMatrix(int col_start, int row_start, int col_size, int row_size)
        {
	        // make sure the requested sub matrix is in the current matrix
	        if (col_start + col_size > m_NumColumns)
		        throw new Exception( "Sub matrix is not contained in source");
	        if (row_start + row_size > m_NumRows)
		        throw new Exception( "Sub matrix is not contained in source");
            Matrix sub = new Matrix(col_size, row_size) ;
	        for (int i = 0; i < col_size; i++)
		    {
		        for (int j = 0; j < row_size; j++)
			    {
			        sub.SetElement(i, j, GetElement(col_start + i, row_start + j));
			    }
		    }
	        return sub ;
        }

        /// <summary>
        /// Sets sub-matrix
        /// </summary>
        /// <param name="col_start"></param>
        /// <param name="row_start"></param>
        /// <param name="other"></param>
        public void SetSubMatrix(int col_start, int row_start, Matrix other)
        {
	        // make sure the requested sub matrix is in the current matrix
	        if (col_start + other.m_NumColumns > m_NumColumns)
		        throw new Exception( "Sub matrix is not contained in source");
	        if (row_start + other.m_NumRows > m_NumRows)
		        throw new Exception( "Sub matrix is not contained in source");
	        for (int i = 0; i < other.m_NumColumns; i++)
	        {
	            for (int j = 0; j < other.m_NumRows; j++)
	            {
	               SetElement(col_start + i, row_start + j, other.GetElement(i, j));
	            }
	        }
        }

        /// <summary>
        /// Sets sub-matrix
        /// </summary>
        /// <param name="col_start"></param>
        /// <param name="row_start"></param>
        /// <param name="other"></param>
        public void SetSubMatrix(Matrix other)
        {
            SetSubMatrix( 0, 0, other);
        }

        /// <summary>
        /// Extracts a diagonal
        /// </summary>
        /// <returns></returns>
        public Matrix ExtractDiagonal()
        {
	        if (m_NumColumns > m_NumRows)
		        throw new Exception( "Can only extract diagonal from square matrix");
	        Matrix	diagonal = new Matrix(m_NumColumns, 1);
	        for (int i = 0; i < m_NumColumns; i++)
		        diagonal.SetElement(i, 0, GetElement(i, i));
	        return diagonal;
        }

        /// <summary>
        /// Returns concatenated columns
        /// </summary>
        /// <returns></returns>
        public Matrix GetConcatenateColumns( Matrix other)
        {
            Matrix m = new Matrix(this);
            m.ConcatenateColumns( other);
            return m;
        }

        /// <summary>
        /// Concatenates columns
        /// </summary>
        public void ConcatenateColumns( Matrix other)
        {
          	if (m_NumRows != other.m_NumRows)
		        throw new Exception("Cannot concatenate matrices, not same size");
            Matrix old = new Matrix(this);
            m_Data = AllocateMemory( m_NumColumns + other.m_NumColumns, m_NumRows);

	        // now populate it
            this.SetSubMatrix(0, 0, old);
            this.SetSubMatrix(old.m_NumColumns, 0, other);
        }

        /// <summary>
        /// Returns concatenated rows
        /// </summary>
        /// <returns></returns>
        public Matrix GetConcatenateRows( Matrix other)
        {
            Matrix m = new Matrix(this);
            m.ConcatenateRows( other);
            return m;
        }

        /// <summary>
        /// Concatenates rows
        /// </summary>
        public void ConcatenateRows( Matrix other)
        {
          	if (m_NumColumns != other.m_NumColumns)
		        throw new Exception("Cannot concatenate matrices, not same size");
            Matrix old = new Matrix(this);
            m_Data = AllocateMemory( m_NumColumns, m_NumRows + other.m_NumRows);

	        // now populate it
            this.SetSubMatrix(0, 0, old);
            this.SetSubMatrix(0, old.m_NumRows, other);
        }

        /// <summary>
        /// Returns concatenated rows
        /// </summary>
        /// <returns></returns>
        public void AddColumn( double[] data)
        {
            Matrix m = new Matrix(data, false);
            ConcatenateColumns( m);
        }

                /// <summary>
        /// Returns concatenated rows
        /// </summary>
        /// <returns></returns>
        public void AddRow( double[] data)
        {
            Matrix m = new Matrix(data, true);
            ConcatenateRows( m);
        }

        /// <summary>
        /// Returns Squared martrix
        /// </summary>
        /// <returns></returns>
        public Matrix GetSquare()
        {
            Matrix m = new Matrix(this);
            m.Square();
            return m;
        }

        /// <summary>
        /// Squares matrix
        /// </summary>
        public void Square()
        {
            Matrix old = new Matrix(this);
           	int size = m_NumColumns ;
	        if (size > m_NumRows) size = m_NumRows ;
            m_Data = AllocateMemory( size, size);
	        double x_step = Convert.ToDouble( m_NumColumns) / size;
	        double y_step = Convert.ToDouble( m_NumRows) / size ;
	        for (int i = 0; i < size; i++)
		    {
		        for (int j = 0; j < size; j++)
			        m_Data[i][j] = GetElement(Convert.ToInt32(i * x_step), Convert.ToInt32(j * y_step));
		    }
        }

        /// <summary>
        /// Returns normalised martrix
        /// </summary>
        /// <returns></returns>
        public Matrix GetNormalised( double min, double max)
        {
            Matrix m = new Matrix(this);
            m.Normalise( min, max);
            return m;
        }

        /// <summary>
        /// Normalises matrix
        /// </summary>
        public void Normalise( double min, double max)
        {
            double e_min = Minimum;
            double scale = (max - min) / (Maximum - e_min);
	        for (int i = 0; i < m_NumColumns; i++)
		    {
		        for (int j = 0; j < m_NumRows; j++)
			    {
			        m_Data[i][j] = (m_Data[i][j] - e_min) * scale + min;
			    }
		    }
        }

        /// <summary>
        /// Returns sum of a column
        /// </summary>
        public double SumColumn( int column)
        {
	        double	sum = 0.0;
            for (int i = 0; i < m_NumRows; i++)
		        sum += m_Data[column][i] ;
	        return sum;
        }

        /// <summary>
        /// Returns sum of a row
        /// </summary>
        public double SumRow( int row)
        {
	        double	sum = 0.0;
            for (int i = 0; i < m_NumColumns; i++)
		        sum += m_Data[i][row];
	        return sum;
        }

        /// <summary>
        /// Returns sumsquare of a column
        /// </summary>
        public double SumColumnSquared( int column)
        {
	        double	sum = 0.0;
            for (int i = 0; i < m_NumRows; i++)
            {
		        double v = m_Data[column][i];
                sum += v*v;
            }
	        return sum;
        }

        /// <summary>
        /// Returns sumsquare of a row
        /// </summary>
        public double SumRowSquared( int row)
        {
	        double	sum = 0.0;
            for (int i = 0; i < m_NumColumns; i++)
            {
		        double v = m_Data[i][row];
                sum += v*v;
            }
	        return sum;
        }

        /// <summary>
        /// Returns min of a column
        /// </summary>
        public double GetColumnMinimum( int column)
        {
	        Matrix m = ExtractSubMatrix(column, 0, m_NumColumns, 1);
            return m.Minimum;
        }

        /// <summary>
        /// Returns max of a column
        /// </summary>
        public double GetColumnMaximum( int column)
        {
	        Matrix m = ExtractSubMatrix(column, 0, m_NumColumns, 1);
            return m.Maximum;
        }

        /// <summary>
        /// Returns min of a row
        /// </summary>
        public double GetRowMinimum( int row)
        {
	        Matrix m = ExtractSubMatrix(0, row, 1, m_NumRows);
            return m.Minimum;
        }

        /// <summary>
        /// Returns max of a row
        /// </summary>
        public double GetRowMaximum( int row)
        {
	        Matrix m = ExtractSubMatrix(0, row, 1, m_NumRows);
            return m.Maximum;
        }
        #endregion

        public void	 WriteAsCSVFile(string filename)
        {
            StreamWriter sw = File.CreateText(filename);
            for (int i = 0; i < m_NumColumns; i++)
            {
                StringBuilder sb = new StringBuilder();
                for (int j = 0; j < m_NumRows; j++)
                {
                    sb.Append( m_Data[i][j].ToString());
                    if (j < m_NumRows - 1) sb.Append(",");
                }
                sw.WriteLine(sb.ToString());
            }
            sw.Close();
        }

        public void ReadFromCSVFile(string filename)
        {
            StreamReader sr = File.OpenText(filename);
            List<double[]> tmp = new List<double[]>();
            string s = sr.ReadLine();
            string[] ss = s.Split(',');
            int len = ss.Length;
            double[] d = new double[len];
            tmp.Add( d);
            while( true)
            {
                s = sr.ReadLine();
                if( s==null) break;
                ss = s.Split(',');
                if( ss.Length <= 0) break;
                d = new double[len]; // zeros by default
                for (int i = 0; i < ss.Length; i++)
                {
                    if( i>=len) break;
                    d[i] = Convert.ToDouble( ss[i]);
                }
                tmp.Add( d);
            }
            sr.Close();
            m_Data = new double[tmp.Count][];
            for (int i = 0; i < m_Data.Length; i++)
            {
                m_Data[i] = tmp[i];
            }
            m_NumColumns = m_Data.Length;
            m_NumRows = len;
        }
    }
}
