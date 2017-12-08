using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Petronode.OilfieldFileAccess;

namespace Petronode.OilfieldFileAccess.SEGY
{
    public class SEGY_File: Oilfield_File
    {
        public byte[] EBCDICHeader = new byte[3200];
        public byte[] CommonHeader = new byte[400];

        #region Constructors
        /// <summary>
        /// Constructor; creates an empty SEG-Y file
        /// </summary>
        public SEGY_File(): base()
        {
        }
        
        /// <summary>
        /// Constructor; reads the SEGY file into memory (including traces)
        /// </summary>
        /// <param name="filename">File name</param>
        public SEGY_File(string filename): base( filename)
        {
            ParseFile( false);
        }

        /// <summary>
        /// Constructor; reads the SEGY file into memory (traces optional)
        /// </summary>
        /// <param name="filename">File name</param>
        /// <param name="readdata">Reads data if true</param>
        public SEGY_File(string filename, bool readdata)
            : base(filename)
        {
            ParseFile( readdata);
        }

        private void ParseFile( bool readdata)
        {
            FileStream fs = null;
            try
            {
                fs = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                fs.Read(EBCDICHeader, 0, EBCDICHeader.Length);
                fs.Read(CommonHeader, 0, CommonHeader.Length);
                for (int i = 0; i < 40; i++)
                {
                    SEGY_Constant sc = new SEGY_Constant(i, EBCDICHeader);
                    Parameters.Add(sc);
                }
                Constants.Add(
                    new SEGY_Constant("JOB_ID", "", 0, SEGY_Constant.Int32, CommonHeader, "Job numeric identifier"));
                Constants.Add(
                    new SEGY_Constant("LINE_NUMBER", "", 4, SEGY_Constant.Int32, CommonHeader, "Line number (Legacy use)"));
                Constants.Add(
                    new SEGY_Constant("ACQUISITION_FILE_NUMBER", "", 8, SEGY_Constant.Int32, CommonHeader, "Reel number (Legacy use)"));
                Constants.Add(
                    new SEGY_Constant("NBR_DATA_TRACE", "", 12, SEGY_Constant.Int16, CommonHeader, "Number of channels"));
                Constants.Add(
                    new SEGY_Constant("NBR_AUX_TRACE", "", 14, SEGY_Constant.Int16, CommonHeader, "Number of aux channels"));
                Constants.Add(
                    new SEGY_Constant("SAMPLING_PERIOD", "us", 16, SEGY_Constant.Int16, CommonHeader, "Sampling period in micro-seconds"));
                Constants.Add(
                    new SEGY_Constant("SAMPLING_PERIOD_ORG", "us", 18, SEGY_Constant.Int16, CommonHeader, "Sampling period - as recorded"));
                Constants.Add(
                    new SEGY_Constant("NSAMPLES", "", 20, SEGY_Constant.UInt16, CommonHeader, "Samples in each channel"));
                Constants.Add(
                    new SEGY_Constant("FORMAT_CODE", "", 24, SEGY_Constant.Int16, CommonHeader, "1-IBM FP, 2-Int32, 3-Int16, 4-UTIG16 (Legacy use), 5-Real32"));
                Constants.Add(
                    new SEGY_Constant("CDP_FOLD", "", 26, SEGY_Constant.Int16, CommonHeader, "CDP fold"));
                Constants.Add(
                    new SEGY_Constant("SORT_CODE", "", 28, SEGY_Constant.Int16, CommonHeader, "1-as recorded, 2-CDP ensemble, 3-single fold, 4-stacked"));
                Constants.Add(
                    new SEGY_Constant("VSUM", "", 30, SEGY_Constant.Int16, CommonHeader, "Number of vertical summations"));
                Constants.Add(
                    new SEGY_Constant("SWEEP_START", "Hz", 32, SEGY_Constant.Int16, CommonHeader, "Sweep frequency start"));
                Constants.Add(
                    new SEGY_Constant("SWEEP_STOP", "Hz", 34, SEGY_Constant.Int16, CommonHeader, "Sweep frequency end"));
                Constants.Add(
                    new SEGY_Constant("SWEEP_LENGTH", "ms", 36, SEGY_Constant.Int16, CommonHeader, "Sweep length"));
                Constants.Add(
                    new SEGY_Constant("SWEEP_TYPE", "", 38, SEGY_Constant.Int16, CommonHeader, "1-linear, 2-parabolic,3-exponential,4-other"));
                Constants.Add(
                    new SEGY_Constant("SWEEP_CHANNEL", "", 40, SEGY_Constant.Int16, CommonHeader, "Trace number of sweep channel"));
                Constants.Add(
                    new SEGY_Constant("SWEEP_TAPER_START", "ms", 42, SEGY_Constant.Int16, CommonHeader, "Sweep taper at start"));
                Constants.Add(
                    new SEGY_Constant("SWEEP_TAPER_STOP", "ms", 44, SEGY_Constant.Int16, CommonHeader, "Sweep taper at end"));
                Constants.Add(
                    new SEGY_Constant("SWEEP_TAPER_CODE", "", 46, SEGY_Constant.Int16, CommonHeader, "1-linear,2-cos-squared,3-other"));
                Constants.Add(
                    new SEGY_Constant("CORRELATED_CODE", "", 48, SEGY_Constant.Int16, CommonHeader, "1-no,2-yes"));
                Constants.Add(
                    new SEGY_Constant("BINARY_GAIN_RECOVERED", "", 50, SEGY_Constant.Int16, CommonHeader, "1-yes,2-no"));
                Constants.Add(
                    new SEGY_Constant("AMPLITUDE_RECOVERED", "", 52, SEGY_Constant.Int16, CommonHeader, "1-no,2-spherical divergence,3-AGC,4-other"));
                Constants.Add(
                    new SEGY_Constant("COORD_UNIT", "", 54, SEGY_Constant.Int16, CommonHeader, "Coordinates: 1-Metric(m), 2-Imperial(ft)"));
                Constants.Add(
                    new SEGY_Constant("POLARITY_IMP", "", 56, SEGY_Constant.Int16, CommonHeader, "Impulse polarity code - response for pressure increase: 1-negative, 2- positive"));
                Constants.Add(
                    new SEGY_Constant("POLARITY_VIB", "", 58, SEGY_Constant.Int16, CommonHeader, "Vibrator polarity code: 1-N,2-NE,3-E,4-SE,5-S,6-SW,7-W,8-NW"));

                // Officially unused last part of the SEG-Y header
                //for( int i=60; i<395; i+=4)
                //{
                //    string t = i.ToString().PadLeft(3,'0');
                //    Constants.Add(
                //        new SEGY_Constant("TR"+t, "", i, SEGY_Constant.Int32, CommonHeader, "Trail " + t));
                //}

                long nsamples = Convert.ToInt64(GetConstant("NSAMPLES"));
                double interval = Convert.ToDouble(GetConstant("SAMPLING_PERIOD")) / 1000.0;
                if (interval <= 0.0)
                {
                    SetConstant("SAMPLING_PERIOD", "1000");
                    interval = 1.0;
                }
                SEGY_Channel index = new SEGY_Channel( "Time", "ms");
                if (readdata)
                {
                    for (int i = 0; i < nsamples; i++) index.Data.Add(interval * i);
                }
                Channels.Add(index);

                int format = Convert.ToInt32(GetConstant("FORMAT_CODE"));
                long channelLength = 240;
                switch (format)
                {
                    case 3:
                    case 4:
                        channelLength += nsamples << 1; break;
                    case 1:
                    case 2:
                    case 5:
                        channelLength += nsamples << 2;
                        break;
                    default:
                        SetConstant("FORMAT_CODE", "1");
                        channelLength += nsamples << 2;
                        break;
                }
                byte[] Buffer = new byte[channelLength];
                int traceCount = 0;
                string globUnit = (GetConstant("COORD_UNIT") == "2") ? "ft" : "m";
                while (true)
                {
                    int read = fs.Read(Buffer, 0, Buffer.Length);
                    if (read < Buffer.Length) break;
                    SEGY_Channel c = new SEGY_Channel(traceCount.ToString(), "sample", Buffer, readdata);
                    foreach (SEGY_Constant cc in c.Parameters)
                    {
                        if (cc.Unit == "unitless") continue;
                        if (cc.Unit == "elevation") cc.Unit = globUnit;
                        if (cc.Unit == "coordinate") cc.Unit = globUnit;
                        if (cc.Unit == "velocity") cc.Unit = globUnit + "/s";
                    }
                    this.Channels.Add(c);
                    traceCount++;
                }
                SetConstant("NBR_DATA_TRACE", traceCount.ToString());
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
        /// Writes the CSV into a file
        /// </summary>
        /// <param name="filename">File name to write to</param>
        public override void Write(string filename)
        {
            FileStream fs = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
            fs.Write(EBCDICHeader, 0, EBCDICHeader.Length);
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
        /// Sets the SEGY constant by name, adding a new parameter if necessary
        /// </summary>
        /// <param name="name">Constant name</param>
        /// <param name="unit">Constant unit</param>
        /// <param name="value">Constant value</param>
        /// <param name="value">Description</param>
        public override void SetConstant(string name, string unit, string value, string description)
        {
            foreach (SEGY_Constant c in Constants)
            {
                if ( c.Name != name) continue;
                c.Value = value;
                return;
            }
            SEGY_Constant newP = new SEGY_Constant(name, unit, value, description);
            Constants.Add(newP);
        }
        
        /// <summary>
        /// Sets the SEGY parameter by name, adding a new parameter if necessary
        /// Note that parameters represent the EBCDIC lines - any other paramenter beyond 40 will
        /// not be saved.
        /// </summary>
        /// <param name="name">Constant name</param>
        /// <param name="unit">Constant unit</param>
        /// <param name="value">Constant value</param>
        /// <param name="value">Description</param>
        public override void SetParameter(string name, string unit, string value, string description)
        {
            foreach (SEGY_Constant c in Parameters)
            {
                if (c.Name != name) continue;
                c.Value = value;
                return;
            }
            SEGY_Constant newP = new SEGY_Constant(name, unit, value, description);
            Parameters.Add(newP);
        }

        /// <summary>
        /// Returns the shallow copy of LAS index (first channel)
        /// </summary>
        public new SEGY_Channel GetIndex()
        {
            return GetChannel( 0);
        }

        /// <summary>
        /// Returns the deep copy of LAS index (first channel)
        /// </summary>
        public new SEGY_Channel GetIndexCopy()
        {
            return GetChannelCopy(0);
        }

        /// <summary>
        /// Returns the shallow copy of LAS channel
        /// </summary>
        public new SEGY_Channel GetChannel(int index)
        {
            SEGY_Channel lc = (SEGY_Channel)base.GetChannel(index);
            return lc;
        }

        /// <summary>
        /// Returns the shallow copy of LAS channel
        /// </summary>
        public new SEGY_Channel GetChannel(string name)
        {
            SEGY_Channel lc = (SEGY_Channel)base.GetChannel(name);
            return lc;
        }

        /// <summary>
        /// Returns the deep copy of LAS channel
        /// </summary>
        public new SEGY_Channel GetChannelCopy(int index)
        {
            if (index < 0 || index >= Channels.Count) return null;
            Oilfield_Channel c = GetChannel(index).Clone();
            return (SEGY_Channel)c;
        }

        /// <summary>
        /// Returns the deep copy of SEGY channel
        /// </summary>
        public new SEGY_Channel GetChannelCopy(string name)
        {
            foreach (SEGY_Channel c in Channels)
            {
                if (c.Name != name) continue;
                return (SEGY_Channel)c.Clone();
            }
            return null;
        }

        /// <summary>
        /// Returns the LAS channel, creating one as necessary
        /// </summary>
        public new SEGY_Channel GetOrCreateChannel(string name, string unit, string description, string format)
        {
            SEGY_Channel c = GetChannel(name);
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
        public new SEGY_Channel GetNewChannel(string name, string unit, string description)
        {
            SEGY_Channel lc = new SEGY_Channel( name, unit);
            lc.Description = description;
            return lc;
        }

        /// <summary>
        /// Returns a new SEG-Y channel
        /// </summary>
        public SEGY_Channel GetNewChannel(SEGY_Channel template)
        {
            SEGY_Channel c = (SEGY_Channel)template.Clone();
            SEGY_Channel index = GetIndex();
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
        public void SetChannelList(List<SEGY_Channel> lst)
        {
            this.Channels.Clear();
            foreach (SEGY_Channel c in lst)
            {
                this.Channels.Add(c);
            }
        }

        /// <summary>
        /// Returns true if the file extension corresponds to SEG-Y
        /// </summary>
        /// <param name="name">file name</param>
        /// <returns>true if the extension corresponds to SEGY</returns>
        public static bool IsSEGYFile(string name)
        {
            FileInfo fi = new FileInfo(name);
            string ext = fi.Extension.ToLower();
            if (ext.StartsWith(".sgy")) return true;
            return ext.StartsWith(".seg");
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
        #endregion
    }
}
