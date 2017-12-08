using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Petronode.OilfieldFileAccess;

namespace Petronode.OilfieldFileAccess.Petrolog
{
    /// <summary>
    /// Describes the Petrolog 10.x constant type
    /// </summary>
    public class Petrolog_Constant: Oilfield_Constant
    {
        private XmlNode m_Node = null;

        /// <summary>
        /// Constructor - creates a default variable
        /// </summary>
        public Petrolog_Constant(): base()
        {
        }

        /// <summary>
        /// Constructor - creates from the parameters
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="unit">Variable unit</param>
        /// <param name="value">Variable value</param>
        /// <param name="description">Description</param>
        public Petrolog_Constant(string name, string unit, string value, string description):
            base(name, unit, value, description)
        {
        }

        /// <summary>
        /// Creates the variable from the input
        /// </summary>
        public Petrolog_Constant(XmlNode node)
        {
            m_Node = node;
            //m_Parent = parent;
            foreach (XmlNode xn in node.ChildNodes)
            {
                if (xn.LocalName == "Mnem") Name = xn.InnerText;
                if (xn.LocalName == "Value") Value = xn.InnerText;
                if (xn.LocalName == "Unit") Unit = xn.InnerText;
                if (xn.LocalName == "Description") Description = xn.InnerText;
            }
        }

        /// <summary>
        /// Converts back to XML string
        /// </summary>
        /// <returns>Converted string</returns>
        public string ToString( string prefix)
        {
            StringBuilder sb = new StringBuilder( prefix);
            sb.Append( "<HeaderField>\r\n");
            sb.Append( prefix);
            sb.Append( "  <Mnem>");
            sb.Append( Name);
            sb.Append("</Mnem>\r\n");
            sb.Append( prefix);
            if( Unit.Length > 0)
            {
                sb.Append( "  <Unit>");
                sb.Append( Unit);
                sb.Append("</Unit>\r\n");
            }
            else
            {
                sb.Append("  <Unit />\r\n");
            }
            sb.Append( prefix);
            if( Value.Length > 0)
            {
                sb.Append( "  <Value>");
                string deBang = Value.Replace("<", "&lt;");
                deBang = deBang.Replace(">", "&gt;");
                sb.Append( deBang);
                sb.Append("</Value>\r\n");
            }
            else
            {
                sb.Append("  <Value />\r\n");
            }
            //if( Description.Length > 0)
            //{
            //    sb.Append( prefix);
            //    sb.Append( "  <Description>");
            //    sb.Append( Description);
            //    sb.Append("</Description>\r\n");
            //}
            sb.Append( prefix);
            sb.Append("</HeaderField>\r\n");
            return sb.ToString();
        }

        /// <summary>
        /// Creats a deep copy of this constant
        /// </summary>
        /// <returns></returns>
        public new Petrolog_Constant Clone()
        {
            Petrolog_Constant tmp = new Petrolog_Constant();
            tmp.m_Node = this.m_Node.Clone();
            tmp.Name = this.Name;
            tmp.Unit = this.Unit;
            tmp.Value = this.Value;
            tmp.Description = this.Description;
            return tmp;
        }
    }
}
