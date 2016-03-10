using System;
using System.Windows;

namespace ChUdp
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChatUdp chat;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            gridLogin.Visibility = Visibility.Hidden;
            gridMain.Visibility = Visibility.Visible;
            chat = new ChatUdp(txtName.Text);
            chat.NewMessage += Chat_NewMessage;
        }

        private void Chat_NewMessage(object sender, UdpMessageArgs e)
        {
            // TODO
        }

        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            await chat.SendMessageAsync(txtMessage.Text);
        }
    }
}
