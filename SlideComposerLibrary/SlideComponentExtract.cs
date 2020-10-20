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
    public class SlideComponentExtract: SlideComponent
    {
        /// <summary>
        /// Constructor, creates the image description
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="args"></param>
        public SlideComponentExtract(Slide Parent, string[] args)
            : base(Parent, args) 
        {
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
                    case "filename": Text = attrib[1].Trim(); break;
                    default: break;
                }
            }
            catch (Exception) { }
            return attrib;
        }

        /// <summary>
        /// Draws the image
        /// </summary>
        /// <param name="source"></param>
        /// <param name="g"></param>
        public override void Draw(string source, Graphics g, Bitmap baseBitmap)
        {
            Bitmap imageBitmap = null;
            Graphics tmpG = null;
            try
            {
                imageBitmap = new Bitmap(dx, dy);
                tmpG = Graphics.FromImage(imageBitmap);
                Rectangle srcRect = new Rectangle( x, y, dx, dy);
                Rectangle dstRect = new Rectangle( 0, 0, dx, dy);
                tmpG.DrawImage(baseBitmap, dstRect, srcRect, GraphicsUnit.Pixel);
                DirectoryInfo di = new DirectoryInfo(source);
                if (!di.Exists) throw new Exception(source + " not found");
                string saveFile = di.FullName + "\\" + this.Text;
                imageBitmap.Save(saveFile);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (imageBitmap != null) imageBitmap.Dispose();
                if (tmpG != null) tmpG.Dispose();
            }
        }
    }
}
