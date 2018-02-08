using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Petronode.Check_Image
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length <= 1)
            {
                Console.WriteLine("Usage: ImageProperties file_name /property");
                Console.WriteLine("Where /property is one of below:");
                Console.WriteLine("  X - image Width");
                Console.WriteLine("  Y - image Height");
                Console.WriteLine("  S - image file size (bytes)");
                Console.WriteLine("  P - image size (pixels)");
                return -1;
            }
            if (args.Length >= 2)
            {
                switch (args[1])
                {
                    case "/X": return GetImageWidth(args[0]);
                    case "/Y": return GetImageHeight(args[0]);
                    case "/S": return GetImageSize(args[0]);
                    case "/P": return GetPixelSize(args[0]);
                    default: return -1;
                }
            }
            return -1;
        }

        static int GetImageWidth( string inp)
        {
            Bitmap bmp = null;
            int tmp = -1;
            try
            {
                bmp = new Bitmap(inp);
                tmp = bmp.Width;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                if (bmp != null) bmp.Dispose();
            }
            return tmp;
        }

        static int GetImageHeight( string inp)
        {
            Bitmap bmp = null;
            int tmp = -1;
            try
            {
                bmp = new Bitmap( inp);
                tmp = bmp.Height;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                if (bmp != null) bmp.Dispose();
            }
            return tmp;
        }

        static int GetImageSize( string inp)
        {
            FileInfo fi = null;
            int tmp = -1;
            try
            {
                fi = new FileInfo( inp);
                if (!fi.Exists) return -1;
                tmp = (int)fi.Length;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return tmp;
        }

        static int GetPixelSize(string inp)
        {
            Bitmap bmp = null;
            int tmp = -1;
            try
            {
                bmp = new Bitmap(inp);
                tmp = bmp.Height * bmp.Width;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                if (bmp != null) bmp.Dispose();
            }
            return tmp;
        }
    }
}
