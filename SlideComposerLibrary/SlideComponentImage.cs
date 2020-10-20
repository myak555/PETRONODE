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
            if (args.Length > 6) source_x = Convert.ToInt32(args[6]);
            if (args.Length > 7) source_y = Convert.ToInt32(args[7]);
            if (args.Length > 8) source_dx = Convert.ToInt32(args[8]);
            if (args.Length > 9) source_dy = Convert.ToInt32(args[9]);
            SaveAttributes();
        }

        /// <summary>
        /// Draws the image
        /// </summary>
        /// <param name="source"></param>
        /// <param name="g"></param>
        public override void Draw(string source, Graphics g, Bitmap baseBitmap)
        {
            Bitmap imageBitmap = GetBitmap(source);
            if (imageBitmap == null) return;
            DrawImage(imageBitmap, g, baseBitmap, false);
        }

        protected override void Draw0(Bitmap imageBitmap, Graphics g, Bitmap baseBitmap, bool over)
        {
            try
            {
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
        }
    }
}
