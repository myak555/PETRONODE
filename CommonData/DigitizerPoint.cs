using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Petronode.CommonData
{
    /// <summary>
    /// Implements the data model for an on-screen digitizer
    /// </summary>
    public class DigitizerPoint
    {
        private char[] c_Separators = { ',', '\t'};

        public Point Location = new Point(0, 0);
        public PointF Value = new PointF(0f, 0f);
        public PointF FitValue = new PointF(0f, 0f);
        public PointF PrefitValue = new PointF(0f, 0f);
        public string OriginalString = "";

        public DigitizerPoint()
        {
        }

        public DigitizerPoint(string s)
        {
            string[] ss1 = s.Split(c_Separators, StringSplitOptions.RemoveEmptyEntries);

            if (ss1.Length > 0) Value.X = Convert.ToSingle(ss1[0]);
            if (ss1.Length > 1) Value.Y = Convert.ToSingle(ss1[1]);
            OriginalString = s;
        }

        public DigitizerPoint(int x, int y, float xV, float yV)
        {
            Location.X = x;
            Location.Y = y;
            Value.X = xV;
            Value.Y = yV;
        }

        public override string ToString()
        {
            StringBuilder sb1 = new StringBuilder();
            sb1.Append(Value.X.ToString("0.000"));
            sb1.Append(",");
            sb1.Append(Value.Y.ToString("0.000"));
            return sb1.ToString();
        }

        public string[] ToLocationStrings()
        {
            string[] tmp = new string[4];
            tmp[0] = Location.X.ToString();
            tmp[1] = Location.Y.ToString();
            tmp[2] = Value.X.ToString("0.000");
            tmp[3] = Value.Y.ToString("0.000");
            return tmp;
        }

        public string[] ToHorizontalFitStrings()
        {
            string[] tmp = new string[4];
            tmp[0] = Value.X.ToString("0.000");
            tmp[1] = Value.Y.ToString("0.000");
            tmp[2] = FitValue.Y.ToString("0.000");
            if (float.IsNaN(FitValue.Y) || float.IsNaN(Value.Y))
            {
                tmp[3] = float.NaN.ToString("0.000");
                return tmp;
            }
            float t = FitValue.Y-Value.Y;
            tmp[3] = t.ToString("0.000");
            return tmp;
        }

        public void SetLocation(Point p)
        {
            Location.X = p.X;
            Location.Y = p.Y;
        }

        public void SetLocation(int x, int y)
        {
            Location.X = x;
            Location.Y = y;
        }

        public void SetValue(PointF p)
        {
            Value.X = p.X;
            Value.Y = p.Y;
        }

        public void SetValue(float x, float y)
        {
            Value.X = x;
            Value.Y = y;
        }

        public void SetFitValue(float x, float y)
        {
            FitValue.X = x;
            FitValue.Y = y;
        }

        public void SetFitValue(double x, double y)
        {
            FitValue.X = Convert.ToSingle(x);
            FitValue.Y = Convert.ToSingle(y);
        }

        public void SetData(DigitizerPoint p)
        {
            Location.X = p.Location.X;
            Location.Y = p.Location.Y;
            Value.X = p.Value.X;
            Value.Y = p.Value.Y;
        }

        public double valueX
        {
            get { return Convert.ToDouble(Value.X); }
        }

        public double valueY
        {
            get { return Convert.ToDouble(Value.Y); }
        }

        public double deltaX
        {
            get { return Convert.ToDouble(Value.X - FitValue.X); }
        }

        public double deltaY
        {
            get { return Convert.ToDouble(Value.Y - FitValue.Y); }
        }

        public double deltaX2
        {
            get
            {
                double tmp = deltaX;
                return tmp * tmp;
            }
        }

        public double deltaY2
        {
            get
            {
                double tmp = deltaY;
                return tmp * tmp;
            }
        }

        public double deltaXp
        {
            get { return Convert.ToDouble(Value.X - PrefitValue.X); }
        }

        public double deltaYp
        {
            get { return Convert.ToDouble(Value.Y - PrefitValue.Y); }
        }

        public double deltaXp2
        {
            get
            {
                double tmp = deltaX2;
                return tmp * tmp;
            }
        }

        public double deltaYp2
        {
            get
            {
                double tmp = deltaY2;
                return tmp * tmp;
            }
        }

        public bool isPlottable
        {
            get
            {
                if (Location.X == int.MaxValue) return false;
                if (Location.Y == int.MaxValue) return false;
                if (float.IsNaN(Value.X)) return false;
                if (float.IsNaN(Value.Y)) return false;
                return true;
            }
        }

        public bool isFitted
        {
            get
            {
                if (Location.X == int.MaxValue) return false;
                if (Location.Y == int.MaxValue) return false;
                if (float.IsNaN(FitValue.X)) return false;
                if (float.IsNaN(FitValue.Y)) return false;
                if (float.IsNaN(PrefitValue.X)) return false;
                if (float.IsNaN(PrefitValue.Y)) return false;
                return true;
            }
        }
    }
}
