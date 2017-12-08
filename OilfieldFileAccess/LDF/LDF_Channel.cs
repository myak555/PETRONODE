using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Petronode.OilfieldFileAccess;
using Petronode.OilfieldFileAccess.Converters;

namespace Petronode.OilfieldFileAccess.LDF
{

    /// <summary>
    /// Descrives SEG-Y "trace"
    /// </summary>
    public class LDF_Channel:Oilfield_Channel
    {
        public byte[] Buffer = null;

        /// <summary>
        /// Constructor - creates a default variable
        /// </summary>
        public LDF_Channel(): base()
        {
            PaddedWidth = 0;
        }

        /// <summary>
        /// Constructor - creates from the parameters
        /// </summary>
        /// <param name="parent">Parent LDF file</param>
        /// <param name="name">Variable name</param>
        /// <param name="unit">Variable unit</param>
        public LDF_Channel(LDF_File parent, string name, string unit)
            : base(name, unit, "Seismic Trace")
        {
            PaddedWidth = 0;
            string UndefFloat = parent.UndefinedFloat.ToString();
            string UndefInt = parent.UndefinedInt.ToString();
            Parameters.Add(
                new LDF_Constant("Source Easting", "m", 0, 4, LDF_Constant.Float32, UndefFloat, "Actual source Easting"));
            Parameters.Add(
                new LDF_Constant("Source Northing", "m", 4, 4, LDF_Constant.Float32, UndefFloat, "Actual source Northing"));
            Parameters.Add(
                new LDF_Constant("Source Depth", "m", 8, 4, LDF_Constant.Float32, UndefFloat, "Actual source Depth from SRD"));
            Parameters.Add(
                new LDF_Constant("Source Easting Nominal", "m", 12, 4, LDF_Constant.Float32, UndefFloat, "Nominal source Easting"));
            Parameters.Add(
                new LDF_Constant("Source Northing Nominal", "m", 16, 4, LDF_Constant.Float32, UndefFloat, "Nominal source Northing"));
            Parameters.Add(
                new LDF_Constant("Source Depth Nominal", "m", 20, 4, LDF_Constant.Float32, UndefFloat, "Nominal source Depth from SRD"));
            Parameters.Add(
                new LDF_Constant("Source Line Easting", "m", 24, 4, LDF_Constant.Float32, UndefFloat, "Actual source line Easting"));
            Parameters.Add(
                new LDF_Constant("Source Line Northing", "m", 28, 4, LDF_Constant.Float32, UndefFloat, "Actual source line Northing"));
            Parameters.Add(
                new LDF_Constant("Source Line Depth", "m", 32, 4, LDF_Constant.Float32, UndefFloat, "Actual source line Depth from SRD"));
            Parameters.Add(
                new LDF_Constant("Source Line Easting Nominal", "m", 36, 4, LDF_Constant.Float32, UndefFloat, "Nominal source line Easting"));
            Parameters.Add(
                new LDF_Constant("Source Line Northing Nominal", "m", 40, 4, LDF_Constant.Float32, UndefFloat, "Nominal source line Northing"));
            Parameters.Add(
                new LDF_Constant("Source Line Depth Nominal", "m", 44, 4, LDF_Constant.Float32, UndefFloat, "Nominal source line Depth from SRD"));
            Parameters.Add(
                new LDF_Constant("Source Orientation Easting", "", 48, 4, LDF_Constant.Float32, UndefFloat, "Source orientation Easting"));
            Parameters.Add(
                new LDF_Constant("Source Orientation Northing", "", 52, 4, LDF_Constant.Float32, UndefFloat, "Source orientation Northing"));
            Parameters.Add(
                new LDF_Constant("Source Orientation Depth", "", 56, 4, LDF_Constant.Float32, UndefFloat, "Source orientation Depth"));
            Parameters.Add(
                new LDF_Constant("Source Sensor Easting", "m", 60, 4, LDF_Constant.Float32, UndefFloat, "Actual source sensor Easting"));
            Parameters.Add(
                new LDF_Constant("Source Sensor Northing", "m", 64, 4, LDF_Constant.Float32, UndefFloat, "Actual source sensor Northing"));
            Parameters.Add(
                new LDF_Constant("Source Sensor Depth", "m", 68, 4, LDF_Constant.Float32, UndefFloat, "Actual source sensor Depth from SRD"));
            Parameters.Add(
                new LDF_Constant("Receiver Easting", "m", 72, 4, LDF_Constant.Float32, UndefFloat, "Actual receiver Easting"));
            Parameters.Add(
                new LDF_Constant("Receiver Northing", "m", 76, 4, LDF_Constant.Float32, UndefFloat, "Actual receiver Northing"));
            Parameters.Add(
                new LDF_Constant("Receiver Depth", "m", 80, 4, LDF_Constant.Float32, UndefFloat, "Actual receiver Depth from SRD"));
            Parameters.Add(
                new LDF_Constant("Receiver Easting Nominal", "m", 84, 4, LDF_Constant.Float32, UndefFloat, "Nominal receiver Easting"));
            Parameters.Add(
                new LDF_Constant("Receiver Northing Nominal", "m", 88, 4, LDF_Constant.Float32, UndefFloat, "Nominal receiver Northing"));
            Parameters.Add(
                new LDF_Constant("Receiver Depth Nominal", "m", 92, 4, LDF_Constant.Float32, UndefFloat, "Nominal receiver Depth from SRD"));
            Parameters.Add(
                new LDF_Constant("Receiver Line Easting", "m", 96, 4, LDF_Constant.Float32, UndefFloat, "Actual receiver line Easting"));
            Parameters.Add(
                new LDF_Constant("Receiver Line Northing", "m", 100, 4, LDF_Constant.Float32, UndefFloat, "Actual receiver line Northing"));
            Parameters.Add(
                new LDF_Constant("Receiver Line Depth", "m", 104, 4, LDF_Constant.Float32, UndefFloat, "Actual receiver line Depth from SRD"));
            Parameters.Add(
                new LDF_Constant("Receiver Line Easting Nominal", "m", 108, 4, LDF_Constant.Float32, UndefFloat, "Nominal receiver line Easting"));
            Parameters.Add(
                new LDF_Constant("Receiver Line Northing Nominal", "m", 112, 4, LDF_Constant.Float32, UndefFloat, "Nominal receiver line Northing"));
            Parameters.Add(
                new LDF_Constant("Receiver Line Depth Nominal", "m", 116, 4, LDF_Constant.Float32, UndefFloat, "Nominal receiver line Depth from SRD"));
            Parameters.Add(
                new LDF_Constant("Receiver Orientation Easting", "", 120, 4, LDF_Constant.Float32, UndefFloat, "Receiver orientation Easting"));
            Parameters.Add(
                new LDF_Constant("Receiver Orientation Northing", "", 124, 4, LDF_Constant.Float32, UndefFloat, "Receiver orientation Northing"));
            Parameters.Add(
                new LDF_Constant("Receiver Orientation Depth", "", 128, 4, LDF_Constant.Float32, UndefFloat, "Receiver orientation Depth"));
            Parameters.Add(
                new LDF_Constant("Shift", "ms", 132, 4, LDF_Constant.Float32, UndefFloat, "Actial shift"));
            Parameters.Add(
                new LDF_Constant("Shift Nominal", "ms", 136, 4, LDF_Constant.Float32, UndefFloat, "Nominal shift"));
            Parameters.Add(
                new LDF_Constant("Common Midpoint Easting", "m", 140, 4, LDF_Constant.Float32, UndefFloat, "Common midpoint Easting"));
            Parameters.Add(
                new LDF_Constant("Common Midpoint Northing", "m", 144, 4, LDF_Constant.Float32, UndefFloat, "Common midpoint Northing"));
            Parameters.Add(
                new LDF_Constant("Common Midpoint Depth", "m", 148, 4, LDF_Constant.Float32, UndefFloat, "Common midpoint Depth"));
            Parameters.Add(
                new LDF_Constant("Common Midpoint Number", "", 152, 4, LDF_Constant.Int32, UndefInt, "Common midpoint number"));
            Parameters.Add(
                new LDF_Constant("Common Midpoint Offset", "m", 156, 4, LDF_Constant.Float32, UndefFloat, "Common midpoint offset"));
            Parameters.Add(
                new LDF_Constant("Source SRD Easting", "m", 160, 4, LDF_Constant.Float32, UndefFloat, "Actual source to SRD Easting"));
            Parameters.Add(
                new LDF_Constant("Source SRD Northing", "m", 164, 4, LDF_Constant.Float32, UndefFloat, "Actual source to SRD Northing"));
            Parameters.Add(
                new LDF_Constant("Source SRD Depth", "m", 168, 4, LDF_Constant.Float32, UndefFloat, "Actual source to SRD Depth"));
            Parameters.Add(
                new LDF_Constant("Source KB Easting", "m", 172, 4, LDF_Constant.Float32, UndefFloat, "Actual source to KB Easting"));
            Parameters.Add(
                new LDF_Constant("Source KB Northing", "m", 176, 4, LDF_Constant.Float32, UndefFloat, "Actual source to KB Northing"));
            Parameters.Add(
                new LDF_Constant("Source KB Depth", "m", 180, 4, LDF_Constant.Float32, UndefFloat, "Actual source to KB Depth"));
            Parameters.Add(
                new LDF_Constant("Source MSL Easting", "m", 184, 4, LDF_Constant.Float32, UndefFloat, "Actual source to MSL Easting"));
            Parameters.Add(
                new LDF_Constant("Source MSL Northing", "m", 188, 4, LDF_Constant.Float32, UndefFloat, "Actual source to MSL Northing"));
            Parameters.Add(
                new LDF_Constant("Source MSL Depth", "m", 192, 4, LDF_Constant.Float32, UndefFloat, "Actual source to MSL Depth"));
            Parameters.Add(
                new LDF_Constant("Receiver SRD Easting", "m", 196, 4, LDF_Constant.Float32, UndefFloat, "Actual receiver to SRD Easting"));
            Parameters.Add(
                new LDF_Constant("Receiver SRD Northing", "m", 200, 4, LDF_Constant.Float32, UndefFloat, "Actual receiver to SRD Northing"));
            Parameters.Add(
                new LDF_Constant("Receiver SRD Depth", "m", 204, 4, LDF_Constant.Float32, UndefFloat, "Actual receiver to SRD Depth"));
            Parameters.Add(
                new LDF_Constant("Receiver KB Easting", "m", 208, 4, LDF_Constant.Float32, UndefFloat, "Actual receiver to KB Easting"));
            Parameters.Add(
                new LDF_Constant("Receiver KB Northing", "m", 212, 4, LDF_Constant.Float32, UndefFloat, "Actual receiver to KB Northing"));
            Parameters.Add(
                new LDF_Constant("Receiver KB Depth", "m", 216, 4, LDF_Constant.Float32, UndefFloat, "Actual receiver to KB Depth"));
            Parameters.Add(
                new LDF_Constant("Receiver MSL Easting", "m", 220, 4, LDF_Constant.Float32, UndefFloat, "Actual receiver to MSL Easting"));
            Parameters.Add(
                new LDF_Constant("Receiver MSL Northing", "m", 224, 4, LDF_Constant.Float32, UndefFloat, "Actual receiver to MSL Northing"));
            Parameters.Add(
                new LDF_Constant("Receiver MSL Depth", "m", 228, 4, LDF_Constant.Float32, UndefFloat, "Actual receiver to MSL Depth"));
            Parameters.Add(
                new LDF_Constant("Cable Length", "m", 232, 4, LDF_Constant.Float32, UndefFloat, "Along-hole Depth"));
            Parameters.Add(
                new LDF_Constant("Frame Number", "", 236, 4, LDF_Constant.Int32, "26", "Frame is always 26"));
            Parameters.Add(
                new LDF_Constant("Shot Sequence Number", "", 240, 4, LDF_Constant.Int32, "0", "Shot number in file"));
            Parameters.Add(
                new LDF_Constant("Sampling Interval", "ms", 244, 4, LDF_Constant.Float32, "1.0", "Sampling Interval"));
            Parameters.Add(
                new LDF_Constant("Time of First Data", "ms", 248, 4, LDF_Constant.Float32, "0.0", "Time of recording start"));
            Parameters.Add(
                new LDF_Constant("VSP Lib Status", "", 252, 4, LDF_Constant.Int32, "4", "Always 4"));
            Parameters.Add(
                new LDF_Constant("Data ID", "", 256, 4, LDF_Constant.Int32, "64", "Always 64"));
            Parameters.Add(
                new LDF_Constant("Trace Select", "", 260, 4, LDF_Constant.Int32, "1", "Indicates the trace is selected"));
            Parameters.Add(
                new LDF_Constant("Evaluation Code", "", 264, 4, LDF_Constant.Int32, "19", "Always 19"));
            Parameters.Add(
                new LDF_Constant("Summed Traces Number", "", 268, 4, LDF_Constant.Int32, "1", "Shots in stack"));
            Parameters.Add(
                new LDF_Constant("Station Number", "", 272, 4, LDF_Constant.Int32, "0", ""));
            Parameters.Add(
                new LDF_Constant("Level Number", "", 276, 4, LDF_Constant.Int32, "0", ""));
            Parameters.Add(
                new LDF_Constant("Stack Number", "", 280, 4, LDF_Constant.Int32, "0", ""));
            Parameters.Add(
                new LDF_Constant("Tool Number", "", 284, 4, LDF_Constant.Int32, "0", ""));
            Parameters.Add(
                new LDF_Constant("File Number", "", 288, 4, LDF_Constant.Int32, "0", ""));
            Parameters.Add(
                new LDF_Constant("Shot Number", "", 292, 4, LDF_Constant.Int32, "0", ""));
            Parameters.Add(
                new LDF_Constant("Shot Time", "", 296, 4, LDF_Constant.Float32, "0.00", "Recorded as number of days since 01-Jan-1970 00:00"));
            Parameters.Add(
                new LDF_Constant("Break Time", "ms", 300, 4, LDF_Constant.Float32, UndefFloat, "First break - downhole"));
            Parameters.Add(
                new LDF_Constant("Transit Time", "ms", 304, 4, LDF_Constant.Float32, UndefFloat, "Transit time"));
            Parameters.Add(
                new LDF_Constant("Transit Time Nominal", "ms", 308, 4, LDF_Constant.Float32, UndefFloat, "Transit time expected"));
            Parameters.Add(
                new LDF_Constant("Transit Time Initial", "ms", 312, 4, LDF_Constant.Float32, UndefFloat, "Transit time autopick"));
            Parameters.Add(
                new LDF_Constant("Transit Time Accuracy", "ms", 316, 4, LDF_Constant.Float32, UndefFloat, "Transit time accuracy"));
            Parameters.Add(
                new LDF_Constant("Transit Time SRD", "ms", 320, 4, LDF_Constant.Float32, UndefFloat, "Transit time to SRD"));
            Parameters.Add(
                new LDF_Constant("Transit Time NMO", "ms", 324, 4, LDF_Constant.Float32, UndefFloat, "Transit time for NMO"));
            Parameters.Add(
                new LDF_Constant("Transit Time Shear", "ms", 328, 4, LDF_Constant.Float32, UndefFloat, "Transit time for shear"));
            Parameters.Add(
                new LDF_Constant("Transit Time Ampmod", "ms", 332, 4, LDF_Constant.Float32, UndefFloat, "Transit time for amplitude spread"));
            Parameters.Add(
                new LDF_Constant("Transit Time Sensor", "ms", 336, 4, LDF_Constant.Float32, UndefFloat, "Transit time for surface sensor"));
            Parameters.Add(
                new LDF_Constant("Slope", "ms", 340, 4, LDF_Constant.Float32, UndefFloat, "Ampmod spread slope"));
            Parameters.Add(
                new LDF_Constant("Q-Factor", "dB/ms", 344, 4, LDF_Constant.Float32, UndefFloat, "Q factor decrement"));
            Parameters.Add(
                new LDF_Constant("Normalization Factor", "", 348, 4, LDF_Constant.Float32, "1.0", "Normalization"));
            Parameters.Add(
                new LDF_Constant("Trace Min Value", "", 352, 4, LDF_Constant.Float32, "-1.0", "Minimum trace value"));
            Parameters.Add(
                new LDF_Constant("Trace Max Value", "", 356, 4, LDF_Constant.Float32, "1.0", "Maximum trace value"));
            Parameters.Add(
                new LDF_Constant("Relative Bearing Raw", "dega", 360, 4, LDF_Constant.Float32, UndefFloat, "Tool rotation sensor"));
            Parameters.Add(
                new LDF_Constant("Relative Bearing Projection", "dega", 364, 4, LDF_Constant.Float32, UndefFloat, "Tool rotation fix"));
            Parameters.Add(
                new LDF_Constant("Angle P", "dega", 368, 4, LDF_Constant.Float32, UndefFloat, "Approach of P-wave"));
            Parameters.Add(
                new LDF_Constant("Angle S", "dega", 372, 4, LDF_Constant.Float32, UndefFloat, "Approach of S-wave"));
            Parameters.Add(
                new LDF_Constant("Angle Source", "dega", 372, 4, LDF_Constant.Float32, UndefFloat, "Approach of direct ray"));
            Parameters.Add(
                new LDF_Constant("Angle Source Static", "dega", 380, 4, LDF_Constant.Float32, UndefFloat, "Approach of direct ray"));
            Parameters.Add(
                new LDF_Constant("Caliper", "in", 384, 4, LDF_Constant.Float32, UndefFloat, "Hole diameter"));
            //BufferConverter.SetBytesFloat(m_TraceHeader); //388 pad_force
            //BufferConverter.SetBytesFloat(m_TraceHeader); //392 compression_pressure
            //BufferConverter.SetBytesFloat(m_TraceHeader); //396 temperature
            //SetEventsFloats(m_TraceHeader); //400 events 1 to 5
            //SetBytesPointer(m_TraceHeader); //420 trace pointer
            //SetEventsFloats(m_TraceHeader); //424 time 1 to 5
            //BufferConverter.SetBytesFloat(m_TraceHeader); //444 slope
            //BufferConverter.SetBytesFloat(m_TraceHeader); //448 wave_number
            //BufferConverter.SetBytesInt(m_TraceHeader, 0); //452 line_number
            //BufferConverter.SetBytesFloat(m_TraceHeader); //456 hole_azimuth
            //BufferConverter.SetBytesFloat(m_TraceHeader); //460 hole_deviation
            //BufferConverter.SetBytesFloat(m_TraceHeader); //464 arm_azimuth
            //SetEventsFloats(m_TraceHeader); //468 frequency 1 to 5
            //SetEventsFloats(m_TraceHeader); //488 velocity 1 to 5
            //SetEventsFloats(m_TraceHeader); //508 polarization 1 to 5
            //SetEventsFloats(m_TraceHeader); //528 orientation 1 to 5
            //SetEventsFloats(m_TraceHeader); //548 user key 1 to 5
            //SetEventsFloats(m_TraceHeader); //568 used key 6 to 10 -> 588
        }

        /// <summary>
        /// Constructor - creates from the parameters
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="unit">Variable unit</param>
        /// <param name="buffer">Buffer to parse</param>
        /// <param name="readdata">if set to true, the data is also parsed</param>
        public LDF_Channel(string name, string unit, byte[] buffer, bool readdata)
            : base(name, unit, "Seismic Trace")
        {
            //PaddedWidth = 0;
            //for (int i = 0; i < ZebraHeaderDescription.c_Headers.Length; i++)
            //{
            //    string value = "0";
            //    name = ZebraHeaderDescription.c_Headers[i];
            //    unit = ZebraHeaderDescription.c_Units[i];
            //    switch( unit)
            //    {
            //        case "elevation":
            //            value = ZebraHeaderDescription.GetElevationValue( i + 1, buffer).ToString( "0.000");
            //            break;
            //        case "coordinate":
            //            value = ZebraHeaderDescription.GetCoordinateValue(i + 1, buffer).ToString("0.00");
            //            break;
            //        case "ms":
            //            value = ZebraHeaderDescription.GetTimeValue(i + 1, buffer).ToString("0.0");
            //            break;
            //        case "unitless":
            //        default:
            //            value = ZebraHeaderDescription.GetValue(i + 1, buffer);
            //            break;
            //    }
            //    LDF_Constant c = new LDF_Constant(name, unit, value, "");
            //    Parameters.Add(c);
            //}
            //if (!readdata) return;
            //Buffer = new byte[ buffer.Length];
            //Array.Copy(buffer, Buffer, Buffer.Length); 
            //float[] Trace = ZebraHeaderDescription.GetTraceData(buffer);
            //foreach (float f in Trace) Data.Add(Convert.ToDouble(f));
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
            LDF_Channel tmp = new LDF_Channel();
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
            //return ZebraHeaderDescription.GetTraceData(Buffer);
            return null;
        }

        /// <summary>
        /// Writes trace as a binary
        /// </summary>
        /// <param name="fs"></param>
        public override void Write(FileStream fs)
        {
            //if (Buffer != null)
            //{
            //    for (int i = 0; i < Parameters.Count; i++)
            //    {
            //        LDF_Constant c = (LDF_Constant)Parameters[i];
            //        string unit = ZebraHeaderDescription.c_Units[i];
            //        switch (unit)
            //        {
            //            case "elevation":
            //                ZebraHeaderDescription.SetElevationValue((float)c.ValueD, i + 1, Buffer);
            //                break;
            //            case "coordinate":
            //                ZebraHeaderDescription.SetCoordinateValue((float)c.ValueD,i + 1, Buffer);
            //                break;
            //            case "ms":
            //                ZebraHeaderDescription.SetTimeValue( (float)c.ValueD, i + 1, Buffer);
            //                break;
            //            case "unitless":
            //            default:
            //                ZebraHeaderDescription.SetValue( c.Value, i + 1, Buffer);
            //                break;
            //        }
            //    }
            //    fs.Write(Buffer, 0, Buffer.Length);
            //}
        }
    }
}
