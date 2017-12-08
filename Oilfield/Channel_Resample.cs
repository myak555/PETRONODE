using System;
using System.Collections.Generic;
using System.Text;
using Petronode.OilfieldFileAccess;

namespace Petronode.Oilfield
{
    public class Channel_Resample
    {
        public static string[] Methods = { 
            "Linear Interpolate",
            "Inverse Distance Average",
            "Inverse Square Distance Average",
            "Gaussian Average",
            "Linear Interpolate Angle",
            "Average Angle",
            "Closest Point",
            "Step From Above",
            "Step From Below",
            "Logarithmic Average",
            "Harmonic Average"};
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

        Oilfield_Channel m_DstIndex = null; 
        List<Channel_Resample_Map> m_mapping = new List<Channel_Resample_Map>();

        /// <summary>
        /// Creates an Resample engine (with the processing map)
        /// </summary>
        /// <param name="srcIndex">Source index</param>
        /// <param name="dstIndex">Destimation index</param>
        public Channel_Resample(Oilfield_Channel srcIndex, Oilfield_Channel dstIndex)
        {
            m_DstIndex = dstIndex;
            double minWindow = Double.MaxValue;
            double maxWindow = Double.MinValue;
            for (int i = 1; i < srcIndex.Data.Count; i++)
            {
                if (Double.IsNaN(srcIndex.Data[i]) || Double.IsNaN(srcIndex.Data[i - 1])) continue;
                double d = Math.Abs(srcIndex.Data[i] - srcIndex.Data[i - 1]);
                if (d > maxWindow) maxWindow = d;
            }
            for (int i = 1; i < dstIndex.Data.Count; i++)
            {
                if (Double.IsNaN(dstIndex.Data[i]) || Double.IsNaN(dstIndex.Data[i - 1])) continue;
                double d = Math.Abs(dstIndex.Data[i] - dstIndex.Data[i - 1]);
                if (d < minWindow) minWindow = d;
            }
            minWindow /= 2.0;
            maxWindow *= 2.0;
            if (maxWindow < minWindow) maxWindow = minWindow;
            for (int i = 0; i < dstIndex.Data.Count; i++)
                m_mapping.Add(new Channel_Resample_Map(srcIndex, dstIndex.Data[i], minWindow, maxWindow));
        }
        
        /// <summary>
        /// Creates an Resample engine (with the processing map)
        /// </summary>
        /// <param name="srcIndex">Source index</param>
        /// <param name="dstIndex">Destimation index</param>
        /// <param name="minWindow">Search Window to include initially</param>
        public Channel_Resample(Oilfield_Channel srcIndex, Oilfield_Channel dstIndex, double minWindow)
        {
            m_DstIndex = dstIndex;
            double maxWindow = Double.MinValue;
            for (int i = 1; i < srcIndex.Data.Count; i++)
            {
                if (Double.IsNaN(srcIndex.Data[i]) || Double.IsNaN(srcIndex.Data[i - 1])) continue;
                double d = Math.Abs(srcIndex.Data[i] - srcIndex.Data[i - 1]);
                if (d > maxWindow) maxWindow = d;
            }
            maxWindow *= 2.0;
            if (maxWindow < minWindow) maxWindow = minWindow;
            for (int i = 0; i < dstIndex.Data.Count; i++)
                m_mapping.Add(new Channel_Resample_Map(srcIndex, dstIndex.Data[i], minWindow, maxWindow));
        }
        
        /// <summary>
        /// Creates an Resample engine (with the processing map)
        /// </summary>
        /// <param name="srcIndex">Source index</param>
        /// <param name="dstIndex">Destimation index</param>
        /// <param name="minWindow">Search Window to include initially</param>
        /// <param name="maxWindow">Search Window to pass the empty values through</param>
        public Channel_Resample(Oilfield_Channel srcIndex, Oilfield_Channel dstIndex, double minWindow, double maxWindow)
        {
            m_DstIndex = dstIndex;
            for (int i = 0; i < dstIndex.Data.Count; i++)
                    m_mapping.Add(new Channel_Resample_Map(srcIndex, dstIndex.Data[i], minWindow, maxWindow));
        }

        /// <summary>
        /// Resamples a single channel
        /// </summary>
        /// <param name="srcChannel"></param>
        /// <param name="dstChannel"></param>
        public void Resample(Oilfield_Channel srcChannel, Oilfield_Channel dstChannel, int method)
        {
            List<double> tmp = new List<double>();
            switch( method)
            {
                case LinearInterpolate:
                    for (int i = 0; i < m_DstIndex.Data.Count; i++) tmp.Add(m_mapping[i].GetValueLinearInterpolate(srcChannel));
                    break;
                case InverseDistanceAverage:
                    for (int i = 0; i < m_DstIndex.Data.Count; i++) tmp.Add(m_mapping[i].GetValueInverseDistanceAverage(srcChannel));
                    break;
                case InverseSquareDistanceAverage:
                    for (int i = 0; i < m_DstIndex.Data.Count; i++) tmp.Add(m_mapping[i].GetValueInverseSquareDistanceAverage(srcChannel));
                    break;
                case GaussianAverage:
                    for (int i = 0; i < m_DstIndex.Data.Count; i++) tmp.Add(m_mapping[i].GetValueGaussianAverage(srcChannel));
                    break;
                case LinearInterpolateAngle:
                    for (int i = 0; i < m_DstIndex.Data.Count; i++) tmp.Add(m_mapping[i].GetValueLinearInterpolateAngle(srcChannel));
                    break;
                case AverageAngle:
                    for (int i = 0; i < m_DstIndex.Data.Count; i++) tmp.Add(m_mapping[i].GetValueAverageAngle(srcChannel));
                    break;
                case StepFromAbove:
                    for (int i = 0; i < m_DstIndex.Data.Count; i++) tmp.Add(m_mapping[i].GetValueStepFromAbove(srcChannel));
                    break;
                case StepFromBelow:
                    for (int i = 0; i < m_DstIndex.Data.Count; i++) tmp.Add(m_mapping[i].GetValueStepFromBelow(srcChannel));
                    break;
                case LogarithmicAverage:
                    for (int i = 0; i < m_DstIndex.Data.Count; i++) tmp.Add(m_mapping[i].GetValueLogarithmicAverage(srcChannel));
                    break;
                case HarmonicAverage:
                    for (int i = 0; i < m_DstIndex.Data.Count; i++) tmp.Add(m_mapping[i].GetValueHarmonicAverage(srcChannel));
                    break;
                default:
                    for (int i = 0; i < m_DstIndex.Data.Count; i++) tmp.Add( m_mapping[i].GetValueClosestPoint( srcChannel));
                    break;
            }
            dstChannel.Data = tmp;
        }

        /// <summary>
        /// Resamples all channels in a file
        /// </summary>
        public void Resample(Oilfield_File srcFile, Oilfield_File dstFile)
        {
            Resample(srcFile, dstFile, LinearInterpolate);
        }

        /// <summary>
        /// Resamples all channels in a file
        /// </summary>
        public void Resample(Oilfield_File srcFile, Oilfield_File dstFile, int default_method)
        {
            for (int i = 1; i < dstFile.Channels.Count; i++)
            {
                Oilfield_Channel c = dstFile.GetChannel(i);
                if ((c.Name.ToLower().Contains("az") || c.Name.ToLower().Contains("ti")) && c.Unit.ToLower().StartsWith("deg"))
                {
                    this.Resample(srcFile.GetChannel(i), c, LinearInterpolateAngle);
                    continue;
                }
                if (c.Unit.ToLower().Equals("m") || c.Unit.ToLower().Equals("ft") || c.Unit.ToLower().Equals("ms") || c.Unit.ToLower().Equals("s"))
                {
                    this.Resample(srcFile.GetChannel(i), c, LinearInterpolate);
                    continue;
                }
                if (c.Unit.ToLower().StartsWith("ohm"))
                {
                    this.Resample(srcFile.GetChannel(i), c, LogarithmicAverage);
                    continue;
                }
                if (c.Unit.ToLower().StartsWith("mho"))
                {
                    this.Resample(srcFile.GetChannel(i), c, HarmonicAverage);
                    continue;
                }
                this.Resample(srcFile.GetChannel(i), c, default_method);
            }
        }

        /// <summary>
        /// Resamples all channels in a file; cretaes channels in the destination as necessary
        /// </summary>
        public void ResampleCreateChannels(Oilfield_File srcFile, Oilfield_File dstFile)
        {
            dstFile.CreateSameChannels(srcFile);
            Resample(srcFile, dstFile);
        }

        /// <summary>
        /// Resamples all channels in a file; cretaes channels in the destination as necessary
        /// </summary>
        public void ResampleCreateChannels(Oilfield_File srcFile, Oilfield_File dstFile, int default_method)
        {
            dstFile.CreateSameChannels(srcFile);
            Resample(srcFile, dstFile, default_method);
        }

        /// <summary>
        /// Retrieves the proceduer numeral from name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int GetProcedure(string name)
        {
            for (int i = 0; i < Methods.Length; i++)
            {
                if (Methods[i].Equals(name)) return i;
            }
            return 0;
        }
    }
}
