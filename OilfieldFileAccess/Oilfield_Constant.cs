using System;
using System.Collections.Generic;
using System.Text;

namespace Petronode.OilfieldFileAccess
{
    public class Oilfield_Constant
    {
        public string Name = "Unknown";
        public string Unit = "";
        public string Value = "";
        public string Description = "";

        #region Constructors
        /// <summary>
        /// Constructor - creates a default variable
        /// </summary>
        protected Oilfield_Constant()
        {
        }

        /// <summary>
        /// Constructor - creates from the parameters
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="unit">Variable unit</param>
        /// <param name="value">Variable value</param>
        /// <param name="description">Description</param>
        protected Oilfield_Constant(string name, string unit, string value, string description)
        {
            Name = name;
            Unit = unit;
            Value = value;
            Description = description;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Converts back to LAS file string
        /// </summary>
        /// <returns>Converted string</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(" ");
            sb.Append(Name);
            if (Description.Length > 0)
            {
                sb.Append("(");
                sb.Append(Description);
                sb.Append(")");
            }
            sb.Append(" = ");
            sb.Append(Value);
            if (Unit.Length > 0)
            {
                sb.Append(" [");
                sb.Append(Unit);
                sb.Append("]");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts to representative strings
        /// </summary>
        /// <returns>Converted strings</returns>
        public virtual string[] ToStrings()
        {
            string[] tmp = new string[4];
            tmp[0] = this.Name;
            tmp[1] = this.Value;
            tmp[2] = this.Unit;
            tmp[3] = this.Description;
            return tmp;
        }

        /// <summary>
        /// Converts to representative strings
        /// </summary>
        /// <returns>Converted strings</returns>
        public virtual string[] ToStringsSimple()
        {
            string[] tmp = new string[2];
            tmp[0] = this.Description;
            tmp[1] = this.Value;
            return tmp;
        }

        /// <summary>
        /// Creates a deep copy of this constant
        /// </summary>
        /// <returns></returns>
        public virtual Oilfield_Constant Clone()
        {
            Oilfield_Constant tmp = new Oilfield_Constant();
            tmp.Name = this.Name;
            tmp.Unit = this.Unit;
            tmp.Value = this.Value;
            tmp.Description = this.Description;
            return tmp;
        }

        /// <summary>
        /// Sets the value using the format given
        /// </summary>
        /// <param name="val">value to set</param>
        /// <param name="format">format</param>
        public virtual void SetValueD(double val, string format)
        {
            this.Value = val.ToString(format);
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Sets and retrieves the constant value as double
        /// </summary>
        public virtual double ValueD
        {
            get
            {
                try { return Convert.ToDouble(this.Value); }
                catch (Exception) { return Double.NaN; }
            }
            set
            {
                this.Value = value.ToString();
            }
        }
        #endregion

        #region Operators
        /// <summary>
        /// Is this class equivalent to another object?
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override bool Equals(object o)
        {
            if (o == null)
            {
                return false;
            }
            if (o is Oilfield_Constant)
            {
                Oilfield_Constant c = (Oilfield_Constant)o;
                return (this.Value == c.Value && this.Unit == c.Unit);
            }
            if (o is double)
            {
                double c = (double)o;
                return (this.ValueD == c);
            }
            if (o is string)
            {
                string c = (string)o;
                return (this.Value == c);
            }
            return false;
        }

        /// <summary>
        /// Compare to other objects
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public int CompareTo(object o)
        {
            if (o == null)
            {
                return 1;  // null sorts before current
            }
            double c1 = this.ValueD;
            double c2 = c1;
            if (o is Oilfield_Constant)
            {
                Oilfield_Constant c = (Oilfield_Constant)o;
                c2 = c.ValueD;
            }
            if (o is double)
            {
                c2 = (double)o;
            }
            if (c1 < c2) return -1;
            if (c1 > c2) return 1;
            return 0;
        }

        /// <summary>
        /// Are these two objects equivalent?
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(object a, Oilfield_Constant b)
        {
            return b.Equals( a);
        }

        /// <summary>
        /// Are these two objects equivalent?
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Oilfield_Constant a,  object b)
        {
            return a.Equals( b);
        }

        /// <summary>
        /// Are these two objects different?
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=( object a, Oilfield_Constant b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Are these two objects different?
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Oilfield_Constant a, object b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Get the hash code of the complex number
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            double c = this.ValueD;
            if (Double.IsNaN(c)) return 0;
            try { return Convert.ToInt32(c); }
            catch (Exception) { return 0; }
        }

        /// <summary>
        /// Unary plus
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double operator +(Oilfield_Constant a)
        {
            return a.ValueD;
        }

        /// <summary>
        /// Unary minus
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double operator -(Oilfield_Constant a)
        {
            return -a.ValueD;
        }

        /// <summary>
        /// Add a real number
        /// </summary>
        /// <param name="a"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static double operator +(Oilfield_Constant a, double f)
        {
            double c = a.ValueD;
            return c + f;
        }

        /// <summary>
        /// Add to a real number
        /// </summary>
        /// <param name="f"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double operator +(double f, Oilfield_Constant a)
        {
            double c = a.ValueD;
            return c + f;
        }

        /// <summary>
        /// Add two constants
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double operator +(Oilfield_Constant a, Oilfield_Constant b)
        {
            double c1 = a.ValueD;
            double c2 = b.ValueD;
            return c1+c2;
        }

        /// <summary>
        /// Subtract a real number
        /// </summary>
        /// <param name="a"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static double operator -(Oilfield_Constant a, double f)
        {
            double c = a.ValueD;
            return c - f;
        }

        /// <summary>
        /// Subtract from a real number
        /// </summary>
        /// <param name="f"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double operator -(double f, Oilfield_Constant a)
        {
            double c = a.ValueD;
            return f - c;
        }

        /// <summary>
        /// Subtract two constants
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double operator -(Oilfield_Constant a, Oilfield_Constant b)
        {
            double c1 = a.ValueD;
            double c2 = b.ValueD;
            return c1 - c2;
        }

        /// <summary>
        /// Multiply by a real number
        /// </summary>
        /// <param name="a"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static double operator *(Oilfield_Constant a, double f)
        {
            double c = a.ValueD;
            return c * f;
        }

        /// <summary>
        /// Multiply a real number
        /// </summary>
        /// <param name="f"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double operator *(double f, Oilfield_Constant a)
        {
            double c = a.ValueD;
            return c * f;
        }

        /// <summary>
        /// Multiply two constants
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double operator *(Oilfield_Constant a, Oilfield_Constant b)
        {
            double c1 = a.ValueD;
            double c2 = b.ValueD;
            return c1 * c2;
        }

        /// <summary>
        /// Divide by a real number
        /// </summary>
        /// <param name="a"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static double operator /(Oilfield_Constant a, double f)
        {
            double c = a.ValueD;
            return c / f;
        }

        /// <summary>
        /// Divide by real number
        /// </summary>
        /// <param name="f"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double operator /(double f, Oilfield_Constant a)
        {
            double c = a.ValueD;
            return f / c;
        }

        /// <summary>
        /// Divide two constants
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double operator /(Oilfield_Constant a, Oilfield_Constant b)
        {
            double c1 = a.ValueD;
            double c2 = b.ValueD;
            return c1 / c2;
        }

        /// <summary>
        /// Determine whether two constants are almost (i.e. within the tolerance) equivalent.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        static public bool IsEqual(object a, object b, double tolerance)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;
            if (b == null) return false;
            if (a is Oilfield_Constant && b is Oilfield_Constant)
            {
                Oilfield_Constant a1 = (Oilfield_Constant) a;
                Oilfield_Constant b1 = (Oilfield_Constant) b;
                if (Math.Abs(a1 - b1) < tolerance) return true; 
                return false;
            }
            if (a is Oilfield_Constant && b is double)
            {
                Oilfield_Constant a1 = (Oilfield_Constant)a;
                double b1 = (double)b;
                if (Math.Abs(a1 - b1) < tolerance) return true;
                return false;
            }
            if (a is double && b is Oilfield_Constant)
            {
                double a1 = (double)a;
                Oilfield_Constant b1 = (Oilfield_Constant)b;
                if (Math.Abs(a1 - b1) < tolerance) return true;
                return false;
            }
            if (a is double && b is double)
            {
                double a1 = (double)a;
                double b1 = (double)b;
                if (Math.Abs(a1 - b1) < tolerance) return true;
                return false;
            }
            return false;
        }
        #endregion
    }
}
