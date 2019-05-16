using System.Windows;
using System.Windows.Controls;
using System.IO.Ports;
using Microsoft.Win32;
using System.IO;

namespace ArduinoTerminal
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private string m_selectedPort = "";
        private readonly SerialPort m_serialPort = new SerialPort();
        public delegate void AddDataDelegate(string myString);
        public AddDataDelegate m_myDelegate;
        public delegate void ClearDelegate ();
        public ClearDelegate m_myClearDelegate;
        private string m_theFile;

        public MainWindow()
        {
            InitializeComponent();
            ListAllPorts();
            m_myDelegate = AddDataMethod;
            m_myClearDelegate = ClearMethod;
        }

        ~MainWindow ()
        {
            if (m_serialPort.IsOpen)
                m_serialPort.Close ();
        }

        private void ListAllPorts()
        {
            portlist.Items.Clear ();
            // Get a list of serial port names.
            var ports = SerialPort.GetPortNames();

            // Display each port name to the console.
            foreach (var port in ports)
            {
                portlist.Items.Add(port);
            }
        }

        private void portlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (portlist.SelectedIndex == -1)
                return;

            m_selectedPort = portlist.SelectedValue.ToString ();

            if (m_serialPort.IsOpen)
                m_serialPort.Close ();

            m_serialPort.BaudRate = 115200;
            m_serialPort.DataBits = 8;
            m_serialPort.Handshake = Handshake.XOnXOff;
            m_serialPort.Parity = Parity.None;
            m_serialPort.PortName = m_selectedPort;
            m_serialPort.StopBits = StopBits.One;
            m_serialPort.DataReceived += _serialPort_DataReceived;

            m_serialPort.Open();
        }

        private void sendbtn_Click(object sender, RoutedEventArgs e)
        {
            if (filePath.Text.Length <= 0)
                return;

            if (!File.Exists(filePath.Text))
                return;

            m_theFile = filePath.Text;

            //Sendet ein "r" an den Arduino um ihn auf den Empfang einer Datei vorzubereiten
            m_serialPort.Write("r");
            //Eine Sekunde später wird der Dateiname gesendet
            System.Threading.Thread.Sleep(1000);
            var filename = Path.GetFileName(filePath.Text);
            m_serialPort.Write(filename);
            // Danach sendet der Arduino in "C" um sich bereit zum Empfang zu melden
        }

        public void AddDataMethod(string myString)
        {
            textBox.AppendText(myString);
            if( scrollToEnd.IsChecked != null && (bool)scrollToEnd.IsChecked )
                textBox.ScrollToEnd();
        }

        public void ClearMethod ()
        {
            textBox.Clear ();
        }

        void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var sp = (SerialPort)sender;
            var s = sp.ReadExisting();

            if(s != "C")
                textBox.Dispatcher.Invoke(m_myDelegate, s);
            else
            {
                var xmodem = new XModem(sp);

                var array = File.ReadAllBytes(m_theFile);

                xmodem.XmodemTransmit(array, array.Length, false);
            }
        }

        private void filebtn_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                filePath.Text = openFileDialog.FileName;

        }

        private void FileListbtn_Click(object sender, RoutedEventArgs e)
        {
            //Sendet ein "l" an den Arduino um die Liste anzufordern
            m_serialPort.Write("l");
        }

        private void delbtn_Click(object sender, RoutedEventArgs e)
        {
            if (delFilename.Text.Length <= 0)
                return;

            m_theFile = delFilename.Text;

            //Sendet ein "d" an den Arduino um ihn auf den Empfang der zu löschenden Datei vorzubereiten
            m_serialPort.Write("d");
            //Eine Sekunde später wird der Dateiname gesendet
            System.Threading.Thread.Sleep(1000);
            m_serialPort.Write(m_theFile);
        }

        private void Button_Click (object sender, RoutedEventArgs e)
        {
            if (m_serialPort.IsOpen)
                m_serialPort.Close ();

            portlist.SelectedIndex = -1;
        }

        private void Button_Click_Send_Input(object sender, RoutedEventArgs e)
        {
            m_serialPort.Write(editCtrl_SendString.Text);
        }

        private void Button_Click_1 (object sender, RoutedEventArgs e)
        {
            textBox.Dispatcher.Invoke (m_myClearDelegate);
        }

        private void Button_Reload (object sender, RoutedEventArgs e)
        {
            ListAllPorts ();
        }

    }


    
}
