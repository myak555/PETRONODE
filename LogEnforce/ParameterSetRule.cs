using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Petronode.OilfieldFileAccess.LAS;

namespace Petronode.LogEnforce
{
    public class ParameterSetRule: Rule
    {
        public string Constant = "undefined";

        /// <summary>
        /// Creates a rule from Xml description
        /// </summary>
        /// <param name="node">node to parse</param>
        public ParameterSetRule(XmlNode node)
            : base(node)
        {
            foreach (XmlNode xn in node.ChildNodes)
            {
                if (xn.LocalName == "Constant") this.Constant = xn.InnerText;
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
            this.Status = "Pass";
            this.Comment = "Parameter " + Constant + " is set to [" + lc + "]";
            return;
        }
    }
}
