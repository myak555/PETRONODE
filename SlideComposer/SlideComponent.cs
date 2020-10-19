using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Petronode.SlideComposer
{
    /// <summary>
    /// Describes a slide component
    /// </summary>
    public class SlideComponent
    {
        /// <summary>
        /// Dictionary with the component properties
        /// </summary>
        //public Dictionary<string, ComponentProperty> Properties = new Dictionary<string,ComponentProperty>(); 

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
        /// Protected constructor; only called from the children
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="args"></param>
        protected SlideComponent( Slide Parent, string[] args)
        {
            //Properties.Add( "x", new ComponentProperty( "x", "0", 0));

            x = (args.Length > 1) ? Convert.ToInt32(args[1]) : 0;
            y = (args.Length > 2) ? Convert.ToInt32(args[2]) : 0;
            dx = (args.Length > 3) ? Convert.ToInt32(args[3]) : Parent.x - x;
            if (dx < 0) dx = Parent.x - x;
            dy = (args.Length > 4) ? Convert.ToInt32(args[4]) : Parent.y - this.y;
            if (dy < 0) dy = Parent.y - this.y;
            Text = (args.Length > 5) ? args[5] : "*.*";
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
                    default: break;
                }
            }
            catch (Exception) { }
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
    }
}
