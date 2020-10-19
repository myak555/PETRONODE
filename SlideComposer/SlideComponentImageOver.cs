using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Petronode.SlideComposer
{
    /// <summary>
    /// Describes the Image component of the slide
    /// </summary>
    public class SlideComponentImageOver: SlideComponent
    {
        /// <summary>
        /// x location of the component
        /// </summary>
        public int source_x = 0;

        /// <summary>
        /// y location of the component
        /// </summary>
        public int source_y = 0;

        /// <summary>
        /// x size of the component
        /// </summary>
        public int source_dx = 0;

        /// <summary>
        /// y size of the component
        /// </summary>
        public int source_dy = 0;

        /// <summary>
        /// Transparency color
        /// </summary>
        public Color TransparentColor= Color.White;

        /// <summary>
        /// Specifies tolerance to transparency
        /// </summary>
        public int TransparentColorTolerance = 0;

        /// <summary>
        /// Constructor, creates the image description
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="args"></param>
        public SlideComponentImageOver(Slide Parent, string[] args)
            : base(Parent, args) 
        {
            source_x = (args.Length > 6) ? Convert.ToInt32(args[6]) : 0;
            source_y = (args.Length > 7) ? Convert.ToInt32(args[7]) : 0;
            source_dx = (args.Length > 8) ? Convert.ToInt32(args[8]) : -1;
            source_dy = (args.Length > 9) ? Convert.ToInt32(args[9]) : -1;
            TransparentColor = (args.Length > 10) ? Slide.GetColorARGB(args[10]) : Color.White;
            TransparentColorTolerance = (args.Length > 11) ? Convert.ToInt32(args[11]) : 0;
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
                    case "source_x": source_x = Convert.ToInt32(attrib[1].Trim()); break;
                    case "source_y": source_y = Convert.ToInt32(attrib[1].Trim()); break;
                    case "source_dx": source_dx = Convert.ToInt32(attrib[1].Trim()); break;
                    case "source_dy": source_dy = Convert.ToInt32(attrib[1].Trim()); break;
                    case "tcolor": TransparentColor = Slide.GetColorARGB(attrib[1].Trim().ToUpper()); break;
                    case "transparentcolor": TransparentColor = Slide.GetColorARGB(attrib[1].Trim().ToUpper()); break;
                    case "ttolerance": TransparentColorTolerance = Convert.ToInt32(attrib[1].Trim()); break;
                    case "transparentcolortolerance": TransparentColorTolerance = Convert.ToInt32(attrib[1].Trim()); break;
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
            DirectoryInfo di = new DirectoryInfo(source);
            if (!di.Exists) throw new Exception(source + " not found");
            FileInfo[] fis = di.GetFiles( Text);
            if( fis.Length == 0) return;
            string filename = fis[0].FullName;
            Bitmap imageBitmap = null;
            Bitmap imageBitmap2 = null;
            try
            {
                imageBitmap = new Bitmap(filename);
                imageBitmap2 = new Bitmap(imageBitmap.Width, imageBitmap.Height);
                for (int i = 0; i < imageBitmap.Width; i++)
                {
                    for (int j = 0; j < imageBitmap.Height; j++)
                    {
                        Color cc = imageBitmap.GetPixel(i, j);
                        Color cc2 = Slide.GetTransparencyColor(cc, TransparentColor, TransparentColorTolerance);
                        imageBitmap2.SetPixel(i, j, cc2);
                    }
                }
                int ddx = (source_dx <= 0) ? imageBitmap2.Width - source_x : source_dx;
                int ddy = (source_dy <= 0) ? imageBitmap2.Height - source_y : source_dy;
                Rectangle srcRectangle = new Rectangle(source_x, source_y, ddx, ddy);
                Rectangle destRectangle = new Rectangle(x, y, dx, dy);
                g.DrawImage(imageBitmap2, destRectangle, srcRectangle, GraphicsUnit.Pixel);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (imageBitmap != null) imageBitmap.Dispose();
                if (imageBitmap2 != null) imageBitmap2.Dispose();
            }
        }
    }
}
