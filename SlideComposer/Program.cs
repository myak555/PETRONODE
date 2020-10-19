using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using Petronode.SlideComposerLibrary;

namespace Petronode.SlideComposer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main( string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Silent mode - single well
            if (args.Length >= 4 && args[3].StartsWith("/s"))
            {
                ComposeSingleWell(args);
                return;
            }

            // Silent mode - multiple wells
            if (args.Length >= 4 && args[3].StartsWith("/m"))
            {
                ComposeMultiWell(args);
                return;
            }

            // Interactive mode
            Application.Run(new Form1( args));
        }

        /// <summary>
        /// Perfoms silent processing for single directory
        /// </summary>
        /// <param name="args"></param>
        static void ComposeSingleWell(string[] args)
        {
            try
            {
                SlideDescriptionFile sdp = new SlideDescriptionFile(args[0]);
                sdp.ComposeSlides(args[1], args[2]);
                Console.WriteLine("Processing completed for " + args[2]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Perfoms silent recursive processing for multiple directories
        /// </summary>
        /// <param name="args"></param>
        static void ComposeMultiWell(string[] args)
        {
            try
            {
                SlideDescriptionFile sdp = new SlideDescriptionFile(args[0]);
                sdp.ComposeSlides(args[1], args[2]);
                DirectoryInfo di = new DirectoryInfo(args[1]);
                if (!di.Exists) throw new Exception(args[1] + "not found");
                DirectoryInfo[] dis = di.GetDirectories();
                if (!Directory.Exists(args[2])) Directory.CreateDirectory(args[2]);
                foreach (DirectoryInfo dd in dis)
                {
                    string src = args[1] + "\\" + dd.Name;
                    string dst = args[2] + "\\" + dd.Name;
                    sdp.ComposeSlides(src, dst);
                    Console.WriteLine("Processing completed for " + dst);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return;
        }
    }
}