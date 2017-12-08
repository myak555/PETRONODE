using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Petronode.OilfieldFileAccess
{
    /// <summary>
    /// Descrives the generic curve type
    /// </summary>
    public class Oilfield_Channel
    {
        public string Name = "Unknown";
        public string Unit = "";
        public string Description = "";
        public double DepthOffset = 0.0;
        public List<double> Data = new List<double>();
        public int Decimals_Count = 2;
        public string Format = "0.00";
        public int PaddedWidth = 10;
        public int ValidCount = 0;
        public int MissingCount = 0;
        public double DataStart = Double.NaN;
        public double DataEnd = Double.NaN;
        public int DataStartIndex = -1;
        public int DataEndIndex = -1;
        public double MinValue = Double.NaN;
        public double MaxValue = Double.NaN;
        public double Average = Double.NaN;
        public List<Oilfield_Constant> Parameters = new List<Oilfield_Constant>();

        #region Constructors
        /// <summary>
        /// Constructor - creates a default variable
        /// </summary>
        public Oilfield_Channel()
        {
        }

        /// <summary>
        /// Constructor - creates from the parameters
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="unit">Variable unit</param>
        /// <param name="description">Description</param>
        public Oilfield_Channel(string name, string unit, string description)
        {
            Name = name;
            Unit = unit;
            Description = description;
        }

        /// <summary>
        /// Creates a deep copy of a given channel
        /// </summary>
        /// <param name="c"></param>
        public Oilfield_Channel(Oilfield_Channel c)
        {
            this.Name = c.Name;
            this.Unit = c.Unit;
            this.Description = c.Description;
            this.DepthOffset = c.DepthOffset;
            this.Decimals_Count = c.Decimals_Count;
            this.Format = c.Format;
            this.PaddedWidth = c.PaddedWidth;
            this.ValidCount = c.ValidCount;
            this.MissingCount = c.MissingCount;
            this.DataStart = c.DataStart;
            this.DataEnd = c.DataEnd;
            this.DataStartIndex = c.DataStartIndex;
            this.DataEndIndex = c.DataEndIndex;
            this.MinValue = c.MinValue;
            this.MaxValue = c.MaxValue;
            this.Average = c.Average;
            foreach (Oilfield_Constant d in c.Parameters) this.Parameters.Add(d);
            foreach (double d in c.Data) this.Data.Add(d);
        }
        #endregion

        #region Public Methods 
        /// <summary>
        /// Converts channel file string
        /// </summary>
        /// <returns>Converted string</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(" ");

            // name is padded to 4 symbols, then dot and unit added and padded to 14 symbols
            string s1 = Name.PadRight(4) + "." + Unit;
            sb.Append(s1);

            // then, if the description is present,
            // the string is padded to 34 symbols, and the description is added
            if (Description.Length > 0) return sb.ToString().PadRight(34) + ":" + Description;
            else return sb.ToString();
        }

        /// <summary>
        /// Converts to strings array
        /// </summary>
        /// <returns>Converted string</returns>
        public virtual string[] ToStrings()
        {
            return ToStrings(1);
        }

        /// <summary>
        /// Retirns desctiption strings
        /// </summary>
        public virtual string[] ToStrings(int type)
        {
            if (type == 1)
            {
                string[] tmp = new string[3];
                tmp[0] = this.Name;
                tmp[1] = this.Unit;
                tmp[2] = this.Description;
                return tmp;
            }
            if (type == 2)
            {
                string[] tmp = new string[7];
                tmp[0] = this.Name;
                tmp[1] = this.Unit;
                tmp[2] = "TypeFloat";
                tmp[3] = this.Description;
                tmp[4] = this.MissingCount.ToString();
                tmp[5] = this.MinValue.ToString(this.Format);
                tmp[6] = this.MaxValue.ToString(this.Format);
                return tmp;
            }
            if (type == 3)
            {
                string[] tmp = new string[9];
                tmp[0] = this.Name;
                tmp[1] = this.Unit;
                tmp[2] = "TypeFloat";
                tmp[3] = this.Description;
                tmp[4] = this.ValidCount.ToString();
                tmp[5] = this.MissingCount.ToString();
                tmp[6] = this.MinValue.ToString(this.Format);
                tmp[7] = this.MaxValue.ToString(this.Format);
                tmp[8] = this.Average.ToString(this.Format);
                return tmp;
            }
            return ToStrings();
        }

        /// <summary>
        /// Converts from strings to values
        /// </summary>
        /// <param name="ss"></param>
        public virtual void FromStrings(string[] ss)
        {
            this.Name = ss[0].Replace( "\"", "");
            this.Unit = ss[1].Replace("\"", "");
            this.Description = ss[3].Replace("\"", "");
            this.ValidCount = Convert.ToInt32(ss[4]);
            this.MissingCount = Convert.ToInt32(ss[5]);
            this.MinValue = Convert.ToDouble(ss[6]);
            this.MaxValue = Convert.ToDouble(ss[7]);
            this.Average = Convert.ToDouble(ss[8]);
        }

        /// <summary>
        /// Converts data at level i to string 
        /// </summary>
        /// <param name="i">level number</param>
        /// <returns>converted string</returns>
        public string ToString(int i, double nullValue)
        {
            double v = Data[i];
            string tmp = Double.IsNaN(v) ? nullValue.ToString(Format) : v.ToString(Format);
            return tmp.PadLeft(PaddedWidth);
        }

        /// <summary>
        /// Adds data to the channel
        /// </summary>
        /// <param name="data"></param>
        /// <param name="nullValue"></param>
        public void AddData(string data, double nullValue)
        {
            // define the format
            int pos = data.LastIndexOf('.');
            int decimals = (pos>=0)? data.Length - pos - 1: 0;
            if (decimals > Decimals_Count)
            {
                Decimals_Count = decimals;
                Format = "0.";
                for (int i = 0; i < Decimals_Count; i++) Format += "0";
            }
            double val = Convert.ToDouble(data);
            if (val == nullValue) val = Double.NaN;
            Data.Add(val);
        }

        /// <summary>
        /// Adds data to the channel
        /// </summary>
        /// <param name="data"></param>
        /// <param name="nullValue"></param>
        public void AddData(double data, double nullValue)
        {
            if (Double.IsNaN(data) || Double.IsNaN(nullValue))
            {
                Data.Add(data);
                return;
            }
            if (data == nullValue) data = Double.NaN;
            Data.Add(data);
        }

        /// <summary>
        /// Sets data to val
        /// </summary>
        /// <param name="val"></param>
        public void SetData(double val)
        {
            for (int i = 0; i < Data.Count; i++) Data[i] = val;
            this.MinValue = val;
            this.MaxValue = val;
        }

        /// <summary>
        /// Recreates the data as sequence of doubles from start to stop
        /// </summary>
        /// <param name="start">First double</param>
        /// <param name="stop">Last double</param>
        /// <param name="step">Step</param>
        public void SetData(double start, double stop, double step)
        {
            Data.Clear();
            if (step == 0.0)
            {
                SetData(start);
                return;
            }
            double val = start;
            for (int i = 1; val <= stop; i++)
            {
                Data.Add( val);
                val = start + step * i;
            }
            this.MinValue = Double.NaN;
            this.MaxValue = Double.NaN;
            if (Data.Count <= 0) return;
            int l = Data.Count - 1;
            if (Data[0] <= Data[l])
            {
                MinValue = Data[0];
                MaxValue = Data[l];
            }
            else
            {
                MinValue = Data[l];
                MaxValue = Data[0];
            }
        }

        /// <summary>
        /// Sets data to val
        /// </summary>
        /// <param name="val"></param>
        public void SetMissingTo(double val)
        {
            for (int i = 0; i < Data.Count; i++)
            {
                if( Double.IsNaN( Data[i])) Data[i] = val;
            }
        }

        /// <summary>
        /// Applies a simple depth offset on the channel data
        /// </summary>
        /// <param name="step"></param>
        public void ApplyDepthOffset( double step)
        {
            if (step == 0.0) return;
            if (DepthOffset == 0.0) return;
            double d_shift = DepthOffset / step;
            bool negative = d_shift < 0.0;
            if (negative) d_shift = -d_shift;
            int shift = Convert.ToInt32( Math.Round(d_shift));
            if (negative) shift = -shift;
            List<double> tmpData = new List<double>();
            for (int i = 0; i < Data.Count; i++)
            {
                int j = i - shift;
                if ( 0<= j && j<Data.Count)
                {
                    tmpData.Add(Data[j]);
                    continue;
                }
                tmpData.Add(Double.NaN);
            }
            this.Data = tmpData;
            DepthOffset = 0.0;
        }

        /// <summary>
        /// Loads the data from file
        /// </summary>
        /// <returns>The datalist</returns>
        public virtual List<double> LoadData()
        {
            return Data;
        }

        /// <summary>
        /// Loads the data from file - this one support the Petrolog notion of "Dimension" - data arrays
        /// </summary>
        /// <returns>The datalist</returns>
        public virtual List<double> LoadData( int dimension)
        {
            return Data;
        }

        /// <summary>
        /// Saves the data to file
        /// </summary>
        public virtual void SaveData()
        {
        }

        /// <summary>
        /// Saves the data to file - this one support the Petrolog notion of "Dimension" - data arrays
        /// </summary>
        public virtual void SaveData( int dimension)
        {
        }

        /// <summary>
        /// Saves the data to file
        /// </summary>
        public virtual void ClearData()
        {
            Data.Clear();
            Data = new List<double>();
        }

        /// <summary>
        /// Returns true if the data is loaded
        /// </summary>
        public bool IsLoaded
        {
            get { return Data.Count > 0; }
        }

        /// <summary>
        /// Locates the upper and lower boundaries for the channel
        /// </summary>
        /// <param name="index">index channel or null</param>
        /// <returns>true is boundearies are located</returns>
        public virtual bool LocateDataBoundaries( Oilfield_Channel index)
        {
            ValidCount = 0;
            MissingCount = 0;
            DataStart = Double.NaN;
            DataEnd = Double.NaN;
            DataStartIndex = -1;
            DataEndIndex = -1;
            MinValue = Double.NaN;
            MaxValue = Double.NaN;
            Average = Double.NaN;
            if (Data.Count <= 0) return false;
            if (index != null)
            {
                DataStart = index.Data[0];
                DataEnd = index.Data[Data.Count - 1];
            }
            DataStartIndex = 0;
            DataEndIndex = Data.Count - 1;
            bool data_located = false;
            for (int i = 0; i < Data.Count; i++)
            {
                DataStartIndex = i;
                if (!Double.IsNaN(Data[i]))
                {
                    data_located = true;
                    break;
                }
            }
            if (!data_located)
            {
                ValidCount = 0;
                MissingCount = Data.Count;
                return false;
            }
            if( index != null) DataStart = index.Data[DataStartIndex];
            data_located = false;
            for (int i = Data.Count - 1; i >= 0; i--)
            {
                DataEndIndex = i;
                if (!Double.IsNaN(Data[i]))
                {
                    data_located = true;
                    break;
                }
            }
            if (!data_located)
            {
                ValidCount = 0;
                MissingCount = Data.Count;
                return false;
            }
            if (index != null) DataEnd = index.Data[DataEndIndex];
            MinValue = Double.MaxValue;
            MaxValue = Double.MinValue;
            Average = 0.0;
            double AverageCount = 0.0;
            for (int i=0; i<Data.Count; i++)
            {
                double d = Data[i];
                if (Double.IsNaN(d))
                {
                    MissingCount++;
                    continue;
                }
                ValidCount++;
                if (MinValue > d) MinValue = d;
                if (MaxValue < d) MaxValue = d;
                Average += d;
                AverageCount += 1.0;
            }
            Average /= AverageCount;
            return true;
        }

        /// <summary>
        /// Returns the channel average from start to end inclusive. Note: no boundary check is performed
        /// </summary>
        /// <param name="start">start index</param>
        /// <param name="end">end index</param>
        /// <returns>ariphmetic average</returns>
        public virtual double GetAverage( int start, int end)
        {
            double average = 0.0;
            double averageCount = 0.0;
            for (int i = start; i <= end; i++)
            {
                double d = Data[i];
                if (Double.IsNaN(d)) continue;
                average += d;
                averageCount += 1.0;
            }
            if (averageCount <= 0.0) return Double.NaN;
            return average /= averageCount;
        }

        /// <summary>
        /// Returns the channel average from start to end depth inclusive.
        /// </summary>
        /// <param name="start">start index</param>
        /// <param name="end">end index</param>
        /// <param name="index">index channel</param>
        /// <returns>ariphmetic average or NaN</returns>
        public virtual double GetAverage(double start, double end, Oilfield_Channel index)
        {
            double average = 0.0;
            double averageCount = 0.0;
            for (int i = 0; i <Data.Count; i++)
            {
                double d = index.Data[i];
                if (d < start || end < d) continue;
                d = Data[i];
                if (Double.IsNaN(d)) continue;
                average += d;
                averageCount += 1.0;
            }
            if (averageCount <= 0.0) return Double.NaN;
            return average /= averageCount;
        }

        /// <summary>
        /// Returns the channel average
        /// </summary>
        /// <returns>ariphmetic average</returns>
        public virtual double GetAverage()
        {
            return GetAverage(0, Data.Count - 1);
        }

        /// <summary>
        /// Changes the channel units
        /// </summary>
        /// <param name="UnitFrom">Unit name to change from. If no match, channel is not changing</param>
        /// <param name="UnitTo">Unit name to change to</param>
        /// <param name="gain">Conversion gain</param>
        /// <param name="offset">Conversion offset</param>
        public virtual bool ChangeUnits(string UnitFrom, string UnitTo, double gain, double offset)
        {
            if (this.Unit != UnitFrom) return false;
            this.Unit = UnitTo;
            if (!this.IsLoaded) return false;
            for (int i = 0; i < Data.Count; i++)
            {
                if (!Double.IsNaN(Data[i])) Data[i] = Data[i] * gain + offset;
            }
            if( !Double.IsNaN( this.Average)) this.Average = this.Average * gain + offset;
            if( !Double.IsNaN( this.MinValue)) this.MinValue = this.MinValue * gain + offset;
            if( !Double.IsNaN( this.MaxValue)) this.MaxValue = this.MaxValue * gain + offset;
            return true;
        }

        /// <summary>
        /// Creates a deep copy of a given channel
        /// </summary>
        /// <param name="c"></param>
        public virtual Oilfield_Channel Clone()
        {
            Oilfield_Channel tmp = new Oilfield_Channel();
            tmp.Name = this.Name;
            tmp.Unit = this.Unit;
            tmp.Description = this.Description;
            tmp.DepthOffset = this.DepthOffset;
            tmp.Decimals_Count = this.Decimals_Count;
            tmp.Format = this.Format;
            tmp.PaddedWidth = this.PaddedWidth;
            tmp.ValidCount = this.ValidCount;
            tmp.MissingCount = this.MissingCount;
            tmp.DataStart = this.DataStart;
            tmp.DataEnd = this.DataEnd;
            tmp.DataStartIndex = this.DataStartIndex;
            tmp.DataEndIndex = this.DataEndIndex;
            tmp.MinValue = this.MinValue;
            tmp.MaxValue = this.MaxValue;
            tmp.Average = this.Average;
            foreach (Oilfield_Constant d in this.Parameters) tmp.Parameters.Add(d.Clone());
            foreach (double d in this.Data) tmp.Data.Add(d);
            return tmp;
        }

        /// <summary>
        /// Writes channel as a binary - this is a dummy virtual method
        /// </summary>
        /// <param name="fs"></param>
        public virtual void Write(FileStream fs)
        {
        }
        #endregion

        #region Operators
        /// <summary>
        /// Indexer, returns the data
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double this[int index]
        {
            get
            {
                if (index < 0) return Double.NaN;
                if (index >= Data.Count) return Double.NaN;
                return Data[index];
            }
            set
            {
                if (index < 0) return;
                if (index >= Data.Count) return;
                Data[index] = value;
            }
        }

        /// <summary>
        /// Indexer, returns the constant (by name)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Oilfield_Constant this[string name]
        {
            get
            {
                foreach (Oilfield_Constant oc in Parameters)
                {
                    if( oc.Name != name) continue;
                    return oc;
                }
                return null;
            }
        }

        ///// <summary>
        ///// Unary plus
        ///// </summary>
        ///// <param name="a"></param>
        ///// <returns></returns>
        //public static double operator +(Oilfield_Constant a)
        //{
        //    return a.ValueD;
        //}

        ///// <summary>
        ///// Unary minus
        ///// </summary>
        ///// <param name="a"></param>
        ///// <returns></returns>
        //public static double operator -(Oilfield_Constant a)
        //{
        //    return -a.ValueD;
        //}

        ///// <summary>
        ///// Add a real number
        ///// </summary>
        ///// <param name="a"></param>
        ///// <param name="f"></param>
        ///// <returns></returns>
        //public static double operator +(Oilfield_Constant a, double f)
        //{
        //    double c = a.ValueD;
        //    return c + f;
        //}

        ///// <summary>
        ///// Add to a real number
        ///// </summary>
        ///// <param name="f"></param>
        ///// <param name="a"></param>
        ///// <returns></returns>
        //public static double operator +(double f, Oilfield_Constant a)
        //{
        //    double c = a.ValueD;
        //    return c + f;
        //}

        ///// <summary>
        ///// Add two constants
        ///// </summary>
        ///// <param name="a"></param>
        ///// <param name="b"></param>
        ///// <returns></returns>
        //public static double operator +(Oilfield_Constant a, Oilfield_Constant b)
        //{
        //    double c1 = a.ValueD;
        //    double c2 = b.ValueD;
        //    return c1 + c2;
        //}

        ///// <summary>
        ///// Subtract a real number
        ///// </summary>
        ///// <param name="a"></param>
        ///// <param name="f"></param>
        ///// <returns></returns>
        //public static double operator -(Oilfield_Constant a, double f)
        //{
        //    double c = a.ValueD;
        //    return c - f;
        //}

        ///// <summary>
        ///// Subtract from a real number
        ///// </summary>
        ///// <param name="f"></param>
        ///// <param name="a"></param>
        ///// <returns></returns>
        //public static double operator -(double f, Oilfield_Constant a)
        //{
        //    double c = a.ValueD;
        //    return f - c;
        //}

        ///// <summary>
        ///// Subtract two constants
        ///// </summary>
        ///// <param name="a"></param>
        ///// <param name="b"></param>
        ///// <returns></returns>
        //public static double operator -(Oilfield_Constant a, Oilfield_Constant b)
        //{
        //    double c1 = a.ValueD;
        //    double c2 = b.ValueD;
        //    return c1 - c2;
        //}

        ///// <summary>
        ///// Multiply by a real number
        ///// </summary>
        ///// <param name="a"></param>
        ///// <param name="f"></param>
        ///// <returns></returns>
        //public static double operator *(Oilfield_Constant a, double f)
        //{
        //    double c = a.ValueD;
        //    return c * f;
        //}

        ///// <summary>
        ///// Multiply a real number
        ///// </summary>
        ///// <param name="f"></param>
        ///// <param name="a"></param>
        ///// <returns></returns>
        //public static double operator *(double f, Oilfield_Constant a)
        //{
        //    double c = a.ValueD;
        //    return c * f;
        //}

        ///// <summary>
        ///// Multiply two constants
        ///// </summary>
        ///// <param name="a"></param>
        ///// <param name="b"></param>
        ///// <returns></returns>
        //public static double operator *(Oilfield_Constant a, Oilfield_Constant b)
        //{
        //    double c1 = a.ValueD;
        //    double c2 = b.ValueD;
        //    return c1 * c2;
        //}

        ///// <summary>
        ///// Divide by a real number
        ///// </summary>
        ///// <param name="a"></param>
        ///// <param name="f"></param>
        ///// <returns></returns>
        //public static double operator /(Oilfield_Constant a, double f)
        //{
        //    double c = a.ValueD;
        //    return c / f;
        //}

        ///// <summary>
        ///// Divide by real number
        ///// </summary>
        ///// <param name="f"></param>
        ///// <param name="a"></param>
        ///// <returns></returns>
        //public static double operator /(double f, Oilfield_Constant a)
        //{
        //    double c = a.ValueD;
        //    return f / c;
        //}

        ///// <summary>
        ///// Divide two constants
        ///// </summary>
        ///// <param name="a"></param>
        ///// <param name="b"></param>
        ///// <returns></returns>
        //public static double operator /(Oilfield_Constant a, Oilfield_Constant b)
        //{
        //    double c1 = a.ValueD;
        //    double c2 = b.ValueD;
        //    return c1 / c2;
        //}

        ///// <summary>
        ///// Determine whether two constants are almost (i.e. within the tolerance) equivalent.
        ///// </summary>
        ///// <param name="a"></param>
        ///// <param name="b"></param>
        ///// <param name="tolerance"></param>
        ///// <returns></returns>
        //static public bool IsEqual(object a, object b, double tolerance)
        //{
        //    if (a == null && b == null) return true;
        //    if (a == null) return false;
        //    if (b == null) return false;
        //    if (a is Oilfield_Constant && b is Oilfield_Constant)
        //    {
        //        Oilfield_Constant a1 = (Oilfield_Constant)a;
        //        Oilfield_Constant b1 = (Oilfield_Constant)b;
        //        if (Math.Abs(a1 - b1) < tolerance) return true;
        //        return false;
        //    }
        //    if (a is Oilfield_Constant && b is double)
        //    {
        //        Oilfield_Constant a1 = (Oilfield_Constant)a;
        //        double b1 = (double)b;
        //        if (Math.Abs(a1 - b1) < tolerance) return true;
        //        return false;
        //    }
        //    if (a is double && b is Oilfield_Constant)
        //    {
        //        double a1 = (double)a;
        //        Oilfield_Constant b1 = (Oilfield_Constant)b;
        //        if (Math.Abs(a1 - b1) < tolerance) return true;
        //        return false;
        //    }
        //    if (a is double && b is double)
        //    {
        //        double a1 = (double)a;
        //        double b1 = (double)b;
        //        if (Math.Abs(a1 - b1) < tolerance) return true;
        //        return false;
        //    }
        //    return false;
        //}
        #endregion
    }
}
