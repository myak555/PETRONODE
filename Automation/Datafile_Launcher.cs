using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Petronode.Automation
{
    /// <summary>
    /// Launches a datafile in the default system application, e.g. CSV in Excel
    /// </summary>
    public class Datafile_Launcher: ApplicationLauncher
    {
        /// <summary>
        /// Constructor, creates a launcher object
        /// </summary>
        public Datafile_Launcher(string app_path)
            : base(app_path)
        {
            Command_Line = "\"" + Command_Line + "\"";
        }

        protected override string LocateApp(string app_path)
        {
            return app_path;
        }
    }
}
