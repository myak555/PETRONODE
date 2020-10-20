using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Petronode.SlideComposerLibrary
{
    /// <summary>
    /// Describes the Text component of the slide
    /// </summary>
    public class SlideComponentText : SlideComponentLabel
    {
        /// <summary>
        /// Constructor, creates the image description
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="args"></param>
        public SlideComponentText(Slide Parent, string[] args)
            : base(Parent, args) 
        {
        }

        /// <summary>
        /// Draws the text
        /// </summary>
        /// <param name="source"></param>
        /// <param name="g"></param>
        public override void Draw(string source, Graphics g, Bitmap baseBitmap)
        {
            Color b = Slide.GetColor(this.BackColor);
            List<string> t = GetFile( source);
            DrawText(t, g, baseBitmap, b);
        }

        protected List<string> GetFile( string source)
        {
            List<string> lines = new List<string>();
            DirectoryInfo di = new DirectoryInfo(source);
            if (!di.Exists) throw new Exception(source + " not found");
            FileInfo[] fis = di.GetFiles(Text);
            if (fis.Length == 0) return lines;
            string filename = fis[0].FullName;
            FileStream fs = null;
            StreamReader sr = null;
            try
            {
                fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                sr = new StreamReader( fs, Encoding.GetEncoding(1251), false);
                while (true)
                {
                    string s = sr.ReadLine();
                    if (s == null) break;
                    if (s.StartsWith("#")) continue;
                    s = s.Replace("\r", "").Replace("\n", "");
                    lines.Add(s);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (sr != null) sr.Close();
                if (fs != null) fs.Close();
            }
            return lines;
        }
    }
}
