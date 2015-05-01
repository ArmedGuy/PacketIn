using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PacketIn;
using PacketIn.Transporters;

namespace PacketInTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var ex = new ExamplePlayer {Id = 3, Name = "walla", Score = 2};
            var ex2 = new ExamplePlayer {Id = 4, Name = "Sten", Score = 45};

            var ts = new ManualTransporter();
            var nl = new NetLink(ts);

            nl.DefaultChannel.OnMessage += DefaultChannel_OnMessage;
            nl.Send(ex);
            nl.Enable();


            var chan = new NetChannel("player");

            nl.AddChannel(chan);

            ex2.Listen(chan);
            chan.Send(ex);

            while (true)
            {
                Console.WriteLine(ex.Name);
                Console.WriteLine(ex2.Name);

                Console.WriteLine(ex.Score);
                Console.WriteLine(ex2.Score);
                Thread.Sleep(200);
            }

        }

        static bool DefaultChannel_OnMessage(object sender, byte[] data)
        {
            Console.WriteLine(Encoding.ASCII.GetString(data));
            return true;
        }
    }
}
