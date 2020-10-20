using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Petronode.OilfieldFileAccess;
using Petronode.OilfieldFileAccess.CSV;
using Petronode.OilfieldFileAccess.LAS;

namespace Petronode.SlideComposerLibrary
{
    /// <summary>
    /// Describes the Image component of the slide
    /// </summary>
    public class SlideComponentDot : SlideComponent
    {
        /// <summary>
        /// name of the X-variable (horizontal scale)
        /// </summary>
        public string Name_x = "1";
        public int Order_x = 1;

        /// <summary>
        /// name of the Y-variable (vertical scale)
        /// </summary>
        public string Name_y = "0";
        public int Order_y = 0;

        /// <summary>
        /// type of marker
        /// </summary>
        public string DotType = "dot";

        /// <summary>
        /// Line thickness
        /// </summary>
        public float LineSize = 1f;

        /// <summary>
        /// Front color
        /// </summary>
        public string FrontColor = "000000";

        /// <summary>
        /// x location of the component
        /// </summary>
        public float Scale_X0 = 0f;

        /// <summary>
        /// y location of the component
        /// </summary>
        public float Scale_Y0 = 0f;

        /// <summary>
        /// x size of the component
        /// </summary>
        public float Scale_X1 = 1f;

        /// <summary>
        /// y size of the component
        /// </summary>
        public float Scale_Y1 = 1f;
        public float Scale_dx = 1f;
        public float Scale_dy = 1f; 

        /// <summary>
        /// type of transform
        /// </summary>
        public string Transform_X = "linear";
        public string Transform_Y = "linear";

        /// <summary>
        /// Last log file (to avoid multiple initializations)
        /// </summary>
        public static Oilfield_File LastLog = null;

        /// <summary>
        /// Constructor, creates the log image
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="args"></param>
        public SlideComponentDot(Slide Parent, string[] args)
            : base(Parent, args) 
        {
            if(args.Length > 6) Name_y = args[6].Trim();
            if(args.Length > 7) Name_x = args[7].Trim();
            if (args.Length > 8) DotType = args[8].Trim().ToLower();
            if (args.Length > 9) LineSize = Convert.ToSingle(args[9].Trim());
            if (args.Length > 10) FrontColor = args[10].Trim();
            if (args.Length > 11) Scale_X0 = Convert.ToSingle(args[11].Trim());
            if (args.Length > 12) Scale_Y0 = Convert.ToSingle(args[12].Trim());
            if (args.Length > 13) Scale_X1 = Convert.ToSingle(args[13].Trim());
            if (args.Length > 14) Scale_Y1 = Convert.ToSingle(args[14].Trim());
            CheckAttributes();
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
                    case "name_x": Name_x = attrib[1].Trim(); break;
                    case "name_y": Name_y = attrib[1].Trim(); break;
                    case "lsize": LineSize = Convert.ToInt32(attrib[1].Trim()); break;
                    case "linesize": LineSize = Convert.ToInt32(attrib[1].Trim()); break;
                    case "fcolor": FrontColor = attrib[1].Trim().ToUpper(); break;
                    case "frontcolor": FrontColor = attrib[1].Trim().ToUpper(); break;
                    case "scale_x0": Scale_X0 = Convert.ToSingle(attrib[1].Trim()); break;
                    case "scale_y0": Scale_Y0 = Convert.ToSingle(attrib[1].Trim()); break;
                    case "scale_x1": Scale_X1 = Convert.ToSingle(attrib[1].Trim()); break;
                    case "scale_y1": Scale_Y1 = Convert.ToSingle(attrib[1].Trim()); break;
                    case "dtype": DotType = attrib[1].Trim(); break;
                    case "transform_x": Transform_X = attrib[1].Trim(); break;
                    case "transform_y": Transform_Y = attrib[1].Trim(); break;
                    default: break;
                }
            }
            catch (Exception) { }
            CheckAttributes();
            SaveAttributes();
            return attrib;
        }

        private void CheckAttributes()
        {
            try { Order_x = Convert.ToInt32(Name_x); }
            catch { Order_x = 0; }

            try { Order_y = Convert.ToInt32(Name_y); }
            catch { Order_y = 1; }

            Scale_dx = Scale_X1 - Scale_X0;
            Scale_dy = Scale_Y1 - Scale_Y0;

            switch (this.DotType)
            {
                case "dot":
                case "circle":
                case "cross":
                case "xcross":
                case "square":
                case "diamond":
                case "tri_up":
                case "tri_down":
                case "tri_left":
                case "tri_right":
                    break;
                default:
                    this.DotType = "dot";
                    break;
            }
            switch (this.Transform_X)
            {
                case "linear":
                case "lg":
                case "ln":
                case "exp":
                case "power10":
                case "inverse":
                case "square":
                    break;
                default:
                    this.DotType = "linear";
                    break;
            }
            switch (this.Transform_Y)
            {
                case "linear":
                case "lg":
                case "ln":
                case "exp":
                case "power10":
                case "inverse":
                case "square":
                    break;
                default:
                    this.DotType = "linear";
                    break;
            }
        }

        /// <summary>
        /// Draws the log line
        /// </summary>
        /// <param name="source"></param>
        /// <param name="g"></param>
        /// <summary>
        public override void Draw(string source, Graphics g, Bitmap baseBitmap)
        {
            Oilfield_Channel chX = null;
            Oilfield_Channel chY = null;
            try
            {
                GetDataFile(source);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            foreach (Oilfield_Channel oc in LastLog.Channels)
            {
                if (oc.Name == Name_x) chX = oc;
                if (oc.Name == Name_y) chY = oc;
            }
            if (chX == null && LastLog.Channels.Count > Order_x)
                chX = LastLog.Channels[Order_x];
            if (chY == null && LastLog.Channels.Count > Order_y)
                chY = LastLog.Channels[Order_y];
            if (chX == null)
                throw new Exception("File " + Text + " has no " + Name_x);
            if (chY == null)
                throw new Exception("File " + Text + " has no " + Name_y);
            try
            {
                Rectangle destRectangle = new Rectangle(x, y, dx + 1, dy + 1);
                g.SetClip(destRectangle);
                Brush myBrush = new SolidBrush( Slide.GetColor( FrontColor));
                Pen myPen = new Pen(myBrush, 1f);
                int i = 0;
                while( i < chX.Data.Count && i < chY.Data.Count)
                {
                    plotPointScaled(g, myPen, myBrush, chX.Data[i], chY.Data[i]);
                    i++;
                } // while
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

        protected void GetDataFile(string source)
        {
            DirectoryInfo di = new DirectoryInfo(source);
            if (!di.Exists) throw new Exception(source + " not found");
            FileInfo[] fis = di.GetFiles(Text);
            if (fis.Length == 0) return;
            string filename = fis[0].FullName;
            string extension = fis[0].Extension.ToLower();
            if (LastLog != null && LastLog.FileName == filename) return;
            switch (extension)
            {
                case ".las":
                    LastLog = new Petronode.OilfieldFileAccess.LAS.LAS_File(filename, true);
                    break;
                default:
                    LastLog = new Petronode.OilfieldFileAccess.CSV.CSV_File(filename, -999.25);
                    break;
            }
        }

        protected float[] GetDashStyle()
        {
            float q = LineSize;
            if (q < 2f) q = 2f;
            float gap = q * 0.25f;
            float dot = q * 0.5f;
            float dash = q * 1.25f;
            List<float> tmp = new List<float>();
            switch (this.DotType)
            {
                case "dot":
                    tmp.Add(dot);
                    tmp.Add(gap);
                    break;
                case "dash":
                    tmp.Add(dash);
                    tmp.Add(gap);
                    break;
                case "dashdot":
                    tmp.Add(dash);
                    tmp.Add(gap);
                    tmp.Add(dot);
                    tmp.Add(gap);
                    break;
                case "dashdotdot":
                    tmp.Add(dash);
                    tmp.Add(gap);
                    tmp.Add(dot);
                    tmp.Add(gap);
                    tmp.Add(dot);
                    tmp.Add(gap);
                    break;
                default:
                    tmp.Add( LineSize);
                    break;
            }
            return tmp.ToArray();
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
                this.FrontColor = ParentSlide.DefaultParameters["fcolor"];
                this.LineSize = Convert.ToSingle(ParentSlide.DefaultParameters["lsize"]);
                this.DotType = ParentSlide.DefaultParameters["dtype"];
                this.Name_x = ParentSlide.DefaultParameters["namex"];
                this.Name_y = ParentSlide.DefaultParameters["namey"];
                this.Scale_X0 = Convert.ToSingle(ParentSlide.DefaultParameters["scalex0"]);
                this.Scale_Y0 = Convert.ToSingle(ParentSlide.DefaultParameters["scaley0"]);
                this.Scale_X1 = Convert.ToSingle(ParentSlide.DefaultParameters["scalex1"]);
                this.Scale_Y1 = Convert.ToSingle(ParentSlide.DefaultParameters["scaley1"]);
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
                ParentSlide.DefaultParameters["text"] = Text;
                ParentSlide.DefaultParameters["fcolor"] = FrontColor;
                ParentSlide.DefaultParameters["lsize"] = LineSize.ToString();
                ParentSlide.DefaultParameters["dtype"] = DotType;
                ParentSlide.DefaultParameters["namex"] = Name_x;
                ParentSlide.DefaultParameters["namey"] = Name_y;
                ParentSlide.DefaultParameters["scalex0"] = Scale_X0.ToString();
                ParentSlide.DefaultParameters["scaley0"] = Scale_Y0.ToString();
                ParentSlide.DefaultParameters["scalex1"] = Scale_X1.ToString();
                ParentSlide.DefaultParameters["scaley1"] = Scale_Y1.ToString();
            }
            catch (Exception) { }
        }

        private void plotPointScaled(Graphics g, Pen pen, Brush brush, double xV, double yV)
        {
            xV = transformData(xV, Transform_X);
            yV = transformData(yV, Transform_Y);
            if (double.IsNaN(xV) || double.IsNaN(yV)) return;
            float xf = (Convert.ToSingle(xV) - Scale_X0) * this.dx / Scale_dx + this.x;
            float yf = (Convert.ToSingle(yV) - Scale_Y0) * this.dy / Scale_dy + this.y;
            float half_size = this.LineSize * 0.5f;
            List<PointF> plg = new List<PointF>();
            switch (this.DotType)
            {
                case "dot":
                    g.DrawEllipse(pen, xf - half_size, yf - half_size, LineSize, LineSize);
                    break;
                case "circle":
                    g.FillEllipse( brush, xf - half_size, yf - half_size, LineSize, LineSize);
                    break;
                case "cross":
                    g.DrawLine(pen, xf, yf - half_size, xf, yf + half_size);
                    g.DrawLine(pen, xf - half_size, yf, xf + half_size, yf);
                    break;
                case "xcross":
                    g.DrawLine(pen, xf - half_size, yf - half_size, xf + half_size, yf + half_size);
                    g.DrawLine(pen, xf + half_size, yf - half_size, xf - half_size, yf + half_size);
                    break;
                case "square":
                    g.FillRectangle(brush, xf - half_size, yf - half_size, LineSize, LineSize);
                    break;
                case "diamond":
                    plg.Add(new PointF(xf, yf - half_size));
                    plg.Add(new PointF(xf + half_size, yf));
                    plg.Add(new PointF(xf, yf + half_size));
                    plg.Add(new PointF(xf - half_size, yf));
                    g.FillPolygon(brush, plg.ToArray());
                    break;
                case "tri_up":
                    plg.Add(new PointF(xf, yf - half_size));
                    plg.Add(new PointF(xf + half_size, yf + half_size));
                    plg.Add(new PointF(xf - half_size, yf + half_size));
                    g.FillPolygon(brush, plg.ToArray());
                    break;
                case "tri_down":
                    plg.Add(new PointF(xf, yf + half_size));
                    plg.Add(new PointF(xf - half_size, yf - half_size));
                    plg.Add(new PointF(xf + half_size, yf - half_size));
                    g.FillPolygon(brush, plg.ToArray());
                    break;
                case "tri_left":
                    plg.Add(new PointF(xf - half_size, yf));
                    plg.Add(new PointF(xf + half_size, yf - half_size));
                    plg.Add(new PointF(xf + half_size, yf + half_size));
                    g.FillPolygon(brush, plg.ToArray());
                    break;
                case "tri_right":
                    plg.Add(new PointF(xf + half_size, yf));
                    plg.Add(new PointF(xf - half_size, yf - half_size));
                    plg.Add(new PointF(xf - half_size, yf + half_size));
                    g.FillPolygon(brush, plg.ToArray());
                    break;
                default:
                    g.DrawEllipse(pen, xf - half_size, yf - half_size, LineSize, LineSize);
                    break;
            }
        }

        private double transformData (double v, string transform)
        {
            switch (transform)
            {
                case "linear":
                    break;
                case "lg":
                    if (v <= 0.0) return double.NaN;
                    return Math.Log10(v);
                case "ln":
                    if (v <= 0.0) return double.NaN;
                    return Math.Log(v);
                case "exp":
                    if ( v < -700.0 || 700.0 < v) return double.NaN;
                    return Math.Exp(v);
                case "power10":
                    if (v < -308.0 || 308.0 < v) return double.NaN;
                    return Math.Pow(10.0, v);
                case "inverse":
                    if ( Math.Abs(v) < 1e-308) return double.NaN;
                    return 1.0 / v;
                case "square":
                    if (Math.Abs(v) > 1e154) return double.NaN;
                    return v * v;
                default:
                    break;
            }
            return v;
        }
    }
}
