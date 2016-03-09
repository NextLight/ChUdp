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
        int port;
        // 12345 is broadcast
        public Sock(int port = 12345)
        {
            this.port = port;
            socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            socket.EnableBroadcast = true;
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            socket.Connect(IPAddress.Broadcast, port);
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
            byte[] buffer = new byte[65536];
            EndPoint sender = new IPEndPoint(IPAddress.Any, port);
            await Task.Run(() => Array.Resize(ref buffer, socket.ReceiveFrom(buffer, ref sender)));
            return new Tuple<byte[], IPAddress>(buffer, ((IPEndPoint)sender).Address);
        }
        
        public async Task<Tuple<string, IPAddress>> ReceiveString()
        {
            var t = await ReceiveBytes();
            return new Tuple<string, IPAddress>(Encoding.UTF8.GetString(t.Item1), t.Item2);
        }
    }
}
