using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Petronode.Automation
{
    public class ProcessingLogging
    {
        private bool isDummy = true;
        private string log_file = "";
        
        /// <summary>
        /// Constructor, creates the processing log object
        /// </summary>
        /// <param name="workfolder"></param>
        public ProcessingLogging(string workfolder)
        {
            isDummy = workfolder.Length < 1;
            log_file = workfolder + "\\_ProcessingLog.txt";
        }

        /// <summary>
        /// Logs event at any given time
        /// </summary>
        /// <param name="description">event description</param>
        public void LogEvent( string description)
        {
            if (isDummy) return;
            StreamWriter sw = null;
            try
            {
                sw = File.AppendText(log_file);
                sw.WriteLine(DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss") + " " + description);
            }
            catch (Exception)
            {
            }
            finally
            {
                if (sw != null) sw.Close();
            }
        }

        /// <summary>
        /// Logs event at any given time
        /// </summary>
        /// <param name="analyst">analyst name</param>
        /// <param name="description">event description</param>
        public void LogEvent(string analyst, string description)
        {
            if (analyst == null || analyst.Length <= 0) LogEvent(description);
            else LogEvent(analyst + " " + description);
        }
    }
}
