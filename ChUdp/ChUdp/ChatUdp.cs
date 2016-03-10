using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ChUdp
{
    class ChatUdp
    {
        readonly Sock sock;
        string name;
        readonly int port;
        Dictionary<IPAddress, string> namesDict = new Dictionary<IPAddress, string>();
        public ChatUdp(string name, int port = 12345)
        {
            sock = new Sock(port);
            Name = name;
            this.port = port;
            RecieveLoop();
        }

        public event EventHandler<UdpMessageArgs> NewMessage;

        private async void RecieveLoop()
        {
            while (true)
            {
                var t = await sock.ReceiveStringAsync();
                if (t.Item1[0] == 0)
                {
                    Sock.Header h;
                    switch (h = (Sock.Header)t.Item1[1])
                    {
                        case Sock.Header.Broadcast:
                        case Sock.Header.Private:
                            NewMessage(this, new UdpMessageArgs(t.Item1, t.Item2, namesDict[t.Item2], h == Sock.Header.Broadcast));
                            break;
                        case Sock.Header.NameRequest:
                            await SendNameAsync(false);
                            namesDict[t.Item2] = t.Item1;
                            break;
                        case Sock.Header.NameReply:
                            namesDict[t.Item2] = t.Item1;
                            break;
                    }
                }
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                SendName(true);
            }
        }

        private async Task<bool> SendNameAsync(bool request) =>
            await sock.SendStringAsync(name, request ? Sock.Header.NameRequest : Sock.Header.NameReply);

        private async void SendName(bool request) => await SendNameAsync(request); // meh

        public async Task<bool> SendMessageAsync(string s) =>
            await sock.SendStringAsync(s, Sock.Header.Broadcast);

        public async Task<bool> SendMessageAsync(string s, IPAddress ip) =>
            await sock.SendStringAsync(s, Sock.Header.Private, new IPEndPoint(ip, port));
    }

    public class UdpMessageArgs : EventArgs
    {
        public string Message { get; private set; }
        public IPAddress Ip { get; private set; }
        public string Name { get; private set; }
        public bool Broadcast { get; private set; }

        public UdpMessageArgs(string s, IPAddress ip, string name, bool broadcast)
        {
            Message = s;
            Ip = ip;
            Name = name;
            Broadcast = broadcast;
        }
    }
}
