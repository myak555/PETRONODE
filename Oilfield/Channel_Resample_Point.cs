using System;
using System.Collections.Generic;
using System.Text;

namespace Petronode.Oilfield
{
    class Channel_Resample_Point
    {
        public int Index = -1;
        public double Distance = 0.0;
        public double AbsoluteDistance = 0.0;
        public double InverseDistanceWeight = 0.0;
        public double InverseSquareDistanceWeight = 0.0;
        public double GaussianWeight = 0.0;

        /// <summary>
        /// Constructor - creates a point
        /// </summary>
        /// <param name="index">index in the source file</param>
        /// <param name="distance">distance to that point</param>
        public Channel_Resample_Point(int index, double distance)
        {
            Index = index;
            Distance = distance;
            AbsoluteDistance = Math.Abs(distance);
        }

        /// <summary>
        /// Creates a normalization for the point, with the weight given
        /// </summary>
        /// <param name="weight"> weight to normalize for</param>
        public void Normalize( double weight)
        {
            double d = AbsoluteDistance;
            if( d < 1e-6) d = 1e-6;
            InverseDistanceWeight = weight/d;
            InverseSquareDistanceWeight = InverseDistanceWeight * InverseDistanceWeight;
            GaussianWeight = Math.Exp( -d*d*weight*weight);
        }
    }
}
