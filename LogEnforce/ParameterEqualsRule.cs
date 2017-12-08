using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Petronode.OilfieldFileAccess.LAS;

namespace Petronode.LogEnforce
{
    public class ParameterEqualsRule: Rule
    {
        public string Constant = "undefined";
        public string Value = "undefined";

        /// <summary>
        /// Creates a rule from Xml description
        /// </summary>
        /// <param name="node">node to parse</param>
        public ParameterEqualsRule(XmlNode node)
            : base(node)
        {
            foreach (XmlNode xn in node.ChildNodes)
            {
                if (xn.LocalName == "Constant") this.Constant = xn.InnerText;
                if (xn.LocalName == "Value") this.Value = xn.InnerText;
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
            if (lc == this.Value)
            {
                this.Status = "Pass";
                this.Comment = "Parameter " + Constant + " is set to [" + this.Value + "]";
                return;
            }
            if (lc.ToUpper() == this.Value.ToUpper())
            {
                this.Status = "Warning";
                this.Comment = "Parameter " + Constant + " is set to [" + lc + "] but [" + this.Value + "] is required.";
                return;
            }
            this.Status = Severity;
            this.Comment = "Parameter " + Constant + " is set to [" + lc + "] but [" + this.Value + "] is required.";
            return;
        }
    }
}
