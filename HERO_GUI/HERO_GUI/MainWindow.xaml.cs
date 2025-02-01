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
using System.Windows.Media.Animation;
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

        private bool heightThreadRunning = false;

        //variables
        int velocity = 50;
        int rideHeigth = 200;

        public MainWindow()
        {
            InitializeComponent();
            double percent = .1;
            //Drawing SCOUT's distance circles
            DrawPizzaSlice(348, 380, 100, 50, 80, Brushes.Green);
            DrawPizzaSlice(348, 380, 60, 50, 80, Brushes.Yellow);
            DrawPizzaSlice(348, 380, 30, 50, 80, Brushes.Red);
            DrawRingSlice(348, 380, Convert.ToInt16(percent*100), Convert.ToInt16(percent * 100 + 5), 45, 90, Brushes.LightGray);

            DrawPizzaSlice(348, 180, 100, 230, 80, Brushes.Green);
            DrawPizzaSlice(348, 180, 60, 230, 80, Brushes.Yellow);
            DrawPizzaSlice(348, 180, 30, 230, 80, Brushes.Red);

            DrawPizzaSlice(298, 280, 100, 120, 120, Brushes.Green);
            DrawPizzaSlice(298, 280, 60, 120, 120, Brushes.Yellow);
            DrawPizzaSlice(298, 280, 30, 120, 120, Brushes.Red);

            DrawPizzaSlice(398, 280, 100, 300, 120, Brushes.Green);
            DrawPizzaSlice(398, 280, 60, 300, 120, Brushes.Yellow);
            DrawPizzaSlice(398, 280, 30, 300, 120, Brushes.Red);

            //Starting Threads
            //Thread tcpSensor_readThread = new Thread(() => tcpSensor_ReadContin(sensorTCPIp, sensorTCPPort));
            //Start the Jetson TCP Control Thread
            jetsonTcpThread = new Thread(InitializeTcpConnection);
            jetsonTcpThread.IsBackground = true;
            jetsonTcpThread.Start();

            double batteryPercent = 0.5;

            //Canvas.SetLeft(batLevel_rec, (int)490*batteryPercent);

            // Create the animation
            DoubleAnimation animation = new DoubleAnimation
            {
                From = 0,                  // Start position
                To = 490,                 // End position
                Duration = TimeSpan.FromSeconds(2), // Duration of animation
                EasingFunction = new QuadraticEase() // Optional: Add easing for smoothness
            };

            // Apply the animation to the Canvas.Left property
            batLevel_rec.BeginAnimation(Canvas.LeftProperty, animation);
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
            //int veloc = Convert.ToInt16(velocity_sld.Value);
            new Thread(() => SendAndWaitForResponse("changeAngle0", "changedAngle","moveForward5")).Start();
        }
        private void velocity_sld_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            velocity = (int)velocity_sld.Value;
            SendAndWaitForResponse($"changeInter{velocity}", "changedInter", "");
        }

        private void height_sld_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            rideHeigth = (int)height_sld.Value * -1;
            //SendMessage($"moveN{rideHeigth}");
            //new Thread(() => SendMessage($"moveN{rideHeigth}")).Start();

            if (!heightThreadRunning)
            {
                heightThreadRunning = true;
                int heightCopy = rideHeigth;
                new Thread(() => { SendMessage($"moveN{heightCopy}"); Thread.Sleep(50); heightThreadRunning = false; }) { IsBackground = true }.Start();
            }
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

        private void move_backward_btn_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                From = 0,                  // Start position
                To = 490,                 // End position
                Duration = TimeSpan.FromSeconds(2), // Duration of animation
                EasingFunction = new QuadraticEase() // Optional: Add easing for smoothness
            };

            // Apply the animation to the Canvas.Left property
            batLevel_rec.BeginAnimation(Canvas.LeftProperty, animation);
        }
    }
}
