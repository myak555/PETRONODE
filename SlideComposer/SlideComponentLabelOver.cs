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
    public class SlideComponentLabelOver : SlideComponentLabel
    {
        /// <summary>
        /// Constructor, creates the image description
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="args"></param>
        public SlideComponentLabelOver(Slide Parent, string[] args)
            : base(Parent, args) 
        {
            this.Text = this.Text.Replace("&comma", ",");
            this.Text = this.Text.Replace("&space", " ");
            this.Text = this.Text.Replace("&left", "(");
            this.Text = this.Text.Replace("&right", ")");
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
            try
            {
                Rectangle destRectangle = new Rectangle(x, y, dx, dy);
                g.SetClip(destRectangle);
                Brush myBrush = new SolidBrush( Slide.GetColor( FontColor));
                Font myFont = this.GetFont();
                g.DrawString( Text, myFont, myBrush, x + 3, y);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (g != null) g.ResetClip();
            }
        }
    }
}
