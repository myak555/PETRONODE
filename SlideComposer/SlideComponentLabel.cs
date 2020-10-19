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
    public class SlideComponentLabel : SlideComponent
    {
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
        /// Constructor, creates the image description
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="args"></param>
        public SlideComponentLabel(Slide Parent, string[] args)
            : base(Parent, args) 
        {
            this.Text = this.Text.Replace("&comma", ",");
            this.Text = this.Text.Replace("&space", " ");
            this.Text = this.Text.Replace("&left", "(");
            this.Text = this.Text.Replace("&right", ")");
            FontSize = (args.Length > 6) ? Convert.ToSingle(args[6]) : 60f;
            FontColor = (args.Length > 7) ? args[7] : "000000";
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
                    default: break;
                }
            }
            catch (Exception) { }
            return attrib;
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
                g.Clear(Slide.GetColor(BackColor));
                Brush myBrush = new SolidBrush( Slide.GetColor( FontColor));
                Font myFont = GetFont();
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
    }
}
