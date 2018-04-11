using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MY.Weatherford.EBCDIC_Set
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 f = new Form1( args);
            if (!f.Completed) Application.Run(f);
            else Console.WriteLine("EBCDIC header updated successfully");
        }
    }
}