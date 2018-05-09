using System;

namespace Petronode.Mathematics
{
	/// <summary>
	/// Projection class describes the recomputation required to perform the projections
	/// </summary>
	public class WellheadProjection
	{
		private double m_Angle = 0.0;

		/// <summary>
		/// Constructor; creates the new projection
		/// </summary>
		public WellheadProjection()
		{
		}

		/// <summary>
		/// Constructor; creates the new projection
		/// </summary>
		public WellheadProjection( Coordinates Wellhead, double Angle)
		{
			this.Angle = Angle;
			this.WellheadCoordinates.Northing = Wellhead.Northing;
			this.WellheadCoordinates.Easting = Wellhead.Easting;
		}

		/// <summary>
		/// Constructor; creates the new projection
		/// </summary>
		/// <param name="WellheadNorth"></param>
		/// <param name="WellheadEast"></param>
		/// <param name="Angle"></param>
		public WellheadProjection( double WellheadNorth, double WellheadEast, double Angle)
		{
			this.Angle = Angle;
			this.WellheadCoordinates.Northing = WellheadNorth;
			this.WellheadCoordinates.Easting = WellheadEast;
		}

		/// <summary>
		/// Wellhead corrdinates
		/// </summary>
		public Coordinates WellheadCoordinates = new Coordinates();

		/// <summary>
		/// Angle in degrees
		/// </summary>
		public double Angle
		{
			get{ return m_Angle;}
			set
			{
				m_Angle = value;
				double tmp = value * Math.PI / 180.0;
				this.Sine = Math.Sin( tmp);
				this.Cosine = Math.Cos( tmp);
			}
		}

		/// <summary>
		/// Angle Cosine
		/// </summary>
		public double Cosine = 1.0;

		/// <summary>
		/// Angle Sine
		/// </summary>
		public double Sine = 0.0;

		/// <summary>
		/// Computes the projection
		/// </summary>
		public double ComputeProjection( Coordinates coord)
		{
			return coord.ComputeProjection( this);
		}
	}
}
