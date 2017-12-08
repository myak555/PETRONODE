using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Petronode.OilfieldFileAccess.LAS;

namespace Petronode.LogEnforce
{
    public class ParameterNumericWithinRule : Rule
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
        public ParameterNumericWithinRule(XmlNode node)
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
            string lc = las.GetParameter(Constant);
            if (lc.Length <= 0)
            {
                this.Status = Severity;
                this.Comment = "Parameter " + Constant + " is not defined";
                return;
            }
            double val = Double.NaN;
            try { val = Convert.ToDouble(lc); }
            catch (Exception) { };
            if (Double.IsNaN(val))
            {
                this.Status = Severity;
                this.Comment = "Parameter " + Constant + " is not set to numeric value, but to [" + lc + "]";
                return;
            }
            if (val == las.MissingValue)
            {
                this.Status = "Warning";
                this.Comment = "Parameter " + Constant + " is set to missing value [" + lc + "]";
                return;
            }
            if (val < MinValueError || MaxValueError < val)
            {
                this.Status = Severity;
                this.Comment = "Parameter " + Constant + " is set outside of range ["
                    + MinValueError.ToString() + ", " + MaxValueError.ToString() + "]";
                return;
            }
            if (val < MinValueWarning || MaxValueWarning < val)
            {
                this.Status = "Warning";
                this.Comment = "Parameter " + Constant + " is set outside of range ["
                    + MinValueWarning.ToString() + ", " + MaxValueWarning.ToString() + "]";
                return;
            }
            this.Status = "Pass";
            this.Comment = "Parameter " + Constant + " is within range ["
                + MinValueWarning.ToString() + ", " + MaxValueWarning.ToString() + "]";
            return;
        }
    }
}
