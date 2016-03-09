using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChUdp
{
    class Sock
    {
        Socket socket;
        // 12345 is broadcast
        public Sock(int port = 12345)
        {
            socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            socket.Connect(new IPEndPoint(IPAddress.Broadcast, port));
        }

        public void SendBytes(byte[] buffer)
        {
            socket.Send(buffer);
        }

        public void SendString(string s)
        {
            SendBytes(Encoding.UTF8.GetBytes(s));
        }

        public async Task<Tuple<byte[], IPAddress>> ReceiveBytes()
        {
            byte[] buffer = new byte[socket.Available];
            EndPoint sender = new IPEndPoint(IPAddress.Any, 1234);
            await Task.Run(() => socket.ReceiveFrom(buffer, ref sender));
            return new Tuple<byte[], IPAddress>(buffer, ((IPEndPoint)sender).Address);
        }

        public async Task<Tuple<string, IPAddress>> ReceiveString()
        {
            var t = await ReceiveBytes();
            return new Tuple<string, IPAddress>(Encoding.UTF8.GetString(t.Item1), t.Item2);
        }
    }
}
