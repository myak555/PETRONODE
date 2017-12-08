using System;
using System.Collections.Generic;
using System.Text;

namespace Petronode.VendorFileAccess.ASC
{
    /// <summary>
    /// Reversed - engineered ASC v 1.2 sensor record
    /// </summary>
    public class SensorDescription
    {
        private char[] c_Separators = { '\t', ' '};

        public bool Valid = false;
        public int ChannelNumber = 0;
        public double Northing = 0.0;
        public double Easting = 0.0;
        public double DepthSRD = 0.0;
        public bool IsActive = false;
        public string Type = "00000";
        public double Gain = 0.0;
        public double Sensitivity = 0.0;
        public double FullScale = 0.0;
        public double LowFrequency = 0.0;
        public double HighFrequency = 0.0;
        public double iN = 0.0;
        public double iE = 0.0;
        public double iD = 0.0;
        public int Polarization = 1;
        public double PStationCorrection = 0.0;
        public double SStationCorrection = 0.0;

        public double MD = 0.0;

        /// <summary>
        /// Creates an empty sensor description
        /// </summary>
        public SensorDescription()
        {
        }
        
        /// <summary>
        /// Creates a sensor description from string
        /// </summary>
        /// <param name="s"></param>
        public SensorDescription(string s)
        {
            try
            {
                string[] ss = s.Split(c_Separators, StringSplitOptions.RemoveEmptyEntries);
                if (!ss[0].StartsWith("Channel")) return;
                if (ss.Length < 18) return;
                ChannelNumber = Convert.ToInt32(ss[1]);
                Northing = Convert.ToDouble(ss[2]);
                Easting = Convert.ToDouble(ss[3]);
                DepthSRD = Convert.ToDouble(ss[4]);
                IsActive = ss[5].StartsWith("1");
                Type = ss[6];
                Gain = Convert.ToDouble(ss[7]);
                Sensitivity = Convert.ToDouble(ss[8]);
                FullScale = Convert.ToDouble(ss[9]);
                LowFrequency = Convert.ToDouble(ss[10]);
                HighFrequency = Convert.ToDouble(ss[11]);
                iN = Convert.ToDouble(ss[12]);
                iE = Convert.ToDouble(ss[13]);
                iD = Convert.ToDouble(ss[14]);
                Polarization = Convert.ToInt32(ss[15]);
                PStationCorrection = Convert.ToDouble(ss[16]);
                SStationCorrection = Convert.ToDouble(ss[17]);
                Valid = true;
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Converts the sensor data to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Channel ");
            sb.Append(ChannelNumber.ToString("0").PadLeft(2, '0') + "\t");
            sb.Append(Northing.ToString("0.00").PadLeft(6, '0') + "\t");
            sb.Append(Easting.ToString("0.00").PadLeft(6, '0') + "\t");
            sb.Append(DepthSRD.ToString("0.00").PadLeft(6, '0') + "\t");
            sb.Append(IsActive ? "1\t" : "0\t");
            sb.Append(Type + "\t");
            sb.Append(Gain.ToString("0.000").PadLeft(6, '0') + "\t");
            sb.Append(Sensitivity.ToString("0.00").PadLeft(6, '0') + "\t");
            sb.Append(FullScale.ToString("0.00").PadLeft(6, '0') + "\t");
            sb.Append(LowFrequency.ToString("0.00").PadLeft(6, '0') + "\t");
            sb.Append(HighFrequency.ToString("0.00").PadLeft(6, '0') + "\t");
            sb.Append(iN.ToString("0.00000") + "\t");
            sb.Append(iE.ToString("0.00000") + "\t");
            sb.Append(iD.ToString("0.00000") + "\t");
            sb.Append((Polarization>=0)? "1\t": "-1\t");
            sb.Append(PStationCorrection.ToString("0.00000") + "\t");
            sb.Append(SStationCorrection.ToString("0.00000"));
            return sb.ToString();
        }

        /// <summary>
        /// Converts the sensor to set of strings for representation
        /// </summary>
        /// <returns></returns>
        public string[] ToStrings()
        {
            string[] tmp = new string[17];
            tmp[0] = ChannelNumber.ToString("0").PadLeft(2,'0');
            tmp[1] = Northing.ToString("0.00");
            tmp[2] = Easting.ToString("0.00");
            tmp[3] = DepthSRD.ToString("0.00");
            tmp[4] = IsActive ? "True" : "False";
            tmp[5] = Type;
            tmp[6] = Gain.ToString("0.000");
            tmp[7] = Sensitivity.ToString("0.000");
            tmp[8] = FullScale.ToString("0.000");
            tmp[9] = LowFrequency.ToString("0.0");
            tmp[10] = HighFrequency.ToString("0.0");
            tmp[11] = iN.ToString("0.00000");
            tmp[12] = iE.ToString("0.00000");
            tmp[13] = iD.ToString("0.00000");
            tmp[14] = Polarization.ToString("0");
            tmp[15] = PStationCorrection.ToString("0.00000");
            tmp[16] = SStationCorrection.ToString("0.00000");
            return tmp;
        }

        /// <summary>
        /// Action, reversed from ToStrings
        /// Exceptions are sunk
        /// </summary>
        public void FromStrings(string[] tmp)
        {
            try{ if( tmp[0] != " ") ChannelNumber = Convert.ToInt32( tmp[0]);}
            catch( Exception){}
            try { if (tmp[1] != " ") Northing = Convert.ToDouble(tmp[1]); }
            catch (Exception) { }
            try { if (tmp[2] != " ") Easting = Convert.ToDouble(tmp[2]); }
            catch (Exception) { }
            try { if (tmp[3] != " ") DepthSRD = Convert.ToDouble(tmp[3]); }
            catch (Exception) { }
            try { if (tmp[4] != " ") IsActive = !tmp[4].StartsWith("F"); }
            catch (Exception) { }
            try { if (tmp[5] != " ") Type = tmp[5]; }
            catch (Exception) { }
            try { if (tmp[6] != " ") Gain = Convert.ToDouble(tmp[6]); }
            catch (Exception) { }
            try { if (tmp[7] != " ") Sensitivity = Convert.ToDouble(tmp[7]); }
            catch (Exception) { }
            try { if (tmp[8] != " ") FullScale = Convert.ToDouble(tmp[8]); }
            catch (Exception) { }
            try { if (tmp[9] != " ") LowFrequency = Convert.ToDouble(tmp[9]); }
            catch (Exception) { }
            try { if (tmp[10] != " ") HighFrequency = Convert.ToDouble(tmp[10]); }
            catch (Exception) { }
            try { if (tmp[11] != " ") iN = Convert.ToDouble(tmp[11]); }
            catch (Exception) { }
            try { if (tmp[12] != " ") iE = Convert.ToDouble(tmp[12]); }
            catch (Exception) { }
            try { if (tmp[13] != " ") iD = Convert.ToDouble(tmp[13]); }
            catch (Exception) { }
            try { if (tmp[14] != " ") Polarization = (tmp[14].StartsWith("-")) ? -1 : 1; }
            catch (Exception) { }
            try { if (tmp[15] != " ") PStationCorrection = Convert.ToDouble(tmp[15]); }
            catch (Exception) { }
            try { if (tmp[16] != " ") SStationCorrection = Convert.ToDouble(tmp[16]); }
            catch (Exception) { }
        }

        /// <summary>
        /// Converts coordinates to Metric (m)
        /// </summary>
        public void ToMetric()
        {
            Northing *= 0.3048;
            Easting *= 0.3048;
            DepthSRD *= 0.3048;
        }

        /// <summary>
        /// Converts coordinates to Imperial (ft) 
        /// </summary>
        public void ToImperial()
        {
            Northing /= 0.3048;
            Easting /= 0.3048;
            DepthSRD /= 0.3048;
        }
    }
}
