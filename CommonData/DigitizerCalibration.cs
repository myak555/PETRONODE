using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

namespace Petronode.CommonData
{
    public class DigitizerCalibration
    {
        private char[] c_Separators = { ',', '\t'};
        private const string c_Key1 = "# Horizontal conversion = ";
        private const string c_Key2 = "# Vertical conversion = ";

        public string XAxisName = "X";
        public string YAxisName = "Y";
        public string XAxisUnits = "pixels";
        public string YAxisUnits = "pixels";
        public Point LeftBottomLocation = new Point(0, 1);
        public Point RightTopLocation = new Point(1, 0);
        public PointF LeftBottomValue = new PointF(0f, 0f);
        public PointF RightTopValue = new PointF(1f, 1f);
        public string OriginalString = "";

        /// <summary>
        /// Creates an dummy calibration 
        /// </summary>
        public DigitizerCalibration()
        {
        }

        /// <summary>
        /// Saves calibration data
        /// </summary>
        /// <param name="sw"></param>
        public void SaveCalibration( StreamWriter sw)
        {
            StringBuilder sb1 = new StringBuilder();
            sb1.Append(c_Key1);
            sb1.Append(LeftBottomLocation.X.ToString());
            sb1.Append(",");
            sb1.Append(RightTopLocation.X.ToString());
            sb1.Append(",");
            sb1.Append(LeftBottomValue.X.ToString());
            sb1.Append(",");
            sb1.Append(RightTopValue.X.ToString());
            sb1.Append(",");
            sb1.Append((XAxisUnits.Length == 0) ? "unitless" : XAxisUnits);
            sw.WriteLine(sb1.ToString());

            StringBuilder sb2 = new StringBuilder();
            sb2.Append(c_Key2);
            sb2.Append(LeftBottomLocation.Y.ToString());
            sb2.Append(",");
            sb2.Append(RightTopLocation.Y.ToString());
            sb2.Append(",");
            sb2.Append(LeftBottomValue.Y.ToString());
            sb2.Append(",");
            sb2.Append(RightTopValue.Y.ToString());
            sb2.Append(",");
            sb2.Append((YAxisUnits.Length == 0) ? "unitless" : YAxisUnits);
            sw.WriteLine(sb2.ToString());

            sw.WriteLine("#");
        }

        /// <summary>
        /// Saves data header
        /// </summary>
        /// <param name="sw"></param>
        public void SaveHeader(StreamWriter sw, bool preserveOriginal)
        {
            if (preserveOriginal)
            {
                sw.WriteLine(OriginalString);
                return;
            }
            StringBuilder sb3 = new StringBuilder();
            sb3.Append(XAxisName);
            sb3.Append(",");
            sb3.Append(YAxisName);
            sw.WriteLine(sb3.ToString());
        }

        /// <summary>
        /// Loads one calibration statement from file
        /// </summary>
        /// <param name="s">string from file</param>
        /// <returns>true if loaded</returns>
        public bool LoadCalibrationLine(string s)
        {
            if(s.StartsWith(c_Key1))
            {
                s = s.Replace(c_Key1, "");
                string[] ss1 = s.Split(c_Separators, StringSplitOptions.RemoveEmptyEntries);
                if (ss1.Length > 0) LeftBottomLocation.X = Convert.ToInt32(ss1[0]);
                if (ss1.Length > 1) RightTopLocation.X = Convert.ToInt32(ss1[1]);
                if (ss1.Length > 2) LeftBottomValue.X = Convert.ToSingle(ss1[2]);
                if (ss1.Length > 3) RightTopValue.X = Convert.ToSingle(ss1[3]);
                if (ss1.Length > 4) XAxisUnits = (ss1[4] == "unitless") ? "" : ss1[4];
                return true;
            }
            if (s.StartsWith(c_Key2))
            {
                s = s.Replace(c_Key2, "");
                string[] ss1 = s.Split(c_Separators, StringSplitOptions.RemoveEmptyEntries);
                if (ss1.Length > 0) LeftBottomLocation.Y = Convert.ToInt32(ss1[0]);
                if (ss1.Length > 1) RightTopLocation.Y = Convert.ToInt32(ss1[1]);
                if (ss1.Length > 2) LeftBottomValue.Y = Convert.ToSingle(ss1[2]);
                if (ss1.Length > 3) RightTopValue.Y = Convert.ToSingle(ss1[3]);
                if (ss1.Length > 4) YAxisUnits = (ss1[4] == "unitless") ? "" : ss1[4];
                return true;
            }
            return false;
        }

        /// <summary>
        /// Loads the header line from string
        /// </summary>
        /// <param name="s">string from file</param>
        public void LoadHeaderLine(string s)
        {
            string[] ss1 = s.Split(c_Separators, StringSplitOptions.RemoveEmptyEntries);
            if (ss1.Length > 0) XAxisName = ss1[0];
            if (ss1.Length > 1) YAxisName = ss1[1];
            OriginalString = s;
        }

        /// <summary>
        /// Converts pixel location to value
        /// </summary>
        public PointF LocationToValue(Point p)
        {
            float x = InterpolateLocation(p.X, LeftBottomLocation.X, RightTopLocation.X, LeftBottomValue.X, RightTopValue.X);
            float y = InterpolateLocation(p.Y, LeftBottomLocation.Y, RightTopLocation.Y, LeftBottomValue.Y, RightTopValue.Y);
            PointF tmp = new PointF(x,y);
            return tmp;
        }

        /// <summary>
        /// Converts pixel location to value
        /// </summary>
        public PointF LocationToValue(DigitizerPoint p)
        {
            float x = InterpolateLocation(p.Location.X, LeftBottomLocation.X, RightTopLocation.X, LeftBottomValue.X, RightTopValue.X);
            float y = InterpolateLocation(p.Location.Y, LeftBottomLocation.Y, RightTopLocation.Y, LeftBottomValue.Y, RightTopValue.Y);
            p.SetValue(x, y);
            return p.Value;
        }

        /// <summary>
        /// Converts value to pixel location 
        /// </summary>
        public Point ValueToLocation(PointF p)
        {
            int x = InterpolateValue(p.X, LeftBottomValue.X, RightTopValue.X, LeftBottomLocation.X, RightTopLocation.X);
            int y = InterpolateValue(p.Y, LeftBottomValue.Y, RightTopValue.Y, LeftBottomLocation.Y, RightTopLocation.Y);
            Point tmp = new Point(x, y);
            return tmp;
        }

        /// <summary>
        /// Converts value to pixel location 
        /// </summary>
        public Point ValueToLocation(DigitizerPoint p)
        {
            int x = InterpolateValue(p.Value.X, LeftBottomValue.X, RightTopValue.X, LeftBottomLocation.X, RightTopLocation.X);
            int y = InterpolateValue(p.Value.Y, LeftBottomValue.Y, RightTopValue.Y, LeftBottomLocation.Y, RightTopLocation.Y);
            p.SetLocation(x, y);
            return p.Location;
        }

        /// <summary>
        /// Converts fit value to pixel location 
        /// </summary>
        public Point FitValueToLocation(DigitizerPoint p)
        {
            int x = InterpolateValue(p.FitValue.X, LeftBottomValue.X, RightTopValue.X, LeftBottomLocation.X, RightTopLocation.X);
            int y = InterpolateValue(p.FitValue.Y, LeftBottomValue.Y, RightTopValue.Y, LeftBottomLocation.Y, RightTopLocation.Y);
            return new Point(x, y);
        }

        private float InterpolateLocation( int x, int x0, int x1, float y0, float y1)
        {
            int dx = x1 - x0;
            if (dx == 0) return float.NaN;
            float y = y0 * Convert.ToSingle(x1 - x) + y1 * Convert.ToSingle(x - x0);
            return y / Convert.ToSingle(dx);
        }

        private int InterpolateValue(float x, float x0, float x1, int y0, int y1)
        {
            float dx = x1 - x0;
            if (-0.0000001f < dx && dx < 0.0000001f) return int.MaxValue;
            float y = Convert.ToSingle(y0) * (x1 - x) + Convert.ToSingle(y1) * (x - x0);
            return Convert.ToInt32( y / dx);
        }
    }
}
