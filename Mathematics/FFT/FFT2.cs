using System;
using System.Collections.Generic;
using System.Text;
using Petronode.Mathematics.Complex;

namespace Petronode.Mathematics.FFT
{
    public class FFT2: IDisposable
    {
        public int nx, ny;                  //Number of Points in Width & height
        public int Width, Height;
        public ComplexD[,] FourierData;     //FourierData Magnitude  Array Used for Inverse FFT
        public ComplexD[,] OutputData;      //FFT Normal

        #region Constructors
        /// <summary>
        /// Creates the 2D FFT
        /// </summary>
        /// <param name="width">array width</param>
        /// <param name="height">array height</param>
        public FFT2( int width, int height)
        {
            Width = width;
            Height = height;
            nx = NearPower2(width);
            ny = NearPower2(height);
            FourierData = new ComplexD[nx<<1, ny<<1];
            OutputData = new ComplexD[nx<<1, ny<<1];
        }

        /// <summary>
        /// Disposes the arrays
        /// </summary>
        public void Dispose()
        {
            nx = 0;
            ny = 0;
            Width = 0;
            Height = 0;
            FourierData = null;
            OutputData = null;
        }
        #endregion

        #region Get and Set Arrays
        /// <summary>
        /// Clears the array with zeros
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < (nx<<1); i++)
                for (int j = 0; j < (ny<<1); j++)
                {
                    FourierData[i, j].Re = 0.0;
                    FourierData[i, j].Im = 0.0;
                }
        }

        /// <summary>
        /// Sets the row for input
        /// </summary>
        /// <param name="row">row number</param>
        /// <param name="input">input array</param>
        public void SetArray(int row, float[] input)
        {
            // field repearts 4 times, with the centre at [nx, ny]
            for (int i = 0; i < input.Length; i++)
            {
                // top left
                FourierData[nx -1 - i, ny - 1 - row].Re = Convert.ToDouble(input[i]);
                FourierData[nx - 1 - i, ny - 1 - row].Im = 0.0;

                // top right
                FourierData[nx - 1 - i, ny + row].Re = -Convert.ToDouble(input[i]);
                FourierData[nx - 1 - i, ny + row].Im = 0.0;

                // bottom left
                FourierData[nx + i, ny - 1 - row].Re = -Convert.ToDouble(input[i]);
                FourierData[nx + i, ny - 1 - row].Im = 0.0;

                // bottom right
                FourierData[nx + i, ny + row].Re = Convert.ToDouble(input[i]);
                FourierData[nx + i, ny + row].Im = 0.0;
            }
            return;
        }

        /// <summary>
        /// Sets full array for input
        /// </summary>
        /// <param name="input">input 2D array</param>
        public void SetArray(float[,] input)
        {
            // field repearts 4 times, with the centre at [nx, ny]
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    FourierData[nx - 1 - i, ny - 1 - j].Re = Convert.ToDouble(input[i,j]);
                    FourierData[nx - 1 - i, ny - 1 - j].Im = 0.0;
                    FourierData[nx - 1 - i, ny + j].Re = -Convert.ToDouble(input[i,j]);
                    FourierData[nx - 1 - i, ny + j].Im = 0.0;
                    FourierData[nx + i, ny - 1 - j].Re = -Convert.ToDouble(input[i,j]);
                    FourierData[nx + i, ny - 1 - j].Im = 0.0;
                    FourierData[nx + i, ny + j].Re = Convert.ToDouble(input[i,j]);
                    FourierData[nx + i, ny + j].Im = 0.0;
                }
            }
        }

        /// <summary>
        /// Sets the row for input
        /// </summary>
        /// <param name="row">row number</param>
        /// <param name="amp">array of amplitudes</param>
        /// <param name="pha">array of phases</param>
        public void SetArray(int row, float[] amp, float[] pha)
        {
            for (int i = 0; i < amp.Length; i++)
            {
                double a = Convert.ToDouble(amp[i]);
                double p = Convert.ToDouble(pha[i]);
                double x = a*Math.Cos(p);
                double y = a*Math.Sin(p);
                FourierData[nx - 1 - i, ny - 1 - row].Re = x;
                FourierData[nx - 1 - i, ny - 1 - row].Im = y;
                FourierData[nx - 1 - i, ny + row].Re = x;
                FourierData[nx - 1 - i, ny + row].Im = y;
                FourierData[nx + i, ny - 1 - row].Re = x;
                FourierData[nx + i, ny - 1 - row].Im = y;
                FourierData[nx + i, ny + row].Re = x;
                FourierData[nx + i, ny + row].Im = y;
            }
        }

        /// <summary>
        /// Sets full array for input
        /// </summary>
        /// <param name="amp">array of amplitudes</param>
        /// <param name="pha">array of phases</param>
        public void SetArray(float[,] amp, float[,] pha)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    double a = Convert.ToDouble(amp[i,j]);
                    double p = Convert.ToDouble(pha[i,j]);
                    double x = a * Math.Cos(p);
                    double y = a * Math.Sin(p);
                    FourierData[i, j].Re = x;
                    FourierData[i, j].Im = y;
                    
                    FourierData[nx - 1 - i, ny - 1 - j].Re = x;
                    FourierData[nx - 1 - i, ny - 1 - j].Im = y;
                    FourierData[nx - 1 - i, ny + j].Re = x;
                    FourierData[nx - 1 - i, ny + j].Im = y;
                    FourierData[nx + i, ny - 1 - j].Re = x;
                    FourierData[nx + i, ny - 1 - j].Im = y;
                    FourierData[nx + i, ny + j].Re = x;
                    FourierData[nx + i, ny + j].Im = y;
                }
            }
        }

        /// <summary>
        /// Retrieves the array from row
        /// </summary>
        /// <param name="row">row number</param>
        /// <param name="output">array as floats</returns>
        public void GetArray(int row, ref float[] output)
        {
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = Convert.ToSingle( FourierData[nx+i, ny+row].Re);
            }
        }

        /// <summary>
        /// Retrieves the array from row
        /// </summary>
        /// <param name="row">row number</param>
        /// <param name="amp">amplitude</param>
        /// <param name="pha">phase</param>
        public void GetArray(int row, ref float[] amp, ref float[] pha)
        {
            for (int i = 0; i < amp.Length; i++)
            {
                //amp[i] = Convert.ToSingle(FourierData[i, row].Re);
                //pha[i] = Convert.ToSingle(FourierData[i, row].Im);
                amp[i] = Convert.ToSingle(FourierData[i, row].GetModulus());
                pha[i] = Convert.ToSingle(FourierData[i, row].GetArgument());
            }
        }
        #endregion
        
        /// <summary>
        /// Calculate Fast FourierData Transform
        /// </summary>
        public void ForwardFFT()
        {
            // Transform the rows
            int nx2 = nx << 1;
            int ny2 = ny << 1;
            double[] x = new double[nx2];
            double[] y = new double[nx2];
            for (int j = 0; j < ny2; j++)
            {
                for (int i = 0; i < nx2; i++)
                {
                    x[i] = FourierData[i, j].Re;
                    y[i] = FourierData[i, j].Im;
                }

                // Calling 1D FFT Function for Rows
                FFT1D(ref x, ref y, FourierDirection.Forward);

                for (int i = 0; i < nx2; i++)
                {
                    OutputData[i, j].Re = x[i];
                    OutputData[i, j].Im = y[i];
                }
            }

            // Transform the columns
            x = new double[ny2];
            y = new double[ny2];
            for (int j = 0; j < nx2; j++)
            {
                for (int i = 0; i < ny2; i++)
                {
                    x[i] = OutputData[j, i].Re;
                    y[i] = OutputData[j, i].Im;
                }

                // Calling 1D FFT Function for Rows
                FFT1D(ref x, ref y, FourierDirection.Forward);

                for (int i = 0; i < ny2; i++)
                {
                    FourierData[j, i].Re = x[i];
                    FourierData[j, i].Im = y[i];
                }
            }

            // flip the array
            for (int j = 0; j < ny; j++)
            {
                for (int i = 0; i < nx; i++)
                {
                    OutputData[i, j] = FourierData[nx+i, ny+j];
                    OutputData[nx+i, ny+j] = FourierData[i, j];
                }
            }
            for (int j = 0; j < ny2; j++)
            {
                for (int i = 0; i < nx2; i++)
                {
                    FourierData[i, j] = OutputData[i, j];
                }
            }
        }

        /// <summary>
        /// Calculate Inverse from ComplexD [,]  FourierData Array
        /// </summary>
        public void InverseFFT()
        {
            // Transform the rows
            int nx2 = nx << 1;
            int ny2 = ny << 1;
            double[] x = new double[nx2];
            double[] y = new double[nx2];
            for (int j = 0; j < ny2; j++)
            {
                for (int i = 0; i < nx2; i++)
                {
                    x[i] = FourierData[i, j].Re;
                    y[i] = FourierData[i, j].Im;
                }

                // Calling 1D FFT Function for Rows
                FFT1D(ref x, ref y, FourierDirection.Backward);

                for (int i = 0; i < nx2; i++)
                {
                    OutputData[i, j].Re = x[i];
                    OutputData[i, j].Im = y[i];
                }
            }

            // Transform the columns
            x = new double[ny2];
            y = new double[ny2];
            for (int j = 0; j < nx2; j++)
            {
                for (int i = 0; i < ny2; i++)
                {
                    x[i] = OutputData[j, i].Re;
                    y[i] = OutputData[j, i].Im;
                }

                // Calling 1D FFT Function for Rows
                FFT1D(ref x, ref y, FourierDirection.Backward);

                for (int i = 0; i < ny2; i++)
                {
                    FourierData[j, i].Re = x[i];
                    FourierData[j, i].Im = y[i];
                }
            }
        }

        /// <summary>
        /// Returns the closest power of 2 for any given n 
        /// </summary>
        /// <param name="n">input array length</param>
        /// <returns>output array length as 2^a</returns>
        private int NearPower2(int n)
        {
            int t = 1;
            while (t < n)
            {
                t = t << 1;
            }
            return t;
        }

        /// <summary>
        /// Seems to be stolen from the Numerical Recepies
        /// Perform a 2D FFT inplace given a complex 2D array
        /// The direction dir, 1 for forward, -1 for reverse
        /// The size of the array (nx,ny)
        /// </summary>
        /// <param name="dir">FourierData direction</param>
        private void FFT2D( FourierDirection dir)
        {
            ////// Transform the rows 
            //ComplexD[] input = new ComplexD[nx << 1];
            double[] x = new double[nx << 1];
            double[] y = new double[nx << 1];
            for (int j = 0; j < ny; j++)
            {
            //    input[0].Re = 0.0;
            //    input[0].Im = 0.0;
                x[0] = 0.0;
                y[0] = 0.0;
                for (int i = 1; i < nx; i++)
                {
                    //input[nx - i].Re = FourierData[i, j].Re;
                    x[nx - i] = FourierData[i, j].Re;
                    //input[nx - i].Im = -FourierData[i, j].Im;
                    y[nx - i] = -FourierData[i, j].Im;
                }
                for (int i = 0; i < nx; i++)
                {
                    //input[nx + i].Re = FourierData[i, j].Re;
                    x[nx + i] = FourierData[i, j].Re;
                    //input[nx + i].Im = FourierData[i, j].Im;
                    y[nx + i] = FourierData[i, j].Im;
                }

                // Calling 1D FFT Function for Rows
                //Fourier.FFT(input, input.Length, dir);
                FFT1D(ref x, ref y, dir);

            //    //double norm = (dir == FourierDirection.Forward) ? 1.0 : 1.0 / Convert.ToDouble(nx);
            //    double norm = 1.0;
                for (int i = 0; i < nx; i++)
                {
                    //FourierData[i, j].Re = input[nx+i].Re * norm;
                    //FourierData[i, j].Im = input[nx+i].Im * norm;
                    FourierData[i, j].Re = x[nx + i];
                    FourierData[i, j].Im = y[nx + i];
                    //OutputData[i, j].Re = real[i];
                    //OutputData[i, j].Im = imag[i];
                }
            }

            //// Transform the columns  
            //real = new double[ny];
            //imag = new double[ny];
            //for (int i=0; i<nx; i++) 
            //{
            //    for (int j=0; j<ny; j++) 
            //    {
            //       real[j] = OutputData[i, j].Re;
            //       imag[j] = OutputData[i, j].Im;
            //    }

            //    // Calling 1D FFT Function for Columns
            //    FFT1D(ref real, ref imag, dir);
            //    for (int j=0; j<ny; j++) 
            //    {
            //        FourierData[i, j].Re = real[j];
            //        FourierData[i, j].Im = imag[j];
            //    }
            //}
        }

        /// <summary>
        ///    Seems to be stolen from the Numerical Recepies
        ///    This computes an in-place complex-to-complex FFT
        ///    x and y are the real and imaginary arrays of 2^m points.
        ///    dir = 1 gives forward transform
        ///    dir = -1 gives reverse transform
        ///    Formula: forward
        ///             N-1
        ///              ---
        ///            1 \         - j k 2 pi n / N
        ///    X(K) = --- > x(n) e                  = Forward transform
        ///            N /                            n=0..N-1
        ///              ---
        ///             n=0
        ///    Formula: reverse
        ///             N-1
        ///             ---
        ///             \          j k 2 pi n / N
        ///    X(n) =    > x(k) e                  = Inverse transform
        ///             /                             k=0..N-1
        ///             ---
        ///             k=0
        /// </summary>
        /// <param name="x">real array</param>
        /// <param name="y">imaginary array</param>
        /// <param name="dir">(1) - forward, (-1) - reverse</param>
        private void FFT1D(ref double[] x, ref double[] y, FourierDirection dir)
        {
            int m = Convert.ToInt32( Math.Log((double)x.Length, 2.0));

            long nn, i, i1, j, k, i2, l, l1, l2;
            double c1, c2, tx, ty, t1, t2, u1, u2, z;
            /* Calculate the number of points */
            nn = 1;
            for (i = 0; i < m; i++)
                nn *= 2;
            /* Do the bit reversal */
            i2 = nn >> 1;
            j = 0;
            for (i = 0; i < nn - 1; i++)
            {
                if (i < j)
                {
                    tx = x[i];
                    ty = y[i];
                    x[i] = x[j];
                    y[i] = y[j];
                    x[j] = tx;
                    y[j] = ty;
                }
                k = i2;
                while (k <= j)
                {
                    j -= k;
                    k >>= 1;
                }
                j += k;
            }
            /* Compute the FFT */
            c1 = -1.0;
            c2 = 0.0;
            l2 = 1;
            for (l = 0; l < m; l++)
            {
                l1 = l2;
                l2 <<= 1;
                u1 = 1.0;
                u2 = 0.0;
                for (j = 0; j < l1; j++)
                {
                    for (i = j; i < nn; i += l2)
                    {
                        i1 = i + l1;
                        t1 = u1 * x[i1] - u2 * y[i1];
                        t2 = u1 * y[i1] + u2 * x[i1];
                        x[i1] = x[i] - t1;
                        y[i1] = y[i] - t2;
                        x[i] += t1;
                        y[i] += t2;
                    }
                    z = u1 * c1 - u2 * c2;
                    u2 = u1 * c2 + u2 * c1;
                    u1 = z;
                }
                c2 = Math.Sqrt((1.0 - c1) / 2.0);
                if (dir == FourierDirection.Forward)
                    c2 = -c2;
                c1 = Math.Sqrt((1.0 + c1) / 2.0);
            }
            /* Scaling for forward transform */
            if (dir == FourierDirection.Forward)
            {
                for (i = 0; i < nn; i++)
                {
                    x[i] /= (double)nn;
                    y[i] /= (double)nn;

                }
            }

           //  return(true) ;
            return;
        }

        public static void FFT1D_Test(int dir, int m, ref double[] x, ref double[] y )
            {
                long nn, i, i1, j, k, i2, l, l1, l2;
                double c1, c2, tx, ty, t1, t2, u1, u2, z;
                /* Calculate the number of points */
                nn = 1;
                for (i = 0; i < m; i++)
                    nn *= 2;
                /* Do the bit reversal */
                i2 = nn >> 1;
                j = 0;
                for (i = 0; i < nn - 1; i++)
                {
                    if (i < j)
                    {
                        tx = x[i];
                        ty = y[i];
                        x[i] = x[j];
                        y[i] = y[j];
                        x[j] = tx;
                        y[j] = ty;
                    }
                    k = i2;
                    while (k <= j)
                    {
                        j -= k;
                        k >>= 1;
                    }
                    j += k;
                }
                /* Compute the FFT */
                c1 = -1.0;
                c2 = 0.0;
                l2 = 1;
                for (l = 0; l < m; l++)
                {
                    l1 = l2;
                    l2 <<= 1;
                    u1 = 1.0;
                    u2 = 0.0;
                    for (j = 0; j < l1; j++)
                    {
                        for (i = j; i < nn; i += l2)
                        {
                            i1 = i + l1;
                            t1 = u1 * x[i1] - u2 * y[i1];
                            t2 = u1 * y[i1] + u2 * x[i1];
                            x[i1] = x[i] - t1;
                            y[i1] = y[i] - t2;
                            x[i] += t1;
                            y[i] += t2;
                        }
                        z = u1 * c1 - u2 * c2;
                        u2 = u1 * c2 + u2 * c1;
                        u1 = z;
                    }
                    c2 = Math.Sqrt((1.0 - c1) / 2.0);
                    if (dir == 1)
                        c2 = -c2;
                    c1 = Math.Sqrt((1.0 + c1) / 2.0);
                }
                /* Scaling for forward transform */
                if (dir == 1)
                {
                    for (i = 0; i < nn; i++)
                    {
                        x[i] /= (double)nn;
                        y[i] /= (double)nn;
                       
                    }
                }
                


              //  return(true) ;
                return;
            }
    }
}
