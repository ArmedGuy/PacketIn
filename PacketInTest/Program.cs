using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using PacketIn;

namespace PacketInTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var ex = new ExamplePlayer {Id = 3, Name = "walla", Score = 2};
            var output = NetEngine.Pack(ex);
            Console.WriteLine(output.Length);
            Console.ReadKey();
        }
    }
}
