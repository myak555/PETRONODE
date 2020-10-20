using System;
using System.Collections.Generic;
using System.Text;

namespace Petronode.SlideComposerLibrary
{
    public class ComponentProperty
    {
        public string Name = "";
        public int LinePosition = -1;
        public string Value = "";

        #region Constructors
        public ComponentProperty()
        {
        }

        public ComponentProperty(string name)
        {
            Name = name;
        }

        public ComponentProperty(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public ComponentProperty(string name, string value, int lineposition)
        {
            Name = name;
            Value = value;
            LinePosition = lineposition;
        }
        #endregion

        /// <summary>
        /// Gets and retrieves the integer value
        /// </summary>
        public int IntValue
        {
            get
            {
                try
                {
                    return Convert.ToInt32(Value);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            set
            {
                Value = value.ToString();
            }
        }

        /// <summary>
        /// Gets and retrieves the float value
        /// </summary>
        public float FloatValue
        {
            get
            {
                try
                {
                    return Convert.ToSingle(Value);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            set
            {
                Value = value.ToString();
            }
        }
    }
}
