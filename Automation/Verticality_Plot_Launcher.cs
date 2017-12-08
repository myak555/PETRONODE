using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Petronode.Automation
{
    public class Verticality_Plot_Launcher: ApplicationLauncher
    {
        /// <summary>
        /// Constructor, creates a launcher object
        /// </summary>
        public Verticality_Plot_Launcher(string app_path, string verticalityLAS, string outputBMP)
            : base(app_path, verticalityLAS)
        {
            Command_Line = "\"" + Command_Line + "\" \"" + outputBMP + "\"";
        }

        protected override string LocateApp(string app_path)
        {
            string Vert_location1 = m_Batch_Path1 + "Verticality_Plot.exe";
            string Vert_location2 = m_Batch_Path1 + "Verticality_Plot.exe";
            if (app_path == null) return "";
            int pos = app_path.IndexOf(c_Splitter);
            string AppName = "";
            if (pos >= 0)
            {
                AppName = app_path.Substring(0, pos + c_Splitter.Length - 1) + "\\Verticality_Plot.exe";
                if (File.Exists(AppName)) return AppName;
            }
            else
            {
                if (File.Exists(Vert_location1)) return Vert_location1;
                if (File.Exists(Vert_location2)) return Vert_location2;
            }
            return "";
        }
    }
}
