using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Petronode.OilfieldFileAccess.LAS;

namespace Petronode.LogEnforce
{
    public class ChannelUnitRule : Rule
    {
        public string Channel = "undefined";
        public string Value = "undefined";

        /// <summary>
        /// Creates a rule from Xml description
        /// </summary>
        /// <param name="node">node to parse</param>
        public ChannelUnitRule(XmlNode node)
            : base(node)
        {
            foreach (XmlNode xn in node.ChildNodes)
            {
                if (xn.LocalName == "Channel") this.Channel = xn.InnerText;
                if (xn.LocalName == "Value") this.Value = xn.InnerText;
            }
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
                this.Comment = "Channel " + Channel + " is not defined.";
                return;
            }
            if (lc.Unit == Value)
            {
                this.Status = "Pass";
                this.Comment = "Channel " + Channel + " has unit [" + Value + "]";
                return;
            }
            if (lc.Unit.ToUpper() == Value.ToUpper())
            {
                this.Status = "Warning";
                this.Comment = "Channel " + Channel + " has unit [" + lc.Unit + "], but [" + Value + "] is required.";
                return;
            }
            this.Status = Severity;
            this.Comment = "Channel " + Channel + " has unit [" + lc.Unit + "], but [" + Value + "] is required.";
            return;
        }
    }
}
