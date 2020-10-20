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
    public class SlideComponentLabel : SlideComponent
    {
        /// <summary>
        /// Line size
        /// </summary>
        public float LineSize = 0f;

        /// <summary>
        /// Font type
        /// </summary>
        public string FontType = "System SansSerif";

        /// <summary>
        /// Font size
        /// </summary>
        public float FontSize = 60f;

        /// <summary>
        /// Font Style
        /// </summary>
        public string Style = "normal";

        /// <summary>
        /// Font bColor
        /// </summary>
        public string FontColor = "000000";

        /// <summary>
        /// Font bColor
        /// </summary>
        public string BackColor = "FFFFFF";

        /// <summary>
        /// Text box location
        /// </summary>
        public int Text_x = 0;

        /// <summary>
        /// Text box location
        /// </summary>
        public int Text_y = 0;

        /// <summary>
        /// Text box size
        /// </summary>
        public int Text_dx = 0;

        /// <summary>
        /// Text box size
        /// </summary>
        public int Text_dy = 0;

        /// <summary>
        /// Callout location
        /// </summary>
        public int Callout_x = -1;

        /// <summary>
        /// Callout location
        /// </summary>
        public int Callout_y = -1;

        /// <summary>
        /// Callout ellipse radius
        /// </summary>
        public int Callout_ellipse = 1;

        /// <summary>
        /// Corner from which to plot call-out (0-top left, CW)
        /// </summary>
        public int Callout_index = 0;

        /// <summary>
        /// Text Position: left, right, center, justified
        /// </summary>
        public string TextPosition = "left";

        /// <summary>
        /// Constructor, creates the image description
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="args"></param>
        public SlideComponentLabel(Slide Parent, string[] args)
            : base( Parent)
        {
            if (args.Length > 1) x = Convert.ToInt32(args[1]);
            if (args.Length > 2) y = Convert.ToInt32(args[2]);
            if (args.Length > 3) dx = Convert.ToInt32(args[3]);
            if (dx <= 0) dx = Parent.x - this.x;
            if (args.Length > 4) dy = Convert.ToInt32(args[4]);
            if (dy <= 0) dy = Parent.y - this.y;
            if (args.Length > 5)
            {
                Text = args[5];
                Text = Text.Replace("&comma", ",");
                Text = Text.Replace("&space", " ");
                Text = Text.Replace("&left", "(");
                Text = Text.Replace("&right", ")");
            }
            if (args.Length > 6) FontSize = Convert.ToSingle(args[6]);
            if (args.Length > 7) FontColor = args[7];
            if (args.Length > 8) BackColor = args[8];
            if (args.Length > 9) LineSize = Convert.ToSingle(args[9]);
            Text_x = x;
            Text_y = y;
            Text_dx = dx;
            Text_dy = dy;
            ComputeCallout();
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
                    case "fontcolor": FontColor = attrib[1].Trim().ToUpper(); break;
                    case "fcolor": FontColor = attrib[1].Trim().ToUpper(); break;
                    case "backcolor": BackColor = attrib[1].Trim().ToUpper(); break;
                    case "bcolor": BackColor = attrib[1].Trim().ToUpper(); break;
                    case "fonttype": FontType = attrib[1].Trim(); break;
                    case "ftype": FontType = attrib[1].Trim(); break;
                    case "fontsize": FontSize = Convert.ToSingle(attrib[1].Trim()); break;
                    case "fsize": FontSize = Convert.ToSingle(attrib[1].Trim()); break;
                    case "fontstyle": Style = attrib[1].Trim().ToLower(); break;
                    case "fstyle": Style = attrib[1].Trim().ToLower(); break;
                    case "text": Text = attrib[1]; break;
                    case "lsize": LineSize = Convert.ToSingle(attrib[1].Trim()); break;
                    case "calloutx": Callout_x = Convert.ToInt32(attrib[1].Trim()); break;
                    case "callouty": Callout_y = Convert.ToInt32(attrib[1].Trim()); break;
                    case "calloutellipse": Callout_ellipse = Convert.ToInt32(attrib[1].Trim()); break;
                    case "tposition": TextPosition = attrib[1].Trim().ToLower(); break;
                    default: break;
                }
            }
            catch (Exception) { }
            ComputeCallout();
            SaveAttributes();
            return attrib;
        }

        /// <summary>
        /// Draws the text
        /// </summary>
        /// <param name="source"></param>
        /// <param name="g"></param>
        public override void Draw(string source, Graphics g, Bitmap baseBitmap)
        {
            Color b = Slide.GetColor(this.BackColor);
            List<string> t = new List<string>(1);
            t.Add(Text);
            DrawText( t, g, baseBitmap, b);
        }

        /// <summary>
        /// Draws the text
        /// </summary>
        /// <param name="g"></param>
        protected void DrawText( List<string> t, Graphics g, Bitmap baseBitmap, Color bkColor)
        {
            switch (Orientation)
            {
                case "0":
                    Draw0( t, g, baseBitmap, bkColor);
                    break;
                case "90":
                    Draw90( t, g, baseBitmap, bkColor);
                    break;
                case "180":
                    Draw180( t, g, baseBitmap, bkColor);
                    break;
                case "270":
                    Draw270( t, g, baseBitmap, bkColor);
                    break;
                case "mirror":
                    DrawMirror( t, g, baseBitmap, bkColor);
                    break;
                case "flip":
                    DrawFlip( t, g, baseBitmap, bkColor);
                    break;
                default:
                    Draw0( t, g, baseBitmap, bkColor);
                    break;
            }
        }

        /// <summary>
        /// Virtial Draw, defined in children
        /// </summary>
        /// <param name="source"></param>
        public override void Draw(string source, BitmapFile bmp, int count)
        {
            Bitmap b = null;
            Graphics g = null;
            int originalX = x; x = 0; // paste is done to the zero coordinates
            int originalY = y; y = 0;
            int originalTextX = Text_x; Text_x -= originalX;
            int originalTextY = Text_y; Text_y -= originalY;
            int originalCalloutX = Callout_x; Callout_x -= originalX;
            int originalCalloutY = Callout_y; Callout_y -= originalY;
            try
            {
                b = bmp.GetBitmap(originalX, originalY, dx + 1, dy + 1);
                g = Graphics.FromImage(b);
                Draw(source, g, b);
                if (count >= 0)
                {
                    b.Save("C:\\temp\\_debug_" + count.ToString("000") + ".jpg", ImageFormat.Jpeg);
                }
                bmp.SetBitmap(b, originalX, originalY);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Draw routine: " + ex.Message);
            }
            finally
            {
                x = originalX;
                y = originalY;
                Text_x = originalTextX;
                Text_y = originalTextY;
                Callout_x = originalCalloutX;
                Callout_y = originalCalloutY;
                if (g != null) g.Dispose();
                if (b != null) b.Dispose();
            }
        }
        
        protected Font GetFont()
        {
            FontFamily family = FontFamily.GenericSansSerif;
            switch (FontType)
            {
                case "System Serif": family = FontFamily.GenericSerif; break;
                case "System Monospace": family = FontFamily.GenericMonospace; break;
                default: break;
            }
            for (int i = 0; i < FontFamily.Families.Length; i++)
            {
                if (FontType != FontFamily.Families[i].Name) continue;
                family = FontFamily.Families[i];
                break;
            }
            FontStyle style = FontStyle.Regular;
            switch (Style)
            {
                case "bold": style = FontStyle.Bold; break;
                case "italic": style = FontStyle.Italic; break;
                case "strikeout": style = FontStyle.Strikeout; break;
                case "underline": style = FontStyle.Underline; break;
                default: break;
            }
            return new Font(family, FontSize, style);
        }

        protected void SetBorderAndCallout( Graphics g)
        {
            if (LineSize <= 0f) return;
            Pen p = new Pen(Slide.GetColor(FontColor), LineSize);
            float half = LineSize * 0.5f;
            g.DrawRectangle(p, (float)Text_x + half, (float)Text_y + half, (float)Text_dx - LineSize, (float)Text_dy - LineSize);
            if (Callout_x < 0 || Callout_y < 0)
            {
                p.Dispose();
                return;
            }
            if ((float)Callout_ellipse >= LineSize)
            {
                float xEllipse = (float)(Callout_x - Callout_ellipse) + half;
                float yEllipse = (float)(Callout_y - Callout_ellipse) + half;
                float dEllipse = Convert.ToSingle(Callout_ellipse << 1) - LineSize;
                g.DrawEllipse(p, xEllipse, yEllipse, dEllipse, dEllipse);
            }
            switch (Callout_index)
            {
                case 0:
                    g.DrawLine(p, (float)Callout_x, (float)Callout_y, half + (float)Text_x, half + (float)Text_y);
                    break;
                case 1:
                    g.DrawLine(p, (float)Callout_x, (float)Callout_y, -half + (float)Text_x + (float)Text_dx, half + (float)Text_y);
                    break;
                case 2:
                    g.DrawLine(p, (float)Callout_x, (float)Callout_y, -half + (float)Text_x + (float)Text_dx, -half + (float)Text_y + (float)Text_dy);
                    break;
                case 3:
                    g.DrawLine(p, (float)Callout_x, (float)Callout_y, half + (float)Text_x, -half + (float)Text_y + (float)Text_dy);
                    break;
                default:
                    break;
            }
            p.Dispose();
        }

        protected void ComputeCallout()
        {
            if (Callout_x < 0 || Callout_y < 0)
            {
                x = Text_x;
                y = Text_y;
                dx = Text_dx;
                dy = Text_dy;
                return;
            }
            
            //decide the corner
            double[] corners = new double[4];
            corners[0] = ComputeDistance(Text_x, Text_y, Callout_x, Callout_y);
            corners[1] = ComputeDistance(Text_x + Text_dx, Text_y, Callout_x, Callout_y);
            corners[2] = ComputeDistance(Text_x + Text_dx, Text_y + Text_dy, Callout_x, Callout_y);
            corners[3] = ComputeDistance(Text_x, Text_y + Text_dy, Callout_x, Callout_y);
            Callout_index = 0;
            double minimum = corners[0];
            for (int i = 1; i < 4; i++)
            {
                if (corners[i] > minimum) continue;
                Callout_index = i;
                minimum = corners[i];
            }
            SetCalloutX();
            SetCalloutY();
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
                this.Text = ParentSlide.DefaultParameters["text"];
                this.FontColor = ParentSlide.DefaultParameters["fcolor"];
                this.BackColor = ParentSlide.DefaultParameters["bcolor"];
                this.FontType = ParentSlide.DefaultParameters["ftype"];
                this.FontSize = Convert.ToSingle( ParentSlide.DefaultParameters["fsize"]);
                this.Style = ParentSlide.DefaultParameters["fstyle"];
                this.LineSize = Convert.ToSingle( ParentSlide.DefaultParameters["lsize"]);
                this.Callout_x = Convert.ToInt32( ParentSlide.DefaultParameters["calloutx"]);
                this.Callout_y = Convert.ToInt32( ParentSlide.DefaultParameters["callouty"]);
                this.Callout_ellipse = Convert.ToInt32(ParentSlide.DefaultParameters["calloutellipse"]);
                this.Orientation = ParentSlide.DefaultParameters["orientation"];
                this.TextPosition = ParentSlide.DefaultParameters["tposition"];
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
                ParentSlide.DefaultParameters["x"] = Text_x.ToString();
                ParentSlide.DefaultParameters["y"] = Text_y.ToString();
                ParentSlide.DefaultParameters["dx"] = Text_dx.ToString();
                ParentSlide.DefaultParameters["dy"] = Text_dy.ToString();
                ParentSlide.DefaultParameters["text"] = Text;
                ParentSlide.DefaultParameters["fcolor"] = FontColor;
                ParentSlide.DefaultParameters["bcolor"] = BackColor;
                ParentSlide.DefaultParameters["ftype"] = FontType;
                ParentSlide.DefaultParameters["fsize"] = FontSize.ToString();
                ParentSlide.DefaultParameters["fstyle"] = Style;
                ParentSlide.DefaultParameters["lsize"] = LineSize.ToString();
                ParentSlide.DefaultParameters["calloutx"] = Callout_x.ToString();
                ParentSlide.DefaultParameters["callouty"] = Callout_y.ToString();
                ParentSlide.DefaultParameters["calloutellipse"] = Callout_ellipse.ToString();
                ParentSlide.DefaultParameters["orientation"] = Orientation;
                ParentSlide.DefaultParameters["tposition"] = TextPosition;
            }
            catch (Exception) { }
        }

        #region Private Methods
        private double ComputeDistance(int x, int y, int cx, int cy)
        {
            double dx = Convert.ToDouble(x - cx);
            double dy = Convert.ToDouble(y - cy);
            double R2 = dx * dx + dy * dy;
            return Math.Sqrt(R2);
        }

        private void SetCalloutX()
        {
            if ( Callout_x - Callout_ellipse < Text_x)
            {
                x = Callout_x - Callout_ellipse;
                dx = Text_dx + Text_x - x;
                return;
            }
            if (Callout_x + Callout_ellipse > Text_x + Text_dx)
            {
                x = Text_x;
                dx = Callout_x + Callout_ellipse - x;
                return;
            }
            x = Text_x;
            dx = Text_dx;
        }

        private void SetCalloutY()
        {
            if (Callout_y - Callout_ellipse < Text_y)
            {
                y = Callout_y - Callout_ellipse;
                dy = Text_dy + Text_y - y;
                return;
            }
            if (Callout_y + Callout_ellipse > Text_y + Text_dy)
            {
                y = Text_y;
                dy = Callout_y + Callout_ellipse - y;
                return;
            }
            y = Text_y;
            dy = Text_dy;
        }

        private void Draw0( List<string> t, Graphics g, Bitmap baseBitmap, Color bkColor)
        {
            Bitmap imageBitmap = null;
            try
            {
                imageBitmap = DrawTextBitmap(t, Text_dx, Text_dy, bkColor);
                Rectangle destRectangle = new Rectangle(x, y, dx, dy);
                g.SetClip(destRectangle);
                g.DrawImage(imageBitmap, Text_x, Text_y);
                SetBorderAndCallout(g);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (imageBitmap != null) imageBitmap.Dispose();
                if (g != null) g.ResetClip();
            }
        }

        private void Draw90( List<string> t, Graphics g, Bitmap baseBitmap, Color bkColor)
        {
            Bitmap imageBitmap = null;
            Bitmap imageBitmap2 = null;
            try
            {
                imageBitmap = DrawTextBitmap(t, Text_dy, Text_dx, bkColor);
                imageBitmap2 = new Bitmap(Text_dx, Text_dy);
                for (int i = 0; i < imageBitmap.Width; i++)
                {
                    for (int j = 0; j < imageBitmap.Height; j++)
                    {
                        Color cc = imageBitmap.GetPixel(i, j);
                        imageBitmap2.SetPixel(imageBitmap2.Width - j - 1, i, cc);
                    }
                }
                Rectangle destRectangle = new Rectangle(x, y, dx, dy);
                g.SetClip(destRectangle);
                g.DrawImage(imageBitmap2, Text_x, Text_y);
                SetBorderAndCallout(g);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (imageBitmap != null) imageBitmap.Dispose();
                if (imageBitmap2 != null) imageBitmap2.Dispose();
                if (g != null) g.ResetClip();
            }
        }

        private void Draw180( List<string> t, Graphics g, Bitmap baseBitmap, Color bkColor)
        {
            Bitmap imageBitmap = null;
            Bitmap imageBitmap2 = null;
            try
            {
                imageBitmap = DrawTextBitmap(t, Text_dx, Text_dy, bkColor);
                imageBitmap2 = new Bitmap(Text_dx, Text_dy);
                for (int i = 0; i < imageBitmap.Width; i++)
                {
                    for (int j = 0; j < imageBitmap.Height; j++)
                    {
                        Color cc = imageBitmap.GetPixel(i, j);
                        imageBitmap2.SetPixel(imageBitmap2.Width - i - 1, imageBitmap2.Height - j - 1, cc);
                    }
                }
                Rectangle destRectangle = new Rectangle(x, y, dx, dy);
                g.SetClip(destRectangle);
                g.DrawImage(imageBitmap2, Text_x, Text_y);
                SetBorderAndCallout(g);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (imageBitmap != null) imageBitmap.Dispose();
                if (imageBitmap2 != null) imageBitmap2.Dispose();
                if (g != null) g.ResetClip();
            }
        }

        private void Draw270( List<string> t, Graphics g, Bitmap baseBitmap, Color bkColor)
        {
            Bitmap imageBitmap = null;
            Bitmap imageBitmap2 = null;
            try
            {
                imageBitmap = DrawTextBitmap(t, Text_dy, Text_dx, bkColor);
                imageBitmap2 = new Bitmap(Text_dx, Text_dy);
                for (int i = 0; i < imageBitmap.Width; i++)
                {
                    for (int j = 0; j < imageBitmap.Height; j++)
                    {
                        Color cc = imageBitmap.GetPixel(i, j);
                        imageBitmap2.SetPixel(j, imageBitmap2.Height - i - 1, cc);
                    }
                }
                Rectangle destRectangle = new Rectangle(x, y, dx, dy);
                g.SetClip(destRectangle);
                g.DrawImage(imageBitmap2, Text_x, Text_y);
                SetBorderAndCallout(g);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (imageBitmap != null) imageBitmap.Dispose();
                if (imageBitmap2 != null) imageBitmap2.Dispose();
                if (g != null) g.ResetClip();
            }
        }

        private void DrawMirror( List<string> t, Graphics g, Bitmap baseBitmap, Color bkColor)
        {
            Bitmap imageBitmap = null;
            Bitmap imageBitmap2 = null;
            try
            {
                imageBitmap = DrawTextBitmap(t, Text_dx, Text_dy, bkColor);
                imageBitmap2 = new Bitmap(Text_dx, Text_dy);
                for (int i = 0; i < imageBitmap.Width; i++)
                {
                    for (int j = 0; j < imageBitmap.Height; j++)
                    {
                        Color cc = imageBitmap.GetPixel(i, j);
                        imageBitmap2.SetPixel(imageBitmap2.Width - i - 1, j, cc);
                    }
                }
                Rectangle destRectangle = new Rectangle(x, y, dx, dy);
                g.SetClip(destRectangle);
                g.DrawImage(imageBitmap2, Text_x, Text_y);
                SetBorderAndCallout(g);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (imageBitmap != null) imageBitmap.Dispose();
                if (imageBitmap2 != null) imageBitmap2.Dispose();
                if (g != null) g.ResetClip();
            }
        }

        private void DrawFlip( List<string> t, Graphics g, Bitmap baseBitmap, Color bkColor)
        {
            Bitmap imageBitmap = null;
            Bitmap imageBitmap2 = null;
            try
            {
                imageBitmap = DrawTextBitmap(t, Text_dx, Text_dy, bkColor);
                imageBitmap2 = new Bitmap(Text_dx, Text_dy);
                for (int i = 0; i < imageBitmap.Width; i++)
                {
                    for (int j = 0; j < imageBitmap.Height; j++)
                    {
                        Color cc = imageBitmap.GetPixel(i, j);
                        imageBitmap2.SetPixel(i, imageBitmap2.Height - j - 1, cc);
                    }
                }
                Rectangle destRectangle = new Rectangle(x, y, dx, dy);
                g.SetClip(destRectangle);
                g.DrawImage(imageBitmap2, Text_x, Text_y);
                SetBorderAndCallout(g);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (imageBitmap != null) imageBitmap.Dispose();
                if (imageBitmap2 != null) imageBitmap2.Dispose();
                if (g != null) g.ResetClip();
            }
        }

        /// <summary>
        /// Draws multiple lines of text
        /// </summary>
        /// <param name="text">Text to draw</param>
        /// <param name="x">Label size x</param>
        /// <param name="y">Label Size y</param>
        /// <param name="bkColor">Background color (set to Color.Transparent for draw over)</param>
        /// <returns>Bitmap with text</returns>
        private Bitmap DrawTextBitmap(List<string> text, int x, int y, Color bkColor)
        {
            if (text.Count == 0) return DrawLabelBitmap("", x, y, bkColor);
            if (text.Count == 1) return DrawLabelBitmap(text[0], x, y, bkColor);
            
            Bitmap img = null;
            Graphics tmpG = null;
            try
            {
                img = new Bitmap(x, y);
                tmpG = Graphics.FromImage(img);
                tmpG.Clear(bkColor);
                int step = img.Height;
                if (text.Count > 0) step /= (text.Count + 1);
                if (step <= 0) step = 1;
                int pos = step >> 1;
                foreach (string s in text)
                {
                    Bitmap bmp = DrawLabelBitmap(s, x, step, bkColor);
                    tmpG.DrawImage(bmp, 0, pos);
                    pos += step;
                    if (pos > img.Height) break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (tmpG != null) tmpG.Dispose();
            }
            return img;
        }

        /// <summary>
        /// Draws one line of text
        /// </summary>
        /// <param name="text">Text to draw</param>
        /// <param name="x">Label size x</param>
        /// <param name="y">Label Size y</param>
        /// <param name="bkColor">Background color (set to Color.Transparent for draw over)</param>
        /// <returns>Bitmap with text</returns>
        private Bitmap DrawLabelBitmap(string text, int x, int y, Color bkColor)
        {
            if (bkColor == Color.Transparent)
            {
                Color substitute = Slide.GetColor(BackColor);
                Bitmap bmp = DrawLabelBitmap( text, x, y, substitute);
                for (int iy = 0; iy < bmp.Height; iy++)
                {
                    for (int ix = 0; ix < bmp.Width; ix++)
                    {
                        Color cc = bmp.GetPixel(ix, iy);
                        if (cc != substitute) continue;
                        bmp.SetPixel(ix, iy, bkColor);
                    }
                }
                return bmp;
            }

            Bitmap img = null;
            Graphics tmpG = null;
            try
            {
                img = new Bitmap(x, y);
                tmpG = Graphics.FromImage(img);
                tmpG.Clear(bkColor);
                Color fColor = Slide.GetColor(FontColor);
                Brush myBrush = new SolidBrush(fColor);
                Font myFont = GetFont();
                tmpG.DrawString( "|" + text + "|", myFont, myBrush, 0, 0);
                int leftPosition = locateLeft( img, bkColor);
                int rightPosition = img.Width - 1; // save the run for left-justified
                switch (TextPosition)
                {
                    case "left":
                        tmpG.Clear(bkColor);
                        tmpG.DrawString(text, myFont, myBrush, leftPosition, 0);
                        break;
                    case "right":
                        rightPosition = locateRight(img, bkColor);
                        tmpG.Clear(bkColor);
                        tmpG.DrawString(text, myFont, myBrush, img.Width - rightPosition, 0);
                        break;
                    case "center":
                        rightPosition = locateRight(img, bkColor);
                        tmpG.Clear(bkColor);
                        tmpG.DrawString(text, myFont, myBrush, (img.Width - rightPosition + leftPosition) >> 1, 0);
                        break;
                    case "justified":
                        tmpG.Clear(bkColor);
                        DrawJustifiedBitmap(img, tmpG, text, myFont, myBrush, bkColor, leftPosition);
                        break;
                    default:
                        tmpG.Clear(bkColor);
                        tmpG.DrawString(text, myFont, myBrush, leftPosition, 0);
                        break;
                }
                myFont.Dispose();
                myBrush.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (tmpG != null) tmpG.Dispose();
            }
            return img;
        }

        private int locateLeft(Bitmap bmp, Color bkColor)
        {
            int retx = bmp.Width - 1;
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x <= retx; x++)
                {
                    Color cc = bmp.GetPixel(x,y);
                    int r = cc.R;
                    int g = cc.G;
                    int b = cc.B;
                    if (r == bkColor.R && g == bkColor.G && b == bkColor.B) continue;
                    retx = x;
                    break;
                }
            }
            return retx;
        }

        private int locateRight(Bitmap bmp, Color bkColor)
        {
            int retx = 0;
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = bmp.Width-1; x >= retx; x--)
                {
                    Color cc = bmp.GetPixel(x, y);
                    if (cc.R == bkColor.R && cc.G == bkColor.G && cc.B == bkColor.B) continue;
                    retx = x;
                    break;
                }
            }
            return retx;
        }

        private void DrawJustifiedBitmap(Bitmap bmp, Graphics tmpG, string text, Font myFont, Brush myBrush,
            Color bkColor, int leftPosition)
        {
            char[] separator = { ' ' };
            string[] ss = text.Split(separator);
            if (ss.Length <= 1)
            {
                tmpG.DrawString(text, myFont, myBrush, leftPosition, 0);
                return;
            }
            Pen p = new Pen(myBrush);
            string testString = text.Replace(" ", "") + "|";
            tmpG.DrawString(testString, myFont, myBrush, leftPosition, 0);
            int rp = locateRight(bmp, bkColor);
            int spacestep = (bmp.Width-rp-leftPosition) / (ss.Length - 1);
            if (spacestep < 1) spacestep = 1;
            tmpG.Clear(bkColor);
            int pos = leftPosition;
            for (int i = 0; i < ss.Length; i++)
            {
                tmpG.DrawString( ss[i], myFont, myBrush, pos, 0);
                pos = locateRight(bmp, bkColor);
                //tmpG.DrawLine(p, pos, 15, pos + spacestep, 15); // for checking
                pos += spacestep;
                if (pos >= bmp.Width) break;
            }
            return;
        }
        #endregion
    }
}
