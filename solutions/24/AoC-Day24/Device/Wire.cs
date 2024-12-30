using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AoC_Day24.Visualization;

namespace AoC_Day24.Device
{
    public class Wire(string name, bool? value) : Element
    {
        public string name = name;
        public bool? value = value;

        public bool? initialValue;

        public bool suspicious;
        public int group;

        public void Set()
        {
            initialValue = value;
        }

        public void Reset()
        {
            value = initialValue;
        }
    }
}