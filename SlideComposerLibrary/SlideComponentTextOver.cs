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
    public class SlideComponentTextOver : SlideComponentText
    {
        /// <summary>
        /// Constructor, creates the image description
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="args"></param>
        public SlideComponentTextOver(Slide Parent, string[] args)
            : base(Parent, args) 
        {
        }

        /// <summary>
        /// Draws the text
        /// </summary>
        /// <param name="source"></param>
        /// <param name="g"></param>
        public override void Draw(string source, Graphics g, Bitmap baseBitmap)
        {
            Color b = Color.Transparent;
            List<string> t = GetFile(source);
            DrawText(t, g, baseBitmap, b);
        }
    }
}
