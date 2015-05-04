using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PacketIn
{
    /// <summary>
    /// Abstract Transporter class with an asyncronous send/recv design.
    /// Connection and disconnection is syncronous.
    /// 
    /// Receving is done by calling Receive(), then listening on the OnReceive event.
    /// 
    /// Receive() should trigger one and ONLY one OnReceive-event per call.
    /// Class can be made Syncronous using locks.
    /// </summary>
    public abstract class NetTransporter
    {
        public abstract void Enable();
        public abstract void Disable();

        public abstract void SendAsync(byte[] data);
        public abstract void ReceiveAsync();

        public event NetRecevier OnReceive;
        public void Received(object sender, byte[] data)
        {
            if (OnReceive != null)
                OnReceive(sender, data);
        }

        public event Action<object, int> OnSent;
        public void Sent(object sender, int numBytes)
        {
            if (OnSent != null)
                OnSent(sender, numBytes);
        }
    }
}
