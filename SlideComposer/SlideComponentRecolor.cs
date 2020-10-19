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
    public class SlideComponentRecolor : SlideComponent
    {
        /// <summary>
        /// Font bColor
        /// </summary>
        public string OriginColor = "FFFFFF";

        /// <summary>
        /// Specifies tolerance to transparency
        /// </summary>
        public int OriginColorTolerance = 0;

        /// <summary>
        /// Constructor, creates the image description
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="args"></param>
        public SlideComponentRecolor(Slide Parent, string[] args)
            : base(Parent, args) 
        {
            this.Text = this.Text.ToUpper();
            OriginColor = (args.Length > 6) ? args[6] : "FFFFFF";
            OriginColorTolerance = (args.Length > 7) ? Convert.ToInt32( args[7]) : 0;
            if (this.Text == "*.*") this.Text = "000000";
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
                    case "fcolor": this.Text = attrib[1].Trim().ToUpper(); break;
                    case "tcolor": OriginColor = attrib[1].Trim().ToUpper(); break;
                    case "transparentcolor": OriginColor = attrib[1].Trim().ToUpper(); break;
                    case "ttolerance": OriginColorTolerance = Convert.ToInt32(attrib[1].Trim()); break;
                    case "transparentcolortolerance": OriginColorTolerance = Convert.ToInt32(attrib[1].Trim()); break;
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
                Color orgColor = Slide.GetColor(this.OriginColor);
                Color resColor = Slide.GetColor(Text);
                for (int yy = 0; yy < this.dy; yy++)
                {
                    int yy1 = yy + this.y;
                    if( yy1 < 0) continue;
                    if( yy1 >= baseBitmap.Height) continue;
                    for (int xx = 0; xx < this.dx; xx++)
                    {
                        int xx1 = xx + this.x;
                        if (xx1 < 0) continue;
                        if (xx1 >= baseBitmap.Width) continue;
                        Color c = baseBitmap.GetPixel(xx1, yy1);
                        int RR = c.R - orgColor.R;
                        if (RR < -this.OriginColorTolerance || RR > this.OriginColorTolerance) continue;
                        int GG = c.G - orgColor.G;
                        if (GG < -this.OriginColorTolerance || GG > this.OriginColorTolerance) continue;
                        int BB = c.B - orgColor.B;
                        if (BB < -this.OriginColorTolerance || BB > this.OriginColorTolerance) continue;
                        baseBitmap.SetPixel(xx1, yy1, resColor);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
