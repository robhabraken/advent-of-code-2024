using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC_Day24.Visualization
{
    public class Coordinate(int x, int y, int offset)
    {
        public int x = x;
        public int y = y;
        public int offset = offset;
    }
}