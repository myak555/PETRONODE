using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Petronode.OilfieldFileAccess
{
    public class Oilfield_File
    {
        protected string m_FileName = "";

        /// <summary>
        /// Describes file Constants
        /// </summary>
        public List<Oilfield_Constant> Constants = new List<Oilfield_Constant>();

        /// <summary>
        /// Describes file Channels
        /// </summary>
        public List<Oilfield_Channel> Channels = new List<Oilfield_Channel>();

        /// <summary>
        /// Describes file Parameters
        /// </summary>
        public List<Oilfield_Constant> Parameters = new List<Oilfield_Constant>();

        #region Constructors
        /// <summary>
        /// Constructor; creates an empty file
        /// </summary>
        public Oilfield_File()
        {
        }
        
        /// <summary>
        /// Constructor; reads the file header to memory
        /// Actually, in the base class, it does nothing, just sets the name
        /// </summary>
        /// <param name="filename">File name</param>
        public Oilfield_File(string filename)
        {
            FileName = filename;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets or sets full name
        /// </summary>
        public virtual string FileName
        {
            get { return m_FileName; }
            set { m_FileName = value; }
        }

        /// <summary>
        /// Gets "qualified" name pattern
        /// </summary>
        public virtual string QualifiedCompanyName
        {
            get
            {
                string s = GetInformation("COMP");
                if (s.Length <= 0) return "";
                s = s.Replace('\\', '_');
                s = s.Replace('/', '_');
                s = s.Replace('*', '_');
                s = s.Replace('%', '_');
                s = s.Replace('?', '_');
                return s;
            }
        }

        /// <summary>
        /// Gets "qualified" name pattern
        /// </summary>
        public virtual string QualifiedFieldName
        {
            get
            {
                string s = GetInformation("FLD");
                if (s.Length <= 0) return "";
                s = s.Replace('\\', '_');
                s = s.Replace('/', '_');
                s = s.Replace('*', '_');
                s = s.Replace('%', '_');
                s = s.Replace('?', '_');
                return s;
            }
        }

        /// <summary>
        /// Gets "qualified" name pattern
        /// </summary>
        public virtual string QualifiedWellName
        {
            get
            {
                string s = GetInformation("WELL");
                if (s.Length <= 0) return "";
                s = s.Replace('\\', '_');
                s = s.Replace('/', '_');
                s = s.Replace('*', '_');
                s = s.Replace('%', '_');
                s = s.Replace('?', '_');
                return s;
            }
        }

        /// <summary>
        /// Gets "qualified" name pattern
        /// </summary>
        public virtual string QualifiedJobName
        {
            get
            {
                string s = GetInformation("WELL");
                if (s.Length <= 0) return "";
                s = s.Replace('\\', '_');
                s = s.Replace('/', '_');
                s = s.Replace('*', '_');
                s = s.Replace('%', '_');
                s = s.Replace('?', '_');
                string j = GetInformation("PRVN");
                if (j.Length <= 0) return s;
                return s + "_" + j;
            }
        }

        /// <summary>
        /// Writes into the original file
        /// </summary>
        public virtual void Write()
        {
            this.Write(this.FileName);
        }

        /// <summary>
        /// Writes into a file
        /// Actually, the base class does nothing here
        /// </summary>
        /// <param name="filename">File name to write to</param>
        public virtual void Write(string filename)
        {
        }

        /// <summary>
        /// Returns true if constant by name exists
        /// </summary>
        /// <param name="name">Constant name</param>
        /// <returns>True if the constant exists</returns>
        public bool ConstantExists(string name)
        {
            foreach (Oilfield_Constant oc in Constants)
            {
                if (oc.Name == name) return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the constant by name
        /// </summary>
        /// <param name="name">Constant name</param>
        /// <returns>value</returns>
        public string GetConstant(string name)
        {
            foreach (Oilfield_Constant oc in Constants)
            {
                if (oc.Name == name) return oc.Value;
            }
            return "";
        }

        /// <summary>
        /// Sets the constant by name
        /// </summary>
        /// <param name="name">Constant name</param>
        /// <param name="val">Constant value</param>
        public void SetConstant(string name, string val)
        {
            foreach (Oilfield_Constant oc in Constants)
            {
                if (oc.Name != name) continue;
                oc.Value = val;
                return;
            }
        }

        /// <summary>
        /// Returns true if parameter by name exists
        /// </summary>
        /// <param name="name">Constant name</param>
        /// <returns>True if the parameter exists</returns>
        public bool ParameterExists(string name)
        {
            foreach (Oilfield_Constant oc in Parameters)
            {
                if (oc.Name == name) return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the parameter by name
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <returns>value</returns>
        public string GetParameter(string name)
        {
            foreach (Oilfield_Constant oc in Parameters)
            {
                if (oc.Name == name) return oc.Value;
            }
            return "";
        }

        /// <summary>
        /// Sets the parameter by name
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="val">parameter value</param>
        public void SetParameter(string name, string val)
        {
            foreach (Oilfield_Constant oc in Parameters)
            {
                if (oc.Name != name) continue;
                oc.Value = val;
                return;
            }
        }

        /// <summary>
        /// Sets the constant by name, adding a new constant if necessary
        /// </summary>
        /// <param name="name">Constant name</param>
        /// <param name="unit">Constant unit</param>
        /// <param name="value">Constant value</param>
        /// <param name="value">Description</param>
        public virtual void SetConstant(string name, string unit, string value, string description)
        {
        }

        /// <summary>
        /// Sets the parameters by name, adding a new parameter if necessary
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="unit">Parameter unit</param>
        /// <param name="value">Parameter value</param>
        /// <param name="value">Description</param>
        public virtual void SetParameter(string name, string unit, string value, string description)
        {
        }

        /// <summary>
        /// Attempts information retrieval from info lines
        /// </summary>
        /// <param name="key">information key</param>
        /// <returns>info string if found, or empty string</returns>
        public virtual string GetInfo(string key)
        {
            return "";
        }

        /// <summary>
        /// Attempts information retrieval from info lines
        /// </summary>
        /// <param name="key">information key</param>
        /// <param name="bandpass">characters to filter out</param>
        /// <returns>info string if found, or empty string</returns>
        public virtual string GetInfo(string key, string bandpass)
        {
            return "";
        }

        /// <summary>
        /// Attempts to get information from: contants, parameters, info lines (in that order)
        /// </summary>
        /// <param name="name">mnemonic</param>
        /// <param name="key">key for info lines</param>
        /// <param name="bandpass">bandpass for info lines</param>
        /// <returns>value if found or empty string</returns>
        public virtual string GetInformation(string name, string key, string bandpass)
        {
            string s = GetConstant(name);
            if (s.Length > 0) return s;
            s = GetParameter(name);
            if (s.Length > 0) return s;
            return GetInfo(key, bandpass);
        }

        /// <summary>
        /// Reformats string, containing a number, to a format provided.
        /// If the string is not a number, it is left as is
        /// </summary>
        /// <param name="s">string to reformat</param>
        /// <param name="format">format as 0.00</param>
        /// <returns>reformatted string</returns>
        protected string Reformat(string s, string format)
        {
            try
            {
                double d = Convert.ToDouble(s);
                return d.ToString(format);
            }
            catch (Exception)
            {
            }
            return s;
        }

        /// <summary>
        /// Attempts to get information from: contants, parameters, info lines (in that order)
        /// </summary>
        /// <param name="name">mnemonic</param>
        /// <returns>value if found or empty string</returns>
        public virtual string GetInformation(string name)
        {
            string s = GetConstant(name);
            if (s.Length > 0) return s;
            s = GetParameter(name);
            if (s.Length > 0) return s;
            return "";
        }

        /// <summary>
        /// Attempts to get information from: contants, parameters, info lines (in that order)
        /// </summary>
        /// <param name="name">mnemonic</param>
        /// <param name="def">default string</param>
        /// <returns>value if found or default string</returns>
        public virtual string GetInformation(string name, string def)
        {
            string s = GetInformation(name);
            if (s.Length > 0) return s;
            return def;
        }

        /// <summary>
        /// Attempts information retrieval from info lines
        /// </summary>
        /// <param name="key">information key</param>
        /// <returns>info string if found, or empty string</returns>
        public virtual string GetAllInfo(string key)
        {
            return "";
        }

        /// <summary>
        /// Returns the shallow copy of index (first channel)
        /// </summary>
        public virtual Oilfield_Channel GetIndex()
        {
            if (Channels.Count <= 0) return null;
            return Channels[0];
        }

        /// <summary>
        /// Returns the deep copy of index (first channel)
        /// </summary>
        public virtual Oilfield_Channel GetIndexCopy()
        {
            return GetChannelCopy(0);
        }

        /// <summary>
        /// Returns the shallow copy of channel
        /// </summary>
        public virtual Oilfield_Channel GetChannel(int index)
        {
            if (index < 0 || index >= Channels.Count) return null;
            return Channels[index];
        }
        
        /// <summary>
        /// Returns the shallow copy of channel
        /// </summary>
        public virtual Oilfield_Channel GetChannel( string name)
        {
            foreach (Oilfield_Channel c in Channels)
            {
                if (c.Name != name) continue;
                return c;
            }
            return null;
        }

        /// <summary>
        /// Returns the deep copy of channel
        /// </summary>
        public virtual Oilfield_Channel GetChannelCopy(int index)
        {
            if (index < 0 || index >= Channels.Count) return null;
            return new Oilfield_Channel( Channels[index]);
        }

        /// <summary>
        /// Returns the deep copy of channel
        /// </summary>
        public virtual Oilfield_Channel GetChannelCopy(string name)
        {
            foreach (Oilfield_Channel c in Channels)
            {
                if (c.Name != name) continue;
                return new Oilfield_Channel(c);
            }
            return null;
        }

        /// <summary>
        /// Returns the channel, creating one as necessary
        /// </summary>
        public virtual Oilfield_Channel GetOrCreateChannel(string name, string unit, string description, string format)
        {
            Oilfield_Channel c = GetChannel(name);
            if (c != null) return c;
            c = GetIndexCopy();
            c.Name = name;
            c.Unit = unit;
            c.Description = description;
            c.Format = format;
            c.SetData(Double.NaN);
            Channels.Add(c);
            return c;
        }

        /// <summary>
        /// Returns a new channel
        /// </summary>
        public virtual Oilfield_Channel GetNewChannel(string name, string unit, string description)
        {
            Oilfield_Channel c = GetIndexCopy();
            if (c != null)
            {
                c.Name = name;
                c.Unit = unit;
                c.Description = description;
                c.SetData(Double.NaN);
            }
            return c;
        }

        /// <summary>
        /// Returns a new LAS channel
        /// </summary>
        public virtual Oilfield_Channel GetNewChannel(Oilfield_Channel template)
        {
            Oilfield_Channel c = new Oilfield_Channel(template);
            Oilfield_Channel index = GetIndex();
            if (c != null)
            {
                c.Data.Clear();
                for (int i = 0; i < index.Data.Count; i++) c.Data.Add(Double.NaN);
            }
            return c;
        }

        /// <summary>
        /// Applies a simple depth offset on the channel data
        /// </summary>
        /// <param name="step"></param>
        public void ApplyDepthOffsets()
        {
            Oilfield_Channel index = GetIndex();
            double step = index.Data[1] - index.Data[0];
            for (int i = 1; i < Channels.Count; i++)
            {
                Channels[i].ApplyDepthOffset(step);
            }
        }

        /// <summary>
        /// Returns fully-qualified name of this file
        /// </summary>
        /// <param name="postfix">Postfix to append</param>
        /// <returns>Fully-qualified name</returns>
        public virtual string GetQualifiedName(string postfix)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append( QualifiedCompanyName);
            sb.Append("_");
            sb.Append( QualifiedFieldName);
            sb.Append("_");
            sb.Append( QualifiedJobName);
            if (postfix.Length > 0)
            {
                sb.Append("_");
                sb.Append(postfix);
            }
            return sb.ToString().Replace(' ', '_');
        }

        /// <summary>
        /// Crops all file data between min index and max index
        /// </summary>
        /// <param name="minIndex">minimum value of index</param>
        /// <param name="maxIndex">maximum value of index</param>
        public virtual void CropByIndex(double minIndex, double maxIndex)
        {
            if (Channels.Count <= 0) return;
            List<List<double>> tmp = new List<List<double>>();
            for (int i=0; i<Channels.Count; i++)
            {
                tmp.Add( Channels[i].Data);
                Channels[i].Data = new List<double>();
            }
            for (int i=0; i<tmp[0].Count; i++)
            {
                double d = tmp[0][i];
                if( d<minIndex || maxIndex<d) continue;
                for (int j=0; j<tmp.Count; j++)
                {
                    Channels[j].Data.Add( tmp[j][i]);
                }
            }
        }

        /// <summary>
        /// Updates the channel statistics for all channels in file
        /// </summary>
        public virtual void UpdateChannelStatistics()
        {
            for (int i = 0; i < Channels.Count; i++)
            {
                Oilfield_Channel oc = GetChannel(i);
                oc.LocateDataBoundaries( this.GetIndex());
            }
        }

        /// <summary>
        /// Changes units on all channels
        /// </summary>
        /// <param name="UnitFrom">Unit name to change from. If no match, channel is not changing</param>
        /// <param name="UnitTo">Unit name to change to</param>
        /// <param name="gain">Conversion gain</param>
        /// <param name="offset">Conversion offset</param>
        public virtual bool ChangeUnits(string UnitFrom, string UnitTo, double gain, double offset)
        {
            bool result = false;
            foreach (Oilfield_Channel oc in Channels)
                result = result || oc.ChangeUnits(UnitFrom, UnitTo, gain, offset);
            return result;
        }

        /// <summary>
        /// Returns and array of channel names
        /// </summary>
        /// <returns></returns>
        public string[] GetChannelList()
        {
            if( Channels.Count <= 0) return null;
            string[] tmp = new string[ Channels.Count];
            for (int i = 0; i < tmp.Length; i++)
            {
                tmp[i] = Channels[i].Name;
            }
            return tmp;
        }

        /// <summary>
        /// Returns an index of channel
        /// </summary>
        /// <returns>channel index or -1</returns>
        public int GetChannelndex( string name)
        {
            if (Channels.Count <= 0) return -1;
            for (int i = 0; i < Channels.Count; i++)
            {
                if( Channels[i].Name == name) return i;
            }
            return -1;
        }

        /// <summary>
        /// Returns and array of channel names in alphabetic order
        /// </summary>
        /// <returns></returns>
        public string[] GetChannelListAlphabetic()
        {
            if (Channels.Count <= 0) return null;
            List<string> tmp = new List<string>();
            for (int i = 0; i < Channels.Count; i++)
            {
                tmp.Add(Channels[i].Name);
            }
            tmp.Sort();
            return tmp.ToArray();
        }

        /// <summary>
        /// Deletes the unwanted channels
        /// </summary>
        /// <param name="names">list of channel names</param>
        public virtual bool DeleteChannels( List<string> names)
        {
            List<Oilfield_Channel> tmp = new List<Oilfield_Channel>();
            foreach (Oilfield_Channel oc in Channels)
            {
                if (names.Contains(oc.Name)) continue;
                tmp.Add(oc);
            }
            Channels = tmp;
            return true;
        }

        /// <summary>
        /// Creates the same channes as in the given file
        /// </summary>
        /// <param name="file"></param>
        public virtual void CreateSameChannels(Oilfield_File file)
        {
            for (int i = 1; i < file.Channels.Count; i++)
            {
                Oilfield_Channel lc = file.Channels[i];
                Oilfield_Channel tmp = this.GetChannel(lc.Name);
                if (tmp != null) continue;
                this.GetOrCreateChannel(lc.Name, lc.Unit, lc.Description, lc.Format);
            }
        }

        /// <summary>
        /// Launches associated Windows application
        /// </summary>
        public virtual void LaunchWinApplication()
        {
            if (String.IsNullOrEmpty( m_FileName))
            {
                throw new Exception("Unable to launch application. File name is not defined");
            }
            if (!File.Exists(m_FileName))
            {
                throw new Exception("Unable to launch application. File does not exist: " + m_FileName);
            }
            Process p = new Process();
            p.StartInfo.FileName = m_FileName;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            p.StartInfo.CreateNoWindow = false;
            p.Start();
        }

        #endregion
    }
}
