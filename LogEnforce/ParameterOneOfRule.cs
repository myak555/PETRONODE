using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Petronode.OilfieldFileAccess.LAS;

namespace Petronode.LogEnforce
{
    public class ParameterOneOfRule: Rule
    {
        public string Constant = "undefined";
        public List<string> Values = new List<string>();

        /// <summary>
        /// Creates a rule from Xml description
        /// </summary>
        /// <param name="node">node to parse</param>
        public ParameterOneOfRule(XmlNode node)
            : base(node)
        {
            foreach (XmlNode xn in node.ChildNodes)
            {
                if (xn.LocalName == "Constant") this.Constant = xn.InnerText;
                if (xn.LocalName == "Values")
                {
                    foreach (XmlNode xnn in xn)
                    {
                        if (xnn.LocalName != "Value") continue;
                        Values.Add(xnn.InnerText);
                    }
                }
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
            string ret = FindValue(lc);
            if (ret.Length > 0)
            {
                this.Status = "Pass";
                this.Comment = "Parameter " + Constant + " is set to [" + lc + "]";
                return;
            }
            ret = FindValueUpper( lc.ToUpper());
            if (ret.Length > 0)
            {
                this.Status = "Warning";
                this.Comment = "Parameter " + Constant + " is set to [" + lc + "] but [" + ret + "] is required.";
                return;
            }
            this.Status = Severity;
            StringBuilder sb = new StringBuilder();
            sb.Append("Parameter " + Constant + " is set to [" + lc + "] but one of [");
            for( int i=0; i<Values.Count; i++)
            {
                sb.Append(Values[i]);
                if (i < Values.Count - 1) sb.Append(", ");
            }
            sb.Append( "] is required.");
            Comment = sb.ToString();
            return;
        }

        private string FindValue(string val)
        {
            foreach (string s in Values)
            {
                if (s == val) return s;
            }
            return "";
        }

        private string FindValueUpper(string val)
        {
            foreach (string s in Values)
            {
                if (s.ToUpper() == val) return s;
            }
            return "";
        }
    }
}
