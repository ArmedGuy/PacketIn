using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PacketIn
{
    /// <summary>
    /// Abstract Transporter class with an asyncronous design.
    /// 
    /// Receving is done by calling Receive(), then listening on the OnReceive event.
    /// </summary>
    public abstract class NetTransporter
    {
        public abstract void Enable();
        public abstract void Disable();

        public abstract void Send(byte[] data);
        public abstract void Receive();

        public event NetRecevier OnReceive;
        public void Received(object sender, byte[] data)
        {
            if (OnReceive != null)
                OnReceive(sender, data);
        }

        public event Action<object> OnConnected;
        public void Connected(object sender)
        {
            if (OnConnected != null)
                OnConnected(sender);
        }
    }
}
