using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Petronode.SlideComposerLibrary
{
    public class RasterFile
    {
        char[] c_Split = { ',' };
        List<long> linePositions = new List<long>();
        List<int> lineLengths = new List<int>();
        public string FileName = "";
        public int X_size = -1;
        public int Y_size = -1;

        public RasterFile(string filename)
        {
            FileName = filename;
            if (!File.Exists(FileName)) return;
            FileStream fs = null;
            StreamReader sr = null;
            try
            {
                fs = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                sr = new StreamReader(fs);
                long pos = 0L;
                while (true)
                {
                    linePositions.Add( pos);
                    string s = sr.ReadLine();
                    if (s == null) break;
                    lineLengths.Add(s.Length);
                    pos += (long)(s.Length+2);
                    string[] ss = s.Split(c_Split);
                    if( ss.Length > X_size) X_size = ss.Length;
                }
                while (linePositions.Count > lineLengths.Count) linePositions.RemoveAt(linePositions.Count - 1); 
                Y_size = linePositions.Count;
            }
            catch( Exception)
            {
            }
            finally
            {
                if( sr != null) sr.Close();
                if( fs != null) fs.Close();
            }
        }

        public Bitmap GetBitmap(int x, int y, int dx, int dy, Color bColor)
        {
            Bitmap bmp = new Bitmap(dx, dy);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(bColor);
            g.Dispose();
            if (!File.Exists(FileName)) return bmp;
            FileStream fs = null;
            try
            {
                fs = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                for( int iy = 0; iy<dy; iy++)
                {
                    int jy = y + iy;
                    if( jy < 0) continue;
                    string fileLine = "";
                    fs.Seek(linePositions[jy], SeekOrigin.Begin);
                    byte[] buffer = new byte[lineLengths[jy]];
                    fs.Read(buffer, 0, buffer.Length);
                    StringBuilder sb = new StringBuilder();
                    foreach( byte b in buffer) sb.Append( (char)b);
                    fileLine = sb.ToString();
                    string[] ss = fileLine.Split( c_Split);
                    for (int ix = 0; ix < dx; ix++)
                    {
                        int jx = x + ix;
                        if (jx < 0) continue;
                        if (jx >= ss.Length) break;
                        bmp.SetPixel( ix, iy, Slide.GetColor( ss[jx]));
                    }
                }
            }
            catch( Exception)
            {
            }
            finally
            {
                if( fs != null) fs.Close();
            }
            return bmp;
        }
    }
}
