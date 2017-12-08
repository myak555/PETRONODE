using System;
using System.Collections.Generic;
using System.Text;

namespace Petronode.OilfieldFileAccess.Converters
{
    /// <summary>
    /// Describes a longitude class
    /// </summary>
    public class Longitude:CoordinateAngle
    {
        /// <summary>
        /// Constructor, cterates an empty convertor
        /// </summary>
        public Longitude()
        {
        }

        /// <summary>
        /// Constructor, cterates an convertor with given angle
        /// </summary>
        public Longitude(double v)
        {
            AngleD = v;
        }

        /// <summary>
        /// Constructor, cterates an convertor with given angle
        /// </summary>
        public Longitude(string v)
        {
            AngleS = v;
        }

        /// <summary>
        /// Converts to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return AngleS;
        }

        /// <summary>
        /// Sets and retrieves the value as a double
        /// </summary>
        public override double AngleD
        {
            get { return dValue; }
            set
            {
                if (value < -180.0 || 180.0 < value) throw new ArgumentException("Longitude value out of range");
                base.AngleD = value;
                sValue += (dValue < 0.0) ? "\" W" : "\" E";
            }
        }

        /// <summary>
        /// Sets and retrieves the value as a string
        /// </summary>
        public override string AngleS
        {
            get { return sValue; }
            set
            {
                value = value.Trim();
                base.AngleS = value;
                if (value.EndsWith("N") || value.EndsWith("S"))
                {
                    if (dValue < 0.0) throw new ArgumentException("N/S not allowed for longitude");
                }
                if (value.EndsWith("E"))
                {
                    if (dValue<0.0) throw new ArgumentException("Negative number for East longitude");
                }
                if (value.EndsWith("W"))
                {
                    if (dValue > 0.0) AngleD = -dValue;
                }
            }
        }
    }
}
