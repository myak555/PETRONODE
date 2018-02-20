using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace Petronode.AUS_Geomag
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
            Form f = null;
            if (args.Length >= 1 && File.Exists(args[0]))
            {
                f = new Form1(args[0]);
            }
            else 
            {
                f = new Form1();
            }
            Application.Run(f);
        }
    }
}