using System;
using System.Collections.Generic;
using System.Text;

namespace Petronode.OilfieldFileAccess.Converters
{
    public class CoordinateAngle
    {
        protected string sValue = "";
        protected double dValue = 0.0;

        protected CoordinateAngle()
        {
        }

        /// <summary>
        /// Sets and retrieves the value as a double
        /// </summary>
        public virtual double AngleD
        {
            get { return dValue; }
            set
            {
                dValue = value;
                bool negativesign = value < 0.0;
                if (negativesign) value = -value;
                double degrees = Math.Floor(value);
                value = (value - degrees) * 60.0;
                double minutes = Math.Floor(value);
                double seconds = (value - minutes) * 60.0;
                StringBuilder sb = new StringBuilder();
                if (negativesign) sb.Append("-");
                sb.Append(degrees.ToString("0"));
                sb.Append(" ");
                sb.Append(minutes.ToString("0"));
                sb.Append("\' ");
                sb.Append(seconds.ToString("0.000"));
                sValue = sb.ToString();
            }
        }

        /// <summary>
        /// Sets and retrieves the value as a string
        /// </summary>
        public virtual string AngleS
        {
            get { return sValue; }
            set
            {
                dValue = Double.NaN;
                value = value.Trim();
                bool negativesign = value.StartsWith("-");
                if (negativesign) value = value.Substring(1);
                if (value.EndsWith("N") || value.EndsWith("E") || value.EndsWith("S") || value.EndsWith("W"))
                {
                    value = value.Substring(0, value.Length - 1).Trim();
                }
                double a = ConvertAngleString(value);
                if (negativesign) a = -a;
                AngleD = a; // reformat to proper string
            }
        }

        /// <summary>
        /// Retrieves the degrees as an integer
        /// </summary>
        public int degrees
        {
            get
            {
                double a = (dValue < 0) ? -Math.Floor(-dValue) : Math.Floor(dValue);
                return Convert.ToInt32( a);
            }
        }

        /// <summary>
        /// Retrieves the minutes as an integer
        /// </summary>
        public int minutes
        {
            get
            {
                double a = (dValue > 0) ? dValue : -dValue;
                a -= Math.Floor(a);
                return Convert.ToInt32(Math.Floor(a*60.0));
            }
        }

        /// <summary>
        /// Retrieves the seconds as an integer
        /// </summary>
        public int seconds
        {
            get
            {
                double a = (dValue > 0) ? dValue : -dValue;
                a *= 60.0;
                a -= Math.Floor(a);
                return Convert.ToInt32(Math.Round(a * 60.0));
            }
        }

        private double ConvertAngleString(string s)
        {
            double degrees = 0.0;
            double minutes = 0.0;
            double seconds = 0.0;

            try
            {
                int pos = s.IndexOf(' ');
                if (pos < 0)
                {
                    degrees = Convert.ToDouble(s);
                    return degrees;
                }
                else
                {
                    degrees = Convert.ToDouble(s.Substring(0, pos));
                    s = s.Substring(pos + 1).Trim();
                }
            }
            catch (Exception)
            {
                throw new ArgumentException("Incorrect format: degrees");
            }
            try
            {
                int pos = s.IndexOf('\'');
                if (pos < 0) throw new ArgumentException();
                minutes = Convert.ToDouble(s.Substring(0, pos));
                if (minutes < 0.0 || 60.0 < minutes)
                    throw new ArgumentException();
                s = s.Substring(pos + 1).Trim();
                pos = s.IndexOf('\"');
                if (pos < 0) return degrees + minutes / 60.0;
            }
            catch (Exception)
            {
                throw new ArgumentException("Incorrect format: minutes");
            }
            try
            {
                int pos = s.IndexOf('\"');
                if (pos < 0) throw new ArgumentException();
                seconds = Convert.ToDouble(s.Substring(0, pos));
                if (seconds < 0.0 || 60.0 < seconds) throw new ArgumentException();
                if (seconds > 0.0 && minutes > 59.0) throw new ArgumentException();
                return degrees + minutes / 60.0 + seconds / 3600.0;
            }
            catch (Exception)
            {
                throw new ArgumentException("Incorrect format: seconds");
            }
        }
    }
}
