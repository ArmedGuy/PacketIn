using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketIn;

namespace PacketInTest
{
    public class ExamplePlayer : NetContent<ExamplePlayer>
    {
        [NetData(0)]
        public int Id { get; set ; }

        [NetData(1)]
        public int Score { get; set; }

        [NetData(2)]
        public string Name { get; set; }
    }
}
