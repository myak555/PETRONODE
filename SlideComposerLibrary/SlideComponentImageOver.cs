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
    public class SlideComponentImageOver: SlideComponent
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
        /// Constructor, creates the image description
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="args"></param>
        public SlideComponentImageOver(Slide Parent, string[] args)
            : base(Parent, args) 
        {
            if (args.Length > 6) source_x = Convert.ToInt32(args[6]);
            if (args.Length > 7) source_y = Convert.ToInt32(args[7]);
            if (args.Length > 8) source_dx = Convert.ToInt32(args[8]);
            if (args.Length > 9) source_dy = Convert.ToInt32(args[9]);
            if (args.Length > 10) TransparentColor = Slide.GetColorARGB(args[10]);
            if (args.Length > 11) TransparentColorTolerance = Convert.ToInt32(args[11]);
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
            SaveAttributes();
            return attrib;
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
            DrawImage(imageBitmap, g, baseBitmap, true);
        }

        protected Bitmap GetBitmap(string source)
        {
            Bitmap imageBitmap = null;
            DirectoryInfo di = new DirectoryInfo(source);
            if (!di.Exists) throw new Exception(source + " not found");
            FileInfo[] fis = di.GetFiles(Text);
            if (fis.Length == 0) return imageBitmap;
            string filename = fis[0].FullName;
            FileStream fs = null;
            try
            {
                fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                imageBitmap = new Bitmap(fs);
            }
            catch (Exception) { }
            finally
            {
                if (fs != null) fs.Close();
            }
            return imageBitmap;
        }

        protected void DrawImage(Bitmap imageBitmap, Graphics g, Bitmap baseBitmap, bool over)
        {
            switch (Orientation)
            {
                case "0":
                    Draw0(imageBitmap, g, baseBitmap, over);
                    break;
                case "90":
                    Draw90(imageBitmap, g, baseBitmap, over);
                    break;
                case "180":
                    Draw180(imageBitmap, g, baseBitmap, over);
                    break;
                case "270":
                    Draw270(imageBitmap, g, baseBitmap, over);
                    break;
                case "mirror":
                    DrawMirror(imageBitmap, g, baseBitmap, over);
                    break;
                case "flip":
                    DrawFlip(imageBitmap, g, baseBitmap, over);
                    break;
                default:
                    Draw0(imageBitmap, g, baseBitmap, over);
                    break;
            }
        }

        /// <summary>
        /// Recovers attributes from the previous component
        /// </summary>
        protected override void RecoverAttributes()
        {
            try
            {
                this.x = Convert.ToInt32(ParentSlide.DefaultParameters["x"]);
                this.y = Convert.ToInt32(ParentSlide.DefaultParameters["y"]);
                this.dx = Convert.ToInt32(ParentSlide.DefaultParameters["dx"]);
                this.dy = Convert.ToInt32(ParentSlide.DefaultParameters["dy"]);
                this.Text = ParentSlide.DefaultParameters["mask"];
                this.source_x = Convert.ToInt32(ParentSlide.DefaultParameters["source_x"]);
                this.source_y = Convert.ToInt32(ParentSlide.DefaultParameters["source_y"]);
                this.source_dx = Convert.ToInt32(ParentSlide.DefaultParameters["source_dx"]);
                this.source_dy = Convert.ToInt32(ParentSlide.DefaultParameters["source_dy"]);
                this.TransparentColor = Slide.GetColorARGB(ParentSlide.DefaultParameters["tcolor"]);
                this.TransparentColorTolerance = Convert.ToInt32(ParentSlide.DefaultParameters["ttolerance"]);
                this.Orientation = ParentSlide.DefaultParameters["orientation"];
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Saves attributes from the previous component
        /// </summary>
        protected override void SaveAttributes()
        {
            try
            {
                ParentSlide.DefaultParameters["x"] = x.ToString();
                ParentSlide.DefaultParameters["y"] = y.ToString();
                ParentSlide.DefaultParameters["dx"] = dx.ToString();
                ParentSlide.DefaultParameters["dy"] = dy.ToString();
                ParentSlide.DefaultParameters["mask"] = Text;
                ParentSlide.DefaultParameters["source_x"] = source_x.ToString();
                ParentSlide.DefaultParameters["source_y"] = source_y.ToString();
                ParentSlide.DefaultParameters["source_dx"] = source_dx.ToString();
                ParentSlide.DefaultParameters["source_dy"] = source_dy.ToString();
                ParentSlide.DefaultParameters["tcolor"] = Slide.GetColorString(TransparentColor);
                ParentSlide.DefaultParameters["ttolerance"] = TransparentColorTolerance.ToString();
                ParentSlide.DefaultParameters["orientation"] = Orientation;
            }
            catch (Exception) { }
        }

        protected virtual void Draw0( Bitmap imageBitmap, Graphics g, Bitmap baseBitmap, bool over)
        {
            Bitmap imageBitmap2 = null;
            try
            {
                imageBitmap2 = new Bitmap(imageBitmap.Width, imageBitmap.Height);
                for (int i = 0; i < imageBitmap.Width; i++)
                {
                    for (int j = 0; j < imageBitmap.Height; j++)
                    {
                        Color cc = imageBitmap.GetPixel(i, j);
                        Color cc2 = over? Slide.GetTransparencyColor(cc, TransparentColor, TransparentColorTolerance): cc;
                        imageBitmap2.SetPixel(i, j, cc2);
                    }
                }
                int ddx = (source_dx <= 0) ? imageBitmap2.Width - source_x : source_dx;
                int ddy = (source_dy <= 0) ? imageBitmap2.Height - source_y : source_dy;
                Rectangle srcRectangle = new Rectangle(source_x, source_y, ddx, ddy);
                Rectangle destRectangle = new Rectangle(x, y, dx, dy);
                g.DrawImage(imageBitmap2, destRectangle, srcRectangle, GraphicsUnit.Pixel);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (imageBitmap2 != null) imageBitmap2.Dispose();
            }
        }

        private void Draw90(Bitmap imageBitmap, Graphics g, Bitmap baseBitmap, bool over)
        {
            Bitmap imageBitmap2 = null;
            try
            {
                imageBitmap2 = new Bitmap(imageBitmap.Height, imageBitmap.Width);
                for (int i = 0; i < imageBitmap.Width; i++)
                {
                    for (int j = 0; j < imageBitmap.Height; j++)
                    {
                        Color cc = imageBitmap.GetPixel(i, j);
                        Color cc2 = over ? Slide.GetTransparencyColor(cc, TransparentColor, TransparentColorTolerance) : cc;
                        imageBitmap2.SetPixel(imageBitmap2.Width - j - 1, i, cc2);
                    }
                }
                int ddx = (source_dx <= 0) ? imageBitmap2.Width - source_x : source_dx;
                int ddy = (source_dy <= 0) ? imageBitmap2.Height - source_y : source_dy;
                Rectangle srcRectangle = new Rectangle(source_x, source_y, ddx, ddy);
                Rectangle destRectangle = new Rectangle(x, y, dx, dy);
                g.DrawImage(imageBitmap2, destRectangle, srcRectangle, GraphicsUnit.Pixel);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (imageBitmap2 != null) imageBitmap2.Dispose();
            }
        }

        private void Draw180(Bitmap imageBitmap, Graphics g, Bitmap baseBitmap, bool over)
        {
            Bitmap imageBitmap2 = null;
            try
            {
                imageBitmap2 = new Bitmap(imageBitmap.Width, imageBitmap.Height);
                for (int i = 0; i < imageBitmap.Width; i++)
                {
                    for (int j = 0; j < imageBitmap.Height; j++)
                    {
                        Color cc = imageBitmap.GetPixel(i, j);
                        Color cc2 = over ? Slide.GetTransparencyColor(cc, TransparentColor, TransparentColorTolerance) : cc;
                        imageBitmap2.SetPixel(imageBitmap2.Width - i - 1, imageBitmap2.Height - j - 1, cc2);
                    }
                }
                int ddx = (source_dx <= 0) ? imageBitmap2.Width - source_x : source_dx;
                int ddy = (source_dy <= 0) ? imageBitmap2.Height - source_y : source_dy;
                Rectangle srcRectangle = new Rectangle(source_x, source_y, ddx, ddy);
                Rectangle destRectangle = new Rectangle(x, y, dx, dy);
                g.DrawImage(imageBitmap2, destRectangle, srcRectangle, GraphicsUnit.Pixel);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (imageBitmap2 != null) imageBitmap2.Dispose();
            }
        }

        private void Draw270(Bitmap imageBitmap, Graphics g, Bitmap baseBitmap, bool over)
        {
            Bitmap imageBitmap2 = null;
            try
            {
                imageBitmap2 = new Bitmap(imageBitmap.Height, imageBitmap.Width);
                for (int i = 0; i < imageBitmap.Width; i++)
                {
                    for (int j = 0; j < imageBitmap.Height; j++)
                    {
                        Color cc = imageBitmap.GetPixel(i, j);
                        Color cc2 = over ? Slide.GetTransparencyColor(cc, TransparentColor, TransparentColorTolerance) : cc;
                        imageBitmap2.SetPixel(j, imageBitmap2.Height - i - 1, cc2);
                    }
                }
                int ddx = (source_dx <= 0) ? imageBitmap2.Width - source_x : source_dx;
                int ddy = (source_dy <= 0) ? imageBitmap2.Height - source_y : source_dy;
                Rectangle srcRectangle = new Rectangle(source_x, source_y, ddx, ddy);
                Rectangle destRectangle = new Rectangle(x, y, dx, dy);
                g.DrawImage(imageBitmap2, destRectangle, srcRectangle, GraphicsUnit.Pixel);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (imageBitmap2 != null) imageBitmap2.Dispose();
            }
        }

        private void DrawMirror(Bitmap imageBitmap, Graphics g, Bitmap baseBitmap, bool over)
        {
            Bitmap imageBitmap2 = null;
            try
            {
                imageBitmap2 = new Bitmap(imageBitmap.Width, imageBitmap.Height);
                for (int i = 0; i < imageBitmap.Width; i++)
                {
                    for (int j = 0; j < imageBitmap.Height; j++)
                    {
                        Color cc = imageBitmap.GetPixel(i, j);
                        Color cc2 = over ? Slide.GetTransparencyColor(cc, TransparentColor, TransparentColorTolerance) : cc;
                        imageBitmap2.SetPixel(imageBitmap2.Width - i - 1, j, cc2);
                    }
                }
                int ddx = (source_dx <= 0) ? imageBitmap2.Width - source_x : source_dx;
                int ddy = (source_dy <= 0) ? imageBitmap2.Height - source_y : source_dy;
                Rectangle srcRectangle = new Rectangle(source_x, source_y, ddx, ddy);
                Rectangle destRectangle = new Rectangle(x, y, dx, dy);
                g.DrawImage(imageBitmap2, destRectangle, srcRectangle, GraphicsUnit.Pixel);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (imageBitmap2 != null) imageBitmap2.Dispose();
            }
        }

        private void DrawFlip(Bitmap imageBitmap, Graphics g, Bitmap baseBitmap, bool over)
        {
            Bitmap imageBitmap2 = null;
            try
            {
                imageBitmap2 = new Bitmap(imageBitmap.Width, imageBitmap.Height);
                for (int i = 0; i < imageBitmap.Width; i++)
                {
                    for (int j = 0; j < imageBitmap.Height; j++)
                    {
                        Color cc = imageBitmap.GetPixel(i, j);
                        Color cc2 = over ? Slide.GetTransparencyColor(cc, TransparentColor, TransparentColorTolerance) : cc;
                        imageBitmap2.SetPixel(i, imageBitmap2.Height - j - 1, cc2);
                    }
                }
                int ddx = (source_dx <= 0) ? imageBitmap2.Width - source_x : source_dx;
                int ddy = (source_dy <= 0) ? imageBitmap2.Height - source_y : source_dy;
                Rectangle srcRectangle = new Rectangle(source_x, source_y, ddx, ddy);
                Rectangle destRectangle = new Rectangle(x, y, dx, dy);
                g.DrawImage(imageBitmap2, destRectangle, srcRectangle, GraphicsUnit.Pixel);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (imageBitmap2 != null) imageBitmap2.Dispose();
            }
        }
    }
}
