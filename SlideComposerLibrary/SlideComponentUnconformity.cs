using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Petronode.SlideComposerLibrary
{
    /// <summary>
    /// Describes the Text component of the slide
    /// </summary>
    public class SlideComponentUnconformity : SlideComponentLabel
    {
        /// <summary>
        /// Top zone color
        /// </summary>
        public string TopColor = "FFFFFF";

        /// <summary>
        /// bottom zone color
        /// </summary>
        public string BottomColor = "FFFFFF";

        /// <summary>
        /// Constructor, creates the image description
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="args"></param>
        public SlideComponentUnconformity(Slide Parent, string[] args)
            : base(Parent, args) 
        {
            if(args.Length > 10) TopColor = args[10];
            if (args.Length > 10) BottomColor = args[11];
            SaveAttributes();
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
                    case "topcolor": TopColor = attrib[1].Trim().ToUpper(); break;
                    case "bottomcolor": BottomColor = attrib[1].Trim().ToUpper(); break;
                    default: break;
                }
            }
            catch (Exception) { }
            SaveAttributes();
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
                Font myFont = GetFont();
                Size myTextSize = TextRenderer.MeasureText(Text, myFont);
                Rectangle destRectangle = new Rectangle(x, y, dx, dy);
                g.SetClip(destRectangle);
                //g.Clear(Color.Bisque); // Test only
                int lineStep = Convert.ToInt32(LineSize);
                if (lineStep < 1) lineStep = 1;
                int half = lineStep>>1;
                int step = (Text_dy >> 1) - lineStep;
                int stepIncrement = 1;
                Brush myBrushF = new SolidBrush(Slide.GetColor(FontColor));
                Brush myBrushT = new SolidBrush(Slide.GetColor(TopColor));
                Brush myBrushB = new SolidBrush(Slide.GetColor(BottomColor));
                g.FillRectangle(myBrushB, Text_x, Text_y + lineStep, Text_dx, Text_dy - (lineStep << 1));
                if (Callout_x >= 0 && Callout_y > Text_y + lineStep)
                {
                    g.FillRectangle(myBrushB, Callout_x, Text_y + lineStep, myTextSize.Width + myFont.Height, Callout_y - Text_y - lineStep);
                    g.DrawString(Text, myFont, myBrushF, (float)Callout_x + myFont.GetHeight() / 2f, Text_y + Text_dy + myFont.GetHeight() / 2f);
                }
                if (Callout_x >= 0 && Callout_y >=0 && Callout_y < Text_y)
                {
                    g.FillRectangle(myBrushT, Callout_x, Callout_y, myTextSize.Width + myFont.Height, Text_y + lineStep - Callout_y);
                    g.DrawString(Text, myFont, myBrushF, (float)Callout_x + myFont.GetHeight() / 2f, (float)Callout_y + myFont.GetHeight() / 2f); 
                }
                myBrushB.Dispose();
                myBrushT.Dispose();
                myBrushF.Dispose();
                Color cTop = Slide.GetColor(TopColor);
                Color cLine = Slide.GetColor(this.FontColor);
                for (int ix = 0; ix < Text_dx; ix++)
                {
                    int jx = ix + Text_x;
                    int step1 = step;
                    if (step1 < 0) step1 = 0;
                    if (step1 > Text_dy - lineStep) step1 = Text_dy - lineStep;
                    for (int iy = lineStep; iy < step1; iy++)
                    {
                        int jy = iy + Text_y;
                        baseBitmap.SetPixel(jx, jy, cTop);
                    }
                    for (int iy = 0; iy < lineStep; iy++)
                    {
                        int jy = iy + Text_y + step1;
                        baseBitmap.SetPixel(jx, jy, cLine);
                    }
                    step += stepIncrement;
                    if (step >= Text_dy) stepIncrement = -1;
                    if (step <= -lineStep) stepIncrement = 1;
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

        /// <summary>
        /// Recovers attributes from the previous component
        /// </summary>
        protected override void RecoverAttributes()
        {
            base.RecoverAttributes();
            try
            {
                this.TopColor = ParentSlide.DefaultParameters["topcolor"];
                this.BottomColor = ParentSlide.DefaultParameters["bottomcolor"];
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Saves attributes from the previous component
        /// </summary>
        protected override void SaveAttributes()
        {
            base.SaveAttributes();
            try
            {
                ParentSlide.DefaultParameters["topcolor"] = TopColor;
                ParentSlide.DefaultParameters["bottomcolor"] = BottomColor;
            }
            catch (Exception) { }
        }
    }
}
