using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketIn
{
    public class NetServer
    {
        internal List<NetLink> Links;

        public NetServer()
        {
            Links = new List<NetLink>();
        }

        public void Broadcast<T>(T data) where T : NetContent<T>
        {
            Broadcast("-", data);
        }
        public void Broadcast<T>(string channel, T data) where T : NetContent<T>
        {
            foreach (var l in Links)
            {
                l.Send(channel, data);
            }
        }
    }
}
