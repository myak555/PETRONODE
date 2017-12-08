using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Petronode.OilfieldFileAccess;

namespace Petronode.OilfieldFileAccess.LDF
{
    public class LDF_File: Oilfield_File
    {
        public byte[] ProcessingHistory = null;
        public byte[] CommonHeader = new byte[1024];
        public int UndefinedInt = 2147483647;
        public float UndefinedFloat = 3.402823e+38f;

        #region Constructors
        /// <summary>
        /// Constructor; creates an empty LDF file
        /// </summary>
        public LDF_File(): base()
        {
            SetDataSchema();
        }
        
        /// <summary>
        /// Constructor; reads the LDF file into memory (including traces)
        /// </summary>
        /// <param name="filename">File name</param>
        public LDF_File(string filename): base( filename)
        {
            SetDataSchema();
            ParseFile(false);
        }

        /// <summary>
        /// Constructor; reads the LDF file into memory (traces optional)
        /// </summary>
        /// <param name="filename">File name</param>
        /// <param name="readdata">Reads data if true</param>
        public LDF_File(string filename, bool readdata)
            : base(filename)
        {
            SetDataSchema();
            ParseFile(readdata);
        }

        private void ParseFile( bool readdata)
        {
            FileStream fs = null;
            try
            {
                fs = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                fs.Read(CommonHeader, 0, CommonHeader.Length);
                foreach( LDF_Constant c in Constants) c.GetBuffer( CommonHeader);

                long nsamples = Convert.ToInt64(GetConstant("Number of Samples"));
                double interval = Convert.ToDouble(GetConstant("Sampling Rate"));
                string trace_measures = GetConstant("Along Measurement");
                if (interval <= 0.0)
                {
                    SetConstant("Sampling Rate", "1.0");
                    interval = 1.0;
                }

                // TODO -- change the along trace units as necessary 

                LDF_Channel index = ( trace_measures.ToUpper().StartsWith( "D"))?
                    new LDF_Channel(this, "Depth", "m") : new LDF_Channel(this, "Time", "ms");
                if (readdata)
                {
                    for (int i = 0; i < nsamples; i++) index.Data.Add(interval * i);
                }
                Channels.Add(index);

                // Unlike SEG-Y, the traces in the LDF are always 4-byte floats
                // The trace header length is always 588 bytes
                long channelLength = 588L + (nsamples<<2);
                byte[] Buffer = new byte[channelLength];
                int traceCount = 0;

                // Note that in the LDF files all length units are in always ft
                // For most applications a conversion on the flight is required
                string globUnit = (GetConstant("Across Measurement") == "ft") ? "ft" : "m";
                int ntraces = Convert.ToInt32(GetConstant("Number of Levels"));

                // Read the file by the declared number of traces
                for (int i=0; i<ntraces; i++)
                {
                    int read = fs.Read(Buffer, 0, Buffer.Length);
                    if (read < Buffer.Length) break;
                //    LDF_Channel c = new LDF_Channel(traceCount.ToString(), "sample", Buffer, readdata);
                //    foreach (LDF_Constant cc in c.Parameters)
                //    {
                //        if (cc.Unit == "unitless") continue;
                //        if (cc.Unit == "elevation") cc.Unit = globUnit;
                //        if (cc.Unit == "coordinate") cc.Unit = globUnit;
                //        if (cc.Unit == "velocity") cc.Unit = globUnit + "/s";
                //    }
                //    this.Channels.Add(c);
                    traceCount++;
                }

                // Determine how much left at the end
                // This may be a processing history or EBCDIC text
                long historyLocation = fs.Position;
                long historyLength = fs.Length - historyLocation;
                if( historyLength > 0L)
                {
                    ProcessingHistory = new byte[ historyLength];
                    fs.Read(ProcessingHistory, 0, ProcessingHistory.Length);
                }
                SetConstant("Number of Levels", traceCount.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if( fs!= null) fs.Close();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes the common header to a file
        /// </summary>
        /// <param name="fs"></param>
        public void WriteCommonHeader(FileStream fs)
        {
            foreach (LDF_Constant c in Constants)
            {
                c.SetBuffer(CommonHeader);
            }
            fs.Write(CommonHeader, 0, CommonHeader.Length);
        }

        /// <summary>
        /// Writes the LDF into a file
        /// </summary>
        /// <param name="filename">File name to write to</param>
        public override void Write(string filename)
        {
            FileStream fs = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
            fs.Write(CommonHeader, 0, CommonHeader.Length);
            //if (SaveConstants)
            //{
            //    foreach (CSV_Constant c in Constants) sw.WriteLine(c.ToString());
            //}
            //sw.WriteLine(LogMnemonicsToString());
            //if (SaveUnits) sw.WriteLine(DataUnitsToString());
            //if (SaveDescriptions) sw.WriteLine(DescriptionsToString());

            // write data
            for (int i = 1; i < Channels.Count; i++)
            {
                Channels[i].Write(fs);
            }
            fs.Close();
        }

        /// <summary>
        /// Sets the LDF constant by name, adding a new parameter if necessary
        /// </summary>
        /// <param name="name">Constant name</param>
        /// <param name="unit">Constant unit</param>
        /// <param name="value">Constant value</param>
        /// <param name="value">Description</param>
        public override void SetConstant(string name, string unit, string value, string description)
        {
            foreach (LDF_Constant c in Constants)
            {
                if ( c.Name != name) continue;
                c.Value = value;
                return;
            }
            LDF_Constant newP = new LDF_Constant(name, unit, value, description);
            Constants.Add(newP);
        }
        
        /// <summary>
        /// Sets the LDF parameter by name, adding a new parameter if necessary
        /// Note that parameters represent the EBCDIC lines - any other paramenter beyond 40 will
        /// not be saved.
        /// </summary>
        /// <param name="name">Constant name</param>
        /// <param name="unit">Constant unit</param>
        /// <param name="value">Constant value</param>
        /// <param name="value">Description</param>
        public override void SetParameter(string name, string unit, string value, string description)
        {
            foreach (LDF_Constant c in Parameters)
            {
                if (c.Name != name) continue;
                c.Value = value;
                return;
            }
            LDF_Constant newP = new LDF_Constant(name, unit, value, description);
            Parameters.Add(newP);
        }

        /// <summary>
        /// Returns the shallow copy of LAS index (first channel)
        /// </summary>
        public new LDF_Channel GetIndex()
        {
            return GetChannel( 0);
        }

        /// <summary>
        /// Returns the deep copy of LAS index (first channel)
        /// </summary>
        public new LDF_Channel GetIndexCopy()
        {
            return GetChannelCopy(0);
        }

        /// <summary>
        /// Returns the shallow copy of LAS channel
        /// </summary>
        public new LDF_Channel GetChannel(int index)
        {
            LDF_Channel lc = (LDF_Channel)base.GetChannel(index);
            return lc;
        }

        /// <summary>
        /// Returns the shallow copy of LDF channel
        /// </summary>
        public new LDF_Channel GetChannel(string name)
        {
            LDF_Channel lc = (LDF_Channel)base.GetChannel(name);
            return lc;
        }

        /// <summary>
        /// Returns the deep copy of LDF channel
        /// </summary>
        public new LDF_Channel GetChannelCopy(int index)
        {
            if (index < 0 || index >= Channels.Count) return null;
            Oilfield_Channel c = GetChannel(index).Clone();
            return (LDF_Channel)c;
        }

        /// <summary>
        /// Returns the deep copy of LDF channel
        /// </summary>
        public new LDF_Channel GetChannelCopy(string name)
        {
            foreach (LDF_Channel c in Channels)
            {
                if (c.Name != name) continue;
                return (LDF_Channel)c.Clone();
            }
            return null;
        }

        /// <summary>
        /// Returns the LDF channel, creating one as necessary
        /// </summary>
        public new LDF_Channel GetOrCreateChannel(string name, string unit, string description, string format)
        {
            LDF_Channel c = GetChannel(name);
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
        /// Returns a new LDF channel
        /// </summary>
        public new LDF_Channel GetNewChannel(string name, string unit, string description)
        {
            LDF_Channel lc = new LDF_Channel(this, name, unit);
            lc.Description = description;
            return lc;
        }

        /// <summary>
        /// Returns a new LDF channel
        /// </summary>
        public LDF_Channel GetNewChannel(LDF_Channel template)
        {
            LDF_Channel c = (LDF_Channel)template.Clone();
            LDF_Channel index = GetIndex();
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
        public void SetChannelList(List<LDF_Channel> lst)
        {
            this.Channels.Clear();
            foreach (LDF_Channel c in lst)
            {
                this.Channels.Add(c);
            }
        }

        /// <summary>
        /// Returns true if the file extension corresponds to LDF
        /// </summary>
        /// <param name="name">file name</param>
        /// <returns>true if the extension corresponds to LDF</returns>
        public static bool IsLDFFile(string name)
        {
            FileInfo fi = new FileInfo(name);
            string ext = fi.Extension.ToLower();
            return ext.StartsWith(".ldf");
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
        
        private void SetDataSchema()
        {
            Constants.Add(
                new LDF_Constant("Magic String", "", 0, 8, LDF_Constant.String, "LDF File", "Magic string to identify LDF"));
            Constants.Add(
                new LDF_Constant("Version", "", 9, 6, LDF_Constant.String, "001E07", "LDF version"));
            Constants.Add(
                new LDF_Constant("Date", "", 16, 17, LDF_Constant.String, "01-Jan-1900 00:00", "Recording date and time"));
            Constants.Add(
                new LDF_Constant("Origin", "", 34, 11, LDF_Constant.String, "Petronode", "Producing software"));
            Constants.Add(
                new LDF_Constant("Along Trace Dimension", "", 46, 6, LDF_Constant.String, "TIME#2", "Type of measurement along trace (TIME,DEPT)"));
            Constants.Add(
                new LDF_Constant("Across Trace Dimension", "", 53, 6, LDF_Constant.String, "NODIM1", "Type of measurement scross trace (NODIM,VOLTS,M/S)"));
            Constants.Add(
                new LDF_Constant("Sampling Rate", "ms", 60, 4, LDF_Constant.Float32, "1.0", "Sampling rate in ms"));
            Constants.Add(
                new LDF_Constant("Number of Samples", "", 64, 4, LDF_Constant.Int32, "1000", "Number of samples in each trace"));
            Constants.Add(
                new LDF_Constant("Number of Levels", "", 68, 4, LDF_Constant.Int32, "0", "Number of levels in file"));
            Constants.Add(
                new LDF_Constant("Undefined Float", "", 72, 4, LDF_Constant.Float32, UndefinedFloat.ToString(), "Value to use as undefined float"));
            Constants.Add(
                new LDF_Constant("Undefined Int", "", 76, 4, LDF_Constant.Int32, UndefinedInt.ToString(), "Value to use as undefined int"));
            Constants.Add(
                new LDF_Constant("Mystery 1", "", 80, 4, LDF_Constant.Float32, "0.0", ""));
            Constants.Add(
                new LDF_Constant("Mystery 2", "", 84, 4, LDF_Constant.Float32, "0.0", ""));
            Constants.Add(
                new LDF_Constant("Global Shift", "ms", 88, 4, LDF_Constant.Float32, "0.0", ""));
            Constants.Add(
                new LDF_Constant("Across Header ID", "", 92, 4, LDF_Constant.Int32, "21", "Nobody knows; irrelevant; apparently always 21"));
            Constants.Add(
                new LDF_Constant("Along Label", "", 96, 49, LDF_Constant.String, "Time", "Along-trace label"));
            Constants.Add(
                new LDF_Constant("Along Measurement", "", 146, 49, LDF_Constant.String, "ms", "Along-trace measurement string"));
            Constants.Add(
                new LDF_Constant("Across Label", "", 196, 49, LDF_Constant.String, "Depth", "Across-trace label"));
            Constants.Add(
                new LDF_Constant("Across Measurement", "", 246, 49, LDF_Constant.String, "m", "Across-trace measurement string"));
            Constants.Add(
                new LDF_Constant("Trace Label", "", 296, 49, LDF_Constant.String, "Amplitude", "Trace label"));
            Constants.Add(
                new LDF_Constant("Trace Measurement", "", 346, 49, LDF_Constant.String, "unitless", "Trace measurement string"));
            Constants.Add(
                new LDF_Constant("Code", "", 396, 49, LDF_Constant.String, "Profile", "Survey Code"));

            // Officially unused last part of the LDF header starts from byte 446 (578 bytes)
            // It probably can be used to record part of the EBCDIC header, but currently we record the
            // EBCDIC after the trace info, so the information is placed into the processing history
        }
        #endregion
    }
}
