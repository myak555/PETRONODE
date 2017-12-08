using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Petronode.OilfieldFileAccess.LAS;

namespace Petronode.LogEnforce
{
    public class ChannelPresentRule : Rule
    {
        public string Channel = "undefined";

        /// <summary>
        /// Creates a rule from Xml description
        /// </summary>
        /// <param name="node">node to parse</param>
        public ChannelPresentRule(XmlNode node)
            : base(node)
        {
            foreach (XmlNode xn in node.ChildNodes)
            {
                if (xn.LocalName == "Channel") this.Channel = xn.InnerText;
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
                this.Status = Severity;
                this.Comment = "Channel " + Channel + " is not defined";
                return;
            }
            this.Status = "Pass";
            this.Comment = "Channel " + Channel + " is present";
            return;
        }
    }
}
