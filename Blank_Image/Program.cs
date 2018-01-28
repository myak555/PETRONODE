using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Petronode.Blank_Image
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length <= 2)
            {
                Console.WriteLine("Usage: Blank_Image file_name X Y");
                Console.WriteLine("Where :");
                Console.WriteLine("  X - image Width");
                Console.WriteLine("  Y - image Height");
                return -1;
            }
            try
            {
                int x = Convert.ToInt32(args[1]);
                int y = Convert.ToInt32(args[2]);
                Bitmap bmp = new Bitmap(x, y);
                Graphics g = Graphics.FromImage(bmp);
                g.Clear(Color.White);
                g.Dispose();
                Save(bmp, args[0]);
                bmp.Dispose();
                return y;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        static void Save(Bitmap bmp, string Destination)
        {
            FileInfo fi = new FileInfo(Destination);
            switch (fi.Extension.ToLower())
            {
                case ".png":
                    bmp.Save(Destination, System.Drawing.Imaging.ImageFormat.Png);
                    break;
                case ".tif":
                case ".tiff":
                    bmp.Save(Destination, System.Drawing.Imaging.ImageFormat.Tiff);
                    break;
                case ".jpg":
                case ".jpeg":
                    bmp.Save(Destination, System.Drawing.Imaging.ImageFormat.Jpeg);
                    break;
                case ".bmp":
                    bmp.Save(Destination, System.Drawing.Imaging.ImageFormat.Bmp);
                    break;
                case ".emf":
                    bmp.Save(Destination, System.Drawing.Imaging.ImageFormat.Emf);
                    break;
                case ".gif":
                    bmp.Save(Destination, System.Drawing.Imaging.ImageFormat.Gif);
                    break;
                case ".wmf":
                    bmp.Save(Destination, System.Drawing.Imaging.ImageFormat.Wmf);
                    break;
                default: break;
            }
        }
    }
}
