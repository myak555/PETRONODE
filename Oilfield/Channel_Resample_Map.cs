using System;
using System.Collections.Generic;
using System.Text;
using Petronode.OilfieldFileAccess;

namespace Petronode.Oilfield
{
    class Channel_Resample_Map
    {
        List<Channel_Resample_Point> MapList = new List<Channel_Resample_Point>();
        Channel_Resample_Point Above = null;
        Channel_Resample_Point Below = null;
        Channel_Resample_Point Closest = null;

        /// <summary>
        /// Creates a single resample map; makes interpolation adjustments
        /// </summary>
        /// <param name="srcIndex">Depth index of the source file</param>
        /// <param name="depth">depth at the destination file</param>
        /// <param name="window">initial window size</param>
        public Channel_Resample_Map(Oilfield_Channel srcIndex, double depth, double minWindow, double maxWindow)
        {
            InitMap(srcIndex, depth, minWindow, maxWindow);
            if (MapList.Count <= 0) return;
            List<Channel_Resample_Point> above = new List<Channel_Resample_Point>();
            List<Channel_Resample_Point> below = new List<Channel_Resample_Point>();
            foreach (Channel_Resample_Point crp in MapList)
            {
                if (crp.Distance <= 0.0) above.Add(crp);
                if (crp.Distance >= 0.0) below.Add(crp);
            }
            double ad_above = Double.MaxValue;
            for (int i = 0; i < above.Count; i++)
            {
                if (above[i].AbsoluteDistance > ad_above) continue;
                Above = above[i];
                ad_above = Above.AbsoluteDistance;
            }
            double ad_below = Double.MaxValue;
            for (int i = 0; i < below.Count; i++)
            {
                if (below[i].AbsoluteDistance > ad_below) continue;
                Below = below[i];
                ad_below = Below.AbsoluteDistance;
            }
            Closest = (ad_below < ad_above) ? Below : Above;
        }

        private void InitMap(Oilfield_Channel srcIndex, double depth, double minWindow, double maxWindow)
        {
            if (depth < srcIndex.MinValue - minWindow) return;
            if (depth > srcIndex.MaxValue + minWindow) return;
            for (int j = 0; j < 20; j++)
            {
                MapList.Clear();
                bool above_exists = false;
                bool below_exists = false;
                for (int i = 0; i < srcIndex.Data.Count; i++)
                {
                    double d = srcIndex.Data[i];
                    if (Double.IsNaN(d)) continue;
                    if (d < depth - minWindow || depth + minWindow < d) continue;
                    double dd = d - depth;
                    above_exists = above_exists && (dd<=0.0);
                    below_exists = below_exists && (dd>=0.0);
                    MapList.Add(new Channel_Resample_Point(i, dd));
                }
                minWindow *= 2.0;
                if (minWindow > maxWindow) break;
                if (MapList.Count >= 5 && above_exists && below_exists) break;
            }
            double weight = 0.0;
            for (int j = 0; j < MapList.Count; j++)
            {
                weight += MapList[j].AbsoluteDistance;
            }
            if (weight < 1e-6) weight = 1e-6;
            weight = 1.0 / weight;
            for (int j = 0; j < MapList.Count; j++)
            {
                MapList[j].Normalize(weight);
            }
        }

        /// <summary>
        /// Returns the value for given depth using linear interpolation method
        /// </summary>
        /// <param name="srcChannel">Source channel</param>
        /// <returns>interpolated value</returns>
        public double GetValueLinearInterpolate(Oilfield_Channel srcChannel)
        {
            if (Above==null || Below==null) return Double.NaN;
            if (Above.Index == Below.Index) return srcChannel.Data[Above.Index];
            double norm = Above.AbsoluteDistance + Below.AbsoluteDistance;
            if (norm <= 0.0) return Double.NaN;
            double d1 = srcChannel.Data[Above.Index];
            double d2 = srcChannel.Data[Below.Index];
            if (Double.IsNaN(d1) || Double.IsNaN(d2)) return Double.NaN;
            double weightedAverage = d1 * Below.AbsoluteDistance + d2 * Above.AbsoluteDistance;
            return weightedAverage / norm;
        }

        /// <summary>
        /// Returns the value for given depth using inverse distance method
        /// </summary>
        /// <param name="srcChannel">Source channel</param>
        /// <returns>interpolated value</returns>
        public double GetValueInverseDistanceAverage(Oilfield_Channel srcChannel)
        {
            double weightedAverage = 0.0;
            double norm = 0.0;
            for (int j = 0; j < MapList.Count; j++)
            {
                Channel_Resample_Point crp = MapList[j];
                double d = srcChannel.Data[crp.Index];
                if (Double.IsNaN(d)) continue;
                norm += crp.InverseDistanceWeight;
                weightedAverage += crp.InverseDistanceWeight * d;
            }
            if (norm <= 0.0) return Double.NaN;
            return weightedAverage / norm;
        }

        /// <summary>
        /// Returns the value for given depth using inverse square distance method
        /// </summary>
        /// <param name="srcChannel">Source channel</param>
        /// <returns>interpolated value</returns>
        public double GetValueInverseSquareDistanceAverage(Oilfield_Channel srcChannel)
        {
            double weightedAverage = 0.0;
            double norm = 0.0;
            for (int j = 0; j < MapList.Count; j++)
            {
                Channel_Resample_Point crp = MapList[j];
                double d = srcChannel.Data[crp.Index];
                if (Double.IsNaN(d)) continue;
                norm += crp.InverseSquareDistanceWeight;
                weightedAverage += crp.InverseSquareDistanceWeight * d;
            }
            if (norm <= 0.0) return Double.NaN;
            return weightedAverage / norm;
        }

        /// <summary>
        /// Returns the value for given depth using Gaussian method
        /// </summary>
        /// <param name="srcChannel">Source channel</param>
        /// <returns>interpolated value</returns>
        public double GetValueGaussianAverage(Oilfield_Channel srcChannel)
        {
            double weightedAverage = 0.0;
            double norm = 0.0;
            for (int j = 0; j < MapList.Count; j++)
            {
                Channel_Resample_Point crp = MapList[j];
                double d = srcChannel.Data[crp.Index];
                if (Double.IsNaN(d)) continue;
                norm += crp.GaussianWeight;
                weightedAverage += crp.GaussianWeight * d;
            }
            if (norm <= 0.0) return Double.NaN;
            return weightedAverage / norm;
        }

        /// <summary>
        /// Returns the value for given depth, interpolating through 360 degrees
        /// </summary>
        /// <param name="srcChannel">Source channel</param>
        /// <returns>interpolated value</returns>
        public double GetValueLinearInterpolateAngle(Oilfield_Channel srcChannel)
        {
            if (Above == null || Below == null) return Double.NaN;
            if (Above.Index == Below.Index) return srcChannel.Data[Above.Index];
            double norm = Above.AbsoluteDistance + Below.AbsoluteDistance;
            if (norm <= 0.0) return Double.NaN;
            double d1 = srcChannel.Data[Above.Index];
            double d2 = srcChannel.Data[Below.Index];
            if (Double.IsNaN(d1) || Double.IsNaN(d2)) return Double.NaN;
            while (d1 < 0.0) d1 += 360.0;
            while (d1 > 360.0) d1 -= 360.0;
            while (d2 < 0.0) d2 += 360.0;
            while (d2 > 360.0) d2 -= 360.0;
            if (d1 == d2) return d1;
            double weightedAverage = d1 * Below.AbsoluteDistance + d2 * Above.AbsoluteDistance;
            weightedAverage /= norm;
            //if (d2 - d1 > 180.0) weightedAverage += 180.0;
            //if (d1 - d2 > 180.0) weightedAverage += 180.0;
            while (weightedAverage < 0.0) weightedAverage += 360.0;
            while (weightedAverage > 360.0) weightedAverage -= 360.0;
            return weightedAverage;
        }

        /// <summary>
        /// Returns the value for given depth, interpolating through 360 degrees
        /// </summary>
        /// <param name="srcChannel">Source channel</param>
        /// <returns>interpolated value</returns>
        public double GetValueAverageAngle(Oilfield_Channel srcChannel)
        {
            if (MapList.Count < 0) return Double.NaN;
            if (MapList.Count == 1) return srcChannel.Data[MapList[0].Index];
            double weightedAverage = 0.0;
            double norm = 0.0;
            for (int j = 0; j < MapList.Count; j++)
            {
                Channel_Resample_Point crp = MapList[j];
                double d = srcChannel.Data[crp.Index];
                if (Double.IsNaN(d)) continue;
                while (d < 0.0) d += 360.0;
                while (d > 360.0) d -= 360.0;
                double n = crp.InverseDistanceWeight;
                if (weightedAverage - d > 180.0) d += 180.0;
                if (d - weightedAverage > 180.0) d += 180.0;
                weightedAverage = norm * weightedAverage + n * d;
                norm += n;
                if (norm > 0.0) weightedAverage /= norm;
                while (weightedAverage < 0.0) weightedAverage += 360.0;
                while (weightedAverage >= 360.0) weightedAverage -= 360.0;
            }
            return weightedAverage;
        }

        /// <summary>
        /// Returns the value for given depth, using ClosestPoint function
        /// </summary>
        /// <param name="srcChannel">Source channel</param>
        /// <returns>interpolated value</returns>
        public double GetValueClosestPoint(Oilfield_Channel srcChannel)
        {
            if (Closest == null) return Double.NaN;
            return srcChannel.Data[Closest.Index];
        }

        /// <summary>
        /// Returns the value for given depth, using ClosestPoint function from Above
        /// </summary>
        /// <param name="srcChannel">Source channel</param>
        /// <returns>interpolated value</returns>
        public double GetValueStepFromAbove(Oilfield_Channel srcChannel)
        {
            if (Above == null) return Double.NaN;
            return srcChannel.Data[Above.Index];
        }

        /// <summary>
        /// Returns the value for given depth, using ClosestPoint function from below
        /// </summary>
        /// <param name="srcChannel">Source channel</param>
        /// <returns>interpolated value</returns>
        public double GetValueStepFromBelow(Oilfield_Channel srcChannel)
        {
            if (Below == null) return Double.NaN;
            return srcChannel.Data[Below.Index];
        }

        /// <summary>
        /// Returns the value for given depth, using Logarithmic function
        /// </summary>
        /// <param name="srcChannel">Source channel</param>
        /// <returns>interpolated value</returns>
        public double GetValueLogarithmicAverage(Oilfield_Channel srcChannel)
        {
            double weightedAverage = 0.0;
            double norm = 0.0;
            for (int j = 0; j < MapList.Count; j++)
            {
                Channel_Resample_Point crp = MapList[j];
                double d = srcChannel.Data[crp.Index];
                if (Double.IsNaN(d)) continue;
                if ( d < 0.000001) continue;
                norm += crp.InverseDistanceWeight;
                weightedAverage += crp.InverseDistanceWeight * Math.Log10(d);
            }
            if (norm <= 0.0) return Double.NaN;
            return Math.Pow( 10.0, weightedAverage / norm);
        }

        /// <summary>
        /// Returns the value for given depth, using Harmonic average
        /// </summary>
        /// <param name="srcChannel">Source channel</param>
        /// <returns>interpolated value</returns>
        public double GetValueHarmonicAverage(Oilfield_Channel srcChannel)
        {
            double weightedAverage = 0.0;
            double norm = 0.0;
            for (int j = 0; j < MapList.Count; j++)
            {
                Channel_Resample_Point crp = MapList[j];
                double d = srcChannel.Data[crp.Index];
                if (Double.IsNaN(d)) continue;
                if (d < 0.000001) continue;
                norm += crp.InverseDistanceWeight;
                weightedAverage += crp.InverseDistanceWeight / d;
            }
            if (norm <= 0.0) return Double.NaN;
            return norm / weightedAverage;
        }
    }
}
