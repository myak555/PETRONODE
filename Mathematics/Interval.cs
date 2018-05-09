using System;

namespace Petronode.Mathematics
{
	/// <summary>
	/// Interval class describes a set of values - ValueFrom, Value and ValueTo 
	/// </summary>
	public class Interval
	{
		public Interval()
		{
		}
		public Interval( float Value)
		{
			this.ValueFrom = Value;
			this.Value = Value;
			this.ValueTo = Value;
		}
		public Interval( float ValueFrom, float Value, float ValueTo)
		{
			this.ValueFrom = ValueFrom;
			this.Value = Value;
			this.ValueTo = ValueTo;
		}
		public float ValueFrom = 0.0f;
		public float Value = 0.0f;
		public float ValueTo = 0.0f;
	}
}
