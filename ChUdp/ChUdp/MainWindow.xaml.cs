using System;
using System.Windows;
using System.Windows.Controls;

namespace ChUdp
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Sock sock;
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
            txtMessage.Focus();
            while (true)
            {
                var t = await sock.ReceiveStringAsync();
                textBlock.Text += t.Item2.MapToIPv4() + ": " + t.Item1 + '\n';
                scrollViewer.ScrollToBottom();
            }
        }

        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            await sock.SendStringAsync(txtMessage.Text);
        }
    }
}
