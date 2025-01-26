using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HERO_GUI
{
    public partial class MainWindow : Window
    {
        //constants
        string sensorTCPIp = "192.168.4.1";
        int sensorTCPPort = 67890;
        string jetsonTCPIp = "192.168.4.1";
        int jetsonTCPPort = 12345;
        private Thread jetsonTcpThread;
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private bool jetsonTcpIsRunning = true;
        private ManualResetEvent messageReceivedEvent = new ManualResetEvent(false);
        private string receivedMessage = string.Empty;

        //variables
        int velocity = 50;
        int rideHeigth = 200;

        public MainWindow()
        {
            InitializeComponent();


            //Starting Threads
            //Thread tcpSensor_readThread = new Thread(() => tcpSensor_ReadContin(sensorTCPIp, sensorTCPPort));
            //Start the Jetson TCP Control Thread
            jetsonTcpThread = new Thread(InitializeTcpConnection);
            jetsonTcpThread.IsBackground = true;
            jetsonTcpThread.Start();
        }

        private void InitializeTcpConnection()
        {
            try
            {
                tcpClient = new TcpClient(jetsonTCPIp, jetsonTCPPort);
                networkStream = tcpClient.GetStream();

                while (jetsonTcpIsRunning)
                {
                    if (networkStream.DataAvailable)
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                        receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                        // Check if we have a waiting thread
                        if (!string.IsNullOrEmpty(receivedMessage))
                        {
                            messageReceivedEvent.Set(); // Unblock waiting thread
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void SendMessage(string message)
        {
            if(networkStream != null && networkStream.CanWrite)
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                networkStream.Write(data, 0, data.Length);
            }
        }

        private void SendAndWaitForResponse(string messageToSend, string expectedResponse, string answer)
        {
            new Thread(() =>
            {
                SendMessage(messageToSend);
                messageReceivedEvent.Reset(); // Prepare to wait

                // Wait until the expected response is received
                bool received = messageReceivedEvent.WaitOne(TimeSpan.FromSeconds(10)); // Wait up to 10 seconds
                if (received && receivedMessage == expectedResponse)
                {
                    SendMessage(answer);
                }
                else
                {

                }
            }).Start();
        }

        //movement control methods
        private void move_forward_btn_Click(object sender, RoutedEventArgs e)
        {
            int veloc = Convert.ToInt16(velocity_sld.Value);
            new Thread(() => SendAndWaitForResponse("changeAngle0", "changedAngle",$"moveForward{veloc}")).Start();
        }
        private void velocity_sld_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            velocity = (int)velocity_sld.Value;
        }

        private void height_sld_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            rideHeigth = (int)height_sld.Value;
        }


        //camera control methods
        private void camLaunch_btn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void camStop_btn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void camRec_btn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void camStopRec_btn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void move_stop_btn_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() => SendMessage("moveN230")).Start();
        }

        private void move_left_btn_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() => SendAndWaitForResponse("changeAngle-30", "changedAngle", "moveForward5")).Start();
        }

        private void move_right_btn_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() => SendAndWaitForResponse("changeAngle30", "changedAngle", "moveForward5")).Start();
        }
    }
}
