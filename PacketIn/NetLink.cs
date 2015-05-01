using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PacketIn
{
    /// <summary>
    /// One to one binding between data endpoints. Is split up by channels.
    /// </summary>
    public class NetLink
    {
        internal NetTransporter Transporter;
        internal List<NetChannel> NetChannels;

        public NetChannel DefaultChannel { get; private set; }

        public NetLink(NetTransporter transporter)
        {
            Transporter = transporter;
            NetChannels = new List<NetChannel>();
            DefaultChannel = new NetChannel("-");
            AddChannel(DefaultChannel);

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

            Transporter.Receive();
            return true;
        }

        public void Enable()
        {
            Transporter.Receive();
        }

        public void Disable()
        {
            Transporter.Disable();
        }

        public void AddChannel(NetChannel chan)
        {
            NetChannels.Add(chan);
            chan.Link = this;
        }

        public void Send<T>(T data) where T : NetContent<T>
        {
            Send("-", data);
        }

        public void Send<T>(string channel, T data) where T : NetContent<T>
        {
            var chanId = channel.GetHashCode();
            foreach (var chan in NetChannels.Where(x => x.Id == chanId))
            {
                chan.Send(data);
            }
        }
    }
}
