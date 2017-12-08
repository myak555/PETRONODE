using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Petronode.OilfieldFileAccess.LAS;

namespace Petronode.LogEnforce
{
    public class ParameterUnitRule : Rule
    {
        public string Constant = "undefined";
        public string Value = "undefined";

        /// <summary>
        /// Creates a rule from Xml description
        /// </summary>
        /// <param name="node">node to parse</param>
        public ParameterUnitRule(XmlNode node)
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
            foreach (LAS_Constant lc in las.Parameters)
            {
                if (lc.Name.ToUpper() != Constant.ToUpper()) continue;
                if (lc.Unit == Value)
                {
                    this.Status = "Pass";
                    this.Comment = "Parameter " + Constant + " has unit [" + Value + "]";
                    return;
                }
                if (lc.Unit.ToUpper() == Value.ToUpper())
                {
                    this.Status = "Warning";
                    this.Comment = "Parameter " + Constant + " has unit [" + lc.Unit + "], but [" + Value + "] is required.";
                    return;
                }
                this.Status = Severity;
                this.Comment = "Parameter " + Constant + " has unit [" + lc.Unit + "], but [" + Value + "] is required.";
                return;
            }
            this.Status = "NA";
            this.Comment = "Parameter " + Constant + " is not defined.";
            return;
        }
    }
}
