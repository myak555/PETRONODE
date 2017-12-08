using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Petronode.Automation
{
    public class LAS_Resample_Launcher: ApplicationLauncher
    {
        /// <summary>
        /// Constructor, creates a launcher object
        /// </summary>
        public LAS_Resample_Launcher(string app_path, string combinedLAS): base(app_path, combinedLAS)
        {
            Command_Line = "\"" + Command_Line + "\"";
        }

        protected override string LocateApp(string app_path)
        {
            string LAS_location1 = m_Batch_Path1 + "LAS_Resample.exe";
            string LAS_location2 = m_Batch_Path2 + "LAS_Resample.exe";
            if (app_path == null) return "";
            int pos = app_path.IndexOf(c_Splitter);
            string AppName = "";
            if (pos >= 0)
            {
                AppName = app_path.Substring(0, pos + c_Splitter.Length - 1) + "\\LAS_Resample.exe";
                if (File.Exists(AppName)) return AppName;
            }
            else
            {
                if (File.Exists(LAS_location1)) return LAS_location1;
                if (File.Exists(LAS_location2)) return LAS_location2;
            }
            return "";
        }
    }
}
