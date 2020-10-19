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
    public class SlideComponentShape : SlideComponent
    {
        /// <summary>
        /// Font size
        /// </summary>
        public float LineSize = 1f;

        /// <summary>
        /// Font bColor
        /// </summary>
        public string FrontColor = "000000";

        /// <summary>
        /// Font bColor
        /// </summary>
        public string BackColor = "FFFFFF";

        /// <summary>
        /// Constructor, creates the image description
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="args"></param>
        public SlideComponentShape(Slide Parent, string[] args)
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
            BackColor = (args.Length > 8) ? args[8] : "FFFFFF";
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
                    case "shape": Text = attrib[1].Trim(); break;
                    case "lsize": LineSize = Convert.ToInt32(attrib[1].Trim()); break;
                    case "linesize": LineSize = Convert.ToInt32(attrib[1].Trim()); break;
                    case "fcolor": FrontColor = attrib[1].Trim().ToUpper(); break;
                    case "frontcolor": FrontColor = attrib[1].Trim().ToUpper(); break;
                    case "bcolor": BackColor = attrib[1].Trim().ToUpper(); break;
                    case "backcolor": BackColor = attrib[1].Trim().ToUpper(); break;
                    default: break;
                }
            }
            catch (Exception) { }
            return attrib;
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
                Brush myBrush = new SolidBrush(Slide.GetColor(FrontColor));
                Pen myPen = new Pen(myBrush, LineSize);
                Brush mybkBrush = new SolidBrush(Slide.GetColor(BackColor));
                switch (this.Text)
                {
                    case "rectangle":
                        g.Clear(Slide.GetColor(BackColor));
                        g.DrawRectangle(myPen, drawRectangle);
                        break;
                    case "ellipse":
                        g.FillEllipse(mybkBrush, destRectangle);
                        g.DrawEllipse(myPen, drawRectangle);
                        break;
                    case "cross":
                        g.Clear(Slide.GetColor(BackColor));
                        g.DrawLine(myPen, x + dx / 2, y, x + dx / 2, y + dy);
                        g.DrawLine(myPen, x, y + dy / 2, x + dx, y + dy / 2);
                        break;
                    case "xcross":
                        g.Clear(Slide.GetColor(BackColor));
                        g.DrawLine(myPen, x, y, x + dx, y + dy);
                        g.DrawLine(myPen, x + dx, y, x, y + dy);
                        break;
                    case "dnline":
                        g.Clear(Slide.GetColor(BackColor));
                        g.DrawLine(myPen, x, y, x + dx, y + dy);
                        break;
                    case "upline":
                        g.Clear(Slide.GetColor(BackColor));
                        g.DrawLine(myPen, x + dx, y, x, y + dy);
                        break;
                    case "vline":
                        g.Clear(Slide.GetColor(BackColor));
                        g.DrawLine(myPen, x, y + dy/2, x+dx, y + dy/2);
                        break;
                    case "hline":
                        g.Clear(Slide.GetColor(BackColor));
                        g.DrawLine(myPen, x + dx/2, y, x + dx/2, y + dy);
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
