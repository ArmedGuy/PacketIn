using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketIn
{
    public class SnapshotDataAttribute : Attribute
    {
        public int Index { get; set; }

        public SnapshotDataAttribute(int index)
        {
            Index = index;
        }
    }
}
