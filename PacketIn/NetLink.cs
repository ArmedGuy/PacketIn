using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace PacketIn
{
    /// <summary>
    /// One to one binding between data endpoints. Is split up by channels.
    /// </summary>
    public class NetLink
    {
        internal NetTransporter Transporter;
        internal List<NetChannel> NetChannels;

        public NetLink(NetTransporter transporter)
        {
            Transporter = transporter;
            NetChannels = new List<NetChannel>();
            AddChannel(new NetChannel("-"));

            Transporter.OnReceive += Transporter_OnReceive;
        }

        internal bool Transporter_OnReceive(object sender, byte[] data)
        {
            using (var reader = new BinaryReader(new MemoryStream(data)))
            {
                var chanId = reader.ReadInt32();
                var channels = NetChannels.Where(x => x.Id == chanId);
                foreach (var c in channels)
                {
                    c.Messaged(sender, data);
                }
            }
            return true;
        }

        public void AddChannel(NetChannel chan)
        {
            NetChannels.Add(chan);
            chan.Link = this;
        }

        public void Send<T>(T data) where T : NetSnapshot
        {
            Send("-", data);
        }

        public void Send<T>(string channel, T data) where T : NetSnapshot
        {
            var chanId = channel.GetHashCode();
            foreach (var chan in NetChannels.Where(x => x.Id == chanId))
            {
                chan.Send(data);
            }
        }
    }
}
