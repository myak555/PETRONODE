using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Petronode.OilfieldFileAccess;

namespace Petronode.OilfieldFileAccess.LAS
{
    public class LAS_File: Oilfield_File
    {
        private char[] _space_split = { ' ' };
        private List<string> m_RawData = new List<string>();
        private int m_PaddedColumnWidth = 11;

        /// <summary>
        /// Describes LAS file information lines
        /// </summary>
        public List<LAS_InfoLine> InfoLines = new List<LAS_InfoLine>();

        /// <summary>
        /// Indicates the file version
        /// </summary>
        public List<LAS_Constant> Version = new List<LAS_Constant>();

        /// <summary>
        /// Dogital represenation of Missing Value
        /// </summary>
        public double MissingValue = -999.25;

        /// <summary>
        /// Indicates the new data header styling
        /// </summary>
        public bool NewDataHeaderStyle = false;

        #region Constructors
        /// <summary>
        /// Construsctor; creates an empty LAS file
        /// </summary>
        /// <param name="filename">File name</param>
        /// <param name="ParseData">Set to true is data parsing is needed</param>
        public LAS_File(): base()
        {
        }
        
        /// <summary>
        /// Construsctor; reads the LAS file header to memory
        /// </summary>
        /// <param name="filename">File name</param>
        /// <param name="ParseData">Set to true is data parsing is needed</param>
        public LAS_File(string filename, bool ParseData): base( filename)
        {
            FileStream fs = null;
            StreamReader sr = null;
            try
            {
                fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                sr = new StreamReader(fs);
                bool version_info_block_on = false;
                bool well_info_block_on = false;
                bool curve_info_block_on = false;
                bool parameter_info_block_on = false;
                bool other_info_block_on = false;
                bool data_on = false;
                while (!sr.EndOfStream)
                {
                    string s = sr.ReadLine();
                    string s2 = s.ToLower();

                    if (s2.StartsWith("~version"))
                    {
                        version_info_block_on = true;
                        well_info_block_on = false;
                        curve_info_block_on = false;
                        parameter_info_block_on = false;
                        other_info_block_on = false;
                        data_on = false;
                        continue;
                    }
                    if (s2.StartsWith("~well"))
                    {
                        version_info_block_on = false;
                        well_info_block_on = true;
                        curve_info_block_on = false;
                        parameter_info_block_on = false;
                        other_info_block_on = false;
                        data_on = false;
                        continue;
                    }
                    if (s2.StartsWith("~curv") || s2.StartsWith("~log_definition"))
                    {
                        version_info_block_on = false;
                        well_info_block_on = false;
                        curve_info_block_on = true;
                        parameter_info_block_on = false;
                        other_info_block_on = false;
                        data_on = false;
                        continue;
                    }
                    if (s2.StartsWith("~paramet") || s2.StartsWith("~log_parameter"))
                    {
                        version_info_block_on = false;
                        well_info_block_on = false;
                        curve_info_block_on = false;
                        parameter_info_block_on = true;
                        other_info_block_on = false;
                        data_on = false;
                        continue;
                    }
                    if (s2.ToLower().StartsWith("~other"))
                    {
                        version_info_block_on = false;
                        well_info_block_on = false;
                        curve_info_block_on = false;
                        parameter_info_block_on = false;
                        other_info_block_on = true;
                        data_on = false;
                        continue;
                    }
                    if (s2.ToUpper().StartsWith("~A") || s2.StartsWith("~log_data"))
                    {
                        version_info_block_on = false;
                        well_info_block_on = false;
                        curve_info_block_on = false;
                        parameter_info_block_on = false;
                        other_info_block_on = false;
                        data_on = true;
                        if (!ParseData) break;
                        continue;
                    }
                    if (version_info_block_on)
                    {
                        if (s.StartsWith("#")) continue;
                        if (s.Length <= 1) continue;
                        LAS_Constant lc = new LAS_Constant(s);
                        if (lc.Name.ToUpper() == "VERS" && !lc.Value.StartsWith("2."))
                            throw new Exception("LAS version other than 2.x cannot be handled.");
                        if (lc.Name.ToUpper() == "WRAP" && !lc.Value.StartsWith("N"))
                            throw new Exception("Wrapped data cannot be handled by most clients. Do not use LAS wrap.");
                        Version.Add(lc);
                    }
                    if (well_info_block_on)
                    {
                        if (s.StartsWith("#")) continue;
                        if (s.Length <= 1) continue;
                        LAS_Constant lc = new LAS_Constant(s);
                        if (lc.Name == "NULL") MissingValue = Convert.ToDouble(lc.Value);
                        Constants.Add(lc);
                    }
                    if (curve_info_block_on)
                    {
                        if (s.StartsWith("#")) continue;
                        if (s.Length <= 1) continue;
                        LAS_Channel lc = new LAS_Channel(s);
                        Channels.Add(lc);
                    }
                    if (parameter_info_block_on)
                    {
                        if (s.StartsWith("#")) continue;
                        if (s.Length <= 1) continue;
                        LAS_Constant lc = new LAS_Constant(s);
                        Parameters.Add(lc);
                    }
                    if (other_info_block_on)
                    {
                        if (s.Length <= 1) continue;
                        LAS_InfoLine lil = new LAS_InfoLine(s);
                        InfoLines.Add(lil);
                    }
                    if (data_on)
                    {
                        if (s.StartsWith("#")) continue;
                        if (s.Length <= 1) continue;
                        if (s.StartsWith("~A")) continue;
                        m_RawData.Add(s);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if( sr != null) sr.Close();
                if (fs != null) fs.Close();
            }
            ConvertConstantsFromEarlierLAS();

            // parse data to channels
            if (Channels.Count <= 0) throw new Exception("No curves have been declared in the file.");
            foreach (string s in m_RawData)
            {
                string[] ss = s.Split(_space_split, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < ss.Length; i++)
                {
                    Channels[i].AddData(ss[i], MissingValue);
                }
            }
            m_RawData.Clear();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Writes the LAS into a file
        /// </summary>
        /// <param name="filename">File name to write to</param>
        public override void Write(string filename)
        {
            StreamWriter sw = File.CreateText(filename);
            sw.WriteLine( "~Version Information");
            sw.WriteLine( "VERS.              2.00:     CWLS log ASCII Standard -VERSION 2.00");
            sw.WriteLine( "WRAP.                NO:     One line per depth step");
            sw.WriteLine( "#");
            sw.WriteLine( "#");
            sw.WriteLine( "~Well Information Block");
            sw.WriteLine( "#MNEM.UNIT     Data Type                      Description");
            sw.WriteLine( "#---------     -----------                    ----------------");
            foreach( LAS_Constant c in Constants) sw.WriteLine( c.ToString());
            sw.WriteLine( "#");
            sw.WriteLine( "#");
            sw.WriteLine( "~Curve Information Block");
            sw.WriteLine( "#MNEM.UNIT   API Codes           Curve Description");
            sw.WriteLine("#---------    -----------         -----------------------------");
            foreach (LAS_Channel c in Channels) sw.WriteLine(c.ToString());
            sw.WriteLine( "#");
            sw.WriteLine( "#");
            sw.WriteLine( "~Parameter Information Block");
            sw.WriteLine( "#MNEM.UNIT     Data                           Description");
            sw.WriteLine( "#---------     -----------                    ----------------");
            foreach (LAS_Constant c in Parameters) sw.WriteLine(c.ToString());
            sw.WriteLine( "#");
            sw.WriteLine( "#");
            sw.WriteLine( "~Other Information");
            foreach (LAS_InfoLine c in InfoLines) sw.WriteLine(c.ToString());
            sw.WriteLine( "#");
            sw.WriteLine( "#");
            if (NewDataHeaderStyle)
            {
                sw.WriteLine("# --- LOG MNEMONICS AND UNITS ---");
                sw.WriteLine(LogMnemonicsToString());
                sw.WriteLine(DataUnitsToString());
                sw.WriteLine("~A");
            }
            else
            {
                sw.WriteLine(DataHeaderToString());
            }
            
            // write data
            LAS_Channel indexChannel = (LAS_Channel)GetIndex();
            if (indexChannel == null)
            {
                sw.Close();
                return;
            }
            for (int i = 0; i < indexChannel.Data.Count; i++)
            {
                StringBuilder sb = new StringBuilder( indexChannel.ToString( i, MissingValue));
                for (int j = 1; j < Channels.Count; j++)
                {
                    sb.Append(' ');
                    sb.Append(Channels[j].ToString(i, MissingValue));
                }
                string tmp = sb.ToString();
                sw.WriteLine( tmp);
            }
            sw.Close();
        }

        /// <summary>
        /// Denoted the standard column width in the printout
        /// </summary>
        public int PaddedColumnWidth
        {
            get { return m_PaddedColumnWidth; }
            set
            {
                m_PaddedColumnWidth = value;
                foreach (LAS_Channel c in Channels) c.PaddedWidth = value-1;
            }
        }

        /// <summary>
        /// Sets the LAS constant by name, adding a new parameter if necessary
        /// </summary>
        /// <param name="name">Constant name</param>
        /// <param name="unit">Constant unit</param>
        /// <param name="value">Constant value</param>
        /// <param name="value">Description</param>
        public override void SetConstant(string name, string unit, string value, string description)
        {
            foreach (LAS_Constant lc in Constants)
            {
                if (lc.Name != name) continue;
                lc.Value = value;
                return;
            }
            LAS_Constant newP = new LAS_Constant(name, unit, value, description);
            Constants.Add(newP);
        }
        
        /// <summary>
        /// Sets the LAS constant by name, adding a new parameter if necessary
        /// </summary>
        /// <param name="name">Constant name</param>
        /// <param name="unit">Constant unit</param>
        /// <param name="value">Constant value</param>
        /// <param name="value">Description</param>
        public override void SetParameter(string name, string unit, string value, string description)
        {
            foreach (LAS_Constant lc in Parameters)
            {
                if (lc.Name != name) continue;
                lc.Value = value;
                return;
            }
            LAS_Constant newP = new LAS_Constant(name, unit, value, description);
            Parameters.Add(newP);
        }

        /// <summary>
        /// Attempts information retrieval from info lines
        /// </summary>
        /// <param name="key">information key</param>
        /// <returns>info string if found, or empty string</returns>
        public override string GetInfo(string key)
        {
            try
            {
                foreach (LAS_InfoLine il in InfoLines)
                {
                    string s = il.GetInfo(key);
                    if (s == null) return "";
                    if (s.Length > 0) return s;
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// Attempts information retrieval from info lines
        /// </summary>
        /// <param name="key">information key</param>
        /// <param name="bandpass">characters to filter out</param>
        /// <returns>info string if found, or empty string</returns>
        public override string GetInfo(string key, string bandpass)
        {
            string s = GetInfo(key);
            if( s.Length <=0 ) return "";
            foreach( char c in bandpass)
            {
                s = s.Replace( c, ' ');
            }
            return s.Trim();
        }

        /// <summary>
        /// Attempts to get information from: contants, parameters, info lines (in that order)
        /// </summary>
        /// <param name="name">mnemonic</param>
        /// <param name="key">key for info lines</param>
        /// <param name="bandpass">bandpass for info lines</param>
        /// <returns>value if found or empty string</returns>
        public override string GetInformation(string name, string key, string bandpass)
        {
            string s = base.GetInformation(name, key, bandpass);
            if (s.Length > 0) return s;
            return GetInfo(key, bandpass);
        }

        // Well site data (as in Slimline infolines)
        //
        //
        //COMPOSITE LOG                  DH1321 1:200                 - SLIMLINE UNIT -   
        //DD6 688/8680CN 23/8/11, MS2 317                                                 
        //                               ANGLE OF HOLE: 90 DEGREES                        
        //CASING OFFSET: 2.10M           HOLE LOGGED FROM 231M TO 3M                      
        //WATER LEVEL: 19 M                                                               
        //Run number                 Log 1st rdg                Log last rdg  3M          
        //Driller TD    231 M        Logger TD     231.1 M      Water level               
        //Perm Datum    K.B.         Elevation     K.B.         Other srvcs   DD6,MS2,ATV 
        //Dril mes from G.L.         Log meas from G.L.         Other srvcs   VO4/SSD     
        //Elevation: KB              DF                         GL                        
        //Casing Driler 16 M         Casing Logger 16 M         Casing size   101.6 MM    
        //Casing Weight HQ RODS      From          G.L.         To            16 M        
        //Bit size      99MM         From          CASING       To            T.D.        
        //Hole fl type  AIR/WATER    Sample source              Fluid loss                
        //Density                    Viscosity                  pH                        
        //RM @ meas tmp              RMF @ mes tmp              RMC @ mes tmp             
        //RM @ BHT                   RMF source                 RMC-source                
        //Max rec temp               Time snc circ              LsdSecTwpRge              
        //Equipment no. HELI 3       Base                       Equip. name               
        //Recorded by   EPT          Witnessed by               Sonde srl no. 688/8680CN  
        //Last title                 Last line                  Permit no.                
        //

        /// <summary>
        /// Attempts to get information from: contants, parameters, info lines (in that order)
        /// </summary>
        /// <param name="name">mnemonic</param>
        /// <returns>value if found or empty string</returns>
        public override string GetInformation(string name)
        {
            string s = base.GetInformation(name);
            if (s.Length > 0) return s;
            switch (name)
            {
                case "CTRY":
                    s = GetConstant("PROV");
                    if (s.Length <= 0) return "AUSTRALIA";
                    return s.ToUpper().EndsWith("ISLAND") ? "NEW ZEALAND" : "AUSTRALIA";
                case "OS":
                    s = GetConstant("OS1");
                    s += " " + GetConstant("OS2");
                    s += " " + GetConstant("OS3");
                    s += " " + GetConstant("OS4");
                    s = s.Trim();
                    if (s.Length > 3) return s;
                    return GetAllInfo("Other srvcs   ");
                case "PD":
                    s = GetInfo("Perm Datum    ");
                    if (s.ToUpper().StartsWith("GROUND LE")) s = "G.L.";
                    return s;
                case "EPD":
                    return "0.00";
                case "LMF":
                    s = GetInfo("Log meas from");
                    if (s.ToUpper().StartsWith("GROUND LE")) s = "G.L.";
                    return s;
                case "MP":
                    s = GetInfo("Dril mes from");
                    if (s.ToUpper().StartsWith("GROUND LE")) s = "G.L.";
                    return s;
                case "EKB":
                    return GetInfo("Elevation: KB ", "MmKBDFGL");
                case "EDF":
                    return GetInfo("DF            ", "MmKBDFGL");
                case "EGL":
                    return GetInfo("GL            ", "MmKBDFGL");
                case "TDD":
                    return Reformat( GetInfo("Driller TD    ", "Mm"), "0.00");
                case "TDL":
                    return Reformat( GetInfo("Logger TD     ", "Mm"), "0.00");
                case "CSGD":
                    return Reformat( GetInfo("Casing Driler ", "Mm"), "0.00");
                case "CSGL":
                    return Reformat( GetInfo("Casing Logger ", "Mm"), "0.00");
                case "BS":
                    s = GetInfo("Bit size      ", "Mm");
                    s = s.Replace("\"", " in");
                    return s;
                case "MUD":
                    return GetInfo("Hole fl type  ");
                case "UNIT":
                    return GetInfo("Equipment no. ");
                case "BASE":
                    return GetInfo("Base          ");
                case "ENG":
                    return GetInfo("Recorded by   ");
                case "WIT":
                    return GetInfo("Witnessed by  ");
                default:
                    break;
             }
             return "";
        }

        /// <summary>
        /// Attempts information retrieval from info lines
        /// </summary>
        /// <param name="key">information key</param>
        /// <returns>info string if found, or empty string</returns>
        public override string GetAllInfo(string key)
        {
            StringBuilder sb = new StringBuilder();
            foreach (LAS_InfoLine il in InfoLines)
            {
                string s = il.GetInfo(key);
                if (s.Length <= 0) continue;
                if (sb.Length > 0) sb.Append(',');
                sb.Append(s);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns the shallow copy of LAS index (first channel)
        /// </summary>
        //public new LAS_Channel GetIndex()
        //{
        //    return GetChannel( 0);
        //}

        /// <summary>
        /// Returns the deep copy of LAS index (first channel)
        /// </summary>
        //public new LAS_Channel GetIndexCopy()
        //{
        //    return GetChannelCopy(0);
        //}

        /// <summary>
        /// Returns the shallow copy of LAS channel
        /// </summary>
        //public new LAS_Channel GetChannel(int index)
        //{
        //    LAS_Channel lc = (LAS_Channel)base.GetChannel(index);
        //    return lc;
        //}

        /// <summary>
        /// Returns the shallow copy of LAS channel
        /// </summary>
        //public new LAS_Channel GetChannel(string name)
        //{
        //    LAS_Channel lc = (LAS_Channel)base.GetChannel(name);
        //    return lc;
        //}

        /// <summary>
        /// Returns the deep copy of LAS channel
        /// </summary>
        public override Oilfield_Channel GetChannelCopy(int index)
        {
            if (index < 0 || index >= Channels.Count) return null;
            LAS_Channel lc = new LAS_Channel(GetChannel(index));
            return (Oilfield_Channel)lc;
        }

        /// <summary>
        /// Returns the deep copy of LAS channel
        /// </summary>
        public override Oilfield_Channel GetChannelCopy(string name)
        {
            foreach (LAS_Channel c in Channels)
            {
                if (c.Name != name) continue;
                LAS_Channel lc = new LAS_Channel(c);
                return (Oilfield_Channel)lc;
            }
            return null;
        }

        /// <summary>
        /// Returns the LAS channel, creating one as necessary
        /// </summary>
        public override Oilfield_Channel GetOrCreateChannel(string name, string unit, string description, string format)
        {
            LAS_Channel c = (LAS_Channel)GetChannel(name);
            if (c != null) return (Oilfield_Channel)c;
            c = (LAS_Channel)GetIndexCopy();
            c.Name = name;
            c.Unit = unit;
            c.Description = description;
            c.Format = format;
            c.SetData(Double.NaN);
            Channels.Add(c);
            return (Oilfield_Channel)c;
        }

        /// <summary>
        /// Returns a new LAS channel
        /// </summary>
        public override Oilfield_Channel GetNewChannel(string name, string unit, string description)
        {
            LAS_Channel lc = (LAS_Channel)base.GetNewChannel(name, unit, description);
            return (Oilfield_Channel)lc;
        }

        /// <summary>
        /// Returns a new LAS channel
        /// </summary>
        public LAS_Channel GetNewChannel(LAS_Channel template)
        {
            LAS_Channel c = new LAS_Channel(template);
            LAS_Channel index = (LAS_Channel)GetIndex();
            if (c != null)
            {
                c.Data.Clear();
                for (int i = 0; i < index.Data.Count; i++) c.Data.Add(Double.NaN);
            }
            return c;
        }

        /// <summary>
        /// Returns fully-qualified name of this LAS
        /// </summary>
        /// <param name="postfix">Postfix to append</param>
        /// <returns>Fully-qualified name</returns>
        public override string GetQualifiedName(string postfix)
        {
            return base.GetQualifiedName(postfix);
        }

        /// <summary>
        /// Crops all file data between min index and max index
        /// </summary>
        /// <param name="minIndex">minimum value of index</param>
        /// <param name="maxIndex">maximum value of index</param>
        public override void CropByIndex(double minIndex, double maxIndex)
        {
            base.CropByIndex(minIndex, maxIndex);
            LAS_Channel index = (LAS_Channel)GetIndex();
            if (index == null || index.Data.Count <= 0)
            {
                SetConstant("STRT", "0.000");
                SetConstant("STOP", "0.000");
                return;
            }
            SetConstant("STRT", index.Data[0].ToString( index.Format));
            SetConstant("STOP", index.Data[index.Data.Count-1].ToString(index.Format));
        }

        /// <summary>
        /// Sets the channel list from generic list
        /// </summary>
        public void SetChannelList(List<LAS_Channel> lst)
        {
            this.Channels.Clear();
            foreach (LAS_Channel c in lst)
            {
                this.Channels.Add(c);
            }
        }

        /// <summary>
        /// Returns true if the file extension corresponds to LAS
        /// </summary>
        /// <param name="name">file name</param>
        /// <returns>true if the extension corresponds to LAS</returns>
        public static bool IsLASFile(string name)
        {
            FileInfo fi = new FileInfo(name);
            string ext = fi.Extension.ToLower();
            return ext.StartsWith(".las");
        }

        /// <summary>
        /// This is used for files that lie about the index step, such as sometimes in WellCAD
        /// </summary>
        /// <returns></returns>
        public double EstimateIndexStep( bool SkipIfDefined)
        {
            double st = Convert.ToDouble( this.GetConstant("STEP"));
            if (st != 0.0 && SkipIfDefined) return st;
            LAS_Channel index = (LAS_Channel)GetIndex();
            if (!index.IsLoaded || index.Data.Count <= 1) return st;
            st = index.Data[1] - index.Data[0];
            SetConstant("STEP", st.ToString( "0.000000"));
            return st;
        }

        /// <summary>
        /// Creates the same channes as in the given file
        /// </summary>
        /// <param name="file"></param>
        public override void CreateSameChannels(Oilfield_File file)
        {
            for (int i = 1; i < file.Channels.Count; i++)
            {
                Oilfield_Channel lc = file.Channels[i];
                Oilfield_Channel tmp = this.GetChannel(lc.Name);
                if (tmp != null) continue;
                this.GetOrCreateChannel(lc.Name, lc.Unit, lc.Description, lc.Format);
            }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Generated the data header
        /// </summary>
        private string DataHeaderToString()
        {
            StringBuilder sb = new StringBuilder("~A ");
            foreach( LAS_Channel c in Channels)
            {
                string s = c.Name;
                if (s.StartsWith("DEPT")) s = "Depth";
                s = " " + s;
                sb.Append( s.PadRight(m_PaddedColumnWidth));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Generated the data header
        /// </summary>
        private string LogMnemonicsToString()
        {
            StringBuilder sb = new StringBuilder("# ");
            foreach (LAS_Channel c in Channels)
            {
                string s = c.Name;
                if (s.StartsWith("DEPT")) s = "Depth";
                s = " " + s;
                sb.Append(s.PadRight(m_PaddedColumnWidth));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Generated the data header
        /// </summary>
        private string DataUnitsToString()
        {
            StringBuilder sb = new StringBuilder("# ");
            foreach (LAS_Channel c in Channels)
            {
                string s = c.Unit;
                s = " " + s;
                sb.Append(s.PadRight(m_PaddedColumnWidth));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts LAS constants from earlier versions of las
        /// </summary>
        private void ConvertConstantsFromEarlierLAS()
        {
            string version_label = "";
            foreach (LAS_Constant c in Version)
            {
                if (c.Name != "VERS") continue;
                version_label = c.Value;
                break;
            }
            if (!version_label.StartsWith("1.")) return;
            foreach (LAS_Constant c in Constants)
            {
                if (c.Name == "COMP") swapConstant(c);
                if (c.Name == "WELL") swapConstant(c);
                if (c.Name == "LOCATION") swapConstant(c);
                if (c.Name == "SRVC") swapConstant(c);
                if (c.Name == "DATE") swapConstant(c);
                if (c.Name == "API") swapConstant(c);
                if (c.Name == "LOC") swapConstant(c);
            }
        }

        private void swapConstant(LAS_Constant c)
        {
            string tmp = c.Value;
            c.Value = c.Description;
            c.Description = tmp;
        }

        #endregion
    }
}
