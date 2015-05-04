using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketIn
{
    public class NetChannel
    {

        internal NetLink Link;
        internal MemoryStream SendBuffer;
        
        public event NetRecevier OnMessage;

        internal void Messaged(object sender, byte[] data)
        {
            if (OnMessage == null) return;
            foreach(var d in OnMessage.GetInvocationList())
            {
                if (((NetRecevier)d).Invoke(sender, data))
                    break;
            }
        }

        public int Id { get; internal set; }
        public NetChannel(string name)
        {
            Id = NetEngine.GetHash(name);
            SendBuffer = new MemoryStream();
        }

        public void Send<T>(T data) where T : NetContent<T>
        {
            var writer = new BinaryWriter(SendBuffer);
            {
                writer.Write(Id);
                writer.Write(NetContent<T>.ContentTypeId);
                writer.Write(data.ContentId);
                var raw = NetEngine.Pack(data);
                writer.Write(raw.Length);
                writer.Write(raw);
            }
            Link.Transporter.Send(SendBuffer.ToArray());
            // Reset Send Buffer
            SendBuffer.Seek(0, SeekOrigin.Begin);
            SendBuffer.SetLength(0);
        }
    }
}
