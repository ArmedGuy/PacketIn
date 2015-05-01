using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PacketIn.Transporters
{
    public class AsyncClientTransporter : NetTransporter
    {
        internal class ReceiveState
        {
            internal byte[] Buffer;
            internal static readonly int BufferSize = 256;
            internal MemoryStream ReceiveBuffer;

            public ReceiveState()
            {
                Buffer = new byte[BufferSize];
                ReceiveBuffer = new MemoryStream();
            }
        }


        private readonly Socket _socket;
        private readonly string _host;
        private readonly int _port;

        public AsyncClientTransporter(string host, int port)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _host = host;
            _port = port;
        }
        public override void Enable()
        {
            _socket.Connect(_host, _port);
        }

        public override void Disable()
        {
            _socket.Close();
        }

        public override void Send(byte[] data)
        {
            _socket.BeginSend(data, 0, data.Length, SocketFlags.None, (iar) =>
            {
                _socket.EndSend(iar);
            }, _socket);
        }

        public override void Receive()
        {
            var state = new ReceiveState();
            _socket.BeginReceive(state.Buffer, 0, ReceiveState.BufferSize, SocketFlags.None, ReceiveCallback, state);
        }

        internal void ReceiveCallback(IAsyncResult iar)
        {
            try
            {
                var state = (ReceiveState) iar.AsyncState;

                var read = _socket.EndReceive(iar);
                if (read > 0)
                {
                    state.ReceiveBuffer.Write(state.Buffer, 0, read);
                    _socket.BeginReceive(state.Buffer, 0, ReceiveState.BufferSize, 0, ReceiveCallback, state);
                }
                else
                {
                    var data = state.ReceiveBuffer.ToArray();
                    Received(_socket, data);
                }
            }
            catch (Exception ex)
            {
                Disable();
                // TODO: handle case
            }
        }
    }
}
