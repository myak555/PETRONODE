using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using Petronode.OilfieldFileAccess.Converters;

namespace Petronode.OilfieldFileAccess.Petrolog
{
    public class Petrolog_Channel:Oilfield_Channel
    {
        protected XmlNode m_Node = null;
        protected Petrolog_File m_Parent = null;

        public string LogType = "unknown";
        public int LogColumn = 0;
        public int LogNumber = 0;
        public string LogTool = "";
        public int LogDimension = 1;
        public int LogColumnStart = 0;

        /// <summary>
        /// Creates an empty variable
        /// </summary>
        public Petrolog_Channel()
        {
        }

        /// <summary>
        /// Creates a variable from Xml description
        /// </summary>
        /// <param name="node"></param>
        public Petrolog_Channel(XmlNode node, Petrolog_File parent)
        {
            m_Node = node;
            m_Parent = parent;
            foreach( XmlNode xn in node.ChildNodes)
            {
                if( xn.LocalName == "LogType") LogType = xn.InnerText;
                if( xn.LocalName == "LogName") Name = xn.InnerText;
                if( xn.LocalName == "LogColumn") LogColumn = Convert.ToInt32( xn.InnerText);
                if( xn.LocalName == "LogNumber") LogNumber = Convert.ToInt32( xn.InnerText);
                if( xn.LocalName == "LogUnit") Unit = xn.InnerText;
                if( xn.LocalName == "LogDescription") Description = xn.InnerText;
                if( xn.LocalName == "LogTool") LogTool = xn.InnerText;
                if( xn.LocalName == "LogDimension") LogDimension = Convert.ToInt32( xn.InnerText);
                if( xn.LocalName == "LogColumnStart") LogColumnStart = Convert.ToInt32( xn.InnerText);
            }
            SetProcesorByType();
        }

        /// <summary>
        /// Creates a deep copy of a given channel
        /// </summary>
        /// <param name="c"></param>
        public Petrolog_Channel(Oilfield_Channel c): base( (Oilfield_Channel)c)
        {
            if (c.GetType() != this.GetType()) return;
            Petrolog_Channel input = (Petrolog_Channel)c;
            this.m_Node = input.m_Node;
            this.m_Parent = input.m_Parent;
            this.LogType = input.LogType;
            this.LogColumn = input.LogColumn;
            this.LogNumber = input.LogNumber;
            this.LogTool = input.LogTool;
            this.LogDimension = input.LogDimension;
            this.LogColumnStart = input.LogColumnStart;
        }

        /// <summary>
        /// Loads the data from file
        /// </summary>
        /// <returns>The datalist</returns>
        public override List<double> LoadData()
        {
            return this.LoadData(0);
        }

        /// <summary>
        /// Loads the data from file
        /// </summary>
        /// <returns>The datalist</returns>
        public override List<double> LoadData( int dimension)
        {
            if (this.LogColumn == 0) return Data;
            Data.Clear();
            if (m_Parent == null) return Data;
            if (dimension < 0 || dimension >= LogDimension) return Data;
            string filename = m_Parent.LogDataName;
            if (!File.Exists(filename)) return Data;
            FileStream reader = File.Open(filename, FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite);
            ProcessLoad(reader, dimension);
            reader.Close();
            return Data;
        }

        /// <summary>
        /// Saves the data to file
        /// </summary>
        public override void SaveData()
        {
            this.SaveData(0);
        }

        /// <summary>
        /// Saves the data to file
        /// </summary>
        public override void SaveData( int dimension)
        {
            if (m_Parent == null) return;
            string filename = m_Parent.LogDataName;
            if (!File.Exists(filename)) return;
            FileStream writer = File.Open(filename, FileMode.Open, System.IO.FileAccess.Write, FileShare.Read);
            ProcessSave(writer, dimension);
            writer.Close();
            
            // This is for debugging only
            //filename = m_Parent.LogDataName + ".csv";
            //StreamWriter sw = File.CreateText(filename);
            //sw.WriteLine( this.Name);
            //foreach (double d in Data)
            //{
            //    sw.WriteLine(d.ToString());
            //}
            //sw.Close();
        }

        /// <summary>
        /// Locates the upper and lower boundaries for the channel
        /// </summary>
        /// <param name="index">index channel or null</param>
        /// <returns>true is boundearies are located</returns>
        public bool LocateDataBoundaries(Petrolog_Channel index)
        {
            return base.LocateDataBoundaries((Oilfield_Channel)index);
        }

        /// <summary>
        /// Returns the channel average from start to end depth inclusive.
        /// </summary>
        /// <param name="start">start index</param>
        /// <param name="end">end index</param>
        /// <param name="index">index channel</param>
        /// <returns>ariphmetic average or NaN</returns>
        public double GetAverage(double start, double end, Petrolog_Channel index)
        {
            return base.GetAverage(start, end, (Oilfield_Channel)index);
        }

        /// <summary>
        /// Extracts frame values from the full frame
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public double[] GetFrameValues(double[] input)
        {
            if (LogDimension < 1) return null;
            double[] output = new double[LogDimension];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = input[LogColumnStart + i];
            }
            return output;
        }

        /// <summary>
        /// Returns desctiption strings
        /// </summary>
        public override string[] ToStrings(int type)
        {
            if (type == 1)
            {
                string[] tmp = new string[4];
                tmp[0] = this.Name;
                tmp[1] = this.Unit;
                tmp[2] = this.Description;
                return tmp;
            }
            if (type == 2)
            {
                string[] tmp = new string[4];
                tmp[0] = this.Name;
                tmp[1] = this.Unit;
                tmp[2] = this.LogType;
                tmp[3] = this.Description;
                return tmp;
            }
            if (type == 3)
            {
                string[] tmp = new string[7];
                tmp[0] = this.Name;
                tmp[1] = this.Unit;
                tmp[2] = this.LogType;
                tmp[3] = this.Description;
                tmp[4] = this.MissingCount.ToString();
                tmp[5] = this.MinValue.ToString(this.Format);
                tmp[6] = this.MaxValue.ToString(this.Format);
                return tmp;
            }
            if (type == 4)
            {
                string[] tmp = new string[9];
                tmp[0] = this.Name;
                tmp[1] = this.Unit;
                tmp[2] = this.LogType;
                tmp[3] = this.Description;
                tmp[4] = this.ValidCount.ToString();
                tmp[5] = this.MissingCount.ToString();
                tmp[6] = this.MinValue.ToString(this.Format);
                tmp[7] = this.MaxValue.ToString(this.Format);
                tmp[8] = this.Average.ToString(this.Format);
                return tmp;
            }
            return ToStrings();
        }

        /// <summary>
        /// Changes the channel units
        /// </summary>
        /// <param name="UnitFrom">Unit name to change from. If no match, channel is not changing</param>
        /// <param name="UnitTo">Unit name to change to</param>
        /// <param name="gain">Conversion gain</param>
        /// <param name="offset">Conversion offset</param>
        public override bool ChangeUnits(string UnitFrom, string UnitTo, double gain, double offset)
        {
            if (this.Unit != UnitFrom) return false;
            this.Unit = UnitTo;
            string filename = m_Parent.LogDataName;
            if (!File.Exists(filename)) return false;
            FileStream reader_writer = File.Open(filename, FileMode.Open, System.IO.FileAccess.ReadWrite, FileShare.Read);
            long startPosition = m_Parent.DataStartPosition + (long)((this.LogColumnStart + 1) << 2);
            reader_writer.Seek(startPosition, SeekOrigin.Begin);
            byte[] buffer = new byte[ LogDimension << 2];
            long step = (long)(LogDimension << 2);
            NumberUnion nu = new NumberUnion();
            long totallength = (reader_writer.Length - m_Parent.DataStartPosition) / m_Parent.RecordWidth;
            for( long k=0; k<totallength; k++)
            {
                int i = reader_writer.Read(buffer, 0, buffer.Length);
                if( i<buffer.Length) break;
                for (int j = 0; j < step; j+=4)
                {
                    double d = Convert.ToDouble( BufferConverter.GetBytesFloat(buffer, nu, j));
                    if ( d <= -1.0e30) continue;
                    d = d * gain + offset;
                    BufferConverter.SetBytesFloat(buffer, nu, Convert.ToSingle(d), j);
                }
                reader_writer.Seek(-step, SeekOrigin.Current);
                reader_writer.Write(buffer, 0, buffer.Length);
                reader_writer.Seek(m_Parent.RecordWidth-step, SeekOrigin.Current);
            }
            reader_writer.Close();
            if (!Double.IsNaN(this.Average)) this.Average = this.Average * gain + offset;
            if (!Double.IsNaN(this.MinValue)) this.MinValue = this.MinValue * gain + offset;
            if (!Double.IsNaN(this.MaxValue)) this.MaxValue = this.MaxValue * gain + offset;
            return true;
        }

        #region Private Methods
        private delegate void FileProcessor(FileStream reader, int dimension);
        private FileProcessor ProcessLoad = null;
        private FileProcessor ProcessSave = null;

        private void NullBufferLoadProcessor(FileStream reader, int dimension)
        {
            DoubleBufferLoadProcessor(reader, dimension);
            for (int i = 0; i < Data.Count; i++) Data[i] = Double.NaN;
        }

        private void DoubleBufferLoadProcessor(FileStream reader, int dimension)
        {
            long startPosition = m_Parent.DataStartPosition;
            reader.Seek(startPosition, SeekOrigin.Begin);
            byte[] buffer = new byte[8];
            NumberUnion nu = new NumberUnion();
            while (true)
            {
                int i = reader.Read(buffer, 0, buffer.Length);
                if (i <= 0) break;
                double d = BufferConverter.GetBytesDouble(buffer, nu, 0);
                if (d <= -1.0e30) d = Double.NaN;
                Data.Add(d);
                reader.Seek(m_Parent.RecordWidth - 8, SeekOrigin.Current);
            }
        }

        private void FloatBufferLoadProcessor(FileStream reader, int dimension)
        {
            long startPosition = m_Parent.DataStartPosition;
            int recordWidth = (int)m_Parent.RecordWidth;
            int offset = (this.LogColumnStart+dimension+1)<<2;
            reader.Seek(startPosition, SeekOrigin.Begin);
            byte[] buffer = new byte[recordWidth];
            NumberUnion nu = new NumberUnion();
            while (true)
            {
                int i = reader.Read(buffer, 0, buffer.Length);
                if (i < buffer.Length) break;
                double d = Convert.ToDouble( BufferConverter.GetBytesFloat(buffer, nu, offset));
                if (d <= -1.0e30) d = Double.NaN;
                Data.Add(d);
            }
        }

        private void NullBufferSaveProcessor(FileStream writer, int dimension)
        {
        }

        private void DoubleBufferSaveProcessor(FileStream writer, int dimension)
        {
            long startPosition = m_Parent.DataStartPosition;
            writer.Seek(startPosition, SeekOrigin.Begin);
            byte[] buffer = new byte[8];
            NumberUnion nu = new NumberUnion();
            for( int i=0; i<Data.Count; i++)
            {
                double d = Data[i];
                if (Double.IsNaN( d)) d = -1.0e30;
                BufferConverter.SetBytesDouble(buffer, nu, d, 0);
                writer.Write( buffer, 0, buffer.Length);
                writer.Seek(m_Parent.RecordWidth - 8, SeekOrigin.Current);
            }
        }

        private void FloatBufferSaveProcessor(FileStream writer, int dimension)
        {
            long startPosition = m_Parent.DataStartPosition + (long)((this.LogColumnStart + dimension + 1) << 2);
            writer.Seek(startPosition, SeekOrigin.Begin);
            byte[] buffer = new byte[4];
            NumberUnion nu = new NumberUnion();
            for (int i = 0; i < Data.Count; i++)
            {
                double d = Data[i];
                if (Double.IsNaN(d)) d = -1.0e30;
                BufferConverter.SetBytesFloat(buffer, nu, Convert.ToSingle(d), 0);
                writer.Write(buffer, 0, buffer.Length);
                writer.Seek(m_Parent.RecordWidth - 4, SeekOrigin.Current);
            }
        }

        private void SetProcesorByType()
        {
            switch (LogType)
            {
                case "TypeDouble":
                    ProcessLoad = new FileProcessor(this.DoubleBufferLoadProcessor);
                    ProcessSave = new FileProcessor(this.DoubleBufferSaveProcessor);
                    break;
                case "TypeFloat": 
                    ProcessLoad = new FileProcessor(this.FloatBufferLoadProcessor);
                    ProcessSave = new FileProcessor(this.FloatBufferSaveProcessor);
                    break;
                default:
                    ProcessLoad = new FileProcessor(this.NullBufferLoadProcessor);
                    ProcessLoad = new FileProcessor(this.NullBufferSaveProcessor);
                    break;
            }
        }
        #endregion
    }
}
