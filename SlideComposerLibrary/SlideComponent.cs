using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Petronode.SlideComposerLibrary
{
    /// <summary>
    /// Describes a slide component
    /// </summary>
    public class SlideComponent
    {
        /// <summary>
        /// Dictionary with the component properties
        /// </summary>
        public Slide ParentSlide = null; 

        /// <summary>
        /// x location of the component
        /// </summary>
        public int x = 0;

        /// <summary>
        /// y location of the component
        /// </summary>
        public int y = 0;

        /// <summary>
        /// x size of the component
        /// </summary>
        public int dx = 0;

        /// <summary>
        /// y size of the component
        /// </summary>
        public int dy = 0;

        /// <summary>
        /// Mask for component search
        /// </summary>
        public string Text = "*.*";

        /// <summary>
        /// Drawing orientation: 0, 90, 180, 270, mirror, flip
        /// </summary>
        public string Orientation = "0";

        /// <summary>
        /// Empty constructor
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="args"></param>
        protected SlideComponent(Slide Parent)
        {
            ParentSlide = Parent;
            RecoverAttributes();
        }

        /// <summary>
        /// Protected constructor; only called from the children
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="args"></param>
        protected SlideComponent( Slide Parent, string[] args)
        {
            ParentSlide = Parent;
            RecoverAttributes();
            if (args.Length > 1) x = Convert.ToInt32(args[1]);
            if (args.Length > 2) y = Convert.ToInt32(args[2]);
            if (args.Length > 3) dx = Convert.ToInt32(args[3]);
            if (dx <= 0) dx = Parent.x - this.x;
            if (args.Length > 4) dy = Convert.ToInt32(args[4]);
            if (dy <= 0) dy = Parent.y - this.y;
            if (args.Length > 5) Text = args[5];
            SaveAttributes();
        }

        /// <summary>
        /// Adds an attribute to the Component
        /// </summary>
        /// <param name="attribute">Attribute string in form Name=Something</param>
        public virtual string[] AddAttribute( string attribute)
        {
            string[] tmp = null;
            int i = attribute.IndexOf('=');
            if( i<=0)
            {
                tmp = new string[1];
                tmp[0] = attribute;
                return tmp;
            }
            tmp = new string[2];
            tmp[0] = attribute.Substring(0, i).Trim().ToLower();
            tmp[1] = attribute.Substring(i + 1);
            try
            {
                switch (tmp[0])
                {
                    case "x": x = Convert.ToInt32(tmp[1].Trim()); break;
                    case "y": y = Convert.ToInt32(tmp[1].Trim()); break;
                    case "dx": dx = Convert.ToInt32(tmp[1].Trim()); break;
                    case "dy": dy = Convert.ToInt32(tmp[1].Trim()); break;
                    case "mask": Text = tmp[1].Trim(); break;
                    case "orientation": Orientation = tmp[1].Trim(); break;
                    default: break;
                }
            }
            catch (Exception) { }
            SaveAttributes();
            return tmp;
        }

        /// <summary>
        /// Virtial Draw, defined in children
        /// </summary>
        /// <param name="source"></param>
        /// <param name="g"></param>
        public virtual void Draw(string source, Graphics g, Bitmap baseBitmap)
        {
        }

        /// <summary>
        /// Virtial Draw, defined in children
        /// </summary>
        /// <param name="source"></param>
        public virtual void Draw(string source, BitmapFile bmp, int count)
        {
            Bitmap b = null;
            Graphics g = null;
            int originalX = x; x = 0; // paste is done to the zero coordinates
            int originalY = y; y = 0;
            try
            {
                b = bmp.GetBitmap(originalX, originalY, dx+1, dy+1);
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
                if (g != null) g.Dispose();
                if (b != null) b.Dispose();
            }
        }

        /// <summary>
        /// Recovers attributes from the previous component
        /// </summary>
        protected virtual void RecoverAttributes()
        {
            try
            {
                this.x = Convert.ToInt32(ParentSlide.DefaultParameters["x"]);
                this.y = Convert.ToInt32(ParentSlide.DefaultParameters["y"]);
                this.dx = Convert.ToInt32(ParentSlide.DefaultParameters["dx"]);
                this.dy = Convert.ToInt32(ParentSlide.DefaultParameters["dy"]);
                this.Text = ParentSlide.DefaultParameters["mask"];
                this.Orientation = ParentSlide.DefaultParameters["orientation"];
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Saves attributes from the previous component
        /// </summary>
        protected virtual void SaveAttributes()
        {
            try
            {
                ParentSlide.DefaultParameters["x"] = x.ToString();
                ParentSlide.DefaultParameters["y"] = y.ToString();
                ParentSlide.DefaultParameters["dx"] = dx.ToString();
                ParentSlide.DefaultParameters["dy"] = dy.ToString();
                ParentSlide.DefaultParameters["mask"] = Text;
                ParentSlide.DefaultParameters["orientation"] = Orientation;
            }
            catch (Exception) { }
        }
    }
}
