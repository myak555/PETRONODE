using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Petronode.SlideComposer
{
    /// <summary>
    /// Provides interface to slide description language
    /// </summary>
    public class SlideDescriptionFile
    {
        char[] c_Splitter = { ' ', ',', '(', ')' };

        /// <summary>
        /// Slides description
        /// </summary>
        public List<Slide> Slides = new List<Slide>();

        /// <summary>
        /// Reads the file and composes the task
        /// </summary>
        /// <param name="definition">file with definitions</param>
        public SlideDescriptionFile(string definition)
        {
            // get definition from file
            FileStream fs = null;
            StreamReader sr = null;
            int line = 0;
            try
            {
                if (!File.Exists(definition)) throw new Exception(definition + " not found");
                fs = File.Open(definition, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                sr = new StreamReader(fs);
                Slide LastSlide = null;
                while (true)
                {
                    line++;
                    string s = sr.ReadLine();
                    if (s == null) break;
                    if (s.StartsWith("#")) continue;
                    if (s.Length == 0) continue;
                    string[] ss = s.Split(c_Splitter, StringSplitOptions.RemoveEmptyEntries);
                    if (ss.Length <= 0) continue;
                    ss[0] = ss[0].ToLower();
                    if (ss[0].StartsWith( "slide"))
                    {
                        LastSlide = new Slide(ss);
                        Slides.Add(LastSlide);
                        continue;
                    }
                    if (LastSlide == null) continue;
                    if (ss[0] == "image")
                    {
                        LastSlide.Components.Add(new SlideComponentImage(LastSlide, ss));
                        continue;
                    }
                    if (ss[0] == "imageover")
                    {
                        LastSlide.Components.Add(new SlideComponentImageOver(LastSlide, ss));
                        continue;
                    }
                    if (ss[0] == "text")
                    {
                        LastSlide.Components.Add(new SlideComponentText(LastSlide, ss));
                        continue;
                    }
                    if (ss[0] == "textover")
                    {
                        LastSlide.Components.Add(new SlideComponentTextOver(LastSlide, ss));
                        continue;
                    }
                    if (ss[0] == "label")
                    {
                        LastSlide.Components.Add(new SlideComponentLabel(LastSlide, ss));
                        continue;
                    }
                    if (ss[0] == "labelover")
                    {
                        LastSlide.Components.Add(new SlideComponentLabelOver(LastSlide, ss));
                        continue;
                    }
                    if (ss[0] == "shape")
                    {
                        LastSlide.Components.Add(new SlideComponentShape(LastSlide, ss));
                        continue;
                    }
                    if (ss[0] == "shapeover")
                    {
                        LastSlide.Components.Add(new SlideComponentShapeOver(LastSlide, ss));
                        continue;
                    }
                    if (ss[0] == "recolor")
                    {
                        LastSlide.Components.Add(new SlideComponentRecolor(LastSlide, ss));
                        continue;
                    }
                    if (s.StartsWith(" ") || s.StartsWith("\t") )
                    {
                        if (LastSlide.Components.Count <= 0) LastSlide.AddAttribute(s);
                        else LastSlide.Components[LastSlide.Components.Count - 1].AddAttribute(s);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception( "Line " + line.ToString( "000") + ":" + ex.Message);
            }
            finally
            {
                if (sr != null) sr.Close();
                if (fs != null) fs.Close();
            }
        }

        /// <summary>
        /// Composes all slides
        /// </summary>
        /// <param name="sourceDir">Source directory</param>
        /// <param name="destDir">Destination directory</param>
        public void ComposeSlides( string sourceDir, string destDir)
        {
            sourceDir = ConvertCurrentDirectory( sourceDir);
            destDir = ConvertCurrentDirectory( destDir);
            DirectoryInfo di = new DirectoryInfo( destDir);
            if( !di.Exists) di.Create();
            foreach( Slide s in Slides)
            {
                CompositeBitmap cb = new CompositeBitmap(s, sourceDir);
                cb.PDF_Width = s.pageX;
                cb.PDF_Height = s.pageY;
                cb.PDF_DPI = s.pageDPI;
                string[] formats = s.Format.Split('+');
                foreach (string f in formats)
                {
                    string savename = di.FullName + "\\" + s.Prefix + "_" + di.Name + "." + f.Trim();
                    cb.Save(savename);
                }
                cb.Dispose();
            }
        }

        private string ConvertCurrentDirectory(string dir)
        {
            if( dir == ".") return Directory.GetCurrentDirectory();
            int j = dir.IndexOf( ".\\");
            if( j < 0) return dir;
            return dir.Replace( ".\\", Directory.GetCurrentDirectory() + "\\");
        }
    }
}
