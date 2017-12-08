using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Petronode.OilfieldFileAccess
{
    /// <summary>
    /// Describes a log (channel) statistics file
    /// </summary>
    public class Log_Statistics_File
    {
        private List<Oilfield_Channel> m_Channels = null;
        private string[] c_Splitters = { "|||" };

        /// <summary>
        /// Constructor, creates a data holder
        /// </summary>
        /// <param name="ch"></param>
        public Log_Statistics_File( List<Oilfield_Channel> ch)
        {
            m_Channels = ch;
        }

        /// <summary>
        /// Loads the channel info from disk
        /// </summary>
        /// <param name="filename">filename to load from</param>
        /// <returns>true if file exists and loaded</returns>
        public bool Load(string filename)
        {
            if (!File.Exists(filename)) return false;
            StreamReader sr = File.OpenText(filename);
            bool ret = true;
            while (true)
            {
                string s = sr.ReadLine();
                if (s == null) break;
                if (s.Length <= 0) continue;
                if (s.StartsWith("#")) continue;
                s = Dequote( s);
                string[] tmp = s.Split( c_Splitters, StringSplitOptions.None);
                if (tmp.Length < 9)
                {
                    ret = false;
                    continue;
                }
                Oilfield_Channel oc = null;
                foreach( Oilfield_Channel c in m_Channels)
                {
                    if( c.Name != tmp[0] || c.Unit != tmp[1]) continue;
                    oc = c;
                    break;
                }
                if( oc == null)
                {
                    ret = false;
                    continue;
                }
                try
                {
                    oc.FromStrings(tmp);
                }
                catch (Exception)
                {
                    ret = false;
                    continue;
                }
            }
            sr.Close();
            return ret;
        }

        /// <summary>
        /// Writes the statistical info to file
        /// </summary>
        /// <param name="filename">filename to write to</param>
        public void Write(string filename)
        {
            StreamWriter sw = File.CreateText(filename);
            sw.WriteLine("#Channel,Unit,Type,Description,Valid Values,Missing Values,Minimum Value,Maximum Value,Average Value");
            foreach (Oilfield_Channel oc in m_Channels)
            {
                string[] tmp = oc.ToStrings(3);
                if( tmp.Length <= 0) continue;
                for (int j = 0; j < tmp.Length - 1; j++)
                {
                    string s = tmp[j].Replace( "\"","''");
                    if (s.Contains(",")) sw.Write("\"" + s + "\",");
                    else sw.Write(s + ",");
                }
                sw.WriteLine(tmp[tmp.Length - 1]);
            }
            sw.Close();
        }

        private string Dequote(string s)
        {
            StringBuilder sb = new StringBuilder();
            bool inquote = false;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c == '\"')
                {
                    inquote = !inquote;
                    continue;
                }
                if (!inquote && c == ',')
                {
                    sb.Append(c_Splitters[0]);
                    continue;
                }
                sb.Append(c);
            }
            string str = sb.ToString().Replace("''", "\"");
            return str;
        }
    }
}
