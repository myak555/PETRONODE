using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Diagnostics;
using Petronode.OilfieldFileAccess.Converters;

namespace Petronode.SlideComposer
{
    public class BitmapFile
    {
        // Instrumentation
        private StreamWriter m_sw = null;
        private FileStream m_instrument_stream = null;

        private string m_Copyright = "TIFF Writer (C)opyright M.Yakimov, 2014"; 

        private uint m_x = 0;
        private uint m_y = 0;
        private string m_name = "";
        private Color m_bkColor = Color.White;
        private int m_resNumerator = 2500;
        private int m_resDenominator = 10;
        private FileStream m_fs = null;
        private byte[] m_buffer = null;

        public byte[] BMPHeader = new byte[54];
        public byte[] TIFHeader = null;

        /// <summary>
        /// Constructor, creates a bitmap file with size x times y
        /// </summary>
        /// <param name="name">file name</param>
        /// <param name="x">width</param>
        /// <param name="y">height</param>
        /// <param name="bkColor">background color</param>
        public BitmapFile( string name, int x, int y, Color bkColor, double resolution)
        {
            // Instrumentation
            //m_instrument_stream = File.Open(name + ".lock", FileMode.Create, FileAccess.Write, FileShare.Read);
            //m_sw = new StreamWriter( m_instrument_stream);
            //if (m_sw != null) m_sw.WriteLine("Instrumentation file for " + name);
            //WriteInstrumentationMessage( "Started");

            m_x = (uint)x;
            m_y = (uint)y;
            m_name = name;
            m_bkColor = bkColor;
            m_resNumerator = Convert.ToInt32( resolution * m_resDenominator);
            SetBMPHeader();
            SetTIFHeader();
            CreateSwapFile();
        }

        /// <summary>
        /// Close the bitmap file
        /// </summary>
        public void Close()
        {
            WriteInstrumentationMessage("Close requested");
            if (m_fs != null)
            {
                m_fs.Close();
                m_fs = null;
            }
        }

        /// <summary>
        /// Sets and retrieves the image width
        /// </summary>
        public uint X
        {
            get { return m_x; }
            set
            {
                if (m_x == value) return;
                m_x = value;
                SetBMPHeader();
                SetTIFHeader();
                CreateSwapFile();
            }
        }

        /// <summary>
        /// Sets and retrieves the image height
        /// </summary>
        public uint Y
        {
            get { return m_y; }
            set
            {
                if (m_y == value) return;
                m_y = value;
                SetBMPHeader();
                SetTIFHeader();
                CreateSwapFile();
            }
        }

        /// <summary>
        /// Retrieves the row size in bytes
        /// </summary>
        public uint RowSize
        {
            get
            {
                return m_x * 3; 
            }
        }

        /// <summary>
        /// Retrieves the row size in bytes
        /// </summary>
        public uint RowSizeBMP
        {
            get
            {
                uint s = (24 * m_x + 31) / 32;
                return s * 4;
            }
        }

        /// <summary>
        /// Retrieves the total image size
        /// </summary>
        public uint ImageSize
        {
            get
            {
                return RowSize * m_y;
            }
        }

        /// <summary>
        /// Retrieves the total image size
        /// </summary>
        public uint ImageSizeBMP
        {
            get
            {
                return RowSizeBMP * m_y;
            }
        }

        /// <summary>
        /// Writes initial file 
        /// </summary>
        public void CreateSwapFile()
        {
            // Instrumentation
            DateTime tStart = DateTime.Now;

            Close();
            m_fs = File.Open(m_name, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            m_fs.Write(TIFHeader, 0, TIFHeader.Length);
            byte[] buff = new byte[RowSize];
            byte[] bk = Slide.GetColorBytes(m_bkColor);
            for( uint i=0; i<buff.Length; i+=3)
            {
                buff[i] = bk[0];
                buff[i + 1] = bk[1];
                buff[i + 2] = bk[2];
            }
            for (uint i = 0; i < m_y; i++)
            {
                m_fs.Write(buff, 0, buff.Length);
            }
            m_fs.Flush();

            // Instrumentation
            TimeSpan ts = DateTime.Now.Subtract( tStart);
            WriteInstrumentationMessage("Swap file creation done: " +
                ts.TotalSeconds.ToString("0.000") + " sec (" + m_fs.Length.ToString("0") + " bytes)");
        }

        /// <summary>
        /// Performs conversion to specified format
        /// </summary>
        /// <param name="output"></param>
        public void Save( string output)
        {
            // Instrumentation
            DateTime tStart = DateTime.Now;

            m_fs.Flush();
            FileStream outFile = File.Open(output, FileMode.Create, FileAccess.Write, FileShare.Read);
            outFile.Write(TIFHeader, 0, TIFHeader.Length);
            byte[] buff = new byte[ RowSize];
            m_fs.Seek(TIFHeader.Length, SeekOrigin.Begin);
            for (int i = 0; i < m_y; i++)
            {
                m_fs.Read(buff, 0, buff.Length);
                outFile.Write(buff, 0, buff.Length);
            }
            outFile.Close();

            // Instrumentation
            TimeSpan ts = DateTime.Now.Subtract(tStart);
            WriteInstrumentationMessage("Save to TIFF done: " +
                ts.TotalSeconds.ToString("0.000") + " sec (" + m_fs.Length.ToString("0") + " bytes)");
        }

        /// <summary>
        /// Deletes swap file 
        /// </summary>
        public void DeleteSwapFile()
        {
            Close();
            try
            {
                File.Delete(m_name);
            }
            catch (Exception ex)
            {
                WriteInstrumentationMessage(ex.Message);
            }

            // Instrumentation
            WriteInstrumentationMessage("Swap file deletion requested");
            if (m_sw != null) m_sw.Close();
            m_sw = null;
            if (m_instrument_stream != null) m_instrument_stream.Close();
            m_instrument_stream = null;
            string logname = m_name + ".log";
            try
            {
                if (File.Exists(logname)) File.Delete(logname);
                File.Move(m_name + ".lock", logname);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Extracts the bitmap from file
        /// </summary>
        /// <param name="x">left position</param>
        /// <param name="y">top position</param>
        /// <param name="dx">horizontal size</param>
        /// <param name="dy">vertical size</param>
        /// <returns></returns>
        public Bitmap GetBitmap(int x, int y, int dx, int dy)
        {
            // Instrumentation
            WriteInstrumentationMessage( "Bitmap requested: at ["
                + x.ToString("0") + "," + y.ToString("0") + "], size ["
                + dx.ToString("0") + "," + dy.ToString("0") + "]");
            DateTime tStart = DateTime.Now;

            // Create output image and fill it with background
            Bitmap tmp = new Bitmap(dx, dy);
            Graphics g = Graphics.FromImage(tmp);
            g.Clear(m_bkColor);
            g.Dispose();

            // Decide how much of the file has to be extracted
            int x1 = x;
            int dx1 = dx;
            int xOrigin = 0;
            if (ComputeLimits( ref x1, ref dx1, ref xOrigin, m_x)) return tmp;
            int y1 = y;
            int dy1 = dy;
            int yOrigin = 0;
            if (ComputeLimits( ref y1, ref dy1, ref yOrigin, m_y)) return tmp;

            // Decide the start and the end of the buffer to be read
            // Note that in bmp files the rows are up side down
            uint rs = RowSize;
            long bufferStart = TIFHeader.Length + (long)(y1) * (long)(rs);
            m_buffer = new byte[rs*dy1];
            m_fs.Seek(bufferStart, SeekOrigin.Begin);
            m_fs.Read(m_buffer, 0, m_buffer.Length);

            // Fill image from the buffer
            for (int j = 0; j < dy1; j++)
            {
                int offset = j * (int)rs + x1 * 3;
                for (int i = 0, k = offset; i < dx1; i++, k += 3)
                {
                    tmp.SetPixel(xOrigin+i, yOrigin+j, Slide.GetBytesColor(m_buffer, k));
                }
            }

            // Instrumentation
            TimeSpan ts = DateTime.Now.Subtract(tStart);
            int pix = dx * dy;
            WriteInstrumentationMessage( "Bitmap extracted: at ["
                + x1.ToString("0") + "," + y1.ToString("0") + "], size ["
                + dx1.ToString("0") + "," + dy1.ToString("0") + "] in "
                + ts.TotalSeconds.ToString("0.000") + " sec (" + pix.ToString("0") + " pixels)");

            return tmp;
        }

        /// <summary>
        /// inserts the bitmap to file
        /// </summary>
        public bool SetBitmap(Bitmap b, int x, int y)
        {
            return SetBitmap(b, x, y, true);
        }

        /// <summary>
        /// inserts the bitmap to file
        /// </summary>
        public bool SetBitmap( Bitmap b, int x, int y, bool preserveBitmap)
        {
            // Instrumentation
            WriteInstrumentationMessage("Bitmap pushed: at ["
                + x.ToString("0") + "," + y.ToString("0") + "], size ["
                + b.Width.ToString("0") + "," + b.Height.ToString("0") + "]");
            DateTime tStart = DateTime.Now;
            bool ReadPerformed = false;

            // Decide how much of the file has to be pasted
            int x1 = x;
            int dx1 = b.Width;
            int xOrigin = 0;
            if (ComputeLimits(ref x1, ref dx1, ref xOrigin, m_x)) return false;
            int y1 = y;
            int dy1 = b.Height;
            int yOrigin = 0;
            if (ComputeLimits(ref y1, ref dy1, ref yOrigin, m_y)) return false;

            // Decide the start and the end of the buffer to be read
            // Note that in bmp files the rows are up side down
            uint rs = RowSize;
            long bufferStart = TIFHeader.Length + (long)(y1) * (long)(rs);
            int bufferSize = (int)(rs * dy1);
            if (m_buffer == null || m_buffer.Length != bufferSize || !preserveBitmap)
            {
                m_buffer = new byte[bufferSize];
                if (dx1 < m_x) // need to fetch the unaffected areas
                {
                    ReadPerformed = true;
                    m_fs.Seek(bufferStart, SeekOrigin.Begin);
                    m_fs.Read(m_buffer, 0, m_buffer.Length);
                }
            }

            // Fill image to the buffer
            for (int j = 0; j < dy1; j++)
            {
                int offset = j * (int)rs + x1 * 3;
                for (int i = 0, k = offset; i < dx1; i++, k += 3)
                {
                    byte[] bb = Slide.GetColorBytes(b.GetPixel(xOrigin + i, yOrigin + j));
                    m_buffer[k] = bb[0];
                    m_buffer[k + 1] = bb[1];
                    m_buffer[k + 2] = bb[2];
                }
            }
            m_fs.Seek(bufferStart, SeekOrigin.Begin);
            m_fs.Write(m_buffer, 0, m_buffer.Length);
            m_fs.Flush();

            // Instrumentation
            TimeSpan ts = DateTime.Now.Subtract(tStart);
            int pix = b.Width * b.Height;
            WriteInstrumentationMessage("Bitmap saved: at ["
                + x1.ToString("0") + "," + y1.ToString("0") + "], size ["
                + dx1.ToString("0") + "," + dy1.ToString("0") + "] in "
                + ts.TotalSeconds.ToString("0.000") + " sec (" + pix.ToString("0") + " pixels)");
            return ReadPerformed;
        }

        private void SetBMPHeader()
        {
            NumberUnion fu = new NumberUnion();
            uint size = ImageSizeBMP + (uint)BMPHeader.Length; // file size is composed of the pixel length and the header
            BufferConverter.SetBytesString(BMPHeader, fu, "BM", 2, 0); // first two bytes are always BM
            BufferConverter.SetBytesUInt32(BMPHeader, fu, size, 2);     // file length
            BufferConverter.SetBytesInt32(BMPHeader, fu, BMPHeader.Length, 10); // data start
            BufferConverter.SetBytesUInt32(BMPHeader, fu, 40, 14);      // size of DIB header
            BufferConverter.SetBytesUInt32(BMPHeader, fu, m_x, 18);     // width
            BufferConverter.SetBytesUInt32(BMPHeader, fu, m_y, 22);     // height
            BufferConverter.SetBytesUInt16(BMPHeader, fu, 1, 26);       // color planes = 1
            BufferConverter.SetBytesUInt16(BMPHeader, fu, 24, 28);      // bits per pixel
            BufferConverter.SetBytesUInt32(BMPHeader, fu, 0, 30);       // no compression
            BufferConverter.SetBytesUInt32(BMPHeader, fu, ImageSizeBMP, 34);     // raw image size
            BufferConverter.SetBytesUInt32(BMPHeader, fu, 11811, 38);   // horizontal bits per meter 300 dpi
            BufferConverter.SetBytesUInt32(BMPHeader, fu, 11811, 42);   // vertical bits per meter 300 dpi
            BufferConverter.SetBytesUInt32(BMPHeader, fu, 0, 46);       // default color palette
            BufferConverter.SetBytesUInt32(BMPHeader, fu, 0, 50);       // important colors, ignored
        }

        private void SetTIFHeader()
        {
            NumberUnion nu = new NumberUnion();

            // the TIFF header length for a single-page file is defined as following
            // 8-bit magic header
            // first IFD = 2 bytes + nValues*12 + 4 bytes
            // 3 * 2 bytes for the bit per sample
            // 2 * 8 bytes for the DPI resolution
            // Copyright + 1 null byte
            // Row start addresses 4 bytes * image height
            // Row lenghts 4 bytes * image height
            int cIFDentries = 17;
            long bufLength = 8 + 2 + (long)cIFDentries * 12 + 4 + 6 + 16 + (long)m_Copyright.Length + 1 + (long)m_y * 8;
            TIFHeader = new byte[bufLength];

            // Write magic number. Note the 42 - the Meaning of Life, Universe and Everything
            BufferConverter.SetBytesString(TIFHeader, nu, "II", 2, 0);   // Little endian
            BufferConverter.SetBytesUInt16(TIFHeader, nu, 42, 2);        // Meaning of life
            BufferConverter.SetBytesUInt32(TIFHeader, nu, 8, 4);         // Address of IFD

            // Write IFD
            // 8 bytes in the magic number, 2 bytes in IFD length, and 4 bytes in the next address
            uint cIFDboundary = (uint)(12 * cIFDentries) + 14;
            // 6 bytes for bits, 16 bytes for DPI, and the Copyrigtht
            uint cListBoundary = cIFDboundary + 22 + (uint)m_Copyright.Length + 1;
            BufferConverter.SetBytesInt16(TIFHeader, nu, (short)cIFDentries, 8); // number of entries

            // Write IFD entries
            MakeIFDEntry(TIFHeader, 0, 254, 4, 1, 2);                    // apparently needed
            MakeIFDEntry(TIFHeader, 1, 256, 4, 1, (uint)m_x);            // x size
            MakeIFDEntry(TIFHeader, 2, 257, 4, 1, (uint)m_y);            // y size
            MakeIFDEntry(TIFHeader, 3, 258, 3, 3, cIFDboundary);         // points to the bit per sample
            MakeIFDEntry(TIFHeader, 4, 259, 3, 1, 1);                    // no compression - flat file
            MakeIFDEntry(TIFHeader, 5, 262, 3, 1, 2);                    // photometric
            MakeIFDEntry(TIFHeader, 6, 273, 4, m_y, cListBoundary);   // offset strips
            MakeIFDEntry(TIFHeader, 7, 274, 3, 1, 1);                    // image orientation
            MakeIFDEntry(TIFHeader, 8, 277, 3, 1, 3);                    // samples per pixel
            MakeIFDEntry(TIFHeader, 9, 278, 3, 1, 1);                    // rows per strip
            MakeIFDEntry(TIFHeader, 10, 279, 4, m_y, cListBoundary + m_y * 4); // strip lengths
            MakeIFDEntry(TIFHeader, 11, 282, 5, 1, cIFDboundary + 6);    // X_resoltion
            MakeIFDEntry(TIFHeader, 12, 283, 5, 1, cIFDboundary + 14);   // Y_resoltion
            MakeIFDEntry(TIFHeader, 13, 284, 3, 1, 1);                   // chunky format
            MakeIFDEntry(TIFHeader, 14, 296, 3, 1, 2);                   // dimension in cm=3
            MakeIFDEntry(TIFHeader, 15, 297, 3, 2, 0);                   // page orger
            MakeIFDEntry(TIFHeader, 16, 33432, 2, (uint)m_Copyright.Length + 1, cIFDboundary + 22); // copyright

            // close IFD
            BufferConverter.SetBytesInt32(TIFHeader, nu, 0, (int)cIFDboundary - 4); // no next IDF

            // now write parameters
            BufferConverter.SetBytesInt16(TIFHeader, nu, 8, (int)cIFDboundary);  // 3 times 8 for bit per sample
            BufferConverter.SetBytesInt16(TIFHeader, nu, 8, (int)cIFDboundary + 2);
            BufferConverter.SetBytesInt16(TIFHeader, nu, 8, (int)cIFDboundary + 4);
            BufferConverter.SetBytesInt32(TIFHeader, nu, m_resNumerator, (int)cIFDboundary + 6);
            BufferConverter.SetBytesInt32(TIFHeader, nu, m_resDenominator, (int)cIFDboundary + 10);
            BufferConverter.SetBytesInt32(TIFHeader, nu, m_resNumerator, (int)cIFDboundary + 14);
            BufferConverter.SetBytesInt32(TIFHeader, nu, m_resDenominator, (int)cIFDboundary + 18);
            BufferConverter.SetBytesString(TIFHeader, nu, m_Copyright, m_Copyright.Length, (int)cIFDboundary + 22);

            // now write the addresses and offsets
            long blen = (long)m_y * 4;
            uint rowSize = (uint)m_x * 3;
            uint startPosition = cListBoundary + m_y * 8;
            for (int i = 0; i < blen; i += 4)
            {
                BufferConverter.SetBytesUInt32(TIFHeader, nu, startPosition, (int)cListBoundary + i);
                startPosition += rowSize;
            }
            cListBoundary += m_y * 4;
            for (int i = 0; i < blen; i += 4)
            {
                BufferConverter.SetBytesUInt32(TIFHeader, nu, rowSize, (int)cListBoundary + i);
            }
        }

        private void MakeIFDEntry(byte[] buff, int index, ushort code, ushort type, uint count, uint val)
        {
            NumberUnion nu = new NumberUnion();
            int offset = index * 12 + 10;
            BufferConverter.SetBytesUInt16(buff, nu, code, offset);      // code
            BufferConverter.SetBytesUInt16(buff, nu, type, offset + 2);  // type
            BufferConverter.SetBytesUInt32(buff, nu, count, offset + 4); // count
            BufferConverter.SetBytesUInt32(buff, nu, val, offset + 8);   // value
        }

        private bool ComputeLimits( ref int a, ref int da, ref int origin, uint ma)
        {
            int a1 = a;
            int da1 = da;
            origin = 0;
            if (a1 > ma) return true; // outside of the file right or top border
            if (a1 < 0)
            {
                origin = -a1;
                da1 += a1;
                a1 = 0;
                if (da1 < 0) return true; // outside of the file left or bottom border
            }
            uint aa1 = (uint)(a1 + da1);
            if (aa1 > ma) da1 -= (int)(aa1 - ma);
            a = a1;
            da = da1;
            return false;
        }

        private void WriteInstrumentationMessage(string Message)
        {
            if (m_sw == null) return;
            try
            {
                DateTime t = DateTime.Now;
                m_sw.WriteLine( t.ToString("dd-MMM-yyyy HH:mm:ss") + "."
                    + t.Millisecond.ToString().PadLeft(3, '0')
                    + " " + Message);
                m_sw.Flush();
            }
            catch (Exception) { return; }
        }
    }
}
