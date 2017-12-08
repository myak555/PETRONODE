using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Petronode.OilfieldFileAccess.LAS;
using Petronode.OilfieldFileAccess.Converters;

namespace Petronode.LogEnforce
{
    public class ConstantLatitudeWithinRule : Rule
    {
        public string Constant = "undefined";
        public double MinValueError = Double.MinValue;
        public double MinValueWarning = Double.MinValue;
        public double MaxValueWarning = Double.MaxValue;
        public double MaxValueError = Double.MaxValue;

        /// <summary>
        /// Creates a rule from Xml description
        /// </summary>
        /// <param name="node">node to parse</param>
        public ConstantLatitudeWithinRule(XmlNode node)
            : base(node)
        {
            foreach (XmlNode xn in node.ChildNodes)
            {
                if (xn.LocalName == "Constant") this.Constant = xn.InnerText;
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
            string lc = las.GetConstant(Constant);
            if (lc.Length <= 0)
            {
                this.Status = Severity;
                this.Comment = "Constant " + Constant + " is not defined";
                return;
            }
            Latitude lat = null;
            try
            {
                lat = new Latitude( ReplaceFunnyChars( lc));
            }
            catch( Exception ex)
            {
                this.Status = Severity;
                this.Comment = "Constant " + Constant + " with value [" + lc + "] could not be read. " + ex.Message;
                return;
            }
            double val = lat.AngleD;
            if (Double.IsNaN(val))
            {
                this.Status = Severity;
                this.Comment = "Constant " + Constant + " has incorrect value [" + lc + "]";
                return;
            }
            if (val < MinValueError || MaxValueError < val)
            {
                this.Status = Severity;
                this.Comment = "Constant " + Constant + " [" + val.ToString("0.0000") + "] is set outside of range ["
                    + MinValueError.ToString() + ", " + MaxValueError.ToString() + "]";
                return;
            }
            if (val < MinValueWarning || MaxValueWarning < val)
            {
                this.Status = "Warning";
                this.Comment = "Constant " + Constant + " [" + val.ToString("0.0000") + "] is set outside of range ["
                    + MinValueWarning.ToString() + ", " + MaxValueWarning.ToString() + "]";
                return;
            }
            this.Status = "Pass";
            this.Comment = "Constant " + Constant + " [" + val.ToString("0.0000") + "] is within range ["
                + MinValueWarning.ToString() + ", " + MaxValueWarning.ToString() + "]";
            return;
        }

        /// <summary>
        /// Removes degrees and other funny chars from the LAT-LONG strings
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string ReplaceFunnyChars(string s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c < ' ' || c > 'z') c = ' ';
                sb.Append(c);
            }
            return sb.ToString().Replace("DMS", "");
        }
    }
}
