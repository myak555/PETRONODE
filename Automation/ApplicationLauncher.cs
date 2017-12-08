using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Petronode.Automation
{
    /// <summary>
    /// Implements a base class for varous launchers
    /// </summary>
    public class ApplicationLauncher
    {
        protected string m_App_Path = "";
        protected string m_Batch_Path1 = "C:\\Program Files\\Petronode\\";
        protected string m_Batch_Path2 = "C:\\Program Files (x86)\\Petronode\\";
        
        /// <summary>
        /// Application command line
        /// </summary>
        public string Command_Line = "";

        /// <summary>
        /// Constructor, creates a launcher object
        /// </summary>
        protected ApplicationLauncher(string app_path)
        {
            m_App_Path = LocateApp( app_path);
        }

        /// <summary>
        /// Constructor, creates a launcher object
        /// </summary>
        protected ApplicationLauncher(string app_path, string command_line)
        {
            m_App_Path = LocateApp(app_path);
            Command_Line = BuildCommandLine( command_line);
        }

        /// <summary>
        /// Returns true if application is found
        /// </summary>
        public bool IsFound
        {
            get { return m_App_Path.Length > 0; }
        }

        /// <summary>
        /// Retrieves the application path
        /// </summary>
        public string ApplicationPath
        {
            get { return m_App_Path; }
        }

        /// <summary>
        /// Launches application
        /// </summary>
        public virtual void Launch()
        {
            Process p = new Process();
            p.StartInfo.FileName = m_App_Path;
            if (Command_Line.Length > 0) p.StartInfo.Arguments = Command_Line;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            p.StartInfo.CreateNoWindow = false;
            p.Start();
        }

        /// <summary>
        /// Launches application
        /// </summary>
        public virtual void Launch( ProcessWindowStyle st, bool window)
        {
            Process p = new Process();
            p.StartInfo.FileName = m_App_Path;
            if (Command_Line.Length > 0) p.StartInfo.Arguments = Command_Line;
            p.StartInfo.WindowStyle = st;
            p.StartInfo.CreateNoWindow = window;
            //p.StartInfo.RedirectStandardInput = false;
            //p.StartInfo.RedirectStandardOutput = false;
            p.Start();
        }

        protected const string c_Splitter = "Petronode\\";
        protected virtual string LocateApp( string app_path)
        {
            return "";
        }

        protected virtual string BuildCommandLine(string comm_line)
        {
            return comm_line;
        }
    }
}
