using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using Petronode.OilfieldFileAccess.LAS;

namespace Petronode.LogEnforce
{
    public class RuleList
    {
        private XmlDocument m_RulesDocument;
        public List<Rule> Rules = new List<Rule>();
        public string Version = "undefined";

        /// <summary>
        /// Creates an empty set of rules
        /// </summary>
        public RuleList()
        {
        }

        /// <summary>
        /// Reads a rules list from XML file
        /// </summary>
        /// <param name="filename">file name</param>
        public RuleList(string filename)
        {
            if (!File.Exists(filename)) throw new Exception("File does not exist: " + filename);
            StreamReader reader = File.OpenText(filename);
            m_RulesDocument = new XmlDocument();
            m_RulesDocument.Load(reader.BaseStream);
            reader.Close();
            foreach (XmlNode xn in m_RulesDocument.ChildNodes)
            {
                if (xn.NodeType == XmlNodeType.XmlDeclaration) continue;
                ParseBody(xn);
            }
        }

        /// <summary>
        /// Enforces rules on las file
        /// </summary>
        /// <param name="las">LAS fiel to check</param>
        public void Enforce(LAS_File las)
        {
            foreach (Rule r in Rules) r.Enforce(las);
        }

        /// <summary>
        /// Returns pass rules count
        /// </summary>
        /// <returns></returns>
        public int GetPassCount()
        {
            int count = 0;
            foreach (Rule r in Rules) if (r.Status == "Pass") count++;
            return count;
        }

        /// <summary>
        /// Returns warning rules count
        /// </summary>
        /// <returns></returns>
        public int GetWarningCount()
        {
            int count = 0;
            foreach (Rule r in Rules) if (r.Status == "Warning") count++;
            return count;
        }

        /// <summary>
        /// Returns error rules count
        /// </summary>
        /// <returns></returns>
        public int GetErrorCount()
        {
            int count = 0;
            foreach (Rule r in Rules) if (r.Status == "Error") count++;
            return count;
        }

        private void ParseBody(XmlNode node)
        {
            foreach (XmlNode xn in node.ChildNodes)
            {
                if (xn.LocalName == "Version")
                {
                    this.Version = xn.InnerText;
                    continue;
                }
                if (xn.LocalName == "Rules")
                {
                    ParseRules( xn);
                    continue;
                }
                throw new Exception("Unknown node: " + xn.LocalName);
            }
        }

        private void ParseRules(XmlNode node)
        {
            foreach (XmlNode xn in node.ChildNodes)
            {
                if (xn.LocalName != "Rule") continue;
                Rule tmp = CreateRule(xn);
                tmp.Number = Rules.Count + 1;
                Rules.Add(tmp);
            }
        }

        private Rule CreateRule(XmlNode node)
        {
            foreach (XmlNode xn in node.ChildNodes)
            {
                if (xn.LocalName != "Type") continue;
                switch (xn.InnerText)
                {
                    case "NamingConvention": return new NamingConventionRule(node);
                    case "ConstantSet": return new ConstantSetRule(node);
                    case "ParameterSet": return new ParameterSetRule(node);
                    case "ConstantEquals": return new ConstantEqualsRule(node);
                    case "ParameterEquals": return new ParameterEqualsRule(node);
                    case "ConstantOneOf": return new ConstantOneOfRule(node);
                    case "ParameterOneOf": return new ParameterOneOfRule(node);
                    case "ConstantStartsWithOneOf": return new ConstantStartsWithOneOfRule(node);
                    case "ParameterStartsWithOneOf": return new ParameterStartsWithOneOfRule(node);
                    case "ConstantContainsOneOf": return new ConstantContainsOneOfRule(node);
                    case "ParameterContainsOneOf": return new ParameterContainsOneOfRule(node);
                    case "ConstantUnit": return new ConstantUnitRule(node);
                    case "ParameterUnit": return new ParameterUnitRule(node);
                    case "ConstantNumericWithin": return new ConstantNumericWithinRule(node);
                    case "ParameterNumericWithin": return new ParameterNumericWithinRule(node);
                    case "ConstantCompare": return new ConstantCompareRule(node);
                    case "ParameterCompare": return new ParameterCompareRule(node);
                    case "ConstantLatitudeWithin": return new ConstantLatitudeWithinRule(node);
                    case "ConstantLongitudeWithin": return new ConstantLongitudeWithinRule(node);
                    case "ChannelPresent": return new ChannelPresentRule(node);
                    case "ChannelUnit": return new ChannelUnitRule(node);
                    case "ChannelUnitOneOf": return new ChannelUnitOneOfRule(node);
                    case "ChannelWithin": return new ChannelWithinRule(node);
                    case "ChannelAverageWithin": return new ChannelAverageWithinRule(node);
                    case "ChannelCoverage": return new ChannelCoverageRule(node);
                    default: return new Rule(node);
                }
            }
            throw new Exception("Node does not contain check type: " + node.ToString());
        }
    }
}
