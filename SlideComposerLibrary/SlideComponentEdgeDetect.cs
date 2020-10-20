using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Petronode.SlideComposerLibrary
{
    /// <summary>
    /// Describes the Text component of the slide
    /// </summary>
    public class SlideComponentEdgeDetect : SlideComponent
    {
        /// <summary>
        /// Edge color
        /// </summary>
        public string EdgeColor = "000000";

        /// <summary>
        /// Specifies edge detection threshhold
        /// </summary>
        public int EdgeTolerance = 128;

        /// <summary>
        /// Specifies edge detection direction
        /// </summary>
        public string EdgeDetection = "north";

        /// <summary>
        /// Constructor, creates the image description
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="args"></param>
        public SlideComponentEdgeDetect(Slide Parent, string[] args)
            : base(Parent, args) 
        {
            this.Text = this.Text.ToUpper();
            EdgeColor = (args.Length > 6) ? args[6] : "000000";
            EdgeTolerance = (args.Length > 7) ? Convert.ToInt32(args[7]) : 0;
            EdgeDetection = (args.Length > 8) ? args[8] : "north";
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
                    case "ecolor": EdgeColor = attrib[1].Trim().ToUpper(); break;
                    case "edgecolor": EdgeColor = attrib[1].Trim().ToUpper(); break;
                    case "etolerance": EdgeTolerance = Convert.ToInt32(attrib[1].Trim()); break;
                    case "edgetolerance": EdgeTolerance = Convert.ToInt32(attrib[1].Trim()); break;
                    case "edirection": EdgeDetection = attrib[1].Trim().ToLower(); break;
                    case "edgedirection": EdgeDetection = attrib[1].Trim().ToLower(); break;
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
                Color orgColor = Slide.GetColor(this.EdgeColor);
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
                        if (RR < -this.EdgeTolerance || RR > this.EdgeTolerance) continue;
                        int GG = c.G - orgColor.G;
                        if (GG < -this.EdgeTolerance || GG > this.EdgeTolerance) continue;
                        int BB = c.B - orgColor.B;
                        if (BB < -this.EdgeTolerance || BB > this.EdgeTolerance) continue;
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
