using System;
using System.Collections.Generic;
using System.Text;

namespace Petronode.CommonData
{
    #region Linear Function
    public class LinearFunction: FitterFunction 
    {
        /// <summary>
        /// Describes a linear function y = a * x + b
        /// </summary>
        public LinearFunction()
        {
            Name = "Linear";
            Description = "Classic linear gain-offset function";
            Parameters.Add(
                new FitterParameter("gain", "Linear function gain", 1.0, 0.0, 2.0, "0.00000"));
            Parameters.Add(
                new FitterParameter("offset", "Linear function offset", 0.0, -1.0, 1.0));
        }

        /// <summary>
        /// Sets parameters based on a given point
        /// </summary>
        /// <param name="x">converted digitizer x</param>
        /// <param name="y">converted digitizer y</param>
        /// <returns>true</returns>
        public override bool SetParametersFromClick(float x, float y)
        {
            Parameters[1].Value = 
                Convert.ToDouble(y) - Parameters[0].Value * Convert.ToDouble(x);
            return true;
        }

        /// <summary>
        /// Makes a deep clone
        /// </summary>
        public override FitterFunction Clone()
        {
            LinearFunction tmp = new LinearFunction();
            tmp.CloneFunction( this);
            return tmp;
        }

        /// <summary>
        /// Computes the function
        /// </summary>
        /// <param name="x">entry variable</param>
        /// <returns>computed value y</returns>
        protected override double Compute_Unsafe(double x)
        {
            return Parameters[0].Value * x + Parameters[1].Value;
        }
    }
    #endregion

    #region Fix Point Gradient Function
    public class FixPointGradientFunction : FitterFunction
    {
        /// <summary>
        /// Describes a gradient function y = y0 + a * (x - x0)
        /// </summary>
        public FixPointGradientFunction()
        {
            Name = "FixPointGradient";
            Description = "� gradient through a fix point";
            Parameters.Add(
                new FitterParameter("gragient", "Slope value", 1.0, 0.0, 2.0, "0.00000"));
            Parameters.Add(
                new FitterParameter("x0", "Fixed point x", 0.0, -1.0, 1.0));
            Parameters.Add(
                new FitterParameter("y0", "Fixed point y", 0.0, -1.0, 1.0));
            Parameters[1].AcceptDigitizerX = true;
            Parameters[2].AcceptDigitizerY = true;
        }

        /// <summary>
        /// Makes a deep clone
        /// </summary>
        public override FitterFunction Clone()
        {
            FixPointGradientFunction tmp = new FixPointGradientFunction();
            tmp.CloneFunction(this);
            return tmp;
        }

        /// <summary>
        /// Computes the function
        /// </summary>
        /// <param name="x">entry variable</param>
        /// <returns>computed value y</returns>
        protected override double Compute_Unsafe(double x)
        {
            return Parameters[0].Value * (x - Parameters[1].Value) + Parameters[2].Value;
        }
    }
    #endregion

    #region Parabolic Function
    public class ParabolicFunction : FitterFunction
    {
        /// <summary>
        /// Describes a parabolic function y = b + a * (x - x0)^n
        /// </summary>
        public ParabolicFunction()
        {
            Name = "Parabolic";
            Description = "Parabolic x^n";
            Parameters.Add(
                new FitterParameter("x0", "Fixed point x", 0.0, -1.0, 1.0));
            Parameters.Add(
                new FitterParameter("a", "Vertical scale", 1.0, -10.0, 10.0, "0.00000"));
            Parameters.Add(
                new FitterParameter("b", "Vertical offset", 0.0, -1.0, 1.0));
            Parameters.Add(
                new FitterParameter("n", "Function power", 2.0, 2.0, 2.0, "0.00000"));
            Parameters[0].AcceptDigitizerX = true;
            Parameters[2].AcceptDigitizerY = true;
        }

        /// <summary>
        /// Makes a deep clone
        /// </summary>
        public override FitterFunction Clone()
        {
            ParabolicFunction tmp = new ParabolicFunction();
            tmp.CloneFunction(this);
            return tmp;
        }

        /// <summary>
        /// Computes the function
        /// </summary>
        /// <param name="x">entry variable</param>
        /// <returns>computed value y</returns>
        protected override double Compute_Unsafe(double x)
        {
            if (-1e-10 < Parameters[3].Value && Parameters[3].Value < 1e-10)
                return Parameters[1].Value + Parameters[2].Value;
            x -= Parameters[0].Value;
            double y = 1.0;
            if (x < 0.0)
            {
                int n = Convert.ToInt32(Parameters[3].Value);
                while (n > 0)
                {
                    y *= x;
                    n--;
                }
                while (n < 0)
                {
                    y /= x;
                    n++;
                }
            }
            else
                y = Math.Pow(x, Parameters[3].Value);
            return y * Parameters[1].Value + Parameters[2].Value;
        }
    }
    #endregion

    #region Exponential Function
    public class ExponentialFunction : FitterFunction
    {
        /// <summary>
        /// Describes an exponent function y = b + a * exp( sigma*(x - x0))
        /// </summary>
        public ExponentialFunction()
        {
            Name = "Exponential";
            Description = "Exponential exp(x)";
            Parameters.Add(
                new FitterParameter("x0", "Fixed point x", 0.0, -1.0, 1.0));
            Parameters.Add(
                new FitterParameter("a", "Vertical scale", 1.0, -10.0, 10.0, "0.00000"));
            Parameters.Add(
                new FitterParameter("b", "Vertical offset", 0.0, -1.0, 1.0));
            Parameters.Add(
                new FitterParameter("s0", "Function power", 1.0, -1.0, 2.0, "0.00000"));
            Parameters[0].AcceptDigitizerX = true;
            Parameters[2].AcceptDigitizerY = true;
        }

        /// <summary>
        /// Makes a deep clone
        /// </summary>
        public override FitterFunction Clone()
        {
            ExponentialFunction tmp = new ExponentialFunction();
            tmp.CloneFunction(this);
            return tmp;
        }

        /// <summary>
        /// Computes the function
        /// </summary>
        /// <param name="x">entry variable</param>
        /// <returns>computed value y</returns>
        protected override double Compute_Unsafe(double x)
        {
            if (-1e-10 < Parameters[3].Value && Parameters[3].Value < 1e-10)
                return Parameters[1].Value + Parameters[2].Value;
            x -= Parameters[0].Value;
            x *= Parameters[3].Value;
            x = Math.Exp(x);
            return x * Parameters[1].Value + Parameters[2].Value;
        }
    }
    #endregion

    #region Logarithmic Function
    public class LogarithmicFunction : FitterFunction
    {
        /// <summary>
        /// Describes an exponent function y = b + a * ln(x - x0)
        /// </summary>
        public LogarithmicFunction()
        {
            Name = "Logarithmic";
            Description = "Logarithmic ln(x)";
            Parameters.Add(
                new FitterParameter("x0", "Fixed point x", 0.0, -1.0, 1.0));
            Parameters.Add(
                new FitterParameter("a", "Vertical scale", 1.0, -10.0, 10.0, "0.00000"));
            Parameters.Add(
                new FitterParameter("b", "Vertical offset", 0.0, -1.0, 1.0));
            Parameters[0].AcceptDigitizerX = true;
            Parameters[2].AcceptDigitizerY = true;
        }

        /// <summary>
        /// Makes a deep clone
        /// </summary>
        public override FitterFunction Clone()
        {
            LogarithmicFunction tmp = new LogarithmicFunction();
            tmp.CloneFunction(this);
            return tmp;
        }

        /// <summary>
        /// Computes the function
        /// </summary>
        /// <param name="x">entry variable</param>
        /// <returns>computed value y</returns>
        protected override double Compute_Unsafe(double x)
        {
            x -= Parameters[0].Value;
            if (x <= 1e-99) return Math.Log( 1e-99);
            x = Math.Log(x);
            return x * Parameters[1].Value + Parameters[2].Value;
        }
    }
    #endregion

    #region Harmonic Function
    public class HarmonicFunction : FitterFunction
    {
        /// <summary>
        /// Describes a periodic function y = b + a * sin( omega*(x - x0))
        /// </summary>
        public HarmonicFunction()
        {
            Name = "Harmonic";
            Description = "Harmonic (sinusoid)";
            Parameters.Add(
                new FitterParameter("x0", "Fixed point x", 0.0, -1.0, 1.0));
            Parameters.Add(
                new FitterParameter("a", "Vertical scale", 0.0, -1.0, 1.0, "0.00000"));
            Parameters.Add(
                new FitterParameter("b", "Vertical offset", 0.0, -1.0, 1.0));
            Parameters.Add(
                new FitterParameter("f", "Signal frequency", 10, 0.0, 1000.0));
            Parameters[0].AcceptDigitizerX = true;
            Parameters[2].AcceptDigitizerY = true;
        }

        /// <summary>
        /// Makes a deep clone
        /// </summary>
        public override FitterFunction Clone()
        {
            HarmonicFunction tmp = new HarmonicFunction();
            tmp.CloneFunction(this);
            return tmp;
        }

        /// <summary>
        /// Computes the function
        /// </summary>
        /// <param name="x">entry variable</param>
        /// <returns>computed value y</returns>
        protected override double Compute_Unsafe(double x)
        {
            x -= Parameters[0].Value;
            x *= Parameters[3].Value * Math.PI;
            x = Math.Sin(x);
            return x * Parameters[1].Value + Parameters[2].Value;
        }
    }
    #endregion

    #region Berlage Function
    public class BerlageFunction : FitterFunction
    {
        /// <summary>
        /// Describes a periodic declining function y = b + a * exp( -s0*(x - x0)) * sin( omega*(x - x0))
        /// </summary>
        public BerlageFunction()
        {
            Name = "BerlagePulse";
            Description = "BerlagePulse (declining sine)";
            Parameters.Add(
                new FitterParameter("x0", "Fixed point x", 0.0, -1.0, 1.0));
            Parameters.Add(
                new FitterParameter("a", "Vertical scale", 0.0, -1.0, 1.0, "0.00000"));
            Parameters.Add(
                new FitterParameter("b", "Vertical offset", 0.0, -1.0, 1.0));
            Parameters.Add(
                new FitterParameter("f", "Signal frequency", 10.0, 0.0, 10000.0));
            Parameters.Add(
                new FitterParameter("s0", "Decrement", 1.0, 0.0, 10.0, "0.00000"));
            Parameters[0].AcceptDigitizerX = true;
            Parameters[2].AcceptDigitizerY = true;
        }

        /// <summary>
        /// Makes a deep clone
        /// </summary>
        public override FitterFunction Clone()
        {
            BerlageFunction tmp = new BerlageFunction();
            tmp.CloneFunction(this);
            return tmp;
        }

        /// <summary>
        /// Computes the function
        /// </summary>
        /// <param name="x">entry variable</param>
        /// <returns>computed value y</returns>
        protected override double Compute_Unsafe(double x)
        {
            x -= Parameters[0].Value;
            if (x <= 0.0) return Parameters[2].Value;
            double y = Math.Exp(-x * Parameters[4].Value);
            x *= Parameters[3].Value * Math.PI;
            y *= Math.Sin(x);
            return y * Parameters[1].Value + Parameters[2].Value;
        }
    }
    #endregion

    #region Ricker Function
    public class RickerWaveletFunction : FitterFunction
    {
        /// <summary>
        /// Describes a standard Ricker wavelet (Mexical Hat)
        /// </summary>
        public RickerWaveletFunction()
        {
            Name = "RickerWavelet";
            Description = "Ricker wavelet (zero phase)";
            Parameters.Add(
                new FitterParameter("x0", "Fixed point x", 0.0, -1.0, 1.0));
            Parameters.Add(
                new FitterParameter("a", "Vertical scale", 0.0, -1.0, 1.0, "0.00000"));
            Parameters.Add(
                new FitterParameter("b", "Vertical offset", 0.0, -1.0, 1.0));
            Parameters.Add(
                new FitterParameter("f", "Signal frequency", 10.0, 0.0, 10000.0));
            Parameters[0].AcceptDigitizerX = true;
            Parameters[2].AcceptDigitizerY = true;
        }

        /// <summary>
        /// Makes a deep clone
        /// </summary>
        public override FitterFunction Clone()
        {
            RickerWaveletFunction tmp = new RickerWaveletFunction();
            tmp.CloneFunction(this);
            return tmp;
        }

        /// <summary>
        /// Computes the function
        /// </summary>
        /// <param name="x">entry variable</param>
        /// <returns>computed value y</returns>
        protected override double Compute_Unsafe(double x)
        {
            x -= Parameters[0].Value;
            x *= Parameters[3].Value * Math.PI;
            x *= x;
            double y = (1.0-2.0*x) * Math.Exp(-x);
            return y * Parameters[1].Value + Parameters[2].Value;
        }
    }
    #endregion

    #region Sigmoid Function
    public class SigmoidFunction : FitterFunction
    {
        /// <summary>
        /// Describes a Sigmoid function
        /// y(x) = left + (right-left)/(1+exp(-slope*(x-x0))) + shift
        /// </summary>
        public SigmoidFunction()
        {
            Name = "Sigmoid";
            Description = "Classic Sigmoid (Velhurst) function";
            Parameters.Add(
                new FitterParameter("x0", "Inflection point location X", 0.0, -1.0, 1.0));
            Parameters.Add(
                new FitterParameter("s0", "Slope", 1.0, 0.0, 2.0, "0.00000"));
            Parameters.Add(
                new FitterParameter("left", "Left-side value", 0.0, -1.0, 1.0));
            Parameters.Add(
                new FitterParameter("right", "Right-side value", 1.0, 0.0, 2.0));
            Parameters.Add(
                new FitterParameter("shift", "Constant shift", 0.0, 0.0, 0.0));
        }

        /// <summary>
        /// Sets parameters based on a given point
        /// </summary>
        /// <param name="x">converted digitizer x</param>
        /// <param name="y">converted digitizer y</param>
        /// <returns>true</returns>
        public override bool SetParametersFromClick(float x, float y)
        {
            Parameters[0].Value = Convert.ToDouble(x);
            Parameters[3].Value = 2.0 * Convert.ToDouble(y);
            return true;
        }

        /// <summary>
        /// Makes a deep clone
        /// </summary>
        public override FitterFunction Clone()
        {
            SigmoidFunction tmp = new SigmoidFunction();
            tmp.CloneFunction(this);
            return tmp;
        }

        /// <summary>
        /// Computes the function
        /// </summary>
        /// <param name="x">entry variable</param>
        /// <returns>computed value y</returns>
        protected override double Compute_Unsafe(double x)
        {
            x -= Parameters[0].Value;
            x *= Parameters[1].Value;
            double e = Math.Exp(-x);
            x = (Parameters[3].Value - Parameters[2].Value) / (1.0 + e);
            x += Parameters[2].Value + Parameters[4].Value;
            return x;
        }
    }
    #endregion

    #region Bathtub Function
    public class BathtubFunction : FitterFunction
    {
        /// <summary>
        /// Describes a Bathtub function
        /// y(x) = left + (middle-left)/(1+exp(-s0*(x-x0)))
        ///        (right-middle)/(1+exp(-s1*(x-x1))) + shift
        /// </summary>
        public BathtubFunction()
        {
            Name = "Bathtub";
            Description = "Bathtub function as superposition of two sigmoids";
            Parameters.Add(
                new FitterParameter("x0", "Left inflection point", 0.0, -1.0, 1.0));
            Parameters.Add(
                new FitterParameter("s0", "Left slope", 1.0, 0.0, 2.0, "0.00000"));
            Parameters.Add(
                new FitterParameter("x1", "Right inflection point", 1.0, 0.0, 2.0));
            Parameters.Add(
                new FitterParameter("s1", "Right slope", 1.0, 0.0, 2.0, "0.00000"));
            Parameters.Add(
                new FitterParameter("left", "Left-side value", 1.0, 0.0, 2.0));
            Parameters.Add(
                new FitterParameter("middle", "Middle value", 0.0, -1.0, 1.0));
            Parameters.Add(
                new FitterParameter("right", "Right-side value", 1.0, 0.0, 2.0));
            Parameters.Add(
                new FitterParameter("shift", "Constant shift", 0.0, 0.0, 0.0));
        }

        /// <summary>
        /// Sets parameters based on a given point
        /// </summary>
        /// <param name="x">converted digitizer x</param>
        /// <param name="y">converted digitizer y</param>
        /// <returns>true</returns>
        public override bool SetParametersFromClick(float x, float y)
        {
            Parameters[0].Value = Convert.ToDouble(x);
            Parameters[3].Value = 2.0 * Convert.ToDouble(y);
            return true;
        }

        /// <summary>
        /// Makes a deep clone
        /// </summary>
        public override FitterFunction Clone()
        {
            BathtubFunction tmp = new BathtubFunction();
            tmp.CloneFunction(this);
            return tmp;
        }

        /// <summary>
        /// Computes the function
        /// </summary>
        /// <param name="x">entry variable</param>
        /// <returns>computed value y</returns>
        protected override double Compute_Unsafe(double x)
        {
            double x1 = x - Parameters[0].Value;
            x1 *= Parameters[1].Value;
            double e = Math.Exp(-x1);
            x1 = (Parameters[5].Value - Parameters[4].Value) / (1.0 + e);
            x1 += Parameters[4].Value;
            double x2 = x - Parameters[2].Value;
            x2 *= Parameters[3].Value;
            e = Math.Exp(-x2);
            x2 = (Parameters[6].Value - Parameters[5].Value) / (1.0 + e);
            return x1 + x2 + Parameters[7].Value;
        }
    }
    #endregion

    #region Hubbert Function
    public class HubbertFunction : FitterFunction
    {
        /// <summary>
        /// Describes a Hubbert function
        /// y(x) = peak * 4 * exp( - slope * (x-x0) / (1+exp(- slope * (x-x0))^2 + shift
        /// </summary>
        public HubbertFunction()
        {
            Name = "Hubbert";
            Description = "Classic Hubbert function with two slopes";
            Parameters.Add(
                new FitterParameter("x0", "Peak location X", 0.0, -1.0, 1.0));
            Parameters.Add(
                new FitterParameter("s0", "Left slope", 1.0, 0.0, 2.0, "0.00000"));
            Parameters.Add(
                new FitterParameter("s1", "Right slope", 1.0, 0.0, 2.0, "0.00000"));
            Parameters.Add(
                new FitterParameter("peak", "Peak absolute value", 1.0, -1.0, 2.0));
            Parameters.Add(
                new FitterParameter("shift", "Constant shift", 0.0, 0.0, 0.0));
            Parameters[0].AcceptDigitizerX = true;
            Parameters[3].AcceptDigitizerY = true;
        }

        /// <summary>
        /// Makes a deep clone
        /// </summary>
        public override FitterFunction Clone()
        {
            HubbertFunction tmp = new HubbertFunction();
            tmp.CloneFunction(this);
            return tmp;
        }

        /// <summary>
        /// Computes the function
        /// </summary>
        /// <param name="x">entry variable</param>
        /// <returns>computed value y</returns>
        protected override double Compute_Unsafe(double x)
        {
            x -= Parameters[0].Value;
            x *= Parameters[(x < 0.0) ? 1 : 2].Value;
            double e = Math.Exp(-x);
            double y = 4.0 * Parameters[3].Value * e / (1 + e) / (1 + e) + Parameters[4].Value;
            return y;
        }
    }
    #endregion

    #region Gauss Function
    public class GaussFunction : FitterFunction
    {
        /// <summary>
        /// Describes a Gauss function
        /// y(x) = peak * exp( - slope * (x-x0)^2)
        /// </summary>
        public GaussFunction()
        {
            Name = "Gauss";
            Description = "Classic Gauss function with two slopes";
            Parameters.Add(
                new FitterParameter("x0", "Peak location X", 0.0, -1.0, 1.0));
            Parameters.Add(
                new FitterParameter("s0", "Left slope", 1.0, 0.0, 2.0, "0.00000"));
            Parameters.Add(
                new FitterParameter("s1", "Right slope", 1.0, 0.0, 2.0, "0.00000"));
            Parameters.Add(
                new FitterParameter("peak", "Peak absolute value", 1.0, -1.0, 2.0));
            Parameters.Add(
                new FitterParameter("shift", "Constant shift", 0.0, 0.0, 0.0));
            Parameters[0].AcceptDigitizerX = true;
            Parameters[3].AcceptDigitizerY = true;
        }

        /// <summary>
        /// Makes a deep clone
        /// </summary>
        public override FitterFunction Clone()
        {
            GaussFunction tmp = new GaussFunction();
            tmp.CloneFunction(this);
            return tmp;
        }

        /// <summary>
        /// Computes the function
        /// </summary>
        /// <param name="x">entry variable</param>
        /// <returns>computed value y</returns>
        protected override double Compute_Unsafe(double x)
        {
            x -= Parameters[0].Value;
            double s = Parameters[(x < 0.0) ? 1 : 2].Value;
            double e = Math.Exp(-s*x*x);
            return Parameters[3].Value * e + Parameters[4].Value;
        }
    }
    #endregion

    #region Kapitsa Function
    public class KapitsaFunction : FitterFunction
    {
        /// <summary>
        /// Describes a Kapitsa function
        /// y(x) = peak / (1 + (slope*(x0-x))^2) + shift
        /// </summary>
        public KapitsaFunction()
        {
            Name = "Kapitsa";
            Description = "Kapitsa function with two slopes";
            Parameters.Add(
                new FitterParameter("x0", "Peak location X", 0.0, -1.0, 1.0));
            Parameters.Add(
                new FitterParameter("s0", "Left slope", 1.0, 0.0, 2.0, "0.00000"));
            Parameters.Add(
                new FitterParameter("s1", "Right slope", 1.0, 0.0, 2.0, "0.00000"));
            Parameters.Add(
                new FitterParameter("peak", "Peak absolute value", 1.0, -1.0, 2.0));
            Parameters.Add(
                new FitterParameter("shift", "Constant shift", 0.0, 0.0, 0.0));
            Parameters[0].AcceptDigitizerX = true;
            Parameters[3].AcceptDigitizerY = true;
        }

        /// <summary>
        /// Makes a deep clone
        /// </summary>
        public override FitterFunction Clone()
        {
            KapitsaFunction tmp = new KapitsaFunction();
            tmp.CloneFunction(this);
            return tmp;
        }

        /// <summary>
        /// Computes the function
        /// </summary>
        /// <param name="x">entry variable</param>
        /// <returns>computed value y</returns>
        protected override double Compute_Unsafe(double x)
        {
            x -= Parameters[0].Value;
            x *= Parameters[(x < 0.0) ? 1 : 2].Value;
            x *= x;
            return Parameters[3].Value / (1.0 + x) + Parameters[4].Value;
        }
    }
    #endregion

    #region Weibull Function
    public class WeibullFunction : FitterFunction
    {
        /// <summary>
        /// Describes a Weibull function
        /// y(x) = B * k * ( B * (x-x0))^(k-1)) * exp(-( B * (x-x0))^(k))) + shift
        /// </summary>
        public WeibullFunction()
        {
            Name = "Weibull";
            Description = "Weibull function";
            Parameters.Add(
                new FitterParameter("x0", "Start production", 0.0, -1.0, 1.0));
            Parameters.Add(
                new FitterParameter("b", "Slope factor", 1.0, 0.0, 2.0));
            Parameters.Add(
                new FitterParameter("k", "Shape factor", 2.0, 1.0, 3.0, "0.00000"));
            Parameters.Add(
                new FitterParameter("scale", "Vertical scale factor", 1.0, -1.0, 2.0));
            Parameters.Add(
                new FitterParameter("shift", "Constant shift", 0.0, 0.0, 0.0));
            Parameters[0].AcceptDigitizerX = true;
            Parameters[4].AcceptDigitizerY = true;
        }

        /// <summary>
        /// Makes a deep clone
        /// </summary>
        public override FitterFunction Clone()
        {
            WeibullFunction tmp = new WeibullFunction();
            tmp.CloneFunction(this);
            return tmp;
        }

        /// <summary>
        /// Computes the function
        /// </summary>
        /// <param name="x">entry variable</param>
        /// <returns>computed value y</returns>
        protected override double Compute_Unsafe(double x)
        {
            x -= Parameters[0].Value;
            if (x <= 0.0) return 0.0;
            x *= Parameters[1].Value;
            double p1 = Math.Pow(x, Parameters[2].Value);
            double p2 = p1 / x;
            p1 = Math.Exp(-p1);
            double y = Parameters[1].Value * Parameters[2].Value * p1 * p2;
            return y * Parameters[3].Value + Parameters[4].Value;
        }
    }
    #endregion
}
