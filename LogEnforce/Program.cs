using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Petronode.LogEnforce
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
            if (args.Length <= 0) Application.Run(new Form1());
            if (args.Length == 1) Application.Run(new Form1(args[0]));
            if (args.Length >= 2) Application.Run(new Form1(args[0], args[1]));
            if (args.Length >= 3) Application.Run(new Form1(args[0], args[1], args[2]));
        }
    }
}