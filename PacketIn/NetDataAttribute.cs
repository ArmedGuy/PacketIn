using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketIn
{
    public class NetDataAttribute : Attribute
    {
        public int Index { get; set; }

        public NetDataAttribute(int index)
        {
            Index = index;
        }
    }
}
