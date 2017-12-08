using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Petronode.OilfieldFileAccess.CSV;
using Petronode.OilfieldFileAccess.SEGY;
using Petronode.OilfieldFileAccess.LAS;
using Petronode.OilfieldFileAccess.Petrolog;

namespace Petronode.OilfieldFileAccess
{
    /// <summary>
    /// Class responsible for oilfield files creation
    /// </summary>
    public class Oilfield_File_Factory
    {
        public static Oilfield_File OpenHeader(string filename)
        {
            if( !File.Exists( filename)) return null;
            if (CSV_File.IsCSVFile(filename)) return new CSV_File(filename);
            if (SEGY_File.IsSEGYFile(filename)) return new SEGY_File(filename);
            if (LAS_File.IsLASFile(filename)) return new LAS_File(filename, false);
            if (Petrolog_File.IsPetrologFile(filename)) return new Petrolog_File(filename);
            return null;
        }

        public static Oilfield_File OpenData(string filename)
        {
            if (!File.Exists(filename)) return null;
            if (CSV_File.IsCSVFile(filename)) return new CSV_File(filename);
            if (SEGY_File.IsSEGYFile(filename)) return new SEGY_File(filename);
            if (LAS_File.IsLASFile(filename)) return new LAS_File(filename, true);
            if (Petrolog_File.IsPetrologFile(filename))
            {
                Petrolog_File pf = new Petrolog_File(filename);
                //Petrolog_Channel index = pf.GetIndex();
                //index.LoadData();
                pf.CreateIndexMetric();
                return pf;
            }
            return null;
        }
    }
}
