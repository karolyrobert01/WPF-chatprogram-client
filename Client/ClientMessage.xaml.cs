using System;
using System.Windows;
using System.Net.Sockets;
using System.Windows.Input;

namespace Client
{
    /// <summary>
    /// Interaction logic for ClientMessage.xaml
    /// </summary>

    public partial class ClientMessage : Window
    {


        public Socket ClientSocket;
        public string LoginName;
        public string LoginPass;
        byte[] byteData = new byte[1024];
        private delegate void UpdateDelegate(string pMessage);

        private void UpdateMessage(string pMessage)
        {
            this.textBox1.Text += pMessage;
        }

        public ClientMessage()
        {
            InitializeComponent();
            Label3.Content = LoginName;
        }



        public ClientMessage(Socket pSocket, String pName)
        {
            InitializeComponent();

            ClientSocket = pSocket;
            LoginName = pName;
            this.Title = pName;

            ClientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None,
                    new AsyncCallback(OnReceive), ClientSocket);

            //ClientSocket.Receive(byteData,SocketFlags.None);
            Label3.Content = LoginName;
        }

        private void OnReceive(IAsyncResult ar)
        {

            //Socket clientSocket = (Socket)ar.AsyncState;
            try
            {
                ClientSocket.EndReceive(ar);

                //Transform the array of bytes received from the user into an
                //intelligent form of object Data
                Data msgReceived = new Data(byteData);

                ClientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnReceive), ClientSocket);
                if (msgReceived.strMessage != null)
                {
                    UpdateDelegate update = new UpdateDelegate(UpdateMessage);
                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, update,
                    msgReceived.strMessage + "\r\n");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Megszunt a szerverel a kapcsolat!", "Üzenet", MessageBoxButton.OK, MessageBoxImage.Error);
                ClientSocket.Close();
                Environment.Exit(1);
            }

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Data msgToSend = new Data();
            msgToSend.cmdCommand = Command.Message;
            msgToSend.strName = LoginName;
            msgToSend.strPass = LoginPass;
            msgToSend.strMessage = textBox2.Text;
            msgToSend.strAddres = textBox3.Text;

            byte[] b = msgToSend.ToByte();
            ClientSocket.Send(b);
        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Data msgToSend = new Data();
            msgToSend.strName = LoginName;
            msgToSend.strPass = LoginPass;
            msgToSend.strMessage = null;
            msgToSend.strAddres = null;
            msgToSend.cmdCommand = Command.Logout;
            byte[] b = msgToSend.ToByte();
            ClientSocket.Send(b);
            Close();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Data msgToSend = new Data();
            msgToSend.strName = LoginName;
            msgToSend.strPass = LoginPass;
            msgToSend.strMessage = null;
            msgToSend.strAddres = null;
            msgToSend.cmdCommand = Command.List;
            byte[] b = msgToSend.ToByte();
            ClientSocket.Send(b);
        }
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
