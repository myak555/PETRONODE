using System;
using System.Collections.Generic;
using System.Text;

namespace Petronode.CommonData
{
    /// <summary>
    /// Based on the 1965 research paper, “A Simplex Method for Function Minimization,”
    /// by J.A. Nelder and R. Mead
    /// </summary>
    public class Nelder_Mead_Optimization
    {
        static Random m_random = new Random(1);

        public int Dimension;                       // Problem dimension
        public Nelder_Mead_Solution[] Solutions;    // Potential solutions
        public double[] minX;                       // Solution constraints
        public double[] maxX;
        public double alpha = 1.0;                  // Reflection
        public double beta = 0.5;                   // Contraction
        public double gamma = 2.0;                  // Expansion
        public double Tolerance = 0.001;            // The main solving loop stops upon reaching this target
        public int maxLoop;                         // Limits the main solving loop

        public delegate double ObjectiveFunctionDelegate(double[] vector);
        public ObjectiveFunctionDelegate onObjectiveFunction = null;

        public delegate void ProgressReportDelegate(int percent, object info);
        public ProgressReportDelegate onReportProgress = null;

        /// <summary>
        /// Constructor, generates the class
        /// </summary>
        /// <param name="size">Number of Amoeba "legs"</param>
        /// <param name="minX">Constraints minimums</param>
        /// <param name="maxX">Constraints maximums</param>
        /// <param name="maxLoop">Maximum number of iterations to prevent locking</param>
        public Nelder_Mead_Optimization(int size,
            double[] minX, double[] maxX, int maxLoop)
        {
            Dimension = minX.Length;
            this.minX = new double[Dimension];
            this.maxX = new double[Dimension];
            Array.Copy(minX, this.minX, this.minX.Length);
            Array.Copy(maxX, this.maxX, this.maxX.Length);
            this.maxLoop = maxLoop;
            Solutions = new Nelder_Mead_Solution[size];
            for (int i = 0; i < Solutions.Length; ++i)
                Solutions[i] = NMS_Factory_Random();
            Array.Sort(Solutions);
        }

        /// <summary>
        /// Constructor, generates the class
        /// </summary>
        /// <param name="size">Number of Amoeba "legs"</param>
        /// <param name="initialX">First point given</param>
        /// <param name="minX">Constraints minimums</param>
        /// <param name="maxX">Constraints maximums</param>
        /// <param name="maxLoop">Maximum number of iterations to prevent locking</param>
        public Nelder_Mead_Optimization(int size,
            double[] initialX, double[] minX, double[] maxX, int maxLoop)
        {
            Dimension = minX.Length;
            this.minX = new double[Dimension];
            this.maxX = new double[Dimension];
            Array.Copy(minX, this.minX, this.minX.Length);
            Array.Copy(maxX, this.maxX, this.maxX.Length);
            this.maxLoop = maxLoop;
            Solutions = new Nelder_Mead_Solution[size];
            Solutions[0] = NMS_Factory_Deterministic( initialX);
            for (int i = 1; i < Solutions.Length; ++i)
                Solutions[i] = NMS_Factory_Random();
            Array.Sort(Solutions);
        }

        /// <summary>
        /// Performs solution initialization (either after the random or manual)
        /// </summary>
        public void InitSolutions()
        {
            if (onObjectiveFunction == null) return;
            foreach (Nelder_Mead_Solution nms in Solutions)
                nms.Value = onObjectiveFunction(nms.Vector);
            Array.Sort(Solutions);
        }

        /// <summary>
        /// Computes the target function by the current delegate
        /// If delegate is not set, result is 0
        /// </summary>
        public double ComputeTarget(double[] v)
        {
            if (onObjectiveFunction == null) return 0.0;
            return onObjectiveFunction(v);
        }

        /// <summary>
        /// Computes the target function by the current delegate
        /// If delegate is not set, result is 0. nms is not modified
        /// </summary>
        public double ComputeTarget(Nelder_Mead_Solution nms)
        {
            if (onObjectiveFunction == null) return 0.0;
            return onObjectiveFunction(nms.Vector);
        }

        /// <summary>
        /// Runs the solution
        /// </summary>
        /// <returns>the best solution found</returns>
        public Nelder_Mead_Solution LocateTarget()
        {
            for (int t = 0; t < maxLoop; t++)
            {
                if (BestSolution.Value < Tolerance) break; // solution in tolerance

                // progress report
                if ((t % 10 == 0) && onReportProgress != null)
                    onReportProgress(t * 100 / maxLoop, (object)BestSolution.ToString());
                
                Nelder_Mead_Solution centroid = GetCentroid();
                Nelder_Mead_Solution reflected = GetReflected(centroid);
                if (reflected.Value < BestSolution.Value)
                {
                    Nelder_Mead_Solution expanded = GetExpanded(reflected, centroid);
                    ReplaceTheWorstWith((expanded.Value < BestSolution.Value) ? expanded : reflected);
                    continue;
                }
                if (!IsWorseThanAllButTheWorst(reflected))
                {
                    ReplaceTheWorstWith(reflected);
                    continue;
                }
                if (reflected.Value <= WorstSolution.Value)
                    ReplaceTheWorstWith(reflected);
                Nelder_Mead_Solution contracted = GetContracted(centroid);
                if (contracted.Value > WorstSolution.Value)
                    ShrinkTowardsTheBestSolution();
                else
                    ReplaceTheWorstWith(contracted);
            }
            return BestSolution;
        }

        /// <summary>
        /// Retrieves the best solution
        /// </summary>
        public Nelder_Mead_Solution BestSolution
        {
            get { return Solutions[0]; }
        }

        /// <summary>
        /// Retrieves the worst solution
        /// </summary>
        public Nelder_Mead_Solution WorstSolution
        {
            get { return Solutions[Solutions.Length-1]; }
        }

        /// <summary>
        /// Converts the class to string for debug
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for( int i=0; i<Solutions.Length; i++)
            {
                Nelder_Mead_Solution s = Solutions[i];
                sb.Append(i.ToString("Vector[0]: "));
                sb.Append(Solutions[i].ToString());
                sb.Append("\r\n");
            }
            return sb.ToString();
        }

        #region Private methods
        private Nelder_Mead_Solution GetCentroid()
        {
            Nelder_Mead_Solution nms = new Nelder_Mead_Solution( BestSolution);
            for (int i = 1; i < Solutions.Length - 1; i++)
                nms.Append(Solutions[i]);
            nms.Scale(1.0 / Convert.ToDouble(Solutions.Length - 1));
            return NMS_Factory_Deterministic(nms);
        }

        private Nelder_Mead_Solution GetReflected(Nelder_Mead_Solution centroid)
        {
            Nelder_Mead_Solution nms = new Nelder_Mead_Solution( WorstSolution, -alpha, centroid, 1.0+alpha);
            return NMS_Factory_Deterministic(nms);
        }

        private Nelder_Mead_Solution GetExpanded(Nelder_Mead_Solution reflected, Nelder_Mead_Solution centroid)
        {
            Nelder_Mead_Solution nms = new Nelder_Mead_Solution(reflected, gamma, centroid, 1.0 - gamma);
            return NMS_Factory_Deterministic(nms);
        }

        private Nelder_Mead_Solution GetContracted( Nelder_Mead_Solution centroid)
        {
            Nelder_Mead_Solution nms = new Nelder_Mead_Solution(WorstSolution, beta, centroid, 1.0 - beta); 
            return NMS_Factory_Deterministic(nms);
        }

        private void ShrinkTowardsTheBestSolution()
        {
            for (int i = 1; i < Solutions.Length; i++)
            {
                Solutions[i].Append( BestSolution);
                Solutions[i].Scale( beta);
                NMS_Factory_Deterministic(Solutions[i]);
            }
            Array.Sort(Solutions);
        }

        private void ReplaceTheWorstWith(Nelder_Mead_Solution newSolution)
        {
            WorstSolution.ReplaceWith(newSolution);
            Array.Sort(Solutions);
        }

        private bool IsWorseThanAllButTheWorst(Nelder_Mead_Solution reflected)
        {
            for (int i=0; i<Solutions.Length-1; i++)
            {
                if (reflected.Value <= Solutions[i].Value)
                    return false;
            }
            return true;
        }

        private Nelder_Mead_Solution NMS_Factory_Random()
        {
            double[] d = new double[Dimension];
            for (int i = 0; i < Dimension; i++)
                d[i] = (maxX[i] - minX[i]) * m_random.NextDouble() + minX[i];
            Nelder_Mead_Solution nms = new Nelder_Mead_Solution(d);
            nms.Value = ComputeTarget(nms.Vector);
            return nms;
        }

        private Nelder_Mead_Solution NMS_Factory_Deterministic( Nelder_Mead_Solution nms)
        {
            for (int i = 0; i < nms.Vector.Length; i++)
            {
                //if (nms.Vector[i] < minX[i])
                //    nms.Vector[i] = (maxX[i] - minX[i]) * 0.1 * m_random.NextDouble() + minX[i];
                //if (nms.Vector[i] > maxX[i])
                //    nms.Vector[i] = maxX[i] - (maxX[i] - minX[i]) * 0.1 * m_random.NextDouble();
                if (nms.Vector[i] < minX[i])
                    nms.Vector[i] = minX[i];
                if (nms.Vector[i] > maxX[i])
                    nms.Vector[i] = maxX[i];
            }
            nms.Value = ComputeTarget( nms.Vector);
            return nms;
        }

        private Nelder_Mead_Solution NMS_Factory_Deterministic(double[] vc )
        {
            Nelder_Mead_Solution nms = new Nelder_Mead_Solution(vc);
            nms.Value = ComputeTarget(nms.Vector);
            return nms;
        }
        #endregion
    }

    /// <summary>
    /// Describes a single Nelder-Mead solution as a point in n-dimensional space
    /// with the associated target function of value
    /// </summary>
    public class Nelder_Mead_Solution : IComparable<Nelder_Mead_Solution>
    {
        public double[] Vector;
        public double Value = 0.0;

        /// <summary>
        /// Constructor, defines the solution based on vector
        /// </summary>
        /// <param name="vector">vector points</param>
        public Nelder_Mead_Solution(double[] vector)
        {
            this.Vector = new double[vector.Length];
            Array.Copy(vector, this.Vector, this.Vector.Length);
        }

        /// <summary>
        /// Constructor, defines the solution based on another solution
        /// </summary>
        /// <param name="other">external solution</param>
        public Nelder_Mead_Solution(Nelder_Mead_Solution other)
        {
            this.Vector = new double[other.Vector.Length];
            Array.Copy(other.Vector, this.Vector, this.Vector.Length);
        }

        /// <summary>
        /// Constructor, defines the solution based two other solutions
        /// other1 * c1 + other2 * c2
        /// </summary>
        public Nelder_Mead_Solution(Nelder_Mead_Solution other1, double c1, Nelder_Mead_Solution other2, double c2)
        {
            this.Vector = new double[other1.Vector.Length];
            for (int i = 0; i < this.Vector.Length; i++)
                this.Vector[i] = other1.Vector[i] * c1 + other2.Vector[i] * c2;
        }

        /// <summary>
        /// Performs value comparison
        /// </summary>
        public int CompareTo(Nelder_Mead_Solution other)
        {
            if (this.Value < other.Value) return -1;
            if (this.Value > other.Value) return 1;
            return 0;
        }

        /// <summary>
        /// Appends solution vector from other
        /// </summary>
        public void Append(Nelder_Mead_Solution other)
        {
            for (int i = 0; i < Vector.Length; i++)
                Vector[i] += other.Vector[i];
        }

        /// <summary>
        /// Scales solution vector by c
        /// </summary>
        public void Scale(double c)
        {
            for (int i = 0; i < Vector.Length; i++)
                Vector[i] *= c;
        }

        /// <summary>
        /// Replaces this to an external solution
        /// </summary>
        public void ReplaceWith(Nelder_Mead_Solution other)
        {
            Array.Copy(other.Vector, this.Vector, this.Vector.Length);
            this.Value = other.Value;
        }

        /// <summary>
        /// Converts the solution to string for debug
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("F( ");
            for (int i = 0; i < Vector.Length; i++)
            {
                if (Vector[i] >= 0.0) sb.Append(" ");
                sb.Append(Vector[i].ToString("0.000 "));
            }
            sb.Append(Value.ToString(") = 0.000"));
            return sb.ToString();
        }
    }
}
