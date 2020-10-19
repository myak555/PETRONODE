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
    public class SlideComponentImage: SlideComponentImageOver
    {
        /// <summary>
        /// Constructor, creates the image description
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="args"></param>
        public SlideComponentImage(Slide Parent, string[] args)
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
            DirectoryInfo di = new DirectoryInfo(source);
            if (!di.Exists) throw new Exception(source + " not found");
            FileInfo[] fis = di.GetFiles( Text);
            if( fis.Length == 0) return;
            string filename = fis[0].FullName;
            Bitmap imageBitmap = null;
            try
            {
                imageBitmap = new Bitmap(filename);
                int ddx = (source_dx <= 0) ? imageBitmap.Width - source_x : source_dx;
                int ddy = (source_dy <= 0) ? imageBitmap.Height - source_y : source_dy;
                Rectangle srcRectangle = new Rectangle(source_x, source_y, ddx, ddy);
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
