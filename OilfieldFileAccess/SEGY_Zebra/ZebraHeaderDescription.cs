using System;
using System.Collections.Generic;
using System.Text;
using Petronode.OilfieldFileAccess;
using Petronode.OilfieldFileAccess.Converters;

namespace Petronode.OilfieldFileAccess.SEGY
{
    public class ZebraHeaderDescription
    {
        public static string[] c_Headers = {
            "01.Trace No",
            "02.Sequence No",
            "03.Org field RSN",
            "04.OFR Trace No",
            "05.CDP No",
            "06.CDP Trace No",
            "07.ID code",
            "08.Num stacked",
            "09.Data Use",
            "10.SRC-REC",
            "11.VD wrt SRD",
            "12.Surface Elev.",
            "13.SRC depth",
            "14.Elev scalar",
            "15.Coord scalar",
            "16.Surface X",
            "17.Surface Y",
            "18.Downhole X",
            "19.Downhole Y",
            "20.Coord Units",
            "21.Start Delay",
            "22.Mute start",
            "23.Mute end",
            "24.Samp/Trace",
            "25.Samp Int (uS)",
            "26.Us(SRC-MON)",
            "27.Us(HYD-DWN)",
            "28.Us(DAT-DWN)",
            "29.Us(T0-DWN)",
            "30.Scale Factor",
            "31.SF Power",
            "32.Reject Flag",
            "33.Vp",
            "34.Vs",
            "35.100xDelta-X"
        };

        public static string[] c_Units = {
            "unitless",
            "unitless",
            "unitless",
            "unitless",
            "unitless",
            "unitless",
            "unitless",
            "unitless",
            "unitless",
            "elevation",
            "elevation",
            "elevation",
            "elevation",
            "unitless",
            "unitless",
            "coordinate",
            "coordinate",
            "coordinate",
            "coordinate",
            "unitless",
            "ms",
            "ms",
            "ms",
            "unitless",
            "unitless",
            "ms",
            "ms",
            "ms",
            "ms",
            "unitless",
            "unitless",
            "unitless",
            "velocity",
            "velocity",
            "unitless"
        };

        public static int[] c_Offsets ={
            0,
            4,
            8,
            12,
            20,
            24,
            28,
            30,
            34,
            36,
            40,
            44,
            48,
            68,
            70,
            72,
            76,
            80,
            84,
            88,
            108,
            110,
            112,
            114,
            116,
            184,
            188,
            192,
            196,
            200,
            202,
            204,
            206,
            208,
            210
        };

        public static int[] c_Sizes ={
            4,
            4,
            4,
            4,
            4,
            4,
            2,
            2,
            2,
            4,
            4,
            4,
            4,
            2,
            2,
            4,
            4,
            4,
            4,
            2,
            2,
            2,
            2,
            2,
            2,
            4,
            4,
            4,
            4,
            2,
            2,
            2,
            2,
            2,
            2
        };

        /// <summary>
        /// Gets value in the trace header as string
        /// </summary>
        /// <param name="position">position in the header as per Zebra convention</param>
        /// <param name="buffer">buffer to get data from</param>
        public static string GetValue(int position, byte[] buffer)
        {
            position--;
            NumberUnion nu = new NumberUnion();
            if (c_Sizes[position] == 2)
            {
                short t = BufferConverter.GetBytesInt16_BE(buffer, nu, c_Offsets[position]);
                return t.ToString("0");
            }
            if (c_Sizes[position] == 4)
            {
                int t = BufferConverter.GetBytesInt32_BE(buffer, nu, c_Offsets[position]);
                return t.ToString("0");
            }
            return "0";
        }

        /// <summary>
        /// Gets value in the trace header as int
        /// </summary>
        /// <param name="position">position in the header as per Zebra convention</param>
        /// <param name="buffer">buffer to get data from</param>
        public static int GetIntValue(int position, byte[] buffer)
        {
            position--;
            NumberUnion nu = new NumberUnion();
            if (c_Sizes[position] == 2)
            {
                short t = BufferConverter.GetBytesInt16_BE(buffer, nu, c_Offsets[position]);
                return Convert.ToInt32( t);
            }
            if (c_Sizes[position] == 4)
            {
                int t = BufferConverter.GetBytesInt32_BE(buffer, nu, c_Offsets[position]);
                return t;
            }
            return 0;
        }

        /// <summary>
        /// Gets elevation value in the trace header as float
        /// Field 14 is used for scale
        /// </summary>
        /// <param name="position">position in the header as per Zebra convention</param>
        /// <param name="buffer">buffer to get data from</param>
        public static float GetElevationValue(int position, byte[] buffer)
        {
            float f = Convert.ToSingle(GetIntValue(position, buffer));
            int scale = GetIntValue(14, buffer);
            if (scale > 1) return f * Convert.ToSingle(scale);
            if (scale < -1) return f / Convert.ToSingle(-scale);
            return f;
        }

        /// <summary>
        /// Gets coordinate value in the trace header as float
        /// Field 15 is uded for scale
        /// </summary>
        /// <param name="position">position in the header as per Zebra convention</param>
        /// <param name="buffer">buffer to get data from</param>
        public static float GetCoordinateValue(int position, byte[] buffer)
        {
            float f = Convert.ToSingle(GetIntValue(position, buffer));
            int scale = GetIntValue(15, buffer);
            if (scale > 1) return f * Convert.ToSingle(scale);
            if (scale < -1) return f / Convert.ToSingle(-scale);
            return f;
        }

        /// <summary>
        /// Gets coordinate value in the trace header as float
        /// File numbers are intsa in micro-seconds
        /// </summary>
        /// <param name="position">position in the header as per Zebra convention</param>
        /// <param name="buffer">buffer to get data from</param>
        public static float GetTimeValue(int position, byte[] buffer)
        {
            float f = Convert.ToSingle(GetIntValue(position, buffer));
            return f/1000f;
        }

        /// <summary>
        /// Sets value in the trace header
        /// </summary>
        /// <param name="val">value as string</param>
        /// <param name="position">position in the header as per Zebra convention</param>
        /// <param name="buffer">buffer to get data from</param>
        public static void SetValue(string val, int position, byte[] buffer) 
        {
            position--;
            NumberUnion nu = new NumberUnion();
            if (c_Sizes[position] == 2)
            {
                short t = 0;
                try
                {
                    t = Convert.ToInt16(val);
                }
                catch (Exception) { }
                BufferConverter.SetBytesInt16_BE( buffer, nu, t, c_Offsets[position]);
            }
            if (c_Sizes[position] == 4)
            {
                int t = 0;
                try
                {
                    t = Convert.ToInt32(val);
                }
                catch (Exception) { }
                BufferConverter.SetBytesInt32_BE(buffer, nu, t, c_Offsets[position]);
            }
        }

        /// <summary>
        /// Sets value in the trace header
        /// </summary>
        /// <param name="val">value as int</param>
        /// <param name="position">position in the header as per Zebra convention</param>
        /// <param name="buffer">buffer to get data from</param>
        public static void SetIntValue(int val, int position, byte[] buffer)
        {
            position--;
            NumberUnion nu = new NumberUnion();
            if (c_Sizes[position] == 2)
            {
                short t = 0;
                try
                {
                    t = Convert.ToInt16(val);
                }
                catch (Exception) { }
                BufferConverter.SetBytesInt16_BE(buffer, nu, t, c_Offsets[position]);
            }
            if (c_Sizes[position] == 4)
            {
                BufferConverter.SetBytesInt32_BE(buffer, nu, val, c_Offsets[position]);
            }
        }

        /// <summary>
        /// Sets elevation value in the trace header
        /// Field 14 is used for scaling
        /// </summary>
        /// <param name="val">elevation</param>
        /// <param name="position">position as per Zebra convention</param>
        /// <param name="buffer">buffer to get data from</param>
        public static void SetElevationValue(float val, int position, byte[] buffer)
        {
            int scale = GetIntValue(14, buffer);
            if (scale > 1) val /= Convert.ToSingle(scale);
            if (scale < -1) val *= Convert.ToSingle(-scale);
            SetIntValue(Convert.ToInt32(val), position, buffer);
        }

        /// <summary>
        /// Sets coordinate value in the trace header
        /// Field 15 is used for scaling
        /// </summary>
        /// <param name="val">coordinate</param>
        /// <param name="position">position as per Zebra convention</param>
        /// <param name="buffer">buffer to get data from</param>
        public static void SetCoordinateValue(float val, int position, byte[] buffer)
        {
            int scale = GetIntValue(15, buffer);
            if (scale > 1) val /= Convert.ToSingle(scale);
            if (scale < -1) val *= Convert.ToSingle(-scale);
            SetIntValue(Convert.ToInt32(val), position, buffer);
        }

        /// <summary>
        /// Sets time value in the trace header
        /// time is always integer in micro-seconds
        /// </summary>
        /// <param name="val">time</param>
        /// <param name="position">position as per Zebra convention</param>
        /// <param name="buffer">buffer to get data from</param>
        public static void SetTimeValue(float val, int position, byte[] buffer)
        {
            SetIntValue(Convert.ToInt32(val * 1000f), position, buffer);
        }

        /// <summary>
        /// Converts buffer float trace
        /// </summary>
        /// <param name="buffer">buffer to get data from</param>
        /// <returns>Float trace</returns>
        public static float[] GetTraceData( byte[] buffer)
        {
            float[] tmp = new float[(buffer.Length - 240) / 4];
            NumberUnion nu = new NumberUnion();
            for (int i = 0, j = 240; i < tmp.Length; i++, j+=4)
            {
                tmp[i] = BufferConverter.GetBytesIBM(buffer, nu, j);
            }
            return tmp;
        }

        /// <summary>
        /// Sets buffer to float trace
        /// </summary>
        /// <param name="trace">trace as floating points</param>
        /// <param name="buffer">buffer to get data from</param>
        public static void SetTraceData(float[] trace, byte[] buffer)
        {
            NumberUnion nu = new NumberUnion();
            for (int i = 0, j = 240; i < trace.Length; i++, j += 4)
            {
                BufferConverter.SetBytesIBM(buffer, nu, trace[i], j);
            }
        }
    }
}
