using System;
using System.Collections.Generic;
using System.Text;

namespace Petronode.CommonData
{
    public class FitterParameter
    {
        private double m_Value = 0.0;
        private double m_MinLimit = -1.0;
        private double m_MaxLimit = 1.0;

        public string Name = "None";
        public string Description = "None";
        public bool AcceptDigitizerX = false;
        public bool AcceptDigitizerY = false;
        public string Format = "0.000";

        /// <summary>
        /// Creates a default parameter 
        /// </summary>
        public FitterParameter()
        {
        }

        /// <summary>
        /// Creates parameter 
        /// </summary>
        /// <param name="name">Name like "gradient"</param>
        /// <param name="description">Description like "angle between line and horizontal"</param>
        /// <param name="v">Value</param>
        /// <param name="min">Minimum limit for optimization</param>
        /// <param name="max">Maximum limit for optimization</param>
        /// <param name="format">Data format as "0.000"</param>
        public FitterParameter(string name, string description, double v, double min, double max, string format)
        {
            this.Name = name;
            this.Description = description;
            this.Value = v;
            this.MinLimit = min;
            this.MaxLimit = max;
            this.Format = format;
        }

        /// <summary>
        /// Creates parameter 
        /// </summary>
        /// <param name="name">Name like "gradient"</param>
        /// <param name="description">Description like "angle between line and horizontal"</param>
        /// <param name="v">Value</param>
        /// <param name="min">Minimum limit for optimization</param>
        /// <param name="max">Maximum limit for optimization</param>
        public FitterParameter(string name, string description, double v, double min, double max)
        {
            this.Name = name;
            this.Description = description;
            this.Value = v;
            this.MinLimit = min;
            this.MaxLimit = max;
        }

        /// <summary>
        /// Creates parameter 
        /// </summary>
        public FitterParameter(FitterParameter p)
        {
            this.Name = p.Name;
            this.Description = p.Description;
            this.Value = p.Value;
            this.MinLimit = p.MinLimit;
            this.MaxLimit = p.MaxLimit;
            this.AcceptDigitizerX = p.AcceptDigitizerX;
            this.AcceptDigitizerY = p.AcceptDigitizerY;
            this.Format = p.Format;
        }

        /// <summary>
        /// Extracts parameter from string by name
        /// </summary>
        public void FromString(string s)
        {
            string pCheck = Name + "=";
            int j = s.IndexOf( pCheck);
            if (j < 0) return;
            s = s.Substring(j + pCheck.Length);
            j = s.IndexOf(",");
            if (j >= 0) s = s.Substring(0, j);
            if (s.Length <= 0) return;
            try
            {
                m_Value = Convert.ToDouble(s);
                m_MinLimit = m_Value;
                m_MaxLimit = m_Value;
            }
            catch
            {
            }
        }

        /// <summary>
        /// ToString for debugging
        /// </summary>
        /// <returns> string representation of parameter</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder( Name );
            sb.Append("=");
            sb.Append(Value.ToString(Format));
            return sb.ToString();
        }

        /// <summary>
        /// Return multiple strings to fill control
        /// </summary>
        public string[] ToStrings()
        {
            string[] tmp = new string[4];
            tmp[0] = Name + " (" + Description + ")";
            tmp[1] = Value.ToString(Format);
            tmp[2] = MinLimit.ToString(Format);
            tmp[3] = MaxLimit.ToString(Format);
            return tmp;
        }

        /// <summary>
        /// Sets and retrieves parameter value
        /// </summary>
        public double Value
        {
            get{ return m_Value;}
            set
            {
                m_Value = value;
                if (m_MinLimit > value) m_MinLimit = value;
                if (m_MaxLimit < value) m_MaxLimit = value;
            }
        }

        /// <summary>
        /// Sets and retrieves minimum limit
        /// </summary>
        public double MinLimit
        {
            get { return m_MinLimit; }
            set
            {
                m_MinLimit = value;
                if (m_Value < value) m_Value = value;
                if (m_MaxLimit < value) m_MaxLimit = value;
            }
        }

        /// <summary>
        /// Sets and retrieves maximum limit
        /// </summary>
        public double MaxLimit
        {
            get { return m_MaxLimit; }
            set
            {
                m_MaxLimit = value;
                if (m_Value > value) m_Value = value;
                if (m_MinLimit > value) m_MinLimit = value;
            }
        }
    }
}
