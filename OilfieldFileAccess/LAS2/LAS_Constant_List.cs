using System;
using System.Collections.Generic;
using System.Text;

namespace Petronode.OilfieldFileAccess.LAS
{
    public class LAS_Constant_List
    {
        /// <summary>
        /// Describes LAS file Constants
        /// </summary>
        public List<LAS_Constant> Constants = new List<LAS_Constant>();
        
        /// <summary>
        /// Sets an empty constant list
        /// </summary>
        public LAS_Constant_List()
        {
        }

        /// <summary>
        /// Sets a constant list from LAS file
        /// </summary>
        public LAS_Constant_List( LAS_File las)
        {
            foreach (LAS_Constant c in las.Parameters)
            {
                this.Constants.Add(c);
            }
        }

        /// <summary>
        /// Returns true if the LAS constant exists
        /// </summary>
        /// <param name="name">Constant name</param>
        public bool ConstantExists(string name)
        {
            foreach (LAS_Constant lc in Constants)
            {
                if (lc.Name == name) return true;
            }
            return false;
        }
        
        /// <summary>
        /// Returns the LAS constant by name
        /// </summary>
        /// <param name="name">Constant name</param>
        /// <returns>value</returns>
        public string GetConstant(string name)
        {
            foreach (LAS_Constant lc in Constants)
            {
                if (lc.Name == name) return lc.Value;
            }
            return "";
        }

        /// <summary>
        /// Sets the LAS constant by name
        /// </summary>
        /// <param name="name">Constant name</param>
        /// <param name="val">Constant value</param>
        public void SetConstant(string name, string val)
        {
            foreach (LAS_Constant lc in Constants)
            {
                if (lc.Name != name) continue;
                lc.Value = val;
                return;
            }
        }

        /// <summary>
        /// Sets the LAS constant by name, adding a new parameter if necessary
        /// </summary>
        /// <param name="name">Constant name</param>
        /// <param name="unit">Constant unit</param>
        /// <param name="value">Constant value</param>
        /// <param name="value">Description</param>
        public void SetConstant(string name, string unit, string value, string description)
        {
            foreach (LAS_Constant lc in Constants)
            {
                if (lc.Name != name) continue;
                lc.Value = value;
                return;
            }
            LAS_Constant newP = new LAS_Constant(name, unit, value, description);
            Constants.Add(newP);
        }

        /// <summary>
        /// Sets the LAS constant by name, adding a new parameter if necessary
        /// </summary>
        /// <param name="name">Constant name</param>
        /// <param name="unit">Constant unit</param>
        /// <param name="value">Constant value</param>
        /// <param name="value">Description</param>
        public void CheckConstant(string name, string unit, string value, string description)
        {
            foreach (LAS_Constant lc in Constants)
            {
                if (lc.Name != name) continue;
                if( lc.Value == "") lc.Value = value;
                return;
            }
            LAS_Constant newP = new LAS_Constant(name, unit, value, description);
            Constants.Add(newP);
        }

        /// <summary>
        /// Reads from ASCII file
        /// </summary>
        /// <param name="FileName"></param>
        public void Read(string FileName)
        {
            LAS_File las = new LAS_File(FileName, false);
            this.Constants.Clear();
            foreach (LAS_Constant c in las.Parameters)
            {
                this.Constants.Add(c);
            }
        }

        /// <summary>
        /// Writes to ASCII file
        /// </summary>
        /// <param name="FileName"></param>
        public void Write(string FileName)
        {
            LAS_File las = new LAS_File();
            foreach (LAS_Constant c in this.Constants)
            {
                las.Parameters.Add(c);
            }
            las.Write(FileName);
        }
    }
}
