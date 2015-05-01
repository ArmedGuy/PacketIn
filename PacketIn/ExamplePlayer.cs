using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketIn
{
    public class ExamplePlayer : NetSnapshot
    {

        [SnapshotData(0)]
        public int Id { get; set ; }

        public int Score { get; set; }

        [SnapshotData(1)]
        public string Name { get; set; }
    }
}
