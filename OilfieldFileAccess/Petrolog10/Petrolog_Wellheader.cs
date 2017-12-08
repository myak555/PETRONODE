using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Petronode.OilfieldFileAccess.Petrolog
{
    public class Petrolog_Wellheader
    {
        private XmlDocument m_HeaderDocument;
        
        /// <summary>
        /// Version string
        /// </summary>
        public string Version = "10.7";

        /// <summary>
        /// General constants list
        /// </summary>
        public List<Petrolog_Constant> General = new List<Petrolog_Constant>();

        /// <summary>
        /// Remarks constants list
        /// </summary>
        public List<Petrolog_Constant> Remarks = new List<Petrolog_Constant>();

        /// <summary>
        /// Recursive wellheaders for runs
        /// </summary>
        public List<Petrolog_Wellheader> Runs = new List<Petrolog_Wellheader>();


        #region Constructors
        /// <summary>
        /// Creates an empty wellheader
        /// </summary>
        public Petrolog_Wellheader()
        {
        }

        /// <summary>
        /// Creates a wellheader from the file given
        /// </summary>
        /// <param name="name">Name of file to parse</param>
        public Petrolog_Wellheader(string name)
        {
            if (!File.Exists(name)) return;
            StreamReader reader = File.OpenText(name);
            m_HeaderDocument = new XmlDocument();
            m_HeaderDocument.Load(reader.BaseStream);
            reader.Close();
            foreach (XmlNode xn in m_HeaderDocument.ChildNodes)
            {
                if (xn.NodeType == XmlNodeType.XmlDeclaration) continue;
                ParseWellHeaderContents(xn);
            }
        }

        /// <summary>
        /// Creates a wellheader from the Run inner nodes
        /// </summary>
        /// <param name="name">Name of file to parse</param>
        public Petrolog_Wellheader(XmlNode node)
        {
            ParseGeneral(node);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Parses header contents
        /// </summary>
        private void ParseWellHeaderContents(XmlNode node)
        {
            foreach (XmlNode xn in node.ChildNodes)
            {
                if (xn.LocalName == "Version")
                {
                    Version = xn.InnerText;
                    continue;
                }
                if (xn.LocalName == "General")
                {
                    ParseGeneral(xn);
                    continue;
                }
                if (xn.LocalName == "Runs")
                {
                    ParseRuns(xn);
                    continue;
                }
                if (xn.LocalName == "Remarks")
                {
                    ParseRemarks(xn);
                    continue;
                }
                if (xn.LocalName == "StrataDefaultsFileName")
                {
                    ParseStrataDefaultsFileName(xn);
                    continue;
                }
                if (xn.LocalName == "Stratas")
                {
                    ParseStratas(xn);
                    continue;
                }
                if (xn.LocalName == "FormationTestInfos")
                {
                    ParseFormationTestInfos(xn);
                    continue;
                }
                if (xn.LocalName == "DevSurveyData")
                {
                    ParseDevSurveyData(xn);
                    continue;
                }
                if (xn.LocalName == "DiscreteData")
                {
                    ParseDiscreteData(xn);
                    continue;
                }
            }
            return;
        }

        private void ParseGeneral(XmlNode inp)
        {
            foreach (XmlNode xn in inp.ChildNodes)
            {
                if (xn.LocalName != "HeaderField") continue;
                AddConstant(xn, General);
            }
        }
        
        private void ParseRuns(XmlNode inp)
        {
            foreach (XmlNode xn in inp.ChildNodes)
            {
                Petrolog_Wellheader tmp = new Petrolog_Wellheader(xn);
                this.Runs.Add(tmp);
            }
        }

        private void ParseRemarks(XmlNode inp)
        {
            foreach (XmlNode xn in inp.ChildNodes)
            {
                if (xn.LocalName != "HeaderField") continue;
                AddConstant(xn, Remarks);
            }
        }
        
        // these are not implemented yet
        private void ParseStrataDefaultsFileName(XmlNode inp) { return; }
        private void ParseStratas(XmlNode inp) { return; }
        private void ParseFormationTestInfos(XmlNode inp) { return; }
        private void ParseDevSurveyData(XmlNode inp) { return; }
        private void ParseDiscreteData(XmlNode inp) { return; }

        private void AddConstant( XmlNode node, List<Petrolog_Constant> Constants)
        {
            Petrolog_Constant tmp = new Petrolog_Constant(node);
            Constants.Add(tmp);
        }
        #endregion

        /// <summary>
        /// Returns a constant from General by name, or a null if not found
        /// </summary>
        /// <param name="name">name of the constant</param>
        /// <returns>Constant or null</returns>
        public Petrolog_Constant Get_Constant(string name)
        {
            int runnumber = 0;
            for( int i=1; i<100; i++)
            {
                string s = "Run_" + i.ToString("0") + "_";
                if( !name.StartsWith(s)) continue;
                runnumber = i;
                name = name.Substring( s.Length);
                break;
            }
            if( runnumber > 0) return Get_Constant(runnumber, name);
            foreach (Petrolog_Constant pc in General)
            {
                if (pc.Name != name) continue;
                return pc;
            }
            foreach (Petrolog_Constant pc in Remarks)
            {
                if (pc.Name != name) continue;
                return pc;
            }
            return null;
        }

        /// <summary>
        /// Returns a constant by run index and by name, or a null if not found
        /// </summary>
        /// <param name="run">run number</param>
        /// <param name="name">name of the constant</param>
        /// <returns>Constant or null</returns>
        public Petrolog_Constant Get_Constant(int run, string name)
        {
            if (run < 1 || run > Runs.Count) return null;
            return Runs[run - 1].Get_Constant(name);
        }

        /// <summary>
        /// Returns a constant value or empty string if not found
        /// </summary>
        /// <param name="name">name of the constant</param>
        /// <returns>value as string or an empty string if not found</returns>
        public string Get_ConstantValue(string name)
        {
            int runnumber = 0;
            for (int i = 1; i < 100; i++)
            {
                string s = "Run_" + i.ToString("0") + "_";
                if (!name.StartsWith(s)) continue;
                runnumber = i;
                name = name.Substring(s.Length);
                break;
            }
            if (runnumber > 0) return Get_ConstantValue(runnumber, name);
            foreach (Petrolog_Constant pc in General)
            {
                if (pc.Name != name) continue;
                return pc.Value;
            }
            foreach (Petrolog_Constant pc in Remarks)
            {
                if (pc.Name != name) continue;
                return pc.Value;
            }
            return "";
        }

        /// <summary>
        /// Returns a constant value or empty string if not found
        /// </summary>
        /// <param name="run">run number</param>
        /// <param name="name">name of the constant</param>
        /// <returns>value as string or an empty string if not found</returns>
        public string Get_ConstantValue(int run, string name)
        {
            if (run < 1 || run > Runs.Count) return "";
            return Runs[run - 1].Get_ConstantValue(name);
        }

        /// <summary>
        /// Sets a constant value
        /// </summary>
        /// <param name="name">name of the constant</param>
        /// <param name="val">value of the constant</param>
        public void Set_ConstantValue(string name, string val)
        {
            int runnumber = 0;
            for (int i = 1; i < 100; i++)
            {
                string s = "Run_" + i.ToString("0") + "_";
                if (!name.StartsWith(s)) continue;
                runnumber = i;
                name = name.Substring(s.Length);
                break;
            }
            if (runnumber > 0)
            {
                Set_ConstantValue(runnumber, name, val);
                return;
            }
            foreach (Petrolog_Constant pc in General)
            {
                if (pc.Name != name) continue;
                pc.Value = val;
                return;
            }
            foreach (Petrolog_Constant pc in Remarks)
            {
                if (pc.Name != name) continue;
                pc.Value = val;
                return;
            }
        }

        /// <summary>
        /// Sets a constant value
        /// </summary>
        /// <param name="run">run number</param>
        /// <param name="name">name of the constant</param>
        /// <param name="val">value of the constant</param>
        public void Set_ConstantValue(int run, string name, string val)
        {
            if (run < 1 || run > Runs.Count) return;
            Runs[run - 1].Set_ConstantValue(name, val);
        }

        /// <summary>
        /// Sets a constant value
        /// </summary>
        /// <param name="name">name of the constant</param>
        /// <param name="val">value of the constant</param>
        /// <param name="unit">value of the constant</param>
        /// <param name="description">value of the constant</param>
        public void Set_ConstantValue(string name, string val, string unit, string description)
        {
            int runnumber = 0;
            for (int i = 1; i < 100; i++)
            {
                string s = "Run_" + i.ToString("0") + "_";
                if (!name.StartsWith(s)) continue;
                runnumber = i;
                name = name.Substring(s.Length);
                break;
            }
            if (runnumber > 0)
            {
                Set_ConstantValue(runnumber, name, val, unit, description);
                return;
            }
            foreach (Petrolog_Constant pc in General)
            {
                if (pc.Name != name) continue;
                pc.Value = val;
                return;
            }
            foreach (Petrolog_Constant pc in Remarks)
            {
                if (pc.Name != name) continue;
                pc.Value = val;
                return;
            }
            Petrolog_Constant tmp = new Petrolog_Constant(name, unit, val, description);
            string t = name.ToUpper();
            if (!t.StartsWith("R"))
            {
                General.Add(tmp);
                return;
            }
            t = t.Substring( 1);
            try
            {
                int i = Convert.ToInt32(t);
                if (i > 0)
                {
                    tmp.Description = "";
                    Remarks.Add(tmp);
                }
                else
                {
                    General.Add(tmp);
                }
            }
            catch (Exception) { General.Add(tmp); }
        }

        /// <summary>
        /// Sets a constant value
        /// </summary>
        /// <param name="run">run number</param>
        /// <param name="name">name of the constant</param>
        /// <param name="val">value of the constant</param>
        /// <param name="unit">value of the constant</param>
        /// <param name="description">value of the constant</param>
        public void Set_ConstantValue(int run, string name, string val, string unit, string description)
        {
            if (run < 1) return;
            if (run > 99) return;
            while (run > Runs.Count) Runs.Add( new Petrolog_Wellheader());
            Runs[run - 1].Set_ConstantValue(name, val, unit, description);
        }

        /// <summary>
        /// Saves the wellheader to file, mimic the Petrolog 10.7 structure
        /// </summary>
        /// <param name="name">filename to write to</param>
        public void Write(string name)
        {
            StreamWriter sw = File.CreateText( name);
            sw.WriteLine( "<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sw.WriteLine( "<HeaderFile xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" noNamespaceSchemaLocation=\"C:\\Program Files (x86)\\CDP\\Petrolog 32Bit\\Default\\Schemas\\HeaderFile.xsd\">");
            sw.WriteLine( "  <Version>" + Version + "</Version>");
            sw.WriteLine( "  <General>");
            foreach (Petrolog_Constant pc in General) sw.Write(pc.ToString("    "));
            sw.WriteLine( "  </General>");
            if (Runs.Count >= 1)
            {
                sw.WriteLine("  <Runs>");
                foreach (Petrolog_Wellheader wh in Runs)
                {
                    sw.WriteLine("    <Run>");
                    foreach (Petrolog_Constant pc in wh.General) sw.Write(pc.ToString("      "));
                    sw.WriteLine("    </Run>");
                }
                sw.WriteLine("  </Runs>");
            }
            else
            {
                sw.WriteLine("  <Runs />");
            }
            if (Remarks.Count >= 1)
            {
                sw.WriteLine("  <Remarks>");
                foreach (Petrolog_Constant pc in Remarks) sw.Write(pc.ToString("    "));
                sw.WriteLine("  </Remarks>");
            }
            else
            {
                sw.WriteLine("  <Remarks />");
            }
            sw.WriteLine("   <StrataDefaultsFileName>..\\defaults.strata</StrataDefaultsFileName>");
            sw.WriteLine( "  <Stratas />");
            sw.WriteLine( "  <FormationTestInfos />");
            sw.WriteLine( "  <DevSurveyData />");
            sw.WriteLine( "  <DiscreteData />");
            sw.Write("</HeaderFile>");
            sw.Close();
        }

        /// <summary>
        /// Creats a deep copy of this wellheader
        /// </summary>
        /// <returns></returns>
        public Petrolog_Wellheader Clone( XmlDocument doc)
        {
            Petrolog_Wellheader tmp = new Petrolog_Wellheader();
            tmp.m_HeaderDocument = doc;
            tmp.Version = this.Version;
            foreach( Petrolog_Constant c in General)
            {
                tmp.General.Add( c.Clone());
            }
            foreach( Petrolog_Constant c in Remarks)
            {
                tmp.Remarks.Add( c.Clone());
            }
            foreach( Petrolog_Wellheader w in Runs)
            {
                tmp.Runs.Add( w.Clone( doc));
            }
            return tmp;
        }
    }
}
