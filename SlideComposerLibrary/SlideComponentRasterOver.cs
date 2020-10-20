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
    public class SlideComponentRasterOver: SlideComponent
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
        /// Last raster file (to avoid multiple initializations)
        /// </summary>
        public static RasterFile LastRaster = null;

        /// <summary>
        /// Constructor, creates the image description
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="args"></param>
        public SlideComponentRasterOver(Slide Parent, string[] args)
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
            GetDataFile(source);
            if (LastRaster.X_size <= 0 || LastRaster.Y_size <= 0) return;
            int ddx = (source_dx <= 0) ? LastRaster.X_size - source_x : source_dx;
            int ddy = (source_dy <= 0) ? LastRaster.Y_size - source_y : source_dy;
            if (ddx <= 0 || ddy <= 0) return;
            Bitmap imageBitmap = null;
            Bitmap imageBitmap2 = null;
            try
            {
                imageBitmap = LastRaster.GetBitmap(source_x, source_y, ddx, ddy, this.ParentSlide.BkColor);
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
                Rectangle srcRectangle = new Rectangle(0, 0, ddx, ddy);
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

        protected void GetDataFile(string source)
        {
            DirectoryInfo di = new DirectoryInfo(source);
            if (!di.Exists) throw new Exception(source + " not found");
            FileInfo[] fis = di.GetFiles(Text);
            if (fis.Length == 0) return;
            string filename = fis[0].FullName;
            if (LastRaster != null && LastRaster.FileName == filename) return;
            LastRaster = new RasterFile(filename);
        }
    }
}
