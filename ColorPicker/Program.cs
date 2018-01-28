using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Petronode.ColorPicker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main( string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 frm = new Form1(args);
            Application.Run(frm);
            return frm.ReturnValue;
        }
    }
}