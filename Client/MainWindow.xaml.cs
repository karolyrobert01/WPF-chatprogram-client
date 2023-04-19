using System;
using System.Windows;
using System.Net;
using System.Net.Sockets;
using System.Windows.Input;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    //KÉSZ

    public partial class MainWindow : Window
    {

        public Socket clientSocket;
        public string strName;
        public string strPass;

        public delegate string getNameDelegate();
        public delegate string getPassDelegate();
        public delegate void UjFormDelegate();

        public MainWindow()
        {
            InitializeComponent();
        }

        public string getLoginName()
        {
            return this.textBox1.Text;
        }

        public string getIP()
        {
            return this.textBox2.Text;
        }
        public string getLoginPass()
        {
            return this.textBox3.Password;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //IPAddress ipAddress = IPAddress.Parse(this.textBox2.Text);

                //getNameDelegate IP = new getNameDelegate(getIP);
                //l_ip = (string)this.Dispatcher.Invoke(IP, null);
                IPAddress ipAddress = IPAddress.Parse(this.textBox2.Text);
                //Server is listening on port 1000
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 1000);

                //Connect to the server
                //clientSocket.Connect(ipEndPoint);
                clientSocket.BeginConnect(ipEndPoint, new AsyncCallback(OnConnect), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGSclient1");
            }
        }

        private void OnReceive(IAsyncResult ar)
        {
            //Socket clientSocket = (Socket)ar.AsyncState;
            try
            {
                clientSocket.EndSend(ar);

                byte[] byteData = new byte[1024];
                //Várunk a válaszra
                clientSocket.Receive(byteData, 0, 1024, SocketFlags.None);

                Data msgRECE = new Data(byteData);
                StatusModifyDelegate deleg = new StatusModifyDelegate(StatusModify);
                if (msgRECE.cmdCommand == Command.Accept)
                {
                    this.Dispatcher.Invoke(deleg, "Regisztrálva a chat programunkba");
                }
                else
                {
                    this.Dispatcher.Invoke(deleg, "Mar van ilyen felhasznalo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGSclient9");
            }
        }

        private void OnSend(IAsyncResult ar)
        {
            try
            {

                clientSocket.EndSend(ar);
                byte[] byteData = new byte[1024];

                //Várunk a válaszra
                clientSocket.Receive(byteData, 0, 1024, SocketFlags.None);

                Data msg = new Data(byteData);

                if (msg.cmdCommand == Command.Decline)
                {
                    StatusModifyDelegate deleg = new StatusModifyDelegate(StatusModify);
                    this.Dispatcher.Invoke(deleg, "Hibás felhasználónév vagy jelszó!");
                }
                else
                {
                    UjFormDelegate pForm = new UjFormDelegate(UjForm);
                    this.Dispatcher.Invoke(pForm, null);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGSclient2");
            }
        }

        private void UjForm()
        {
            ClientMessage uj_form;
            uj_form = new ClientMessage(clientSocket, textBox1.Text);
            uj_form.Show();
            Close();
        }

        private void OnConnect(IAsyncResult ar)
        {
            try
            {
                //clientSocket.EndConnect(ar);

                //We are connected so we login into the server
                string l_fhName;
                string l_fhPass;
                Data msgToSend = new Data();
                msgToSend.cmdCommand = Command.Login;

                //l_fhName = this.textBox1.Text;
                getNameDelegate fhName = new getNameDelegate(getLoginName);
                l_fhName = (string)this.textBox1.Dispatcher.Invoke(fhName, null);

                getPassDelegate fhPass = new getPassDelegate(getLoginPass);
                l_fhPass = (string)this.textBox3.Dispatcher.Invoke(fhPass, null);


                msgToSend.strName = l_fhName;
                msgToSend.strPass = l_fhPass;
                msgToSend.strMessage = null;
                msgToSend.strAddres = null;

                byte[] b = msgToSend.ToByte();

                //Send the message to the server
                clientSocket.BeginSend(b, 0, b.Length, SocketFlags.None, new AsyncCallback(OnSend), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGSclient3");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //IPAddress ipAddress = IPAddress.Parse(this.textBox2.Text);

                //getNameDelegate IP = new getNameDelegate(getIP);
                //l_ip = (string)this.Dispatcher.Invoke(IP, null);
                IPAddress ipAddress = IPAddress.Parse(this.textBox2.Text);
                //Server is listening on port 1000
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 1000);

                //Connect to the server
                //clientSocket.Connect(ipEndPoint);
                clientSocket.BeginConnect(ipEndPoint, new AsyncCallback(OnConnect2), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGSclient4");
            }

        }
        private void OnConnect2(IAsyncResult ar)
        {
            try
            {
                //clientSocket.EndConnect(ar);

                //We are connected so we login into the server
                string l_fhName;
                string l_fhPass;
                Data msgToSend = new Data();
                msgToSend.cmdCommand = Command.Register;

                //l_fhName = this.textBox1.Text;
                getNameDelegate fhName = new getNameDelegate(getLoginName);
                l_fhName = (string)this.textBox1.Dispatcher.Invoke(fhName, null);

                getPassDelegate fhPass = new getPassDelegate(getLoginPass);
                l_fhPass = (string)this.textBox3.Dispatcher.Invoke(fhPass, null);


                msgToSend.strName = l_fhName;
                msgToSend.strPass = l_fhPass;
                msgToSend.strMessage = null;
                msgToSend.strAddres = null;

                byte[] b = msgToSend.ToByte();

                //Send the message to the server
                clientSocket.BeginSend(b, 0, b.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGSclient5");
            }
        }

        public delegate void StatusModifyDelegate(string message);
        void StatusModify(string message)
        {
            label5.Content = message;
            label5.Visibility = Visibility.Visible;
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

