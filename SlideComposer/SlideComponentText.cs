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
            FontSize = (args.Length > 6) ? Convert.ToSingle(args[6]) : 60f;
            FontColor = (args.Length > 7) ? args[7] : "000000";
            BackColor = (args.Length > 8) ? args[8] : "FFFFFF";
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
            Bitmap txtBitmap = null; 
            FileStream fs = null;
            StreamReader sr = null;
            Graphics g1 = null;
            try
            {
                txtBitmap = new Bitmap(dx, dy);
                g1 = Graphics.FromImage(txtBitmap);
                g1.Clear( Slide.GetColor( BackColor));
                Brush myBrush = new SolidBrush( Slide.GetColor( FontColor));
                Font myFont = this.GetFont();
                int location = myFont.Height;
                fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                sr = new StreamReader(fs);
                while (true)
                {
                    string s = sr.ReadLine();
                    if (s == null) break;
                    if (s.StartsWith("#")) continue;
                    g1.DrawString(s, myFont, myBrush, myFont.Height, location);
                    location += myFont.Height + myFont.Height / 2;
                }
                Rectangle srcRectangle = new Rectangle(0, 0, dx, dy);
                Rectangle destRectangle = new Rectangle(x, y, dx, dy);
                g.DrawImage( txtBitmap, destRectangle, srcRectangle, GraphicsUnit.Pixel);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (sr != null) sr.Close();
                if (fs != null) fs.Close();
                if (g1 != null) g1.Dispose();
                if (txtBitmap != null) txtBitmap.Dispose();
            }
        }
    }
}
