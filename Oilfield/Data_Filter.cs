using System;
using System.Collections.Generic;
using System.Text;
using Petronode.OilfieldFileAccess;

namespace Petronode.Oilfield
{
    public class Data_Filter
    {
        private Oilfield_File m_src = null;

        public double DataStart = 0.0;
        public double DataEnd = 0.0;
        public int DataStartIndex = 0;
        public int DataEndIndex = 0;

        /// <summary>
        /// Constructor. Crates a filter class. Note: the filter is performed on memory only.
        /// </summary>
        /// <param name="source">File to take information from</param>
        public Data_Filter(Oilfield_File source)
        {
            m_src = source;
        }

        #region Gunning Filtering
        /// <summary>
        /// Filter the channel using Ganning Algorithm 
        /// </summary>
        /// <param name="name">channel name in the source file</param>
        /// <param name="n">half-size of filter</param>
        /// <param name="k">number of rejected values</param>
        public void FilterChannelGunning(string name, int n, int k)
        {
            Oilfield_Channel src_index = m_src.GetIndex();
            if (src_index == null) return;
            Oilfield_Channel src_channel = m_src.GetChannel(name);
            if (src_channel == null) return;
            
            // locate upper and lower boundaries
            DataStart = Double.NaN;
            DataEnd = Double.NaN;
            DataStartIndex = -1;
            DataEndIndex = -1;
            if (!src_channel.LocateDataBoundaries(src_index)) return;
            DataStart = src_channel.DataStart;
            DataEnd = src_channel.DataEnd;
            DataStartIndex = src_channel.DataStartIndex;
            DataEndIndex = src_channel.DataEndIndex;

            // perform Gunning filtering
            int l = src_channel.Data.Count;
            List<double> buff = new List<double>(l);
            buff.AddRange( src_channel.Data);
            for (int i = 0; i < l; i++)
            {
                src_channel.Data[i] = Double.NaN;
                if (i < DataStartIndex) continue;
                if (i > DataEndIndex) continue;
                List<double> tmp = new List<double>( (n<<1) + 1);
                for (int j = i - n; j <= i + n; j++)
                {
                    if (j < 0 || j >= l) continue;
                    if (Double.IsNaN(buff[j])) continue;
                    tmp.Add(buff[j]);
                }
                if (tmp.Count <= 0) continue;
                tmp.Sort();
                int kk = k;
                while (tmp.Count < (kk + 5 + kk)) kk--;
                double avearge = 0.0;
                double avearge_count = 0.0;
                for (int j = kk; j < tmp.Count - kk; j++)
                {
                    avearge += tmp[j];
                    avearge_count += 1.0;
                }
                if (avearge_count <= 0.0) continue;
                src_channel.Data[i] = avearge / avearge_count;
            }
        }

        /// <summary>
        /// Performs Gunning filtering on all channels
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        public void FilterChannelsGunning( int n, int k)
        {
            for (int i = 1; i < m_src.Channels.Count; i++)
                FilterChannelGunning(m_src.Channels[i].Name, n, k);
        }
        #endregion

        #region Interpolation
        /// <summary>
        /// Interpolate the channel
        /// </summary>
        /// <param name="name">channel name in the source file</param>
        public void InterpolateChannel(string name)
        {
            InterpolateChannelRaw(name, false);
        }

        /// <summary>
        /// Interpolate the channel as Azimuth (360 degree wrap-around)
        /// </summary>
        /// <param name="name">channel name in the source file</param>
        public void InterpolateChannelAsAzimuth(string name)
        {
            InterpolateChannelRaw(name, true);
        }

        /// <summary>
        /// Interpolatesd all channels in the set
        /// </summary>
        public void InterpolateChannels()
        {
            for (int i = 1; i < m_src.Channels.Count; i++)
                InterpolateChannelRaw(m_src.Channels[i].Name, false);
        }

        /// <summary>
        /// Interpolate the channel
        /// </summary>
        /// <param name="name">channel name in the source file</param>
        /// <param name="isAzimuth">st to true is wrap-around is needed</param>
        private void InterpolateChannelRaw(string name, bool isAzimuth)
        {
            Oilfield_Channel src_index = m_src.GetIndex();
            if (src_index == null) return;
            Oilfield_Channel src_channel = m_src.GetChannel(name);
            if (src_channel == null) return;

            // locate upper and lower boundaries
            DataStart = Double.NaN;
            DataEnd = Double.NaN;
            DataStartIndex = -1;
            DataEndIndex = -1;
            if (!src_channel.LocateDataBoundaries(src_index)) return;
            DataStart = src_channel.DataStart;
            DataEnd = src_channel.DataEnd;
            DataStartIndex = src_channel.DataStartIndex;
            DataEndIndex = src_channel.DataEndIndex;

            // perform interpolation for missing lines
            for (int i = 0; i < src_channel.Data.Count; i++)
            {
                if (!Double.IsNaN(src_channel.Data[i])) continue;
                if (i < DataStartIndex) continue;
                if (i > DataEndIndex) continue;
                double d = Convert.ToDouble(i);
                double depth1 = 0.0;
                double a1 = 0.0;
                for (int j = i; j >= 0; j--)
                {
                    if (Double.IsNaN(src_channel.Data[j])) continue;
                    depth1 = Convert.ToDouble(j);
                    a1 = src_channel.Data[j];
                    break;
                }
                double depth2 = 1.0;
                double a2 = 0.0;
                for (int j = i; j < src_channel.Data.Count; j++)
                {
                    if (Double.IsNaN(src_channel.Data[j])) continue;
                    depth2 = Convert.ToDouble(j);
                    a2 = src_channel.Data[j];
                    break;
                }
                double dd = depth2 - depth1;
                if (Math.Abs(dd) < 0.001) continue;
                double w1 = (depth2 - d) / dd;
                double w2 = (d - depth1) / dd;
                if (isAzimuth)
                {
                    if (a1 > 270.0 && a2 < 90.0) a1 -= 360.0;
                    if (a1 < 90.0 && a2 > 270.0) a2 -= 360.0;
                }
                src_channel.Data[i] = w1 * a1 + w2 * a2;
                if (isAzimuth)
                {
                    while (src_channel.Data[i] < 0.0) src_channel.Data[i] += 360.0;
                    while (src_channel.Data[i] >= 360.0) src_channel.Data[i] -= 360.0;
                }
            }
        }
        #endregion

        #region Despiking
        /// <summary>
        /// Despike the channel from lower outliers 
        /// </summary>
        /// <param name="name">channel name in the source file</param>
        /// <param name="threshold">relative value of threshold</param>
        public void DespikeFromLow(string name, double threshold)
        {
            Oilfield_Channel src_index = m_src.GetIndex();
            if (src_index == null) return;
            Oilfield_Channel src_channel = m_src.GetChannel(name);
            if (src_channel == null) return;

            // locate upper and lower boundaries and determine threshold
            DataStart = Double.NaN;
            DataEnd = Double.NaN;
            DataStartIndex = -1;
            DataEndIndex = -1;
            if (!src_channel.LocateDataBoundaries(src_index)) return;
            DataStart = src_channel.DataStart;
            DataEnd = src_channel.DataEnd;
            DataStartIndex = src_channel.DataStartIndex;
            DataEndIndex = src_channel.DataEndIndex;
            double tr = src_channel.Average * threshold;

            // remove all values below threshhold
            for (int i = 0; i < src_channel.Data.Count; i++)
            {
                double d = src_channel.Data[i];
                if (Double.IsNaN(d)) continue;
                if (d < tr) src_channel.Data[i] = Double.NaN;
            }

            // compute average of the remaining and fill the gaps
            double avr = src_channel.GetAverage(DataStartIndex, DataEndIndex);
            for (int i = 0; i < src_channel.Data.Count; i++)
            {
                if (i < DataStartIndex || i > DataEndIndex) continue;
                if (Double.IsNaN(src_channel.Data[i])) src_channel.Data[i] = avr;
            }
        }
        #endregion
    }
}
