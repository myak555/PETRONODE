using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petronode.ENVIAccess
{
    public class ENVIDataCube
    {
        public bool isValid = false;
        public int Lines = 0;
        public int Samples = 0;
        public int Bands = 0;
        public int DataType = 0;
        public string Interleave = "BSQ";
        public int HeaderOffset = 0;
        public int ByteOrder = 0;
        public string FileType = "ENVI Standard";
        public float DataIgnoreValue = -1.0f;
        public string[] Description = new string[0];
        public double[] Wavelengths = new double[0];
        public byte[] DataPadding = new byte[0];
        public float[] Data = new float[0];
        public string Filename = "";
        public ENVIResampler Resampler = null;

        #region Public Methods
        /// <summary>
        /// Constructor
        /// </summary>
        public ENVIDataCube()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ENVIDataCube(ENVIDataCube templateCube)
        {
            this.DataType = templateCube.DataType;
            this.Interleave = templateCube.Interleave;
            this.HeaderOffset = templateCube.HeaderOffset;
            this.ByteOrder = templateCube.ByteOrder;
            this.DataIgnoreValue = templateCube.DataIgnoreValue;
            if (templateCube.Description.Length > 0)
            {
                this.Description = new string[templateCube.Description.Length];
                Array.Copy(templateCube.Description, this.Description, this.Description.Length);
            }
            if (this.HeaderOffset > 0)
            {
                this.DataPadding = new byte[this.HeaderOffset];
                Array.Copy(templateCube.DataPadding, this.DataPadding, this.DataPadding.Length);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filename">source header file</param>
        public ENVIDataCube( string filename)
        {
            Filename = filename;
            if (!File.Exists(Filename)) return;
            LoadHeader();
            LoadData();
            isValid = true;
        }

        /// <summary>
        /// returns the estimated data size assuming float data record (TODO)
        /// </summary>
        public long EstimatedDataSize
        {
            get
            {
                long tmp = Lines * Samples * Bands;
                return (tmp<<2) + HeaderOffset;
            }
        }

        /// <summary>
        /// Returns the sample index in the Data array
        /// </summary>
        /// <param name="line"></param>
        /// <param name="sample"></param>
        /// <param name="band"></param>
        /// <returns></returns>
        public int GetSampleIndex( int line, int sample, int band)
        {
            return (line * Samples + sample) * Bands + band;
        }

        /// <summary>
        /// Returns the start of spectrum in the Data array
        /// </summary>
        /// <param name="line"></param>
        /// <param name="sample"></param>
        /// <returns></returns>
        public int GetPixelIndex(int line, int sample)
        {
            return (line * Samples + sample) * Bands;
        }

        /// <summary>
        /// Returns trace at coordinates [line, sample]
        /// </summary>
        /// <param name="line"></param>
        /// <param name="sample"></param>
        /// <returns></returns>
        public ENVITrace GetPixel(int line, int sample)
        {
            double[] tmp = new double[Bands];
            for (int i = 0; i < tmp.Length; ++i)
            {
                tmp[i] = Double.NaN;
            }
            int traceIndex = GetPixelIndex(line, sample);
            for (int i = 0, j = traceIndex; i < tmp.Length; ++i, ++j)
            {
                if (j >= Data.Length) break;
                tmp[i] = Convert.ToDouble(Data[j]);
            }
            return new ENVITrace(this.Wavelengths, tmp) { ParentCube = this};
        }

        /// <summary>
        /// Returns trace at coordinates [line, sample], cropped to wavelengths
        /// </summary>
        /// <param name="line"></param>
        /// <param name="sample"></param>
        /// <param name="wlFrom"></param>
        /// <param name="wlTo"></param>
        /// <returns></returns>
        public ENVITrace GetPixel(int line, int sample, double wlFrom, double wlTo)
        {
            List<double> tmpWl = new List<double>();
            List<double> tmpTr = new List<double>();
            int traceIndex = GetPixelIndex(line, sample);
            for (int i = 0, j = traceIndex; i < Wavelengths.Length; ++i, ++j)
            {
                if (j >= Data.Length) break;
                if (Wavelengths[i] < wlFrom) continue;
                if (Wavelengths[i] > wlTo) continue;
                if (Double.IsNaN( Data[j])) continue;
                tmpWl.Add( Wavelengths[i]);
                tmpTr.Add( Convert.ToDouble(Data[j]));
            }
            return new ENVITrace(tmpWl.ToArray(), tmpTr.ToArray()) { ParentCube = this };
        }

        /// <summary>
        /// Returns trace at coordinates [line, sample], resampled to the current resampler
        /// </summary>
        /// <param name="line"></param>
        /// <param name="sample"></param>
        /// <param name="wlFrom"></param>
        /// <param name="wlTo"></param>
        /// <returns></returns>
        public ENVITrace GetPixelResampled(int line, int sample)
        {
            if (Resampler == null) return new ENVITrace() { ParentCube = this};
            int traceIndex = GetPixelIndex(line, sample);
            return new ENVITrace(Resampler.Wavelengths, Resampler.GetResampled(traceIndex)) { ParentCube = this };
        }

        /// <summary>
        /// Sets and returns trace resampler object
        /// </summary>
        /// <param name="wlFrom"></param>
        /// <param name="wlTo"></param>
        /// <param name="wlStep"></param>
        public ENVIResampler SetResampler(double wlFrom, double wlTo, double wlStep)
        {
            Resampler = new ENVIResampler(wlFrom, wlTo, wlStep, Wavelengths) { ParentCube = this };
            return Resampler;
        }

        /// <summary>
        /// Sets and returns trace resampler object
        /// <param name="wavelengths"></param>
        /// </summary>
        public ENVIResampler SetResampler(double[] wavelengths)
        {
            Resampler = new ENVIResampler(wavelengths, Wavelengths) { ParentCube = this };
            return Resampler;
        }

        /// <summary>
        /// Returns trace at coordinates [line, sample], cropped to wavelengths
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public ENVISlice GetWavelengthSlice(double wl)
        {
            if (Data.Length <= 0) return new ENVISlice();
            int closestBand = 0;
            double delta = Math.Abs( Wavelengths[0]-wl);
            for( int band=1; band<Wavelengths.Length; ++band)
            {
                double v = Math.Abs(Wavelengths[band] - wl);
                if (v > delta) continue;
                closestBand = band;
                delta = v;
            }
            int sliceSize = Lines * Samples;
            double[] resultData = new double[sliceSize];
            int resultDataPointer = 0;
            for( int line=0; line<Lines; ++line)
            {
                for (int sample = 0; sample < Samples; ++sample)
                {
                    int index = GetSampleIndex(line, sample, closestBand);
                    resultData[resultDataPointer++] = Data[index];
                }
            }
            return new ENVISlice(Lines, Samples, Wavelengths[closestBand], resultData) { ParentCube = this };
        }

        /// <summary>
        /// Saves data to the original file
        /// </summary>
        public void Save()
        {
            SaveHeader();
            SaveData();
        }

        /// <summary>
        /// Saves data to a new file
        /// </summary>
        /// <param name="filename">Filename to save into</param>
        public void Save( string filename)
        {
            Filename = filename;
            Save();
        }

        /// <summary>
        /// Returns the data cube truncated by line, sample and band
        /// </summary>
        /// <param name="lineFrom"></param>
        /// <param name="lineTo"></param>
        /// <param name="sampleFrom"></param>
        /// <param name="sampleTo"></param>
        /// <param name="wlFrom"></param>
        /// <param name="wlTo"></param>
        /// <returns></returns>
        public ENVIDataCube GetCropped( int lineFrom, int lineTo, int sampleFrom, int sampleTo, double wlFrom, double wlTo) 
        {
            ENVIDataCube result = new ENVIDataCube(this);
            List<double> tmpWl = new List<double>();
            List<int> tmpWlIndex = new List<int>();
            for ( int wl=0; wl<Wavelengths.Length; wl++)
            {
                double v = Wavelengths[wl];
                if (v < wlFrom || wlTo < v) continue;
                tmpWl.Add(v);
                tmpWlIndex.Add(wl);
            }
            if (tmpWl.Count <= 0) return result;
            result.Bands = tmpWl.Count;
            result.Wavelengths = tmpWl.ToArray();
            if (lineFrom < 0 || lineFrom>=lineTo || lineTo>Lines) return result;
            result.Lines = lineTo - lineFrom;
            if (sampleFrom < 0 || sampleFrom >= sampleTo || sampleTo > Samples) return result;
            result.Samples = sampleTo - sampleFrom;
            int dataLength = result.Lines * result.Samples * result.Bands;
            try
            {
                result.Data = new float[dataLength];
            }
            catch(Exception)
            {
                return result;
            }
            int dataPtr = 0;
            for( int line=lineFrom; line<lineTo; ++line)
            {
                for (int sample = sampleFrom; sample < sampleTo; ++sample)
                {
                    foreach( int band in tmpWlIndex)
                    {
                        int index = GetSampleIndex(line, sample, band);
                        result.Data[dataPtr++] = this.Data[index];
                    }
                }
            }
            result.isValid = true;
            return result;
        }
        #endregion

        #region Private Methods
        protected void LoadHeader() 
        {
            FileStream fs = null;
            StreamReader sr = null;
            try
            {
                fs = File.Open(Filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                sr = new StreamReader(fs);
                string r = sr.ReadToEnd();
                ParseHeader(r);
                if (Bands == 0 && Wavelengths.Length > 0) Bands = Wavelengths.Length;
                if (Bands != Wavelengths.Length) throw new Exception("Incorrect number of bands supplied in the header");
            }
            catch ( Exception e)
            {
                throw (new Exception(e.Message));
            }
            finally
            {
                if (sr != null) sr.Close();
                if (fs != null) fs.Close();
            }
        }

        protected void SaveHeader()
        {
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                fs = File.Open(Filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                sw = new StreamWriter(fs);
                sw.Write("ENVI\ndescription = {\n");
                for( int i=0; i<Description.Length; i++)
                {
                    sw.Write(Description[i] + "\n");
                }
                sw.Write("}\n");
                sw.Write("lines = " + Lines.ToString() + "\n");
                sw.Write("samples = " + Samples.ToString() + "\n");
                sw.Write("bands = " + Bands.ToString() + "\n");
                sw.Write("data type = " + DataType.ToString() + "\n");
                sw.Write("interleave = " + Interleave.ToLower() + "\n");
                sw.Write("header offset = " + HeaderOffset.ToString() + "\n");
                sw.Write("byte order = " + ByteOrder.ToString() + "\n");
                sw.Write("file type = ENVI Standard\n");
                sw.Write("wavelength = {\n");
                int wlCount = 1;
                for (int i = 0; i < Wavelengths.Length-1; i++)
                {
                    sw.Write(Wavelengths[i].ToString("F4") + ",");
                    if( wlCount < 10)
                    {
                        sw.Write(" ");
                    }
                    else
                    {
                        sw.Write("\n");
                        wlCount = 0;
                    }
                    wlCount++;
                }
                sw.Write(Wavelengths[Wavelengths.Length - 1].ToString("F4") + " }\n");
                sw.Write("data ignore value = " + DataIgnoreValue.ToString());
            }
            catch (Exception e)
            {
                throw (new Exception(e.Message));
            }
            finally
            {
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }
        }

        protected void SaveData()
        {
            FileInfo fi = new FileInfo(Filename);
            string datFilename = fi.FullName.Replace(fi.Extension, ".dat");
            FileStream fs = null;
            BinaryWriter bw = null;
            try
            {
                fs = File.Open(datFilename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                bw = new BinaryWriter(fs);
                if (HeaderOffset > 0) bw.Write(DataPadding);
                if (Lines * Samples * Bands > 0)
                {
                    switch (Interleave)
                    {
                        case "BIL":
                            InterleaveBIL(bw);
                            break;
                        case "BIP":
                            InterleaveBIP(bw);
                            break;
                        default: // BSQ by default
                            InterleaveBSQ(bw);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                throw (new Exception(e.Message));
            }
            finally
            {
                if (bw != null) bw.Close();
                if (fs != null) fs.Close();
            }
        }

        protected void ParseHeader( string r)
        {
            if (!r.StartsWith("ENVI")) throw new Exception("Malformed ENVI header (must start with \"ENVI\")");
            r = r.Replace("\r", ""); // modify Windows end-of-line symbols
            string[] splitR = r.Split('\n');
            int i = 1; // skip ENVI marker
            while( i<splitR.Length)
            {
                string line = splitR[i++];
                if (line.StartsWith("#")) continue;
                int eqIndex = line.IndexOf("=");
                if (eqIndex <= 0) continue;
                string command = line.Substring(0,eqIndex).Trim().ToLower();
                switch(command)
                {
                    case "description":
                        i = ParseDescription(splitR, i);
                        break;
                    case "lines":
                        Lines = ParseInteger(line);
                        if (Lines < 0) throw new Exception("Number of lines cannot be negative");
                        break;
                    case "samples":
                        Samples = ParseInteger(line);
                        if (Samples < 0) throw new Exception("Number of samples cannot be negative");
                        break;
                    case "bands":
                        Bands = ParseInteger(line);
                        if (Bands < 0) throw new Exception("Number of bands cannot be negative");
                        break;
                    case "data type":
                        DataType = ParseInteger(line);
                        if (DataType != 4) throw new Exception("Only data type 4 (32-bit float) is implemented");
                        break;
                    case "interleave":
                        Interleave = ParseString(line).ToUpper();
                        if (Interleave != "BIL" && Interleave != "BSQ" && Interleave != "BIP")
                            throw new Exception("Only BIL, BSQ and BIP interleave is implemented");
                        break;
                    case "header offset":
                        HeaderOffset = ParseInteger(line);
                        if (HeaderOffset < 0) throw new Exception("Header offset cannot be negative");
                        break;
                    case "byte order":
                        ByteOrder = ParseInteger(line);
                        if (ByteOrder != 0) throw new Exception("Only byte order 0 (Intel) is implemented");
                        break;
                    case "file type":
                        FileType = ParseString(line);
                        if (FileType != "ENVI Standard") throw new Exception("Only \"ENVI Standard\" file type is implemented");
                        break;
                    case "wavelength":
                        i = ParseWavelengths(splitR, i);
                        break;
                    case "data ignore value":
                        DataIgnoreValue = ParseFloat(line);
                        break;
                    default:
                        throw new Exception("Command \"" + command + "\" is not implemented");
                }
            }
        }
        
        protected int ParseDescription(string[] r, int index)
        {
            List<string> tmpDescription = new List<string>();
            while( index < r.Length)
            {
                string line = r[index++];
                if (line.StartsWith("}")) break;
                if (line.EndsWith("}"))
                {
                    line = line.Substring(0, line.Length - 1);
                    tmpDescription.Add(line);
                    break;
                }
                tmpDescription.Add(line);
            }
            Description = tmpDescription.ToArray();
            return index;
        }

        protected int ParseInteger(string r)
        {
            r = r.Replace(" ", "");
            string[] splitR = r.Split('=');
            if (splitR.Length <= 1) return -1;
            try
            {
                return Convert.ToInt32(splitR[1]);
            }
            catch
            {
                throw new Exception("Found \"" + splitR[1] + " in the integer field; malformed number");
            }
        }

        protected float ParseFloat(string r)
        {
            r = r.Replace(" ", "");
            string[] splitR = r.Split('=');
            if (splitR.Length <= 1) return -1.0f;
            try
            {
                return Convert.ToSingle(splitR[1]);
            }
            catch
            {
                throw new Exception("Found \"" + splitR[1] + " in the float field; malformed number");
            }
        }

        protected string ParseString(string r)
        {
            string[] splitR = r.Split('=');
            if (splitR.Length <= 1) return "";
            return splitR[1].Trim();
        }

        protected int ParseWavelengths(string[] r, int index)
        {
            List<double> tmpWavelengths = new List<double>();
            while (index < r.Length)
            {
                string line = r[index++];
                if (line.StartsWith("}")) break;
                if (line.EndsWith("}"))
                {
                    ParseWavelengthsLine(ref tmpWavelengths, line.Substring(0, line.Length - 1));
                    break;
                }
                ParseWavelengthsLine(ref tmpWavelengths, line);
            }
            Wavelengths = tmpWavelengths.ToArray();
            return index;
        }

        protected void ParseWavelengthsLine( ref List<double> tmpWavelengths, string r)
        {
            r = r.Replace(" ", "");
            string[] splitR = r.IndexOf(",") >= 0 ? r.Split(',') : r.Split('\t');
            for (int index=0; index < splitR.Length; index++)
            {
                string line = splitR[index];
                if (line.Length <= 0) continue;
                double v;
                try
                {
                    v = Convert.ToDouble(line);
                }
                catch
                {
                    throw new Exception("Found \"" + line + " in the wavelength field; malformed number");
                }
                if( v <= 0.0)
                {
                    throw new Exception("Found \"" + line + " in the wavelength field; must be positive");
                }
                tmpWavelengths.Add(v);
            }
        }

        protected void LoadData()
        {
            FileInfo fi = new FileInfo(Filename);
            string datFilename = fi.FullName.Replace(fi.Extension, ".dat");
            fi = new FileInfo(datFilename);
            if( !fi.Exists) throw new Exception(datFilename + " not found");
            long l = EstimatedDataSize;
            if ( fi.Length < l)
                throw new Exception(datFilename + " has incorrect length of " + fi.Length.ToString() + " (expected " + l.ToString() + ")");
            FileStream fs = null;
            BinaryReader br = null;
            try
            {
                fs = File.Open(datFilename, FileMode.Open, FileAccess.Read, FileShare.Read);
                br = new BinaryReader(fs);
                if( HeaderOffset > 0)
                {
                    DataPadding = br.ReadBytes(HeaderOffset);
                    if (DataPadding.Length != HeaderOffset) throw new Exception("Unexpected end of data file");
                }
                LoadFloats(br, l - HeaderOffset);
            }
            catch (Exception e)
            {
                throw (new Exception(e.Message));
            }
            finally
            {
                if (br != null) br.Close();
                if (fs != null) fs.Close();
            }
        }

        protected void LoadFloats( BinaryReader br, long length)
        {
            // TODO: Handle reasonably small files only
            Data = new float[length>>2];
            switch (Interleave)
            {
                case "BIL":
                    SortBIL(br);
                    break;
                case "BIP":
                    SortBIP(br);
                    break;
                default: // BSQ by default
                    SortBSQ(br);
                    break;
            }
        }

        protected void SortBIL( BinaryReader br)
        {
            for( int line = 0; line < Lines; ++line)
            {
                for (int band = 0; band < Bands; ++band)
                {
                    for (int sample = 0; sample < Samples; ++sample)
                    {
                        int index = GetSampleIndex(line, sample, band);
                        float v = br.ReadSingle();
                        Data[index] = (v != DataIgnoreValue) ? v : Single.NaN;
                    }
                }
            }
        }

        protected void SortBIP(BinaryReader br)
        {
            for (int line = 0; line < Lines; ++line)
            {
                for (int sample = 0; sample < Samples; ++sample)
                {
                    for (int band = 0; band < Bands; ++band)
                    {
                        int index = GetSampleIndex(line, sample, band);
                        float v = br.ReadSingle();
                        Data[index] = (v != DataIgnoreValue) ? v : Single.NaN;
                    }
                }
            }
        }

        protected void SortBSQ(BinaryReader br)
        {
            for (int band = 0; band < Bands; ++band)
            {
                for (int line = 0; line < Lines; ++line)
                {
                    for (int sample = 0; sample < Samples; ++sample)
                    {
                        int index = GetSampleIndex(line, sample, band);
                        float v = br.ReadSingle();
                        Data[index] = (v != DataIgnoreValue) ? v : Single.NaN;
                    }
                }
            }
        }

        protected void InterleaveBIL(BinaryWriter bw)
        {
            for (int line = 0; line < Lines; ++line)
            {
                for (int band = 0; band < Bands; ++band)
                {
                    for (int sample = 0; sample < Samples; ++sample)
                    {
                        double v = Data[GetSampleIndex(line, sample, band)];
                        float vf = Double.IsNaN(v) ? DataIgnoreValue : Convert.ToSingle(v);
                        bw.Write(vf);

                    }
                }
            }
        }

        protected void InterleaveBIP(BinaryWriter bw)
        {
            for (int line = 0; line < Lines; ++line)
            {
                for (int sample = 0; sample < Samples; ++sample)
                {
                    for (int band = 0; band < Bands; ++band)
                    {
                        double v = Data[GetSampleIndex(line, sample, band)];
                        float vf = Double.IsNaN(v) ? DataIgnoreValue : Convert.ToSingle(v);
                        bw.Write(vf);
                    }
                }
            }
        }

        protected void InterleaveBSQ(BinaryWriter bw)
        {
            for (int band = 0; band < Bands; ++band)
            {
                for (int line = 0; line < Lines; ++line)
                {
                    for (int sample = 0; sample < Samples; ++sample)
                    {
                        double v = Data[GetSampleIndex(line, sample, band)];
                        float vf = Double.IsNaN(v) ? DataIgnoreValue : Convert.ToSingle(v);
                        bw.Write(vf);
                    }
                }
            }
        }
        #endregion
    }
}
