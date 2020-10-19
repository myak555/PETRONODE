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
    public class SlideComponentShapeOver : SlideComponentShape
    {
        /// <summary>
        /// Constructor, creates the image description
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="args"></param>
        public SlideComponentShapeOver(Slide Parent, string[] args)
            : base(Parent, args) 
        {
            this.Text = this.Text.ToLower();
            switch (this.Text)
            {
                case "rectangle":
                case "ellipse":
                case "cross":
                case "xcross":
                case "dnline":
                case "upline":
                case "vline":
                case "hline":
                    break;
                default:
                    this.Text = "rectangle";
                    break;
            }
            LineSize = (args.Length > 6) ? Convert.ToSingle(args[6]) : 1f;
            FrontColor = (args.Length > 7) ? args[7] : "000000";
        }

        /// <summary>
        /// Draws the shape
        /// </summary>
        /// <param name="source"></param>
        /// <param name="g"></param>
        public override void Draw(string source, Graphics g, Bitmap baseBitmap)
        {
            try
            {
                int dec1 = 0;
                int dec2 = 0;
                int q = Convert.ToInt32(LineSize);
                if (q > 1)
                {
                    dec1 = q >> 1;
                    dec2 = q - 1;
                }
                Rectangle destRectangle = new Rectangle(x, y, dx + 1, dy + 1);
                Rectangle drawRectangle = new Rectangle(x + dec1, y + dec1, dx - dec2, dy - dec2);
                g.SetClip(destRectangle);
                Brush myBrush = new SolidBrush( Slide.GetColor( FrontColor));
                Pen myPen = new Pen(myBrush, LineSize);
                switch (this.Text)
                {
                    case "rectangle":
                        g.DrawRectangle(myPen, drawRectangle);
                        break;
                    case "ellipse":
                        g.DrawEllipse(myPen, drawRectangle);
                        break;
                    case "cross":
                        g.DrawLine(myPen, x + (dx >> 1), y, x + (dx >> 1), y + dy);
                        g.DrawLine(myPen, x, y + (dy >> 1), x + dx, y + (dy >> 1));
                        break;
                    case "xcross":
                        g.DrawLine(myPen, x, y, x + dx, y + dy);
                        g.DrawLine(myPen, x + dx, y, x, y + dy);
                        break;
                    case "dnline":
                        g.DrawLine(myPen, x, y, x + dx, y + dy);
                        break;
                    case "upline":
                        g.DrawLine(myPen, x + dx, y, x, y + dy);
                        break;
                    case "vline":
                        g.DrawLine(myPen, x, y + dy / 2, x + dx, y + dy / 2);
                        break;
                    case "hline":
                        g.DrawLine(myPen, x + dx / 2, y, x + dx / 2, y + dy);
                        break;
                    default:
                        break;
                }
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
