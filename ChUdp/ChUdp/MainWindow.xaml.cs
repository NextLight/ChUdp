using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ChUdp
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Sock sock;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            sock = new Sock();
            // TODO send name
            gridLogin.Visibility = Visibility.Hidden;
            gridMain.Visibility = Visibility.Visible;
            await Task.Run(async () =>
            {
                while (true)
                {
                    var t = await sock.ReceiveString();
                    textBlock.Text += t.Item2.ToString() + ": " + t.Item1 + '\n';
                    Thread.Sleep(70);
                }
            }
            );
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            sock.SendString(txtMessage.Text);
        }
    }
}
