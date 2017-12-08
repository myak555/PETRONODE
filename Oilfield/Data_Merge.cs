using System;
using System.Collections.Generic;
using System.Text;
using Petronode.OilfieldFileAccess;

namespace Petronode.Oilfield
{
    public class Data_Merge
    {
        public const int LinearInterpolate = 0;
        public const int InverseDistanceAverage = 1;
        public const int InverseSquareDistanceAverage = 2;
        public const int GaussianAverage = 3;
        public const int LinearInterpolateAngle = 4;
        public const int AverageAngle = 5;
        public const int ClosestPoint = 6;
        public const int StepFromAbove = 7;
        public const int StepFromBelow = 8;
        public const int LogarithmicAverage = 9;
        public const int HarmonicAverage = 10;

        private Oilfield_File m_src = null;
        private Oilfield_File m_dst = null;
        private Channel_Resample m_Resampler = null;

        /// <summary>
        /// Constructor. Crates a merge class. Note: the merge is performed on memory only.
        /// </summary>
        /// <param name="source">File to take information from</param>
        /// <param name="destination">File to save the information into</param>
        public Data_Merge(Oilfield_File source, Oilfield_File destination)
        {
            m_src = source;
            m_dst = destination;
            double step = 0.0;
            double step2 = 0.0;
            try
            {
                step = Math.Abs(Convert.ToDouble(m_dst.GetConstant("STEP")));
                step2 = Math.Abs(Convert.ToDouble(m_src.GetConstant("STEP")));
            }
            catch (Exception) { }
            if (step < 0.001) step = 0.001;
            Oilfield_Channel src_Index = source.GetIndex();
            Oilfield_Channel dest_Index = destination.GetIndex();
            if (step2 < 0.001 && src_Index.Data.Count > 2)
                step2 = Math.Abs(src_Index.Data[1] - src_Index.Data[0]);
            step2 = 2.0 * Math.Max( step, step2);
            m_Resampler = new Channel_Resample(src_Index, dest_Index, step * 0.5, step2);
        }

        /// <summary>
        /// Extract the channel from source and merges to the destination
        /// </summary>
        /// <param name="name">channel name in the source file</param>
        public void MergeChannel(string name, int method)
        {
            Oilfield_Channel src_channel = m_src.GetChannel(name);
            if (src_channel == null) return;
            Oilfield_Channel dst_channel = m_dst.GetChannel(name);
            if (dst_channel == null)
            {
                dst_channel = m_dst.GetNewChannel(src_channel);
                m_dst.Channels.Add(dst_channel);
            }
            else
            {
                dst_channel.SetData(Double.NaN);
            }
            m_Resampler.Resample(src_channel, dst_channel, method);
        }
    }
}
