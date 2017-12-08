using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Petronode.OilfieldFileAccess.CSV;

namespace Petronode.VendorFileAccess.ASC
{
    /// <summary>
    /// Defines access to ASC sensor description file
    /// Note that at this time ASC simply ignores the header (meters or feet).
    /// Whatever number is in the file is used as a coordinate, based on menu that pops up upon loading
    /// 
    /// V1.2 METRIC
    /// Description Northing  Easting    Depth S  Type     Gain Sens Vmax Frequency Resp             Orientation
    /// ................ .......ft.......ft.......ft. ..... .......x .V/g ...V Lo(Hz) .Hi(Hz) .....N .....E .....D Se ..
    /// Channel 01	0.00000	0.00000	0.00000	0	00100	100.000	 52.000	2.0000	0.00000	1666.00	1.0000	0.00000	0.00000	1	0.00000	0.00000
    /// Channel 02	0.00000	0.00000	0.00000	0	01100	100.000	 52.000	2.0000	0.00000	1666.00	0.00000	1.0000	0.00000	1	0.00000	0.00000
    /// Channel 03	498.700	-664.800	2542.84	1	00001	100.000	 52.000	2.0000	0.00000	1666.00	1.0000	0.00000	0.00000	1	0.00000	0.00000
    /// Channel 04	498.700	-664.800	2542.84	1	01001	100.000	 52.000	2.0000	0.00000	1666.00	0.00000	1.0000	0.00000	1	0.00000	0.00000
    /// Channel 05	498.700	-664.800	2542.84	1	02001	100.000	 52.000	2.0000	0.00000	1666.00	0.00000	0.00000	-1.0000	1	0.00000	0.00000
    /// </summary>
    public class SensorDescriptionFile
    {
        private bool m_Modified = false;
        private bool m_Metric = true;
        private string m_FileName = "";

        public List<SensorDescription> Sensors = new List<SensorDescription>();

        #region Constructors
        /// <summary>
        /// Constructor; creates an empty file
        /// </summary>
        public SensorDescriptionFile()
        {
        }

        /// <summary>
        /// Constructor, reads from the file name given
        /// </summary>
        /// <param name="filename">filename to read</param>
        public SensorDescriptionFile( string filename)
        {
            m_FileName = filename;
            if (File.Exists(m_FileName)) Read();
        }
        #endregion

        #region PublicMethods
        /// <summary>
        /// Saves the file
        /// </summary>
        public void Save()
        {
            Write();
            m_Modified = false;
        }

        /// <summary>
        /// Saves with different file name
        /// </summary>
        /// <param name="filename"></param>
        public void Save( string filename)
        {
            m_FileName = filename;
            Save();
        }

        /// <summary>
        /// Returns a CSV file
        /// </summary>
        /// <returns></returns>
        public CSV_File ToCSVFile()
        {
            CSV_File csv = new CSV_File();
            string depthUnit = this.Metric? "m": "ft";
            csv.Channels.Add(ChannelToCSV("Sensor", "unitless", "0"));
            csv.Channels.Add(ChannelToCSV("MD", depthUnit, "0.00"));
            csv.Channels.Add(ChannelToCSV("Northing", depthUnit, "0.00"));
            csv.Channels.Add(ChannelToCSV("Easting", depthUnit, "0.00"));
            csv.Channels.Add(ChannelToCSV("Depth SRD", depthUnit, "0.00"));
            csv.Channels.Add(ChannelToCSV("Active", "unitless", "0"));
            csv.Channels.Add(ChannelToCSV("Type", "unitless", "0"));
            csv.Channels.Add(ChannelToCSV("Gain", "unitless", "0.000"));
            csv.Channels.Add(ChannelToCSV("Sensitivity", "unit/V", "0.000"));
            csv.Channels.Add(ChannelToCSV("Full Scale", "unit", "0.000"));
            csv.Channels.Add(ChannelToCSV("Filter Min", "Hz", "0.0"));
            csv.Channels.Add(ChannelToCSV("Filter Max", "Hz", "0.0"));
            csv.Channels.Add(ChannelToCSV("iN", "unitless", "0.00000"));
            csv.Channels.Add(ChannelToCSV("iE", "unitless", "0.00000"));
            csv.Channels.Add(ChannelToCSV("iD", "unitless", "0.00000"));
            csv.Channels.Add(ChannelToCSV("Polarity", "unitless", "0"));
            csv.Channels.Add(ChannelToCSV("P Static", "s", "0.0000"));
            csv.Channels.Add(ChannelToCSV("S Static", "s", "0.0000"));
            csv.SaveDescriptions = false;
            return csv;
        }

        /// <summary>
        /// Returns a chort CSV file for FM loading 
        /// </summary>
        /// <returns></returns>
        public CSV_File ToCSVFMFile()
        {
            CSV_File csv = new CSV_File();
            //Chan,Inst,Axis,MD,iN,iE,iD
            csv.Channels.Add(ChannelToCSV("Chan", "", "0"));
            csv.Channels.Add(ChannelToCSV("Inst", "", "0"));
            csv.Channels.Add(ChannelToCSV("Axis", "", "0"));
            csv.Channels.Add(ChannelToCSV("MD", "", "0"));
            csv.Channels.Add(ChannelToCSV("iN", "", "0.00000"));
            csv.Channels.Add(ChannelToCSV("iE", "", "0.00000"));
            csv.Channels.Add(ChannelToCSV("iD", "", "0.00000"));
            csv.SaveUnits = false;
            csv.SaveDescriptions = false;
            return csv;
        }

        /// <summary>
        /// Loads data from fully qualified CSV file
        /// Exceptions are thrown if the column names are wrong
        /// </summary>
        /// <param name="?"></param>
        public void FromCSVFile( CSV_File csv)
        {
            SensorDescriptionFile sdf = new SensorDescriptionFile();
            CSV_Channel index = csv.GetIndex();
            if (index == null) throw new Exception("File contains no valid data");
            CSV_Channel MD = CSVToChannel(csv, "MD");
            CSV_Channel Northing = CSVToChannel(csv, "Northing");
            CSV_Channel Easting = CSVToChannel(csv, "Easting");
            CSV_Channel DepthSRD = CSVToChannel( csv, "Depth SRD");
            CSV_Channel IsActive = CSVToChannel( csv, "Active");
            CSV_Channel Type = CSVToChannel( csv, "Type");
            CSV_Channel Gain = CSVToChannel( csv, "Gain");
            CSV_Channel Sensitivity = CSVToChannel( csv, "Sensitivity");
            CSV_Channel FullScale = CSVToChannel( csv, "Full Scale");
            CSV_Channel LowFrequency = CSVToChannel( csv, "Filter Min");
            CSV_Channel HighFrequency = CSVToChannel( csv, "Filter Max");
            CSV_Channel iN = CSVToChannel( csv, "iN");
            CSV_Channel iE = CSVToChannel( csv, "iE");
            CSV_Channel iD = CSVToChannel( csv, "iD");
            CSV_Channel Polarization = CSVToChannel( csv, "Polarity");
            CSV_Channel PStationCorrection = CSVToChannel( csv, "P Static");
            CSV_Channel SStationCorrection = CSVToChannel( csv, "S Static");
            sdf.Metric = !DepthSRD.Unit.ToUpper().Contains("F");
            for (int i = 0; i < index.Data.Count; i++)
            {
                SensorDescription sd = new SensorDescription();
                sd.ChannelNumber = i + 1;
                sd.MD = MD.Data[i];
                sd.Northing = Northing.Data[i];
                sd.Easting = Easting.Data[i];
                sd.DepthSRD = DepthSRD.Data[i];
                sd.IsActive = IsActive.Data[i] > 0.0;
                sd.Type = Type.Data[i].ToString("0").PadLeft(5, '0');
                sd.Gain = Gain.Data[i];
                sd.Sensitivity = Sensitivity.Data[i];
                sd.FullScale = FullScale.Data[i];
                sd.LowFrequency = LowFrequency.Data[i];
                sd.HighFrequency = HighFrequency.Data[i];
                sd.iN = iN.Data[i];
                sd.iE = iE.Data[i];
                sd.iD = iD.Data[i];
                sd.Polarization = (Polarization.Data[i] < 0.0) ? -1 : 1;
                sd.PStationCorrection = PStationCorrection.Data[i];
                sd.SStationCorrection = SStationCorrection.Data[i];
                sdf.Sensors.Add(sd);
            }
            if( sdf.Sensors.Count <= 0) throw new Exception( "CSV file contains no data.");
            this.Sensors.Clear();
            this.Metric = sdf.Metric;
            this.Sensors = sdf.Sensors;
            m_Modified = true;
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Sets and retrieves true if the file has been modified since reading
        /// </summary>
        public bool IsModified
        {
            get { return m_Modified;}
            set { m_Modified = value; }
        }

        /// <summary>
        /// Sets and retrieves if the file is metric (M) or imperial (ft)
        /// </summary>
        public bool Metric
        {
            get { return m_Metric; }
            set
            {
                if (value == m_Metric) return;
                if (value) // convert from imperial to metric
                {
                    foreach (SensorDescription sd in Sensors) sd.ToMetric();
                }
                else // convert from metric to imperial
                {
                    foreach (SensorDescription sd in Sensors) sd.ToImperial();
                }
                m_Metric = value;
                m_Modified = true;
            }
        }
        #endregion

        #region Private Methods
        private void Read()
        {
            FileStream fs = null;
            StreamReader sr = null;
            try
            {
                fs = File.Open(m_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                sr = new StreamReader(fs);
                ReadHeader( sr);
                while (true)
                {
                    string s = sr.ReadLine();
                    if (s == null) break;
                    SensorDescription sd = new SensorDescription(s);
                    if (!sd.Valid) continue;
                    Sensors.Add(sd);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (sr != null) sr.Close();
                if (fs != null) fs.Close();
            }
        }

        private void Write()
        {
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                fs = File.Open(m_FileName, FileMode.Create, FileAccess.Write, FileShare.Read);
                sw = new StreamWriter(fs);
                WriteHeader(sw);
                foreach (SensorDescription sd in Sensors)
                {
                    sw.WriteLine(sd.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }
        }

        private void ReadHeader(StreamReader sr)
        {
            sr.ReadLine();
            sr.ReadLine();
            string s = sr.ReadLine();
            m_Metric = !s.Contains( ".ft.");
        }
        
        private void WriteHeader( StreamWriter sw)
        {
            if( m_Metric)
            {
                sw.WriteLine("V1.2 METRIC");
                sw.WriteLine("Description Northing  Easting    Depth S  Type     Gain Sens Vmax Frequency Resp             Orientation");
                sw.WriteLine("................ .......m .......m .......m . ..... .......x .V/g ...V Lo(Hz) .Hi(Hz) .....N .....E .....D Se ..");
            }
            else
            {
                sw.WriteLine("V1.2 IMPERIAL");
                sw.WriteLine("Description Northing  Easting    Depth S  Type     Gain Sens Vmax Frequency Resp             Orientation");
                sw.WriteLine("................ .......ft.......ft.......ft. ..... .......x .V/g ...V Lo(Hz) .Hi(Hz) .....N .....E .....D Se ..");
            }
        }

        private CSV_Channel ChannelToCSV(string name, string unit, string format)
        {
            CSV_Channel c = new CSV_Channel(name, unit);
            c.Format = format;
            switch (name)
            {
                case "Inst":
                    for (int i = 0; i < Sensors.Count; i++)
                    {
                        double tmp = 0.0;
                        try{ tmp = Convert.ToDouble( Sensors[i].Type.Substring(2));}
                        catch(Exception){}
                        c.Data.Add(tmp);
                    }
                    break;
                case "Axis":
                    for (int i = 0; i < Sensors.Count; i++)
                    {
                        double tmp = 0.0;
                        try{ tmp = Convert.ToDouble( Sensors[i].Type.Substring(1,1));}
                        catch(Exception){}
                        c.Data.Add(tmp);
                    }
                    break;
                case "Chan":
                case "Sensor":
                    for (int i = 0; i < Sensors.Count; i++) c.Data.Add((double)Sensors[i].ChannelNumber);
                    break;
                case "MD":
                    for (int i = 0; i < Sensors.Count; i++) c.Data.Add(Sensors[i].MD);
                    break;
                case "Northing":
                    for (int i = 0; i < Sensors.Count; i++) c.Data.Add(Sensors[i].Northing);
                    break;
                case "Easting":
                    for (int i = 0; i < Sensors.Count; i++) c.Data.Add(Sensors[i].Easting);
                    break;
                case "Depth SRD":
                    for (int i = 0; i < Sensors.Count; i++) c.Data.Add(Sensors[i].DepthSRD);
                    break;
                case "Active":
                    for (int i = 0; i < Sensors.Count; i++) c.Data.Add(Sensors[i].IsActive? 1.0: 0.0);
                    break;
                case "Type":
                    for (int i = 0; i < Sensors.Count; i++) c.Data.Add(Convert.ToDouble(Sensors[i].Type));
                    break;
                case "Gain":
                    for (int i = 0; i < Sensors.Count; i++) c.Data.Add(Sensors[i].Gain);
                    break;
                case "Sensitivity":
                    for (int i = 0; i < Sensors.Count; i++) c.Data.Add(Sensors[i].Sensitivity);
                    break;
                case "Full Scale":
                    for (int i = 0; i < Sensors.Count; i++) c.Data.Add(Sensors[i].FullScale);
                    break;
                case "Filter Min":
                    for (int i = 0; i < Sensors.Count; i++) c.Data.Add(Sensors[i].LowFrequency);
                    break;
                case "Filter Max":
                    for (int i = 0; i < Sensors.Count; i++) c.Data.Add(Sensors[i].HighFrequency);
                    break;
                case "iN":
                    for (int i = 0; i < Sensors.Count; i++) c.Data.Add(Sensors[i].iN);
                    break;
                case "iE":
                    for (int i = 0; i < Sensors.Count; i++) c.Data.Add(Sensors[i].iE);
                    break;
                case "iD":
                    for (int i = 0; i < Sensors.Count; i++) c.Data.Add(Sensors[i].iD);
                    break;
                case "Polarity":
                    for (int i = 0; i < Sensors.Count; i++) c.Data.Add(Sensors[i].Polarization);
                    break;
                case "P Static":
                    for (int i = 0; i < Sensors.Count; i++) c.Data.Add(Sensors[i].PStationCorrection);
                    break;
                case "S Static":
                    for (int i = 0; i < Sensors.Count; i++) c.Data.Add(Sensors[i].SStationCorrection);
                    break;
                default: break;
            }
            return c;
        }

        private CSV_Channel CSVToChannel(CSV_File csv, string name)
        {
            CSV_Channel c = csv.GetChannel(name);
            if (c == null) throw new Exception("File does not contain channel " + name + ".");
            return c;
        }
        #endregion
    }
}
