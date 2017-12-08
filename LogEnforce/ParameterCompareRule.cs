using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Petronode.OilfieldFileAccess.LAS;

namespace Petronode.LogEnforce
{
    public class ParameterCompareRule : Rule
    {
        public string Constant = "undefined";
        public string CompareWith = "undefined";
        public string Comparison = "EQUAL";

        /// <summary>
        /// Creates a rule from Xml description
        /// </summary>
        /// <param name="node">node to parse</param>
        public ParameterCompareRule(XmlNode node)
            : base(node)
        {
            foreach (XmlNode xn in node.ChildNodes)
            {
                if (xn.LocalName == "Constant") this.Constant = xn.InnerText;
                if (xn.LocalName == "CompareWith") this.CompareWith = xn.InnerText;
                if (xn.LocalName == "Comparison") this.Comparison = xn.InnerText.ToUpper();
            }
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
            string lc2 = las.GetParameter(CompareWith);
            if (lc2.Length <= 0) lc2 = las.GetConstant(CompareWith);
            if (lc2.Length <= 0)
            {
                this.Status = Severity;
                this.Comment = "Parameter " + CompareWith + " is not defined";
                return;
            }
            double val = Double.NaN;
            double val2 = Double.NaN;
            try
            {
                val = Convert.ToDouble(lc);
                val2 = Convert.ToDouble(lc2);
            }
            catch (Exception)
            {
                this.Status = Severity;
                this.Comment = "Parameters " + Constant + " and/or " + CompareWith + " are not numeric";
                return;
            }
            if (val == -999.25 || val2 == -999.25)
            {
                this.Status = Severity;
                this.Comment = "Parameters " + Constant + " and/or " + CompareWith + " are set to missing";
                return;
            }
            if (Comparison == "LESS" && val >= val2)
            {
                this.Status = Severity;
                this.Comment = "Parameter " + Constant + " is more or equal to " + CompareWith +
                    " [" + val + ">=" + val2 + "]";
                return;
            }
            if (Comparison == "LESS_OR_EQUAL" && val > val2)
            {
                this.Status = Severity;
                this.Comment = "Parameter " + Constant + " is more than " + CompareWith +
                    " [" + val + ">" + val2 + "]";
                return;
            }
            if (Comparison == "EQUAL" && val != val2)
            {
                this.Status = Severity;
                this.Comment = "Parameter " + Constant + " is not equal to " + CompareWith +
                    " [" + val + "!=" + val2 + "]";
                return;
            }
            if (Comparison == "MORE_OR_EQUAL" && val < val2)
            {
                this.Status = Severity;
                this.Comment = "Parameter " + Constant + " is less than " + CompareWith +
                    " [" + val + "<" + val2 + "]";
                return;
            }
            if (Comparison == "MORE" && val <= val2)
            {
                this.Status = Severity;
                this.Comment = "Parameter " + Constant + " is less or equal to  " + CompareWith +
                    " [" + val + "<=" + val2 + "]";
                return;
            }
            if (Comparison == "NOT_EQUAL" && val == val2)
            {
                this.Status = Severity;
                this.Comment = "Parameter " + Constant + " is equal to " + CompareWith +
                    " [" + val + "==" + val2 + "]";
                return;
            }
            this.Status = "Pass";
            this.Comment = "Parameter " + Constant + " is " + Comparison + " than/to " + CompareWith;
            return;
        }
    }
}
