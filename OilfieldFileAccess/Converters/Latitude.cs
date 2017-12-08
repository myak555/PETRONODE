using System;
using System.Collections.Generic;
using System.Text;

namespace Petronode.OilfieldFileAccess.Converters
{
    /// <summary>
    /// Describes a latitude class
    /// </summary>
    public class Latitude:CoordinateAngle
    {
        /// <summary>
        /// Constructor, cterates an empty class
        /// </summary>
        public Latitude()
        {
        }

        /// <summary>
        /// Constructor, cterates a class with given angle
        /// </summary>
        public Latitude(double v)
        {
            AngleD = v;
        }

        /// <summary>
        /// Constructor, cterates a class with given angle
        /// </summary>
        public Latitude(string v)
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
                if (value < -90.0 || 90.0 < value) throw new ArgumentException("Latitude value out of range");
                base.AngleD = value;
                sValue += (dValue < 0.0) ? "\" S" : "\" N";
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
                if (value.EndsWith("E") || value.EndsWith("W"))
                {
                    if (dValue < 0.0) throw new ArgumentException("E/W not allowed for latitude");
                }
                if (value.EndsWith("N"))
                {
                    if (dValue < 0.0) throw new ArgumentException("Negative number for North latitude");
                }
                if (value.EndsWith("S"))
                {
                    if (dValue > 0.0) AngleD = -dValue;
                }
            }
        }
    }
}
