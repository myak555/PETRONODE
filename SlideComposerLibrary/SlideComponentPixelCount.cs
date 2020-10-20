using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Petronode.SlideComposerLibrary
{
    /// <summary>
    /// Describes the Image component of the slide
    /// </summary>
    public class SlideComponentPixelCount: SlideComponent
    {
        /// <summary>
        /// Count color
        /// </summary>
        public Color CountColor= Color.White;

        /// <summary>
        /// Specifies tolerance to CountColor
        /// </summary>
        public int CountColorTolerance = 0;

        /// <summary>
        /// Specifies if the new file must be created
        /// </summary>
        public bool NewFile = false;

        /// <summary>
        /// Constructor, creates the image description
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="args"></param>
        public SlideComponentPixelCount(Slide Parent, string[] args)
            : base(Parent, args) 
        {
            CountColor = (args.Length > 6) ? Slide.GetColorARGB(args[6]) : Color.White;
            CountColorTolerance = (args.Length > 7) ? Convert.ToInt32(args[7]) : 0;
            NewFile = (args.Length > 8) ? Convert.ToBoolean(args[8].Trim()) : false;
        }

        /// <summary>
        /// Changes the component attributes
        /// </summary>
        /// <param name="attribute">attribute string in form Name=Value</param>
        /// <returns>array of two strings: name and attribute</returns>
        public override string[] AddAttribute(string attribute)
        {
            string[] attrib = base.AddAttribute(attribute);
            if (attrib.Length <= 1) return attrib;
            try
            {
                switch (attrib[0])
                {
                    case "ccolor": CountColor = Slide.GetColorARGB(attrib[1].Trim().ToUpper()); break;
                    case "countcolor": CountColor = Slide.GetColorARGB(attrib[1].Trim().ToUpper()); break;
                    case "ctolerance": CountColorTolerance = Convert.ToInt32(attrib[1].Trim()); break;
                    case "countcolortolerance": CountColorTolerance = Convert.ToInt32(attrib[1].Trim()); break;
                    case "newfile": NewFile = Convert.ToBoolean(attrib[1].Trim()); break;
                    default: break;
                }
            }
            catch (Exception) { }
            return attrib;
        }

        /// <summary>
        /// Draws the image
        /// </summary>
        /// <param name="source"></param>
        /// <param name="g"></param>
        public override void Draw(string source, Graphics g, Bitmap baseBitmap)
        {
            Bitmap imageBitmap = null;
            Graphics tmpG = null;
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                imageBitmap = new Bitmap(dx, dy);
                tmpG = Graphics.FromImage(imageBitmap);
                Rectangle srcRect = new Rectangle( x, y, dx, dy);
                Rectangle dstRect = new Rectangle( 0, 0, dx, dy);
                tmpG.DrawImage(baseBitmap, dstRect, srcRect, GraphicsUnit.Pixel);
                int Count = 0;
                for (int iy = 0; iy < imageBitmap.Height; iy++)
                {
                    for (int ix = 0; ix < imageBitmap.Width; ix++)
                    {
                        Color cc = imageBitmap.GetPixel(ix, iy);
                        Color cc2 = Slide.GetTransparencyColor(cc, CountColor, CountColorTolerance);
                        if( cc2.A == 0) Count++;
                    }
                }
                DirectoryInfo di = new DirectoryInfo(source);
                if (!di.Exists) throw new Exception(source + " not found");
                string saveFile = di.FullName + "\\" + this.Text;
                if (NewFile) fs = File.Open(saveFile, FileMode.Create, FileAccess.Write, FileShare.Read);
                else fs = File.Open(saveFile, FileMode.Append, FileAccess.Write, FileShare.Read);
                sw = new StreamWriter(fs);
                StringBuilder sb = new StringBuilder();
                sb.Append( x.ToString());
                sb.Append(",");
                sb.Append( y.ToString());
                sb.Append(",");
                sb.Append( dx.ToString());
                sb.Append(",");
                sb.Append( dy.ToString());
                sb.Append(",");
                sb.Append( Count.ToString());
                sw.WriteLine(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (imageBitmap != null) imageBitmap.Dispose();
                if (tmpG != null) tmpG.Dispose();
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }
        }
    }
}
