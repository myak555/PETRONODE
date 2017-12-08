using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Petronode.OilfieldFileAccess;

namespace Petronode.OilfieldFileAccess.SEGY
{

    /// <summary>
    /// Descrives SEG-Y "trace"
    /// </summary>
    public class SEGY_Channel:Oilfield_Channel
    {
        public byte[] Buffer = null;

        /// <summary>
        /// Constructor - creates a default variable
        /// </summary>
        public SEGY_Channel(): base()
        {
            PaddedWidth = 0;
        }

        /// <summary>
        /// Constructor - creates from the parameters
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="unit">Variable unit</param>
        public SEGY_Channel(string name, string unit)
            : base(name, unit, "Seismic Trace")
        {
            PaddedWidth = 0;
        }

        /// <summary>
        /// Constructor - creates from the parameters
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="unit">Variable unit</param>
        /// <param name="buffer">Buffer to parse</param>
        /// <param name="readdata">if set to true, the data is also parsed</param>
        public SEGY_Channel(string name, string unit, byte[] buffer, bool readdata)
            : base(name, unit, "Seismic Trace")
        {
            PaddedWidth = 0;
            for (int i = 0; i < ZebraHeaderDescription.c_Headers.Length; i++)
            {
                string value = "0";
                name = ZebraHeaderDescription.c_Headers[i];
                unit = ZebraHeaderDescription.c_Units[i];
                switch( unit)
                {
                    case "elevation":
                        value = ZebraHeaderDescription.GetElevationValue( i + 1, buffer).ToString( "0.000");
                        break;
                    case "coordinate":
                        value = ZebraHeaderDescription.GetCoordinateValue(i + 1, buffer).ToString("0.00");
                        break;
                    case "ms":
                        value = ZebraHeaderDescription.GetTimeValue(i + 1, buffer).ToString("0.0");
                        break;
                    case "unitless":
                    default:
                        value = ZebraHeaderDescription.GetValue(i + 1, buffer);
                        break;
                }
                SEGY_Constant c = new SEGY_Constant(name, unit, value, "");
                Parameters.Add(c);
            }
            if (!readdata) return;
            Buffer = new byte[ buffer.Length];
            Array.Copy(buffer, Buffer, Buffer.Length); 
            float[] Trace = ZebraHeaderDescription.GetTraceData(buffer);
            foreach (float f in Trace) Data.Add(Convert.ToDouble(f));
        }

        /// <summary>
        /// Converts back to LAS file string
        /// </summary>
        /// <returns>Converted string</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name + ",");
            sb.Append(Unit);
            string s = sb.ToString().Trim();
            while( s.EndsWith(",")) s = s.Substring(0, s.Length-1);
            return s;
        }

        /// <summary>
        /// Creates a deep copy of a given channel
        /// </summary>
        /// <param name="c"></param>
        public override Oilfield_Channel Clone()
        {
            SEGY_Channel tmp = new SEGY_Channel();
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
        /// Returns a trace
        /// </summary>
        /// <returns></returns>
        public float[] GetTrace()
        {
            return ZebraHeaderDescription.GetTraceData(Buffer);
        }

        /// <summary>
        /// Writes trace as a binary
        /// </summary>
        /// <param name="fs"></param>
        public override void Write(FileStream fs)
        {
            if (Buffer != null)
            {
                for (int i = 0; i < Parameters.Count; i++)
                {
                    SEGY_Constant c = (SEGY_Constant)Parameters[i];
                    string unit = ZebraHeaderDescription.c_Units[i];
                    switch (unit)
                    {
                        case "elevation":
                            ZebraHeaderDescription.SetElevationValue((float)c.ValueD, i + 1, Buffer);
                            break;
                        case "coordinate":
                            ZebraHeaderDescription.SetCoordinateValue((float)c.ValueD,i + 1, Buffer);
                            break;
                        case "ms":
                            ZebraHeaderDescription.SetTimeValue( (float)c.ValueD, i + 1, Buffer);
                            break;
                        case "unitless":
                        default:
                            ZebraHeaderDescription.SetValue( c.Value, i + 1, Buffer);
                            break;
                    }
                }
                fs.Write(Buffer, 0, Buffer.Length);
            }
        }
    }
}
