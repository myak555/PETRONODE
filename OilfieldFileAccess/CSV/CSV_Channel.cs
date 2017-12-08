using System;
using System.Collections.Generic;
using System.Text;
using Petronode.OilfieldFileAccess;

namespace Petronode.OilfieldFileAccess.CSV
{

    /// <summary>
    /// Descrives CSV curve type
    /// </summary>
    public class CSV_Channel:Oilfield_Channel
    {
        /// <summary>
        /// Constructor - creates a default variable
        /// </summary>
        public CSV_Channel(): base()
        {
            PaddedWidth = 0;
        }

        /// <summary>
        /// Constructor - creates from the parameters
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="unit">Variable unit</param>
        /// <param name="description">Description</param>
        public CSV_Channel(string name, string unit): base( name, unit, "")
        {
            PaddedWidth = 0;
        }

        /// <summary>
        /// Converts back to CSV-like string
        /// </summary>
        /// <returns>Converted string</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name + ",");
            sb.Append(Unit + ",");
            sb.Append(Description);
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
            CSV_Channel tmp = new CSV_Channel();
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
    }
}
