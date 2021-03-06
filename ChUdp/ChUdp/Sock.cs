﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChUdp
{
    class Sock
    {
        private readonly Socket socket;
        private readonly EndPoint broadcastEndPoint;
        private readonly int port;

        public Sock(int port = 12345) // 12345 is for broadcast
        {
            this.port = port;
            broadcastEndPoint = new IPEndPoint(IPAddress.Broadcast, port);
            socket = new Socket(SocketType.Dgram, ProtocolType.Udp) {EnableBroadcast = true};
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
        }

        private async Task<bool> SendBytesAsync(byte[] buffer)
        {
            return await Task.Run(() => socket.SendTo(buffer, broadcastEndPoint) == buffer.Length);
        }

        public async Task<bool> SendStringAsync(string s)
        {
            return await SendBytesAsync(Encoding.UTF8.GetBytes(s));
        }

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
