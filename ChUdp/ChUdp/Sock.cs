using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChUdp
{
    class Sock
    {
        public enum Header : byte { Name = 1, Broadcast, Private }
        Socket socket;
        readonly IPEndPoint broadcastEndPoint;
        readonly int port;

        public Sock(int port)
        {
            this.port = port;
            socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            socket.EnableBroadcast = true;
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            broadcastEndPoint = new IPEndPoint(IPAddress.Broadcast, port);
        }

        private async Task<bool> SendBytesAsync(byte[] buffer, IPEndPoint endPoint) =>
            await Task.Run(() => socket.SendTo(buffer, endPoint) == buffer.Length);

        public async Task<bool> SendStringAsync(string s, Header h, IPEndPoint endPoint) =>
            await SendBytesAsync(Encoding.UTF8.GetBytes((char)0 + (char)h + s), endPoint);

        public async Task<bool> SendStringAsync(string s, Header h) =>
            await SendStringAsync(s, h, broadcastEndPoint);

        private async Task<Tuple<byte[], IPAddress>> ReceiveBytesAsync()
        {
            byte[] buffer = new byte[65536];
            EndPoint sender = new IPEndPoint(IPAddress.Any, port);
            await Task.Run(() => Array.Resize(ref buffer, socket.ReceiveFrom(buffer, ref sender)));
            return new Tuple<byte[], IPAddress>(buffer, ((IPEndPoint)sender).Address);
        }
        
        public async Task<Tuple<string, IPAddress>> ReceiveStringAsync()
        {
            var t = await ReceiveBytesAsync();
            return new Tuple<string, IPAddress>(Encoding.UTF8.GetString(t.Item1), t.Item2);
        }
    }
}

