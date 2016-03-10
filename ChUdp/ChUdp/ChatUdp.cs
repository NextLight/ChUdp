using System;
using System.Net;
using System.Threading.Tasks;

namespace ChUdp
{
    class ChatUdp
    {
        readonly Sock sock;
        string name;
        readonly int port;
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
                NewMessage(this, new UdpMessageArgs(t.Item1, t.Item2));
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                SendName();
            }
        }

        private async Task<bool> SendNameAsync() =>
            await sock.SendStringAsync(name, Sock.Header.Name);

        private async void SendName() => await SendNameAsync(); // meh

        public async Task<bool> SendMessageAsync(string s) =>
            await sock.SendStringAsync(s, Sock.Header.Broadcast);

        public async Task<bool> SendMessageAsync(string s, IPAddress ip) =>
            await sock.SendStringAsync(s, Sock.Header.Private, new IPEndPoint(ip, port));
    }

    public class UdpMessageArgs : EventArgs
    {
        public string Message { get; private set; }
        public IPAddress Ip { get; private set; }

        public UdpMessageArgs(string message, IPAddress ip)
        {
            Message = message;
            Ip = ip;
        }
    }
}
