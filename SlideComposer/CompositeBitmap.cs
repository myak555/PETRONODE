using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Petronode.SimplePdf;

namespace Petronode.SlideComposer
{
    public class CompositeBitmap
    {
        DirectoryInfo m_Input = null;
        double m_PDF_Width = 21.59;
        double m_PDF_Height = 27.94;
        double m_PDF_DPI = 250.0;

        /// <summary>
        /// Base Bitmap class
        /// </summary>
        public Bitmap BaseBitmap = null;
        public BitmapFile BaseBitmapFile = null;

        /// <summary>
        /// Generates a bitmap from the slide
        /// </summary>
        /// <param name="slide">Slide to generate</param>
        /// <param name="input">Inpit directory</param>
        public CompositeBitmap(Slide slide, string input)
        {
            m_Input = new DirectoryInfo( input);
            if (!m_Input.Exists) throw new Exception(input + " not found");
            m_PDF_Width = slide.pageX;
            m_PDF_Height = slide.pageY;
            m_PDF_DPI = slide.pageDPI;

            if (slide.OnMemory)
            {
                // make bitmap
                BaseBitmap = new Bitmap(slide.x, slide.y);
                Graphics g = Graphics.FromImage(BaseBitmap);
                g.Clear(slide.BkColor);

                //draw all components
                foreach (SlideComponent sc in slide.Components) sc.Draw(input, g, BaseBitmap);
                g.Dispose();
            }
            else
            {
                BaseBitmapFile = new BitmapFile(input + "\\_Swap.tiff", slide.x, slide.y, slide.BkColor, slide.pageDPI);
                int count = -1; // increment counter for debug output
                foreach (SlideComponent sc in slide.Components) 
                    sc.Draw(input, BaseBitmapFile, count);
            }
        }

        /// <summary>
        /// Sets and retrieves the PDF page width
        /// </summary>
        public double PDF_Width
        {
            get{ return m_PDF_Width;}
            set
            {
                m_PDF_Width = value;
                if( m_PDF_Width <= 0.0) m_PDF_Width = 21.59;
            }
        }

        /// <summary>
        /// Sets and retrieves the PDF page height
        /// </summary>
        public double PDF_Height
        {
            get { return m_PDF_Height; }
            set
            {
                m_PDF_Height = value;
                if (m_PDF_Height <= 0.0) m_PDF_Height = 27.94;
            }
        }

        /// <summary>
        /// Sets and retrieves the PDF DPI
        /// </summary>
        public double PDF_DPI
        {
            get { return m_PDF_DPI; }
            set
            {
                m_PDF_DPI = value;
                if (m_PDF_DPI <= 0.0) m_PDF_DPI = 250.0;
            }
        }

        /// <summary>
        /// Saves the file
        /// </summary>
        /// <param name="output"></param>
        public void Save(string output)
        {
            FileInfo fi = new FileInfo(output);
            string ext = fi.Extension.ToLower();
            if (BaseBitmap != null)
            {
                switch (ext)
                {
                    case ".png":
                        BaseBitmap.Save(output, ImageFormat.Png);
                        break;
                    case ".tif":
                    case ".tiff":
                        BaseBitmap.Save(output, ImageFormat.Tiff);
                        break;
                    case ".bmp":
                        BaseBitmap.Save(output, ImageFormat.Bmp);
                        break;
                    case ".emf":
                        BaseBitmap.Save(output, ImageFormat.Emf);
                        break;
                    case ".pdf":
                        SaveAsPDF(BaseBitmap, output);
                        break;
                    default:
                        BaseBitmap.Save(output, ImageFormat.Jpeg);
                        break;
                }
            }
            if (BaseBitmapFile != null)
            {
                switch (ext)
                {
                    case ".tif":
                    case ".tiff":
                        BaseBitmapFile.Save(output);
                        break;
                    case ".pdf":
                        SaveAsPDF(BaseBitmapFile, output);
                        break;
                    default:
                        throw new Exception("The file is exceeding the maximum size for format " + ext + ".");
                }
            }
            return;
        }

        /// <summary>
        /// Saves the PDF from the memory file
        /// </summary>
        /// <param name="b"></param>
        /// <param name="output"></param>
        public void SaveAsPDF(Bitmap b, string output)
        {
            FileInfo fi = new FileInfo(output);
            
            // Following are the calls to the Gios PDF library
            PdfDocument myPdfDocument = new PdfDocument(PdfDocumentFormat.InCentimeters(m_PDF_Width, m_PDF_Height));
            int horizontalStep = Convert.ToInt32(m_PDF_Width * m_PDF_DPI / 2.54);
            int verticalStep = Convert.ToInt32(m_PDF_Height * m_PDF_DPI / 2.54);
            Bitmap tmp = new Bitmap(horizontalStep, verticalStep);
            int origin = 0;
            List<string> tmpFiles = new List<string>();
            int tmpFileCount = 0;
            while (origin < b.Height)
            {
                CopyImage(b, tmp, origin);
                string temp_file = fi.DirectoryName + "\\_tempPDFComponent_" + tmpFileCount.ToString().PadLeft( 5, '0') + ".jpg";
                tmp.Save(temp_file, ImageFormat.Jpeg);
                tmpFiles.Add(temp_file);
                PdfPage newPdfPage = myPdfDocument.NewPage();
                PdfImage myImage = myPdfDocument.NewImage(temp_file);
                newPdfPage.Add(myImage, 0.0, 0.0, m_PDF_DPI);
                newPdfPage.SaveToDocument();
                origin += verticalStep;
                tmpFileCount++;
            }
            myPdfDocument.SaveToFile(output);
            tmp.Dispose();
            foreach (string f in tmpFiles)
            {
                try { File.Delete(f); }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// Saves the PDF from the disk swap file
        /// </summary>
        /// <param name="b"></param>
        /// <param name="output"></param>
        public void SaveAsPDF(BitmapFile b, string output)
        {
            FileInfo fi = new FileInfo(output);

            // Following are the calls to the Gios PDF library
            PdfDocument myPdfDocument = new PdfDocument(PdfDocumentFormat.InCentimeters(m_PDF_Width, m_PDF_Height));
            int horizontalStep = Convert.ToInt32(m_PDF_Width * m_PDF_DPI / 2.54);
            int verticalStep = Convert.ToInt32(m_PDF_Height * m_PDF_DPI / 2.54);
            int origin = 0;
            List<string> tmpFiles = new List<string>();
            int tmpFileCount = 0;
            while (origin < b.Y)
            {
                Bitmap tmp = b.GetBitmap(0, origin, horizontalStep, verticalStep);
                string temp_file = fi.DirectoryName + "\\_tempPDFComponent_" + tmpFileCount.ToString().PadLeft(5, '0') + ".jpg";
                tmp.Save(temp_file, ImageFormat.Jpeg);
                tmpFiles.Add(temp_file);
                PdfPage newPdfPage = myPdfDocument.NewPage();
                PdfImage myImage = myPdfDocument.NewImage(temp_file);
                newPdfPage.Add(myImage, 0.0, 0.0, m_PDF_DPI);
                newPdfPage.SaveToDocument();
                origin += verticalStep;
                tmpFileCount++;
                tmp.Dispose();
            }
            myPdfDocument.SaveToFile(output);
            foreach (string f in tmpFiles)
            {
                try { File.Delete(f); }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// Disposes bitmap
        /// </summary>
        public void Dispose()
        {
            if (BaseBitmap != null) BaseBitmap.Dispose();
            if (BaseBitmapFile != null) BaseBitmapFile.DeleteSwapFile();
        }

        private void CopyImage(Bitmap from, Bitmap to, int origin)
        {
            for (int y = 0; y < to.Height; y++)
            {
                int y_from = y + origin;
                if (y_from >= from.Height)
                {
                    for (int x = 0; x < to.Width; x++) to.SetPixel(x, y, Color.White);
                    continue;
                }
                for (int x = 0; x < to.Width; x++)
                {
                    if (x >= from.Width)
                    {
                        to.SetPixel(x, y, Color.White);
                        continue;
                    }
                    to.SetPixel(x, y, from.GetPixel( x, y_from));
                }
            }
        }
    }
}
