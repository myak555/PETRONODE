using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using Petronode.OilfieldFileAccess;
using Petronode.OilfieldFileAccess.Converters;

namespace Petronode.OilfieldFileAccess.Petrolog
{
    public class Petrolog_File: Oilfield_File
    {
        private const string c_wellheader_ext = ".wellheader";
        private const string c_logheader_ext = ".logheader";
        private const string c_logdata_ext = ".logdata";
        private const string c_logstats_ext = ".logstats";
        private string m_Name = "";
        private XmlDocument m_HeaderDocument;
        private int m_NumberOfLogs = 0;
        private int m_NumberOfColumns = 0;
        private double m_DepthStart = 0.0;
        private double m_DepthEnd = 0.0;
        private double m_DepthFrame = 0.0;
        private long m_RecordWidth = 0;
        private long m_DataStartPosition = 0;
        private long m_NumberOfDataRecords = 0;
        private long m_LogDataBlockSize = 0;
        private int m_NumberOfEmptyColumns = 0;
        private Log_Statistics_File m_Log_Statistics = null;

        public List<XmlNode> HeaderNodes = new List<XmlNode>();
        public Petrolog_Wellheader Wellheader = new Petrolog_Wellheader();

        #region Constructors
        /// <summary>
        /// Creates an empty class
        /// </summary>
        public Petrolog_File(): base()
        {
        }

        /// <summary>
        /// Creates a class from the logheader given
        /// </summary>
        public Petrolog_File( string filename): base()
        {
            FileInfo fi = new FileInfo(filename);
            string ext = fi.Extension;
            int pos = fi.Name.LastIndexOf(ext);
            m_Name = fi.DirectoryName + "\\" + fi.Name.Substring(0, pos);
            ParseLogHeader();
            Wellheader = new Petrolog_Wellheader(WellHeaderDataName);
            foreach (Petrolog_Constant pc in Wellheader.General)
            {
                this.Constants.Add((Oilfield_Constant)pc);
            }
            foreach (Petrolog_Wellheader wh in Wellheader.Runs)
            {
                string date = wh.Get_ConstantValue("DATE");
                if (date.Length > 0)
                {
                    this.Constants.Add(new Petrolog_Constant("DATE", "", date, "Job date"));
                    break;
                }
                string year = wh.Get_ConstantValue("YEAR");
                string month = wh.Get_ConstantValue("MONT");
                string day = wh.Get_ConstantValue("DAY");
                if (year.Length > 0 && month.Length > 0 && day.Length > 0)
                {
                    this.Constants.Add(new Petrolog_Constant("DATE", "", day + "/" + month + "/" + year, "Job date"));
                    break;
                }
            }
        }
        #endregion

        /// <summary>
        /// Gets or sets full name
        /// </summary>
        public override string FileName
        {
            get { return LogHeaderDataName; }
            set { m_FileName = value; }
        }

        /// <summary>
        /// Recreates the Imperial index
        /// </summary>
        public void CreateIndexImperial()
        {
            Channels[0].Unit = "FT";
            List<double> tmp = Channels[0].Data;
            tmp.Clear();
            double d0 = DepthStart;
            double dd = DepthFrame;
            double d1 = DepthEnd;
            if (dd == 0.0) return;
            Channels[0].ValidCount = 0;
            for (int i = 0; i < 1000000; i++)
            {
                double d = d0 + dd * i;
                Channels[0].ValidCount++;
                if (dd > 0.0 && d >= d1) break;
                if (dd < 0.0 && d <= d1) break;
                tmp.Add(d);
            }
            Channels[0].MissingCount = 0;
            Channels[0].DataStartIndex = 0;
            Channels[0].DataEndIndex = Channels[0].ValidCount - 1;
            Channels[0].DataStart = tmp[0];
            Channels[0].DataEnd = tmp[Channels[0].DataEndIndex];
            Channels[0].MinValue = Math.Min(d0, d1);
            Channels[0].MaxValue = Math.Max(d0, d1);
            Channels[0].Average = (d0 + d1) * 0.5;
        }

        /// <summary>
        /// Recreates the Metric index
        /// </summary>
        public void CreateIndexMetric()
        {
            Channels[0].Unit = "M";
            List<double> tmp = Channels[0].Data;
            tmp.Clear();
            double d0 = DepthStartMeters;
            double dd = DepthFrameMeters;
            double d1 = DepthEndMeters;
            if (dd == 0.0) return;
            Channels[0].ValidCount = 0;
            for (int i = 0; i < 1000000; i++)
            {
                double d = d0 + dd * i;
                tmp.Add(d);
                Channels[0].ValidCount++;
                if (dd > 0.0 && d >= d1) break;
                if (dd < 0.0 && d <= d1) break;
            }
            Channels[0].MissingCount = 0;
            Channels[0].DataStartIndex = 0;
            Channels[0].DataEndIndex = Channels[0].ValidCount-1;
            Channels[0].DataStart = tmp[0];
            Channels[0].DataEnd = tmp[Channels[0].DataEndIndex];
            Channels[0].MinValue = Math.Min(d0, d1);
            Channels[0].MaxValue = Math.Max(d0, d1);
            Channels[0].Average = (d0 + d1) * 0.5;
        }

        /// <summary>
        /// Returns the well header file name
        /// </summary>
        public string WellHeaderDataName
        {
            get { return m_Name + c_wellheader_ext; }
        }

        /// <summary>
        /// Returns the log header file name
        /// </summary>
        public string LogHeaderDataName
        {
            get { return m_Name + c_logheader_ext; }
        }

        /// <summary>
        /// Returns the log data file name
        /// </summary>
        public string LogDataName
        {
            get { return m_Name + c_logdata_ext; }
        }

        /// <summary>
        /// Returns the log statistics file name
        /// </summary>
        public string LogStatisticsName
        {
            get { return m_Name + c_logstats_ext; }
        }

        /// <summary>
        /// Returns number of logs in file
        /// </summary>
        public int NumberOfLogs { get { return m_NumberOfLogs; } }

        /// <summary>
        /// Returns total number of columns
        /// </summary>
        public int NumberOfColumns { get { return m_NumberOfColumns; } }

        /// <summary>
        /// Returns start depth
        /// </summary>
        public double DepthStart { get { return m_DepthStart; } }

        /// <summary>
        /// Returns start depth in meters
        /// </summary>
        public double DepthStartMeters { get { return m_DepthStart * 0.3048; } }

        /// <summary>
        /// Returns end depth
        /// </summary>
        public double DepthEnd { get { return m_DepthEnd; } }

        /// <summary>
        /// Returns end depth in meters
        /// </summary>
        public double DepthEndMeters { get { return m_DepthEnd * 0.3048; } }

        /// <summary>
        /// Returns depth frame
        /// </summary>
        public double DepthFrame { get { return m_DepthFrame; } }

        /// <summary>
        /// Returns depth frame in meters
        /// </summary>
        public double DepthFrameMeters { get { return m_DepthFrame * 0.3048; } }

        /// <summary>
        /// Returns depth frame
        /// </summary>
        public long RecordWidth { get { return m_RecordWidth; } }

        /// <summary>
        /// Returns data start position
        /// </summary>
        public long DataStartPosition { get { return m_DataStartPosition; } }

        /// <summary>
        /// Returns the record position corrsponding to depth
        /// </summary>
        /// <param name="index">index (depth or time)</param>
        /// <param name="metric">set to true if metric indexing is needed</param>
        /// <returns>-1 if outside of file boundaries, or actual record position</returns>
        public long GetRecordPosition(double index, bool metric)
        {
            if (metric) index /= 0.3048;
            if (index < m_DepthStart) return -1;
            if (index > m_DepthEnd) return -1;
            index = Math.Round((index - m_DepthStart) / m_DepthFrame, 0);
            long pos = Convert.ToInt64(index);
            if (pos >= m_NumberOfDataRecords) return -1;
            return m_DataStartPosition + pos * m_RecordWidth;
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
            foreach (Petrolog_Constant lc in Constants)
            {
                if (lc.Name != name) continue;
                lc.Value = value;
                return;
            }
            Petrolog_Constant newP = new Petrolog_Constant(name, unit, value, description);
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
            foreach (Petrolog_Constant lc in Parameters)
            {
                if (lc.Name != name) continue;
                lc.Value = value;
                return;
            }
            Petrolog_Constant newP = new Petrolog_Constant(name, unit, value, description);
            Parameters.Add(newP);
        }

        /// <summary>
        /// Returns the shallow copy of LAS index (first channel)
        /// </summary>
        public new Petrolog_Channel GetIndex()
        {
            return GetChannel(0);
        }

        /// <summary>
        /// Returns the deep copy of LAS index (first channel)
        /// </summary>
        public new Petrolog_Channel GetIndexCopy()
        {
            return GetChannelCopy(0);
        }

        /// <summary>
        /// Returns the shallow copy of Petrolog channel
        /// </summary>
        public new Petrolog_Channel GetChannel(int index)
        {
            Petrolog_Channel pc = (Petrolog_Channel)base.GetChannel(index);
            return pc;
        }

        /// <summary>
        /// Returns the shallow copy of Petrolog channel
        /// </summary>
        public new Petrolog_Channel GetChannel(string name)
        {
            Petrolog_Channel pc = (Petrolog_Channel)base.GetChannel(name);
            return pc;
        }

        /// <summary>
        /// Returns the deep copy of Petrolog channel
        /// </summary>
        public new Petrolog_Channel GetChannelCopy(int index)
        {
            if (index < 0 || index >= Channels.Count) return null;
            return new Petrolog_Channel(GetChannel(index));
        }

        /// <summary>
        /// Returns the deep copy of Petrolog channel
        /// </summary>
        public new Petrolog_Channel GetChannelCopy(string name)
        {
            foreach (Petrolog_Channel c in Channels)
            {
                if (c.Name != name) continue;
                return new Petrolog_Channel(c);
            }
            return null;
        }

        /// <summary>
        /// Gets a block of data from the specified interval
        /// </summary>
        /// <param name="name">Channel name</param>
        /// <param name="start_index">starting index</param>
        /// <param name="end_index">ending inedex</param>
        /// <param name="metric">set to true if the depth is in meters (by default Petrolog is FT)</param>
        /// <returns>null if the channel cannot be converted or not found, or array of values</returns>
        public double[] GetChannelValues(string name, double start_index, double end_index, bool metric)
        {
            // check for data file existence
            string dataFileName = this.LogDataName;
            if( !File.Exists( dataFileName)) throw new FileNotFoundException( "File not found: " + dataFileName);

            // locate channel
            Petrolog_Channel pc = GetChannel(name);
            if (pc == null) return null;
            if (pc.LogType != "TypeDouble" && pc.LogType != "TypeFloat") return null;

            // locate top and bottom positions
            if (metric)
            {
                start_index /= 0.3048;
                end_index /= 0.3048;
            }
            if (start_index < m_DepthStart) return null;
            if (end_index < start_index) return null;
            if (end_index > m_DepthEnd) return null;
            long startPosition = GetRecordPosition(start_index, false);
            long endPosition = GetRecordPosition(end_index, false);
            int offset = 8 + (pc.LogColumn - 1) * 4;

            // extract values
            FileStream reader = File.Open(dataFileName, FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite);
            reader.Seek(startPosition, SeekOrigin.Begin);
            byte[] buffer = new byte[m_RecordWidth];
            List<double> tmp = new List<double>();
            NumberUnion nu = new NumberUnion();
            while (true)
            {
                int i = reader.Read(buffer, 0, buffer.Length);
                if (i <= 0) break;
                if (reader.Position > endPosition) break;
                float f = BufferConverter.GetBytesFloat(buffer, nu, offset);
                tmp.Add( Convert.ToDouble( f));
            }
            reader.Close();
            return tmp.ToArray();
        }

        /// <summary>
        /// Saves the wellheader
        /// </summary>
        public void SaveWellHeader()
        {
            Wellheader.Write(WellHeaderDataName);
        }

        /// <summary>
        /// Forces wellheader
        /// </summary>
        public void ForceWellHeader( Petrolog_Wellheader wellheader)
        {
            Petrolog_Wellheader old = this.Wellheader;
            this.Wellheader = wellheader.Clone( this.m_HeaderDocument);
            foreach (Petrolog_Constant c in Wellheader.General)
            {
                string s = old.Get_ConstantValue(c.Name);
                if (s.Length <= 0) continue;
                c.Value = s;
            }
            foreach (Petrolog_Constant c in Wellheader.Remarks)
            {
                string s = old.Get_ConstantValue(c.Name);
                if (s.Length <= 0) continue;
                c.Value = s;
            }
            for (int i = 1; i <= Wellheader.Runs.Count; i++)
            {
                foreach (Petrolog_Constant c in Wellheader.Runs[i-1].General)
                {
                    string s = old.Get_ConstantValue(i, c.Name);
                    if (s.Length <= 0) continue;
                    c.Value = s;
                }
                foreach (Petrolog_Constant c in Wellheader.Runs[i-1].Remarks)
                {
                    string s = old.Get_ConstantValue(i, c.Name);
                    if (s.Length <= 0) continue;
                    c.Value = s;
                }
            }
        }

        /// <summary>
        /// Returns true if the file extension corresponds to Petrolog
        /// </summary>
        /// <param name="name">file name</param>
        /// <returns>true if the extension corresponds to LAS</returns>
        public static bool IsPetrologFile(string name)
        {
            FileInfo fi = new FileInfo(name);
            string ext = fi.Extension.ToLower();
            if (ext.StartsWith(".logdata")) return true;
            if (ext.StartsWith(".logheader")) return true;
            if (ext.StartsWith(".wellheader")) return true;
            return false;
        }

        /// <summary>
        /// Returns fully-qualified name of this Petrolog file
        /// </summary>
        /// <param name="postfix">Postfix to append</param>
        /// <returns>Fully-qualified name</returns>
        public override string GetQualifiedName(string postfix)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetConstant("CN"));
            sb.Append("_");
            sb.Append(GetConstant("FN"));
            sb.Append("_");
            sb.Append(GetConstant("WN"));
            if (postfix.Length > 0)
            {
                sb.Append("_");
                sb.Append(postfix);
            }
            return sb.ToString().Replace(' ', '_');
        }

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
                case "COMP":
                    s = GetConstant("CN");
                    return s.ToUpper();
                case "FLD":
                    s = GetConstant("FN");
                    return s.ToUpper();
                case "WELL":
                    s = GetConstant("WN");
                    return s.ToUpper();
                default:
                    break;
            }
            return "";
        }

        /// <summary>
        /// Writes data to file-set
        /// </summary>
        public override void Write( string filename)
        {
            FileInfo fi = new FileInfo(filename);
            string ext = fi.Extension;
            int pos = fi.Name.LastIndexOf(ext);
            m_Name = fi.DirectoryName + "\\" + fi.Name.Substring(0, pos);
            if (m_Log_Statistics != null) m_Log_Statistics.Write(LogStatisticsName);
            SaveWellHeader();
            SaveLogHeader();
        }

        /// <summary>
        /// Updates the channel statistics for all channels in file
        /// </summary>
        public override void UpdateChannelStatistics()
        {
            CreateIndexMetric();
            m_Log_Statistics = new Log_Statistics_File(this.Channels);
            bool res = m_Log_Statistics.Load(LogStatisticsName);
            if (res) return;

            // This is logic with the frame read - what are the limitations?
            FileStream reader = File.Open( LogDataName, FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite);
            reader.Seek(DataStartPosition, SeekOrigin.Begin);
            int[] ValidCounts = new int[Channels.Count];
            int[] MissingCounts = new int[Channels.Count];
            double[] Averages = new double[Channels.Count];
            double[] minValues = new double[Channels.Count];
            double[] maxValues = new double[Channels.Count];
            for (int i = 0; i < ValidCounts.Length; i++)
            {
                ValidCounts[i] = 0;
                MissingCounts[i] = 0;
                Averages[i] = 0;
                minValues[i] = Double.MaxValue;
                maxValues[i] = Double.MinValue;
            }
            int len = GetIndex().Data.Count;
            for (int i = 0; i < len; i++)
            {
                double[] frame = GetNextDataFrame(reader);
                if (frame == null) break;
                for (int j = 1; j < Channels.Count; j++)
                {
                    Petrolog_Channel c = (Petrolog_Channel)Channels[j];
                    double[] channelValues = c.GetFrameValues(frame);
                    for (int k = 0; k < channelValues.Length; k++)
                    {
                        double d = channelValues[k];
                        if (Double.IsNaN(d))
                        {
                            MissingCounts[j]++;
                            continue;
                        }
                        ValidCounts[j]++;
                        Averages[j] += d;
                        if (d < minValues[j]) minValues[j] = d;
                        if (d > maxValues[j]) maxValues[j] = d;
                    }
                }
            }
            for (int i = 1; i < Channels.Count; i++)
            {
                Petrolog_Channel c = (Petrolog_Channel)Channels[i];
                c.ValidCount = ValidCounts[i];
                c.MissingCount = MissingCounts[i];
                if (ValidCounts[i] > 0)
                {
                    c.Average = Averages[i] / Convert.ToDouble(ValidCounts[i]);
                    c.MinValue = minValues[i];
                    c.MaxValue = maxValues[i];
                }
                else
                {
                    c.Average = Double.NaN;
                    c.MinValue = Double.NaN;
                    c.MaxValue = Double.NaN;
                }
            }
            reader.Close();

            // This is logic with multiple reads - very slow!
            //for (int i = 1; i < Channels.Count; i++)
            //{
            //    Petrolog_Channel pc = GetChannel(i);
            //    double minValue = Double.MaxValue;
            //    double maxValue = Double.MinValue;
            //    double Average = 0.0;
            //    double AverageCount = 0.0;
            //    for (int dim = 0; dim < pc.LogDimension; dim++)
            //    {
            //        pc.LoadData( dim);
            //        pc.LocateDataBoundaries(this.GetIndex());
            //        double d = (double)pc.ValidCount;
            //        if (d < 1.0) continue;
            //        if (minValue > pc.MinValue) minValue = pc.MinValue;
            //        if (maxValue < pc.MaxValue) maxValue = pc.MaxValue;
            //        AverageCount += d;
            //        Average += pc.Average * d;
            //        pc.ClearData();
            //    }
            //    if (AverageCount < 1.0)
            //    {
            //        minValue = Double.NaN;
            //        maxValue = Double.NaN;
            //        Average = Double.NaN;
            //    }
            //    else
            //    {
            //        Average /= AverageCount;
            //    }
            //    pc.MinValue = minValue;
            //    pc.MaxValue = maxValue;
            //    pc.Average = Average;
            //}
            m_Log_Statistics.Write(LogStatisticsName);
        }

        /// <summary>
        /// gets entire data frame from position specified
        /// </summary>
        /// <param name="reader">File reader</param>
        /// <param name="position">position is file</param>
        /// <returns>Array of double</returns>
        public double[] GetDataFrame(FileStream reader, long position)
        {
            long startPosition = DataStartPosition + position * RecordWidth;
            reader.Seek(startPosition, SeekOrigin.Begin);
            byte[] buffer = new byte[RecordWidth];
            NumberUnion nu = new NumberUnion();
            int i = reader.Read(buffer, 0, buffer.Length);
            if (i <= 0) return null;
            long len = (RecordWidth - 8) >> 2;
            double[] output = new double[len+1];
            double d = BufferConverter.GetBytesDouble(buffer, nu, 0);
            if (d <= -1.0e30) d = Double.NaN;
            output[0] = d;
            for (i = 1; i <= len; i++)
            {
                d = BufferConverter.GetBytesFloat(buffer, nu, 4 + (i<<2));
                if (d <= -1.0e30) d = Double.NaN;
                output[i] = d;
            }
            return output;
        }

        /// <summary>
        /// gets entire data frame from position specified
        /// </summary>
        /// <param name="reader">File reader</param>
        /// <returns>Array of double</returns>
        public double[] GetNextDataFrame(FileStream reader)
        {
            byte[] buffer = new byte[RecordWidth];
            NumberUnion nu = new NumberUnion();
            int i = reader.Read(buffer, 0, buffer.Length);
            if (i <= 0) return null;
            long len = (RecordWidth - 8) >> 2;
            double[] output = new double[len + 1];
            double d = BufferConverter.GetBytesDouble(buffer, nu, 0);
            if (d <= -1.0e30) d = Double.NaN;
            output[0] = d;
            for (i = 1; i <= len; i++)
            {
                d = BufferConverter.GetBytesFloat(buffer, nu, 4 + (i << 2));
                if (d <= -1.0e30) d = Double.NaN;
                output[i] = d;
            }
            return output;
        }

        /// <summary>
        /// Changes units on all channels
        /// </summary>
        /// <param name="UnitFrom">Unit name to change from. If no match, channel is not changing</param>
        /// <param name="UnitTo">Unit name to change to</param>
        /// <param name="gain">Conversion gain</param>
        /// <param name="offset">Conversion offset</param>
        public override bool ChangeUnits(string UnitFrom, string UnitTo, double gain, double offset)
        {
            string filename1 = LogDataName;
            string filename2 = filename1 + "_tmp";
            if (!File.Exists(filename1)) return false;
            FileStream reader = File.Open(filename1, FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite);
            FileStream writer = File.Open(filename2, FileMode.CreateNew, System.IO.FileAccess.Write, FileShare.None);
            byte[] buffer = new byte[DataStartPosition];
            reader.Read(buffer, 0, buffer.Length);
            writer.Write(buffer, 0, buffer.Length);
            buffer = new byte[RecordWidth];
            NumberUnion nu = new NumberUnion();
            long totallength = (reader.Length - DataStartPosition) / RecordWidth;
            for (long i = 0; i < totallength; i++)
            {
                int count = reader.Read(buffer, 0, buffer.Length);
                if (count < buffer.Length) break;
                for (int j = 0; j < Channels.Count; j++)
                {
                    Petrolog_Channel pc = (Petrolog_Channel)Channels[j];
                    if (pc.Unit != UnitFrom) continue;
                    for (int k = 0; k < pc.LogDimension; k++)
                    {
                        int loc = (pc.LogColumnStart + k + 1) << 2;
                        double d = Convert.ToDouble(BufferConverter.GetBytesFloat(buffer, nu, loc));
                        if (d <= -1.0e30) continue;
                        d = d * gain + offset;
                        BufferConverter.SetBytesFloat(buffer, nu, Convert.ToSingle(d), loc);
                    }
                }
                writer.Write(buffer, 0, buffer.Length);
            }
            writer.Close();
            reader.Close();
            File.Delete(filename1);
            File.Move(filename2, filename1);
            bool result = false;
            for (int j = 0; j < Channels.Count; j++)
            {
                Petrolog_Channel pc = (Petrolog_Channel)Channels[j];
                if (pc.Unit != UnitFrom) continue;
                result = true;
                pc.Unit = UnitTo;
                if (!Double.IsNaN(pc.Average)) pc.Average = pc.Average * gain + offset;
                if (!Double.IsNaN(pc.MinValue)) pc.MinValue = pc.MinValue * gain + offset;
                if (!Double.IsNaN(pc.MaxValue)) pc.MaxValue = pc.MaxValue * gain + offset;
            }
            return result;
        }

        /// <summary>
        /// Deletes the unwanted channels
        /// </summary>
        /// <param name="names">list of channel names to remove</param>
        public override bool DeleteChannels(List<string> names)
        {
            // create byte map
            int[] bytemap = new int[RecordWidth];
            for (int i = 0; i < bytemap.Length; i++) bytemap[i] = i;
            int newRecordWidth = bytemap.Length;
            List<Petrolog_Channel> channelCopy = new List<Petrolog_Channel>();
            foreach (Petrolog_Channel pc in Channels) channelCopy.Add( new Petrolog_Channel(pc));
            foreach (Petrolog_Channel pc in Channels)
            {
                if (!names.Contains(pc.Name)) continue;
                int datastart = (pc.LogColumnStart + 1) << 2;
                int datashift = pc.LogDimension << 2;
                for (int i = datastart; i < datastart + datashift; i++) bytemap[i] = -1;
                for (int i = datastart + datashift; i < bytemap.Length; i++)
                {
                    if (bytemap[i] < 0) continue;
                    bytemap[i] -= datashift;
                }
                newRecordWidth -= datashift;
            }

            // create mapped channels
            List<Oilfield_Channel> mappedChannels = new List<Oilfield_Channel>();
            foreach (Petrolog_Channel pc in channelCopy)
            {
                if (names.Contains(pc.Name)) continue;
                mappedChannels.Add(pc);
            }
            int logNumber = 0;
            int logColumnNumber = 0;
            foreach (Petrolog_Channel pc in mappedChannels)
            {
                pc.LogNumber = logNumber;
                pc.LogColumn = logColumnNumber;
                pc.LogColumnStart = logColumnNumber;
                logNumber++;
                logColumnNumber += pc.LogDimension;
            }

            // the new header length is 0x67 + (logColumnNumber + EmptyColumns) * 48
            // don't ask me why. It is a crap from Petrolog 1.0 time
            int newHeaderLength = 0x67 + (logColumnNumber + m_NumberOfEmptyColumns) * 48;

            // copy file via a temporary one
            string filename1 = LogDataName;
            string filename2 = filename1 + "_tmp";
            if (!File.Exists(filename1)) return false;
            FileStream reader = File.Open(filename1, FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite);
            FileStream writer = File.Open(filename2, FileMode.CreateNew, System.IO.FileAccess.Write, FileShare.None);
            byte[] buffer = new byte[DataStartPosition];
            byte[] newbuffer = new byte[newHeaderLength];
            reader.Read(buffer, 0, buffer.Length);
            int index = 0;
            for (int i = 0; i < 0x67; i++) newbuffer[index++] = buffer[i];
            NumberUnion nu = new NumberUnion();
            BufferConverter.SetBytesInt32(newbuffer, nu, newHeaderLength, 0x0);
            BufferConverter.SetBytesInt32(newbuffer, nu, logColumnNumber, 0x33);
            BufferConverter.SetBytesInt32(newbuffer, nu, logNumber, 0x33 + 4);
            //BufferConverter.SetBytesInt32(newbuffer, nu, m_NumberOfDataRecords, 0x33 + 8); Very strange, converting long to int
            BufferConverter.SetBytesInt32(newbuffer, nu, newRecordWidth, 0x33 + 12);
            int channelIndex = 0;
            for (int i = 0x67; i < newbuffer.Length; i += 48)
            {
                if( channelIndex >= mappedChannels.Count) break;
                for (int j = 0; j < 40; j++) newbuffer[i + j] = 0x20;
                for (int j = 40; j < 48; j++) newbuffer[i + j] = 0x0;
                if (channelIndex == 0)
                {
                    newbuffer[i] = 0x44;
                    newbuffer[i + 1] = 0x45;
                    newbuffer[i + 2] = 0x50;
                    newbuffer[i + 3] = 0x54;
                    newbuffer[i + 4] = 0x48;
                    newbuffer[i + 20] = 0x46;
                    newbuffer[i + 21] = 0x54;
                    newbuffer[i + 40] = 0x01;
                    newbuffer[i + 44] = 0x01;
                }
                else
                {
                    Petrolog_Channel pc = (Petrolog_Channel)mappedChannels[channelIndex];
                    for (int j = 0; j < pc.Name.Length && j < 20; j++) newbuffer[i + j] = (byte)(pc.Name[j]);
                    for (int j = 0; j < pc.Unit.Length && j < 20; j++) newbuffer[i + j + 20] = (byte)(pc.Unit[j]);
                    BufferConverter.SetBytesInt32(newbuffer, nu, pc.LogDimension, i+40);
                }
                channelIndex++;
                index += 48;
            }
            while (index < newbuffer.Length) newbuffer[index++] = 0x0;
            writer.Write(newbuffer, 0, newbuffer.Length);
            buffer = new byte[RecordWidth];
            newbuffer = new byte[newRecordWidth];
            for (long i = 0; i < m_NumberOfDataRecords; i++)
            {
                int count = reader.Read(buffer, 0, buffer.Length);
                if (count < buffer.Length) break;
                for (int j = 0; j < bytemap.Length; j++)
                {
                    int k = bytemap[j];
                    if( k < 0) continue;
                    newbuffer[k] = buffer[j];
                }
                //double depth_index = m_DepthStart + m_DepthFrame * Convert.ToDouble(i);
                BufferConverter.SetBytesDouble(newbuffer, nu, 0.0, 0);
                writer.Write(newbuffer, 0, newbuffer.Length);
            }
            writer.Close();
            reader.Close();
            File.Delete(filename1);
            File.Move(filename2, filename1);

            // update file details
            m_RecordWidth = newRecordWidth;
            m_NumberOfLogs = logNumber;
            m_NumberOfColumns = logColumnNumber;
            m_DataStartPosition = newHeaderLength;
            m_LogDataBlockSize = (long)newRecordWidth * (long)m_NumberOfDataRecords;
            Channels = mappedChannels;
            m_Log_Statistics = new Log_Statistics_File(Channels);
            this.Write();
            return true;
        }

        /// <summary>
        /// Remaps the channels
        /// </summary>
        /// <param name="mapping">list of maps</param>
        public bool RemapChannels(List<Channel_Map> mapping)
        {
            // create byte map
            int[] bytemap = new int[RecordWidth];
            for (int i = 0; i < bytemap.Length; i++) bytemap[i] = i;
            foreach (Channel_Map cm in mapping)
            {
                foreach (Petrolog_Channel pc in Channels)
                {
                    if (pc.Name != cm.Name) continue;
                    int dataDestination = (pc.LogColumnStart + 1) << 2;
                    for (int i = 0; i < 4; i++) bytemap[dataDestination + i] = -1;
                    foreach (string s in cm.Map)
                    {
                        Petrolog_Channel srcChannel = GetChannel(s);
                        if (srcChannel == null) continue;
                        int dataSource = (srcChannel.LogColumnStart + 1) << 2;
                        for (int i = 0; i < 4; i++) bytemap[dataDestination + i] = dataSource + i;
                        pc.ValidCount = srcChannel.ValidCount;
                        pc.MissingCount = srcChannel.MissingCount;
                        pc.MinValue = srcChannel.MinValue;
                        pc.MaxValue = srcChannel.MaxValue;
                        pc.Average = srcChannel.Average;
                        break; // channel found
                    }
                }
            }

            // copy file via a temporary one
            string filename1 = LogDataName;
            string filename2 = filename1 + "_tmp";
            if (!File.Exists(filename1)) return false;
            FileStream reader = File.Open(filename1, FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite);
            FileStream writer = File.Open(filename2, FileMode.CreateNew, System.IO.FileAccess.Write, FileShare.None);
            byte[] buffer = new byte[DataStartPosition];
            reader.Read(buffer, 0, buffer.Length);
            writer.Write(buffer, 0, buffer.Length);
            buffer = new byte[RecordWidth];
            byte[] newbuffer = new byte[RecordWidth];
            for (long i = 0; i < m_NumberOfDataRecords; i++)
            {
                int count = reader.Read(buffer, 0, buffer.Length);
                if (count < buffer.Length) break;
                for (int j = 0; j < bytemap.Length; j++)
                {
                    int k = bytemap[j];
                    if (k < 0) continue;
                    newbuffer[j] = buffer[k];
                }
                writer.Write(newbuffer, 0, newbuffer.Length);
            }
            writer.Close();
            reader.Close();
            File.Delete(filename1);
            File.Move(filename2, filename1);

            // update file details
            this.Write();
            return true;
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

        #region Private Methods
        /// <summary>
        /// Saves the top-level header
        /// </summary>
        private void SaveLogHeader()
        {
            string name = m_Name + c_logheader_ext;
            StreamWriter writer = File.CreateText(name);
            writer.WriteLine( "<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            writer.WriteLine( "<DataFile xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" noNamespaceSchemaLocation=\"C:\\Program Files\\CDP\\Petrolog 32Bit\\Default\\Schemas\\DataFile.xsd\">");
            writer.WriteLine( "  <Version>10.7</Version>");
            writer.WriteLine( "  <FileBase>Depth</FileBase>");
            writer.WriteLine( "  <FileType>RawData</FileType>");
            writer.WriteLine( "  <NumberOfLogs>" + m_NumberOfLogs.ToString() + "</NumberOfLogs>");
            writer.WriteLine( "  <NumberOfColumns>" + m_NumberOfColumns.ToString() + "</NumberOfColumns>");
            writer.WriteLine( "  <DepthStart>" + m_DepthStart.ToString() + "</DepthStart>");
            writer.WriteLine( "  <DepthEnd>" + m_DepthEnd.ToString() + "</DepthEnd>");
            writer.WriteLine( "  <DepthFrame>" + m_DepthFrame.ToString() + "</DepthFrame>");
            writer.WriteLine( "  <NumberOfDataRecords>" + m_NumberOfDataRecords.ToString() + "</NumberOfDataRecords>");
            writer.WriteLine( "  <LogDataRecordWidth>" + m_RecordWidth.ToString() + "</LogDataRecordWidth>");
            writer.WriteLine( "  <LogDataStartPosition>" + m_DataStartPosition.ToString() + "</LogDataStartPosition>");
            writer.WriteLine( "  <LogDataBlockSize>" + m_LogDataBlockSize.ToString() + "</LogDataBlockSize>");
            writer.WriteLine( "  <NumberOfEmptyColumns>" + m_NumberOfEmptyColumns.ToString() + "</NumberOfEmptyColumns>");
            writer.WriteLine( "  <LogInfos>");
            for (int i = 0; i < Channels.Count; i++ )
            {
                Petrolog_Channel c = (Petrolog_Channel)(Channels[i]);
                writer.WriteLine("    <LogInfo>");
                writer.WriteLine("      <LogType>" + c.LogType + "</LogType>");
                writer.WriteLine("      <LogName>" + c.Name + "</LogName>");
                writer.WriteLine("      <LogColumn>" + c.LogColumn.ToString() + "</LogColumn>");
                writer.WriteLine("      <LogNumber>" + c.LogNumber.ToString() + "</LogNumber>");
                if (i == 0)
                {
                    writer.WriteLine("      <LogUnit>FT</LogUnit>");
                }
                else
                {
                    if (c.Unit.Length > 0) writer.WriteLine("      <LogUnit>" + c.Unit + "</LogUnit>");
                    else writer.WriteLine("      <LogUnit />");
                }
                if (c.Description.Length > 0)
                {
                    string deBang = c.Description.Replace("<", "&lt;");
                    deBang = deBang.Replace(">", "&gt;");
                    writer.WriteLine("      <LogDescription>" + deBang + "</LogDescription>");
                }
                else
                {
                    writer.WriteLine("      <LogDescription />");
                }
                if (c.LogTool.Length > 0) writer.WriteLine("      <LogTool>" + c.LogTool + "</LogTool>");
                else writer.WriteLine("      <LogTool />");
                writer.WriteLine("      <LogStatus>None</LogStatus>");
                writer.WriteLine("      <LogKey />");
                writer.WriteLine("      <LogDimension>" + c.LogDimension.ToString() + "</LogDimension>");
                writer.WriteLine("      <LogColumnStart>" + c.LogColumnStart.ToString() + "</LogColumnStart>");
                writer.WriteLine("      <OriginalDepthFrame>0.00656167979002624</OriginalDepthFrame>");
                writer.WriteLine("      <ArrayLogInfo>");
                writer.WriteLine("        <ArrayToolType />");
                writer.WriteLine("        <ArrayXUnit />");
                writer.WriteLine("        <ArrayXSpacing>1</ArrayXSpacing>");
                writer.WriteLine("        <ArrayXStart>0</ArrayXStart>");
                writer.WriteLine("      </ArrayLogInfo>");
                writer.WriteLine("      <LogInfoStatistics>");
                writer.WriteLine("        <Dirty>true</Dirty>");
                writer.WriteLine("        <SampleRate>1</SampleRate>");
                writer.WriteLine("        <MinimumValue>-1E+30</MinimumValue>");
                writer.WriteLine("        <MaximumValue>-1E+30</MaximumValue>");
                writer.WriteLine("        <StatisticalMinimumValue>-1E+30</StatisticalMinimumValue>");
                writer.WriteLine("        <StatisticalMaximumValue>-1E+30</StatisticalMaximumValue>");
                writer.WriteLine("        <PercentageCutoff>2.5</PercentageCutoff>");
                writer.WriteLine("        <NetIntervalTvd>0</NetIntervalTvd>");
                writer.WriteLine("        <NumberOfSamples>0</NumberOfSamples>");
                writer.WriteLine("        <NumberOfHistogramPoints>0</NumberOfHistogramPoints>");
                writer.WriteLine("        <MeanArithmetic>0</MeanArithmetic>");
                writer.WriteLine("        <MeanGeometric>0</MeanGeometric>");
                writer.WriteLine("        <MeanHarmonic>0</MeanHarmonic>");
                writer.WriteLine("        <StandardDeviation>0</StandardDeviation>");
                writer.WriteLine("        <Variance>0</Variance>");
                writer.WriteLine("        <Median>0</Median>");
                writer.WriteLine("        <Skewness>0</Skewness>");
                writer.WriteLine("        <Kurtosis>0</Kurtosis>");
                writer.WriteLine("        <Histogram>0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0</Histogram>");
                writer.WriteLine("      </LogInfoStatistics>");
                writer.WriteLine("      <ProcessingType />");
                writer.WriteLine("    </LogInfo>");
            }
            writer.WriteLine( "  </LogInfos>");
            writer.WriteLine( "</DataFile>");
            writer.Close();
        }
        
        /// <summary>
        /// Parses the top-level header
        /// </summary>
        private void ParseLogHeader()
        {
            string name = m_Name + c_logheader_ext;
            if( !File.Exists( name)) return;
            StreamReader reader = File.OpenText( name);
            m_HeaderDocument = new XmlDocument();
            m_HeaderDocument.Load(reader.BaseStream);
            reader.Close();
            foreach (XmlNode xn in m_HeaderDocument.ChildNodes)
            {
                if (xn.NodeType == XmlNodeType.XmlDeclaration) continue;
                ParseLogHeaderContents(xn);
            }
        }

        /// <summary>
        /// Parses header contents
        /// </summary>
        private void ParseLogHeaderContents(XmlNode node)
        {
            foreach (XmlNode xn in node.ChildNodes)
            {
                if (xn.LocalName == "LogInfos")
                {
                    ParseLogInfos( xn);
                    continue;
                }
                if (xn.LocalName == "NumberOfLogs") m_NumberOfLogs = Convert.ToInt32(xn.InnerText);
                if (xn.LocalName == "NumberOfColumns") m_NumberOfColumns = Convert.ToInt32(xn.InnerText);
                if (xn.LocalName == "DepthStart") m_DepthStart = Convert.ToDouble(xn.InnerText);
                if (xn.LocalName == "DepthEnd") m_DepthEnd = Convert.ToDouble(xn.InnerText);
                if (xn.LocalName == "DepthFrame") m_DepthFrame = Convert.ToDouble(xn.InnerText);
                if (xn.LocalName == "LogDataRecordWidth") m_RecordWidth = Convert.ToInt64(xn.InnerText);
                if (xn.LocalName == "LogDataStartPosition") m_DataStartPosition = Convert.ToInt64(xn.InnerText);
                if (xn.LocalName == "NumberOfDataRecords") m_NumberOfDataRecords = Convert.ToInt64(xn.InnerText);
                if (xn.LocalName == "LogDataBlockSize") m_LogDataBlockSize = Convert.ToInt64(xn.InnerText);
                if (xn.LocalName == "NumberOfEmptyColumns") m_NumberOfEmptyColumns = Convert.ToInt32(xn.InnerText);
                HeaderNodes.Add(xn);
            }
            return;
        }

        /// <summary>
        /// Parses log infos
        /// </summary>
        private void ParseLogInfos(XmlNode node)
        {
            foreach (XmlNode xn in node.ChildNodes)
            {
                Petrolog_Channel v = new Petrolog_Channel(xn, this);
                Channels.Add(v);
            }
        }
        #endregion
    }
}
