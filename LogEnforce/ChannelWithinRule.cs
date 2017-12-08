using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Petronode.OilfieldFileAccess.LAS;

namespace Petronode.LogEnforce
{
    public class ChannelWithinRule : Rule
    {
        public string Channel = "undefined";
        public string Hole = "BOTH";
        public double MinValueError = Double.MinValue;
        public double MinValueWarning = Double.MinValue;
        public double MaxValueWarning = Double.MaxValue;
        public double MaxValueError = Double.MaxValue;
        public double SkipBelowCasing = 0.0;

        /// <summary>
        /// Creates a rule from Xml description
        /// </summary>
        /// <param name="node">node to parse</param>
        public ChannelWithinRule(XmlNode node)
            : base(node)
        {
            foreach (XmlNode xn in node.ChildNodes)
            {
                if (xn.LocalName == "Channel") this.Channel = xn.InnerText;
                if (xn.LocalName == "Hole") this.Hole = xn.InnerText.ToUpper();
                if (xn.LocalName == "SkipBelowCasing") this.SkipBelowCasing = Convert.ToDouble(xn.InnerText);
                if (xn.LocalName == "Values")
                {
                    foreach (XmlNode xnn in xn)
                    {
                        if (xnn.LocalName == "MinValueError") MinValueError = Convert.ToDouble(xnn.InnerText);
                        if (xnn.LocalName == "MinValueWarning") MinValueWarning = Convert.ToDouble(xnn.InnerText);
                        if (xnn.LocalName == "MaxValueWarning") MaxValueWarning = Convert.ToDouble(xnn.InnerText);
                        if (xnn.LocalName == "MaxValueError") MaxValueError = Convert.ToDouble(xnn.InnerText);
                    }
                }
            }
            if (MinValueWarning < MinValueError) MinValueWarning = MinValueError;
            if (MaxValueWarning > MaxValueError) MaxValueWarning = MaxValueError;
        }

        /// <summary>
        /// Enforces rule on file
        /// </summary>
        /// <param name="las">LAS file to check</param>
        public override void Enforce(LAS_File las)
        {
            LAS_Channel lc = (LAS_Channel)las.GetChannel(Channel);
            if (lc == null)
            {
                this.Status = "NA";
                this.Comment = "Channel " + Channel + " is not defined";
                return;
            }
            if (!lc.IsLoaded)
            {
                this.Status = Severity;
                this.Comment = "Channel " + Channel + " contains no data";
                return;
            }
            LAS_Channel index = (LAS_Channel)las.GetIndex();
            if (Hole != "OPEN" && Hole != "CASED")
            {
                EnforceInRange(lc, index, Double.MinValue, Double.MaxValue);
                return;
            }
            string csgString = las.GetParameter("CSGL");
            if (csgString.Length <= 0) csgString = las.GetParameter("CSGD");
            double csg = Double.NaN;
            try { csg = Convert.ToDouble(csgString); }
            catch (Exception) { }
            if (Double.IsNaN(csg))
            {
                EnforceInRange(lc, index, Double.MinValue, Double.MaxValue);
                return;
            }
            if (Hole == "OPEN") EnforceInRange(lc, index, csg + SkipBelowCasing, Double.MaxValue);
            else EnforceInRange(lc, index, Double.MinValue, csg);
        }

        private void EnforceInRange(LAS_Channel lc, LAS_Channel index, double indexRangeTop, double indexRangeBottom)
        {
            double min = Double.MaxValue;
            double minIndex = Double.NaN;
            double max = Double.MinValue;
            double maxIndex = Double.NaN;
            double topIndex = Double.MaxValue;
            double bottomIndex = Double.MinValue;
            for (int i = 0; i < lc.Data.Count; i++)
            {
                double d = index.Data[i];
                if (d < indexRangeTop || indexRangeBottom < d) continue;
                if( d < topIndex) topIndex = d;
                if( d > bottomIndex) bottomIndex = d;
                d = lc.Data[i];
                if (Double.IsNaN(d)) continue;
                if (d < min)
                {
                    min = d;
                    minIndex = index.Data[i];
                }
                if (d > max)
                {
                    max = d;
                    maxIndex = index.Data[i];
                }
            }
            if (Double.IsNaN(minIndex) || Double.IsNaN(maxIndex))
            {
                this.Status = Severity;
                this.Comment = "Channel " + Channel + " does not contain data between " 
                    + topIndex.ToString( "0.000") + " and " + bottomIndex.ToString( "0.000") + " " + index.Unit;
                return;
            }
            if (min < MinValueError)
            {
                this.Status = Severity;
                this.Comment = "Channel " + Channel + " reached minimum of ["
                    + min.ToString() + "] at " + minIndex.ToString("0.000") + " " + index.Unit +
                    ", while it should be in range ["
                    + MinValueError.ToString() + ", " + MaxValueError.ToString() + "]";
                return;
            }
            if (max > MaxValueError)
            {
                this.Status = Severity;
                this.Comment = "Channel " + Channel + " reached maximum of ["
                    + max.ToString() + "] at " + maxIndex.ToString("0.000") + " " + index.Unit +
                    ", while it should be in range ["
                    + MinValueError.ToString() + ", " + MaxValueError.ToString() + "]";
                return;
            }
            if (min < MinValueWarning)
            {
                this.Status = "Warning";
                this.Comment = "Channel " + Channel + " reached minimum of ["
                    + min.ToString() + "] at " + minIndex.ToString("0.000") + " " + index.Unit +
                    ", while it should be in range ["
                    + MinValueWarning.ToString() + ", " + MaxValueWarning.ToString() + "]";
                return;
            }
            if (max > MaxValueWarning)
            {
                this.Status = "Warning";
                this.Comment = "Channel " + Channel + " reached maximum of ["
                    + max.ToString() + "] at " + maxIndex.ToString("0.000") + " " + index.Unit +
                    ", while it should be in range ["
                    + MinValueWarning.ToString() + ", " + MaxValueWarning.ToString() + "]";
                return;
            }
            this.Status = "Pass";
            this.Comment = "Channel " + Channel + " is within range ["
                + min.ToString() + ", " + max.ToString() + "] between " 
                + topIndex.ToString( "0.000") + " and " + bottomIndex.ToString( "0.000") + " " + index.Unit;
            return;
        }
    }
}
