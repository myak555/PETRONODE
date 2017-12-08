using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using Petronode.OilfieldFileAccess.LAS;

namespace Petronode.LogEnforce
{
    public class NamingConventionRule: Rule
    {
        public string Capitals = "undefined";
        public string Underscore = "undefined";
        public List<string> Values = new List<string>();

        /// <summary>
        /// Creates a rule from Xml description
        /// </summary>
        /// <param name="node">node to parse</param>
        public NamingConventionRule(XmlNode node)
            : base(node)
        {
            foreach (XmlNode xn in node.ChildNodes)
            {
                if (xn.LocalName == "Capitals") this.Capitals = xn.InnerText.ToUpper();
                if (xn.LocalName == "Underscore") this.Underscore = xn.InnerText.ToUpper();
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
            FileInfo fi = new FileInfo( las.FileName);
            string filename = fi.Name;
            StringBuilder sb = new StringBuilder();
            foreach (string s in Values)
            {
                string lc = las.GetConstant(s);
                if (lc.Length <= 0) lc = las.GetParameter(s);
                if (lc.Length > 0) sb.Append( lc);
                else sb.Append( s);
            }
            string mask = (Capitals == "YES")? sb.ToString().ToUpper(): sb.ToString();
            if (Underscore == "YES") mask = mask.Replace(' ', '_');

            if (filename.StartsWith( mask))
            {
                if (Underscore == "YES" && filename.Contains(" "))
                {
                    this.Status = "Warning";
                    this.Comment = "File name should not contain spaces: " + filename;
                    return;
                }
                this.Status = "Pass";
                this.Comment = "Naming convention is followed in " + filename;
                return;
            }
            if (Capitals == "YES" && filename.ToUpper().StartsWith(mask))
            {
                this.Status = "Warning";
                this.Comment = "File name must be in capitals: " + filename;
                return;
            }
            this.Status = Severity;
            this.Comment = "Naming convention is not followed in file " + filename + ". Should start with [" + mask + "]";
            return;
        }
    }
}
