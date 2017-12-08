using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Petronode.OilfieldFileAccess.LAS;

namespace Petronode.LogEnforce
{
    public class Rule
    {
        public int Number = 0;
        public string Name = "undefined";
        public string Status = "NA";
        public string Comment = "";
        public string Severity = "Error";

        /// <summary>
        /// Creates an empty rule
        /// </summary>
        public Rule()
        {
        }

        /// <summary>
        /// Creates a rule from Xml description
        /// </summary>
        /// <param name="node">node to parse</param>
        public Rule(XmlNode node)
        {
            foreach( XmlNode xn in node.ChildNodes)
            {
                if (xn.LocalName == "Name") this.Name = xn.InnerText;
                if (xn.LocalName == "Comment") this.Comment = xn.InnerText;
                if (xn.LocalName == "Severity") this.Severity = xn.InnerText;
            }
        }

        /// <summary>
        /// Returns the Rule result as strings
        /// </summary>
        /// <returns></returns>
        public string[] ToStrings()
        {
            string[] tmp = new string[4];
            tmp[0] = Number.ToString();
            tmp[1] = Name;
            tmp[2] = Status;
            tmp[3] = Comment;
            return tmp;
        }

        /// <summary>
        /// Returns the Rule result as a single string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append( Number.ToString("0000") + " ");
            sb.Append(Status.PadRight(7) + " : ");
            sb.Append(Name + " : ");
            sb.Append( Comment);
            return sb.ToString();
        }

        /// <summary>
        /// Enforces the rule on LAS
        /// </summary>
        /// <param name="las">las file to check</param>
        public virtual void Enforce(LAS_File las)
        {
        }
    }
}
