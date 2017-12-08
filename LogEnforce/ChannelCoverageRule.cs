using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Petronode.OilfieldFileAccess.LAS;

namespace Petronode.LogEnforce
{
    public class ChannelCoverageRule : Rule
    {
        public string Channel = "undefined";
        public string Hole = "BOTH";
        public double MinValueError = Double.MinValue;
        public double MinValueWarning = Double.MinValue;
        public double MaxValueWarning = Double.MaxValue;
        public double MaxValueError = Double.MaxValue;

        /// <summary>
        /// Creates a rule from Xml description
        /// </summary>
        /// <param name="node">node to parse</param>
        public ChannelCoverageRule(XmlNode node)
            : base(node)
        {
            foreach (XmlNode xn in node.ChildNodes)
            {
                if (xn.LocalName == "Channel") this.Channel = xn.InnerText;
                if (xn.LocalName == "Hole") this.Hole = xn.InnerText.ToUpper();
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
            if (Hole == "OPEN") EnforceInRange(lc, index, csg, Double.MaxValue);
            else EnforceInRange(lc, index, Double.MinValue, csg);
        }

        private void EnforceInRange(LAS_Channel lc, LAS_Channel index, double indexRangeTop, double indexRangeBottom)
        {
            int totalCount = 0;
            int validCount = 0;
            double topIndex = Double.MaxValue;
            double bottomIndex = Double.MinValue;
            for (int i = 0; i < lc.Data.Count; i++)
            {
                double d = index.Data[i];
                if (d < indexRangeTop || indexRangeBottom < d) continue;
                if (d < topIndex) topIndex = d;
                if (d > bottomIndex) bottomIndex = d;
                d = lc.Data[i];
                totalCount++;
                if (!Double.IsNaN(d)) validCount++;
            }
            if (totalCount <= 0)
            {
                this.Status = Severity;
                this.Comment = "Channel " + Channel + " does not contain data between "
                    + topIndex.ToString("0.000") + " and " + bottomIndex.ToString("0.000") + " " + index.Unit;
                return;
            }
            double coverage = Convert.ToDouble(validCount) * 100.0 / Convert.ToDouble(totalCount);
            if (coverage < MinValueError || MaxValueError < coverage)
            {
                this.Status = Severity;
                this.Comment = "Channel " + Channel + " has a coverage of "
                    + coverage.ToString( "0.00") + "% in " + totalCount.ToString("0")
                    + " samples, while it should be in range ["
                    + MinValueError.ToString() + ", " + MaxValueError.ToString() + "]";
                return;
            }
            if (coverage < MinValueWarning || MaxValueWarning < coverage)
            {
                this.Status = Severity;
                this.Comment = "Channel " + Channel + " has a coverage of "
                    + coverage.ToString("0.00") + "% in " + totalCount.ToString("0")
                    + " samples, while it should be in range ["
                    + MinValueWarning.ToString() + ", " + MaxValueWarning.ToString() + "]";
                return;
            }
            this.Status = "Pass";
            this.Comment = "Channel " + Channel + " has a coverage of "
                + coverage.ToString("0.00") + "% in " + totalCount.ToString("0")
                + " samples from "
                + topIndex.ToString("0.000") + " and " + bottomIndex.ToString("0.000") + " " + index.Unit;
            return;
        }
    }
}
