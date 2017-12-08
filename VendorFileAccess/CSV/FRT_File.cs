using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Petronode.OilfieldFileAccess.CSV;

namespace Petronode.VendorFileAccess.CSV
{
    public class FRT_File: CSV_File
    {
        private char[] c_SplitCharacters1 = { ',' };
        private char[] c_SplitCharacters2 = { ' ', '(', ')'};

        public FRT_File( string filename): base()
        {
            this.FileName = filename;
            StreamReader sr = File.OpenText(filename);
            bool headermode = true;
            while (true)
            {
                string s = sr.ReadLine();
                if (s == null) break;
                if (s.Length <= 0) continue;
                if (!headermode)
                {
                    ProcessLine(s);
                    continue;
                }
                if (headermode && !s.StartsWith("Time")) continue;
                string[] ss = s.Split(c_SplitCharacters1);
                foreach (string s1 in ss)
                {
                    string[] ss1 = s1.Split(c_SplitCharacters2, StringSplitOptions.RemoveEmptyEntries);
                    if( ss1.Length >= 2)
                        this.Channels.Add( new CSV_Channel( ss1[0], ss1[1]));
                    else
                        this.Channels.Add(new CSV_Channel(ss1[0], ""));
                }
                headermode = false;
            }
            sr.Close();
        }

        private void ProcessLine( string line)
        {
            CSV_Line l = new CSV_Line(line);
            if (l.Substrings.Length != this.Channels.Count) return;
            for (int i = 0; i < Channels.Count; i++)
            {
                Channels[i].Data.Add(l.GetDouble(i));
            }
        }
    }
}
