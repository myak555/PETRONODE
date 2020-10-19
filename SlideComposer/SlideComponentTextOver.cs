using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Petronode.SlideComposer
{
    /// <summary>
    /// Describes the Text component of the slide
    /// </summary>
    public class SlideComponentTextOver : SlideComponentLabel
    {
        /// <summary>
        /// Constructor, creates the image description
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="args"></param>
        public SlideComponentTextOver(Slide Parent, string[] args)
            : base(Parent, args) 
        {
            FontSize = (args.Length > 6) ? Convert.ToSingle(args[6]) : 60f;
            FontColor = (args.Length > 7) ? args[7] : "000000";
        }

        /// <summary>
        /// Draws the text
        /// </summary>
        /// <param name="source"></param>
        /// <param name="g"></param>
        public override void Draw(string source, Graphics g, Bitmap baseBitmap)
        {
            DirectoryInfo di = new DirectoryInfo(source);
            if (!di.Exists) throw new Exception(source + " not found");
            FileInfo[] fis = di.GetFiles(Text);
            if (fis.Length == 0) return;
            string filename = fis[0].FullName;
            FileStream fs = null;
            StreamReader sr = null;
            try
            {
                Rectangle destRectangle = new Rectangle(x, y, dx, dy);
                g.SetClip(destRectangle);
                Brush myBrush = new SolidBrush( Slide.GetColor( FontColor));
                Font myFont = this.GetFont();
                int location = y + myFont.Height;
                fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                sr = new StreamReader(fs);
                while (true)
                {
                    string s = sr.ReadLine();
                    if (s == null) break;
                    if (s.StartsWith("#")) continue;
                    g.DrawString(s, myFont, myBrush, x + myFont.Height, location);
                    location += myFont.Height + myFont.Height / 2;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (sr != null) sr.Close();
                if (fs != null) fs.Close();
                if (g != null) g.ResetClip();
            }
        }
    }
}
