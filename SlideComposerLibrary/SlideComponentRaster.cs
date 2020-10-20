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
    public class SlideComponentRaster: SlideComponentRasterOver
    {
        /// <summary>
        /// Constructor, creates the image description
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="args"></param>
        public SlideComponentRaster(Slide Parent, string[] args)
            : base(Parent, args) 
        {
            source_x = (args.Length > 6) ? Convert.ToInt32(args[6]) : 0;
            source_y = (args.Length > 7) ? Convert.ToInt32(args[7]) : 0;
            source_dx = (args.Length > 8) ? Convert.ToInt32(args[8]) : -1;
            source_dy = (args.Length > 9) ? Convert.ToInt32(args[9]) : -1;
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
            try
            {
                imageBitmap = LastRaster.GetBitmap(source_x, source_y, ddx, ddy, this.ParentSlide.BkColor);
                Rectangle srcRectangle = new Rectangle(0, 0, ddx, ddy);
                Rectangle destRectangle = new Rectangle(x, y, dx, dy);
                g.DrawImage(imageBitmap, destRectangle, srcRectangle, GraphicsUnit.Pixel);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (imageBitmap != null) imageBitmap.Dispose();
            }
        }
    }
}
