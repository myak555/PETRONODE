using System;

namespace Petronode.Mathematics
{
	/// <summary>
	/// Coordinates class describes a point in Northing/Easting space
	/// and defines projections
	/// </summary>
	public class Coordinates
	{
		/// <summary>
		/// Constructor; creates an empty class
		/// </summary>
		public Coordinates()
		{
		}

		/// <summary>
		/// Constructor; creates a class with set coordinates
		/// </summary>
		public Coordinates( double Northing, double Easting)
		{
			this.Northing = Northing;
			this.Easting = Easting;
		}

		/// <summary>
		/// Constructor; creates a class with set coordinates
		/// </summary>
		public Coordinates( Coordinates Coord)
		{
			this.Northing = Coord.Northing;
			this.Easting = Coord.Easting;
		}

		/// <summary>
		/// Northing coordinate
		/// </summary>
		public double Northing = 0.0f;

		/// <summary>
		/// Easting coordinate
		/// </summary>
		public double Easting = 0.0f;

		/// <summary>
		/// Northing coordinate
		/// </summary>
		public double X
		{
			get{ return Northing;}
			set{ Northing = value;}
		}

		/// <summary>
		/// Easting coordinate
		/// </summary>
		public double Y
		{
			get{ return Easting;}
			set{ Easting = value;}
		}

		/// <summary>
		/// Stores the projection coordinate 
		/// </summary>
		public double Projection = 0.0;

		/// <summary>
		/// Projection coordinate
		/// </summary>
		public double R
		{
			get{ return Projection;}
			set{ Projection = value;}
		}

		/// <summary>
		/// Computes the projection from the current X,Y coordinates onto the plane 
		/// crossing point StartX, StartY at Azimuth angle 
		/// </summary>
		public double ComputeProjection( WellheadProjection p)
		{
			// The transform matrix for rotation is
			//
			//  CosA   SinA
			// -SinA   CosA
			//
			// or, if multiplied by the vector (X,Y)
			// Xnew = X*CosA + Y*SinA
			Projection = (this.Northing - p.WellheadCoordinates.Northing) * p.Cosine +
				(this.Easting - p.WellheadCoordinates.Easting) * p.Sine;
			return Projection;
		}

		/// <summary>
		/// Compute the point coordinates based on the current Projection value and the
		/// Wellhead data from the WellheadProjection
		/// </summary>
		/// <param name="p">Wellhead projection to fetch the data from</param>
		public void ComputeCoordinates( WellheadProjection p)
		{
			this.Northing = this.Projection * p.Cosine + p.WellheadCoordinates.Northing;
			this.Easting = this.Projection * p.Sine + p.WellheadCoordinates.Easting;
		}

		/// <summary>
		/// Compute the point coordinates based on the current Projection value and the
		/// Wellhead data from the WellheadProjection
		/// </summary>
		/// <param name="p">Wellhead projection to fetch the data from</param>
		public void ComputeCoordinates( double Extent, WellheadProjection p)
		{
			this.Projection = Extent;
			this.ComputeCoordinates( p);
		}

        /// <summary>
        /// Computes the range from this point to the point c
        /// </summary>
        /// <param name="c">coordinate to find the range</param>
        /// <returns>range</returns>
        public double ComputeRange(Coordinates c)
        {
            double dN = c.Northing - this.Northing;
            double dE = c.Easting - this.Easting;
            return Math.Sqrt(dN * dN + dE * dE);
        }
	}
}
