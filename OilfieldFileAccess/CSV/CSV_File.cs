using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Petronode.OilfieldFileAccess;

namespace Petronode.OilfieldFileAccess.CSV
{
    public class CSV_File: Oilfield_File
    {
        /// <summary>
        /// Digital represenation of Missing Value
        /// </summary>
        public double MissingValue = Double.NaN;

        /// <summary>
        /// Instructs to save constants (on top of the file)
        /// </summary>
        public bool SaveConstants = true;

        /// <summary>
        /// Instructs to keep a unit line
        /// </summary>
        public bool SaveUnits = true;

        /// <summary>
        /// Instructs to keep the channel descriptions
        /// </summary>
        public bool SaveDescriptions = true;

        #region Constructors
        /// <summary>
        /// Constructor; creates an empty CSV file
        /// </summary>
        public CSV_File(): base()
        {
        }
        
        /// <summary>
        /// Constructor; reads the CSV file into memory
        /// </summary>
        /// <param name="filename">File name</param>
        public CSV_File(string filename): base( filename)
        {
            ParseFile();
        }

        /// <summary>
        /// Constructor; reads the CSV file into memory
        /// </summary>
        /// <param name="filename">File name</param>
        /// <param name="missing">Explicit represenetation of missing values</param>
        public CSV_File(string filename, double missing)
            : base(filename)
        {
            MissingValue = missing;
            ParseFile();
        }

        private void ParseFile()
        {
            FileStream fs = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fs);
            bool constant_block_on = true;
            bool curve_block_on = false;
            bool unit_block_on = false;
            bool description_block_on = false;
            bool data_block_on = false;
            while (!sr.EndOfStream)
            {
                string s = sr.ReadLine();
                if (s == null) break;

                // read constants if any
                if (constant_block_on)
                {
                    if (s.StartsWith("//"))
                    {
                        CSV_Constant c = new CSV_Constant(s);
                        Constants.Add(c);
                        continue;
                    }
                    else
                    {
                        constant_block_on = false;
                        curve_block_on = true;
                    }
                }

                // read channel (curve) names
                if (curve_block_on)
                {
                    if (s.StartsWith("//")) continue;
                    CSV_Line line = new CSV_Line(s);
                    if (line.IsAllDigital())
                    {
                        for (int i = 1; i <= line.Substrings.Length; i++)
                        {
                            this.Channels.Add(new CSV_Channel("COL_" + i.ToString().PadLeft(3, '0'), ""));
                        }
                        curve_block_on = false;
                        data_block_on = true;
                    }
                    else
                    {
                        for (int i = 0; i < line.Substrings.Length; i++)
                        {
                            string name = line.Substrings[i];
                            if (name.Length <= 0) name = "COL_" + i.ToString().PadLeft(3, '0');
                            this.Channels.Add(new CSV_Channel(name, ""));
                        }
                        curve_block_on = false;
                        unit_block_on = true;
                        continue;
                    }
                }

                // read unit names
                if (unit_block_on)
                {
                    if (s.StartsWith("//")) continue;
                    CSV_Line line = new CSV_Line(s);
                    if (line.IsAllDigital())
                    {
                        unit_block_on = false;
                        data_block_on = true;
                    }
                    else
                    {
                        for (int i = 0; i < this.Channels.Count; i++)
                        {
                            string unit = line.GetString(i);
                            this.Channels[i].Unit = unit;
                        }
                        unit_block_on = false;
                        description_block_on = true;
                        continue;
                    }
                }

                // read descriptions
                if (description_block_on)
                {
                    if (s.StartsWith("//")) continue;
                    CSV_Line line = new CSV_Line(s);
                    if (line.IsAllDigital())
                    {
                        description_block_on = false;
                        data_block_on = true;
                    }
                    else
                    {
                        for (int i = 0; i < this.Channels.Count; i++)
                        {
                            string description = line.GetString(i);
                            this.Channels[i].Description = description;
                        }
                        description_block_on = false;
                        data_block_on = true;
                        continue;
                    }
                }

                // read data
                if (data_block_on)
                {
                    if (s.StartsWith("//")) continue;
                    CSV_Line line = new CSV_Line(s);
                    for (int i = 0; i < this.Channels.Count; i++)
                    {
                        if (line.Substrings[i].Length < 1)
                        {
                            Channels[i].Data.Add(Double.NaN);
                            continue;
                        }
                        double value = line.GetDouble(i);
                        if (value == MissingValue) value = Double.NaN;
                        Channels[i].Data.Add(value);
                    }
                }
            }
            sr.Close();
            fs.Close();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes the CSV into a file
        /// </summary>
        /// <param name="filename">File name to write to</param>
        public override void Write(string filename)
        {
            FileStream fs = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
            StreamWriter sw = new StreamWriter( fs);
            if( SaveConstants)
            {
                foreach (CSV_Constant c in Constants) sw.WriteLine(c.ToString());
            }
            sw.WriteLine(LogMnemonicsToString());
            if (SaveUnits) sw.WriteLine(DataUnitsToString());
            if (SaveDescriptions) sw.WriteLine(DescriptionsToString());
            
            // write data
            CSV_Channel indexChannel = GetIndex();
            if (indexChannel == null)
            {
                sw.Close();
                fs.Close();
                return;
            }
            for (int i = 0; i < indexChannel.Data.Count; i++)
            {
                StringBuilder sb = new StringBuilder( indexChannel.ToString( i, MissingValue));
                for (int j = 1; j < Channels.Count; j++)
                {
                    sb.Append("," + Channels[j].ToString(i, MissingValue));
                }
                string tmp = sb.ToString();
                sw.WriteLine( tmp);
            }
            sw.Close();
            fs.Close();
        }

        /// <summary>
        /// Sets the CSV constant by name, adding a new parameter if necessary
        /// </summary>
        /// <param name="name">Constant name</param>
        /// <param name="unit">Constant unit</param>
        /// <param name="value">Constant value</param>
        /// <param name="value">Description</param>
        public override void SetConstant(string name, string unit, string value, string description)
        {
            foreach (CSV_Constant c in Constants)
            {
                if ( c.Name != name) continue;
                c.Value = value;
                return;
            }
            CSV_Constant newP = new CSV_Constant(name, unit, value, description);
            Constants.Add(newP);
        }
        
        /// <summary>
        /// Sets the CSV parameter by name, adding a new parameter if necessary
        /// Note that parameters cannot be saved to CSV file - they are for run-time only
        /// </summary>
        /// <param name="name">Constant name</param>
        /// <param name="unit">Constant unit</param>
        /// <param name="value">Constant value</param>
        /// <param name="value">Description</param>
        public override void SetParameter(string name, string unit, string value, string description)
        {
            foreach (CSV_Constant c in Parameters)
            {
                if (c.Name != name) continue;
                c.Value = value;
                return;
            }
            CSV_Constant newP = new CSV_Constant(name, unit, value, description);
            Parameters.Add(newP);
        }

        /// <summary>
        /// Returns the shallow copy of LAS index (first channel)
        /// </summary>
        public new CSV_Channel GetIndex()
        {
            return GetChannel( 0);
        }

        /// <summary>
        /// Returns the deep copy of LAS index (first channel)
        /// </summary>
        public new CSV_Channel GetIndexCopy()
        {
            return GetChannelCopy(0);
        }

        /// <summary>
        /// Returns the shallow copy of LAS channel
        /// </summary>
        public new CSV_Channel GetChannel(int index)
        {
            CSV_Channel lc = (CSV_Channel)base.GetChannel(index);
            return lc;
        }

        /// <summary>
        /// Returns the shallow copy of LAS channel
        /// </summary>
        public new CSV_Channel GetChannel(string name)
        {
            CSV_Channel lc = (CSV_Channel)base.GetChannel(name);
            return lc;
        }

        /// <summary>
        /// Returns the deep copy of LAS channel
        /// </summary>
        public new CSV_Channel GetChannelCopy(int index)
        {
            if (index < 0 || index >= Channels.Count) return null;
            Oilfield_Channel c = GetChannel(index).Clone();
            return (CSV_Channel)c;
        }

        /// <summary>
        /// Returns the deep copy of CSV channel
        /// </summary>
        public new CSV_Channel GetChannelCopy(string name)
        {
            foreach (CSV_Channel c in Channels)
            {
                if (c.Name != name) continue;
                return (CSV_Channel)c.Clone();
            }
            return null;
        }

        /// <summary>
        /// Returns the LAS channel, creating one as necessary
        /// </summary>
        public new CSV_Channel GetOrCreateChannel(string name, string unit, string description, string format)
        {
            CSV_Channel c = GetChannel(name);
            if (c != null) return c;
            c = GetIndexCopy();
            c.Name = name;
            c.Unit = unit;
            c.Description = description;
            c.Format = format;
            Channels.Add(c);
            return c;
        }

        /// <summary>
        /// Returns a new LAS channel
        /// </summary>
        public new CSV_Channel GetNewChannel(string name, string unit, string description)
        {
            CSV_Channel lc = new CSV_Channel( name, unit);
            lc.Description = description;
            return lc;
        }

        /// <summary>
        /// Returns a new LAS channel
        /// </summary>
        public CSV_Channel GetNewChannel(CSV_Channel template)
        {
            CSV_Channel c = (CSV_Channel)template.Clone();
            CSV_Channel index = GetIndex();
            if (c != null)
            {
                c.Data.Clear();
                for (int i = 0; i < index.Data.Count; i++) c.Data.Add(Double.NaN);
            }
            return c;
        }

        /// <summary>
        /// Sets the channel list from generic list
        /// </summary>
        public void SetChannelList(List<CSV_Channel> lst)
        {
            this.Channels.Clear();
            foreach (CSV_Channel c in lst)
            {
                this.Channels.Add(c);
            }
        }

        /// <summary>
        /// Returns true if the file extension corresponds to CSV
        /// </summary>
        /// <param name="name">file name</param>
        /// <returns>true if the extension corresponds to CSV</returns>
        public static bool IsCSVFile(string name)
        {
            FileInfo fi = new FileInfo(name);
            string ext = fi.Extension.ToLower();
            return ext.StartsWith(".csv");
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
        private string LogMnemonicsToString()
        {
            CSV_Line line = new CSV_Line( Channels.Count);
            for( int i=0; i<Channels.Count; i++) line.SetString( i, Channels[i].Name);
            return line.ToString();
        }

        /// <summary>
        /// Generated the data header
        /// </summary>
        private string DataUnitsToString()
        {
            CSV_Line line = new CSV_Line(Channels.Count);
            for (int i = 0; i < Channels.Count; i++) line.SetString(i, Channels[i].Unit);
            return line.ToString();
        }

        /// <summary>
        /// Generated the data header
        /// </summary>
        private string DescriptionsToString()
        {
            CSV_Line line = new CSV_Line(Channels.Count);
            for (int i = 0; i < Channels.Count; i++) line.SetString(i, Channels[i].Description);
            return line.ToString();
        }
        #endregion
    }
}
