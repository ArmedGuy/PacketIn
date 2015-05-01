using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PacketIn
{
    #region Engine Delegates
    public delegate byte[] NetPacker(object obj);

    public delegate object NetUnpacker(byte[] data);

    public delegate bool NetRecevier(object sender, byte[] data);
    #endregion

    public static class NetEngine
    {
        internal static Dictionary<Type, NetPacker> Packers = new Dictionary<Type, NetPacker>(); 
        internal static Dictionary<Type, NetUnpacker> Unpackers = new Dictionary<Type, NetUnpacker>();

        public static void PackerFor<T>(NetPacker p)
        {
            Packers[typeof(T)] = p;
        }

        public static void UnpackerFor<T>(NetUnpacker p)
        {
            Unpackers[typeof (T)] = p;
        }

        static NetEngine()
        {
            #region Default Packers
            PackerFor<int>      (obj => BitConverter.GetBytes((int)obj));
            PackerFor<double>   (obj => BitConverter.GetBytes((double)obj));
            PackerFor<float>    (obj => BitConverter.GetBytes((float)obj));
            PackerFor<char>     (obj => BitConverter.GetBytes((char)obj));
            PackerFor<bool>     (obj => BitConverter.GetBytes((bool)obj));
            PackerFor<long>     (obj => BitConverter.GetBytes((long)obj));
            PackerFor<short>    (obj => BitConverter.GetBytes((short)obj));
            PackerFor<double>   (obj => BitConverter.GetBytes((double)obj));
            PackerFor<ulong>    (obj => BitConverter.GetBytes((ulong)obj));
            PackerFor<ushort>   (obj => BitConverter.GetBytes((ushort)obj));

            PackerFor<string>   (obj => Encoding.UTF8.GetBytes((string)obj));
            #endregion
        }

        public static byte[] Pack<T>(T snapshot) where T : NetSnapshot
        {
            var mStream = new MemoryStream();
            var t = typeof (T);
            var props = t.GetProperties()
                .Where(x => Attribute.IsDefined(x, typeof(SnapshotDataAttribute)));
            props = props
                .OrderBy(
                    x =>
                        ((SnapshotDataAttribute)
                            Attribute.GetCustomAttribute(x, typeof (SnapshotDataAttribute))).Index);
            foreach (var prop in props)
            {
                var val = prop.GetValue(snapshot, null);
                var vType = val.GetType();
                if (Packers.ContainsKey(vType))
                {
                    var buf = Packers[vType].Invoke(val);
                    mStream.Write(buf, 0, buf.Length);
                }
                else
                {
                    throw new Exception("No Packer exists for specified type.");
                }
            }

            return mStream.ToArray();
        }
    }
}
