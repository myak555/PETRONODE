using System;
using System.Collections;
using System.Xml;

namespace Petronode.Mathematics
{
	/// <summary>
	/// Parameter describes interface for basic parameter
	/// </summary>
	public class Parameter
	{
		private string m_Value;
		private string m_Unit;
		private string m_Description;

		/// <summary>
		/// Constrictor - creates a parameter
		/// </summary>
		public Parameter( string Name, string Value, string Unit, string Description)
		{
			this.Name = Name;
			this.Value = Value;
			this.Unit = Unit;
			this.Description = Description;
		}
		public Parameter( XmlElement Elem)
		{
			Load( Elem);
		}

		/// <summary>
		/// Loads the parameter from Xml
		/// </summary>
		public void Load( XmlElement Elem)
		{
			foreach( XmlAttribute a in Elem.Attributes)
			{
				if( a.Name == "Name") this.Name = a.Value;
				if( a.Name == "Value") this.Value = a.Value;
				if( a.Name == "Unit") this.Unit = a.Value;
				if( a.Name == "Description") this.Description = a.Value;
			}
			IsModified = false;
		}

		/// <summary>
		/// Saves the parameter into Root
		/// </summary>
		public void Save( XmlDocument Doc, XmlElement Root)
		{
			XmlElement tmp = Doc.CreateElement("Parameter");
			XmlAttribute a = Doc.CreateAttribute( "Name");
			a.Value = Name;
			tmp.Attributes.Append( a);
			if( Value != null)
			{
				a = Doc.CreateAttribute( "Value");
				a.Value = Value;
				tmp.Attributes.Append( a);
			}
			if( Unit != null)
			{
				a = Doc.CreateAttribute( "Unit");
				a.Value = Unit;
				tmp.Attributes.Append( a);
			}
			if( Description != null)
			{
				a = Doc.CreateAttribute( "Description");
				a.Value = Description;
				tmp.Attributes.Append( a);
			}
			Root.AppendChild( tmp);
			IsModified = false;
		}

		/// <summary>
		/// Parameter name
		/// </summary>
		public string Name;

		/// <summary>
		/// Parameter value as a string
		/// </summary>
		public string Value
		{
			get{ return m_Value;}
			set
			{
				if( m_Value == value) return;
				m_Value = value;
				IsModified = true;
			}
		}

		/// <summary>
		/// Parameter value as a boolean
		/// </summary>
		public bool BoolValue
		{
			get
			{
				string tmp = m_Value.ToLower();
				if( tmp == "true") return true;
				if( tmp == "false") return false;
				return Convert.ToSingle( m_Value) != 0.0f;
			}
			set
			{
				if( value && this.Value.ToLower()=="true") return;
				if( !value && this.Value.ToLower()=="false") return;
				m_Value = value? "True" : "False";
				IsModified = true;
			}
		}

		/// <summary>
		/// Parameter value as a Single
		/// </summary>
		public float SingleValue
		{
			get{ return Convert.ToSingle( m_Value);}
			set
			{
				this.Value = value.ToString();
				IsModified = true;
			}
		}

		/// <summary>
		/// Parameter value as a Double
		/// </summary>
		public double DoubleValue
		{
			get{ return Convert.ToDouble( m_Value);}
			set
			{
				this.Value = value.ToString();
				IsModified = true;
			}
		}

		/// <summary>
		/// Parameter unit
		/// </summary>
		public string Unit
		{
			get{ return m_Unit;}
			set
			{
				if( m_Unit == value) return;
				m_Unit = value;
				IsModified = true;
			}
		}

		/// <summary>
		/// Parameter Description
		/// </summary>
		public string Description
		{
			get{ return m_Description;}
			set{ m_Description = value;}
		}

		/// <summary>
		/// Set to true if this paramter has been modified
		/// </summary>
		public bool IsModified = false;

		/// <summary>
		/// Converts the parameter to English units
		/// </summary>
		public void ConvertToEnglish()
		{
			if( this.m_Unit == "m") 
			{
				this.DoubleValue /= 0.3048;
				this.Unit = "ft";
				return;
			}
			if( this.m_Unit == "cm") 
			{
				this.DoubleValue *= 0.39370079;
				this.Unit = "in";
				return;
			}
			if( this.m_Unit == "degc") 
			{
				this.DoubleValue = this.DoubleValue * 1.8 + 32.0;
				this.Unit = "degf";
				return;
			}
			if( this.m_Unit == "kPa") 
			{
				this.DoubleValue /= 6.9383562;
				this.Unit = "psi";
				return;
			}
			if( this.m_Unit == "N") 
			{
				this.DoubleValue /= 4.454352;
				this.Unit = "lbs";
				return;
			}
			if( this.m_Unit == "m/s") 
			{
				this.DoubleValue /= 0.3048;
				this.Unit = "ft/s";
				return;
			}
			if( this.m_Unit == "us/m") 
			{
				this.DoubleValue *= 0.3048;
				this.Unit = "us/ft";
				return;
			}
		}
		/// <summary>
		/// Converts the parameter to Metric units
		/// </summary>
		public void ConvertToMetric()
		{
			if( this.m_Unit == "ft") 
			{
				this.DoubleValue *= 0.3048;
				this.Unit = "m";
				return;
			}
			if( this.m_Unit == "in") 
			{
				this.DoubleValue /= 0.39370079;
				this.Unit = "cm";
				return;
			}
			if( this.m_Unit == "degf") 
			{
				this.DoubleValue = (this.DoubleValue - 32.0)/1.8;
				this.Unit = "degc";
				return;
			}
			if( this.m_Unit == "psi") 
			{
				this.DoubleValue *= 6.9383562;
				this.Unit = "kPa";
				return;
			}
			if( this.m_Unit == "lbs") 
			{
				this.DoubleValue *= 4.454352;
				this.Unit = "N";
				return;
			}
			if( this.m_Unit == "ft/s") 
			{
				this.DoubleValue *= 0.3048;
				this.Unit = "m/s";
				return;
			}
			if( this.m_Unit == "us/ft") 
			{
				this.DoubleValue /= 0.3048;
				this.Unit = "us/m";
				return;
			}
		}
	}
	
	/// <summary>
	/// Parameter Collection defines a group of parameters
	/// </summary>
	public class ParameterCollection: ArrayList
	{
		public string CollectionName;

		/// <summary>
		/// Constructor - creates an empty list
		/// </summary>
		public ParameterCollection( string CollectionName)
		{
			this.CollectionName = CollectionName;
		}

		/// <summary>
		/// Indexer - returns a parameter
		/// </summary>
		public new Parameter this[ int index]
		{
			get{ return (Parameter)base[index];}
		}

        /// <summary>
        /// Indexer - returns a parameter
        /// </summary>
        public Parameter this[string Name]
		{
			get
			{
				foreach( Parameter p in this)
				{
					if( p.Name == Name) return p;
				}
				return null;
			}
		}

		/// <summary>
		/// Loads a parameter from Root
		/// </summary>
		public void Load( XmlElement Root)
		{
			this.Clear();
			foreach( XmlNode n in Root.ChildNodes)
			{
				if( n.NodeType != XmlNodeType.Element) continue;
				if( n.Name != this.CollectionName) continue;
				foreach( XmlNode k in n.ChildNodes)
				{
					if( k.NodeType != XmlNodeType.Element) continue;
					if( k.Name != "Parameter") continue;
					Parameter tmp = new Parameter( (XmlElement)k);
					base.Add( tmp);
				}
			}
		}

		/// <summary>
		/// Saves the parameter collection into Root
		/// </summary>
		public void Save( XmlDocument Doc, XmlElement Root)
		{
			XmlElement CollectionElement = Doc.CreateElement( this.CollectionName);
			foreach( Parameter p in this)
			{
				p.Save( Doc, CollectionElement);
			}
			Root.AppendChild( CollectionElement);
		}

		/// <summary>
		/// Forces a parameter on the collection; if the parameter exists (by name),
		/// the value and unit remain the same, but the description changes, or if the
		/// parameter does not exist, the new one is created
		/// </summary>
		public void ForceParameter( string Name, string Value, string Unit, string Description)
		{
			Parameter p = this[ Name];
			if( p != null)
			{
				p.Description = Description;
				return;
			}
			p = new Parameter( Name, Value, Unit, Description);
			p.IsModified = true;
			base.Add( p);
		}

		/// <summary>
		/// Converts the parameter to English units
		/// </summary>
		public void ConvertToEnglish()
		{
			foreach( Parameter p in this) p.ConvertToEnglish();
		}

		/// <summary>
		/// Converts the parameter to Metric units
		/// </summary>
		public void ConvertToMetric()
		{
			foreach( Parameter p in this) p.ConvertToMetric();
		}

		/// <summary>
		/// Retrieves true if some member has been modified
		/// </summary>
		public bool IsModified
		{
			get
			{
				foreach( Parameter p in this)
				{
					if( p.IsModified) return true;
				}
				return false;
			}
			set
			{
				foreach( Parameter p in this) p.IsModified = value;
			}
		}
	}
}
