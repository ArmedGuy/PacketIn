using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PacketIn
{
    public abstract class NetContent<T>
        where T : NetContent<T>
    {
        public int ContentId { get; protected set; }

        // ContentProperties is only shared between instances of the same NetContent<T> (where T is specific)
        // so therefore this is ok.
        internal static IEnumerable<PropertyInfo> ContentProperties;
        public static int ContentTypeId { get; internal set; }

        // Reflection cache for Packing and Unpacking of NetContent
        static NetContent()
        {
            var props = typeof(T).GetProperties()
                .Where(x => Attribute.IsDefined(x, typeof(NetDataAttribute)));
            ContentProperties = props
                .OrderBy(
                    x =>
                        ((NetDataAttribute)
                            Attribute.GetCustomAttribute(x, typeof(NetDataAttribute))).Index);

            ContentTypeId = NetEngine.GetHash(typeof (T).Name);
        }

        /// <summary>
        /// Makes the NetContent listen to a channel for model updates.
        /// </summary>
        /// <param name="channel">The channel to listen on.</param>
        public void Listen(NetChannel channel)
        {
            channel.OnMessage += channel_OnMessage;
        }


        /// <summary>
        /// Callback from Channel, filters messages based on Content Type and ContentId.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        internal bool channel_OnMessage(object sender, byte[] data)
        {
            using (var reader = new BinaryReader(new MemoryStream(data)))
            {
                reader.ReadInt32(); // pass channel ID
                if (ContentTypeId != reader.ReadInt32())
                    return false;
                if (ContentId != reader.ReadInt32())
                    return false;
                var len = reader.ReadInt32();
                var pack = reader.ReadBytes(len);
                NetEngine.Unpack((T)this, pack);
            }
            return true;
        }
    }
}
