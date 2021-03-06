﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PacketIn.Transporters
{
    public class ManualTransporter : NetTransporter
    {
        public Queue<byte[]> Buffer = new Queue<byte[]>(); 
        public override void Enable()
        {
        }

        public override void Disable()
        {
        }

        public override void SendAsync(byte[] data)
        {
            Buffer.Enqueue(data);
            Sent(this, data.Length);
        }

        public override void ReceiveAsync()
        {
            (new Thread(() =>
            {
                while(Buffer.Count == 0)
                    Thread.Sleep(1);
                var data = Buffer.Dequeue();
                Received(Buffer, data);
            })).Start();
        }
    }
}
