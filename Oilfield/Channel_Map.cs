using System;
using System.Collections.Generic;
using System.Text;

namespace Petronode.Oilfield
{
    public class Channel_Map
    {
        public string Name = "undefined";
        public List<string> Map = new List<string>();

        public Channel_Map()
        {
        }

        public Channel_Map(List<string> mapping)
        {
            Name = mapping[0];
            for (int i = 1; i < mapping.Count; i++)
            {
                Map.Add(mapping[i]);
            }
        }

        public Channel_Map(string name, string mapping1)
        {
            Name = name;
            Map.Add(mapping1);
        }

        public Channel_Map(string name, string mapping1, string mapping2)
        {
            Name = name;
            Map.Add(mapping1);
            Map.Add(mapping2);
        }

        public Channel_Map(string name, string mapping1, string mapping2, string mapping3)
        {
            Name = name;
            Map.Add(mapping1);
            Map.Add(mapping2);
            Map.Add(mapping3);
        }

        public Channel_Map(string name, string mapping1, string mapping2, string mapping3, string mapping4)
        {
            Name = name;
            Map.Add(mapping1);
            Map.Add(mapping2);
            Map.Add(mapping3);
            Map.Add(mapping4);
        }

        public Channel_Map(string name, string mapping1, string mapping2, string mapping3, string mapping4, string mapping5)
        {
            Name = name;
            Map.Add(mapping1);
            Map.Add(mapping2);
            Map.Add(mapping3);
            Map.Add(mapping4);
            Map.Add(mapping5);
        }

        public string[] ToStrings( int count)
        {
            string[] tmp = new string[count];
            tmp[0] = Name;
            for (int i = 1; i < tmp.Length; i++)
            {
                tmp[i] = (i <= Map.Count) ? Map[i-1] : "";
            }
            return tmp;
        }
    }
}
