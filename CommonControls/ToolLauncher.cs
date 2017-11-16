using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;

namespace Petronode.CommonControls
{
    public class ToolLauncher
    {
        private static string c_RegPath = "http\\shell\\open\\command";

        /// <summary>
        /// Starts al application with the command line
        /// </summary>
        /// <returns>0 if success, 1 if an error</returns>
        public static int Launch( string Executable, string CommandLine)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = Executable;
                p.StartInfo.Arguments = CommandLine;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                p.StartInfo.CreateNoWindow = false;
                p.Start();
            }
            catch( Exception ex)
            {
                MessageBox.Show( "Error: " + ex.Message, "Application Launch", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// Locates the local browser and launches http://petronode.com
        /// </summary>
        /// <returns>0 if success, 1 if no browser found</returns>
        public static int LaunchBrowser()
        {
            return LaunchBrowser( "http://petronode.com");
        }

        /// <summary>
        /// Locates the local browser and launches the url
        /// </summary>
        /// <param name="url">URL string</param>
        /// <returns>0 if success, 1 if no browser found</returns>
        public static int LaunchBrowser( string url)
        {
            string Executable = LocateBrowser();
            return Launch( Executable, "\"" + url + "\"");
        }

        /// <summary>
        /// Locates the local browser and launches the local manuals in it
        /// </summary>
        /// <param name="file">Help name</param>
        /// <returns>0 if success, 1 if no browser found</returns>
        public static int LaunchLocalHelp(string file)
        {
            string url = LocateLocalHelp(file); 
            return LaunchBrowser( url);
        }

        /// <summary>
        /// Locates the local executable and launches it
        /// </summary>
        /// <param name="utility_name">Name of the utility, without .exe</param>
        /// <returns>0 if success, 1 if lauch error</returns>
        public static int LaunchPetronodeUtility(string utility_name)
        {
            return LaunchPetronodeUtility( utility_name, "");
        }
        
        /// <summary>
        /// Locates the local executable and launches it
        /// </summary>
        /// <param name="utility_name">Name of the utility, without .exe</param>
        /// <returns>0 if success, 1 if lauch error</returns>
        public static int LaunchPetronodeUtility( string utility_name, string Command_Line)
        {
            string name = LocatePetronodeApplication(utility_name);
            return Launch( "\"" + name + "\"", Command_Line);
        }

        private static string LocateBrowser()
        {
            // default is the Explorer
            string def_path = "C:\\Program Files\\Internet Explorer\\iexplore.exe";
            try
            {
                string appPath = "";
                RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(c_RegPath);
                if (regKey != null)
                {
                    appPath = regKey.GetValue("").ToString();
                    regKey.Close();
                }
                else
                {
                    if (File.Exists(def_path)) return def_path;
                    return "";
                }
                int pos = appPath.LastIndexOf(".exe\"");
                if (pos <= 0) return appPath;
                return appPath.Substring( 0, pos+5);
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static string LocateLocalHelp( string help_name)
        {
            StringBuilder sb = new StringBuilder(Application.StartupPath);
            sb.Append("\\manuals\\");
            sb.Append(help_name);
            sb.Append("\\");
            sb.Append(help_name);
            sb.Append(".html");
            string name = sb.ToString();
            if (!File.Exists(name)) return Application.StartupPath + "\\Manuals.html";
            return name;
        }

        private static string LocatePetronodeApplication(string app_name)
        {
            StringBuilder sb = new StringBuilder(Application.StartupPath);
            sb.Append("\\");
            sb.Append(app_name);
            sb.Append(".exe");
            string name = sb.ToString();
            if (!File.Exists(name))
            {
                MessageBox.Show("Error: Unable to locate " + name, "Installation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return Application.StartupPath + "\\Petronode.About.exe";
            }
            return name;
        }
    }
}
