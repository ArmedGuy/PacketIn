using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PacketIn
{
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
    }
}
