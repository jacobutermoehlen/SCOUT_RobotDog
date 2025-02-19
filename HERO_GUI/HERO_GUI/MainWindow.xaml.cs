using System;
using System.Collections.Generic;
using System.Globalization;
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
        double velocity = 50;
        int rideHeigth = 200;
        int angle = 0;
        int curveAngle = 55;

        private bool isHoldingW = false;
        private bool isHoldingA = false;
        private bool isHoldingD = false;

        private bool keyboardControl = false;

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

            this.PreviewKeyDown += moveConstantlyForwardStart;
            this.PreviewKeyUp += moveConstantlyForwardStop;
        }

        private void moveConstantlyForwardStart(Object sender, KeyEventArgs e)
        {
            if(keyboardControl)
            {
                if (e.Key == Key.W && !isHoldingW)
                {
                    isHoldingW = true;
                    if (angle == 0)
                    {
                        new Thread(() => SendMessage("moveCForward1")).Start();
                    }
                    else
                    {
                        new Thread(() => SendAndWaitForResponse("changeAngle0", "changedAngle", "moveCForward1")).Start();
                    }
                }
                else if (e.Key == Key.A && !isHoldingA)
                {
                    isHoldingA = true;

                    if (angle == 0)
                    {
                        angle = -curveAngle;
                        scout_rec.Fill = Brushes.Purple;
                        //new Thread(() => SendAndWaitForResponse("changeAngle15", "changedAngle", "test123456789"));
                        SendMessage($"changeAngle{angle}");
                    }
                }
                else if (e.Key == Key.D && !isHoldingD)
                {
                    isHoldingD = true;

                    if (angle == 0)
                    {
                        angle = curveAngle;
                        scout_rec.Fill = Brushes.Yellow;
                        //new Thread(() => SendAndWaitForResponse("changeAngle15", "changedAngle", "test123456789"));
                        SendMessage($"changeAngle{angle}");
                    }
                }
            }
        }

        private void moveConstantlyForwardStop(Object sender, KeyEventArgs e)
        {
            if (keyboardControl)
            {
                if (e.Key == Key.W)
                {
                    isHoldingW = false;
                    if (angle == 0)
                    {
                        new Thread(() => SendMessage("moveCForward0")).Start();
                        grid_main.Background = Brushes.White;
                    }
                    else
                    {
                        new Thread(() => SendAndWaitForResponse("changeAngle0", "changedAngle", "moveCForward0")).Start();
                    }
                }
                else if (e.Key == Key.A)
                {
                    isHoldingA = false;

                    if (angle == -curveAngle)
                    {

                        angle = 0;

                        SendMessage($"changeAngle{angle}");
                    }
                }
                else if (e.Key == Key.D)
                {
                    isHoldingD = false;

                    if (angle == curveAngle)
                    {

                        angle = 0;

                        SendMessage($"changeAngle{angle}");
                    }
                }
            }
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
            velocity = velocity_sld.Value;
            SendAndWaitForResponse($"changeInter{velocity.ToString(CultureInfo.InvariantCulture)}", "changedInter", "");
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
                //new Thread(() => { SendMessage($"moveN{heightCopy}"); Thread.Sleep(100); heightThreadRunning = false; }) { IsBackground = true }.Start();
                new Thread(() => { SendMessage($"changeRideHeight{heightCopy}"); Thread.Sleep(100); heightThreadRunning = false; }) { IsBackground = true }.Start();
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
            new Thread(() => SendMessage("stopMovement")).Start();
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

        private void move_forward_btn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            new Thread(() => SendAndWaitForResponse("changeAngle0", "changedAngle", "moveCForward1")).Start();
        }

        private void move_forward_btn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            new Thread(() => SendAndWaitForResponse("changeAngle0", "changedAngle", "moveCForward0")).Start();
        }

        private void keyboardToggle_ckb_Checked(object sender, RoutedEventArgs e)
        {
            keyboardControl = true;

            move_forward_btn.IsEnabled = false;
            move_left_btn.IsEnabled = false;
            move_right_btn.IsEnabled = false;
            move_backward_btn.IsEnabled = false;
        }

        private void keyboardToggle_ckb_Unchecked(object sender, RoutedEventArgs e)
        {
            keyboardControl = false;

            move_forward_btn.IsEnabled = true;
            move_left_btn.IsEnabled = true;
            move_right_btn.IsEnabled = true;
            move_backward_btn.IsEnabled = true;
        }
    }
}
