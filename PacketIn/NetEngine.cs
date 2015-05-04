using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PacketIn
{
    #region Engine Delegates
    public delegate void NetPacker(object obj, BinaryWriter writer);

    public delegate object NetUnpacker(BinaryReader reader);

    public delegate bool NetRecevier(object sender, byte[] data);
    #endregion

    public static class NetEngine
    {
        internal static Dictionary<Type, NetPacker> Packers = new Dictionary<Type, NetPacker>(); 
        internal static Dictionary<Type, NetUnpacker> Unpackers = new Dictionary<Type, NetUnpacker>();

        internal static MD5 HashGeneratorMd5;

        /// <summary>
        /// Define a Packer function for type T.
        /// </summary>
        /// <typeparam name="T">The type to pack.</typeparam>
        /// <param name="p">A function that can pack a value of type T.</param>
        public static void PackerFor<T>(NetPacker p)
        {
            Packers[typeof(T)] = p;
        }

        /// <summary>
        /// Define an Unpacker function for type T.
        /// </summary>
        /// <typeparam name="T">The type to unpack.</typeparam>
        /// <param name="p">A function that can unpack a value of type T.</param>
        public static void UnpackerFor<T>(NetUnpacker p)
        {
            Unpackers[typeof (T)] = p;
        }

        static NetEngine()
        {
            #region Default Packers
            PackerFor<int>      ((obj, writer) =>
            {
                writer.Write((int)obj);
            });
            PackerFor<double>   ((obj, writer) =>
            {
                writer.Write((double)obj);
            });
            PackerFor<char>     ((obj, writer) =>
            {
                writer.Write((char)obj);
            });
            PackerFor<bool>     ((obj, writer) =>
            {
                writer.Write((bool)obj);
            });
            PackerFor<long>     ((obj, writer) =>
            {
                writer.Write((long)obj);
            });
            PackerFor<float>    ((obj, writer) =>
            {
                writer.Write((float)obj);
            });
            PackerFor<short>    ((obj, writer) =>
            {
                writer.Write((short)obj);
            });
            PackerFor<double>   ((obj, writer) =>
            {
                writer.Write((double)obj);
            });
            PackerFor<ulong>    ((obj, writer) =>
            {
                writer.Write((ulong)obj);
            });
            PackerFor<ushort>   ((obj, writer) =>
            {
                writer.Write((ushort)obj);
            });

            PackerFor<string>   ((obj, writer) =>
            {
                writer.Write((string)obj);
            });
            #endregion

            #region Default Unpackers
            UnpackerFor<string>(reader => reader.ReadString());
            UnpackerFor<int>(reader => reader.ReadInt32());
            UnpackerFor<double>(reader => reader.ReadDouble());
            UnpackerFor<char>(reader => reader.ReadChar());
            UnpackerFor<bool>(reader => reader.ReadBoolean());
            UnpackerFor<float>(reader => reader.ReadSingle());
            UnpackerFor<long>(reader => reader.ReadInt64());
            UnpackerFor<short>(reader => reader.ReadInt16());
            UnpackerFor<double>(reader => reader.ReadDouble());
            UnpackerFor<ulong>(reader => reader.ReadUInt64());
            UnpackerFor<ushort>(reader => reader.ReadUInt16());
            #endregion

            HashGeneratorMd5 = MD5.Create();
            HashGeneratorMd5.Initialize();
        }

        /// <summary>
        /// Pack NetContent for sending.
        /// </summary>
        /// <typeparam name="T">Type of NetContent to send.</typeparam>
        /// <param name="content">The NetContent to pack.</param>
        /// <returns></returns>
        public static byte[] Pack<T>(T content) where T : NetContent<T>
        {
            var mStream = new MemoryStream();
            var bWriter = new BinaryWriter(mStream);
            
            foreach (var prop in NetContent<T>.ContentProperties)
            {
                var val = prop.GetValue(content, null);
                var vType = val.GetType();
                if (Packers.ContainsKey(vType))
                {
                    Packers[vType].Invoke(val, bWriter);
                }
                else
                {
                    throw new Exception("No Packer exists for specified type.");
                }
            }

            return mStream.ToArray();
        }

        /// <summary>
        /// Unpack raw NetContent data and apply it to Target.
        /// </summary>
        /// <typeparam name="T">The type of the target.</typeparam>
        /// <param name="targetContent">The target NetContent to apply new data to.</param>
        /// <param name="data">The raw data to apply.</param>
        public static void Unpack<T>(T targetContent, byte[] data) where T : NetContent<T>
        {
            var mStream = new MemoryStream(data);
            var bReader = new BinaryReader(mStream);
            foreach (var prop in NetContent<T>.ContentProperties)
            {
                if (Unpackers.ContainsKey(prop.PropertyType))
                {
                    var obj = Unpackers[prop.PropertyType].Invoke(bReader);
                    prop.SetValue(targetContent, obj);
                }
                else
                {
                    throw new Exception("No Unpacker exists for specified type.");
                }
            }
        }

        /// <summary>
        /// Hashes a string to a 32 bit integer, used for identification of NetChannels and ContentTypes.
        /// </summary>
        /// <param name="id">String ID</param>
        /// <returns>Hashed Integer ID</returns>
        public static int GetHash(string id)
        {
            var b = Encoding.UTF8.GetBytes(id);
            var output = HashGeneratorMd5.ComputeHash(b);
            return BitConverter.ToInt32(output, 0);
        }
    }
}
